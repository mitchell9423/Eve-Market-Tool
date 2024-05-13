using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace EveMarket.UI
{
	public class TimerScript : MonoBehaviour
	{
		[SerializeField] private Color closeColor;
		[SerializeField] private Color normalColor;
		[SerializeField] private Color farColor;
		[SerializeField] private Color disabledColor;

		[SerializeField] private TMP_Text text;
		public TimeSpan TimerInterval { get; set; } = new TimeSpan(0, 20, 0);

		// Start is called before the first frame update
		void Start()
		{
			if (text == null) text = GetComponent<TMP_Text>();
			text.faceColor = closeColor;
		}

		// Update is called once per frame
		void Update()
		{
			if (StaticData.CorpOrderRecord == null) return;

			DateTime Expiration = DateTime.Parse(StaticData.CorpOrderRecord.Expiration, null, DateTimeStyles.AdjustToUniversal);

			TimeSpan remainingTime = Expiration - DateTime.Now;

			int minutes = Math.Clamp(remainingTime.Minutes, 0, 59);
			int seconds = Math.Clamp(remainingTime.Seconds, 0, 59);

			double percentage = minutes / (double)TimerInterval.Minutes;

			if (!EveMarket.EnableTimedUpdate)
			{
				text.faceColor = disabledColor;
			}
			else if (percentage <= 0.1f)
			{
				text.faceColor = closeColor;
			}
			else if (percentage <= 0.7f)
			{
				text.faceColor = normalColor;
			}
			else
			{
				text.faceColor = farColor;
			}

			string secondsStr = seconds < 10 ? $"0{seconds}" : $"{seconds}";
			string minutesStr = minutes < 10 ? $"0{minutes}" : $"{minutes}";

			text.text = $"{minutesStr}:{secondsStr} - Till Corp Update";
		}
	}
}
