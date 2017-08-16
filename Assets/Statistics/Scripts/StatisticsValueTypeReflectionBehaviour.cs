using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatisticsValueTypeReflectionBehaviour : MonoBehaviour
{
	[SerializeField]
	public string valueTypeName;

	private Type valueType_;
	public Type valueType
	{
		get
		{
			return valueType_;
		}
	}

	protected Type FindTypeByString(string typename)
	{
		Type type = Type.GetType(typename);

		if (type == null)
		{
			Debug.LogAssertionFormat("{0}에 해당하는 타입을 찾을 수 없습니다.", typename);
			return type;
		}

		if (typeof(IStatisticsValue).IsAssignableFrom(type) == false)
		{
			Debug.LogAssertionFormat("{0}은 IStatisticsValue를 구현해야 합니다.", type.Name);
		}

		if (type.IsClass == false)
		{
			Debug.LogAssertionFormat("{0}은 클래스여야 합니다.", type.Name);
		}

		return type;
	}

	// Unity Built-in Method
	protected virtual void Awake()
	{
		valueType_ = FindTypeByString(valueTypeName);
	}
}
