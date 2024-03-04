using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace GameEvent
{
	[Serializable]
	public class CustomGameEvent : UnityEvent<Component, object> { }

	[Serializable]
	public class GameEventContainer
	{
		[SerializeField] public GameEvent gameEvent;
		[SerializeField] public CustomGameEvent customUnityEvent;
	}

	public class GameEventListener : MonoBehaviour
	{
		[SerializeField] List<GameEventContainer> gameEventListeners = new List<GameEventContainer>();

		private void Awake()
		{
			for (int i = 0; i < gameEventListeners.Count; i++)
			{
				if (gameEventListeners[i] == null) gameEventListeners[i] = new GameEventContainer();
				gameEventListeners[i].gameEvent.Register(gameEventListener: this);
			}
		}

		private void OnDestroy()
		{
			for (int i = 0; i < gameEventListeners.Count; i++)
			{
				gameEventListeners[i].gameEvent.Deregister(gameEventListener: this);
			}
		}

		public void RaiseEvent(Component sender, object data)
		{
			for (int i = 0; i < gameEventListeners.Count; i++)
			{
				gameEventListeners[i].customUnityEvent.Invoke(sender, data);
			}
		}
	}
}
