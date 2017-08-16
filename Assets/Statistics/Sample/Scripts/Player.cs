using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField]
	private StatisticsManager statisticsManager_;
	private Rigidbody rb_;
	public Statistics<PlayerValue> statistics;

	protected void Start()
	{
		rb_ = GetComponent<Rigidbody>();
		statistics = statisticsManager_.GetStatisticsInstance<PlayerValue>();
	}

	// Update is called once per frame
	protected void FixedUpdate()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		
		if(Input.GetKey(KeyCode.Space) && Physics.Raycast(transform.position, Vector3.down, 0.5f))
		{
			rb_.AddForce(Vector3.up * statistics.currentStat.jumpForce, ForceMode.Impulse);
		}

		transform.Translate(new Vector3(-vertical, 0, horizontal) * statistics.currentStat.speed * Time.fixedDeltaTime);
	}
}
