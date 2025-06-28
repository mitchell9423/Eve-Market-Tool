using EveMarket.Network;
using EveMarket.Network.OAuth;
using EveMarket.Util;
using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace EveMarket.StateMachine
{
	public interface IEveState
	{
		void Enter();
		IEnumerator Execute();
		void Exit();
		bool IsCompleted();
	}

    public class EveMarketState : IEveState
	{
		protected bool completed = false;

		public virtual void Enter() { }

		public virtual IEnumerator Execute()
        {
			completed = true;
			yield return null;
        }

		public virtual void Exit() { }

		public bool IsCompleted() => completed;
    }

    public class Authentication : EveMarketState
	{
		public override IEnumerator Execute()
		{
			Task<bool> logInTask = LoginManager.Login();
			yield return new WaitUntil(() => logInTask.IsCompleted);

			if (logInTask.Result == true)
			{
				EveStateMachine.SetNextState(new VerifyToken(), AppState.VerifyToken);
			}
            else
            {
				EveStateMachine.SetNextState(new UpdateMarketOrders(), AppState.UpdateMarketOrders);
			}

			yield return base.Execute();
		}
	}

	public class VerifyToken : EveMarketState
	{
		public override IEnumerator Execute()
		{
			Task<bool> verifyTask = HttpHandler.instance.VerifyToken(); // assume returns true if valid

			yield return new WaitUntil(() => verifyTask.IsCompleted);

			if (verifyTask.Result)
			{
				EveStateMachine.SetNextState(new UpdateCorpOrders(), AppState.UpdateCorpOrders);
			}
			else
			{
				EveStateMachine.SetNextState(new Authentication(), AppState.Authentication);
			}

			yield return base.Execute();
		}
	}


	public class UpdateCorpOrders : EveMarketState
	{
		private string accessToken;

		public override void Enter()
		{
			base.Enter();
			accessToken = AppSettings.Settings.TokenResponse.AccessToken;
			if (StaticData.CorpOrderRecord == null)
			{
				StaticData.CorpOrderRecord = new CorpOrderRecord(0, null, DateTime.Now.ToString(), "");
			}
		}

		public override IEnumerator Execute()
		{
			if (HttpHandler.instance.IsExpired(StaticData.CorpOrderRecord.Expiration))
			{
				EveMarketRequest request = new EveMarketRequest(
					extension: accessToken,
					type_id: AppSettings.Settings.CorpId,
					etag: StaticData.CorpOrderRecord.ETag
					);

				yield return NetworkManager.AsyncRequest<CorpOrder>(request);
			}
			else
			{
				EveStateMachine.SetNextState(new UpdateMarketOrders(), AppState.UpdateMarketOrders);
			}

			yield return base.Execute();
		}
	}

	public class LoadAppSettings : EveMarketState
	{
		public override IEnumerator Execute()
		{
			yield return new WaitUntil(() => AppSettings.LoadAppSettings());

			EveStateMachine.SetNextState(new LoadStaticData(), AppState.LoadStaticData);
			yield return base.Execute();
		}
	}

	public class LoadStaticData : EveMarketState
	{
		public override IEnumerator Execute()
		{
			if (StaticData.LoadStaticData())
			{
				EveStateMachine.SetNextState(new ConstructMarketObjects(), AppState.ConstructMarketObjects);
				//EveDelegate.StaticLoadComplete?.Invoke();
			}
			else
			{
				EveStateMachine.SetNextState(new UpdateStaticData(), AppState.UpdateStaticData);
			}

			yield return base.Execute();
		}
	}

	public class UpdateStaticData : EveMarketState
	{
		public override IEnumerator Execute()
		{
			Task<bool> updateTask = StaticData.UpdateStaticData();

			yield return new WaitUntil(() => updateTask.IsCompleted);

			if (updateTask.Exception != null || updateTask.Result)
			{
				Debug.LogError($"Static Data Update Failed!\n{updateTask.Exception}");
			}
			else
			{
				// Use the result from the task
				bool result = updateTask.Result;
				Debug.Log("Static Data Update Complete." + result);

				EveStateMachine.SetNextState(new UpdateMarketObjects(), AppState.UpdateMarketObjects);
			}

			// Set your completion flag based on the task's result
			yield return base.Execute();
		}
	}

	public class ConstructMarketObjects : EveMarketState
	{
		public override IEnumerator Execute()
		{
			StaticData.ConstructMarketObjects();
			yield return base.Execute();
		}
	}

	public class ConstructEveMarketUI : EveMarketState
	{
        public override void Enter()
        {
            base.Enter();
			EveDelegate.CreateUI?.Invoke();
		}
    }

	public class UpdateEveMarketUI : EveMarketState
	{
		public override IEnumerator Execute()
		{
			yield return new WaitForNextFrameUnit();
			StaticData.UpdateChangedRecordsOnly = false;
			yield return base.Execute();
		}
	}

	public class UpdateMarketObjects : EveMarketState
	{
		public override IEnumerator Execute()
		{
			yield return new WaitUntil(() => StaticData.UpdateMarketObjects());
			EveStateMachine.SetNextState(new UpdateEveMarketUI(), AppState.UpdateEveMarketUI);
			yield return base.Execute();
		}
	}

	public class SaveMarketData : EveMarketState
	{
		public override IEnumerator Execute()
		{
			yield return new WaitUntil(() => StaticData.SaveMarketData());
			yield return base.Execute();
		}
	}

	public class UpdateMarketOrders : EveMarketState
	{
		public override IEnumerator Execute()
		{
			lock (StaticData.MarketObjects)
			{
				foreach (var marketObject in StaticData.MarketObjects.Values)
				{
					foreach (var item in marketObject.Items.Values)
					{
						for (int k = 0; k < Enum.GetValues(typeof(Region)).Length - 1; k++)
						{
							Region region = (Region)k;

							OrderRecord record = item.GetOrderRecord(region);

							if (record == null) continue;

							if (HttpHandler.instance.IsExpired(record.Expiration))
							{
								EveMarketRequest eveMarketRequest = new EveMarketRequest(
									type_id: item.TypeId,
									region: region,
									etag: record.ETag
									);

								StaticData.UpdateItemMarketData(eveMarketRequest);
							}

							yield return null;
						}
					}
				}
			}

			yield return base.Execute();
		}
	}
}
