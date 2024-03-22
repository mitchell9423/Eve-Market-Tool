
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace EveMarket
{
	[CustomEditor(typeof(EveMarket))]
	public class EveMarketEditor : Editor
	{
		float nameWidth;
		float priceLabelWidth;
		float priceWidth;

		EveMarket eveMarket;

		SerializedProperty unityMainThreadDispatcher;
		SerializedProperty httpHandler;
		SerializedProperty displayPanel;
		SerializedProperty ui;

		Dictionary<Type, string> TypeLabel = new Dictionary<Type, string>()
		{
			{ typeof(MarketGroup), "Market Group" },
			{ typeof(UniverseItem), "Universe Item" },
			{ typeof(MarketPrice), "Market Price" }
		};

		void OnEnable()
		{
			EveMarket.ShowGUI = EditorPrefs.GetBool("ShowGUI", false);
			eveMarket = (EveMarket)target;
			unityMainThreadDispatcher = serializedObject.FindProperty("unityMainThreadDispatcher");
			httpHandler = serializedObject.FindProperty("httpHandler");
			displayPanel = serializedObject.FindProperty("displayPanel");
			ui = serializedObject.FindProperty("ui");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(unityMainThreadDispatcher);
			EditorGUILayout.PropertyField(httpHandler);
			EditorGUILayout.PropertyField(displayPanel);
			EditorGUILayout.PropertyField(ui);

			EditorGUILayout.Space(10);

			if (GUILayout.Button("Load Static Data"))
			{
				eveMarket.LoadStaticData();
			}

			EditorGUILayout.Space(10);

			if (GUILayout.Button("Update Static Data"))
			{
				eveMarket.UpdateStaticData();
			}

			EditorGUILayout.Space(10);

			using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				EditorGUILayout.LabelField("Show GUI Display", EditorStyles.boldLabel, GUILayout.Width(120));

				EditorGUI.BeginChangeCheck();
				EveMarket.ShowGUI = EditorGUILayout.Toggle(EveMarket.ShowGUI);
				if (EditorGUI.EndChangeCheck())
				{
					EditorPrefs.SetBool("ShowGUI", EveMarket.ShowGUI);
				}
			}

			using (new GUILayout.VerticalScope(EditorStyles.helpBox))
			{
				EditorGUILayout.LabelField("Object Model List", EditorStyles.boldLabel);
			}

			for (int i = 0; i < StaticData.MarketObjects.Count; i++)
			{
				MarketObject marketObject = StaticData.MarketObjects.ElementAt(i).Value;

				EditorGUILayout.Space(10);

				using (new GUILayout.VerticalScope(EditorStyles.helpBox))
				{
					EditorGUILayout.LabelField($"Group: {marketObject.GroupName}");

					EditorGUILayout.Space(10);

					for (int j = 0; j < marketObject.ItemCount; j++)
					{
						MarketItem marketItem = marketObject.GetItemByIndex(j);

						using (new GUILayout.HorizontalScope())
						{
							EditorGUILayout.LabelField($"  {marketItem.ItemName}", GUILayout.Width(250));
							EditorGUILayout.Space();
							EditorGUILayout.LabelField($"Average Price:", GUILayout.Width(87));
							EditorGUILayout.LabelField($"{marketItem.AveragePrice}", GUILayout.Width(100));

							// Add more types here as needed
						}
					}

					EditorGUILayout.Space();
				}
			}

			EditorGUILayout.Space(10);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
