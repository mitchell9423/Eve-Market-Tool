
using EveMarket.Util;
using System;
using System.Collections.Generic;

namespace EveMarket.Network
{
	public static class NetworkManager
	{
		static Dictionary<Type, string> ModelTypeToURL = new Dictionary<Type, string>()
		{
			{ typeof(MarketPrice), NetworkSettings.MARKET_PRICES_URI},
			{ typeof(MarketGroup), NetworkSettings.MARKET_GROUP_URI},
			{ typeof(UniverseItem), NetworkSettings.UNIVERSE_TYPES_URI}
		};

		public static string GetURL<T>(string extension = "")
		{
			return $"{ModelTypeToURL[typeof(T)]}{extension}";
		}

		public static void AsyncRequest<T>(string extension = "") where T : IDataModel
		{
			string url = GetURL<T>(extension);
			_ = HttpHandler.instance.AsyncGetRequest<T>(url, StaticData.HandleResponse<T>);
		}

		public static void JobsComplete()
		{
			EveDelegate.StaticUpdateComplete -= JobsComplete;
		}
	}
}
