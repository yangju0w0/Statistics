using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using UnityEditor.Callbacks;

[CustomEditor(typeof(StatisticsExpression))]
public class StatisticsExpressionInspector : StatisticsValueTypeReflectionInspector
{
	private StatisticsExpression target_;

	public override void OnEnable()
	{
		base.OnEnable();

		target_ = target as StatisticsExpression;
		Debug.Assert(target_ != null);
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		if (isTypeChanged_)
		{
			target_.UpdateExpressionList();
		}

		ExpressionTypeField();
		ExpressionPriorityField();
		UseLevelToggleField();
		LevelField();

		GUI.enabled = !Application.isPlaying;

		GUILayout.Label("계산식");
		var fieldExpressionList = typeof(StatisticsExpression)
							.GetField("fieldExpressionList_", BindingFlags.NonPublic | BindingFlags.Instance)
							.GetValue(target_) as List<StatisticsExpression.FieldExpression>;

		fieldExpressionList.ForEach(expression => ExpressionField(expression));
	}

	private void ExpressionTypeField()
	{
		EditorGUI.BeginChangeCheck();
		StatisticsExpression.ExpressionType expressionType = (StatisticsExpression.ExpressionType)EditorGUILayout.EnumPopup("계산식 분류", target_.expressionType);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Expression Type Changed");
			target_.expressionType = expressionType;
		}
	}

	private void ExpressionPriorityField()
	{
		EditorGUI.BeginChangeCheck();
		int priority = EditorGUILayout.IntField("우선순위 (숫자가 클수록 우선)", target_.expressionPriority);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Expression Priority Changed");
			target_.expressionPriority = priority;
		}
	}

	private void UseLevelToggleField()
	{
		EditorGUI.BeginChangeCheck();
		bool useLevel = EditorGUILayout.Toggle("레벨 사용", target_.useLevel);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Use Level Toggle Changed");
			target_.useLevel = useLevel;
		}
	}

	private void LevelField()
	{
		bool originEnabled = GUI.enabled;

		GUI.enabled = target_.useLevel && GUI.enabled;

		EditorGUI.BeginChangeCheck();
		int level = EditorGUILayout.IntField("레벨", target_.level);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Level Changed");
			target_.level = level;
		}

		GUI.enabled = originEnabled;
	}

	private void ExpressionField(StatisticsExpression.FieldExpression expression)
	{
		bool originEnabled = GUI.enabled;
		
		EditorGUILayout.BeginHorizontal();
			ExpressionToggleField(expression);
			GUI.enabled = expression.enabled && originEnabled;
			EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal();

					ExpressionOperatorTypeField(expression);
					EditorGUILayout.LabelField("(", GUILayout.Width(15));
					ExpressionOperandValueField(expression);

					GUI.enabled = target_.useLevel && GUI.enabled;
					EditorGUILayout.LabelField("+ (", GUILayout.Width(21));
					ExpressionOperandIncreasementValueField(expression);
					EditorGUILayout.LabelField("× Level )", GUILayout.Width(60));

					GUI.enabled = expression.enabled && originEnabled;
					EditorGUILayout.LabelField(")", GUILayout.Width(15));

					EditorGUILayout.BeginVertical();
						ExpressionPreviewLabel(expression);
					EditorGUILayout.EndVertical();

				EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		GUI.enabled = originEnabled;
	}
	
	private void ExpressionOperandIncreasementValueField(StatisticsExpression.FieldExpression expression)
	{
		EditorGUI.BeginChangeCheck();
		float increasementValue = EditorGUILayout.FloatField(expression.operandValueIncreasementPerLevel);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Expression Increasement Value Changed");
			expression.operandValueIncreasementPerLevel = increasementValue;
		}
	}

	private void ExpressionOperandValueField(StatisticsExpression.FieldExpression expression)
	{
		EditorGUI.BeginChangeCheck();
		float operand = EditorGUILayout.FloatField(expression.operandValue);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Expression Operand Value Changed");
			expression.operandValue = operand;
		}
	}

	private static string[] operatorTypeIconCharacter_ = { "＋", "×" };
	private void ExpressionOperatorTypeField(StatisticsExpression.FieldExpression expression)
	{
		EditorGUI.BeginChangeCheck();
		int operatorType = EditorGUILayout.Popup((int)expression.operatorType, operatorTypeIconCharacter_, GUILayout.MaxWidth(40));
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Expression Operator Type Changed");
			expression.operatorType = (StatisticsExpression.FieldExpression.OperatorType) operatorType;
		}
	}
	
	private void ExpressionToggleField(StatisticsExpression.FieldExpression expression)
	{
		EditorGUI.BeginChangeCheck();
		bool enabled = EditorGUILayout.ToggleLeft(expression.fieldName, expression.enabled, GUILayout.MaxWidth(120));
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Expression Toggle Changed");
			expression.enabled = enabled;
		}
	}

	private void ExpressionPreviewLabel(StatisticsExpression.FieldExpression expression)
	{
		float previewValue = expression.operandValue + (target_.useLevel ? expression.operandValueIncreasementPerLevel * target_.level : 0);
		EditorGUILayout.LabelField("[" + operatorTypeIconCharacter_[(int)expression.operatorType] + previewValue + "]", GUILayout.MaxWidth(40));
	}
}
