using EveMarket.Util;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace EveMarket.Network
{
	public class HttpHandler : MonoBehaviour
	{
		public int codeRecievedCounter;
		public static HttpHandler instance;

		private readonly HttpClient _httpClient;
		public HttpHandler()
		{
			_httpClient = new HttpClient();
			instance = this;
		}

		private Coroutine Refresh_Co;
		private List<UnityWebRequestAsyncOperation> asyncOps = new List<UnityWebRequestAsyncOperation>();

		public void ClearRequestList() => asyncOps.Clear();

		public IEnumerator Get<T>(string url, Action<string> callback)
		{
			using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
			{
				// Request and wait for the desired page.
				yield return webRequest.SendWebRequest();

				if (webRequest.result == UnityWebRequest.Result.ConnectionError)
				{
					UnityEngine.Debug.Log($"Web request Error: {webRequest.error}");
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
					string Expiration = webRequest.GetResponseHeader("expires");

					if (!string.IsNullOrEmpty(Expiration))
					{
						DateTime gmtDate = DateTime.Parse(Expiration, null, DateTimeStyles.AdjustToUniversal);
						Expiration = TimeZoneInfo.ConvertTimeFromUtc(gmtDate, TimeZoneInfo.Local).ToString();
					}

					if (webRequest.result == UnityWebRequest.Result.DataProcessingError || webRequest.result == UnityWebRequest.Result.ProtocolError)
					{
						// look into: https://esi.evetech.net/latest/markets/10000002/orders/?datasource=tranquility&order_type=all&page=1&type_id=60771
						Debug.LogError($"Data Processing Error: {webRequest.error}\n{url}\n{webRequest.GetRequestHeader("If-None-Match")}\n{webRequest.downloadHandler.text}");
						UnityMainThreadDispatcher.Enqueue(() =>
						{
							callback(webRequest.responseCode, webRequest.GetResponseHeader("etag"), Expiration, null, region, 0);
						});
					}
					else if (webRequest.result == UnityWebRequest.Result.ConnectionError)
					{
						string objectName = "Unknown";
						if (StaticData.UniverseItems.TryGetValue(type_id, out UniverseItem item))
						{
							objectName = item.Name;
						}
						else if (StaticData.MarketGroups.TryGetValue(type_id, out MarketGroup group))
						{
							objectName = group.Name;
						}

						Debug.LogError($"Web request Error: {webRequest.error}\n{url}[{objectName}]\n{webRequest.downloadHandler.text}");
						UnityMainThreadDispatcher.Enqueue(() =>
						{
							callback(webRequest.responseCode, webRequest.GetResponseHeader("etag"), Expiration, null, region, 0);
						});
					}
					else
					{
						UnityMainThreadDispatcher.Enqueue(() =>
						{
							callback(webRequest.responseCode, webRequest.GetResponseHeader("etag"), Expiration, webRequest.downloadHandler.text, region, type_id);
						});
					}

					tcs.SetResult(true);
				};
			}

			await tcs.Task;
		}

		public bool IsExpired(string expirationDate)
		{
			bool expired = true;

			if (DateTime.TryParse(expirationDate, out DateTime expiration))
			{
				return IsExpired(expiration);
			}

			return expired;
		}

		public bool IsExpired(DateTime expiration)
		{
			return DateTime.Now >= expiration;
		}

		IEnumerator RefreshTimer()
		{
			yield return new WaitForSeconds(float.Parse(AppSettings.Settings.TokenResponse.ExpiresIn) - 10);
			RefreshToken();
		}

		public void RefreshToken()
		{
			//StartCoroutine(instance.GetAccessToken("refresh_token", "refresh_token", AppSettings.Settings.TokenResponse.RefreshToken));
		}

		private IEnumerator UseAccessToken()
		{
			UnityEngine.Debug.Log($"Token Response: {AppSettings.Settings.TokenResponse.AccessToken}");

			// Fetch ESI data
			_ = NetworkManager.AsyncRequest<CorpOrder>(
				extension: AppSettings.Settings.TokenResponse.AccessToken,
				type_id: AppSettings.Settings.CorpId,
				ETag: StaticData.CorpOrderRecord.ETag
			);

			// 🔐 Verify Token Info
			yield return StartCoroutine(VerifyAccessToken());
		}

		public async Task<bool> VerifyToken()
		{

			if (IsExpired(AppSettings.Settings.AccessTokenExpiresAt))
				return false;

			using (UnityWebRequest www = UnityWebRequest.Get(NetworkSettings.VERIFICATION_ENDPOINT))
			{
				www.SetRequestHeader("Authorization", $"Bearer {AppSettings.Settings.TokenResponse.AccessToken}");

				var operation = www.SendWebRequest();

				while (!operation.isDone)
					await Task.Yield();

				if (www.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError($"❌ Token verification failed: {www.responseCode} - {www.error}");
					return false;
				}

				Debug.Log("✅ Token verified.");
				return true;
			}
		}

		public IEnumerator VerifyAccessToken()
		{
			string token = AppSettings.Settings.TokenResponse.AccessToken;

			UnityWebRequest www = UnityWebRequest.Get(NetworkSettings.VERIFICATION_ENDPOINT);
			www.SetRequestHeader("Authorization", $"Bearer {token}");

			yield return www.SendWebRequest();

			if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
			{
				UnityEngine.Debug.LogError($"❌ Token request failed:\nStatus Code: {www.responseCode}\nError: {www.error}\nBody: {www.downloadHandler.text}");
				//UnityEngine.Debug.LogError(www.error);
			}
			else
			{
				CharacterVerificationResponse response = JsonConvert.DeserializeObject<CharacterVerificationResponse>(www.downloadHandler.text);
				UnityEngine.Debug.Log($"Logged in Character: {response.CharacterName} (ID: {response.CharacterID})");

				// Optional: store response in settings for future use
				AppSettings.Settings.CharacterInfo = response;
				AppSettings.SaveAppSettings();

				UnityEngine.Debug.Log($"Welcome, Capsuleer: {AppSettings.Settings.CharacterInfo.CharacterName}");
			}
		}
	}
}
