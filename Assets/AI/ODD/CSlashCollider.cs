/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AI.ODD
{
	public class CSlashCollider : IWeaponCollider
	{
		const float SlashDuration = 4f / 24f;
		float m_AttackTime;
		Def.OddAttackType m_AttackType;

		[SerializeField] Vector3[] m_Scale = new Vector3[Def.OddAttackTypeCount];
		[SerializeField] Vector3[] m_PosOffset = new Vector3[Def.OddAttackTypeCount];
		[SerializeField] Vector3[] m_StartRotation = new Vector3[Def.OddAttackTypeCount];
		[SerializeField] Vector3[] m_EndRotation = new Vector3[Def.OddAttackTypeCount];
		[SerializeField] float[] m_AttackDuration = new float[Def.OddAttackTypeCount];
		[SerializeField] Vector3[] m_HandRotationAxis = new Vector3[Def.OddAttackTypeCount];

		private void OnTriggerEnter(Collider other)
		{
			(m_Weapon as CMeleeWeapon).OnColliderEnter(m_Collider, other);
		}
		private void Update()
		{
			var attackIdx = (int)m_AttackType;
			var duration = m_AttackDuration[attackIdx];
			if(m_AttackTime == duration)
			{
				OnColliderDeactivation();
				return;
			}
			var startRot = m_StartRotation[attackIdx];
			var endRot = m_EndRotation[attackIdx];
			m_AttackTime = Mathf.Min(m_AttackTime + Time.deltaTime, duration);
			var angle = Vector3.LerpUnclamped(startRot, endRot, m_AttackTime / duration);

			transform.localRotation = Quaternion.Euler(angle);
		}
		public override void OnColliderActivation(Def.OddAttackType attackType)
		{
			base.OnColliderActivation(attackType);

			gameObject.SetActive(true);
			m_AttackType = attackType;

			transform.localScale = m_Scale[(int)m_AttackType];
			transform.localRotation = Quaternion.Euler(m_StartRotation[(int)m_AttackType]);
			transform.localPosition = m_PosOffset[(int)m_AttackType];

			m_AttackTime = 0f;
		}
		public override void OnColliderDeactivation()
		{
			base.OnColliderDeactivation();

			gameObject.SetActive(false);
		}
		public override void OnArmsRotation(float angleToTarget, float chestHandAngle)
		{
			base.OnArmsRotation(angleToTarget, chestHandAngle);

			var axis = m_HandRotationAxis[(int)m_AttackType];

			transform.Rotate(axis * angleToTarget, Space.Self);
		}
	}
}
