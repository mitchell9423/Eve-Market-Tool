using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;

namespace EveMarket
{
	public class Program : MonoBehaviour
	{
		[SerializeField] HttpHandler httpHandler;
		[SerializeField] DisplayPanel displayPanel;

		const string marketGroupsURL = "https://esi.evetech.net/latest/markets/groups/";
		const string universeTypesURL = "https://esi.evetech.net/latest/universe/types/";

		[SerializeField] MarketGroup marketGroup;
		[SerializeField] ItemInfo itemInfo;

		// Start is called before the first frame update
		void Start()
		{
			httpHandler = gameObject.AddComponent<HttpHandler>();
			displayPanel = GetComponent<DisplayPanel>();
			StartCoroutine(GetData<ItemInfo>());
		}

		IEnumerator GetData<T>()
		{
			string url = "";

			Type type = typeof(T);

			switch (type)
			{
				case Type _ when type == typeof(MarketGroup):
					url = $"{marketGroupsURL}512";
					break;
				case Type _ when type == typeof(ItemInfo):
					url = $"{universeTypesURL}28385";
					break;
				default:
					break;
			}

			yield return httpHandler.Get<T>(url, HandleResponse<T>);
		}

		void HandleResponse<T>(string response)
		{
			if (response != null)
			{
				T itemInfo = JsonConvert.DeserializeObject<T>(response);

				displayPanel.SetDisplayText(JsonConvert.SerializeObject(itemInfo, Formatting.Indented));

				Debug.Log($"Received data: {response}");
				// You can deserialize the JSON here
			}
			else
			{
				Debug.Log("Error receiving data.");
			}
		}
	}
}
