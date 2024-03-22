
using System.Collections.Generic;
using UnityEngine;
using EveMarket.Util;
using Unity.VisualScripting;
using UnityEditor;
using System.Text;
using System.Linq;

namespace EveMarket
{
	public class EveMarket : MonoBehaviour
	{
		public ObjectType objectType = ObjectType.MarketGroup;

		[SerializeField] UnityMainThreadDispatcher unityMainThreadDispatcher;
		[SerializeField] HttpHandler httpHandler;
		[SerializeField] DisplayPanel displayPanel;
		[SerializeField] DisplayPanel ui;

		public static bool ShowGUI { get; set; }

		StringBuilder sb = new StringBuilder();

		private void Awake()
		{
			AppSettings.LoadPlayerPrefs();

			if (!gameObject.TryGetComponent(out unityMainThreadDispatcher))
			{
				unityMainThreadDispatcher = gameObject.AddComponent<UnityMainThreadDispatcher>();
			}

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
			LoadStaticData();
		}

		public void LoadStaticData()
		{
			StaticData.LoadStaticData();
			BuildDisplayString();
			ui.CreateGroupContainers();
		}

		public void UpdateStaticData()
		{
			StaticData.UpdateStaticData();
			BuildDisplayString();
		}

		public void UpdateMarketData(Component sender, object obj)
		{
			StaticData.UpdateMarketData(obj as List<int>);
			BuildDisplayString();
		}

		void BuildDisplayString()
		{
			sb.Clear();

			for (int i = 0; i < StaticData.MarketObjects.Count; i++)
			{
				MarketObject marketObject = StaticData.MarketObjects.ElementAt(i).Value;

				sb.Append($"\nGroup: {marketObject.GroupName}\n");

				for (int j = 0; j < marketObject.ItemCount; j++)
				{
					MarketItem marketItem = marketObject.GetItemByIndex(j);

					sb.Append($"\n  {marketItem.ItemName}   Average Price: {marketItem.AveragePrice}");
				}

				sb.Append($"\n\n");
			}
		}
	}
}
