using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EveMarket.Util
{
	public static class EveDelegate
	{
		public delegate void JobStatus();
		public static JobStatus StaticUpdateComplete;
		public static JobStatus StaticLoadComplete;
	}
}
