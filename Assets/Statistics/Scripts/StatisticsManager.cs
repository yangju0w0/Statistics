using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif

[Serializable]
public class StatisticsManager : StatisticsValueTypeReflectionBehaviour
{
	// TDOO : float 이외의 타입에도 적용할 수 있도록 변경해야 함
	[Serializable]
	// Use for initialization
	public class FieldValue
	{
		public string fieldName;
		public float fieldValue;
	}

	[SerializeField]
	// Use for initialization
	private List<FieldValue> fieldValueList_ = new List<FieldValue>();

	[SerializeField]
	private List<StatisticsExpression> expressionList_ = new List<StatisticsExpression>();

	public const string FIELD_DEFAULT_STAT = "defaultStat_";
	public const string FIELD_CURRENT_STAT = "currentStat_";

	// == Runtime values ==
	protected object statistics_;

	private Type statisticsType_;
	public Type statisticsType
	{
		get { return statisticsType_; }
	}

	private Type statisticsValueType_;
	public Type statisticsValueType
	{
		get { return statisticsValueType_; }
	}

	IStatisticsValue statisticsDefaultValue_;
	IStatisticsValue statisticsCurrentValue_;

	public Statistics<T> GetStatisticsInstance<T>() where T : IStatisticsValue, new()
	{
		if (typeof(T).IsAssignableFrom(statisticsValueType_) == false)
			return null;

		Type genericType = typeof(Statistics<>).MakeGenericType(typeof(T));
		ConstructorInfo constructor = genericType.GetConstructor(new[] { this.GetType(), typeof(T), typeof(T) });
		object result = constructor.Invoke(new object[] { this, (T)statisticsDefaultValue_, (T)statisticsCurrentValue_ });

		return result as Statistics<T>;
	}

	public void AddExpression(StatisticsExpression expression)
	{
		expressionList_.Add(expression);
		expression.expressionChanged += target => UpdateCurrentValue();
		UpdateCurrentValue();
	}
	
	public void RemoveExpression(StatisticsExpression expression)
	{
		expressionList_.Remove(expression);
		expression.expressionChanged -= target => UpdateCurrentValue();
		UpdateCurrentValue();
	}

	public void ForEachExpression(Action<StatisticsExpression> action)
	{
		expressionList_.ForEach(action);
	}

	public StatisticsExpression FindExpression(Predicate<StatisticsExpression> match)
	{
		return expressionList_.Find(match);
	}

	// Unity Built-in Method
	protected override void Awake()
	{
		base.Awake();

		statisticsValueType_ = valueType;
		statisticsType_ = typeof(Statistics<>).MakeGenericType(statisticsValueType_);
		ConstructorInfo constructor = statisticsType_.GetConstructor(new[] { this.GetType() });
		statistics_ = constructor.Invoke(new object[] { this });

		statisticsDefaultValue_ = (IStatisticsValue)statisticsType_.GetField(FIELD_DEFAULT_STAT, BindingFlags.NonPublic|BindingFlags.Instance).GetValue(statistics_);
		statisticsCurrentValue_ = (IStatisticsValue)statisticsType_.GetField(FIELD_CURRENT_STAT, BindingFlags.NonPublic|BindingFlags.Instance).GetValue(statistics_);

		fieldValueList_.ForEach(value =>
		{
			FieldInfo field = statisticsValueType_.GetField(value.fieldName);

			if (field == null)
			{
				Debug.LogAssertionFormat("{0}에 해당하는 필드를 찾을 수 없습니다.", value.fieldName);
				return;
			}

			field.SetValue(statisticsDefaultValue_, Convert.ChangeType(value.fieldValue, field.FieldType));
		});

		expressionList_.ForEach(expression => expression.expressionChanged += target => UpdateCurrentValue());
		UpdateCurrentValue();
	}

	private void UpdateCurrentValue()
	{
		expressionList_.Sort((l, r) =>
		{
			int result = (int)l.expressionType - (int)r.expressionType;
			if (result == 0)
			{
				result = r.expressionPriority - l.expressionPriority;
			}
			return result;
		});

		statisticsCurrentValue_.CloneValuesFrom(statisticsDefaultValue_);
		expressionList_.ToList().ForEach(expression => expression.ApplyExpression(statisticsCurrentValue_));
	}

#if UNITY_EDITOR
	public void UpdateValueList()
	{
		Type selectedType = FindTypeByString(valueTypeName);

		if (FindTypeByString(valueTypeName) == null)
			return;

		FieldInfo[] fieldInfo = selectedType.GetFields()
			.Where(info => info.IsPublic)
			.Where(info => !info.IsStatic)
			.Where(info => info.FieldType.Equals(typeof(int)) || info.FieldType.Equals(typeof(long)) || info.FieldType.Equals(typeof(float)) || info.FieldType.Equals(typeof(double)))
			.ToArray();

		var fieldJoin = from info in fieldInfo
						join value in fieldValueList_
						on info.Name equals value.fieldName into joinResult
						from value in joinResult.DefaultIfEmpty(new FieldValue() { fieldName = info.Name })
						select value;

		fieldValueList_ = fieldJoin.ToList();
	}

	[DidReloadScripts]
	private static void OnScriptsReloaded()
	{
		FindObjectsOfType<StatisticsManager>().ToList().ForEach(value => value.UpdateValueList());
	}
#endif
}