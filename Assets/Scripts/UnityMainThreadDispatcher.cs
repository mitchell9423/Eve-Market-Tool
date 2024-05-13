using System.Collections.Generic;
using UnityEngine;

namespace EveMarket.Util
{
	[ExecuteAlways]
	public class UnityMainThreadDispatcher : MonoBehaviour
	{
		private readonly Queue<global::System.Action> _executionQueue = new Queue<global::System.Action>();

		public static UnityMainThreadDispatcher Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(this);
			}
		}

		public void Enqueue(global::System.Action action)
		{
			lock (_executionQueue)
			{
				_executionQueue.Enqueue(action);
			}
		}

		void Update()
		{
			while (_executionQueue.Count > 0)
			{
				global::System.Action action;
				lock (_executionQueue)
				{
					action = _executionQueue.Dequeue();
				}
				action?.Invoke();
			}
		}
	}
}

