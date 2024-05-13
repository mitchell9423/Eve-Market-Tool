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
					string Expiration = webRequest.GetResponseHeader("expires");

					if (!string.IsNullOrEmpty(Expiration))
					{
						DateTime gmtDate = DateTime.Parse(Expiration, null, DateTimeStyles.AdjustToUniversal);
						Expiration = TimeZoneInfo.ConvertTimeFromUtc(gmtDate, TimeZoneInfo.Local).ToString();
					}

					if (webRequest.result == UnityWebRequest.Result.DataProcessingError || webRequest.result == UnityWebRequest.Result.ProtocolError)
					{
						Debug.LogError($"Data Processing Error: {webRequest.error}\n{url}\n{webRequest.GetRequestHeader("If-None-Match")}\n{webRequest.downloadHandler.text}");
						UnityMainThreadDispatcher.Instance?.Enqueue(() =>
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
						UnityMainThreadDispatcher.Instance?.Enqueue(() =>
						{
							callback(webRequest.responseCode, webRequest.GetResponseHeader("etag"), Expiration, null, region, 0);
						});
					}
					else
					{
						UnityMainThreadDispatcher.Instance?.Enqueue(() =>
						{
							callback(webRequest.responseCode, webRequest.GetResponseHeader("etag"), Expiration, webRequest.downloadHandler.text, region, type_id);
						});
					}

					tcs.SetResult(true);
				};
			}

			await tcs.Task;
		}

		// Call this to start the login process
		public void StartLoginProcess()
		{
			string scope = "publicData esi-calendar.respond_calendar_events.v1 esi-calendar.read_calendar_events.v1 esi-location.read_location.v1 esi-location.read_ship_type.v1 esi-mail.organize_mail.v1 esi-mail.read_mail.v1 esi-mail.send_mail.v1 esi-skills.read_skills.v1 esi-skills.read_skillqueue.v1 esi-wallet.read_character_wallet.v1 esi-wallet.read_corporation_wallet.v1 esi-search.search_structures.v1 esi-clones.read_clones.v1 esi-characters.read_contacts.v1 esi-universe.read_structures.v1 esi-bookmarks.read_character_bookmarks.v1 esi-killmails.read_killmails.v1 esi-corporations.read_corporation_membership.v1 esi-assets.read_assets.v1 esi-planets.manage_planets.v1 esi-fleets.read_fleet.v1 esi-fleets.write_fleet.v1 esi-ui.open_window.v1 esi-ui.write_waypoint.v1 esi-characters.write_contacts.v1 esi-fittings.read_fittings.v1 esi-fittings.write_fittings.v1 esi-markets.structure_markets.v1 esi-corporations.read_structures.v1 esi-characters.read_loyalty.v1 esi-characters.read_opportunities.v1 esi-characters.read_chat_channels.v1 esi-characters.read_medals.v1 esi-characters.read_standings.v1 esi-characters.read_agents_research.v1 esi-industry.read_character_jobs.v1 esi-markets.read_character_orders.v1 esi-characters.read_blueprints.v1 esi-characters.read_corporation_roles.v1 esi-location.read_online.v1 esi-contracts.read_character_contracts.v1 esi-clones.read_implants.v1 esi-characters.read_fatigue.v1 esi-killmails.read_corporation_killmails.v1 esi-corporations.track_members.v1 esi-wallet.read_corporation_wallets.v1 esi-characters.read_notifications.v1 esi-corporations.read_divisions.v1 esi-corporations.read_contacts.v1 esi-assets.read_corporation_assets.v1 esi-corporations.read_titles.v1 esi-corporations.read_blueprints.v1 esi-bookmarks.read_corporation_bookmarks.v1 esi-contracts.read_corporation_contracts.v1 esi-corporations.read_standings.v1 esi-corporations.read_starbases.v1 esi-industry.read_corporation_jobs.v1 esi-markets.read_corporation_orders.v1 esi-corporations.read_container_logs.v1 esi-industry.read_character_mining.v1 esi-industry.read_corporation_mining.v1 esi-planets.read_customs_offices.v1 esi-corporations.read_facilities.v1 esi-corporations.read_medals.v1 esi-characters.read_titles.v1 esi-alliances.read_contacts.v1 esi-characters.read_fw_stats.v1 esi-corporations.read_fw_stats.v1 esi-characterstats.read.v1";
			string responseType = "code";
			string url = $"{NetworkSettings.AUTHORIZATION_ENDPOINT}?response_type={responseType}&redirect_uri={Uri.EscapeDataString(NetworkSettings.CALLBACK_URL)}&client_id={NetworkSettings.CLIENTID}&scope={scope}";

			if (DateTime.Now > AppSettings.Settings.RefreshExpiration)
			{
				// Open this URL in an external browser
				Application.OpenURL(url);
			}
			else
			{
				RefreshToken();
			}
		}

		public bool IsExpired()
		{
			bool expired = true;

			if (DateTime.TryParse(StaticData.CorpOrderRecord.Expiration, out DateTime expiration))
			{
				expired = DateTime.Now >= expiration;
			}

			return expired;
		}

		IEnumerator RefreshTimer()
		{
			yield return new WaitForSeconds(float.Parse(AppSettings.Settings.TokenResponse.ExpiresIn) - 10);
			RefreshToken();
		}

		public void RefreshToken()
		{
			StartCoroutine(instance.GetAccessToken("refresh_token", "refresh_token", AppSettings.Settings.TokenResponse.RefreshToken));
		}

		// This should be triggered by your local web server setup or a manual method to enter the code
		public void OnAuthorizationCodeReceived(string code)
		{
			GetNewAccessToken(code);
		}

		void GetNewAccessToken(string code)
		{
			StartCoroutine(instance.GetAccessToken("authorization_code", "code", code));
		}

		private IEnumerator GetAccessToken(string grantType, string fieldName, string code)
		{
			string authHeader = Convert.ToBase64String(global::System.Text.Encoding.UTF8.GetBytes($"{NetworkSettings.CLIENTID}:{NetworkSettings.CLIENT_SECRET}"));
			WWWForm form = new WWWForm();
			form.AddField("grant_type", grantType);
			form.AddField(fieldName, code);

			UnityWebRequest www = UnityWebRequest.Post(NetworkSettings.TOKEN_ENDPOINT, form);
			www.SetRequestHeader("Authorization", $"Basic {authHeader}");
			www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

			yield return www.SendWebRequest();

			if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
			{
				Debug.LogError(www.error);
			}
			else
			{
				Debug.Log($"Token Response: {www.downloadHandler.text}");
				AppSettings.Settings.TokenResponse = JsonConvert.DeserializeObject<TokenResponse>(www.downloadHandler.text);

				if (Refresh_Co != null) StopCoroutine(Refresh_Co);
				Refresh_Co = StartCoroutine(RefreshTimer());

				AppSettings.Settings.RefreshExpiration = DateTime.Now + TimeSpan.Parse(AppSettings.Settings.TokenResponse.ExpiresIn);
				AppSettings.SaveAppSettings();

				StartCoroutine(UseAccessToken());
			}

		}

		private IEnumerator UseAccessToken()
		{
			Debug.Log($"Token Response: {AppSettings.Settings.TokenResponse.AccessToken}");
			_ = NetworkManager.AsyncRequest<CorpOrder>(extension: AppSettings.Settings.TokenResponse.AccessToken, type_id: AppSettings.Settings.CorpId, ETag: StaticData.CorpOrderRecord.ETag);

			yield return null;

			//UnityWebRequest www = UnityWebRequest.Get(NetworkSettings.VERIFICATION_ENDPOINT);
			//www.SetRequestHeader("Authorization", $"Bearer {token}");

			//yield return www.SendWebRequest();

			//if (www.result == (UnityWebRequest.Result.ConnectionError | UnityWebRequest.Result.ProtocolError))
			//{
			//	Debug.LogError(www.error);
			//}
			//else
			//{
			//	CharacterVerificationResponse response = JsonConvert.DeserializeObject<CharacterVerificationResponse>(www.downloadHandler.text);
			//	Debug.Log($"Logged in Character ID: {response.CharacterId}");
			//	// Further actions after successful login
			//}
		}
	}
}
