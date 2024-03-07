using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EveMarket
{
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

		public static int MarginPercentage { get; set; } = 10;
		public static string BuyRange { get; set; } = "4";
		public static System BuyOrderSystem { get; set; }
		public static Region BuyRegion { get; set; } = Region.Lonetrek;
		public static Region SellRegion { get; set; }

		public static void LoadPlayerPrefs()
		{
			MarginPercentage = PlayerPrefs.GetInt(MarginPercentage.ToString(), 15);
			BuyRange = PlayerPrefs.GetString(BuyRange.ToString(), "station");
			BuyOrderSystem = (System)PlayerPrefs.GetInt(BuyOrderSystem.ToString(), (int)System.Tunttaras);
			BuyRegion = (Region)PlayerPrefs.GetInt(BuyRegion.ToString(), (int)Region.Lonetrek);
			SellRegion = (Region)PlayerPrefs.GetInt(SellRegion.ToString(), (int)Region.The_Forge);
		}

		public static void SavePlayerPrefs()
		{
			PlayerPrefs.SetInt(MarginPercentage.ToString(), MarginPercentage);
			PlayerPrefs.SetString(BuyRange.ToString(), BuyRange);
			PlayerPrefs.SetInt(BuyOrderSystem.ToString(), (int)BuyOrderSystem);
			PlayerPrefs.SetInt(BuyRegion.ToString(), (int)BuyRegion);
			PlayerPrefs.SetInt(SellRegion.ToString(), (int)SellRegion);
		}
	}
}
