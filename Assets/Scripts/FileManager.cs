using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace EveMarket.Util
{
	public static class FileManager
	{
		const string MARKET_GROUPS_PATH = "StaticData\\MarketGroups.json";
		const string UNIVERSE_TYPES_PATH = "StaticData\\UniverseItems.json";
		const string MARKET_PRICES_PATH = "StaticData\\MarketPrices.json";
		const string CORP_ORDERS_PATH = "StaticData\\CorpOrders.json";
		const string ITEM_ORDERS_PATH = "StaticData\\ItemOrders.json";
		const string ROUTES_PATH = "StaticData\\Routes.json";
		const string APP_SETTINGS = "StaticData\\Settings.json";

		static readonly Dictionary<Type, string> TypeFilePath = new Dictionary<Type, string>()
		{
			{ typeof(CorpOrderRecord), CORP_ORDERS_PATH },
			{ typeof(Dictionary<int, MarketPrice>), MARKET_PRICES_PATH },
			{ typeof(Dictionary<int, MarketGroup>), MARKET_GROUPS_PATH },
			{ typeof(Dictionary<int, UniverseItem>), UNIVERSE_TYPES_PATH },
			{ typeof(Dictionary<Region, Dictionary<int, OrderRecord>>), ITEM_ORDERS_PATH },
			{ typeof(Dictionary<int, List<RouteData>>), ROUTES_PATH },
			{ typeof(Settings), APP_SETTINGS }
		};

		private static string GetFilePath<T>()
		{
			string path;

			if (!TypeFilePath.TryGetValue(typeof(T), out path))
			{
				TypeFilePath[typeof(T)] = $"StaticData\\{typeof(T)}.json";
				Debug.LogWarning($"File path for type {typeof(T)} not found.");
			}

			return $"{Path.GetFullPath(@"./")}{path}";
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

			try
			{
				T deserializedObject = JsonConvert.DeserializeObject<T>(data);

				if (deserializedObject == null)
				{
					Debug.LogWarning($"Failed to deserialize {path} to type {typeof(T)}.");
				}

				return deserializedObject;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				return null;
			}
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
