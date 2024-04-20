using EveMarket.Network;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace EveMarket.Util
{
	public class HttpHandler : MonoBehaviour
	{
		public static HttpHandler instance;

		private readonly HttpClient _httpClient;
		public HttpHandler()
		{
			_httpClient = new HttpClient();
			instance = this;
		}

		private List<UnityWebRequestAsyncOperation> asyncOps = new List<UnityWebRequestAsyncOperation>();
		public void ClearRequestList() => asyncOps.Clear();

		public IEnumerator Get<T>(string url, global::System.Action<string> callback)
		{
			using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
			{
				// Request and wait for the desired page.
				yield return webRequest.SendWebRequest();

				if (webRequest.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.Log($"Web request Error: {webRequest.error}");
					callback(null);
				}
				else
				{
					callback(webRequest.downloadHandler.text);
				}
			}
		}

		public void StopAllRequests()
		{
			lock (asyncOps)
			{
				foreach (var asyncOp in asyncOps)
				{
					if (!asyncOp.isDone)
					{
						asyncOp.webRequest.Abort();
						NetworkManager.CompleteNetworkTask();
					}
				}
			}
		}

		public async Task AsyncGetRequest<T>(string url, string tag, global::System.Action<long, string, string, string, Region, int> callback, Region region, int type_id = 0)
		{
			NetworkManager.Status = UpdateStatus.Updating;
			Interlocked.Increment(ref NetworkManager.totalRequests);
			Interlocked.Increment(ref NetworkManager.pendingRequests);
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

			UnityWebRequest webRequest = UnityWebRequest.Get(url);

			if (!string.IsNullOrEmpty(tag))
			{
				webRequest.SetRequestHeader("accept", "application/json");
				webRequest.SetRequestHeader("If-None-Match", tag);
				webRequest.SetRequestHeader("Cache-Control", "no-cache");
			}

			lock (asyncOps)
			{
				asyncOps.Add(webRequest.SendWebRequest());

				asyncOps.Last().completed += (AsyncOperation op) =>
				{
					if (webRequest.result == UnityWebRequest.Result.DataProcessingError)
					{
						Debug.LogError($"Data Processing Error: {webRequest.error}\n{url}\n{webRequest.GetRequestHeader("If-None-Match")}");
						UnityMainThreadDispatcher.Instance.Enqueue(() =>
						{
							callback(webRequest.responseCode, webRequest.GetResponseHeader("etag"), webRequest.GetResponseHeader("expires"), null, region, 0);
						});
					}
					else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
					{
						Debug.LogError($"Web Protocol Error: {webRequest.error}\n{url}");
						UnityMainThreadDispatcher.Instance.Enqueue(() =>
						{
							callback(webRequest.responseCode, webRequest.GetResponseHeader("etag"), webRequest.GetResponseHeader("expires"), null, region, 0);
						});
					}
					else if (webRequest.result == UnityWebRequest.Result.ConnectionError)
					{
						string itemName = type_id > 0 ? StaticData.ItemObjects[type_id].Name : "";
						Debug.LogError($"Web request Error: {webRequest.error}\n{url}[{itemName}]");
						UnityMainThreadDispatcher.Instance.Enqueue(() =>
						{
							callback(webRequest.responseCode, webRequest.GetResponseHeader("etag"), webRequest.GetResponseHeader("expires"), null, region, 0);
						});
					}
					else
					{
						UnityMainThreadDispatcher.Instance.Enqueue(() =>
						{
							callback(webRequest.responseCode, webRequest.GetResponseHeader("etag"), webRequest.GetResponseHeader("expires"), webRequest.downloadHandler.text, region, type_id);
						});
					}

					tcs.SetResult(true);
				};
			}

			await tcs.Task;
		}
	}
}
