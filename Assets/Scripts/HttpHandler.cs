using EveMarket.Util;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
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

		private static int pendingRequests = 0;
		private static int completedRequests = 0;
		private static int totalRequests = 0;

		private readonly HttpClient _httpClient;

		public HttpHandler()
		{
			_httpClient = new HttpClient();
			instance = this;
		}

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

		public async Task AsyncGetRequest<T>(string url, System.Action<string> callback)
		{
			Interlocked.Increment(ref totalRequests);
			Interlocked.Increment(ref pendingRequests);
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

			UnityWebRequest webRequest = UnityWebRequest.Get(url);
			UnityWebRequestAsyncOperation asyncOp = webRequest.SendWebRequest();

			asyncOp.completed += (AsyncOperation op) =>
			{
				if (webRequest.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.Log($"Web request Error: {webRequest.error}");
					callback(null);
				}
				else
				{

					UnityMainThreadDispatcher.Instance.Enqueue(() =>
					{
						callback(webRequest.downloadHandler.text);
					});
				}
				tcs.SetResult(true);
			};

			await tcs.Task;
		}

		public static void CompleteStaticUpdateTask()
		{
			Interlocked.Increment(ref completedRequests);
			if (Interlocked.Decrement(ref pendingRequests) == 0)  // Decrement counter and check
			{
				completedRequests = 0;
				totalRequests = 0;

				EveDelegate.StaticUpdateComplete?.Invoke();
			}

			Debug.Log($"Completed {completedRequests} requests with {pendingRequests} of {totalRequests} requests pending.");
		}
	}
}
