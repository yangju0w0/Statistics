using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class StatisticsValueTypeReflectionInspector : Editor
{
	public List<Type> statisticsValueTypeList;
	private int selectedTypeIdx_;
	public Type selectedType
	{
		get { return statisticsValueTypeList.ElementAtOrDefault(selectedTypeIdx_); }
	}

	private string[] statisticsValueTypeLabelArray;
	protected Type statisticsValueType_ = typeof(IStatisticsValue);

	protected bool isTypeChanged_;

	private StatisticsValueTypeReflectionBehaviour reflectionTarget_;

	public virtual void OnEnable()
	{
		reflectionTarget_ = target as StatisticsValueTypeReflectionBehaviour;
		Debug.Assert(reflectionTarget_ != null);

		Assembly[] assembly = AppDomain.CurrentDomain.GetAssemblies();

		statisticsValueTypeList = assembly
			.SelectMany(assem => assem.GetTypes())
			.Where(type => statisticsValueType_.IsAssignableFrom(type))
			.Where(type => type.IsClass)
			.ToList();

		statisticsValueTypeLabelArray = statisticsValueTypeList
			.Select(type =>
			{
				bool hasChildValue = statisticsValueTypeList
					.Where(alltypes => type.Equals(alltypes) != true)
					.ToList()
					.Exists(alltypes => type.IsAssignableFrom(alltypes));

				if (hasChildValue)
				{
					return GetMenuItemName(type) + "/" + type.Name + " (★)";
				}
				return GetMenuItemName(type);
			})
			.ToArray();
	}

	protected string GetMenuItemName(Type type)
	{
		if (type.BaseType == null || type.BaseType.Equals(typeof(object)))
		{
			return type.Name;
		}
		return GetMenuItemName(type.BaseType) + "/" + type.Name;
	}

	public override void OnInspectorGUI()
	{
		GUI.enabled = !Application.isPlaying;

		selectedTypeIdx_ = statisticsValueTypeList.FindIndex(type => type.FullName.Equals(reflectionTarget_.valueTypeName));

		EditorGUI.BeginChangeCheck();
		selectedTypeIdx_ = EditorGUILayout.Popup("값 분류", selectedTypeIdx_, statisticsValueTypeLabelArray);
		isTypeChanged_ = EditorGUI.EndChangeCheck();
		if (isTypeChanged_)
		{
			reflectionTarget_.valueTypeName = statisticsValueTypeList[selectedTypeIdx_].FullName;
			Undo.RecordObject(reflectionTarget_, "Statistics Value Type Changed");
		}

		GUI.enabled = true;
	}
}
