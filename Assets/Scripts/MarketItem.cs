using EveMarket;
using EveMarket.Network;
using EveMarket.Util;
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
		UniverseItem item;
		MarketPrice price;
		Dictionary<Region, Dictionary<int, OrderRecord>> orders;

		public MarketItem(UniverseItem _item, MarketPrice _price, Dictionary<Region, Dictionary<int, OrderRecord>> orders)
		{
			item = _item;
			price = _price;
			this.orders = orders;
			SetReprocessType();
			SetCurrentSellPrice();
			SetMaxBuy();
			SetCurrentBuyPrice();
		}

		public int TypeId { get => item.TypeId; }
		public int GroupId { get => item.MarketGroupId; }
		public string ItemName { get => item.Name; }
		public double AveragePrice { get => price.AveragePrice; }
		public double CurrentSellPrice { get; set; }
		public double CurrentBuyPrice { get; set; }
		public double MaxBuyPrice { get; set; }
		public double CompressedPrice { get; set; }
		public double ReprocessPrice { get; set; }
		public ReprocessType ReprocessType { get; private set; } = ReprocessType.None;

		private Color textColor;

		private double SetReprocessPrice()
		{
			return ReprocessPrice = Math.Round(Reprocess.CalcReprocessedValue(this), 2);
		}

		private double SetMaxBuy()
		{
			return MaxBuyPrice = (double)Math.Round(SetReprocessPrice() * ((100 - (double)AppSettings.MarginPercentage) / 100), 2);
		}

		private void SetCurrentSellPrice()
		{
			if (orders.ContainsKey(AppSettings.SellRegion) && orders[AppSettings.SellRegion][TypeId].marketOrders != null && orders[AppSettings.SellRegion][TypeId].marketOrders.Count() > 0)
			{
				List<MarketOrder> sellOrders = orders[AppSettings.SellRegion][TypeId].marketOrders.Where(rec =>
				!rec.IsBuyOrder
				&& rec.LocationId == 60003760
				&& (rec.SystemId == 30000142 || rec.SystemId == 30000144)
				).ToList();

				double lowestPice = 0;

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

				CurrentSellPrice = lowestPice - 0.01;
			}
		}

		private void SetCurrentBuyPrice()
		{
			double highestPice = 0.00d;

			if (orders.ContainsKey(AppSettings.BuyRegion) && orders[AppSettings.BuyRegion][TypeId].marketOrders != null)
			{
				List<MarketOrder> marketOrders = orders[AppSettings.BuyRegion][TypeId].marketOrders;
				var buyOrders = marketOrders.FindAll(rec => 
				rec.IsBuyOrder
				&& rec.Price <= MaxBuyPrice
				//&& !(rec.Range == StaticData.RangeStringName[Range.Region] || rec.Range == StaticData.RangeStringName[Range.Jump_40])
				&& !((rec.LocationId == 60005143 && rec.Range == "1") || (rec.LocationId == 60003826 && rec.Range == "4") || (rec.LocationId == 60000469 && rec.Range == "3") || (rec.LocationId == 60002263 && rec.Range == "3"))
				);

				if (buyOrders != null)
				{
					foreach (var order in buyOrders)
					{
						int distance = 0;

						if (StaticData.Routes != null)
						{
							lock (StaticData.Routes)
							{
								if (StaticData.SystemIds.ContainsKey(AppSettings.BuyOrderSystem) && StaticData.Routes.TryGetValue(StaticData.SystemIds[AppSettings.BuyOrderSystem], out List<RouteData> routes))
								{
									if (StaticData.RangeStringToInt.ContainsKey(AppSettings.BuyRange))
									{
										distance = routes.Find(route => route.Destination == order.SystemId).NumJumps - StaticData.RangeStringToInt[AppSettings.BuyRange];
									}
								}
							}
						}

						if (StaticData.RangeStringToInt[order.Range] >= distance && order.Price > highestPice)
							highestPice = order.Price;
					}
				}
			}

			if (highestPice > 0)
			{
				if (ReprocessType == ReprocessType.Ice)
				{
					highestPice += 100;
				}
				else
				{
					highestPice += 0.01;
				}
			}

			CurrentBuyPrice =  highestPice;
		}

		public void UpdateOrders()
		{
			var region = AppSettings.SellRegion;
			orders[region][TypeId] = new OrderRecord(StaticData.OrderRecords[region][TypeId].marketOrders);
			SetCurrentSellPrice();

			region = AppSettings.BuyRegion;
			if (orders.ContainsKey(region) && orders[region].ContainsKey(TypeId))
			{
				orders[region][TypeId] = new OrderRecord(StaticData.OrderRecords[region][TypeId].marketOrders);
			}
			SetMaxBuy();
			SetCurrentBuyPrice();
		}

		public void ClearOrders(int TypeId)
		{
			orders[AppSettings.SellRegion][TypeId].marketOrders.Clear();
		}

		private void SetReprocessType()
		{
			switch (StaticData.GroupObjects[GroupId].Name)
			{
				case "Bezdnacine":
				case "Rakovene":
				case "Talassonite":
					ReprocessType = ReprocessType.Abyssal;
					break;
				case "Hedbergite":
				case "Hemorphite":
				case "Jaspet":
				case "Kernite":
				case "Omber":
				case "Ytirium":
					ReprocessType = ReprocessType.Coherent;
					break;
				case "Cobaltite":
				case "Euxenite":
				case "Titanite":
				case "Scheelite":
					ReprocessType = ReprocessType.Common;
					break;
				case "Arkonor":
				case "Bistot":
				case "Spodumain":
				case "Eifyrium":
				case "Ducinium":
					ReprocessType = ReprocessType.Complex;
					break;
				case "Xenotime":
				case "Monazite":
				case "Loparite":
				case "Ytterbite":
					ReprocessType = ReprocessType.Exceptional;
					break;
				case "Ice Ores":
					ReprocessType = ReprocessType.Ice;
					break;
				case "Mercoxite":
					ReprocessType = ReprocessType.Mercoxit;
					break;
				case "Carnotite":
				case "Zircon":
				case "Pollucite":
				case "Cinnabar":
					ReprocessType = ReprocessType.Rare;
					break;
				case "Plagioclase":
				case "Pyroxeres":
				case "Scordite":
				case "Veldspar":
				case "Mordunium":
					ReprocessType = ReprocessType.Simple;
					break;
				case "Zeolites":
				case "Sylvite":
				case "Bitumens":
				case "Coesite":
					ReprocessType = ReprocessType.Ubiquitous;
					break;
				case "Otavite":
				case "Sperrylite":
				case "Vanadinite":
				case "Chromite":
					ReprocessType = ReprocessType.Uncommon;
					break;
				case "Crokite":
				case "Dark Ochre":
				case "Gneiss":
					ReprocessType = ReprocessType.Variegated;
					break;
				default:
					break;
			}
		}
	}
}
