using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EveMarket
{
	public class MarketObject
	{
		public class MarketItem
		{
			UniverseItem item;
			MarketPrice price;

			public MarketItem(UniverseItem _item, MarketPrice _price)
			{
				item = _item;
				price = _price;
			}

			public string ItemName { get => item.Name; }
			public double AveragePrice { get => price.AveragePrice; }
		}

		public MarketGroup Group { get; private set; }
		public List<MarketItem> Items { get; private set; } = new List<MarketItem>();

		public string GroupName { get => Group.Name; }
		public int ItemCount { get => Items.Count; }

		public MarketObject(MarketGroup _group)
		{
			Group = _group;

			lock (StaticData.itemObjects)
			{
				lock (StaticData.marketPrices)
				{
					foreach (var itemId in Group.Types)
					{
						if (StaticData.itemObjects.ContainsKey(itemId) && StaticData.marketPrices.ContainsKey(itemId))
						{
							Items.Add(new MarketItem(StaticData.itemObjects[itemId], StaticData.marketPrices[itemId]));
						}
					}
				}
			}
		}

		public MarketItem GetItemByIndex(int index)
		{
			return Items[index];
		}
	}
}
