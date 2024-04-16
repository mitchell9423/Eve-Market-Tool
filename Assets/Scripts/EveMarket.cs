
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using EveMarket.Util;
using System.Text;
using System.Linq;
using EveMarket.UI;
using System;

namespace EveMarket
{
	public class EveMarket : MonoBehaviour
	{
		public ObjectType objectType = ObjectType.MarketGroup;

		[SerializeField] UnityMainThreadDispatcher unityMainThreadDispatcher;
		[SerializeField] HttpHandler httpHandler;
		[SerializeField] DisplayPanel ui;

		public delegate void AppEvent();
		public static AppEvent UpdateUI;
		public static AppEvent SettingsLoadComplete;

		public static bool EnableTimedUpdate { get; set; }
		public static bool ShowGUI { get; set; }
		private static DateTime LastUpdate { get; set; }
		public static TimeSpan TimerInterval { get; set; } = new TimeSpan(0, 10, 0);
		public static TimeSpan TimeSinceLastUpdate { get; set; }
		public static TimeSpan RemainingTime { get; set; }

		StringBuilder sb = new StringBuilder();

		private void Awake()
		{
			UpdateUI += ui.CreateGroupContainers;
			SettingsLoadComplete += UpdateSettings;
			EveDelegate.Subscribe(ref EveDelegate.ResetAutoUpdateTimer, ResetTimer);
			EveDelegate.Subscribe(ref EveDelegate.ItemMarketUpdateComplete, StaticData.SaveMarketData);

			if (!gameObject.TryGetComponent(out unityMainThreadDispatcher))
			{
				unityMainThreadDispatcher = gameObject.AddComponent<UnityMainThreadDispatcher>();
			}

			if (!gameObject.TryGetComponent(out httpHandler))
			{
				httpHandler = gameObject.AddComponent<HttpHandler>();
			}
		}

		private void OnDestroy()
		{
			UpdateUI -= ui.CreateGroupContainers;
			SettingsLoadComplete -= UpdateSettings;
			EveDelegate.Unsubscribe(ref EveDelegate.ResetAutoUpdateTimer, ResetTimer);
			EveDelegate.Unsubscribe(ref EveDelegate.ItemMarketUpdateComplete, StaticData.SaveMarketData);
		}

		private void Start()
		{
			Application.runInBackground = true;
			AppSettings.LoadAppSettings();
			LoadStaticData();

			lock (StaticData.MarketObjects)
			{
				var mos = StaticData.MarketObjects.Values.ToArray();

				for (int i = 0; i < StaticData.MarketObjects.Count; i++)
				{
					MarketObject mo = StaticData.MarketObjects.Values.ToArray()[i];

					lock (StaticData.GroupObjects)
					{
						StaticData.UpdateMarketData(StaticData.GroupObjects[mo.Group.TypeId].Types);
					}
				}
			}

			StartCoroutine(TimedUpdate());
		}

		private void Update()
		{
			TimeSinceLastUpdate = DateTime.Now - LastUpdate;
			RemainingTime = TimerInterval - TimeSinceLastUpdate;
		}

		public IEnumerator TimedUpdate()
		{
			if (EnableTimedUpdate)
			{
				lock (StaticData.OrderRecordExpirations)
				{
					var regions = StaticData.OrderRecordExpirations.Keys.ToArray();
					for (int i = 0; i < regions.Length; i++)
					{
						var itemTypes = StaticData.OrderRecordExpirations[regions[i]].Keys.ToArray();
						for (int j = 0; j < itemTypes.Length; j++)
						{
							if (DateTime.TryParse(StaticData.OrderRecordExpirations[regions[i]][itemTypes[j]], out DateTime expiration))
							{
								bool isExpired = DateTime.Now >= expiration;

								if (isExpired)
								{
									StaticData.UpdateItemMarketData(itemTypes[j]);
								}
							}

							yield return null;
						}
					}
				}

				//lock (StaticData.MarketObjects)
				//{
				//	var mos = StaticData.MarketObjects.Values.ToArray();

				//	for (int i = 0; i < StaticData.MarketObjects.Count; i++)
				//	{
				//		MarketObject mo = StaticData.MarketObjects.Values.ToArray()[i];

				//		lock (StaticData.GroupObjects)
				//		{
				//			StaticData.UpdateMarketData(StaticData.GroupObjects[mo.Group.TypeId].Types);
				//		}

				//		yield return null;
				//	}
				//}
			}

			yield return new WaitForSeconds(60);
			EveDelegate.ItemMarketUpdateComplete?.Invoke();
			StartCoroutine(TimedUpdate());
		}

		private void ResetTimer()
		{
			LastUpdate = DateTime.Now;
		}

		public void LoadStaticData()
		{
			StaticData.LoadStaticData();
			BuildDisplayString();
			ui.CreateGroupContainers();
		}

		public void UpdateStaticData()
		{
			StaticData.UpdateStaticData();
			BuildDisplayString();
		}

		public void UpdateMarketData(Component sender, object obj)
		{
			StaticData.UpdateMarketData(obj as List<int>);
			BuildDisplayString();
		}

		private void BuildDisplayString()
		{
			sb.Clear();

			for (int i = 0; i < StaticData.MarketObjects.Count; i++)
			{
				MarketObject marketObject = StaticData.MarketObjects.ElementAt(i).Value;

				sb.Append($"\nGroup: {marketObject.GroupName}\n");

				for (int j = 0; j < marketObject.ItemCount; j++)
				{
					MarketItem marketItem = marketObject.GetItemByIndex(j);

					sb.Append($"\n  {marketItem.ItemName}   Average Price: {marketItem.AveragePrice}");
				}

				sb.Append($"\n\n");
			}
		}

		private void UpdateSettings()
		{
			EnableTimedUpdate = AppSettings.Settings.EnableTimedUpdate;
			ui.SetProfitMargin();
		}
	}
}
