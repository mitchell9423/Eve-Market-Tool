using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EveMarket
{
	public interface IDataModel
	{
		public int TypeId { get; set; }
	}

	public interface IObjectModel : IDataModel
	{
		public string Description { get; set; }
		public string Name { get; set; }
	}

	[Serializable]
	public class MarketGroupIds
	{
		List<int> Ids { get; set; } = new List<int>();
	}

	[Serializable]
	public class MarketGroup : IObjectModel
	{
		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("market_group_id")]
		public int TypeId { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("parent_group_id")]
		public int ParentGroupId { get; set; }

		[JsonProperty("types")]
		public List<int> Types { get; set; }
	}

	[Serializable]
	public class UniverseItem : IObjectModel
	{
		[JsonProperty("capacity")]
		public double Capacity { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		//[JsonProperty("dogma_attributes")]
		//public List<DogmaAttribute> DogmaAttributes { get; set; }

		[JsonProperty("group_id")]
		public int GroupId { get; set; }

		[JsonProperty("icon_id")]
		public int IconId { get; set; }

		[JsonProperty("market_group_id")]
		public int MarketGroupId { get; set; }

		[JsonProperty("mass")]
		public double MassDouble { get; set; }
		//public decimal Mass => Convert.ToDecimal(MassDouble);

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("packaged_volume")]
		public double PackagedVolume { get; set; }

		[JsonProperty("portion_size")]
		public int PortionSize { get; set; }

		[JsonProperty("published")]
		public bool Published { get; set; }

		[JsonProperty("radius")]
		public double Radius { get; set; }

		[JsonProperty("type_id")]
		public int TypeId { get; set; }

		[JsonProperty("volume")]
		public double Volume { get; set; }
	}

	[Serializable]
	public class MarketPrice : IDataModel
	{
		[JsonProperty("adjusted_price")]
		public double AdjustedPrice { get; set; }

		[JsonProperty("average_price")]
		public double AveragePrice { get; set; }

		[JsonProperty("type_id")]
		public Int32 TypeId { get; set; }
	}

	[Serializable]
	public class OrderRecord
	{
		public string Expiration;
		public string ETag;
		public int TypeId;

		public List<MarketOrder> marketOrders;

		public OrderRecord(int typeId, List<MarketOrder> marketOrders, string Expiration, string ETag)
		{
			TypeId = typeId;
			this.marketOrders = marketOrders ?? new List<MarketOrder>();
			this.Expiration = Expiration;
			this.ETag = ETag;
		}

		public void AddOrders(List<MarketOrder> marketOrders)
		{
			this.marketOrders = marketOrders;
		}
	}

	[Serializable]
	public class MarketOrder : IDataModel
	{
		[JsonProperty("duration")]
		public int Duration { get; set; }

		[JsonProperty("is_buy_order")]
		public bool IsBuyOrder { get; set; }

		[JsonProperty("issued")]
		public DateTime Issued { get; set; }

		[JsonProperty("location_id")]
		public Int64 LocationId { get; set; }

		[JsonProperty("min_volume")]
		public int MinVolume { get; set; }

		[JsonProperty("order_id")]
		public Int64 OrderId { get; set; }

		[JsonProperty("price")]
		public double Price { get; set; }

		[JsonProperty("range")]
		public string Range { get; set; }

		[JsonProperty("system_id")]
		public int SystemId { get; set; }

		[JsonProperty("type_id")]
		public int TypeId { get; set; }

		[JsonProperty("volume_remain")]
		public int VolumeRemain { get; set; }

		[JsonProperty("volume_total")]
		public int VolumeTotal { get; set; }
	}

	[Serializable]
	public class CorpOrderRecord
	{
		public string Expiration;
		public string ETag;
		public int TypeId;

		public List<CorpOrder> CorpOrders;

		public CorpOrderRecord(int typeId, List<CorpOrder> corpOrders, string expiration, string eTag)
		{
			TypeId = typeId;
			CorpOrders = corpOrders ?? new List<CorpOrder>();
			Expiration = expiration;
			ETag = eTag;
		}
	}

	[Serializable]
	public class CorpOrder : IDataModel
	{
		[JsonProperty("duration")]
		public int Duration { get; set; }

		[JsonProperty("escrow")]
		public double Escrow { get; set; }

		[JsonProperty("is_buy_order")]
		public bool IsBuyOrder { get; set; }

		[JsonProperty("issued")]
		public DateTime Issued { get; set; }

		[JsonProperty("issued_by")]
		public int IssuedBy { get; set; }

		[JsonProperty("location_id")]
		public Int64 LocationId { get; set; }

		[JsonProperty("min_volume")]
		public int MinVolume { get; set; }

		[JsonProperty("order_id")]
		public Int64 OrderId { get; set; }

		[JsonProperty("price")]
		public double Price { get; set; }

		[JsonProperty("range")]
		public string Range { get; set; }

		[JsonProperty("region_id")]
		public int RegionId { get; set; }

		[JsonProperty("type_id")]
		public int TypeId { get; set; }

		[JsonProperty("volume_remain")]
		public int VolumeRemain { get; set; }

		[JsonProperty("volume_total")]
		public int VolumeTotal { get; set; }

		[JsonProperty("wallet_division")]
		public int WalletDivision { get; set; }
	}

	[Serializable]
	public class CharacterVerificationResponse
	{
		[JsonProperty("CharacterID")]
		public long CharacterID { get; set; }

		[JsonProperty("CharacterName")]
		public string CharacterName { get; set; }

		[JsonProperty("ExpiresOn")]
		public string ExpiresOn { get; set; }

		[JsonProperty("Scopes")]
		public string Scopes { get; set; }

		[JsonProperty("TokenType")]
		public string TokenType { get; set; }

		[JsonProperty("CharacterOwnerHash")]
		public string CharacterOwnerHash { get; set; }

		[JsonProperty("IntellectualProperty")]
		public string IntellectualProperty { get; set; }
	}

	[Serializable]
	public class TokenResponse
	{
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		[JsonProperty("expires_in")]
		public string ExpiresIn { get; set; }

		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; }

		[JsonProperty("scope")]
		public string Scope { get; set; }
	}

	[Serializable]
	public class VerifyResponse
	{
		public string CharacterName { get; set; }
		public long CharacterID { get; set; }
		public string ExpiresOn { get; set; }
		public string Scopes { get; set; }
		public string TokenType { get; set; }
		public string CharacterOwnerHash { get; set; }
	}

	[Serializable]
	public class DogmaAttribute
	{
		[JsonProperty("attribute_id")]
		public int AttributeId { get; set; }

		[JsonProperty("value")]
		public double Value { get; set; }
	}

	[Serializable]
	public class RouteInfo
	{
		public List<int> Route { get; set; } = new List<int>();
	}
}
