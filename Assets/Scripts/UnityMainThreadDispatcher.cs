using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace EveMarket.Util
{
	[ExecuteAlways]
	public class UnityMainThreadDispatcher : MonoBehaviour
	{
		private readonly Queue<System.Action> _executionQueue = new Queue<System.Action>();

		public static UnityMainThreadDispatcher Instance { get; set; }

		private void Awake()
		{
			Instance = this;
		}

		public void Enqueue(System.Action action)
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
				System.Action action;
				lock (_executionQueue)
				{
					action = _executionQueue.Dequeue();
				}
				action?.Invoke();
			}
		}
	}
}

