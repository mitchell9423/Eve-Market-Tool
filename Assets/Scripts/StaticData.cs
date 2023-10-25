using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System;
using GameEvent;
using System.Text;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace EveMarket
{
	public static class StaticData
	{
		private static int pendingOperations = 0;

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

		private static Dictionary<int, MarketGroup> groupObjects = new Dictionary<int, MarketGroup>();
		private static Dictionary<int, UniverseItem> itemObjects = new Dictionary<int, UniverseItem>();

		public static void UpdateStaticData()
		{
			sb.Clear();

			foreach (var groupId in GroupIdsToName.Keys)
			{
				_ = NetworkManager.RequestGroupInfoAsync(groupId);
			}
		}

		public static void LoadStaticData()
		{
			groupObjects = FileManager.DeserializeFromFile<Dictionary<int, MarketGroup>, MarketGroup>();
			itemObjects = FileManager.DeserializeFromFile<Dictionary<int, UniverseItem>, UniverseItem>();

			Debug.Log("Static Data Loaded.");
		}

		public static void HandleResponse<T>(string response) where T : IDataModel
		{
			if (response != null)
			{
				try
				{
					T objectModel = JsonConvert.DeserializeObject<T>(response);

					if (objectModel is MarketGroup marketGroup)
					{
						lock (groupObjects) 
						{ 
							groupObjects[marketGroup.Id] = marketGroup;
							lock (sb) { sb.Append($"{groupObjects[marketGroup.Id].Id,5} : {groupObjects[marketGroup.Id].Name}\n"); } 
						}

						foreach (var itemId in marketGroup.Types)
						{
							_ = NetworkManager.RequestItemInfoAsync(itemId);
						}
					}
					else if (objectModel is UniverseItem universeItem)
					{
						lock (itemObjects) 
						{ 
							itemObjects[universeItem.Id] = universeItem;
							lock (sb) { sb.Append($"{itemObjects[universeItem.Id].Id,5} : {itemObjects[universeItem.Id].Name}\n"); } 
						}
					}

					string str = "";
					lock (sb) { str = sb.ToString(); }
					Task.Run(() => 
					{
						lock (DisplayPanel.text)
						{
							DisplayPanel.text = str;
						}
					});

				}
				catch (Exception ex)
				{
					lock (sb)
					{
						sb.Append($"{ex}\n");
						DisplayPanel.text = sb.ToString();
					}
				}
			}
			else
			{
				Debug.Log("Error receiving data.");
			}
		}
	}
}
