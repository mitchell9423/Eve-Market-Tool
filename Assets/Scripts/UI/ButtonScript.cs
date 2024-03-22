using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvent;

namespace EveMarket
{
	public class ButtonScript : MonoBehaviour
	{
		private Component sender;
		private object data;

		public GameEvent.GameEvent buttonEvent;

		public void InitButtonEvent(Component sender = null, object data = null)
		{
			this.sender = sender??this;
			this.data = data;
		}

		public void RaiseEvent()
		{
			buttonEvent?.Invoke(sender, data);
		}
	}
}
