using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace GameEvent
{
	[Serializable]
	public class CustomGameEvent : UnityEvent<Component, object> { }

	public class GameEventListener : MonoBehaviour
	{
		[SerializeField] GameEvent gameEvent;
		[SerializeField] CustomGameEvent customUnityEvent;

		private void Awake() => gameEvent.Register(gameEventListener: this);
		private void OnDestroy() => gameEvent.Deregister(gameEventListener: this);
		public void RaiseEvent(Component sender, object data) => customUnityEvent.Invoke(sender, data);
	}
}
