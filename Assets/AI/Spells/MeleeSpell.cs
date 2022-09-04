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

namespace Assets.AI.Spells
{
	public class MeleeSpell : ISpell
	{
		ConfigEnum<Def.MeleeAttackType> m_AttackType;
		ConfigEnum<Def.SpellHitMask> m_HitMask;
		ConfigEnum<Def.SpellDamageMask> m_DamageMask;

		ConfigSpellWeapon m_SpellWeapon;
		ConfigVector2 m_SpellWeaponScale;
		ConfigBoolean m_FlipWeaponX;
		ConfigBoolean m_FlipWeaponY;
		ConfigVFXDef m_UnderWeaponEffect;
		ConfigFloat m_OffsetHeight;
		ConfigFloat m_OffsetForward;
		ConfigFloat m_UnderEffectOffsetForward;
		ConfigVector3 m_PrismBaseHeightDepth;
		ConfigVector3 m_ColliderOffset;
		ConfigBoolean m_SlashFromOffset;
		ConfigInteger m_ArcSize;
		ConfigInteger m_StartingOffset;
		ConfigBoolean m_Flip;
		ConfigBoolean m_Vertical;
		ConfigFloat m_PierceDistance;
		ConfigFloat m_Speed;
		ConfigFloat m_SlashFadeOutAngle;
		ConfigFloat m_PierceReturnSpeed;
		ConfigFloat m_SpriteAngleOffset;

		ConfigVFXDef m_CastVFX;
		ConfigVFXDef m_OnHitVFX;

		public void TriggerOnBlockHit(CBlock target)
		{
			ApplyOnHitEffects(target);
		}
		public void TriggerOnBridgeHit(BridgeComponent target)
		{
			ApplyOnHitEffects(target);
		}
		public void TriggerOnEntityHit(CLivingEntity target)
		{
			ApplyOnHitEffects(target);
		}
		public Def.MeleeAttackType GetAttackType() => m_AttackType.GetValue();
		public Def.SpellHitMask GetHitMask() => m_HitMask.GetValue();
		public Def.SpellDamageMask GetDamageMask() => m_DamageMask.GetValue();
		public string GetSpellWeapon() => m_SpellWeapon.GetValue();
		public Vector2 GetWeaponScale() => m_SpellWeaponScale.GetValue();
		public VFXDef GetUnderWeaponEffect() => m_UnderWeaponEffect.GetValue();
		public bool GetFlipWeaponX() => m_FlipWeaponX.GetValue();
		public bool GetFlipWeaponY() => m_FlipWeaponY.GetValue();
		public bool GetSlashFromOffset() => m_SlashFromOffset.GetValue();
		public float GetOffsetHeight() => m_OffsetHeight.GetValue();
		public float GetOffsetForward() => m_OffsetForward.GetValue();
		public float GetUnderEffectOffsetForward() => m_UnderEffectOffsetForward.GetValue();
		public Vector3 GetPrismBaseHeightDepth() => m_PrismBaseHeightDepth.GetValue();
		public Vector3 GetColliderOffset() => m_ColliderOffset.GetValue();
		public int GetArcSize() => m_ArcSize.GetValue();
		public int GetStartingOffset() => m_StartingOffset.GetValue();
		public bool GetFlip() => m_Flip.GetValue();
		public bool GetVertical() => m_Vertical.GetValue();
		public float GetPierceDistance() => m_PierceDistance.GetValue();
		public float GetSpeed() => m_Speed.GetValue();
		public float GetPierceReturnSpeed() => m_PierceReturnSpeed.GetValue();
		public float GetSlashFadeOutAngle() => m_SlashFadeOutAngle.GetValue();
		public float GetSpriteAngleOffset() => m_SpriteAngleOffset.GetValue();
		public VFXDef GetCastVFX() => m_CastVFX.GetValue();
		public VFXDef GetOnHitVFX() => m_OnHitVFX.GetValue();
		public MeleeSpell()
			:base("MeleeSpell")
		{
			m_AttackType = new ConfigEnum<Def.MeleeAttackType>("AttackType", Def.MeleeAttackType.Piercing);
			m_HitMask = new ConfigEnum<Def.SpellHitMask>("HitMask", Def.SpellHitMask.LIVINIG_ENTITIES);
			m_DamageMask = new ConfigEnum<Def.SpellDamageMask>("DamageMask", Def.SpellDamageMask.LIVINIG_ENTITIES);
			m_SpellWeapon = new ConfigSpellWeapon("SpellWeapon", "");
			m_SpellWeaponScale = new ConfigVector2("WeaponScale", Vector2.one);
			m_SlashFromOffset = new ConfigBoolean("SlashFromOffset", false);
			m_OffsetHeight = new ConfigFloat("OffsetHeight", 1f);
			m_OffsetForward = new ConfigFloat("OffsetForward", 0.5f);
			m_UnderEffectOffsetForward = new ConfigFloat("UnderEffectOffsetFWD", 0.5f);
			m_PrismBaseHeightDepth = new ConfigVector3("PrismBaseHeightDepth", Vector3.one);
			m_ColliderOffset = new ConfigVector3("ColliderOffset", Vector3.zero);
			m_ArcSize = new ConfigInteger("ArcSize", 60);
			m_StartingOffset = new ConfigInteger("StartingOffset", 0);
			m_Flip = new ConfigBoolean("Flip", false);
			m_Vertical = new ConfigBoolean("Vertical", false);
			m_PierceDistance = new ConfigFloat("PierceDistance", 1f);
			m_Speed = new ConfigFloat("Speed", 1f);
			m_PierceReturnSpeed = new ConfigFloat("PierceReturnSpeed", 1f);
			m_SlashFadeOutAngle = new ConfigFloat("SlashFadeOutAngle", 20f);

			m_FlipWeaponX = new ConfigBoolean("FlipWeaponX", false);
			m_FlipWeaponY = new ConfigBoolean("FlipWeaponY", false);
			m_UnderWeaponEffect = new ConfigVFXDef()
			{
				VFXTarget = new ConfigEnum<Def.VFXTarget>("Under_VFXTarget", Def.VFXTarget.MONSTER),
				VFXTypeName = new ConfigString("Under_VFXTypeName", ""),
				VFXType = new ConfigEnum<Def.VFXType>("Under_VFXType", Def.VFXType.CAST),
				VFXVersion = new ConfigInteger("Under_VFXVersion", 0),
				VFXFacing = new ConfigEnum<Def.VFXFacing>("Under_VFXFacing", Def.VFXFacing.DontFaceAnything),
				VFXEnd = new ConfigEnum<Def.VFXEnd>("Under_VFXEnd", Def.VFXEnd.SelfDestroy),
				VFXScale = new ConfigVector2("Under_VFXScale", Vector2.one),
				FPSOverride = new ConfigFloat("Under_FPSOverride", 0f)
			};

			m_SpriteAngleOffset = new ConfigFloat("SpriteAngleOffset", 0f);
			m_CastVFX = new ConfigVFXDef()
			{
				VFXTarget = new ConfigEnum<Def.VFXTarget>("Cast_VFXTarget", Def.VFXTarget.MONSTER),
				VFXTypeName = new ConfigString("Cast_VFXTypeName", ""),
				VFXType = new ConfigEnum<Def.VFXType>("Cast_VFXType", Def.VFXType.CAST),
				VFXVersion = new ConfigInteger("Cast_VFXVersion", 0),
				VFXFacing = new ConfigEnum<Def.VFXFacing>("Cast_VFXFacing", Def.VFXFacing.DontFaceAnything),
				VFXEnd = new ConfigEnum<Def.VFXEnd>("Cast_VFXEnd", Def.VFXEnd.SelfDestroy),
				VFXScale = new ConfigVector2("Cast_VFXScale", Vector2.one),
				FPSOverride = new ConfigFloat("Cast_FPSOverride", 0f)
			};
			m_OnHitVFX = new ConfigVFXDef()
			{
				VFXTarget = new ConfigEnum<Def.VFXTarget>("OnHit_VFXTarget", Def.VFXTarget.MONSTER),
				VFXTypeName = new ConfigString("OnHit_VFXTypeName", ""),
				VFXType = new ConfigEnum<Def.VFXType>("OnHit_VFXType", Def.VFXType.ONHIT),
				VFXVersion = new ConfigInteger("OnHit_VFXVersion", 0),
				VFXFacing = new ConfigEnum<Def.VFXFacing>("OnHit_VFXFacing", Def.VFXFacing.DontFaceAnything),
				VFXEnd = new ConfigEnum<Def.VFXEnd>("OnHit_VFXEnd", Def.VFXEnd.SelfDestroy),
				VFXScale = new ConfigVector2("OnHit_VFXScale", Vector2.one),
				FPSOverride = new ConfigFloat("OnHit_FPSOverride", 0f)
			};

			m_Config.Add(m_AttackType);
			m_Config.Add(m_HitMask);
			m_Config.Add(m_DamageMask);
			m_Config.Add(m_SpellWeapon);
			m_Config.Add(m_SpellWeaponScale);
			m_Config.Add(m_SlashFromOffset);
			m_Config.Add(m_OffsetHeight);
			m_Config.Add(m_OffsetForward);
			m_Config.Add(m_UnderEffectOffsetForward);
			m_Config.Add(m_PrismBaseHeightDepth);
			m_Config.Add(m_ColliderOffset);
			m_Config.Add(m_ArcSize);
			m_Config.Add(m_StartingOffset);
			m_Config.Add(m_Flip);
			m_Config.Add(m_Vertical);
			m_Config.Add(m_PierceDistance);
			m_Config.Add(m_Speed);
			m_Config.Add(m_PierceReturnSpeed);
			m_Config.Add(m_SlashFadeOutAngle);
			m_Config.Add(m_FlipWeaponX);
			m_Config.Add(m_FlipWeaponY);
			m_UnderWeaponEffect.AddToConfig(m_Config);
			m_Config.Add(m_SpriteAngleOffset);
			m_CastVFX.AddToConfig(m_Config);
			m_OnHitVFX.AddToConfig(m_Config);
		}
		protected override void PerformAttack(CLivingEntity entity, Vector3 pos)
		{
			var melee = new GameObject("Melee").AddComponent<CMelee>();

			melee.SetMelee(this);
		}
		protected override void PerformCast(CLivingEntity entity, Vector3 pos)
		{
			var castVFXConf = m_CastVFX.GetValue();
			if (castVFXConf.IsValid())
			{
				var castVFX = new GameObject("Melee_Cast").AddComponent<CVFX>();
				var mov = m_CasterLE.gameObject.GetComponent<CMovableEntity>();
				var xzPos = new Vector2(m_CasterLE.transform.position.x, m_CasterLE.transform.position.z) + mov.GetDirection() * m_OffsetForward.GetValue();
				var yPos = m_CasterLE.transform.position.y + m_OffsetHeight.GetValue();
				castVFX.transform.position = new Vector3(xzPos.x, yPos, xzPos.y);
				castVFX.Set(castVFXConf);
				castVFX.GetSprite().GetRenderer().gameObject.layer = Def.RCLayerProjectile;

				var casterView = m_CasterLE.GetComponent<CMovableEntity>().GetDirection();
				var angle = GameUtils.AngleBetween2D(new Vector2(0, -1f), casterView) * Mathf.Rad2Deg;
				angle *= -1f;
				castVFX.transform.Rotate(Vector3.up, angle + GetSpriteAngleOffset(), Space.World);
				castVFX.transform.Rotate(Vector3.right, -90f, Space.Self);
			}
		}
	}
}