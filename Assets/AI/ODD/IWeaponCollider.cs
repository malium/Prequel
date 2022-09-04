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
	[RequireComponent(typeof(Collider))]
	public class IWeaponCollider : MonoBehaviour
	{
		protected Collider m_Collider;
		[SerializeField] protected IOddWeapon m_Weapon;

		private void Awake()
		{
			m_Collider = gameObject.GetComponent<Collider>();
		}
		public virtual void Init(IOddWeapon weapon)
		{
			m_Weapon = weapon;
		}
		public virtual void OnColliderActivation(Def.OddAttackType attackType)
		{

		}
		public virtual void OnColliderDeactivation()
		{

		}
		public virtual void OnArmsRotation(float angleToTarget, float chestHandAngle)
		{

		}
		public IOddWeapon GetWeapon() => m_Weapon;
		public Collider GetCollider() => m_Collider;
	}
}
