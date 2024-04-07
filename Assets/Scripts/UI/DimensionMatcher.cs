using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EveMarket.UI
{
	[ExecuteAlways]
	public class DimensionMatcher : MonoBehaviour
	{
		[SerializeField] private RectTransform targetRectTransform;
		[SerializeField] private RectTransform sourceRectTransform;

		private void Start()
		{
			//MatchRect();
		}

		private void OnRectTransformDimensionsChange()
		{
			MatchRect();
		}

		private void MatchRect()
		{
			if (targetRectTransform == null) return;

			if (sourceRectTransform == null)
			{
				sourceRectTransform = GetComponent<RectTransform>();
			}

			targetRectTransform.anchorMin = sourceRectTransform.anchorMin;
			targetRectTransform.anchorMax = sourceRectTransform.anchorMax;
			targetRectTransform.pivot = sourceRectTransform.pivot;
			targetRectTransform.sizeDelta = sourceRectTransform.sizeDelta;
			targetRectTransform.anchoredPosition = sourceRectTransform.anchoredPosition;
		}
	}
}
