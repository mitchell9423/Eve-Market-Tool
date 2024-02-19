using EveMarket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Reprocess
{
	public struct MineralOutput
	{
		int Tritanium;
		int Pyerite;
		int Mexallon;
		int Isogen;
		int Nocxium;

		public MineralOutput(
			int Tritanium = 0, 
			int Pyerite = 0, 
			int Mexallon = 0,
			int Isogen = 0, 
			int Nocxium = 0
			)
		{
			this.Tritanium = Tritanium;
			this.Pyerite = Pyerite;
			this.Mexallon = Mexallon;
			this.Isogen = Isogen;
			this.Nocxium = Nocxium;
		}

		public double GetNetValue()
		{
			double netValue = 0;
			float netYield = CalcNetYield();

			foreach (var marketItem in StaticData.MarketObjects[1857].Items)
			{
				if (marketItem.ItemName == "Tritanium")
				{
					netValue += marketItem.CurrentSellPrice * Mathf.FloorToInt(Tritanium * netYield);
				}

				if (marketItem.ItemName == "Pyerite")
				{
					netValue += marketItem.CurrentSellPrice * Mathf.FloorToInt(Pyerite * netYield);
				}

				if (marketItem.ItemName == "Mexallon")
				{
					netValue += marketItem.CurrentSellPrice * Mathf.FloorToInt(Mexallon * netYield);
				}

				if (marketItem.ItemName == "Isogen")
				{
					netValue += marketItem.CurrentSellPrice * Mathf.FloorToInt(Mexallon * netYield);
				}

				if (marketItem.ItemName == "Nocxium")
				{
					netValue += marketItem.CurrentSellPrice * Mathf.FloorToInt(Mexallon * netYield);
				}
			}
			
			return netValue;
		}
	}

	static float baseYield = .54f;

	static int reprocessingSkillLvl = 5;
	static float reprocessingSkillPercentage = 1 + (.03f * reprocessingSkillLvl);

	static int reprocessingEfficiencySkillLvl = 5;
	static float reprocessingEfficiencySkillPercentage = 1 + (.02f * reprocessingEfficiencySkillLvl);

	// Only applied to Plag, Pyroxeres, Scordite, Veldspar, Mordunium.
	static int simpleOreProcessingSkillLvl = 4;
	static float simpleOreProcessingSkillPercentage = 1 + (.02f * simpleOreProcessingSkillLvl);

	static float implantsBonus = 1.04f;

	public static float CalcNetYield()
	{
		float netYield = baseYield;
		netYield *= reprocessingSkillPercentage;
		netYield *= reprocessingEfficiencySkillPercentage;
		netYield *= simpleOreProcessingSkillPercentage;
		netYield *= implantsBonus;
		netYield = Mathf.Ceil(netYield * 1000)/1000;
		return netYield;
	}

	public static Dictionary<string, MineralOutput> OreOutputs = new Dictionary<string, MineralOutput>()
	{
		{
			"Azure Plagioclase",
			new MineralOutput(
				Tritanium: 184,
				Mexallon: 74
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
			"Luminous Kernite",
			new MineralOutput(
				Mexallon: 63,
				Isogen: 126
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
			"Plagioclase",
			new MineralOutput(
				Tritanium: 175,
				Mexallon: 70
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
			"Solid Pyroxeres", 
			new MineralOutput(
				Pyerite: 95,
				Mexallon: 32
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
			return Mathf.Round((float)OreOutputs[item.ItemName].GetNetValue()*100) / 100;
		}

		return 0;
	}
}
