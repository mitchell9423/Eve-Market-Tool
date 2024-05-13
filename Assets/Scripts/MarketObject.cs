using System;
using System.Collections.Generic;
using System.Linq;

namespace EveMarket
{
	[Serializable]
	public class MarketObject
	{
		public int GroupId { get; set; }
		public MarketGroup Group => StaticData.GetMarketGroup(GroupId);
		public Dictionary<int, MarketItem> Items { get; private set; } = new Dictionary<int, MarketItem>();
		public string GroupName { get => Group.Name; }
		public int ItemCount { get => Items.Count; }
		public int[] GetMarketItemIds() => Items.Keys.ToArray();

		public MarketObject(MarketGroup marketGroup)
		{
			GroupId = marketGroup.TypeId;

			foreach (var typeId in marketGroup.Types)
			{
				if (StaticData.UniverseItems[typeId].Name.Contains("Batch") || StaticData.UniverseItems[typeId].Name.Contains("Compressed")) continue;

				Items[typeId] = StaticData.MarketItems[typeId] = new MarketItem(typeId);
			}
		}

		public void UpdateMaretData()
		{
			foreach (var item in Items.Values)
			{
				item.UpdateMarketData();
			}
		}

		public MarketItem GetItemByIndex(int index)
		{
			return Items[index];
		}
	}
}
