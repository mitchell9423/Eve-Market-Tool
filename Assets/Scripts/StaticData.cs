
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Text;
using EveMarket.Util;
using EveMarket.Network;
using System.Linq;
using System.Threading.Tasks;

namespace EveMarket
{
	public enum ObjectType
	{
		MarketPrice,
		MarketGroup,
		UniverseItem
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

		public static Dictionary<int, MarketPrice> marketPrices = new Dictionary<int, MarketPrice>();
		public static Dictionary<int, MarketGroup> groupObjects = new Dictionary<int, MarketGroup>();
		public static Dictionary<int, UniverseItem> itemObjects = new Dictionary<int, UniverseItem>();

		public static void UpdateStaticData()
		{
			EveDelegate.Subscribe(ref EveDelegate.StaticUpdateComplete, SaveStaticData);

			lock (sb) { sb.Clear(); }

			NetworkManager.AsyncRequest<List<MarketPrice>>();

			foreach (var groupId in GroupIdsToName.Keys)
			{
				NetworkManager.AsyncRequest<MarketGroup>(groupId.ToString());
			}
		}

		public static void LoadStaticData()
		{
			if (marketPrices != null)
			{
				lock (marketPrices)
				{
					marketPrices = FileManager.DeserializeFromFile<Dictionary<int, MarketPrice>>();
				}
			}

			if (groupObjects != null)
			{
				lock (groupObjects)
				{
					groupObjects = FileManager.DeserializeFromFile<Dictionary<int, MarketGroup>>();
				}
			}


			if (itemObjects != null)
			{
				lock (itemObjects)
				{
					itemObjects = FileManager.DeserializeFromFile<Dictionary<int, UniverseItem>>();
				}
			}

			Debug.Log("Static Data Loaded.");
		}

		public static void HandleResponse<T>(string response)
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
				else if (objectModel is List<MarketPrice> marketPriceArray)
				{
					lock (marketPrices)
					{
						for (int i = 0; i < marketPriceArray.Count; i++)
						{
							marketPrices[marketPriceArray[i].Id] = marketPriceArray[i];
							localSb.Append($"{marketPrices[marketPriceArray[i].Id].Id,5} : {marketPrices[marketPriceArray[i].Id].AveragePrice}\n");
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

			lock (sb)
			{
				str = sb.ToString();
			}

			UnityMainThreadDispatcher.Instance.Enqueue(() =>
			{
				Debug.Log(str);
			});

			HttpHandler.CompleteStaticUpdateTask();
		}

		private static void SaveStaticData()
		{
			Debug.Log($"SaveStaticData()");

			lock (groupObjects)
			{
				FileManager.SerializeObject(groupObjects);
			}

			lock (itemObjects)
			{
				FileManager.SerializeObject(itemObjects);
			}

			lock (marketPrices)
			{
				FileManager.SerializeObject(marketPrices);
			}

			EveDelegate.Unsubscribe(ref EveDelegate.StaticUpdateComplete, SaveStaticData);
		}
	}
}
