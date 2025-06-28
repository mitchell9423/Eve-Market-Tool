using System.Collections;
using UnityEngine;

namespace EveMarket.StateMachine
{
	public class EveStateMachine : MonoBehaviour
	{
		public static AppState AppState { get; private set; }
		public static IEveState CurrentState { get; private set; }
		static IEveState NextState { get; set; }
		static bool CancelState { get; set; } = false;

		Coroutine StateMachine_Co;
		Coroutine ActiveState_Co;

		bool isRunning;

		// Start is called before the first frame update
		void Start()
		{
			StateMachine_Co = StartCoroutine(RunStateMachine());
		}

		private void OnDestroy()
		{
			isRunning = false;

			if (StateMachine_Co != null)
			{
				StopCoroutine(StateMachine_Co);
			}
		}

		IEnumerator RunStateMachine()
		{
			isRunning = true;
			while (isRunning)
			{
				if (NextState != null)
				{
					ChangeState();
				}
				else if (CurrentState.IsCompleted())
				{
					AppState = AppState.Idle;
				}

				yield return new WaitForSeconds(.2f);
			}
		}

		public static void SetNextState(IEveState eveState, AppState appState, bool interrupt = false)
		{
			if (eveState == CurrentState) return;

			AppState = appState;
			CancelState = interrupt;
			NextState = eveState;
		}

		private void ChangeState()
		{
			if (CancelState) CurrentState.Exit();

			if (CurrentState == null || CurrentState.IsCompleted())
			{
				if (ActiveState_Co != null)
					StopCoroutine(ActiveState_Co);

				if (CurrentState != null)
					CurrentState.Exit();

				CurrentState = NextState;
				NextState = null;
				CurrentState.Enter();
				ActiveState_Co = StartCoroutine(CurrentState.Execute());
			}
		}
	}
}
