
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
using System.Threading;

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
		Ice,
		Mercoxit,
		Rare,
		Simple,
		Ubiquitous,
		Uncommon,
		Variegated
	}

	public enum Product
	{
		HeavyWater,
		LiquidOzone,
		StrontiumClathrates,
		HeliumIsotopes,
		NitrogenIsotopes,
		OxygenIsotopes,
		HydrogenIsotopes,
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
		Lonetrek,
		The_Citadel,
		None
	}

	public enum System
	{
		Inaro,
		Jita,
		Tunttaras,
		Umokka,
		Urlen,
		Ylandoki,
		None
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
		public static Dictionary<System, int> SystemIds { get; set; } = new Dictionary<System, int>()
		{
			{ System.Jita, 30000142 },
			{ System.Inaro, 30002788 },
			{ System.Tunttaras, 30001379 },
			{ System.Umokka, 30001409 },
			{ System.Urlen, 30000139 },
			{ System.Ylandoki, 30001395 }
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
			{ Range.Region, "region" },
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
			Mineral,
			Ice_Ores,
			Ice_Products
		}

		private static Dictionary<int, string> GroupIdsToName = new Dictionary<int, string>()
		{
			{ 1857, "Mineral" },
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
			{ 1033, "Ice Products" },
			{ 1855, "Ice Ores"}
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
			{ Region.Lonetrek, 10000016 },
			{ Region.The_Citadel, 10000033 }
		};

		public static bool IsSubscribed { get; set; } = false;

		public static async Task WaitForPendingMarketRequestsToComplete()
		{
			while (NetworkManager.pendingMarketRequests > 10)
			{
				await Task.Delay(100); // Wait for 100 milliseconds before checking again
			}
		}

		public static async Task WaitForPendingGroupRequestsToComplete()
		{
			while (NetworkManager.pendingMarketGroups > 2)
			{
				await Task.Delay(100); // Wait for 100 milliseconds before checking again
			}
		}

		public static async void UpdateStaticData()
		{
			EveDelegate.Subscribe(ref EveDelegate.StaticUpdateComplete, SaveStaticData);

			lock (sb) { sb.Clear(); }

			await NetworkManager.AsyncRequest<List<MarketPrice>>();

			var marketGroupIds = GroupIdsToName.Keys.ToArray();

			for (int i = 0; i < marketGroupIds.Length; i++)
			{
				await WaitForPendingMarketRequestsToComplete();
				await NetworkManager.AsyncRequest<MarketGroup>(marketGroupIds[i].ToString());
			}

			ConstructMarketObjects();
		}

		public static async void UpdateMarketData(List<int> ids)
		{
			if (ids == null) return;

			await WaitForPendingGroupRequestsToComplete();

			if (!IsSubscribed)
			{
				EveDelegate.Subscribe(ref EveDelegate.MarketUpdateComplete, SaveMarketData);
				IsSubscribed = true;
			}

			Interlocked.Increment(ref NetworkManager.pendingMarketGroups);

			Debug.Log($"{NetworkManager.pendingMarketGroups} market group requests started.");

			foreach (var id in ids)
			{
				await RequestMarketOrders(id);
			}

			NetworkManager.CompleteGroupUpdate();
		}

		public static void LoadStaticData()
		{
			Dictionary<int, MarketPrice> marketPrices = FileManager.DeserializeFromFile<Dictionary<int, MarketPrice>>();
			if (marketPrices != null)
			{
				lock (MarketPrices)
				{
					MarketPrices = marketPrices;
				}
			}

			Dictionary<int, MarketGroup> groupObjects = FileManager.DeserializeFromFile<Dictionary<int, MarketGroup>>();
			if (groupObjects != null)
			{
				lock (GroupObjects)
				{
					GroupObjects = groupObjects;
				}
			}

			Dictionary<int, UniverseItem> itemObjects = FileManager.DeserializeFromFile<Dictionary<int, UniverseItem>>();
			if (itemObjects != null)
			{
				lock (ItemObjects)
				{
					ItemObjects = itemObjects;
				}
			}

			Dictionary <Region, Dictionary<int, OrderRecord>> orderRecords = FileManager.DeserializeFromFile<Dictionary<Region, Dictionary<int, OrderRecord>>>();
			if (orderRecords != null)
			{
				lock (OrderRecords)
				{
					OrderRecords = orderRecords;
				}
			}

			Dictionary<int, List<RouteData>> routes = FileManager.DeserializeFromFile<Dictionary<int, List<RouteData>>>();
			if (routes != null)
			{
				Routes = routes;
			}

			ConstructMarketObjects();

			Debug.Log("Static Data Loaded.");
		}

		public static void ConstructMarketObjects()
		{

			lock (StaticData.GroupObjects)
			{
				lock (MarketObjects)
				{
					MarketObjects[StaticData.GroupObjects[1857].TypeId] = new MarketObject(StaticData.GroupObjects[1857]);
					MarketObjects[StaticData.GroupObjects[1033].TypeId] = new MarketObject(StaticData.GroupObjects[1033]);

					foreach (var group in StaticData.GroupObjects.Values)
					{
						MarketObjects[group.TypeId] = new MarketObject(group);
					}
				}
			}
		}

		public static async void HandleResponse<T>(string response, Region region = Region.The_Forge, int type_id = 0)
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
						await NetworkManager.AsyncRequest<UniverseItem>(typeId.ToString());
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

						NetworkManager.CompleteMarketUpdateTask();
					}

					int origin = SystemIds[AppSettings.BuyOrderSystem];

					foreach (var order in marketOrders)
					{
						lock (Routes)
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
						}

						await WaitForPendingMarketRequestsToComplete();
						Interlocked.Increment(ref NetworkManager.pendingMarketRequests);
						Debug.Log($"{NetworkManager.pendingMarketRequests} market request(s) pending.");
						NetworkManager.AsyncRequest<List<int>>(data: new RouteData(origin: origin, destination: order.SystemId));
						break;
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

						NetworkManager.CompleteMarketUpdateTask();
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

				//UnityMainThreadDispatcher.Instance.Enqueue(() =>
				//{
				//	Debug.Log(typeof(T));
				//});

				NetworkManager.CompleteUpdateTask();
			}
		}

		public static async Task RequestMarketOrders(int typeId)
		{
			lock (OrderRecords)
			{
				if (!OrderRecords.ContainsKey(AppSettings.SellRegion))
					OrderRecords[AppSettings.SellRegion] = new Dictionary<int, OrderRecord>();
			}

			await WaitForPendingMarketRequestsToComplete();
			Interlocked.Increment(ref NetworkManager.pendingMarketRequests);
			Debug.Log($"{NetworkManager.pendingMarketRequests} market request(s) pending.");
			NetworkManager.AsyncRequest<List<MarketOrder>>(region: AppSettings.SellRegion, type_id: typeId);

			if (AppSettings.BuyRegion != AppSettings.SellRegion)
			{

				lock (OrderRecords)
				{
					if (!OrderRecords.ContainsKey(AppSettings.BuyRegion))
						OrderRecords[AppSettings.BuyRegion] = new Dictionary<int, OrderRecord>();
				}

				for (int i = 0; i < Enum.GetValues(typeof(Region)).Length - 1; i++)
				{
					await WaitForPendingMarketRequestsToComplete();
					Interlocked.Increment(ref NetworkManager.pendingMarketRequests);
					Debug.Log($"{NetworkManager.pendingMarketRequests} market request(s) pending.");
					NetworkManager.AsyncRequest<List<MarketOrder>>(region: (Region)i, type_id: typeId);
				}
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

		public static void SaveMarketData()
		{
			Debug.Log($"Saving Market Data");

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

			IsSubscribed = false;
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
