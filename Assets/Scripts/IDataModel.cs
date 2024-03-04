using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		[JsonProperty("dogma_attributes")]
		public List<DogmaAttribute> DogmaAttributes { get; set; }

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
		public List<MarketOrder> marketOrders = new List<MarketOrder>();

		public OrderRecord(List<MarketOrder> marketOrders)
		{
			this.marketOrders = marketOrders;
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
