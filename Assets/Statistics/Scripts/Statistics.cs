using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics<T> where T : IStatisticsValue, new()
{
	public Statistics(StatisticsManager manager)
	{
		defaultStat_ = new T();
		currentStat_ = new T();
	}

	public Statistics(StatisticsManager manager, T defaultStat, T currentStat)
	{
		defaultStat_ = defaultStat;
		currentStat_ = currentStat;
		statisticsManager_ = manager;
	}

	public T defaultStat { get { return defaultStat_; } }
	private T defaultStat_;

	public T currentStat { get { return currentStat_; } }
	private T currentStat_;

	public StatisticsManager statisticsManager
	{
		get { return statisticsManager_; }
	}
	private StatisticsManager statisticsManager_;

	// StatisticsManager Wrapper Methods
	public void AddExpression(StatisticsExpression expression)
	{
		statisticsManager_.AddExpression(expression);
	}

	public void RemoveExpression(StatisticsExpression expression)
	{
		statisticsManager_.RemoveExpression(expression);
	}

	public void ForEachExpression(Action<StatisticsExpression> action)
	{
		statisticsManager_.ForEachExpression(action);
	}

	public StatisticsExpression FindExpression(Predicate<StatisticsExpression> match)
	{
		return statisticsManager_.FindExpression(match);
	}
}
