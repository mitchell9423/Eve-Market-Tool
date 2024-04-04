using EveMarket;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EveMarket.UI
{
	public class ItemContianer : MonoBehaviour
	{
		[Header("Prefab Refs")]
		public UnityEngine.Object itemInfoPanelPrefab;

		[Header("UI Elements")]
		[SerializeField] private List<InfoPanel> infoPanels = new List<InfoPanel>();

		public void PopulateItemContainer(MarketObject marketObject)
		{
			List<MarketItem> items = null;
			lock (marketObject)
			{
				items = marketObject.Items;
				items.Sort((item1, item2) => item1.ItemName.CompareTo(item2.ItemName));
			}
			
			foreach (var item in items)
			{
				int index = 0;
				if (item.ItemName.Contains("Compressed")) continue;

				InfoPanel infoPanel;

				if ((index = infoPanels.FindIndex(panel => panel.GetName() == item.ItemName)) < 0)
				{
					GameObject obj = (GameObject)Instantiate(itemInfoPanelPrefab, transform);
					infoPanel = obj.GetComponent<InfoPanel>();
					infoPanels.Add(infoPanel);
				}
				else
				{
					infoPanel = infoPanels[index];
				}

				infoPanel.UpdateInfoPanel(item);
			}
		}
	}
}
