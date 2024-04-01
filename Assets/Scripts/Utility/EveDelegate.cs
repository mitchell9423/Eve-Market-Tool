using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace EveMarket.Util
{
	public static class EveDelegate
	{
		public delegate void JobStatus();
		public static JobStatus StaticUpdateComplete;
		public static JobStatus MarketUpdateComplete;
		public static JobStatus StaticLoadComplete;

		public static void Subscribe(ref JobStatus _delegate, Action action)
		{
			// Unsubscribe the action to ensure it's not added if it already exists.
			Unsubscribe(ref _delegate, action);

			// Subscribe the action.
			_delegate += new JobStatus(action);
		}

		public static void Unsubscribe(ref JobStatus _delegate, Action action)
		{
			_delegate -= new JobStatus(action);
		}
	}
}
