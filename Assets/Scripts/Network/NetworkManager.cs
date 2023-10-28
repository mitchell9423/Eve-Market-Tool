
using EveMarket.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace EveMarket.Network
{
	public static class NetworkManager
	{
		static Dictionary<Type, string> ModelTypeToURI = new Dictionary<Type, string>()
		{
			{ typeof(List<MarketPrice>), NetworkSettings.MARKET_PRICES_URI},
			{ typeof(MarketGroup), NetworkSettings.MARKET_GROUP_URI},
			{ typeof(UniverseItem), NetworkSettings.UNIVERSE_TYPES_URI}
		};

		public static void AsyncRequest<T>(string extension = "")
		{
			try
			{
				string baseUri;
				lock (ModelTypeToURI)
				{
					baseUri = ModelTypeToURI[typeof(T)];
				}

				string url = baseUri + extension;
				_ = HttpHandler.instance.AsyncGetRequest<T>(url, StaticData.HandleResponse<T>);
			}
			catch (Exception ex)
			{
				Debug.LogError($"An error occurred: {ex.Message}");
			}
		}

		public static void JobsComplete()
		{
			EveDelegate.StaticUpdateComplete -= JobsComplete;
		}
	}
}
