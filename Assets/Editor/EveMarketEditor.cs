
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace EveMarket
{
	[CustomEditor(typeof(EveMarket))]
	public class EveMarketEditor : Editor
	{
		EveMarket eveMarket;
		SerializedProperty unityMainThreadDispatcher;
		SerializedProperty httpHandler;
		SerializedProperty displayPanel;
		SerializedProperty modelList;

		Dictionary<Type, string> TypeLabel = new Dictionary<Type, string>()
		{
			{ typeof(MarketGroup), "Market Group" },
			{ typeof(UniverseItem), "Universe Item" },
			{ typeof(MarketPrice), "Market Price" }
		};

		void OnEnable()
		{
			eveMarket = (EveMarket)target;
			//modelList = serializedObject.FindProperty("modelList");
			unityMainThreadDispatcher = serializedObject.FindProperty("unityMainThreadDispatcher");
			httpHandler = serializedObject.FindProperty("httpHandler");
			displayPanel = serializedObject.FindProperty("displayPanel");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(unityMainThreadDispatcher);
			EditorGUILayout.PropertyField(httpHandler);
			EditorGUILayout.PropertyField(displayPanel);

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

			if (GUILayout.Button("Clear Display"))
			{
				eveMarket.ClearDisplay();
			}

			EditorGUILayout.Space(10);

			// Custom List Display
			EditorGUILayout.LabelField("Object Model List", EditorStyles.boldLabel);
			for (int i = 0; i < eveMarket.modelList.Count; i++)
			{
				EditorGUILayout.Space(10);
				using (new GUILayout.VerticalScope(EditorStyles.helpBox))
				{
					IDataModel objModel = eveMarket.modelList[i];
					if (objModel is IObjectModel objectModel)
					{
						EditorGUILayout.LabelField(TypeLabel[objectModel.GetType()], EditorStyles.boldLabel);
						using (new GUILayout.HorizontalScope())
						{
							EditorGUILayout.LabelField("Name:", GUILayout.Width(50));
							objectModel.Name = EditorGUILayout.TextField(objectModel.Name);
							EditorGUILayout.Space();
							EditorGUILayout.LabelField("Id:", GUILayout.Width(20));
							objectModel.Id = EditorGUILayout.IntField(objectModel.Id);
						}
						// Add more fields as needed
					}
					// Add more types as needed

					EditorGUILayout.Space();
				}
			}

			EditorGUILayout.Space(10);

			// Add / Remove buttons
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Add Element"))
				{
					eveMarket.modelList.Add(new MarketGroup()); // Add a new MarketGroup as an example
				}
				if (GUILayout.Button("Remove Element"))
				{
					if (eveMarket.modelList.Count > 0)
					{
						eveMarket.modelList.RemoveAt(eveMarket.modelList.Count - 1);
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
