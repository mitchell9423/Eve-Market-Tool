using EveMarket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

public static class Reprocess
{
	public struct MineralOutput
	{
		Dictionary<Mineral, int> BaseYield;

		public MineralOutput(
			int Tritanium = 0,
			int Pyerite = 0,
			int Mexallon = 0,
			int Isogen = 0,
			int Nocxium = 0,
			int Megacyte = 0,
			int Zydrine = 0,
			int Morphite = 0,
			int NeoJadarite = 0,
			int ChromodynamicTricarboxyls = 0
			)
		{
			BaseYield = new Dictionary<Mineral, int>();
			BaseYield[Mineral.Tritanium] = Tritanium;
			BaseYield[Mineral.Pyerite] = Pyerite;
			BaseYield[Mineral.Mexallon] = Mexallon;
			BaseYield[Mineral.Isogen] = Isogen;
			BaseYield[Mineral.Nocxium] = Nocxium;
			BaseYield[Mineral.Megacyte] = Megacyte;
			BaseYield[Mineral.Morphite] = Morphite;
			BaseYield[Mineral.Zydrine] = Zydrine;
			BaseYield[Mineral.NeoJadarite] = NeoJadarite;
			BaseYield[Mineral.ChromodynamicTricarboxyls] = ChromodynamicTricarboxyls;
		}

		public double GetNetValue(ReprocessType reprocessType)
		{
			double netValue = 0;
			float netYield = CalcNetYield(reprocessType);

			if (!StaticData.MarketObjects.ContainsKey(1857)) return 0;
			foreach (var marketItem in StaticData.MarketObjects[1857].Items)
			{
				string name = marketItem.ItemName;
				name = name.Replace("-", "");
				name = name.Replace(" ", "");
				Mineral mineral = (Mineral)Enum.Parse(typeof(Mineral), name);

				netValue += marketItem.CurrentSellPrice * Mathf.FloorToInt(BaseYield[mineral] * netYield);
			}
			
			return netValue;
		}
	}

	static float baseYield = .54f;

	static int reprocessingSkillLvl = 5;
	static float ReprocessingSkillPercentage = 1 + (.03f * reprocessingSkillLvl);

	static int reprocessingEfficiencySkillLvl = 5;
	static float ReprocessingEfficiencySkillPercentage = 1 + (.02f * reprocessingEfficiencySkillLvl);

	// 2% bonus to Bezdnacine, Rakovene, and Talassonite reprocessing yield per skill level.
	static int AbyssalOreProcessingSkillLvl = 0;
	static float AbyssalOreProcessingSkillPercentage = 1 + (.02f * AbyssalOreProcessingSkillLvl);

	// 2% bonus to Hedbergite, Hemorphite, Jaspet, Kernite, Omber, and Ytirium reprocessing yield per skill level.
	static int CoherentOreProcessingSkillLvl = 1;
	static float CoherentOreProcessingSkillPercentage = 1 + (.02f * CoherentOreProcessingSkillLvl);

	// 2% bonus to Cobaltite, Euxenite, Titanite, and Scheelite reprocessing yield per skill level.
	static int CommonOreProcessingSkillLvl = 0;
	static float CommonOreProcessingSkillPercentage = 1 + (.02f * CommonOreProcessingSkillLvl);

	// 2% bonus to Arkonor, Bistot, Spodumain, Eifyrium, and Ducinium reprocessing yield per skill level.
	static int ComplexOreProcessingSkillLvl = 0;
	static float ComplexOreProcessingSkillPercentage = 1 + (.02f * ComplexOreProcessingSkillLvl);

	// 2% bonus to Xenotime, Monazite, Loparite, and Ytterbite reprocessing yield per skill level.
	static int ExceptionalOreProcessingSkillLvl = 0;
	static float ExceptionalOreProcessingSkillPercentage = 1 + (.02f * ExceptionalOreProcessingSkillLvl);

	// 2% bonus to Mercoxite reprocessing yield per skill level.
	static int MercoxiteOreProcessingSkillLvl = 4;
	static float MercoxiteOreProcessingSkillPercentage = 1 + (.02f * MercoxiteOreProcessingSkillLvl);

	// 2% bonus to Carnotite, Zircon, Pollucite, and Cinnabar reprocessing yield per skill level.
	static int RareOreProcessingSkillLvl = 0;
	static float RareOreProcessingSkillPercentage = 1 + (.02f * RareOreProcessingSkillLvl);

	// 2% bonus to Plagioclase, Pyroxeres, Scordite, Veldspar, and Mordunium reprocessing yield per skill level.
	static int SimpleOreProcessingSkillLvl = 4;
	static float SimpleOreProcessingSkillPercentage = 1 + (.02f * SimpleOreProcessingSkillLvl);

	// 2% bonus to Zeolites, Sylvite, Bitumens, and Coesite reprocessing yield per skill level.
	static int UbiquitousOreProcessingSkillLvl = 0;
	static float UbiquitousOreProcessingSkillPercentage = 1 + (.02f * UbiquitousOreProcessingSkillLvl);

	// 2% bonus to Otavite, Sperrylite, Vanadinite, and Chromite reprocessing yield per skill level.
	static int UncommonOreProcessingSkillLvl = 0;
	static float UncommonOreProcessingSkillPercentage = 1 + (.02f * UncommonOreProcessingSkillLvl);

	// 2% bonus to Crokite, Dark Ochre, and Gneiss reprocessing yield per skill level.
	static int VariegatedOreProcessingSkillLvl = 1;
	static float VariegatedOreProcessingSkillPercentage = 1 + (.02f * VariegatedOreProcessingSkillLvl);

	static float implantsBonus = 1.04f;

	public static float CalcNetYield(ReprocessType reprocessType)
	{
		float netYield = baseYield;
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
		netYield = Mathf.Ceil(netYield * 1000)/1000;
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
		}
	};

	public static double CalcReprocessedValue(MarketItem item)
	{
		if (OreOutputs.ContainsKey(item.ItemName))
		{
			float val = (float)OreOutputs[item.ItemName].GetNetValue(item.ReprocessType);
			val = Mathf.Round(val);
			return val / 100;
		}

		return 0;
	}
}
