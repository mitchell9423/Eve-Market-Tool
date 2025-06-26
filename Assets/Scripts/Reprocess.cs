using System;
using System.Collections.Generic;
using UnityEngine;


namespace EveMarket.Util
{
	public static class Reprocess
	{
		public struct MineralOutput
		{
			Dictionary<Product, int> BaseYield;

			public MineralOutput(
				int HeavyWater = 0,
				int LiquidOzone = 0,
				int StrontiumClathrates = 0,
				int HeliumIsotopes = 0,
				int NitrogenIsotopes = 0,
				int OxygenIsotopes = 0,
				int HydrogenIsotopes = 0,
				int Tritanium = 0,
				int Pyerite = 0,
				int Mexallon = 0,
				int Isogen = 0,
				int Nocxium = 0,
				int Megacyte = 0,
				int Zydrine = 0,
				int Morphite = 0,
				int NeoJadarite = 0,
				int ChromodynamicTricarboxyls = 0,
				int Eleutrium = 0
				)
			{
				BaseYield = new Dictionary<Product, int>();
				BaseYield[Product.HeavyWater] = HeavyWater;
				BaseYield[Product.LiquidOzone] = LiquidOzone;
				BaseYield[Product.StrontiumClathrates] = StrontiumClathrates;
				BaseYield[Product.HeliumIsotopes] = HeliumIsotopes;
				BaseYield[Product.NitrogenIsotopes] = NitrogenIsotopes;
				BaseYield[Product.OxygenIsotopes] = OxygenIsotopes;
				BaseYield[Product.HydrogenIsotopes] = HydrogenIsotopes;
				BaseYield[Product.Tritanium] = Tritanium;
				BaseYield[Product.Pyerite] = Pyerite;
				BaseYield[Product.Mexallon] = Mexallon;
				BaseYield[Product.Isogen] = Isogen;
				BaseYield[Product.Nocxium] = Nocxium;
				BaseYield[Product.Megacyte] = Megacyte;
				BaseYield[Product.Morphite] = Morphite;
				BaseYield[Product.Zydrine] = Zydrine;
				BaseYield[Product.NeoJadarite] = NeoJadarite;
				BaseYield[Product.ChromodynamicTricarboxyls] = ChromodynamicTricarboxyls;
				BaseYield[Product.Eleutrium] = Eleutrium;
			}

			public double GetNetValue(ReprocessType reprocessType)
			{
				double netValue = 0;
				float netYield = CalcNetYield(reprocessType);

				if (!StaticData.MarketObjects.ContainsKey(1857)) return 0;

				var productTypeID = reprocessType == ReprocessType.Ice ? 1033 : 1857;

				if (StaticData.MarketObjects.ContainsKey(productTypeID))
				{
					foreach (var marketItem in StaticData.MarketObjects[productTypeID].Items.Values)
					{
						string name = marketItem.ItemName;
						name = name.Replace("-", "");
						name = name.Replace(" ", "");
						Product product = (Product)Enum.Parse(typeof(Product), name);

						if (BaseYield[product] > 0)
						{
							float amountProduct = BaseYield[product];
							float netProduct = Mathf.Floor(BaseYield[product] * netYield);
							double netPrice = marketItem.CurrentSellPrice[AppSettings.Settings.SellRegion] * Mathf.Round(BaseYield[product] * netYield);
							netValue += marketItem.CurrentSellPrice[AppSettings.Settings.SellRegion] * Mathf.Floor(BaseYield[product] * netYield);
						}
					}
				}

				return netValue;
			}
		}

		//static float baseYield = .54f;

		public static int ReprocessingSkillLvl = 5;
		static float ReprocessingSkillPercentage => 1 + (.03f * ReprocessingSkillLvl);

		public static int ReprocessingEfficiencySkillLvl = 5;
		static float ReprocessingEfficiencySkillPercentage => 1 + (.02f * ReprocessingEfficiencySkillLvl);

		public static int ScrapmetalProcessingSkillLvl = 4;
		static float ScrapmetalProcessingSkillPercentage => 1 + (.02f * ScrapmetalProcessingSkillLvl);

		public static int IceProcessingSkillLvl = 5;
		static float IceProcessingSkillPercentage => 1 + (.02f * IceProcessingSkillLvl);

		// 2% bonus to Bezdnacine, Rakovene, and Talassonite reprocessing yield per skill level.
		public static int AbyssalOreProcessingSkillLvl = 0;
		static float AbyssalOreProcessingSkillPercentage => 1 + (.02f * AbyssalOreProcessingSkillLvl);

		// 2% bonus to Hedbergite, Hemorphite, Jaspet, Kernite, Omber, and Ytirium reprocessing yield per skill level.
		public static int CoherentOreProcessingSkillLvl = 4;
		static float CoherentOreProcessingSkillPercentage => 1 + (.02f * CoherentOreProcessingSkillLvl);

		// 2% bonus to Cobaltite, Euxenite, Titanite, and Scheelite reprocessing yield per skill level.
		public static int CommonOreProcessingSkillLvl = 0;
		static float CommonOreProcessingSkillPercentage => 1 + (.02f * CommonOreProcessingSkillLvl);

		// 2% bonus to Arkonor, Bistot, Spodumain, Eifyrium, and Ducinium reprocessing yield per skill level.
		public static int ComplexOreProcessingSkillLvl = 0;
		static float ComplexOreProcessingSkillPercentage => 1 + (.02f * ComplexOreProcessingSkillLvl);

		// 2% bonus to Xenotime, Monazite, Loparite, and Ytterbite reprocessing yield per skill level.
		public static int ExceptionalOreProcessingSkillLvl = 0;
		static float ExceptionalOreProcessingSkillPercentage => 1 + (.02f * ExceptionalOreProcessingSkillLvl);

		// 2% bonus to Mercoxite reprocessing yield per skill level.
		public static int MercoxiteOreProcessingSkillLvl = 5;
		static float MercoxiteOreProcessingSkillPercentage => 1 + (.02f * MercoxiteOreProcessingSkillLvl);

		// 2% bonus to Carnotite, Zircon, Pollucite, and Cinnabar reprocessing yield per skill level.
		public static int RareOreProcessingSkillLvl = 0;
		static float RareOreProcessingSkillPercentage => 1 + (.02f * RareOreProcessingSkillLvl);

		// 2% bonus to Plagioclase, Pyroxeres, Scordite, Veldspar, and Mordunium reprocessing yield per skill level.
		public static int SimpleOreProcessingSkillLvl = 5;
		static float SimpleOreProcessingSkillPercentage => 1 + (.02f * SimpleOreProcessingSkillLvl);

		// 2% bonus to Zeolites, Sylvite, Bitumens, and Coesite reprocessing yield per skill level.
		public static int UbiquitousOreProcessingSkillLvl = 0;
		static float UbiquitousOreProcessingSkillPercentage => 1 + (.02f * UbiquitousOreProcessingSkillLvl);

		// 2% bonus to Otavite, Sperrylite, Vanadinite, and Chromite reprocessing yield per skill level.
		public static int UncommonOreProcessingSkillLvl = 0;
		static float UncommonOreProcessingSkillPercentage => 1 + (.02f * UncommonOreProcessingSkillLvl);

		// 2% bonus to Crokite, Dark Ochre, and Gneiss reprocessing yield per skill level.
		public static int VariegatedOreProcessingSkillLvl = 4;
		static float VariegatedOreProcessingSkillPercentage => 1 + (.02f * VariegatedOreProcessingSkillLvl);

		static float implantsBonus = 1.04f;

		public static float CalcNetYield(ReprocessType reprocessType)
		{
			float netYield = AppSettings.Settings.BaseYield;
			netYield *= ReprocessingSkillPercentage;
			netYield *= ReprocessingEfficiencySkillPercentage;

			switch (reprocessType)
			{
				case ReprocessType.None:
					break;
				case ReprocessType.Abyssal:
					netYield *= AbyssalOreProcessingSkillPercentage;
					break;
				case ReprocessType.Coherent:
					netYield *= CoherentOreProcessingSkillPercentage;
					break;
				case ReprocessType.Common:
					netYield *= CommonOreProcessingSkillPercentage;
					break;
				case ReprocessType.Complex:
					netYield *= ComplexOreProcessingSkillPercentage;
					break;
				case ReprocessType.Exceptional:
					netYield *= ExceptionalOreProcessingSkillPercentage;
					break;
				case ReprocessType.Ice:
					netYield *= IceProcessingSkillPercentage;
					break;
				case ReprocessType.Mercoxit:
					netYield *= MercoxiteOreProcessingSkillPercentage;
					break;
				case ReprocessType.Rare:
					netYield *= RareOreProcessingSkillPercentage;
					break;
				case ReprocessType.Simple:
					netYield *= SimpleOreProcessingSkillPercentage;
					break;
				case ReprocessType.Ubiquitous:
					netYield *= UbiquitousOreProcessingSkillPercentage;
					break;
				case ReprocessType.Uncommon:
					netYield *= UncommonOreProcessingSkillPercentage;
					break;
				case ReprocessType.Variegated:
					netYield *= VariegatedOreProcessingSkillPercentage;
					break;
				default:
					break;
			}

			netYield *= implantsBonus;
			netYield = Mathf.Ceil(netYield * 1000) / 1000;
			return netYield;
		}

		public static Dictionary<string, MineralOutput> OreOutputs = new Dictionary<string, MineralOutput>()
	{
		{
			"Abyssal Bezdnacine",
			new MineralOutput(
				Tritanium: 42000,
				Isogen: 5040,
				Megacyte: 134
				)
		},
		{
			"Abyssal Rakovene",
			new MineralOutput(
				Tritanium: 42000,
				Isogen: 3360,
				Zydrine: 210
				)
		},
		{
			"Abyssal Talassonite",
			new MineralOutput(
				Tritanium: 42000,
				Nocxium: 1008,
				Megacyte: 34
				)
		},
		{
			"Azure Plagioclase",
			new MineralOutput(
				Tritanium: 184,
				Mexallon: 74
				)
		},
		{
			"Bezdnacine",
			new MineralOutput(
				Tritanium: 40000,
				Isogen: 4800,
				Megacyte: 128
				)
		},
		{
			"Concentrated Veldspar",
			new MineralOutput(
				Tritanium: 420
				)
		},
		{
			"Condensed Scordite",
			new MineralOutput(
				Tritanium: 158,
				Pyerite: 95
				)
		},
		{
			"Crokite",
			new MineralOutput(
				Pyerite: 800,
				Mexallon: 2000,
				Nocxium: 800
				)
		},
		{
			"Crystalline Crokite",
			new MineralOutput(
				Pyerite: 880,
				Mexallon: 2200,
				Nocxium: 880
				)
		},
		{
			"Dense Veldspar",
			new MineralOutput(
				Tritanium: 440
				)
		},
		{
			"Fiery Kernite",
			new MineralOutput(
				Mexallon: 66,
				Isogen: 132
				)
		},
		{
			"Golden Omber",
			new MineralOutput(
				Pyerite: 99,
				Isogen: 83
				)
		},
		{
			"Hadal Bezdnacine",
			new MineralOutput(
				Tritanium: 44000,
				Isogen: 5280,
				Megacyte: 141
				)
		},
		{
			"Hadal Rakovene",
			new MineralOutput(
				Tritanium: 44000,
				Isogen: 3520,
				Zydrine: 220
				)
		},
		{
			"Hadal Talassonite",
			new MineralOutput(
				Tritanium: 44000,
				Nocxium: 1056,
				Megacyte: 35
				)
		},
		{
			"Hiemal Tricarboxyl Vapor",
			new MineralOutput(
				ChromodynamicTricarboxyls: 1
				)
		},
		{
			"Luminous Kernite",
			new MineralOutput(
				Mexallon: 63,
				Isogen: 126
				)
		},
		{
			"Kernite",
			new MineralOutput(
				Mexallon: 60,
				Isogen: 120
				)
		},
		{
			"Massive Scordite",
			new MineralOutput(
				Tritanium: 165,
				Pyerite: 99
				)
		},
		{
			"Mordunium",
			new MineralOutput(
				Pyerite: 80
				)
		},
		{
			"Nesosilicate Rakovene",
			new MineralOutput(
				Tritanium: 1500,
				Isogen: 120,
				Zydrine: 7,
				NeoJadarite: 65
				)
		},
		{
			"Omber",
			new MineralOutput(
				Pyerite: 90,
				Isogen: 75
				)
		},
		{
			"Plagioclase",
			new MineralOutput(
				Tritanium: 175,
				Mexallon: 70
				)
		},
		{
			"Plum Mordunium",
			new MineralOutput(
				Pyerite: 84
				)
		},
		{
			"Plunder Mordunium",
			new MineralOutput(
				Pyerite: 92
				)
		},
		{
			"Prize Mordunium",
			new MineralOutput(
				Pyerite: 88
				)
		},
		{
			"Pyroxeres",
			new MineralOutput(
				Pyerite: 90,
				Mexallon: 30
				)
		},
		{
			"Radiant Hemorphite",
			new MineralOutput(
				Isogen: 264,
				Nocxium: 99
				)
		},
		{
			"Rakovene",
			new MineralOutput(
				Tritanium: 40000,
				Isogen: 3200,
				Zydrine: 200
				)
		},
		{
			"Rich Plagioclase",
			new MineralOutput(
				Tritanium: 193,
				Mexallon: 77
				)
		},
		{
			"Scordite",
			new MineralOutput(
				Tritanium: 150,
				Pyerite: 90
				)
		},
		{
			"Sharp Crokite",
			new MineralOutput(
				Pyerite: 840,
				Mexallon: 2100,
				Nocxium: 840
				)
		},
		{
			"Silvery Omber",
			new MineralOutput(
				Pyerite: 95,
				Isogen: 79
				)
		},
		{
			"Solid Pyroxeres",
			new MineralOutput(
				Pyerite: 95,
				Mexallon: 32
				)
		},
		{
			"Talassonite",
			new MineralOutput(
				Tritanium: 40000,
				Nocxium: 960,
				Megacyte: 32
				)
		},
		{
			"Veldspar",
			new MineralOutput(
				Tritanium: 400
				)
		},
		{
			"Viscous Pyroxeres",
			new MineralOutput(
				Pyerite: 99,
				Mexallon: 33
				)
		},
		{
			"White Glaze",
			new MineralOutput(
				HeavyWater: 69,
				LiquidOzone: 35,
				StrontiumClathrates: 1,
				NitrogenIsotopes: 414
				)
		}
	};

		public static double CalcReprocessedValue(MarketItem item)
		{
			if (item == null) return 0;

			if (OreOutputs.ContainsKey(item.ItemName))
			{
				float val = (float)OreOutputs[item.ItemName].GetNetValue(item.ReprocessType);

				if (item.ReprocessType != ReprocessType.Ice)
				{
					val /= 100;
				}

				val = Mathf.Round(val * 100) / 100;
				val -= (val * AppSettings.Settings.ReprocessTax);
				return val;
			}

			return 0;
		}
	}
}
