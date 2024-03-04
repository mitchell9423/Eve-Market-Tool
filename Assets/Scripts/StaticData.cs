
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Text;
using EveMarket.Util;
using EveMarket.Network;
using System.Linq;
using System.Threading.Tasks;
using static UnityEditor.Progress;
using static UnityEngine.UI.Image;
using static System.Collections.Specialized.BitVector32;

namespace EveMarket
{
	public enum ReprocessType
	{
		None,
		Abyssal,
		Coherent,
		Common,
		Complex,
		Exceptional,
		Mercoxit,
		Rare,
		Simple,
		Ubiquitous,
		Uncommon,
		Variegated
	}

	public enum Mineral
	{
		Tritanium, 
		Pyerite, 
		Mexallon,
		Isogen, 
		Nocxium,
		Megacyte,
		Morphite,
		Zydrine,
		NeoJadarite,
		ChromodynamicTricarboxyls
	}

	public enum Group
	{
		Arkonor,
		Bistot,
		Pyroxeres,
		Plagioclase,
		Spodumain,
		Veldspar,
		Scordite,
		Crokite,
		Dark_Ochre,
		Kernite,
		Gneiss,
		Omber,
		Hedbergite,
		Hemorphite,
		Jaspet,
		Mercoxit,
		Bezdnacine,
		Rakovene,
		Talassonite,
		Mordunium,
		Mineral
	}

	public enum ObjectType
	{
		MarketPrice,
		MarketGroup,
		UniverseItem
	}

	public enum Region
	{
		The_Forge,
		Lonetrek
	}

	public enum System
	{
		Tunttaras,
		Ylandoki
	}

	public enum Range
	{
		Station,
		Solar_System,
		Jump_1,
		Jump_2,
		Jump_3,
		Jump_4,
		Jump_5,
		Jump_10,
		Jump_20,
		Jump_30,
		Jump_40,
		Region
	}

	public static class StaticData
	{
		private static StringBuilder sb = new StringBuilder();
		public static Dictionary<System, int> BuyOrderSystems { get; set; } = new Dictionary<System, int>()
		{
			{ System.Ylandoki, 30001395 },
			{ System.Tunttaras, 30001379 }			
		};

		public static Dictionary<Range, string> RangeStringName = new Dictionary<Range, string>()
		{
			{ Range.Station, "station" },
			{ Range.Solar_System, "solarsystem" },
			{ Range.Jump_1, "1" },
			{ Range.Jump_2, "2" },
			{ Range.Jump_3, "3" },
			{ Range.Jump_4, "4" },
			{ Range.Jump_5, "5" },
			{ Range.Jump_10, "10" },
			{ Range.Jump_20, "20" },
			{ Range.Jump_30, "30" },
			{ Range.Jump_40, "40" },
			{ Range.Region, "Region" },
		};

		public static Dictionary<string, int> RangeStringToInt = new Dictionary<string, int>()
		{
			{ "region", 100 },
			{ "solarsystem", 0 },
			{ "station", 0 },
			{ "1", 1 },
			{ "2", 2 },
			{ "3", 3 },
			{ "4", 4 },
			{ "5", 5 },
			{ "10", 10 },
			{ "20", 20 },
			{ "30", 30 },
			{ "40", 40 }
		};

		private static Dictionary<int, string> GroupIdsToName = new Dictionary<int, string>()
		{
			{ 512, "Arkonor" },
			{ 514, "Bistot" },
			{ 515, "Pyroxeres" },
			{ 516, "Plagioclase" },
			{ 517, "Spodumain" },
			{ 518, "Veldspar" },
			{ 519, "Scordite" },
			{ 521, "Crokite" },
			{ 522, "Dark Ochre" },
			{ 523, "Kernite" },
			{ 525, "Gneiss" },
			{ 526, "Omber" },
			{ 527, "Hedbergite" },
			{ 528, "Hemorphite" },
			{ 529, "Jaspet" },
			{ 530, "Mercoxit" },
			{ 2538, "Bezdnacine" },
			{ 2539, "Rakovene" },
			{ 2540, "Talassonite" },
			{ 3487, "Mordunium" },
			{ 1857, "Mineral" }
		};

		public static Dictionary<int, List<RouteData>> Routes = new Dictionary<int, List<RouteData>>();
		public static Dictionary<int, MarketPrice> MarketPrices = new Dictionary<int, MarketPrice>();
		public static Dictionary<int, MarketGroup> GroupObjects = new Dictionary<int, MarketGroup>();
		public static Dictionary<int, UniverseItem> ItemObjects = new Dictionary<int, UniverseItem>();
		public static Dictionary<Region, Dictionary<int, OrderRecord>> OrderRecords = new Dictionary<Region, Dictionary<int, OrderRecord>>();
		public static Dictionary<int, MarketObject> MarketObjects = new Dictionary<int, MarketObject>();
		public static Dictionary<Region, int> RegionId = new Dictionary<Region, int>()
		{
			{ Region.The_Forge, 10000002 },
			{ Region.Lonetrek, 10000016 }
		};

		public static void UpdateStaticData()
		{
			EveDelegate.Subscribe(ref EveDelegate.StaticUpdateComplete, SaveStaticData);

			lock (sb) { sb.Clear(); }

			foreach (var groupId in GroupIdsToName.Keys)
			{
				NetworkManager.AsyncRequest<MarketGroup>(groupId.ToString());
			}

			ConstructMarketObjects();
		}

		public static void UpdateMarketData(List<int> ids)
		{
			EveDelegate.Subscribe(ref EveDelegate.StaticUpdateComplete, SaveMarketData);

			NetworkManager.AsyncRequest<List<MarketPrice>>();

			foreach (var id in ids)
			{
				RequestMarketOrders(id);
			}
		}

		public static void LoadStaticData()
		{
			if (MarketPrices != null)
			{
				lock (MarketPrices)
				{
					MarketPrices = FileManager.DeserializeFromFile<Dictionary<int, MarketPrice>>();
				}
			}

			if (GroupObjects != null)
			{
				lock (GroupObjects)
				{
					GroupObjects = FileManager.DeserializeFromFile<Dictionary<int, MarketGroup>>();
				}
			}

			if (ItemObjects != null)
			{
				lock (ItemObjects)
				{
					ItemObjects = FileManager.DeserializeFromFile<Dictionary<int, UniverseItem>>();
				}
			}

			if (OrderRecords != null)
			{
				lock (OrderRecords)
				{
					try
					{
						OrderRecords = FileManager.DeserializeFromFile<Dictionary<Region, Dictionary<int, OrderRecord>>>();
					}
					catch { }
				}
			}

			lock (Routes)
			{
				try
				{
					Routes = FileManager.DeserializeFromFile<Dictionary<int, List<RouteData>>>();
				}
				catch
				{
					Routes = new Dictionary<int, List<RouteData>>();
				}
			}

			ConstructMarketObjects();

			Debug.Log("Static Data Loaded.");
		}

		private static void ConstructMarketObjects()
		{
			lock (StaticData.GroupObjects)
			{
				foreach (var group in StaticData.GroupObjects.Values)
				{
					MarketObjects[group.TypeId] = new MarketObject(group);
				}
			}

		}

		public static void HandleResponse<T>(string response, Region region = Region.The_Forge, int type_id = 0)
		{
			string str;

			if (string.IsNullOrEmpty(response))
			{
				lock (sb)
				{
					sb.Append($"Error receiving data.\n");
				}

				Debug.Log("Error receiving data.");
				return;
			}

			try
			{
				T objectModel = JsonConvert.DeserializeObject<T>(response);

				if (objectModel == null)
				{
					Debug.Log("Error receiving data.");
					return;
				}

				StringBuilder localSb = new StringBuilder();

				if (objectModel is MarketGroup marketGroup)
				{
					lock (GroupObjects)
					{
						GroupObjects[marketGroup.TypeId] = marketGroup;
						localSb.Append($"{GroupObjects[marketGroup.TypeId].TypeId,5} : {GroupObjects[marketGroup.TypeId].Name}\n");
					}

					foreach (var typeId in marketGroup.Types)
					{
						NetworkManager.AsyncRequest<UniverseItem>(typeId.ToString());
					}
				}
				else if (objectModel is UniverseItem universeItem)
				{
					lock (ItemObjects)
					{
						ItemObjects[universeItem.TypeId] = universeItem;
						localSb.Append($"{ItemObjects[universeItem.TypeId].TypeId,5} : {ItemObjects[universeItem.TypeId].Name}\n");
					}
				}
				else if (objectModel is List<MarketPrice> marketPriceArray)
				{
					lock (MarketPrices)
					{
						for (int i = 0; i < marketPriceArray.Count; i++)
						{
							MarketPrices[marketPriceArray[i].TypeId] = marketPriceArray[i];
							localSb.Append($"{MarketPrices[marketPriceArray[i].TypeId].TypeId,5} : {MarketPrices[marketPriceArray[i].TypeId].AveragePrice}\n");
						}
					}
				}
				else if (objectModel is List<MarketOrder> marketOrders)
				{
					lock (OrderRecords)
					{
						if (!OrderRecords.ContainsKey(region))
						{
							OrderRecords[region] = new Dictionary<int, OrderRecord>();
						}

						OrderRecords[region][type_id] = new OrderRecord(marketOrders);
					}

					if (Routes == null) Routes = new Dictionary<int, List<RouteData>>();

					lock (Routes)
					{
						int origin = StaticData.BuyOrderSystems[AppSettings.BuyOrderSystem];

						foreach (var order in marketOrders)
						{
							if (Routes.ContainsKey(origin))
							{
								if (Routes[origin] != null)
								{
									if (Routes[origin].Exists(route => route.Destination == order.SystemId))
									{
										continue;
									}
								}
							}

							NetworkManager.AsyncRequest<List<int>>(data: new RouteData(origin: origin, destination: order.SystemId));
							break;
						}
					}
				}
				else if (objectModel is List<int> route)
				{
					if (Routes == null) Routes = new Dictionary<int, List<RouteData>>();

					lock (Routes)
					{
						if (route.Count > 0)
						{
							int origin = route[route.Count - 1];
							int destination = route[0];

							if (!Routes.ContainsKey(origin)) Routes[origin] = new List<RouteData>();

							if (!Routes[origin].Exists(rte => rte.Destination == destination))
							{
								Routes[origin].Add(
									new RouteData(
										origin: origin,
										destination: destination,
										route: route
										));
							}
						}
					}
				}

				lock (sb)
				{
					sb.Append(localSb);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error deserializing type {typeof(T)} object.");
				Debug.Log($"{ex}");
			}
			finally
			{
				lock (sb)
				{
					str = sb.ToString();
				}

				UnityMainThreadDispatcher.Instance.Enqueue(() =>
				{
					Debug.Log(typeof(T));
				});

				NetworkManager.CompleteStaticUpdateTask();
			}
		}

		public static void RequestMarketOrders(int typeId)
		{
			lock (OrderRecords)
			{
				if (!OrderRecords.ContainsKey(AppSettings.SellRegion))
					OrderRecords[AppSettings.SellRegion] = new Dictionary<int, OrderRecord>();
			}

			NetworkManager.AsyncRequest<List<MarketOrder>>(region: AppSettings.SellRegion, type_id: typeId);

			if (AppSettings.BuyRegion != AppSettings.SellRegion)
			{
				lock (OrderRecords)
				{
					if (!OrderRecords.ContainsKey(AppSettings.BuyRegion))
						OrderRecords[AppSettings.BuyRegion] = new Dictionary<int, OrderRecord>();
				}

				NetworkManager.AsyncRequest<List<MarketOrder>>(region: AppSettings.BuyRegion, type_id: typeId);
			}
		}

		private static void SaveStaticData()
		{
			EveDelegate.Unsubscribe(ref EveDelegate.StaticUpdateComplete, SaveStaticData);

			lock (GroupObjects)
			{
				FileManager.SerializeObject(GroupObjects);
			}

			lock (ItemObjects)
			{
				FileManager.SerializeObject(ItemObjects);
			}
		}

		private static void SaveMarketData()
		{
			Debug.Log($"Saving Market Data");

			EveDelegate.Unsubscribe(ref EveDelegate.StaticUpdateComplete, SaveMarketData);

			lock (MarketPrices)
			{
				FileManager.SerializeObject(MarketPrices);
			}

			lock (OrderRecords)
			{
				FileManager.SerializeObject(OrderRecords);
			}

			lock (Routes)
			{
				FileManager.SerializeObject(Routes);
			}

			foreach (var marketObject in MarketObjects.Values)
			{
				marketObject.UpdateMarketData();
			}
		}

		public static void UpdateMarketObjects()
		{
			foreach (var marketObject in MarketObjects.Values)
			{
				foreach (var marketItem in marketObject.Items)
				{
					marketItem.UpdateOrders();
				}
			}
		}
	}
}
