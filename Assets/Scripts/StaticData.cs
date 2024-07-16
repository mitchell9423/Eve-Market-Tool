using EveMarket.Network;
using EveMarket.StateMachine;
using EveMarket.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace EveMarket
{
	public enum AppState
	{
		Authentication,
		LoadAppSettings,
		LoadStaticData,
		UpdateStaticData,
		UpdateEveMarketUI,
		UpdateMarketOrders,
		UpdateCorpOrders,
		ConstructMarketObjects,
		UpdateMarketObjects,
		ConstructEveMarketUI,
		SaveMarketData,
		Idle
	}

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
		Minerals,
		Ice_Products,
		Ice_Ores
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
		None,
		Inaro,
		Jita,
		Tunttaras,
		Umokka,
		Urlen,
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

	public struct OrderRecordMeta
	{
		public string Expiration;
		public string ETag;

		public OrderRecordMeta(string exp, string tag)
		{
			Expiration = exp;
			ETag = tag;
		}
	}

	public static class StaticData
	{
		private static StringBuilder sb = new StringBuilder();
		public static Dictionary<Region, List<System>> Locations { get; set; } = new Dictionary<Region, List<System>>()
		{
			{
				Region.The_Forge, new List<System>()
				{
					System.Jita
				}
			},
			{
				Region.Lonetrek, new List<System>()
				{
					System.Tunttaras,
					System.Umokka,
					System.Ylandoki
				}
			},
			{
				Region.The_Citadel, new List<System>()
				{
					System.Inaro,
					System.Urlen
				}
			}
		};
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
			Minerals,
			Ice_Ores,
			Ice_Products
		}

		private static Dictionary<int, string> GroupIdsToName = new Dictionary<int, string>()
		{
			{ 1857, "Minerals" },
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
		public static Dictionary<int, MarketGroup> MarketGroups = new Dictionary<int, MarketGroup>();
		public static Dictionary<int, MarketItem> MarketItems = new Dictionary<int, MarketItem>();
		public static Dictionary<int, UniverseItem> UniverseItems = new Dictionary<int, UniverseItem>();

		private static List<UniverseItem> GetUniverseItem()
		{
			return FileManager.DeserializeFromFile<List<UniverseItem>>();
		}

		public static CorpOrderRecord CorpOrderRecord { get; set; }
		public static Dictionary<Region, Dictionary<int, OrderRecord>> OrderRecords = new Dictionary<Region, Dictionary<int, OrderRecord>>();
		public static Dictionary<Region, Dictionary<int, OrderRecordMeta>> OrderRecordMeta = new Dictionary<Region, Dictionary<int, OrderRecordMeta>>();
		public static Dictionary<int, MarketObject> MarketObjects = new Dictionary<int, MarketObject>();
		public static Dictionary<Region, int> RegionId = new Dictionary<Region, int>()
		{
			{ Region.The_Forge, 10000002 },
			{ Region.Lonetrek, 10000016 },
			{ Region.The_Citadel, 10000033 }
		};

		public static bool IsSubscribed { get; set; } = false;
		public static bool UpdateChangedRecordsOnly { get; set; } = false;
		public static List<int> ItemsToUpdate = new List<int>();

		public static MarketItem GetMarketItem(int typeId)
		{
			lock (MarketItems)
			{
				if (MarketItems.TryGetValue(typeId, out MarketItem marketItem))
				{
					return marketItem;
				}

				return null;
			}
		}

		public static OrderRecord GetOrderRecord(Region region, int typeId)
		{
			if (typeId == 0)
			{
				Debug.LogWarning($"Record lookup error: type = {typeId}");
				return null;
			}

			lock (OrderRecords)
			{
				if (OrderRecords.ContainsKey(region) && OrderRecords[region].ContainsKey(typeId))
				{
					return OrderRecords[region][typeId];
				}

				return null;
			}
		}

		public static UniverseItem GetUniverseItem(int typeId)
		{
			lock (UniverseItems)
			{
				if (UniverseItems.TryGetValue(typeId, out UniverseItem universeItem))
				{
					return universeItem;
				}

				return new UniverseItem();
			}
		}

		public static MarketGroup GetMarketGroup(int typeId)
		{
			lock (MarketGroups)
			{
				if (MarketGroups.TryGetValue(typeId, out MarketGroup marketGroup))
				{
					return marketGroup;
				}

				return new MarketGroup();
			}
		}

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

		public static async Task<bool> UpdateStaticData()
		{
			Debug.Log("Updating Static Data.");

			try
			{
				EveDelegate.Subscribe(ref EveDelegate.StaticUpdateComplete, SaveStaticData);

				var marketGroupIds = GroupIdsToName.Keys.ToArray();

				for (int i = 0; i < marketGroupIds.Length; i++)
				{
					await WaitForPendingMarketRequestsToComplete();

					Debug.Log($"Requesting market group info - {GroupIdsToName[marketGroupIds[i]]}.");

					await NetworkManager.AsyncRequest<MarketGroup>(marketGroupIds[i].ToString());
				}

				return true;
			}
			catch (Exception ex)
			{
				// Log the error
				Debug.LogError($"An error occurred: {ex.Message}");

				// Return false if an exception occurs
				return false;
			}
		}

		public static async void UpdateMarketData(List<int> ids, string tag = "")
		{
			if (ids == null) return;

			await WaitForPendingGroupRequestsToComplete();

			if (!IsSubscribed)
			{
				IsSubscribed = true;
			}

			Interlocked.Increment(ref NetworkManager.pendingMarketGroups);
			int groupRequestId = Interlocked.Increment(ref NetworkManager.totalMarketGroups);

			//Debug.Log($"Starting market group request {NetworkManager.totalMarketGroups}...");

			foreach (var id in ids)
			{
				for (int i = 0; i < Enum.GetValues(typeof(Region)).Length - 1; i++)
				{
					await RequestMarketItemOrders(typeId: id, region: (Region)i, tag: tag);
				}
			}

			NetworkManager.CompleteGroupUpdate(groupRequestId);
		}

		public static async void UpdateItemMarketData(int typeId = 0, Region region = Region.The_Forge, string tag = "")
		{
			await RequestMarketItemOrders(typeId: typeId, region: region, tag: tag);
		}

		public static bool LoadStaticData()
		{
			Debug.Log("Loading static data.");

			bool success = true;
			Dictionary<int, UniverseItem> universeItem = null;

			try
			{
				universeItem = FileManager.DeserializeFromFile<Dictionary<int, UniverseItem>>();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				universeItem = new Dictionary<int, UniverseItem>();
				FileManager.SerializeObject(universeItem);
				success = false;
			}
			finally
			{
				lock (UniverseItems)
				{
					UniverseItems = universeItem;
				}
			}

			CorpOrderRecord corpOrderRecord = FileManager.DeserializeFromFile<CorpOrderRecord>();
			if (corpOrderRecord != null)
			{
				CorpOrderRecord = corpOrderRecord;
			}
			else
			{
				success = false;
			}

			Dictionary<int, MarketPrice> marketPrices = FileManager.DeserializeFromFile<Dictionary<int, MarketPrice>>();
			if (marketPrices != null)
			{
				lock (MarketPrices)
				{
					MarketPrices = marketPrices;
				}
			}
			else
			{
				success = false;
			}

			Dictionary<int, MarketGroup> groupObjects = FileManager.DeserializeFromFile<Dictionary<int, MarketGroup>>();
			if (groupObjects != null)
			{
				lock (MarketGroups)
				{
					MarketGroups = groupObjects;
				}
			}
			else
			{
				success = false;
			}

			Dictionary<Region, Dictionary<int, OrderRecord>> orderRecords = FileManager.DeserializeFromFile<Dictionary<Region, Dictionary<int, OrderRecord>>>();
			if (orderRecords != null)
			{
				lock (OrderRecords)
				{
					OrderRecords = orderRecords;
				}
			}
			else
			{
				success = false;
			}

			Dictionary<int, List<RouteData>> routes = FileManager.DeserializeFromFile<Dictionary<int, List<RouteData>>>();
			if (routes != null)
			{
				Routes = routes;
			}
			else
			{
				success = false;
			}

			//if (itemObjects == null)
			//{
			//	UpdateStaticData();
			//}
			//else
			//{
			//	EveDelegate.StaticLoadComplete?.Invoke();
			//	Debug.Log("Static Data Loaded.");
			//}
			Debug.Log("Static Data Loaded.");

			return success;
		}

		public static void QueueItemForUpdate(int typeId)
		{
			if (!ItemsToUpdate.Exists(id => id == typeId))
			{
				ItemsToUpdate.Add(typeId);
			}
		}

		public static void ConstructMarketObjects()
		{
			Debug.Log("Constructing Market Objects.");

			lock (MarketGroups)
			{
				lock (MarketObjects)
				{
					if (MarketGroups.TryGetValue(1857, out MarketGroup Minerals))
					{
						MarketObjects[Minerals.TypeId] = new MarketObject(Minerals);
					}

					if (MarketGroups.TryGetValue(1033, out MarketGroup IceProducts))
					{
						MarketObjects[IceProducts.TypeId] = new MarketObject(IceProducts);
					}

					foreach (var group in MarketGroups.Values)
					{
						if (
							group.Name == EnumToString(Group.Ice_Ores)
							|| group.Name == Group.Veldspar.ToString()
							|| group.Name == Group.Plagioclase.ToString()
							|| group.Name == Group.Pyroxeres.ToString()
							|| group.Name == Group.Scordite.ToString()
							//|| group.Name == Group.Kernite.ToString()
							)
						{
							Debug.Log($"Creating Market Group {group.Name}.");
							MarketObjects[group.TypeId] = new MarketObject(group);
						}
					}
				}
			}

			EveStateMachine.SetNextState(new ConstructEveMarketUI(), AppState.ConstructEveMarketUI);
		}

		public static async void HandleCorpOrder(long code, string tag, string expiration, string response, Region region = Region.The_Forge, int type_id = 0)
		{
			try
			{
				List<CorpOrder> corpOrders = new List<CorpOrder>();

				if (!string.IsNullOrEmpty(response))
				{
					corpOrders = JsonConvert.DeserializeObject<List<CorpOrder>>(response);
				}

				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append($"Corp Order - Code:{code} - ");
				var oldExpiration = CorpOrderRecord.Expiration;
				var oldTag = CorpOrderRecord.ETag;
				string action = $"Updated.";

				if (code == 304)
				{
					action = $"No Change.";
					//Debug.Log($"Corp Order - Code:{code} - No Change.\n[{type_id} : {expiration} : {tag}]");
					CorpOrderRecord.Expiration = expiration;
					CorpOrderRecord.ETag = tag;
				}
				else if (corpOrders != null)
				{
					stringBuilder.Append($"Updated.\n[{type_id} : ");
					//Debug.Log($"Corp Order - Code:{code} - Updated.\n[{type_id} : {expiration} : {tag}]");
					CorpOrderRecord = new CorpOrderRecord(type_id, corpOrders, expiration, tag);
				}

				stringBuilder.Append($"{action} [{type_id}]\nExpiration:{oldExpiration}");

				if (oldExpiration != expiration)
				{
					stringBuilder.Append($" => {expiration}");
				}

				if (oldTag != tag)
				{
					stringBuilder.Append($"\nOld:{oldTag}\nNew:{tag}");
				}
				else
				{
					stringBuilder.Append($"\nTag:{oldTag}");
				}

				Debug.Log(stringBuilder);
				await Task.Run(() => FileManager.SerializeObject(CorpOrderRecord));
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error handling response for type {typeof(MarketOrder)}.\n\n{ex}");
			}
			finally
			{
				NetworkManager.CompleteNetworkTask();
			}

			EveStateMachine.SetNextState(new UpdateMarketOrders(), AppState.UpdateMarketOrders);
		}

		public static async void HandleMarketOrder(long code, string tag, string expiration, string response, Region region = Region.The_Forge, int type_id = 0)
		{
			if (type_id <= 0) return;

			try
			{
				List<MarketOrder> newMarketOrders = null;

				if (!string.IsNullOrEmpty(response))
				{
					newMarketOrders = JsonConvert.DeserializeObject<List<MarketOrder>>(response);
				}

				int origin = SystemIds[AppSettings.Settings.BuyOrderSystem];

				if (newMarketOrders != null)
				{
					foreach (var order in newMarketOrders)
					{
						lock (Routes)
						{
							if (Routes.ContainsKey(origin) && Routes[origin] != null)
							{
								if (Routes[origin].Exists(route => route.Destination == order.SystemId))
								{
									continue;
								}
							}
						}

						await WaitForPendingMarketRequestsToComplete();
						Interlocked.Increment(ref NetworkManager.pendingMarketRequests);
						_ = NetworkManager.AsyncRequest<List<int>>(data: new RouteData(origin: origin, destination: order.SystemId));
					}
				}

				lock (MarketItems)
				{
					if (code == 304)
					{
						Debug.Log($"Market Order - No Change - {MarketItems[type_id].ItemName} - {type_id} -  {region} - [Expires : {expiration}]");
					}
					else if (newMarketOrders != null)
					{
						Debug.Log($"Market Order - Updated - {MarketItems[type_id].ItemName} - {type_id} -  {region} - [Expires : {expiration}]\nETag : {tag}]");
					}
					else
					{
						Debug.Log($"Market Order - Cdoe:{code} - {MarketItems[type_id].ItemName} - {type_id} -  {region} - [Expires : {expiration}]\nETag : {tag}]");
					}

					MarketItems[type_id].UpdateOrderRecord(region, newMarketOrders, expiration, tag);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error handling response for type {typeof(MarketOrder)}.\n\n{ex}");
			}
			finally
			{
				OrderRecords[region][type_id].Expiration = expiration;
				OrderRecords[region][type_id].ETag = tag;
				NetworkManager.CompleteMarketUpdateTask();
				NetworkManager.CompleteNetworkTask();
			}
		}

		public static void HandleRouteData(long code, string tag, string expiration, string response, Region region = Region.The_Forge, int type_id = 0)
		{
			List<int> route = JsonConvert.DeserializeObject<List<int>>(response);

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

		public static async void HandleResponse<T>(long code, string tag, string expiration, string response, Region region = Region.The_Forge, int type_id = 0) where T : class
		{

			if (code != 200 && code != 304)
			{
				Debug.LogWarning($"Error receiving data : code {code} : Region {region} : TypeId {type_id} : type {typeof(T).Name}.\n{response}");
				NetworkManager.CompleteMarketUpdateTask();
				return;
			}

			if (typeof(T) == typeof(CorpOrder))
			{
				//Debug.Log(response);
				HandleCorpOrder(code, tag, expiration, response, region, type_id);
				//EveStateMachine.SetNextState(new LoadStaticData(), AppState.LoadStaticData);
				return;
			}

			if (typeof(T) == typeof(List<MarketOrder>))
			{
				HandleMarketOrder(code, tag, expiration, response, region, type_id);
				return;
			}

			if (string.IsNullOrEmpty(response))
			{
				lock (sb)
				{
					sb.Append($"Error receiving data : code {code} : type {typeof(T)}.\n");
				}

				Debug.LogWarning($"Error receiving data : code {code} : type {typeof(T)}.\n");
				NetworkManager.CompleteMarketUpdateTask();
				return;
			}

			if (typeof(T) == typeof(List<int>))
			{
				HandleRouteData(code, tag, expiration, response, region, type_id);
				return;
			}

			string str;

			try
			{
				T objectModel = JsonConvert.DeserializeObject<T>(response);

				if (objectModel == null)
				{
					Debug.LogWarning("Error receiving data.");
					return;
				}

				StringBuilder localSb = new StringBuilder();

				if (objectModel is MarketGroup marketGroup)
				{
					lock (MarketGroups)
					{
						MarketGroups[marketGroup.TypeId] = marketGroup;
						Debug.Log($"GroupObject {MarketGroups[marketGroup.TypeId].Name} downloaded.");
						localSb.Append($"{MarketGroups[marketGroup.TypeId].TypeId,5} : {MarketGroups[marketGroup.TypeId].Name}\n");
					}

					foreach (var typeId in marketGroup.Types)
					{
						await NetworkManager.AsyncRequest<UniverseItem>(typeId.ToString());
					}
				}
				else if (objectModel is UniverseItem universeItem)
				{
					lock (UniverseItems)
					{
						UniverseItems[universeItem.TypeId] = universeItem;

						Debug.Log($"Downloaded UniverseItem {UniverseItems[universeItem.TypeId].Name}.");
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
				else if (objectModel is List<int> route)
				{
				}

				lock (sb)
				{
					sb.Append(localSb);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error deserializing type {typeof(T)} object.\n\n{ex}");
			}
			finally
			{
				lock (sb)
				{
					str = sb.ToString();
				}

				NetworkManager.CompleteNetworkTask();
			}
		}

		public static async Task RequestMarketItemOrders(int typeId = 0, Region region = Region.The_Forge, string tag = "")
		{
			await WaitForPendingMarketRequestsToComplete();
			Interlocked.Increment(ref NetworkManager.pendingMarketRequests);
			_ = NetworkManager.AsyncRequest<List<MarketOrder>>(region: region, type_id: typeId, ETag: tag);
		}

		private static void SaveStaticData()
		{
			EveDelegate.Unsubscribe(ref EveDelegate.StaticUpdateComplete, SaveStaticData);

			lock (MarketGroups)
			{
				FileManager.SerializeObject(MarketGroups);
			}

			lock (UniverseItems)
			{
				FileManager.SerializeObject(UniverseItems);
			}

			Debug.Log($"Static Data Saved!");
		}

		public static bool SaveMarketData()
		{
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

			lock (CorpOrderRecord)
			{
				FileManager.SerializeObject(CorpOrderRecord);
			}

			IsSubscribed = false;
			Debug.Log($"Market Data Saved.");
			return true;
		}

		public static bool UpdateNewRecordItems()
		{
			//Debug.Log($"Updating Market Objects With New Records!");

			if (ItemsToUpdate.Count > 0)
			{
				foreach (var typeId in ItemsToUpdate)
				{
					MarketItem item = GetMarketItem(typeId);
					item.UpdateMarketData();
				}

				ItemsToUpdate.Clear();
			}

			return true;
		}

		public static bool UpdateMarketObjects()
		{
			if (UpdateChangedRecordsOnly)
			{
				UpdateNewRecordItems();
			}
			else
			{
				//Debug.Log($"Updating All Market Objects!");
				lock (MarketObjects)
				{
					foreach (var marketObject in MarketObjects.Values)
					{
						marketObject.UpdateMaretData();
					}
				}
			}

			return true;
		}

		public static string EnumToString(Enum @enum)
		{
			string str = @enum.ToString().Replace("_", " ");
			return str;
		}
	}
}
