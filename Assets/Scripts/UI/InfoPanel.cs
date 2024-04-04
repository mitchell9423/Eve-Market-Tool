using EveMarket;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace EveMarket.UI
{
	public class InfoPanel : MonoBehaviour
	{
		[SerializeField] private TMP_Text itemName;
		[SerializeField] private TMP_Text buyPrice;
		[SerializeField] private TMP_Text maxBuyPrice;
		[SerializeField] private TMP_Text sellPrice;

		[SerializeField] private Color defaultColor;
		[SerializeField] private Color outdatedColor;
		[SerializeField] private Color completedColor;
		[SerializeField] private Color disabledColor;

		private List<TMP_Text> textObjects;

		private void Start()
		{
			itemName = transform.Find("Name").GetComponent<TMP_Text>();
			buyPrice = transform.Find("BuyPrice").GetComponent<TMP_Text>();
			maxBuyPrice = transform.Find("MaxBuyPrice").GetComponent<TMP_Text>();
			sellPrice = transform.Find("SellPrice").GetComponent<TMP_Text>();
			InitMat();
		}

		public void UpdateInfoPanel(MarketItem item)
		{
			itemName.text = item.ItemName;
			buyPrice.text = $"Buy: {item.CurrentBuyPrice}";
			maxBuyPrice.text = $"Max Buy: {item.MaxBuyPrice}";
			sellPrice.text = StaticData.GroupObjects[item.GroupId].Name == "Minerals" || StaticData.GroupObjects[item.GroupId].Name == "Ice Products" ? $"Sell: {item.CurrentSellPrice}" : $"Sell: {item.ReprocessPrice}";
			UpdateInfoColor(item);
		}

		public string GetName()
		{
			return itemName.text;
		}

		private void UpdateInfoColor(MarketItem item)
		{
			Image image = GetComponent<Image>();

			if (StaticData.MarketObjects[item.GroupId].GroupName == Group.Minerals.ToString()
				|| StaticData.MarketObjects[item.GroupId].GroupName == StaticData.EnumToString(Group.Ice_Products))
			{
				image.color = defaultColor;
				return;
			}

			if (item.CurrentBuyPrice == 0.00d)
			{
				image.color = disabledColor;
				return;
			}

			switch (item.ItemStatus)
			{
				case MarketItem.EItemStatus.Updated:
					image.color = defaultColor;
					break;
				case MarketItem.EItemStatus.Outdated:
					image.color = outdatedColor;
					break;
				case MarketItem.EItemStatus.Completed:
					image.color = completedColor;
					break;
				default:
					break;
			}
		}

		private void InitMat()
		{
			List<TMP_Text> textObjects = new List<TMP_Text>() { itemName, buyPrice, maxBuyPrice, sellPrice };

			for (int i = 0; i < textObjects.Count; i++)
			{
				Material mat = textObjects[i].materialForRendering;
				mat.SetFloat("_Stencil", 1);
				mat.SetFloat("_StencilOp", 0);
				mat.SetFloat("_StencilComp", 3);
				mat.SetFloat("_StencilWriteMask", 0);
				mat.SetFloat("_StencilReadMask", 1);
				mat.SetFloat("_ColorMask", 15);
				mat.SetFloat("_CullMode", 0);
			}
		}
	}
}
