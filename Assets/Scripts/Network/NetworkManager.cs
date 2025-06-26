
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

		public static async Task AsyncRequest<T>(EveMarketRequest eveMarketRequest) where T : class
		{
			try
			{
				string baseUri;

				lock (ModelTypeToURI) { baseUri = ModelTypeToURI[typeof(T)]; }

				baseUri = baseUri.Replace("[region_id]", StaticData.RegionId[eveMarketRequest.Region].ToString());
				baseUri = baseUri.Replace("[type_id]", eveMarketRequest.Type_id.ToString());
				baseUri = baseUri.Replace("[destination]", eveMarketRequest.Data.Destination.ToString());
				baseUri = baseUri.Replace("[origin]", eveMarketRequest.Data.Origin.ToString());

				eveMarketRequest.Url = baseUri + eveMarketRequest.Extension;
				eveMarketRequest.Callback = StaticData.HandleResponse<T>;

				if (typeof(T) == typeof(UniverseItem))
                {
					Debug.LogWarning($"Requesting {typeof(T)}\nURL: {eveMarketRequest.Url}");
                }
				await HttpHandler.instance.AsyncGetRequest<T>(eveMarketRequest);
			}
			catch (Exception ex)
			{
				string typeLog = $"type: {typeof(T)}";
				string extensionLog = string.IsNullOrEmpty(eveMarketRequest.Extension) ? "" : $"extension: {eveMarketRequest.Extension}";
				string regionLog = $"region: {eveMarketRequest.Region}";
				string idLog = $"type_id: {eveMarketRequest.Type_id}";

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
