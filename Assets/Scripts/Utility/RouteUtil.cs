using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EveMarket.Util
{
	public struct RouteData
	{
		public int Origin { get; set; }
		public int Destination { get; set; }
		public List<int> Route { get; set; }
		public int NumJumps { get; set; }

		public RouteData(int origin = 0, int destination = 0, List<int> route = null)
		{
			Origin = origin;
			Destination = destination;
			NumJumps = 0;

			if (route != null)
			{
				Route = route;
				NumJumps = SetNumJumps();
			}
			else
			{
				Route = new List<int>();
			}
		}

		private int SetNumJumps()
		{
			if (Route != null && Origin != Destination)
			{
				return Route.Count - 1;
			}

			return 0;
		}
	}

	public static class RouteUtil
	{
		public static RouteData GetRoute(ref RouteData data)
		{
			return data;
		}
	}
}
