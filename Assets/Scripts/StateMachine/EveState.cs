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

	public class Authentication : IEveState
	{
		private bool completed = false;

		public void Enter()
		{
		}

		public IEnumerator Execute()
		{
			Task<bool> logInTask = LoginManager.Login();
			yield return new WaitUntil(() => logInTask.IsCompleted);

			if (logInTask.Result == true)
			{
				EveStateMachine.SetNextState(new VerifyToken(), AppState.VerifyToken);
			}

			completed = true;
			yield return null;
		}

		public void Exit()
		{
			completed = true;
		}

		public bool IsCompleted()
		{
			return completed;
		}
	}

	public class VerifyToken : IEveState
	{
		private bool completed = false;

		public void Enter() { }

		public IEnumerator Execute()
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

			completed = true;
			yield return null;
		}

		public void Exit() => completed = true;

		public bool IsCompleted() => completed;
	}


	public class UpdateCorpOrders : IEveState
	{
		private bool completed = false;
		private string accessToken;

		public void Enter()
		{
			accessToken = AppSettings.Settings.TokenResponse.AccessToken;
			if (StaticData.CorpOrderRecord == null)
			{
				StaticData.CorpOrderRecord = new CorpOrderRecord(0, null, DateTime.Now.ToString(), "");
			}
		}

		public IEnumerator Execute()
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

			completed = true;
			yield return null;
		}

		public void Exit()
		{
			completed = true;
		}

		public bool IsCompleted()
		{
			return completed;
		}
	}

	public class LoadAppSettings : IEveState
	{
		private bool completed = false;

		public void Enter()
		{
			completed = false;
		}

		public IEnumerator Execute()
		{
			yield return new WaitUntil(() => AppSettings.LoadAppSettings());

			EveStateMachine.SetNextState(new LoadStaticData(), AppState.LoadStaticData);
			completed = true;
		}

		public void Exit()
		{
		}

		public bool IsCompleted()
		{
			return completed;
		}
	}

	public class LoadStaticData : IEveState
	{
		private bool completed = false;

		public void Enter()
		{
			//Debug.Log($"Enter {GetType()}");
		}

		public IEnumerator Execute()
		{
			//Debug.Log($"Execute {GetType()}");
			if (StaticData.LoadStaticData())
			{
				EveStateMachine.SetNextState(new ConstructMarketObjects(), AppState.ConstructMarketObjects);
				//EveDelegate.StaticLoadComplete?.Invoke();
			}
			else
			{
				EveStateMachine.SetNextState(new UpdateStaticData(), AppState.UpdateStaticData);
			}

			yield return null;
			completed = true;
		}

		public void Exit()
		{
			//Debug.Log($"Exit {GetType()}");
			completed = true;
		}

		public bool IsCompleted()
		{
			return completed;
		}
	}

	public class UpdateStaticData : IEveState
	{
		private bool completed = false;

		public void Enter()
		{
			//Debug.Log($"Enter {GetType()}");
		}

		public IEnumerator Execute()
		{
			Task<bool> updateTask = StaticData.UpdateStaticData();

			//Debug.Log($"Execute {GetType()}");
			yield return new WaitUntil(() => updateTask.IsCompleted);

			if (updateTask.Exception != null)
			{
				Debug.LogError("Task failed: " + updateTask.Exception);
			}
			else
			{
				// Use the result from the task
				bool result = updateTask.Result;
				Debug.Log("Update Static Data completed with result: " + result);

				EveStateMachine.SetNextState(new UpdateMarketObjects(), AppState.UpdateMarketObjects);
			}

			completed = true; // Set your completion flag based on the task's result
		}

		public void Exit()
		{
			//Debug.Log($"Exit {GetType()}");
			completed = true;
		}

		public bool IsCompleted()
		{
			return completed;
		}
	}

	public class ConstructMarketObjects : IEveState
	{
		private bool completed = false;

		public void Enter()
		{
			//Debug.Log($"Enter {GetType()}");
		}

		public IEnumerator Execute()
		{
			//Debug.Log($"Execute {GetType()}");
			StaticData.ConstructMarketObjects();
			yield return null;
			completed = true;
		}

		public void Exit()
		{
			//Debug.Log($"Exit {GetType()}");
			completed = true;
		}

		public bool IsCompleted()
		{
			return completed;
		}
	}

	public class ConstructEveMarketUI : IEveState
	{
		private bool completed = false;

		public void Enter()
		{
			//Debug.Log($"Enter {GetType()}");
			EveDelegate.CreateUI?.Invoke();
		}

		public IEnumerator Execute()
		{
			//Debug.Log($"Execute {GetType()}");
			yield return null;
			completed = true;
		}

		public void Exit()
		{
			//Debug.Log($"Exit {GetType()}");
			completed = true;
		}

		public bool IsCompleted()
		{
			return completed;
		}
	}

	public class UpdateEveMarketUI : IEveState
	{
		private bool completed = false;

		public void Enter()
		{
			//Debug.Log($"Enter {GetType()}");
			EveDelegate.UpdateUI?.Invoke();
		}

		public IEnumerator Execute()
		{
			//Debug.Log($"Execute {GetType()}");
			yield return new WaitForNextFrameUnit();
			StaticData.UpdateChangedRecordsOnly = false;
			completed = true;
		}

		public void Exit()
		{
			//Debug.Log($"Exit {GetType()}");
			completed = true;
		}

		public bool IsCompleted()
		{
			return completed;
		}
	}

	public class UpdateMarketObjects : IEveState
	{
		private bool completed = false;

		public void Enter()
		{
			//Debug.Log($"Enter {GetType()}");
		}

		public IEnumerator Execute()
		{
			//Debug.Log($"Execute {GetType()}");
			yield return new WaitUntil(() => StaticData.UpdateMarketObjects());
			EveStateMachine.SetNextState(new UpdateEveMarketUI(), AppState.UpdateEveMarketUI);
			completed = true;
		}

		public void Exit()
		{
			//Debug.Log($"Exit {GetType()}");
			completed = true;
		}

		public bool IsCompleted()
		{
			return completed;
		}
	}

	public class SaveMarketData : IEveState
	{
		private bool completed = false;

		public void Enter()
		{
			//Debug.Log($"Enter {GetType()}");
		}

		public IEnumerator Execute()
		{
			//Debug.Log($"Execute {GetType()}");
			yield return new WaitUntil(() => StaticData.SaveMarketData());
			completed = true;
		}

		public void Exit()
		{
			//Debug.Log($"Exit {GetType()}");
			completed = true;
		}

		public bool IsCompleted()
		{
			return completed;
		}
	}

	public class UpdateMarketOrders : IEveState
	{
		private bool completed = false;

		public void Enter()
		{
			StaticData.UpdateChangedRecordsOnly = true;
			//Debug.Log($"Enter {GetType()}");
		}

		public IEnumerator Execute()
		{
			//Debug.Log($"Execute {GetType()}");

			lock (StaticData.MarketObjects)
			{
				foreach (var marketObject in StaticData.MarketObjects.Values)
				{
					foreach (var item in marketObject.Items.Values)
					{
						for (int k = 0; k < Enum.GetValues(typeof(Region)).Length - 1; k++)
						{
							Region region = (Region)k;
							bool isExpired = true;

							OrderRecord record = item.GetOrderRecord(region);

							if (record == null)
							{
								continue;
							}

							if (DateTime.TryParse(record.Expiration, out DateTime expiration))
							{
								isExpired = DateTime.Now >= expiration;
							}

							if (isExpired)
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

			//EveStateMachine.SetNextState(new UpdateEveMarketUI(), AppState.UpdateEveMarketUI);

			yield return null;
			completed = true;
		}

		public void Exit()
		{
			//Debug.Log($"Exit {GetType()}");
			completed = true;
		}

		public bool IsCompleted()
		{
			return completed;
		}
	}
}
