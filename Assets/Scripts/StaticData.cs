
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Text;
using EveMarket.Util;
using EveMarket.Network;
using System.Linq;

namespace EveMarket
{
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

		private static Dictionary<int, MarketPrice> marketPrices = new Dictionary<int, MarketPrice>();
		private static Dictionary<int, MarketGroup> groupObjects = new Dictionary<int, MarketGroup>();
		private static Dictionary<int, UniverseItem> itemObjects = new Dictionary<int, UniverseItem>();

		public static List<IDataModel> DataModels
		{
			get 
			{
				lock (itemObjects) 
				{
					return itemObjects.Values.Cast<IDataModel>().ToList();
				}
			}
		}

		public static void UpdateStaticData()
		{
			lock (sb) { sb.Clear(); }

			NetworkManager.AsyncRequest<MarketPrice>();

			foreach (var groupId in GroupIdsToName.Keys)
			{
				NetworkManager.AsyncRequest<MarketGroup>(groupId.ToString());
			}
		}

		public static void LoadStaticData()
		{
			lock (marketPrices)
			{
				marketPrices = FileManager.DeserializeFromFile<Dictionary<int, MarketPrice>, MarketPrice>();
			}

			lock (groupObjects)
			{
				groupObjects = FileManager.DeserializeFromFile<Dictionary<int, MarketGroup>, MarketGroup>();
			}

			lock (itemObjects)
			{
				itemObjects = FileManager.DeserializeFromFile<Dictionary<int, UniverseItem>, UniverseItem>();
			}

			EveDelegate.StaticLoadComplete?.Invoke();

			Debug.Log("Static Data Loaded.");
		}

		public static void HandleResponse<T>(string response) where T : IDataModel
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







			T objectModel = JsonConvert.DeserializeObject<T>(response);
			if (objectModel == null)
			{
				Debug.Log("Error receiving data.");
				return;
			}

			StringBuilder localSb = new StringBuilder();
			if (objectModel is MarketGroup marketGroup)
			{
				lock (groupObjects)
				{
					groupObjects[marketGroup.Id] = marketGroup;
					localSb.Append($"{groupObjects[marketGroup.Id].Id,5} : {groupObjects[marketGroup.Id].Name}\n");
				}

				foreach (var itemId in marketGroup.Types)
				{
					NetworkManager.AsyncRequest<UniverseItem>(itemId.ToString());
				}
			}
			else if (objectModel is UniverseItem universeItem)
			{
				lock (itemObjects)
				{
					itemObjects[universeItem.Id] = universeItem;
					localSb.Append($"{itemObjects[universeItem.Id].Id,5} : {itemObjects[universeItem.Id].Name}\n");
				}
			}

			lock (sb)
			{
				sb.Append(localSb);
			}

			try
			{


			}
			catch (Exception ex)
			{
				Debug.Log($"{ex}");
			}

			lock (sb)
			{
				str = sb.ToString();
			}

			UnityMainThreadDispatcher.Instance.Enqueue(() =>
			{
				DisplayPanel.text = str;
			});
		}
	}
}
