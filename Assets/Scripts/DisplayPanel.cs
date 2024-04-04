
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using EveMarket.Network;
using TMPro;
using EveMarket.Util;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EveMarket.UI
{
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
			public int compressedLabelWidth;

			// Define the number of rows and columns
			public int index;
			public int numObjects;

			public int blockWidth;
			public int columns;
			public int rows;

			// Define Label Texts
			public string priceLabel;

			public void SetRowCol()
			{
				columns = Mathf.FloorToInt((float)Screen.width / (blockWidth + 20));
				rows = numObjects > 0 ? Mathf.CeilToInt((float)numObjects / columns) : 0;
			}
		}

		[Header("Prefab Refs")]
		public UnityEngine.Object groupContainerPrefab;

		[Header("UI Elements")]
		public Transform contentGrid;
		public TMP_Dropdown presetDropdown;

		private System preset = System.None;
		private PanelState panelState = PanelState.CurrentSellPrice;
		private Dictionary<PanelState, Action> Panels;
		private Dictionary<PanelState, string> PriceLayoutLabel = new Dictionary<PanelState, string>()
		{
				{ PanelState.AveragePrice, "Average Price" },
				{ PanelState.CurrentSellPrice, "Sell Price" }
		};

		private string tempBuyRegion = null; // Holds the current value of the text field
		private string tempMargin = null;
		private string tempBuyRange = null;
		private string tempBuySystem = null;
		private System tempPreset = System.None;

		private Vector2 scrollPos;
		public float scrollPosX;
		Color defaultColor;

		LayoutPropertyBlock props = new LayoutPropertyBlock();
		public static bool ShowGUI => EveMarket.ShowGUI;


		private bool showDropdown = false;
		private int selectedIndex = 0;
		private string[] options = Enum.GetNames(typeof(System));
		private string selectedOption => options[selectedIndex];
		[SerializeField] private List<GroupContainer> groupContainers = new List<GroupContainer>();

		private void OnEnable()
		{
			defaultColor = GUI.backgroundColor;
			Panels = new Dictionary<PanelState, Action>()
			{
				{ PanelState.AveragePrice, PriceLayout },
				{ PanelState.CurrentSellPrice, PriceLayout }
			};

			DisplayBuySellPrice();
		}

		private void Start()
		{
		}

		public void CreateGroupContainers()
		{
			foreach (MarketObject marketObject in StaticData.MarketObjects.Values)
			{
				CreateGroupContainer(marketObject);
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

		public void UpdatePreset(int val)
		{
			preset = (System)val;
			ApplyPreset();
			AppSettings.SaveAppSettings();
		}

		private void OnGUI()
		{
			if (!ShowGUI)
            {
				return;
			}

			Event e = Event.current;

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

				using (new GUILayout.VerticalScope())
				{
					using (new GUILayout.HorizontalScope(GUI.skin.box))
					{
						Button("Average Price", "Tool Tip", DisplayAveragePrice);

						GUILayout.Space(10);

						Button("Current Sell Price", "Tool Tip", DisplayBuySellPrice);

						if (panelState == PanelState.CurrentSellPrice)
						{
							GUILayout.Space(10);
							GUILayout.Label($"Buy Region: ", GUILayout.Width(props.compressedLabelWidth));
							if (tempBuyRegion == null) tempBuyRegion = AppSettings.Settings.BuyRegion.ToString();
							tempBuyRegion = GUILayout.TextField(tempBuyRegion, GUILayout.Width(props.compressedLabelWidth));

							GUILayout.Space(10);
							GUILayout.Label($"Buy System: ", GUILayout.Width(props.compressedLabelWidth));
							if (tempBuySystem == null) tempBuySystem = AppSettings.Settings.BuyOrderSystem.ToString();
							tempBuySystem = GUILayout.TextField(tempBuySystem, GUILayout.Width(props.compressedLabelWidth));

							GUILayout.Space(10);
							GUILayout.Label($"Buy Range: ", GUILayout.Width(props.compressedLabelWidth));
							if (tempBuyRange == null) tempBuyRange = AppSettings.Settings.BuyRange;
							tempBuyRange = GUILayout.TextField(tempBuyRange, GUILayout.Width(props.compressedLabelWidth));

							GUILayout.Space(10);
							GUILayout.Label($"Margin %: ", GUILayout.Width(props.compressedLabelWidth));
							if (tempMargin == null) tempMargin = AppSettings.Settings.MarginPercentage.ToString();
							tempMargin = GUILayout.TextField(tempMargin, GUILayout.Width(props.compressedLabelWidth));

							GUILayout.Space(10);
							GUILayout.Label($"Preset: ", GUILayout.Width(props.compressedLabelWidth));
							if (!showDropdown)
							{
								if (GUILayout.Button(tempPreset.ToString(), GUILayout.Width(props.compressedLabelWidth)))
								{
									showDropdown = !showDropdown;
								}
							}

							if (showDropdown)
							{
								for (int i = 0; i < options.Length; i++)
								{
									if (GUILayout.Button(options[i]))
									{
										tempPreset = preset = (System)i;
										ApplyPreset();
										showDropdown = false;
									}
								}
							}

							if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
							{
								if (Enum.TryParse(typeof(Region), tempBuyRegion, true, out object newRegion))
								{
									AppSettings.Settings.BuyRegion = (Region)newRegion;
								}

								if (Enum.TryParse(typeof(System), tempBuySystem, true, out object newSystem))
								{
									AppSettings.Settings.BuyOrderSystem = (System)newSystem;
								}

								AppSettings.Settings.BuyRange = tempBuyRange;

								if (int.TryParse(tempMargin, out int newValue))
								{
									AppSettings.Settings.MarginPercentage = newValue;
								}

								AppSettings.SaveAppSettings();
								UpdateMarketObjects();
							}
						}
					}

#if UNITY_EDITOR
					using (new GUILayout.HorizontalScope(GUI.skin.box))
					{
						if (panelState == PanelState.CurrentSellPrice)
						{
							EditorGUI.BeginChangeCheck();
							GUILayout.Label($"Skills: ", GUILayout.Width(props.compressedLabelWidth));

							GUILayout.Space(10);
							GUILayout.Label($"Reprocessing: ", GUILayout.Width(props.compressedLabelWidth + 5));
							if (Int32.TryParse(GUILayout.TextField(Reprocess.ReprocessingSkillLvl.ToString(), GUILayout.Width(props.compressedLabelWidth)), out Reprocess.ReprocessingSkillLvl)) { }

							GUILayout.Space(10);
							GUILayout.Label($"Efficiency: ", GUILayout.Width(props.compressedLabelWidth));
							if (Int32.TryParse(GUILayout.TextField(Reprocess.ReprocessingEfficiencySkillLvl.ToString(), GUILayout.Width(props.compressedLabelWidth)), out Reprocess.ReprocessingEfficiencySkillLvl)) { }

							GUILayout.Space(10);
							GUILayout.Label($"Simple: ", GUILayout.Width(props.compressedLabelWidth - 30));
							if (Int32.TryParse(GUILayout.TextField(Reprocess.SimpleOreProcessingSkillLvl.ToString(), GUILayout.Width(props.compressedLabelWidth)), out Reprocess.SimpleOreProcessingSkillLvl)) { }

							GUILayout.Space(10);
							GUILayout.Label($"Ice: ", GUILayout.Width(props.compressedLabelWidth - 55));
							if (Int32.TryParse(GUILayout.TextField(Reprocess.IceProcessingSkillLvl.ToString(), GUILayout.Width(props.compressedLabelWidth)), out Reprocess.IceProcessingSkillLvl)) { }

							GUILayout.Space(10);
							GUILayout.Label($"Common: ", GUILayout.Width(props.compressedLabelWidth - 20));
							if (Int32.TryParse(GUILayout.TextField(Reprocess.CommonOreProcessingSkillLvl.ToString(), GUILayout.Width(props.compressedLabelWidth)), out Reprocess.CommonOreProcessingSkillLvl)) { }

							GUILayout.Space(10);
							GUILayout.Label($"Coherent: ", GUILayout.Width(props.compressedLabelWidth));
							if (Int32.TryParse(GUILayout.TextField(Reprocess.CoherentOreProcessingSkillLvl.ToString(), GUILayout.Width(props.compressedLabelWidth)), out Reprocess.CoherentOreProcessingSkillLvl)) { }

							GUILayout.Space(10);
							GUILayout.Label($"Variegated: ", GUILayout.Width(props.compressedLabelWidth));
							if (Int32.TryParse(GUILayout.TextField(Reprocess.VariegatedOreProcessingSkillLvl.ToString(), GUILayout.Width(props.compressedLabelWidth)), out Reprocess.VariegatedOreProcessingSkillLvl)) { }

							GUILayout.Space(10);
							GUILayout.Label($"Mercoxite: ", GUILayout.Width(props.compressedLabelWidth));
							if (Int32.TryParse(GUILayout.TextField(Reprocess.MercoxiteOreProcessingSkillLvl.ToString(), GUILayout.Width(props.compressedLabelWidth)), out Reprocess.MercoxiteOreProcessingSkillLvl)) { }
							if (EditorGUI.EndChangeCheck())
							{
								StaticData.UpdateMarketObjects();
							}
						}
					}
#endif
				}
			}

			scrollPos = GUILayout.BeginScrollView(scrollPos);
			Panels[panelState].Invoke();
			GUILayout.EndScrollView();
		}

		private void ApplyPreset()
		{
			if (preset == System.None) return;

			//tempBuyRegion = (AppSettings.Settings.BuyRegion = AppSettings.Presets[preset].buyRegion).ToString();
			//tempBuySystem = (AppSettings.Settings.BuyOrderSystem = AppSettings.Presets[preset].buySystem).ToString();
			//tempBuyRange = AppSettings.Settings.BuyRange = AppSettings.Presets[preset].buyRange;
			//tempMargin = (AppSettings.MarginPercentage = AppSettings.Presets[preset].profitMargin).ToString();

			AppSettings.Settings.BuyRegion = AppSettings.Presets[preset].buyRegion;
			AppSettings.Settings.BuyOrderSystem = AppSettings.Presets[preset].buySystem;
			AppSettings.Settings.BuyRange = AppSettings.Presets[preset].buyRange;

			AppSettings.Settings.ActivePreset = preset;
			UpdateMarketObjects();
		}

		private void UpdateMarketObjects()
		{
			StaticData.UpdateMarketObjects();
			foreach (var marketObject in StaticData.MarketObjects.Values)
			{
				CreateGroupContainer(marketObject);
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

		private void DisplayBuySellPrice()
		{
			panelState = PanelState.CurrentSellPrice;


			// Item block widths
			props.nameWidth = 200;
			props.spacer = 5;
			props.labelWidth = 28;
			props.priceWidth = 90;
			props.compressedLabelWidth = 80;
			props.blockWidth = props.nameWidth + (props.labelWidth * 2) + ((props.priceWidth + props.spacer) * 3) + props.compressedLabelWidth;

			// Define the number of rows and columns
			props.numObjects = StaticData.MarketObjects.Count;
			props.index = 0;

			props.priceLabel = "Sell: ";

			props.SetRowCol();
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
			props.SetRowCol();
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
								MarketItem marketItem = marketObject.GetItemByIndex(j);
								if (marketItem.ItemName.Contains("Compressed")) continue;

								double compressedPrice = 0;
								double reprocessedPrice = 0;

								if (panelState == PanelState.CurrentSellPrice)
								{
									MarketItem compressedItem = marketObject.Items.Find(item => item.ItemName == $"Compressed {marketItem.ItemName}");
									compressedPrice = compressedItem != null ? compressedItem.CurrentSellPrice : 0;

									if (marketObject.GroupName != "Minerals")
									{
										reprocessedPrice = marketItem.ReprocessPrice;
									}
								}

								using (new GUILayout.VerticalScope(GUI.skin.box))
								{
									using (new GUILayout.HorizontalScope())
									{
										double sellPrice = panelState == PanelState.AveragePrice ? marketItem.AveragePrice : marketItem.CurrentSellPrice;

										Color defaultColor = GUI.contentColor;
										GUI.enabled = (!marketItem.ItemName.Contains("Compressed") && marketItem.CurrentBuyPrice > 0) || (marketItem.ItemName.Contains("Compressed") && marketItem.CurrentSellPrice > 0);

										GUILayout.Label($"  {marketItem.ItemName}", GUILayout.Width(props.nameWidth));
										if (panelState == PanelState.CurrentSellPrice)
										{
                                            if (marketItem.CurrentBuyPrice > reprocessedPrice)
											{
												GUI.contentColor = Color.red;
											}
                                            else if (marketItem.CurrentBuyPrice > marketItem.MaxBuyPrice)
											{
												GUI.contentColor = Color.yellow;
											}
											else if (marketItem.CurrentBuyPrice > 0 && marketItem.CurrentBuyPrice < marketItem.MaxBuyPrice)
											{
												GUI.contentColor = Color.green;
											}

											GUILayout.Space(props.spacer);
											GUILayout.Label($"Buy: ", GUILayout.Width(props.labelWidth));
											GUILayout.Label($"{marketItem.CurrentBuyPrice}", GUILayout.Width(props.priceWidth));

											if (marketObject.GroupName == "Minerals" || marketObject.GroupName == "Ice Products")
											{
												GUILayout.Space(props.spacer);
												GUILayout.Label($"{props.priceLabel}", GUILayout.Width(props.labelWidth));
												GUILayout.Label($"{sellPrice}", GUILayout.Width(props.priceWidth));
											}
											else
											{
												GUILayout.Space(props.spacer);
												GUILayout.Label($"Max Buy: ", GUILayout.Width(props.labelWidth));
												GUILayout.Label($"{marketItem.MaxBuyPrice}", GUILayout.Width(props.priceWidth));

												//GUILayout.Space(props.spacer);
												//GUILayout.Label($"Compressed: ", GUILayout.Width(props.compressedLabelWidth));
												//GUILayout.Label($"{compressedPrice}", GUILayout.Width(props.priceWidth));

												GUILayout.Space(props.spacer);
												GUILayout.Label($"Sell Price: ", GUILayout.Width(props.compressedLabelWidth));
												GUILayout.Label($"{reprocessedPrice}", GUILayout.Width(props.priceWidth));
											}
										}
                                        else
										{
											GUILayout.Space(props.spacer);
											GUILayout.Label($"{props.priceLabel}", GUILayout.Width(props.labelWidth));
											GUILayout.Label($"{sellPrice}", GUILayout.Width(props.priceWidth));
										}

										GUI.contentColor = defaultColor;
										GUI.enabled = true;

										// Add more types here as needed
									}
								}
							}

							//GUILayout.Space(10);
						}

						GUI.backgroundColor = defaultColor;

						index++;
					}

					GUILayout.Space(20);
				}

			}
		}
	}
}
