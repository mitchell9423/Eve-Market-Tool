using EveMarket.Network;
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
		const string UNIVERSE_TYPES_PATH = "Assets\\StaticData\\UniverseItems.json";
		const string MARKET_PRICES_PATH = "Assets\\StaticData\\MarketPrices.json";

		static readonly Dictionary<Type, string> TypeFilePath = new Dictionary<Type, string>()
		{
			{ typeof(MarketPrice), MARKET_PRICES_PATH },
			{ typeof(MarketGroup), MARKET_GROUPS_PATH },
			{ typeof(UniverseItem), UNIVERSE_TYPES_PATH }
		};

		private static string GetFilePath<T>()
		{
			return $"{Path.GetFullPath(@"./")}{TypeFilePath[typeof(T)]}";
		}

		public static void SerializeObject<T>(T item)
		{
			string path = GetFilePath<T>();

			string data = JsonConvert.SerializeObject(item, Formatting.Indented);

			Task.Run(() => WriteFile(path, data));
		}

		public static T DeserializeFromFile<T,Y>()
		{
			Type type = typeof(Y);

			string path = GetFilePath<Y>();

			string data = File.ReadAllText(path);

			return JsonConvert.DeserializeObject<T>(data);
		}

		static void WriteFile(string path, string data)
		{
			if (!File.Exists(path))
			{
				File.Create(path).Dispose();
			}

			File.WriteAllText(path, data);
		}
	}
}
