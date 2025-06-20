
using EveMarket.StateMachine;
using EveMarket.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace EveMarket.Network
{
	public enum UpdateStatus
	{
		Idle,
		Updating
	}

	public static class NetworkManager
	{
		// Dev code
		public static bool EnableLogin { get; set; } = true;

		public static int pendingMarketGroups = 0;
		public static int totalMarketGroups = 0;
		public static int pendingMarketRequests = 0;
		public static int pendingRequests = 0;
		public static int completedRequests = 0;
		public static int totalRequests = 0;

		public static UpdateStatus Status = UpdateStatus.Idle;

		static Dictionary<Type, string> ModelTypeToURI = new Dictionary<Type, string>()
		{
			{ typeof(CorpOrder), NetworkSettings.CORP_ORDERS_URI},
			{ typeof(List<MarketPrice>), NetworkSettings.MARKET_PRICES_URI},
			{ typeof(MarketGroup), NetworkSettings.MARKET_GROUP_URI},
			{ typeof(UniverseItem), NetworkSettings.UNIVERSE_TYPES_URI},
			{ typeof(List<MarketOrder>), NetworkSettings.ITEM_ORDERS_URI},
			{ typeof(List<int>), NetworkSettings.ROUTE_URI}
		};

		public static async Task AsyncRequest<T>(string extension = "", Region region = Region.The_Forge, int type_id = 0, RouteData data = new RouteData(), string ETag = "") where T : class
		{
			try
			{
				string baseUri;

				lock (ModelTypeToURI) { baseUri = ModelTypeToURI[typeof(T)]; }

				baseUri = baseUri.Replace("[region_id]", StaticData.RegionId[region].ToString());
				baseUri = baseUri.Replace("[type_id]", type_id.ToString());
				baseUri = baseUri.Replace("[destination]", data.Destination.ToString());
				baseUri = baseUri.Replace("[origin]", data.Origin.ToString());

				string url = baseUri + extension;

				await HttpHandler.instance.AsyncGetRequest<T>(url, ETag, StaticData.HandleResponse<T>, region, type_id);
			}
			catch (Exception ex)
			{
				string typeLog = $"type: {typeof(T)}";
				string extensionLog = string.IsNullOrEmpty(extension) ? "" : $"extension: {extension}";
				string regionLog = $"region: {region}";
				string idLog = $"type_id: {type_id}";

				Debug.LogError($"A request error occurred...\n" +
                    $"{typeLog}  {extensionLog}  {regionLog}  {idLog}\n" +
                    $"{ex.Message}");
			}
		}

		public static void CompleteGroupUpdate(int groupRequestId)
		{
			//Debug.Log($"Market group request {groupRequestId} complete!");

			if (Interlocked.Decrement(ref pendingMarketGroups) <= 0)
			{
				pendingMarketGroups = 0;
				totalMarketGroups = 0;
			}
		}

		public static void CompleteMarketUpdateTask()
		{
			if (Interlocked.Decrement(ref pendingMarketRequests) <= 0)
			{
				//EveDelegate.ResetAutoUpdateTimer?.Invoke();
				EveStateMachine.SetNextState(new SaveMarketData(), AppState.SaveMarketData);
				pendingMarketRequests = 0;
			}

			//Debug.Log($"{pendingMarketRequests} Market Requests pending!");
		}

		public static void CompleteNetworkTask()
		{
			Interlocked.Increment(ref completedRequests);
			if (Interlocked.Decrement(ref pendingRequests) <= 0)  // Decrement counter and check
			{
				EveDelegate.StaticUpdateComplete?.Invoke();
				ChangeUpdateStateToIdle();
			}
		}

		public static void JobsComplete()
		{
			EveDelegate.Unsubscribe(ref EveDelegate.StaticUpdateComplete, JobsComplete);
			StaticData.IsSubscribed = false;
		}

		public static void ChangeUpdateStateToIdle()
		{
			HttpHandler.instance.ClearRequestList();
			completedRequests = 0;
			totalRequests = 0;
			Status = UpdateStatus.Idle;
		}
	}
}
