
using System.Collections.Generic;
using UnityEngine;
using EveMarket.Util;
using Unity.VisualScripting;
using UnityEditor;

namespace EveMarket
{
	[ExecuteAlways]
	public class EveMarket : MonoBehaviour
	{
		[SerializeField] UnityMainThreadDispatcher unityMainThreadDispatcher;
		[SerializeField] HttpHandler httpHandler;
		[SerializeField] DisplayPanel displayPanel;

		[SerializeField] public List<IDataModel> modelList = new List<IDataModel>();

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
			EveDelegate.StaticLoadComplete += CreateObjectList;
			LoadStaticData();
		}

		public void LoadStaticData()
		{
			StaticData.LoadStaticData();
		}

		public void UpdateStaticData()
		{
			EveDelegate.StaticUpdateComplete += CreateObjectList;
			StaticData.UpdateStaticData();
		}

		public void ClearDisplay()
		{
			DisplayPanel.ClearDisplay();
		}

		public void CreateObjectList()
		{
			EveDelegate.StaticUpdateComplete -= CreateObjectList;
			EveDelegate.StaticLoadComplete -= CreateObjectList;
			modelList = StaticData.DataModels;
		}
	}
}
