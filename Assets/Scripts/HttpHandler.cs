using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
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
		}

		void Start()
		{
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

		public Task GetAsync<T>(string url, System.Action<string> callback)
		{
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
					callback(webRequest.downloadHandler.text);
				}
				tcs.SetResult(true);
			};

			return tcs.Task;
		}

		//private IEnumerator FetchData(List<object> itemGroupIdsToNames)
		//{
		//	foreach (var groupObject in itemGroupIdsToNames)
		//	{
		//		string groupId = groupObject.id; // Replace with actual field
		//		string groupName = groupObject.name; // Replace with actual field

		//		yield return FetchJson($"{marketGroupsURL}{groupId}", groupJson =>
		//		{
		//			if (groupJson != null)
		//			{
		//				// Deserialize groupJson to get types
		//				List<string> types = new List<string>(); // Replace with actual deserialization

		//				foreach (var typeId in types)
		//				{
		//					StartCoroutine(FetchJson($"{universeTypesURL}{typeId}", itemJson =>
		//					{
		//						if (itemJson != null)
		//						{
		//							// Deserialize itemJson to get item details
		//							// ...

		//							if (!mappedItems.ContainsKey(groupName))
		//							{
		//								mappedItems[groupName] = new Dictionary<string, object>();
		//							}

		//							mappedItems[groupName][itemJson.name] = new
		//							{
		//								name = itemJson.name,
		//								id = itemJson.type_id,
		//								group_name = groupName,
		//								group_id = itemJson.group_id
		//							};
		//						}
		//					}));
		//				}
		//			}
		//		});
		//	}
		//}
	}
}
