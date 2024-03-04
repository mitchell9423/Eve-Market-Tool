using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EveMarket.Network
{
	public static class NetworkSettings
	{
		public const string MARKET_PRICES_URI = "https://esi.evetech.net/latest/markets/prices/";
		public const string MARKET_GROUP_URI = "https://esi.evetech.net/latest/markets/groups/";
		public const string UNIVERSE_TYPES_URI = "https://esi.evetech.net/latest/universe/types/";
		public const string ITEM_ORDERS_URI = "https://esi.evetech.net/latest/markets/[region_id]/orders/?datasource=tranquility&order_type=all&page=1&type_id=[type_id]";
		public const string ITEM_HISTORY_URI = "https://esi.evetech.net/latest/markets/[region_id]/history/?datasource=tranquility&type_id=[type_id]";
		public const string ROUTE_URI = "https://esi.evetech.net/latest/route/[destination]/[origin]/?datasource=tranquility&flag=shortest";
	}
}
