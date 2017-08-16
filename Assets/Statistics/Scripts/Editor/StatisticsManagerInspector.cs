using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;

[CustomEditor(typeof(StatisticsManager))]
public class StatisticsManagerInspector : StatisticsValueTypeReflectionInspector
{
	private StatisticsManager target_;

	public override void OnEnable()
	{
		base.OnEnable();

		target_ = target as StatisticsManager;
		Debug.Assert(target_ != null);
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (isTypeChanged_)
		{
			target_.UpdateValueList();
		}


		if(Application.isPlaying)
		{
			GUILayout.Label("현재 값");
			ValueDisplay();
		}
		else
		{
			GUILayout.Label("초기값");
			var fieldValueList = typeof(StatisticsManager)
				.GetField("fieldValueList_", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(target_) as List<StatisticsManager.FieldValue>;

			fieldValueList.ForEach(field => ValueField(field));
		}

		GUILayout.Label("계산식 목록");
		GUI.enabled = !Application.isPlaying;
		ExpressionListField();
	}

	private void ValueField(StatisticsManager.FieldValue value)
	{
		EditorGUI.BeginChangeCheck();
		float fieldValue = EditorGUILayout.FloatField(value.fieldName, value.fieldValue);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Field Value Changed");
			value.fieldValue = fieldValue;
		}
	}

	private void ValueDisplay()
	{
		object statistics = typeof(StatisticsManager)
				.GetField("statistics_", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(target_);

		object defaultStat = target_.statisticsType.GetField(StatisticsManager.FIELD_DEFAULT_STAT, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(statistics);
		object currentStat = target_.statisticsType.GetField(StatisticsManager.FIELD_CURRENT_STAT, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(statistics);

		target_.statisticsValueType.GetFields()
			.Where(info => info.IsPublic)
			.Where(info => !info.IsStatic)
			.Where(info => info.FieldType.Equals(typeof(int)) || info.FieldType.Equals(typeof(long)) || info.FieldType.Equals(typeof(float)) || info.FieldType.Equals(typeof(double)))
			.ToList().ForEach(type =>
			{
				EditorGUILayout.BeginHorizontal();
				
				EditorGUILayout.LabelField(type.Name, GUILayout.MaxWidth(120));
				EditorGUILayout.LabelField(type.GetValue(defaultStat).ToString(), GUILayout.MaxWidth(30));
				
				EditorGUILayout.LabelField("=> " + type.GetValue(currentStat).ToString(), EditorStyles.boldLabel, GUILayout.MaxWidth(60));

				EditorGUILayout.EndHorizontal();
			});
	}

	private void ExpressionListField()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("expressionList_"), true);
		if (EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
		}
	}
}
