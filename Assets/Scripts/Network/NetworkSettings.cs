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
		public const string CORP_ORDERS_URI = "https://esi.evetech.net/latest/corporations/[type_id]/orders/?datasource=tranquility&page=1&token=";

		public const string CLIENTID = "4d1c1598cf604721822990f7aed8a23a";
		public static string CLIENT_SECRET = "BneNYjHUq6luK6HrgjRj2AvMjomFYeeDupkN8Uot";
		public const string CALLBACK_URL = "http://localhost:8080/oauth-callback"; // This must match with the registered callback URL
		public const string AUTHORIZATION_ENDPOINT = "https://login.eveonline.com/oauth/authorize";
		public const string TOKEN_ENDPOINT = "https://login.eveonline.com/oauth/token";
		public const string VERIFICATION_ENDPOINT = "https://login.eveonline.com/oauth/verify";
	}
}
