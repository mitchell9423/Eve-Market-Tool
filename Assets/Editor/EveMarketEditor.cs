
using UnityEngine;
using UnityEditor;

namespace EveMarket
{
	[CustomEditor(typeof(EveMarket))]
	public class EveMarketEditor : Editor
	{
		EveMarket targetObject;
		SerializedProperty unityMainThreadDispatcher;
		SerializedProperty httpHandler;
		SerializedProperty displayPanel;
		SerializedProperty modelList;

		void OnEnable()
		{
			targetObject = target as EveMarket;
			unityMainThreadDispatcher = serializedObject.FindProperty("unityMainThreadDispatcher");
			httpHandler = serializedObject.FindProperty("httpHandler");
			displayPanel = serializedObject.FindProperty("displayPanel");
			modelList = serializedObject.FindProperty("modelList");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(unityMainThreadDispatcher);
			EditorGUILayout.PropertyField(httpHandler);
			EditorGUILayout.PropertyField(displayPanel);

			EditorGUILayout.Space(10);

			if (GUILayout.Button("Update Static Data"))
			{
				targetObject.UpdateStaticData();
			}

			EditorGUILayout.Space(10);

			if (GUILayout.Button("Clear Display"))
			{
				targetObject.ClearDisplay();
			}

			if (targetObject.modelList.Count > 0)
			{
				EditorGUILayout.Space(10);

				EditorGUILayout.LabelField("Objects");
				EditorGUILayout.PropertyField(modelList);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
