using EveMarket;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
	[SerializeField] private TMP_Text itemName;
	[SerializeField] private TMP_Text buyPrice;
	[SerializeField] private TMP_Text maxBuyPrice;
	[SerializeField] private TMP_Text sellPrice;

	private void Start()
	{
		itemName = transform.Find("Name").GetComponent<TMP_Text>();
		buyPrice = transform.Find("BuyPrice").GetComponent<TMP_Text>();
		maxBuyPrice = transform.Find("MaxBuyPrice").GetComponent<TMP_Text>();
		sellPrice = transform.Find("SellPrice").GetComponent<TMP_Text>();
	}

	public void UpdateInfoPanel(MarketItem item)
	{
		itemName.text = item.ItemName;
		buyPrice.text = $"Buy: {item.CurrentBuyPrice}";
		maxBuyPrice.text = $"Max Buy: {item.MaxBuyPrice}";
		sellPrice.text = StaticData.GroupObjects[item.GroupId].Name == "Minerals" || StaticData.GroupObjects[item.GroupId].Name == "Ice Products" ? $"Sell: {item.CurrentSellPrice}" : $"Sell: {item.ReprocessPrice}";
	}

	public string GetName()
	{
		return itemName.text;
	}
}
