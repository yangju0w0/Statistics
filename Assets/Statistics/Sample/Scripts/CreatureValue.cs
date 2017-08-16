using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureValue : IStatisticsValue
{
	public CreatureValue() { }

	public virtual void CloneValuesFrom(IStatisticsValue value)
	{
		CreatureValue creatureValue = value as CreatureValue;

		if (creatureValue == null)
		{
			return;
		}

		maxHp = creatureValue.maxHp;
	}

	public int maxHp = 100;
}
