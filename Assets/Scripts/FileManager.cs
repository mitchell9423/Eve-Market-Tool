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
		const string MARKET_GROUPS_PATH = "StaticData/MarketGroups.json";
		const string UNIVERSE_TYPES_PATH = "StaticData/UniverseItems.json";
		const string MARKET_PRICES_PATH = "StaticData/MarketPrices.json";
		const string CORP_ORDERS_PATH = "StaticData/CorpOrders.json";
		const string ITEM_ORDERS_PATH = "StaticData/ItemOrders.json";
		const string ROUTES_PATH = "StaticData/Routes.json";
		const string APP_SETTINGS = "StaticData/Settings.json";

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
			//string userDirectory = Application.persistentDataPath;

			if (!TypeFilePath.TryGetValue(typeof(T), out string relativePath))
			{
				TypeFilePath[typeof(T)] = $"StaticData/{typeof(T)}.json";
				Debug.LogWarning($"File path for type {typeof(T)} not found.");
			}

			//string filePath = Path.Combine(userDirectory, $"{path}");
			//Debug.LogWarning($"{typeof(T)} path: {filePath}");
			return EnsureFileAndDirectory(relativePath);
			//return $"{Path.GetFullPath(@"./")}{path}";
		}

		public static string EnsureFileAndDirectory(string relativePath)
		{
			// Combine persistentDataPath with the relative path
			string fullPath = Path.Combine(Application.persistentDataPath, relativePath);
			string directory = Path.GetDirectoryName(fullPath);

			// Create directory if it doesn't exist
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			// Create file if it doesn't exist
			if (!File.Exists(fullPath))
				File.WriteAllText(fullPath, ""); // Empty file

			return fullPath;
		}

		public static void SerializeObject<T>(T @object)
		{
			if (@object == null)
			{
				Debug.LogWarning($"The object you are trying to serialize is null.");
				return;
			}

			string data;

			lock (@object)
			{
				data = JsonConvert.SerializeObject(@object, Formatting.Indented);
			}

			UnityMainThreadDispatcher.Enqueue(() =>
			{
				string path = EnsureFileAndDirectory(GetFilePath<T>());

				Debug.LogWarning($"Saving {typeof(T)} @ {path}");

				if (string.IsNullOrEmpty(path)) { return; }

				Task.Run(() => WriteFile(path, data));
			});
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
