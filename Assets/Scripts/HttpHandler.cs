using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace EveMarket
{
	public class HttpHandler : MonoBehaviour
	{
		private readonly HttpClient _httpClient;

		public HttpHandler()
		{
			_httpClient = new HttpClient();
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
	}
}
