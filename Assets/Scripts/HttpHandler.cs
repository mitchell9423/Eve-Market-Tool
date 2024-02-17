using EveMarket.Network;
using EveMarket.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace EveMarket
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

		public IEnumerator Get<T>(string url, System.Action<string> callback)
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
						NetworkManager.CompleteStaticUpdateTask();
					}
				}
			}
		}

		public async Task AsyncGetRequest<T>(string url, System.Action<string, Region, int> callback, Region region, int type_id = 0)
		{
			NetworkManager.Status = UpdateStatus.Updating;
			Interlocked.Increment(ref NetworkManager.totalRequests);
			Interlocked.Increment(ref NetworkManager.pendingRequests);
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

			UnityWebRequest webRequest = UnityWebRequest.Get(url);

			lock (asyncOps)
			{
				asyncOps.Add(webRequest.SendWebRequest());

				asyncOps.Last().completed += (AsyncOperation op) =>
				{
					if (webRequest.result == UnityWebRequest.Result.ProtocolError)
					{
						Debug.LogError($"Web Protocol Error: {webRequest.error}\n{url}");
						UnityMainThreadDispatcher.Instance.Enqueue(() =>
						{
							callback(null, region, 0);
						});
					}
					else if (webRequest.result == UnityWebRequest.Result.ConnectionError)
					{
						Debug.LogError($"Web request Error: {webRequest.error}\n{url}[{StaticData.ItemObjects[type_id].Name}]");
						UnityMainThreadDispatcher.Instance.Enqueue(() =>
						{
							callback(null, region, 0);
						});
					}
					else
					{
						UnityMainThreadDispatcher.Instance.Enqueue(() =>
						{
							callback(webRequest.downloadHandler.text, region, type_id);
						});
					}

					tcs.SetResult(true);
				};
			}

			await tcs.Task;
		}
	}
}
