using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EveMarket.UI
{
	public class TimerScript : MonoBehaviour
	{
		[SerializeField] private Color startColor;
		[SerializeField] private Color endingColor;
		[SerializeField] private Color stopColor;
		[SerializeField] private Color disabledColor;

		[SerializeField] private TMP_Text text;

		// Start is called before the first frame update
		void Start()
		{
			if (text == null) text = GetComponent<TMP_Text>();
			text.faceColor = startColor;
		}

		// Update is called once per frame
		void Update()
		{
			int minutes = Math.Clamp(EveMarket.RemainingTime.Minutes, 0, 59);
			int seconds = Math.Clamp(EveMarket.RemainingTime.Seconds, 0, 59);

			double percentage = minutes / (double)EveMarket.TimerInterval.Minutes;

			if (!EveMarket.EnableTimedUpdate)
			{
				text.faceColor = disabledColor;
			}
			else if (percentage >= 0.5f)
			{
				text.faceColor = startColor;
			}
			else if (percentage <= 0.05f)
			{
				text.faceColor = stopColor;
			}
			else
			{
				text.faceColor = endingColor;
			}

			string secondsStr = seconds < 10 ? $"0{seconds}" : $"{seconds}";
			string minutesStr = minutes < 10 ? $"0{minutes}" : $"{minutes}";

			text.text = $"{minutesStr}:{secondsStr} - Till Next Update";
		}
	}
}
