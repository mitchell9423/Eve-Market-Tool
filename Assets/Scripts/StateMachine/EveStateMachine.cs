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

		// Start is called before the first frame update
		void Start()
		{
			StateMachine_Co = StartCoroutine(RunStateMachine());
		}

		private void OnDestroy()
		{
			if (StateMachine_Co != null)
			{
				StopCoroutine(StateMachine_Co);
			}
		}

		IEnumerator RunStateMachine()
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

			StateMachine_Co = StartCoroutine(RunStateMachine());
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
				//Debug.Log($"State Changed to {CurrentState.GetType()}");
				CurrentState.Enter();
				ActiveState_Co = StartCoroutine(CurrentState.Execute());
			}
		}
	}
}
