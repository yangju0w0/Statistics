using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerValue : CreatureValue, IStatisticsValue
{
	public PlayerValue() : base() { }

	public override void CloneValuesFrom(IStatisticsValue value)
	{
		base.CloneValuesFrom(value);
		PlayerValue playerValue = value as PlayerValue;

		if (playerValue == null)
		{
			return;
		}
		
		speed = playerValue.speed;
		jumpForce = playerValue.jumpForce;
	}

	public float jumpForce = 1.0f;
	public float speed = 2.0f;
}
