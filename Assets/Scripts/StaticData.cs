
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

namespace EveMarket
{
	public enum MarginStatus
	{
		Normal,
		High,
		Low
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

	public static class StaticData
	{
		private static StringBuilder sb = new StringBuilder();

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
			{ 530, "Mercoxit" }
		};

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

						//if (!OrderRecords[region].ContainsKey(type_id))
						//{
						//	OrderRecords[region][type_id] = new OrderRecord(marketOrders);
						//}
						//else
						//{
						//	OrderRecords[region][type_id].AddOrders(marketOrders);
						//}
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
				foreach (var regionOrders in OrderRecords.Values)
				{
					if (regionOrders.ContainsKey(typeId))
						regionOrders[typeId].ClearOrders();
				}
			}

			if (!OrderRecords.ContainsKey(NetworkSettings.SellRegion))
				OrderRecords[NetworkSettings.SellRegion] = new Dictionary<int, OrderRecord>();
			NetworkManager.AsyncRequest<List<MarketOrder>>(region: NetworkSettings.SellRegion, type_id: typeId);

			if (NetworkSettings.BuyRegion != NetworkSettings.SellRegion)
			{
				if (!OrderRecords.ContainsKey(NetworkSettings.BuyRegion))
					OrderRecords[NetworkSettings.BuyRegion] = new Dictionary<int, OrderRecord>();
				NetworkManager.AsyncRequest<List<MarketOrder>>(region: NetworkSettings.BuyRegion, type_id: typeId);
			}
		}

		private static void SaveStaticData()
		{
			Debug.Log($"SaveStaticData()");

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

			foreach (var marketObject in MarketObjects.Values)
			{
				marketObject.UpdateMarketData();
			}
		}
	}
}
