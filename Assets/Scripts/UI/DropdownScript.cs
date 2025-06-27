using EveMarket.Util;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace EveMarket.UI
{
	public class DropdownScript : MonoBehaviour
	{
		[SerializeField] private TMP_Dropdown dropdown;
		private string[] options = Enum.GetNames(typeof(System));

		private void Awake()
		{
			if (dropdown == null || options == null) return;

			dropdown.ClearOptions();
			List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();
			foreach (var option in options)
			{
				TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData(option);
				dropdownOptions.Add(newOption);
			}

			dropdown.options = dropdownOptions;
		}

		// Start is called before the first frame update
		void Start()
		{
			EveDelegate.Subscribe(ref EveDelegate.AppSettingsChanged, UpdateDropdpwn);
		}

		private void OnDestroy()
		{
			EveDelegate.Unsubscribe(ref EveDelegate.AppSettingsChanged, UpdateDropdpwn);
		}

		void UpdateDropdpwn()
		{
			dropdown.value = (int)AppSettings.Settings.ActivePreset;
			dropdown.RefreshShownValue();
			Debug.Log($"Active Preset: {AppSettings.Settings.ActivePreset}");
		}
	}
}
