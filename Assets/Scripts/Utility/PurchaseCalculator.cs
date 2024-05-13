using EveMarket.Util;
using System;
using TMPro;
using UnityEngine;

namespace EveMarket.UI
{
	public class PurchaseCalculator : MonoBehaviour
	{
		[SerializeField] TMP_InputField IskAmount;
		[SerializeField] TMP_InputField UnitPrice;
		[SerializeField] TMP_InputField PurchaseQuantity;

		double iskAmount;
		double unitPrice;

		// Start is called before the first frame update
		void Start()
		{
			EveDelegate.Subscribe(ref EveDelegate.AppSettingsChanged, InitFields);
		}

		private void OnDestroy()
		{
			EveDelegate.Unsubscribe(ref EveDelegate.AppSettingsChanged, InitFields);
		}

		public void UpdateValues()
		{
			iskAmount = 0;
			unitPrice = 0;

			double.TryParse(IskAmount.text, out iskAmount);
			double.TryParse(UnitPrice.text, out unitPrice);

			Calculate();
		}

		void Calculate()
		{
			PurchaseQuantity.text = "";

			if (iskAmount <= 0)
			{
				IskAmount.text = "";
			}

			if (unitPrice <= 0)
			{
				UnitPrice.text = "";
			}

			if (iskAmount > 0 && unitPrice > 0)
			{
				float tax = 0.0127f;
				float taxedPrice = (float)(unitPrice + (unitPrice * tax));
				double trueValue = iskAmount / taxedPrice;

				double roundDown = Math.Floor(trueValue);
				int converted = (int)roundDown;

				PurchaseQuantity.text = converted.ToString();
				UpdateSettings();
			}
		}

		// Called on UI changes
		void UpdateSettings()
		{
			AppSettings.Settings.IskAmount = iskAmount;
			AppSettings.Settings.UnitPrice = unitPrice;
			AppSettings.SaveAppSettings();
		}

		// Called to initialize saved settings
		void InitFields()
		{
			IskAmount.text = (iskAmount = AppSettings.Settings.IskAmount).ToString();
			UnitPrice.text = (unitPrice = AppSettings.Settings.UnitPrice).ToString();

			if (unitPrice > 0)
			{
				Calculate();
			}
			else
			{
				PurchaseQuantity.text = "";
			}
		}
	}
}
