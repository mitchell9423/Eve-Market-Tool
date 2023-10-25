using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Progress;

namespace EveMarket
{
	public static class FileManager
	{
		const string MARKET_GROUPS_PATH = "Assets\\StaticData\\MarketGroups.json";
		const string UNIVERSE_ITEMS_PATH = "Assets\\StaticData\\UniverseItems.json";

		private static string GetMarketGroupPath()
		{
			return $"{Path.GetFullPath(@"./")}{MARKET_GROUPS_PATH}";
		}

		private static string GetUniverseItemPath()
		{
			return $"{Path.GetFullPath(@"./")}{UNIVERSE_ITEMS_PATH}";
		}

		public static void SerializeObject<T>(T item)
		{
			string path = "";

			if (item is MarketGroup)
			{
				path = GetMarketGroupPath();
			}
			else
			{
				path = GetUniverseItemPath();
			}

			string data = JsonConvert.SerializeObject(item, Formatting.Indented);
			_ = WriteFile(path, data);
		}

		public static T DeserializeFromFile<T,Y>()
		{
			Type type = typeof(T);
			string path = "";

			if (type == typeof(MarketGroup))
			{
				path = GetMarketGroupPath();
			}
			else if (type == typeof(UniverseItem))
			{
				path = GetUniverseItemPath();
			}

			return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
		}

		static async Task WriteFile(string path, string data)
		{
			if (!File.Exists(path))
			{
				File.Create(path).Dispose();
			}

			File.WriteAllText(path, data);
		}
	}
}
