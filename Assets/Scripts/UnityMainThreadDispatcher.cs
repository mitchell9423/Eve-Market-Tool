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

		private void Update()
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

		public static void Enqueue(global::System.Action action)
		{
			lock (Instance?._executionQueue)
			{
				Instance?._executionQueue.Enqueue(action);
			}
		}

		public static void Log(string message)
        {
			Enqueue(() =>
			{
				Debug.Log(message);
			});
		}

		public static void LogWarning(string message)
		{
			Enqueue(() =>
			{
				Debug.LogWarning(message);
			});
		}

		public static void LogError(string message)
		{
			Enqueue(() =>
			{
				Debug.LogError(message);
			});
		}
	}
}

