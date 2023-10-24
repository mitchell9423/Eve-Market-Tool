using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace EveMarket
{
	public class DisplayPanel : MonoBehaviour
	{
		private Vector2 scrollPos;
		private string text = "";

		private void OnGUI()
		{
			int fontSize = 25;

			// Create a new style for the TextField
			GUI.backgroundColor = Color.black;
			GUIStyle textFieldStyle = new GUIStyle();
			textFieldStyle.fontSize = fontSize;
			textFieldStyle.normal.textColor = Color.cyan;
			textFieldStyle.normal.background = Texture2D.grayTexture;

			// Create TextField
			float boxHeight = 5;
			GUI.Box(new Rect(5f, boxHeight, Screen.width - 10f, Screen.height - (boxHeight + 5)), "");
			scrollPos = GUI.BeginScrollView(new Rect(10f, boxHeight + 10f, Screen.width - 20f, (Screen.height - (boxHeight + 5)) - 20), scrollPos, new Rect(0, 0, Screen.width - 20f, text.Length * 20));

			// Create a new style for the suggestions
			float warningLabelHieght = 0;
			GUI.Label(new Rect(10, warningLabelHieght, Screen.width - 20f, textFieldStyle.CalcHeight(new GUIContent(text), Screen.width - 20f)), text, textFieldStyle);

			GUI.EndScrollView();
		}

		public void SetDisplayText(string newtext)
		{
			text = newtext;
		}
	}
}
