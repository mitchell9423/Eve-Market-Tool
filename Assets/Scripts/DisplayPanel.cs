using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Windows;
using GameEvent;

namespace EveMarket
{
	[ExecuteAlways]
	public class DisplayPanel : MonoBehaviour
	{
		private Vector2 scrollPos;
		public static string text = "";
		public float scrollPosX;
		public float scrollWidthOffset;
		public float viewPosX;
		public float viewWidthOffset;

		private void OnGUI()
		{
			GUI.backgroundColor = Color.black;

			//int fontSize = 25;

			// Create a new style for the TextField
			GUIStyle displayFieldStyle = new GUIStyle(GUI.skin.box);
			displayFieldStyle.normal.textColor = Color.cyan;
			displayFieldStyle.alignment = TextAnchor.UpperLeft;

			string buttonLabel = "Load Static Data";

			float posY = 10;
			float posX = 10;

			float buttonHeight = displayFieldStyle.CalcHeight(new GUIContent(buttonLabel), 150.0f);
			if (GUI.Button(new Rect(posX, posY, 150.0f, buttonHeight), "Load Static Data"))
			{
				StaticData.UpdateStaticData();
			}

			// Create TextField
			// Calculate the size of the text content
			Vector2 textSize = displayFieldStyle.CalcSize(new GUIContent(text));

			posX = 20;
			posY += buttonHeight + 10;
			float positionRectWidth = Screen.width - (posX * 2);
			float positionRectHeight = Screen.height - posY - 10;
			float contentRectWidth = Mathf.Max(positionRectWidth - 10f, textSize.x + 5);
			float contentRectHeight = Mathf.Max(positionRectHeight - 10f, textSize.y + 5);

			Rect scrollViewPositionRect = new Rect(
				posX,
				posY,
				positionRectWidth,
				positionRectHeight
				);

			Rect scrollViewRect = new Rect(
				0,
				0,
				contentRectWidth,
				contentRectHeight
				);

			scrollPos = GUI.BeginScrollView(scrollViewPositionRect, scrollPos, scrollViewRect);

			GUI.Box(new Rect(
				0,
				0,
				contentRectWidth,
				contentRectHeight
				), text, displayFieldStyle);

			GUI.EndScrollView();
		}

		public static void SetDisplayText(string _text)
		{
			text = _text;
		}
	}
}
