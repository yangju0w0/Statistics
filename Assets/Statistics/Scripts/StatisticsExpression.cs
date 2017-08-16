using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif

[Serializable]
public class StatisticsExpression : StatisticsValueTypeReflectionBehaviour
{
	[SerializeField]
	private bool useLevel_ = false;
	public bool useLevel
	{
		get { return useLevel_; }
		set
		{
			var old = useLevel_;
			useLevel_ = value;
			if (expressionChanged != null && old != useLevel_)
				expressionChanged.Invoke(this);
		}
	}

	[SerializeField]
	private int level_ = 1;
	public int level
	{
		get { return level_; }
		set
		{
			var old = level_;
			level_ = Mathf.Max(value, 0);

			if (expressionChanged != null && level_ != old)
				expressionChanged.Invoke(this);
		}
	}

	[Serializable]
	public enum ExpressionType
	{
		Level,
		Equipment,
		Artifact,
		Condition
	}

	[SerializeField]
	private ExpressionType expressionType_ = ExpressionType.Level;
	public ExpressionType expressionType
	{
		get { return expressionType_; }
		set
		{
			var old = expressionType_;
			expressionType_ = value;

			if (expressionChanged != null && expressionType_ != old)
				expressionChanged.Invoke(this);
		}
	}

	[SerializeField]
	private int expressionPriority_ = 0;
	public int expressionPriority
	{
		get { return expressionPriority_; }
		set
		{
			var old = expressionPriority_;
			expressionPriority_ = value;

			if (expressionChanged != null && expressionPriority_ != old)
				expressionChanged.Invoke(this);
		}
	}

	public delegate void ExpressionChanged(StatisticsExpression target);
	public event ExpressionChanged expressionChanged;

	[Serializable]
	// Use for initialization
	public class FieldExpression
	{
		[Serializable]
		public enum OperatorType
		{
			Addition,
			Multiply
		}

		public bool enabled = false;

		public string fieldName;

		public OperatorType operatorType = OperatorType.Addition;
		public float operandValue = 0;
		public float operandValueIncreasementPerLevel = 0;
		public float ApplyExpression(float value, int level)
		{
			float result = value;
			if (enabled)
			{
				float operand = operandValue + (operandValueIncreasementPerLevel * level);

				if (operatorType == OperatorType.Addition)
				{
					result += operand;
				}
				else if (operatorType == OperatorType.Multiply)
				{
					result *= operand;
				}
			}

			return result;
		}
	}

	[SerializeField]
	// Use for initialization
	private List<FieldExpression> fieldExpressionList_ = new List<FieldExpression>();

	public bool IsComputableType(Type type)
	{
		return valueType.IsAssignableFrom(type);
	}

	public bool ApplyExpression(IStatisticsValue target)
	{
		if (IsComputableType(target.GetType()) == false)
		{
			Debug.LogAssertionFormat("{0}에 {1}을 적용할 수 없습니다.", target.GetType().Name, valueType.Name);
			return false;
		}

		fieldExpressionList_.ForEach(expression =>
		{
			if (expression.enabled == false)
			{
				return;
			}

			FieldInfo field = valueType.GetField(expression.fieldName);

			if (field == null)
			{
				Debug.LogAssertionFormat("{0}에 해당하는 필드를 찾을 수 없습니다.", expression.fieldName);
				return;
			}

			float valueResult = expression.ApplyExpression((float)Convert.ToDouble(field.GetValue(target)), useLevel ? level_ : 0);
			field.SetValue(target, Convert.ChangeType(valueResult, field.FieldType));
		});

		return true;
	}

#if UNITY_EDITOR
	public void UpdateExpressionList()
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
						join expression in fieldExpressionList_
						on info.Name equals expression.fieldName into joinResult
						from expression in joinResult.DefaultIfEmpty(new FieldExpression() { fieldName = info.Name })
						select expression;

		fieldExpressionList_ = fieldJoin.ToList();
	}

	[DidReloadScripts]
	private static void OnScriptsReloaded()
	{
		FindObjectsOfType<StatisticsExpression>().ToList().ForEach(expression => expression.UpdateExpressionList());
	}
#endif
}
