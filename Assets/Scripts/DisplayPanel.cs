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

namespace EveMarket
{
	[ExecuteAlways]
	public class DisplayPanel : MonoBehaviour
	{
		public enum PanelState
		{
			AveragePrice
		}

		private PanelState panelState = PanelState.AveragePrice;
		private Dictionary<PanelState, Action> Panels;

		private Vector2 scrollPos;
		public float scrollPosX;

		Color defaultColor;
		public static bool ShowGUI => EveMarket.ShowGUI;

		private void OnEnable()
		{
			defaultColor = GUI.backgroundColor;
			Panels = new Dictionary<PanelState, Action>()
			{
				{ PanelState.AveragePrice, AveragePriceLayout }
			};
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
					Button("Load Static Data", "Tool Tip", StaticData.LoadStaticData);
					GUILayout.Space(10);
					Button("Update Static Data", "Tool Tip", StaticData.UpdateStaticData);
				}
			}

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(10);

				using (new GUILayout.HorizontalScope(GUI.skin.box))
				{
					Button("Average Price", "Tool Tip", DisplayAveragePrice);
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

		private void DisplayAveragePrice() => panelState = PanelState.AveragePrice;
		private void AveragePriceLayout()
		{
			// Item block widths
			int nameWidth = 300;
			int spacer = 10;
			int labelWidth = 87;
			int priceWidth = 100;
			int blockWidth = nameWidth + spacer + priceWidth + labelWidth;

			// Define the number of rows and columns
			int numObjects = EveMarket.MarketObjects.Count;
			int columns = Mathf.FloorToInt((float)Screen.width / blockWidth);
			int rows = numObjects > 0 ? Mathf.CeilToInt((float)numObjects / columns) : 0;
			int index = 0;

			// Create the grid

			for (int row = 0; row < rows; row++)
			{
				if (index >= numObjects) break;

				GUILayout.Space(10);

				using (new GUILayout.HorizontalScope())
				{
					for (int col = 0; col < columns; col++)
					{
						if (index >= numObjects) break;

						GUI.backgroundColor = Color.black;

						MarketObject marketObject = EveMarket.MarketObjects.ElementAt(index).Value;

						GUILayout.Space(10);

						using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(blockWidth)))
						{
							GUIStyle boldLabel = new GUIStyle(GUI.skin.label);
							boldLabel.fontStyle = FontStyle.Bold;
							boldLabel.fontSize = 20;
							GUILayout.Label($"{marketObject.GroupName}", boldLabel);

							GUILayout.Space(10);

							for (int j = 0; j < marketObject.ItemCount; j++)
							{
								using (new GUILayout.VerticalScope(GUI.skin.box))
								{
									MarketObject.MarketItem marketItem = marketObject.GetItemByIndex(j);

									using (new GUILayout.HorizontalScope())
									{
										GUILayout.Label($"  {marketItem.ItemName}", GUILayout.Width(nameWidth));
										GUILayout.Space(spacer);
										GUILayout.Label($"Average Price:", GUILayout.Width(labelWidth));
										GUILayout.Label($"{marketItem.AveragePrice}", GUILayout.Width(priceWidth));

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
