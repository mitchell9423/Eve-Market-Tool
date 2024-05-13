using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EveMarket.UI
{
	public class ItemContianer : MonoBehaviour
	{
		[Header("Prefab Refs")]
		public UnityEngine.Object itemInfoPanelPrefab;

		[Header("UI Elements")]
		[SerializeField] public List<InfoPanel> infoPanels = new List<InfoPanel>();

		public void PopulateItemContainer(MarketObject marketObject)
		{
			List<MarketItem> items = null;
			lock (marketObject)
			{
				items = marketObject.Items.Values.ToList();
				items.Sort((item1, item2) => item1.ItemName.CompareTo(item2.ItemName));
			}

			PopulateItemContainer(items, AppSettings.Presets[AppSettings.Settings.ActivePreset]);
		}

		public void PopulateItemContainer(List<MarketItem> items, BuyPreset buyPreset)
		{
			foreach (var item in items)
			{
				int index = 0;
				if (item.ItemName.Contains("Compressed")) continue;

				InfoPanel infoPanel;

				if ((index = infoPanels.FindIndex(panel => panel.GetName() == item.ItemName)) < 0)
				{
					GameObject obj = (GameObject)Instantiate(itemInfoPanelPrefab, transform);
					infoPanel = obj.GetComponent<InfoPanel>();
					infoPanel.GroupName = StaticData.MarketGroups[item.GroupId].Name;
					infoPanel.TypeId = item.TypeId;
					infoPanel.System = buyPreset.buySystem;
					infoPanel.Region = buyPreset.buyRegion;
					infoPanels.Add(infoPanel);
				}
				else
				{
					infoPanel = infoPanels[index];
				}

				infoPanel.UpdateInfoPanel(item);
			}
		}

		public void UpdateContainer()
		{
			foreach (var panel in infoPanels)
			{
				panel.UpdateInfoPanel();
			}
		}
	}
}
