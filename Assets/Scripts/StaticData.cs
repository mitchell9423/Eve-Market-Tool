using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System;
using GameEvent;
using System.Text;
using System.Threading;
using System.Linq;

namespace EveMarket
{
	public static class StaticData
	{
		private static int pendingOperations = 0;

		private static StringBuilder sb = new StringBuilder();

		const string MARKET_GROUP_URL = "https://esi.evetech.net/latest/markets/groups/";
		const string UNIVERSE_ITEMS_URL = "https://esi.evetech.net/latest/universe/types/";
		const string MARKET_GROUPS_PATH = "Assets\\StaticData\\MarketGroups.json";
		const string UNIVERSE_ITEMS_PATH = "Assets\\StaticData\\UniverseItems.json";

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

		private static Dictionary<int, IDataModel> marketObjects = new Dictionary<int, IDataModel>();
		private static Dictionary<int, IDataModel> items = new Dictionary<int, IDataModel>();

		public static void UpdateStaticData()
		{
			sb.Clear();

			foreach (var groupId in GroupIdsToName.Keys)
			{
				_ = GetDataAsync<MarketGroup>(groupId);
			}
		}

		private static async Task GetDataAsync<T>(int id) where T : IDataModel
		{
			Type type = typeof(T);

			string url = "";

			if (type == typeof(MarketGroup))
			{
				Debug.LogWarning($"Getting data for Market Group {id}");
				url = $"{MARKET_GROUP_URL}{id}";
			}
			else if (type == typeof(ItemInfo))
			{
				Debug.LogWarning($"Getting data for Universe Item {id}");
				url = $"{UNIVERSE_ITEMS_URL}{id}";
			}

			Interlocked.Increment(ref pendingOperations);
			Debug.LogWarning($"There are {pendingOperations} pending operations.");
			await HttpHandler.instance.GetAsync<T>(url, HandleResponse<T>);
		}

		private static void HandleResponse<T>(string response) where T : IDataModel
		{
			if (response != null)
			{
				try
				{
					T objectModel = JsonConvert.DeserializeObject<T>(response);

					lock (marketObjects)
					{
						marketObjects[objectModel.Id] = objectModel;

						lock (sb)
						{
							sb.Append($"{marketObjects[objectModel.Id].Id,5} : {marketObjects[objectModel.Id].Name}\n");
							DisplayPanel.text = sb.ToString();
						}
					}

					if (objectModel is MarketGroup marketGroup)
					{
						foreach (var itemId in marketGroup.Types)
						{
							_ = GetDataAsync<ItemInfo>(itemId);
						}
					}

				}
				catch (Exception ex)
				{
					lock (sb)
					{
						sb.Append($"{ex}\n");
						DisplayPanel.text = sb.ToString();
					}
				}
				//Debug.Log($"Received data: {response}");
			}
			else
			{
				Debug.Log("Error receiving data.");
			}

			CheckAndSave();
		}

		private static void CheckAndSave()
		{
			if (Interlocked.Decrement(ref pendingOperations) == 0)  // Decrement counter and check
			{
				SaveMarketObjectsToJson();
			}

			Debug.LogWarning($"There are {pendingOperations} pending operations.");
		}

		private static void SaveMarketObjectsToJson()
		{
			Debug.Log("SaveMarketObjectsToJson.");

			string path = Path.GetFullPath(@"./");

			Dictionary<int, MarketGroup> marketGroups = new Dictionary<int, MarketGroup>();
			Dictionary<int, ItemInfo> universeItems = new Dictionary<int, ItemInfo>();

			lock (marketObjects)
			{
				foreach (var item in marketObjects.Values)
				{
					if (item is MarketGroup marketGroup)
					{
						marketGroups[marketGroup.Id] = marketGroup;
					}
					else if (item is ItemInfo itemInfo)
					{
						universeItems[itemInfo.Id] = itemInfo;
					}
				}
			}

			Task.Run(() => WriteFiles(marketGroups, universeItems));
		}

		static void WriteFiles(Dictionary<int, MarketGroup>  marketGroups, Dictionary<int, ItemInfo>  universeItems)
		{
			string groupPath = GetMarketGroupPath();

			if (!File.Exists(groupPath))
			{
				File.Create(groupPath).Dispose();
			}

			File.WriteAllText(groupPath, JsonConvert.SerializeObject(marketGroups, Formatting.Indented));

			string itemPath = GetUniverseItemPath();

			if (!File.Exists(itemPath))
			{
				File.Create(itemPath).Dispose();
			}

			File.WriteAllText(itemPath, JsonConvert.SerializeObject(universeItems, Formatting.Indented));
		}

		private static string GetMarketGroupPath()
		{
			string path = Path.GetFullPath(@"./");
			path += $"{MARKET_GROUPS_PATH}";
			return path;
		}

		private static string GetUniverseItemPath()
		{
			string path = Path.GetFullPath(@"./");
			path += $"{UNIVERSE_ITEMS_PATH}";
			return path;
		}

		public static void LoadStaticData()
		{
			string path = GetMarketGroupPath();
			string file = File.ReadAllText(path);
			Dictionary<int, MarketGroup> marketDictionary = JsonConvert.DeserializeObject<Dictionary<int, MarketGroup>>(file);

			foreach (var item in marketDictionary)
			{
				marketObjects[item.Key] = item.Value;
			}

			path = GetUniverseItemPath();
			file = File.ReadAllText(path);
			Dictionary<int, ItemInfo> itemDictionary = JsonConvert.DeserializeObject<Dictionary<int, ItemInfo>>(file);

			foreach (var item in itemDictionary)
			{
				marketObjects[item.Key] = item.Value;
			}

			Debug.Log("Static Data Loaded.");
		}
	}
}
