
using EveMarket.Network;
using EveMarket.StateMachine;
using EveMarket.UI;
using EveMarket.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EveMarket
{
	public class EveMarket : MonoBehaviour
	{
		[SerializeField] bool EnableLogginProcess = true;

		public ObjectType objectType = ObjectType.MarketGroup;

		//[SerializeField] UnityMainThreadDispatcher unityMainThreadDispatcher;
		//[SerializeField] HttpHandler httpHandler;
		//[SerializeField] EveSSOAuthenticator Authenticator;
		//[SerializeField] EveStateMachine stateMachine;
		[SerializeField] DisplayPanel displayPanel;

		public static bool EnableTimedUpdate { get; set; } = true;
		public static bool ShowGUI { get; set; }
		private static DateTime LastUpdate { get; set; }
		public static TimeSpan TimerInterval { get; set; } = new TimeSpan(0, 10, 0);
		public static TimeSpan TimeSinceLastUpdate { get; set; }
		public static TimeSpan RemainingTime { get; set; }

		StringBuilder sb = new StringBuilder();

        private void Awake()
        {
			NetworkManager.EnableLogin = EnableLogginProcess;
        }

        private void Start()
		{
			Application.runInBackground = true;

			EveDelegate.Subscribe(ref EveDelegate.CreateUI, displayPanel.CreateGroupContainers);

			if (displayPanel == null)
			{
				displayPanel = FindObjectOfType<DisplayPanel>();
			}

			if (!gameObject.TryGetComponent(out EveStateMachine stateMachine))
			{
				stateMachine = gameObject.AddComponent<EveStateMachine>();
			}

			if (!gameObject.TryGetComponent(out UnityMainThreadDispatcher unityMainThreadDispatcher))
			{
				unityMainThreadDispatcher = gameObject.AddComponent<UnityMainThreadDispatcher>();
			}

			if (!gameObject.TryGetComponent(out HttpHandler httpHandler))
			{
				httpHandler = gameObject.AddComponent<HttpHandler>();
			}
			/*

			if (!gameObject.TryGetComponent(out Authenticator))
			{
				Authenticator = gameObject.AddComponent<EveSSOAuthenticator>();
			}
			Authenticator.SetCodeReceivedCallback(code =>
			{
				UnityEngine.Debug.LogError($"SetCodeReceivedCallback Called");
				Debug.Log("Received OAuth code: " + code);
				// Further processing like exchanging the code for a token
				Authenticator.OnAuthCodeReceived(code);
			});
			*/
			EveStateMachine.SetNextState(new LoadAppSettings(), AppState.LoadAppSettings);

			StartCoroutine(TimedUpdate());
		}

		private void OnDestroy()
		{
			EveDelegate.Unsubscribe(ref EveDelegate.CreateUI, displayPanel.CreateGroupContainers);

			EveDelegate.Unsubscribe(ref EveDelegate.StaticLoadComplete, ConstructMarketObjects);
			EveDelegate.Unsubscribe(ref EveDelegate.PresetChanged, UpdateMarketObjects);
			//EveDelegate.Unsubscribe(ref EveDelegate.AppSettingsChanged, UpdateSettings);
			EveDelegate.Unsubscribe(ref EveDelegate.ResetAutoUpdateTimer, ResetTimer);
		}

		private void Update()
		{
			TimeSinceLastUpdate = DateTime.Now - LastUpdate;
			RemainingTime = TimerInterval - TimeSinceLastUpdate;
		}

		public void UpdateMarketObjects() => StaticData.UpdateMarketObjects();
		public void ConstructMarketObjects() => StaticData.ConstructMarketObjects();

		public IEnumerator TimedUpdate()
		{
			if (EnableTimedUpdate)
			{
				yield return new WaitForSeconds(60);

				if (EveStateMachine.AppState == AppState.Idle)
				{
					EveStateMachine.SetNextState(new UpdateCorpOrders(), AppState.UpdateCorpOrders);
				}

				StartCoroutine(TimedUpdate());
			}
		}

		private void ResetTimer()
		{
			LastUpdate = DateTime.Now;
		}

		//public void LoadStaticData()
		//{
		//	StaticData.LoadStaticData();
		//	//ui.CreateGroupContainers();
		//}

		//public void UpdateStaticData()
		//{
		//	StaticData.UpdateStaticData();
		//}

		public void UpdateMarketData(Component sender, object obj)
		{
			StaticData.UpdateMarketData(obj as List<int>);
		}

		private void UpdateSettings()
		{
			EnableTimedUpdate = AppSettings.Settings.EnableTimedUpdate;
			Debug.Log($"EnableTimedUpdate = {EnableTimedUpdate}");
			//ui.SetProfitMargin();
		}
	}
}
