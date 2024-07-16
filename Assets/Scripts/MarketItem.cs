
using EveMarket.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EveMarket
{
	[Serializable]
	public class MarketItem : IComparer<MarketItem>
	{
		public enum EItemStatus
		{
			Updated,
			Outdated,
			Completed
		}

		public int TypeId { get; set; }
		public int GroupId => GetGroupId();
		public string ItemName => GetItemName();
		public UniverseItem Item => GetUniverseItem();
		public Dictionary<Region, double> CurrentSellPrice = new Dictionary<Region, double>();
		public Dictionary<Region, Dictionary<System, double>> CurrentBuyPrice = new Dictionary<Region, Dictionary<System, double>>();
		public double MaxBuyPrice { get; set; }
		public double CompressedPrice { get; set; }
		public Dictionary<Region, double> ReprocessPrice = new Dictionary<Region, double>();
		public Dictionary<Region, Dictionary<System, EItemStatus>> ItemStatus = new Dictionary<Region, Dictionary<System, EItemStatus>>();
		public ReprocessType ReprocessType { get; private set; } = ReprocessType.None;

		private Color textColor;

		public MarketItem(int typeId)
		{
			TypeId = typeId;
			EveDelegate.Subscribe(ref EveDelegate.UpdateItemNotify, UpdateEventHandler);
			Debug.Log($"Creating Market Item {ItemName}.");

			InitDictionaries();
			SetReprocessType();
			UpdateMarketData();
		}

		~MarketItem()
		{
			EveDelegate.Unsubscribe(ref EveDelegate.UpdateItemNotify, UpdateEventHandler);
		}

		private void InitDictionaries()
		{
			foreach (var region in StaticData.Locations.Keys)
			{
				CurrentSellPrice[region] = 0;
				CurrentBuyPrice[region] = new Dictionary<System, double>();
				ItemStatus[region] = new Dictionary<System, EItemStatus>();

				foreach (var system in StaticData.Locations[region])
				{
					CurrentBuyPrice[region][system] = 0;
					ItemStatus[region][system] = EItemStatus.Updated;
				}
			}
		}

		private int GetGroupId()
		{
			if (Item == null) return 0;
			return Item.MarketGroupId;
		}

		private string GetItemName()
		{
			if (Item == null) return "";
			return Item.Name;
		}

		private double SetReprocessPrice(Region region)
		{
			return ReprocessPrice[region] = Math.Round(Reprocess.CalcReprocessedValue(this), 2);
		}

		private double SetMaxBuy(Region region = Region.None)
		{
			double profitMargin = (double)AppSettings.Settings.MarginPercentage;
			double profitMarginModifier = ((100 - profitMargin) / 100);
			double reprocessedValue = SetReprocessPrice(region);
			MaxBuyPrice = (double)Math.Round(reprocessedValue * profitMarginModifier, 2);
			return MaxBuyPrice;
		}

		private void SetCurrentSellPrice()
		{
			foreach (var region in StaticData.Locations.Keys)
			{
				SetMaxBuy(region);
				OrderRecord record = GetOrderRecord(region);

				if (record == null) return;

				if (record.marketOrders != null && record.marketOrders.Count() > 0)
				{
					List<MarketOrder> sellOrders = record.marketOrders.Where(rec =>
					StaticData.CorpOrderRecord.CorpOrders.Find(order => order.OrderId == rec.OrderId) == null
					&& !rec.IsBuyOrder
					&& rec.LocationId == 60003760
					//&& (rec.SystemId == 30000142 || rec.SystemId == 30000144)
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

					CurrentSellPrice[region] = lowestPice - 0.01;
				}
			}
		}

		private void SetCurrentBuyPrice(Region region = Region.None, System system = System.None)
		{
			if (region == Region.None)
			{
				foreach (var eRegion in StaticData.Locations.Keys)
				{
					SetCurrentSystemBuyPrice(eRegion, system);
				}
			}
			else
			{
				SetCurrentSystemBuyPrice(region, system);
			}
		}

		private void SetCurrentSystemBuyPrice(Region region = Region.None, System system = System.None)
		{
			if (system == System.None)
			{
				foreach (var eSystem in StaticData.Locations[region])
				{
					CalcCurrentBuyPrice(region, eSystem);
				}
			}
			else
			{
				CalcCurrentBuyPrice(region, system);
			}
		}

		private void CalcCurrentBuyPrice(Region region, System system)
		{
			double epsilon = 1e-2;
			double modifier = 0.00d;

			if (ReprocessType == ReprocessType.Ice)
			{
				modifier = 100.00d;
			}
			else
			{
				modifier = 0.01d;
			}

			double highestPice = 0.00d;
			double currentSetBuyPrice = 0.00d;

			OrderRecord orderRecord = GetOrderRecord(region);

			if (orderRecord == null) return;

			List<MarketOrder> marketOrders = orderRecord.marketOrders;

			if (!StaticData.SystemIds.ContainsKey(system))
			{
				return;
			}

			var myOrders = marketOrders.FindAll(rec => StaticData.CorpOrderRecord.CorpOrders.Find(order => order.OrderId == rec.OrderId) != null);
			var myOrder = myOrders.Find(order => order.SystemId == StaticData.SystemIds[system]);

			if (marketOrders != null)
			{
				var buyOrders = marketOrders.FindAll(rec =>
				StaticData.CorpOrderRecord.CorpOrders.Find(order => order.OrderId == rec.OrderId) == null
				&& rec.IsBuyOrder
				&& MaxBuyPrice - (rec.Price + modifier) >= 0
				//&& !((rec.LocationId == 60005143 && rec.Range == "1") || (rec.LocationId == 60003826 && rec.Range == "4") || (rec.LocationId == 60000469 && rec.Range == "3") || (rec.LocationId == 60002263 && rec.Range == "3"))
				);

				if (myOrder == null)
				{
					ItemStatus[region][system] = EItemStatus.Completed;
				}
				else
				{
					ItemStatus[region][system] = EItemStatus.Updated;
					currentSetBuyPrice = myOrder.Price;
				}

				if (buyOrders != null)
				{
					foreach (var order in buyOrders)
					{
						int distanceToOrderRange = 0;

						if (StaticData.Routes != null)
						{
							lock (StaticData.Routes)
							{
								if (StaticData.SystemIds.ContainsKey(system) && StaticData.Routes.TryGetValue(StaticData.SystemIds[system], out List<RouteData> routes))
								{
									if (StaticData.RangeStringToInt.ContainsKey(order.Range))
									{
										var routeToOrder = routes.Find(route => route.Destination == order.SystemId);
										distanceToOrderRange = routeToOrder.NumJumps - StaticData.RangeStringToInt[order.Range];
									}
								}
							}
						}

						if (order.Price > highestPice)
						{
							if (myOrder == null)
							{
								highestPice = order.Price;
							}
							else if (distanceToOrderRange <= StaticData.RangeStringToInt[myOrder.Range])
							{
								highestPice = order.Price;
							}
						}

					}
				}
			}

			double currentBuyPrice = highestPice > 0 ? highestPice + modifier : highestPice;

			if (ItemStatus[region][system] != EItemStatus.Completed
				&& Math.Abs(currentBuyPrice - currentSetBuyPrice) >= epsilon)
			{
				ItemStatus[region][system] = EItemStatus.Outdated;
			}

			CurrentBuyPrice[region][system] = currentBuyPrice;
		}

		private void SetReprocessType()
		{
			if (!StaticData.MarketGroups.ContainsKey(GroupId)) return;

			switch (StaticData.MarketGroups[GroupId].Name)
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

		private UniverseItem GetUniverseItem() => StaticData.GetUniverseItem(TypeId);

		public OrderRecord GetOrderRecord(Region region) => StaticData.GetOrderRecord(region, TypeId);

		public bool UpdateOrderRecord(Region region, List<MarketOrder> marketOrders, string expiration, string etag)
		{
			OrderRecord record = GetOrderRecord(region);

			if (record != null)
			{
				record.ETag = etag;
				record.Expiration = expiration;

				if (marketOrders != null)
				{
					record.marketOrders = marketOrders;
					UpdateMarketData(region: region);
				}

				return true;
			}
			else if (marketOrders != null)
			{
				lock (StaticData.OrderRecords)
				{
					if (!StaticData.OrderRecords.ContainsKey(region))
					{
						StaticData.OrderRecords[region] = new Dictionary<int, OrderRecord>();
					}

					StaticData.OrderRecords[region][TypeId] = new OrderRecord(TypeId, marketOrders, expiration, etag);
				}

				UpdateMarketData(region: region);
				return true;
			}

			return false;
		}

		public void UpdateMarketData(Region region = Region.None, System system = System.None)
		{
			SetCurrentSellPrice();
			SetCurrentBuyPrice(region, system);
			EveDelegate.UpdateUINotify?.Invoke(TypeId);
			Debug.Log($"Market Item {ItemName} Updated!");
		}

		public void UpdateEventHandler(int typeId = 0)
		{
			if (typeId == 0 || typeId == TypeId)
			{
				UpdateMarketData();
			}
		}

		public int Compare(MarketItem item1, MarketItem item2)
		{
			if (item1 == null) return (item2 == null) ? 0 : -1;
			if (item2 == null) return 1;

			return item1.ItemName.CompareTo(item2.ItemName);
		}
	}
}
