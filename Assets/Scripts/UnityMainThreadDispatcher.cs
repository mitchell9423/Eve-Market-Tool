using System.Collections.Generic;
using System.Diagnostics;
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

		private static string GetMeta()
		{
			var frame = new StackTrace(true).GetFrame(2); // 1 to get caller
			var file = frame.GetFileName();

			if (string.IsNullOrEmpty(file))
            {
				return "";
			}

			var line = frame.GetFileLineNumber();

			frame = new StackTrace(true).GetFrame(4);
			var method = frame?.GetMethod();
			var dtype = method?.DeclaringType;
			var name = method?.Name;

			string relativePath = file.Replace(Application.dataPath, "Assets"); // make path relative for Unity
			string meta = $"\n{dtype}:{name}() (at <a href=\"{relativePath}\" line=\"{line}\">{relativePath}:{line}</a>)";
			return meta;
		}

		public static void Log(string message)
		{
			string meta = GetMeta();
			Enqueue(() =>
			{
				UnityEngine.Debug.Log($"{message}{meta}");
			});
		}

		public static void LogWarning(string message)
		{
			string meta = GetMeta();
			Enqueue(() =>
			{
				UnityEngine.Debug.LogWarning($"{message}{meta}");
			});
		}

		public static void LogError(string message)
		{
			string meta = GetMeta();
			Enqueue(() =>
			{
				UnityEngine.Debug.LogError($"{message}{meta}");
			});
		}
	}
}

