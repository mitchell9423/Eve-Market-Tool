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

		static UnityMainThreadDispatcher instance; 
		public static UnityMainThreadDispatcher Instance 
		{ 
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<UnityMainThreadDispatcher>();
				}

				return instance;
			}
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

