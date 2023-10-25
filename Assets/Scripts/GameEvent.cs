using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvent
{
	[CreateAssetMenu(menuName = "Game Event", fileName = "New Game Event")]
	public class GameEvent : ScriptableObject
	{
		HashSet<GameEventListener> _listeners = new HashSet<GameEventListener>();

		public void Invoke(Component sender, object data)
		{
			foreach (var globalEventListener in _listeners)
			{
				globalEventListener.RaiseEvent(sender, data);
			}
		}

		public void Register(GameEventListener gameEventListener) => _listeners.Add(gameEventListener);
		public void Deregister(GameEventListener gameEventListener) => _listeners.Remove(gameEventListener);
	}
}
