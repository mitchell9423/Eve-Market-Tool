using EveMarket.Util;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace EveMarket.UI
{
	public class InfoPanel : MonoBehaviour
	{
		[SerializeField] private TMP_Text itemName;
		[SerializeField] private TMP_Text buyLabel;
		[SerializeField] private TMP_InputField buyPrice;
		[SerializeField] private TMP_Text maxBuyPrice;
		[SerializeField] private TMP_Text sellPrice;

		[SerializeField] private Color defaultColor;
		[SerializeField] private Color outdatedColor;
		[SerializeField] private Color completedColor;
		[SerializeField] private Color disabledColor;

		private List<TMP_Text> textObjects;
		public Region Region { get; set; }
		public System System { get; set; }
		public int TypeId { get; set; }
		public string GroupName { get; set; }

		private void Start()
		{
			EveDelegate.Subscribe(ref EveDelegate.UpdateUINotify, UpdateUIHandler);
			itemName = transform.Find("Name").GetComponent<TMP_Text>();
			buyPrice = transform.Find("BuyPrice").GetComponent<TMP_InputField>();
			maxBuyPrice = transform.Find("MaxBuyPrice").GetComponent<TMP_Text>();
			sellPrice = transform.Find("SellPrice").GetComponent<TMP_Text>();
			InitMat();
		}

		private void OnDestroy()
		{
			EveDelegate.Unsubscribe(ref EveDelegate.UpdateUINotify, UpdateUIHandler);
		}

		//private void Update()
		//{
		//	UpdateInfoPanel();
		//}

		public void UpdateUIHandler(int typeId)
		{
			if (typeId == 0 || typeId == TypeId)
			{
				UpdateInfoPanel();
			}
		}

		public void UpdateInfoPanel(MarketItem item = null)
		{
			if (item == null)
			{
				if (TypeId <= 0) return;
				item = StaticData.GetMarketItem(TypeId);
			}

			itemName.text = item.ItemName;

			if (GroupName == Group.Minerals.ToString()
				|| GroupName == StaticData.EnumToString(Group.Ice_Products))
			{
				if (buyPrice != null) buyPrice.gameObject.SetActive(false);
				buyLabel.text = "";
				maxBuyPrice.text = $"";

				CorpOrder corpOrder = StaticData.CorpOrderRecord.CorpOrders.Find(order => order.TypeId == TypeId);
				if (corpOrder != null)
				{
					buyLabel.text = $"Order";
					maxBuyPrice.text = $"{corpOrder.Price}";
				}
			}
			else
			{
				if (!item.CurrentBuyPrice[Region].ContainsKey(System))
                {
					item.CurrentBuyPrice[Region][System] = 0;
				}

				buyPrice.text = $"{item.CurrentBuyPrice[Region][System]}";
				maxBuyPrice.text = $"Max Buy: {item.MaxBuyPrice}";
			}

			sellPrice.text = GroupName == "Minerals" || GroupName == "Ice Products" ? $"Sell: {item.CurrentSellPrice[AppSettings.Settings.SellRegion]}" : $"Sell: {item.ReprocessPrice[AppSettings.Settings.SellRegion]}";
			UpdateInfoColor(item);
		}

		public string GetName()
		{
			return itemName.text;
		}

		private void UpdateInfoColor(MarketItem item)
		{
			Image image = GetComponent<Image>();

			if (GroupName == Group.Minerals.ToString()
				|| GroupName == StaticData.EnumToString(Group.Ice_Products))
			{
				image.color = defaultColor;
				return;
			}

			if (item.CurrentBuyPrice[Region][System] == 0.00d)
			{
				image.color = disabledColor;
				return;
			}

			switch (item.ItemStatus[Region][System])
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
			List<TMP_Text> textObjects = new List<TMP_Text>() { itemName, maxBuyPrice, sellPrice };
			if (buyPrice != null) textObjects.AddRange(buyPrice.GetComponentsInChildren<TMP_Text>());

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
