using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Windows;
using GameEvent;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using EveMarket.Network;
using UnityEditor.Sprites;

namespace EveMarket
{
	[ExecuteAlways]
	public class DisplayPanel : MonoBehaviour
	{
		public enum PanelState
		{
			AveragePrice,
			CurrentSellPrice
		}

		private struct LayoutPropertyBlock
		{
			// Item block widths
			public int nameWidth;
			public int spacer;
			public int labelWidth;
			public int priceWidth;

			// Define the number of rows and columns
			public int index;
			public int numObjects;

			public int blockWidth;
			public int columns => Mathf.FloorToInt((float)Screen.width / blockWidth);
			public int rows => numObjects > 0 ? Mathf.CeilToInt((float)numObjects / columns) : 0;

			// Define Label Texts
			public string priceLabel;
		}

		private PanelState panelState = PanelState.AveragePrice;
		private Dictionary<PanelState, Action> Panels;
		private Dictionary<PanelState, string> PriceLayoutLabel = new Dictionary<PanelState, string>()
		{
				{ PanelState.AveragePrice, "Average Price" },
				{ PanelState.CurrentSellPrice, "Sell Price" }
		};

		private Vector2 scrollPos;
		public float scrollPosX;
		Color defaultColor;

		LayoutPropertyBlock props = new LayoutPropertyBlock();
		public static bool ShowGUI => EveMarket.ShowGUI;

		private void OnEnable()
		{
			defaultColor = GUI.backgroundColor;
			Panels = new Dictionary<PanelState, Action>()
			{
				{ PanelState.AveragePrice, PriceLayout },
				{ PanelState.CurrentSellPrice, PriceLayout }
			};
			DisplayAveragePrice();
		}

		private void OnGUI()
		{
			if (!ShowGUI)
            {
				return;
			}

			GUI.backgroundColor = Color.black;

			GUILayout.Space(10);

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(10);

				using (new GUILayout.HorizontalScope(GUI.skin.box))
				{
					GUI.enabled = NetworkManager.Status == UpdateStatus.Idle;
					Button("Load Static Data", "Tool Tip", StaticData.LoadStaticData);
					GUILayout.Space(10);
					Button("Update Static Data", "Tool Tip", StaticData.UpdateStaticData);
					GUI.enabled = NetworkManager.Status != UpdateStatus.Idle;
					GUILayout.Space(10);
					Button("Cancel Update", "Tool Tip", HttpHandler.instance.StopAllRequests);
					GUI.enabled = true;
				}
			}

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(10);

				using (new GUILayout.HorizontalScope(GUI.skin.box))
				{
					Button("Average Price", "Tool Tip", DisplayAveragePrice);

					GUILayout.Space(10);

					Button("Current Sell Price", "Tool Tip", DisplayBuySellPrice);
				}
			}

			scrollPos = GUILayout.BeginScrollView(scrollPos);
			Panels[panelState].Invoke();
			GUILayout.EndScrollView();
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

		private void DisplayBuySellPrice()
		{
			panelState = PanelState.CurrentSellPrice;

			// Item block widths
			props.nameWidth = 300;
			props.spacer = 10;
			props.labelWidth = 28;
			props.priceWidth = 90;
			props.blockWidth = props.nameWidth + ((props.spacer + props.priceWidth + props.labelWidth) * 2);

			// Define the number of rows and columns
			props.numObjects = StaticData.MarketObjects.Count;
			props.index = 0;

			props.priceLabel = "Sell: ";
		}

		private void DisplayAveragePrice()
		{
			panelState = PanelState.AveragePrice;

			// Item block widths
			props.nameWidth = 300;
			props.spacer = 10;
			props.labelWidth = 87;
			props.priceWidth = 100;
			props.blockWidth = props.nameWidth + props.spacer + props.priceWidth + props.labelWidth;

			// Define the number of rows and columns
			props.numObjects = StaticData.MarketObjects.Count;
			props.index = 0;

			props.priceLabel = "Average Price:";
		}

		private void PriceLayout()
		{
			// Create the grid
			int index = 0;

			for (int row = 0; row < props.rows; row++)
			{
				if (index >= props.numObjects) break;

				GUILayout.Space(10);

				using (new GUILayout.HorizontalScope())
				{
					for (int col = 0; col < props.columns; col++)
					{
						if (index >= props.numObjects) break;

						GUI.backgroundColor = Color.black;

						MarketObject marketObject = StaticData.MarketObjects.ElementAt(index).Value;

						GUILayout.Space(10);

						using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(props.blockWidth)))
						{
							GUIStyle boldLabel = new GUIStyle(GUI.skin.label);
							boldLabel.fontStyle = FontStyle.Bold;
							boldLabel.fontSize = 20;

							using (new GUILayout.HorizontalScope())
							{
								GUILayout.Label($"{marketObject.GroupName}", boldLabel);
								GUILayout.Space(10);
								Button("Update Market Data", "Tool Tip", () => StaticData.UpdateMarketData(ids: marketObject.Group.Types));
							}

							GUILayout.Space(10);

							for (int j = 0; j < marketObject.ItemCount; j++)
							{
								using (new GUILayout.VerticalScope(GUI.skin.box))
								{
									MarketItem marketItem = marketObject.GetItemByIndex(j);

									using (new GUILayout.HorizontalScope())
									{
										double price = panelState == PanelState.AveragePrice ? marketItem.AveragePrice : marketItem.CurrentSellPrice;

										Color defaultColor = GUI.contentColor;
										GUI.enabled = (!marketItem.ItemName.Contains("Compressed") && marketItem.CurrentBuyPrice > 0) || (marketItem.ItemName.Contains("Compressed") && marketItem.CurrentSellPrice > 0);

										GUILayout.Label($"  {marketItem.ItemName}", GUILayout.Width(props.nameWidth));
										if (panelState == PanelState.CurrentSellPrice)
										{
											GUI.contentColor = marketItem.GetTextColor(defaultColor);
											GUILayout.Space(props.spacer);
											GUILayout.Label($"Buy: ", GUILayout.Width(props.labelWidth));
											GUILayout.Label($"{marketItem.CurrentBuyPrice}", GUILayout.Width(props.priceWidth));
										}
										GUILayout.Space(props.spacer);
										GUILayout.Label($"{props.priceLabel}", GUILayout.Width(props.labelWidth));
										GUILayout.Label($"{price}", GUILayout.Width(props.priceWidth));

										GUI.contentColor = defaultColor;
										GUI.enabled = true;

										// Add more types here as needed
									}
								}
							}

							GUILayout.Space(10);
						}

						GUI.backgroundColor = defaultColor;

						index++;
					}
				}
			}
		}
	}
}
