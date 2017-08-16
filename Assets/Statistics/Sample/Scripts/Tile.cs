using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	[SerializeField]
	private StatisticsExpression expression_;
	
	private void OnCollisionEnter(Collision collision)
	{
		StatisticsManager statisticsManager = collision.gameObject.GetComponent<StatisticsManager>();
		if (statisticsManager != null && expression_.IsComputableType(statisticsManager.statisticsValueType))
		{
			statisticsManager.AddExpression(expression_);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		StatisticsManager statisticsManager = collision.gameObject.GetComponent<StatisticsManager>();
		if (statisticsManager != null && expression_.IsComputableType(statisticsManager.statisticsValueType))
		{
			statisticsManager.RemoveExpression(expression_);
		}
	}
}
