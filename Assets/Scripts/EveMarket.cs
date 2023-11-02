
using System.Collections.Generic;
using UnityEngine;
using EveMarket.Util;
using Unity.VisualScripting;
using UnityEditor;
using System.Text;
using System.Linq;

namespace EveMarket
{
	[ExecuteAlways]
	public class EveMarket : MonoBehaviour
	{
		public ObjectType objectType = ObjectType.MarketGroup;

		[SerializeField] UnityMainThreadDispatcher unityMainThreadDispatcher;
		[SerializeField] HttpHandler httpHandler;
		[SerializeField] DisplayPanel displayPanel;

		public bool showGUI = false;

		public Dictionary<int, MarketObject> marketObjects = new Dictionary<int, MarketObject>();

		StringBuilder sb = new StringBuilder();

		private void OnEnable()
		{
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
			ConstructMarketObjects();
		}

		public void UpdateStaticData()
		{
			StaticData.UpdateStaticData();
		}

		public void ClearDisplay()
		{
			DisplayPanel.ClearDisplay();
		}

		public void ConstructMarketObjects()
		{
			lock (StaticData.groupObjects)
			{
				foreach (var group in StaticData.groupObjects.Values)
				{
					marketObjects[group.Id] = new MarketObject(group);
				}
			}

			BuildDisplayString();
		}

		void BuildDisplayString()
		{
			sb.Clear();

			for (int i = 0; i < marketObjects.Count; i++)
			{
				MarketObject marketObject = marketObjects.ElementAt(i).Value;

				sb.Append($"\nGroup: {marketObject.GroupName}\n");

				for (int j = 0; j < marketObject.ItemCount; j++)
				{
					MarketObject.MarketItem marketItem = marketObject.GetItemByIndex(j);

					sb.Append($"\n  {marketItem.ItemName}   Average Price: {marketItem.AveragePrice}");
				}

				sb.Append($"\n\n");
			}

			DisplayPanel.SetDisplayText(sb.ToString());
		}

		public void ToggleGUI()
		{
			DisplayPanel.showGUI = showGUI;
		}
	}
}
