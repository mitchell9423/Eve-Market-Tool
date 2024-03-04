using EveMarket.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

namespace EveMarket
{
	[Serializable]
	public class MarketObject
	{
		public MarketGroup Group { get; private set; }
		public List<MarketItem> Items { get; private set; } = new List<MarketItem>();

		public string GroupName { get => Group.Name; }
		public int ItemCount { get => Items.Count; }

		public MarketObject(MarketGroup _group)
		{
			Group = _group;
			foreach (var typeId in Group.Types)
			{
				lock (StaticData.ItemObjects)
				{
					lock (StaticData.MarketPrices)
					{
						if (StaticData.ItemObjects.ContainsKey(typeId) && StaticData.MarketPrices.ContainsKey(typeId))
						{
							lock (StaticData.OrderRecords)
							{
								foreach (var regionOrders in StaticData.OrderRecords.Values)
								{
									if (!regionOrders.ContainsKey(typeId)) regionOrders[typeId] = new OrderRecord(new List<MarketOrder>());
								}

								Items.Add(new MarketItem(StaticData.ItemObjects[typeId], StaticData.MarketPrices[typeId], StaticData.OrderRecords));
							}
						}
					}
				}
			}
		}

		public void UpdateMarketData()
		{
			for (int i = 0; i < Items.Count; i++)
			{
				Items[i].UpdateOrders();
			}
		}

		public MarketItem GetItemByIndex(int index)
		{
			return Items[index];
		}
	}
}
