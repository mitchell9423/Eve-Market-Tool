using EveMarket;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace EveMarket.UI
{
	public class GroupHeader : MonoBehaviour
	{
		[SerializeField] private TMP_Text Title;
		[SerializeField] private Button updateButton;

		public string GetTitle()
		{
			return Title.text;
		}

		public void SetHeader(MarketObject marketObject)
		{
			Title.text = marketObject.GroupName;
			updateButton.GetComponent<ButtonScript>().InitButtonEvent(data: marketObject.Group.Types);
		}
	}
}
