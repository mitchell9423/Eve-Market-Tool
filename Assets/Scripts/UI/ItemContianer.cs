using EveMarket;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemContianer : MonoBehaviour
{
	[Header("Prefab Refs")]
	public UnityEngine.Object itemInfoPanelPrefab;

	[Header("UI Elements")]
	[SerializeField] private List<InfoPanel> infoPanels = new List<InfoPanel>();

	public void PopulateItemContainer(MarketObject marketObject)
	{
		foreach (var item in marketObject.Items)
		{
			int index = 0;
			if (item.ItemName.Contains("Compressed")) continue;

			InfoPanel infoPanel;

			if ((index = infoPanels.FindIndex(panel => panel.GetName() == item.ItemName)) < 0)
			{
				GameObject obj = PrefabUtility.InstantiatePrefab(itemInfoPanelPrefab, transform) as GameObject;
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
