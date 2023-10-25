using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace EveMarket
{
	public static class NetworkManager
	{

		const string MARKET_GROUP_URI = "https://esi.evetech.net/latest/markets/groups/";
		const string UNIVERSE_ITEMS_URI = "https://esi.evetech.net/latest/universe/types/";


		public static string GetGroupInfoURL(int id)
		{
			return $"{MARKET_GROUP_URI}{id}";
		}

		public static string GetItemInfoURL(int id)
		{
			return $"{UNIVERSE_ITEMS_URI}{id}";
		}

		public static async Task RequestGroupInfoAsync(int id)
		{
			string url = GetGroupInfoURL(id);
			await HttpHandler.instance.GetAsync<MarketGroup>(url, StaticData.HandleResponse<MarketGroup>);
		}

		public static async Task RequestItemInfoAsync(int id)
		{
			string url = GetItemInfoURL(id);
			await HttpHandler.instance.GetAsync<UniverseItem>(url, StaticData.HandleResponse<UniverseItem>);
		}

		public static void JobsComplete()
		{
			HttpHandler.StatusComplete -= JobsComplete;
		}
	}
}
