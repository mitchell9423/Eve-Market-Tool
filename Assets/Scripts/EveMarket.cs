using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EveMarket
{
	[ExecuteAlways]
	public class EveMarket : MonoBehaviour
	{
		[SerializeField] HttpHandler httpHandler;
		[SerializeField] DisplayPanel displayPanel;

		[SerializeField] MarketGroup marketGroup;
		[SerializeField] ItemInfo itemInfo;

		private void OnEnable()
		{
			if (!gameObject.TryGetComponent(out httpHandler))
			{
				httpHandler = gameObject.AddComponent<HttpHandler>();
			}

			if (!gameObject.TryGetComponent(out displayPanel))
			{
				displayPanel = gameObject.AddComponent<DisplayPanel>();
			}
		}

		private void Start()
		{
			StaticData.LoadStaticData();
		}
	}
}
