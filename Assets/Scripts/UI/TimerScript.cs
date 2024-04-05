using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EveMarket.UI
{
	public class TimerScript : MonoBehaviour
	{
		[SerializeField] private TMP_Text text;

		// Start is called before the first frame update
		void Start()
		{
			if (text == null) text = GetComponent<TMP_Text>();
		}

		// Update is called once per frame
		void Update()
		{
			int minutes = EveMarket.RemainingTime.Minutes;
			int seconds = EveMarket.RemainingTime.Seconds;

			string secondsStr = seconds < 10 ? $"0{seconds}" : $"{seconds}";
			string minutesStr = minutes < 10 ? $"0{minutes}" : $"{minutes}";

			text.text = $"{minutesStr}:{secondsStr} - Till Next Update";
		}
	}
}
