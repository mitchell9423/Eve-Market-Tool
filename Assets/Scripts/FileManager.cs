using EveMarket.Network;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using static EveMarket.Util.EveDelegate;
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
			{ typeof(Dictionary<int, MarketPrice>), MARKET_PRICES_PATH },
			{ typeof(Dictionary<int, MarketGroup>), MARKET_GROUPS_PATH },
			{ typeof(Dictionary<int, UniverseItem>), UNIVERSE_TYPES_PATH }
		};

		private static string GetFilePath<T>()
		{
			if (TypeFilePath.TryGetValue(typeof(T), out string path))
			{
				return $"{Path.GetFullPath(@"./")}{path}";
			}

			Debug.LogWarning($"File path for type {typeof(T).Name} not found.");

			return null;
		}

		public static void SerializeObject<T>(T @object)
		{
			if (@object == null)
			{
				Debug.LogWarning($"The object you are trying to serialize is null.");
				return;
			}

			string path = GetFilePath<T>();

			if (string.IsNullOrEmpty(path)) { return; }

			string data;

			lock (@object)
			{
				data = JsonConvert.SerializeObject(@object, Formatting.Indented);
			}

			Task.Run(() => WriteFile(path, data));
		}

		public static T DeserializeFromFile<T>() where T : class
		{
			string path = GetFilePath<T>();

			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			string data = ReadFile(path);
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

		static string ReadFile(string path)
		{
			if (!File.Exists(path))
			{
				File.Create(path).Dispose();
			}

			return File.ReadAllText(path);
		}
	}
}
