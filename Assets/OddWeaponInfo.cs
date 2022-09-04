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

namespace Assets
{
	[CreateAssetMenu(fileName = "WeaponInfo", menuName = "Assets/WeaponInfo", order = 5)]
	public class OddWeaponInfo : ScriptableObject
	{
		[Header("Base Stats")]
		public string WeaponName = "Unknown Weapon";
		public Def.OddWeaponType WeaponType = Def.OddWeaponType.COUNT;
		public Sprite WeaponSprite;
		
		[Header("Collider")]
		public float[] ColliderActivationDelay;
		public Vector3 ColliderOffset;
		public Vector3 ColliderSize;

		[Header("Combat Stats")]
		public float Damage;
		public Def.DamageType DamageType;

		[Header("Cover Stats")]
		public float DamageReductionFixed = 0f;
		public float DamageReductionPCT = 0f;
		public float CoverRangeDegrees = 90f;

		[Header("Heavy Combat Stats")]
		public float HeavyDamage;
		public Def.DamageType HeavyDamageType;

		[Header("Melee Info")]
		[Header("VFX")]
		public UnityEngine.VFX.VisualEffectAsset[] VFXs;
		public Vector3[] PosOffsetOnAttack;
		public Vector3[] RotationOnAttack;
		public Vector3[] ScaleOnAttack;
		public float[] VFXActivationDelay;
	}
}
