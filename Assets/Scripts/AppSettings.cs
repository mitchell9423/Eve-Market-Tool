using EveMarket.Util;
using System.Collections.Generic;
using UnityEngine;


namespace EveMarket
{
	public struct BuyPreset
	{
		public Region buyRegion;
		public System buySystem;
		public string buyRange;
		public int profitMargin;

		public BuyPreset(Region buyRegion, System buySystem, string buyRange, int profitMargin)
		{
			this.buyRegion = buyRegion;
			this.buySystem = buySystem;
			this.buyRange = buyRange;
			this.profitMargin = profitMargin;
		}
	}

	public class Settings
	{
		public bool EnableTimedUpdate { get; set; } = true;
		public int MarginPercentage { get; set; } = 15;
		public string BuyRange { get; set; } = "4";
		public System BuyOrderSystem { get; set; } = System.Tunttaras;
		public Region BuyRegion { get; set; } = Region.Lonetrek;
		public Region SellRegion { get; set; } = Region.The_Forge;
		public System ActivePreset { get; set; } = System.None;
	}

	public static class AppSettings
	{
		private enum Prefs
		{
			MarginPercentage,
			BuyRange,
			BuyOrderSystem,
			BuyRegion,
			SellRegion
		}

		public static Settings Settings { get; set; }

		//public static int MarginPercentage { get; set; } = 15;
		//public static string BuyRange { get; set; } = "4";
		//public static System BuyOrderSystem { get; set; } = System.Tunttaras;
		//public static Region BuyRegion { get; set; } = Region.Lonetrek;
		//public static Region SellRegion { get; set; } = Region.The_Forge;

		public static Dictionary<System, BuyPreset> Presets = new Dictionary<System, BuyPreset>()
		{
			{ System.Tunttaras, new BuyPreset(Region.Lonetrek, System.Tunttaras, "4", 10) },
			{ System.Umokka, new BuyPreset(Region.Lonetrek, System.Umokka, "3", 10) },
			{ System.Ylandoki, new BuyPreset(Region.Lonetrek, System.Ylandoki, "1", 10) },
			{ System.Inaro, new BuyPreset(Region.The_Citadel, System.Inaro, "3", 10) }
		};

		public static void LoadAppSettings()
		{
			Debug.Log($"Loading App Settings.");
			Settings = FileManager.DeserializeFromFile<Settings>();

			if (Settings == null)
			{
				Debug.LogWarning($"Creating new settings object.");
				Settings = new Settings();
			}

			EveMarket.SettingsLoadComplete?.Invoke();
			//MarginPercentage = PlayerPrefs.GetInt(MarginPercentage.ToString(), 15);
			//BuyRange = PlayerPrefs.GetString(BuyRange.ToString(), "station");
			//BuyOrderSystem = (System)PlayerPrefs.GetInt(BuyOrderSystem.ToString(), (int)System.Tunttaras);
			//BuyRegion = (Region)PlayerPrefs.GetInt(BuyRegion.ToString(), (int)Region.Lonetrek);
			//SellRegion = (Region)PlayerPrefs.GetInt(SellRegion.ToString(), (int)Region.The_Forge);
		}

		public static void SaveAppSettings()
		{
			FileManager.SerializeObject(Settings);
			//PlayerPrefs.SetInt(MarginPercentage.ToString(), MarginPercentage);
			//PlayerPrefs.SetString(BuyRange.ToString(), BuyRange);
			//PlayerPrefs.SetInt(BuyOrderSystem.ToString(), (int)BuyOrderSystem);
			//PlayerPrefs.SetInt(BuyRegion.ToString(), (int)BuyRegion);
			//PlayerPrefs.SetInt(SellRegion.ToString(), (int)SellRegion);
		}
	}
}
