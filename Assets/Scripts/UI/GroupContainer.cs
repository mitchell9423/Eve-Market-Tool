using System;
using UnityEngine;

namespace EveMarket.UI
{
	[Serializable]
	public class GroupContainer : MonoBehaviour
	{
		[SerializeField] public GroupHeader GroupHeader;
		[SerializeField] public ItemContianer ItemContianer;

		[SerializeField] public GameObject Sizebox;
		[SerializeField] public GameObject Border;

		private void Awake()
		{
			GroupHeader = GetComponentInChildren<GroupHeader>();
			ItemContianer = GetComponentInChildren<ItemContianer>();
		}
	}
}
