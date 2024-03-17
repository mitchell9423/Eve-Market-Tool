
using EveMarket.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace EveMarket.Network
{
	public enum UpdateStatus
	{
		Idle,
		Updating
	}

	public static class NetworkManager
	{
		public static int pendingMarketGroups = 0;
		public static int pendingRequests = 0;
		public static int completedRequests = 0;
		public static int totalRequests = 0;

		public static UpdateStatus Status = UpdateStatus.Idle;

		static Dictionary<Type, string> ModelTypeToURI = new Dictionary<Type, string>()
		{
			{ typeof(List<MarketPrice>), NetworkSettings.MARKET_PRICES_URI},
			{ typeof(MarketGroup), NetworkSettings.MARKET_GROUP_URI},
			{ typeof(UniverseItem), NetworkSettings.UNIVERSE_TYPES_URI},
			{ typeof(List<MarketOrder>), NetworkSettings.ITEM_ORDERS_URI},
			{ typeof(List<int>), NetworkSettings.ROUTE_URI}
		};

		public static async Task AsyncRequest<T>(string extension = "", Region region = Region.The_Forge, int type_id = 0, RouteData data = new RouteData())
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

				await HttpHandler.instance.AsyncGetRequest<T>(url, StaticData.HandleResponse<T>, region, type_id);
			}
			catch (Exception ex)
			{
				Debug.LogError($"An error occurred: {ex.Message}");
			}
		}

		public static void CompleteGroupUpdate()
		{
			if (Interlocked.Decrement(ref pendingMarketGroups) == 0)
			{
				Debug.Log($"All market group(s) have completed.");
				EveDelegate.MarketUpdateComplete?.Invoke();
				ChangeUpdateStateToIdle();
			}
			else
			{
				Debug.Log($"{pendingMarketGroups} market group(s) pending.");
			}

		}

		public static void CompleteUpdateTask()
		{
			Interlocked.Increment(ref completedRequests);
			if (Interlocked.Decrement(ref pendingRequests) == 0 && pendingMarketGroups == 0)  // Decrement counter and check
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
