
using EveMarket.StateMachine;
using EveMarket.Util;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EveMarket.UI
{
	public class DisplayPanel : MonoBehaviour
	{
		public enum PanelState
		{
			SystemView,
			RegionView
		}

		private struct LayoutPropertyBlock
		{
			// Item block widths
			//public int nameWidth;
			//public int spacer;
			//public int labelWidth;
			//public int priceWidth;
			//public int compressedLabelWidth;

			// Define the number of rows and columns
			//public int index;
			//public int numObjects;

			//public int blockWidth;
			//public int columns;
			//public int rows;

			// Define Label Texts
			//public string priceLabel;
			/*
			public void SetRowCol()
			{
				columns = Mathf.FloorToInt((float)Screen.width / (blockWidth + 20));
				rows = numObjects > 0 ? Mathf.CeilToInt((float)numObjects / columns) : 0;
			}
			*/
		}

		[Header("Prefab Refs")]
		public UnityEngine.Object groupContainerPrefab;

		[Header("UI Elements")]
		public Transform contentGrid;
		public TMP_Dropdown presetDropdown;
		[SerializeField] TMP_InputField ProfitMargin;
		[SerializeField] TMP_InputField ReprocessTax;
		[SerializeField] TMP_InputField BaseYield;

		private System preset = System.None;
		public static PanelState panelState = PanelState.RegionView;

		public float scrollPosX;
		Color defaultColor;

		public static bool ShowGUI => EveMarket.ShowGUI;

		[SerializeField] private List<GroupContainer> groupContainers = new List<GroupContainer>();

		private void Start()
		{
			EveDelegate.Subscribe(ref EveDelegate.AppSettingsChanged, ApplyAppSettings);
		}

		private void OnEnable()
		{
			defaultColor = GUI.backgroundColor;
		}

		private void OnDestroy()
		{
			EveDelegate.Unsubscribe(ref EveDelegate.AppSettingsChanged, ApplyAppSettings);
		}

		public void CreateGroupContainers()
		{
			if (ProfitMargin != null)
			{
				ProfitMargin.SetTextWithoutNotify(AppSettings.Settings.MarginPercentage.ToString());
			}

			if (BaseYield != null)
			{
				BaseYield.SetTextWithoutNotify(AppSettings.Settings.BaseYield.ToString());
			}

			if (ReprocessTax != null)
			{
				ReprocessTax.SetTextWithoutNotify(AppSettings.Settings.ReprocessTax.ToString());
			}

			if (panelState == PanelState.RegionView)
			{
				CreateRegionContainers();
			}
			else
			{
				foreach (MarketObject marketObject in StaticData.MarketObjects.Values)
				{
					CreateGroupContainer(marketObject);
				}
			}

			EveStateMachine.SetNextState(new Authentication(), AppState.Authentication);
		}

		public void CreateRegionContainers()
		{
			foreach (BuyPreset preset in AppSettings.Presets.Values)
			{
				GroupContainer oreContainer = ((GameObject)Instantiate(groupContainerPrefab, contentGrid)).GetComponent<GroupContainer>();
				oreContainer.name = $"{preset.buyRegion} {preset.buySystem} Ore";

				List<MarketItem> items = new List<MarketItem>();

				foreach (MarketObject marketObject in StaticData.MarketObjects.Values)
				{
					if (marketObject.GroupId != 1857 && marketObject.GroupId != 1033)
					{
						foreach (MarketItem item in marketObject.Items.Values)
						{
							items.Add(item);
						}
					}
				}

				oreContainer.GroupHeader.SetHeader($"{oreContainer.name}");
				items.Sort((item1, item2) => item1.ItemName.CompareTo(item2.ItemName));
				oreContainer.ItemContianer.PopulateItemContainer(items, preset);
				groupContainers.Add(oreContainer);
			}

			foreach (MarketObject marketObject in StaticData.MarketObjects.Values)
			{
				if (marketObject.GroupId == 1857 || marketObject.GroupId == 1033)
				{
					GroupContainer groupContainer = ((GameObject)Instantiate(groupContainerPrefab, contentGrid)).GetComponent<GroupContainer>();
					groupContainer.name = marketObject.GroupName;
					groupContainer.GroupHeader.SetHeader(marketObject);
					groupContainer.ItemContianer.PopulateItemContainer(marketObject);
					groupContainers.Add(groupContainer);
				}
			}
		}

		public void CreateGroupContainer(MarketObject marketObject)
		{
			GroupContainer groupContainer;

			int index = 0;

			// Find container in container list or add if does not exist.
			if ((index = groupContainers.FindIndex(Container => Container.name == marketObject.GroupName)) < 0)
			{
				groupContainer = ((GameObject)Instantiate(groupContainerPrefab, contentGrid)).GetComponent<GroupContainer>();
				groupContainers.Add(groupContainer);
			}
			else
			{
				groupContainer = groupContainers[index];
			}

			if (!groupContainer)
			{
				return;
			}
			else
			{
				groupContainer.name = marketObject.GroupName;
			}

			if (groupContainer.GroupHeader)
			{
				groupContainer.GroupHeader.SetHeader(marketObject);

				groupContainer.ItemContianer.PopulateItemContainer(marketObject);
			}
		}

		public void UpdateUIContainers(int typeId)
		{
			foreach (var container in groupContainers)
			{
				foreach (InfoPanel panel in container.ItemContianer.infoPanels)
				{
					if (StaticData.ItemsToUpdate.Exists(id => id == panel.TypeId))
					{
						panel.UpdateInfoPanel();
					}
				}
				container.ItemContianer.UpdateContainer();
			}
		}

		public void ApplyAppSettings()
		{
			if (ProfitMargin != null)
			{
				ProfitMargin.text = AppSettings.Settings.MarginPercentage.ToString();
			}
			if (ReprocessTax != null)
			{
				ReprocessTax.text = AppSettings.Settings.ReprocessTax.ToString();
			}
			if (BaseYield != null)
			{
				BaseYield.text = AppSettings.Settings.BaseYield.ToString();
			}
		}

		public void SetProfitMargin()
		{
			if (ProfitMargin != null)
			{
				ProfitMargin.text = AppSettings.Settings.MarginPercentage.ToString();
			}
		}

		public void UpdateProfitMargin()
		{
			if (Int32.TryParse(ProfitMargin.text, out int val))
			{
				AppSettings.SetProfitMargin(val);
				EveDelegate.UpdateItemNotify?.Invoke(0);
			}
		}

		public void UpdateBaseYield()
		{
			if (float.TryParse(BaseYield.text, out float val))
			{
				AppSettings.SetBasePercent(val);
				EveDelegate.UpdateItemNotify?.Invoke(0);
			}
		}

		public void UpdateReprocessTax()
		{
			if (float.TryParse(ReprocessTax.text, out float val))
			{
				AppSettings.SetReprocessTax(val);
				EveDelegate.UpdateItemNotify?.Invoke(0);
			}
		}

		public void UpdatePreset(int val)
		{
			if (AppSettings.Presets.ContainsKey((System)val))
			{
				preset = (System)val;
				AppSettings.UpdatePreset((System)val);
				EveDelegate.UpdateUINotify?.Invoke(0);
			}
			else
			{
				presetDropdown.value = (int)preset;
			}
		}

		private void UpdateMarketObjects()
		{
			if (StaticData.UpdateChangedRecordsOnly)
			{
				StaticData.UpdateNewRecordItems();
			}
			else
			{
				StaticData.UpdateMarketObjects();
			}
		}

		private void Button(string text, string toolTip, Action action)
		{
			GUIContent content = new GUIContent(text, toolTip);
			GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.fixedWidth = buttonStyle.CalcSize(content).x;
			buttonStyle.hover.textColor = Color.yellow;
			if (GUILayout.Button(content, buttonStyle))
			{
				action.Invoke();
			}
		}
	}
}
