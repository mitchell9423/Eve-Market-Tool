using EveMarket;
using EveMarket.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;


namespace EveMarket
{
	[Serializable]
	public class MarketItem
	{
		float marginPercentage = .20f;
		UniverseItem item;
		MarketPrice price;
		Dictionary<Region, Dictionary<int, OrderRecord>> orders;

		public MarketItem(UniverseItem _item, MarketPrice _price, Dictionary<Region, Dictionary<int, OrderRecord>> orders)
		{
			item = _item;
			price = _price;
			this.orders = orders;
			CurrentSellPrice = GetCurrentSellPrice();
			CurrentBuyPrice = GetCurrentBuyPrice();
			SetMarginStatus();
		}

		public int TypeId { get => item.TypeId; }
		public string ItemName { get => item.Name; }
		public double AveragePrice { get => price.AveragePrice; }
		public double MarginPercentage { get => marginPercentage; }
		public double CurrentSellPrice { get; set; }
		public double CurrentBuyPrice { get; set; }
		public double CompressedPrice { get; set; }
		public MarginStatus MarginStatus { get; set; }

		private Color textColor;

		public Color GetTextColor(Color defaultColor)
		{
			if (MarginStatus == MarginStatus.Low)
			{
				return Color.red;
			}
			else if (MarginStatus == MarginStatus.High)
			{
				return Color.green;
			}
			else { return defaultColor; }
		}

		public void SetMarginStatus()
		{
			if (CurrentSellPrice < CurrentBuyPrice)
			{
				MarginStatus = MarginStatus.Low;
			}
            else if (CurrentSellPrice > CurrentBuyPrice * (1 + marginPercentage))
			{
				MarginStatus = MarginStatus.High;
			}
			else { MarginStatus = MarginStatus.Normal; }
        }

		private double GetCurrentSellPrice()
		{
			if (!orders.ContainsKey(NetworkSettings.SellRegion) || orders[NetworkSettings.SellRegion][TypeId].marketOrders == null) return 0;

			var sellOrders = orders[NetworkSettings.SellRegion][TypeId].marketOrders.Where(rec => !rec.IsBuyOrder && (rec.SystemId == 30000142 || rec.SystemId == 30000144));
			double lowestPice = 0.00d;

			if (sellOrders != null && sellOrders.Count() > 0)
			{
				lowestPice = sellOrders.First().Price;
				foreach (var order in sellOrders)
				{
					if (order.Price < lowestPice)
					{
						lowestPice = order.Price;
					}
				}
			}

			return lowestPice;
		}

		private double GetCurrentBuyPrice()
		{
			if (!orders.ContainsKey(NetworkSettings.BuyRegion) || orders[NetworkSettings.BuyRegion][TypeId].marketOrders == null) return 0;

			var buyOrders = orders[NetworkSettings.BuyRegion][TypeId].marketOrders.Where(rec => rec.IsBuyOrder);
			double highestPice = 0.00d;

			if (buyOrders != null && buyOrders.Count() > 0)
			{
				foreach (var order in buyOrders)
				{
					if (order.Price > highestPice)
					{
						highestPice = order.Price;
					}
				}
			}

			return highestPice;
		}

		public void UpdateOrders()
		{
			var region = NetworkSettings.BuyRegion;
			orders[region][TypeId] = new OrderRecord(StaticData.OrderRecords[region][TypeId].marketOrders);
			CurrentBuyPrice = GetCurrentBuyPrice();

			region = NetworkSettings.SellRegion;
			orders[region][TypeId] = new OrderRecord(StaticData.OrderRecords[region][TypeId].marketOrders);
			CurrentSellPrice = GetCurrentSellPrice();
		}

		public void ClearOrders(int TypeId)
		{
			orders[NetworkSettings.SellRegion][TypeId].marketOrders.Clear();
		}
	}
}
