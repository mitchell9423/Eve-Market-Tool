using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EveMarket.UI
{
	[Serializable]
	public class GroupContainer : MonoBehaviour
	{
		[SerializeField] public GroupHeader GroupHeader;
		[SerializeField] public ItemContianer ItemContianer;

		private void Awake()
		{
			GroupHeader = GetComponentInChildren<GroupHeader>();
			ItemContianer = GetComponentInChildren<ItemContianer>();
		}
	}
}
