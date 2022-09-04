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
	public class ProjectileSpell : ISpell
	{
		ConfigEnum<Def.ProjectileTravelType> m_TravelType;
		ConfigEnum<Def.ProjectileTravelCollider> m_TravelColliderType;
		ConfigEnum<Def.ProjectileAreaDamageType> m_AreaDamageType;
		ConfigEnum<Def.SpellHitMask> m_HitMask;
		ConfigEnum<Def.SpellDamageMask> m_DamageMask;
		ConfigFloat m_MaxTravelDistance;
		ConfigFloat m_SpriteAngleOffset;

		ConfigInteger m_ProjectileCount;
		ConfigInteger m_BurstCount;
		ConfigInteger m_ShotAngleOffset;
		ConfigInteger m_BurstShotAngleOffset;
		ConfigFloat m_ProjectileShotDelay;
		ConfigFloat m_ProjectileArcSize;
		ConfigEnum<Def.VFXPlanes> m_Planes;

		ConfigBoolean m_EndOnSelectedPos;
		ConfigBoolean m_FadeAway;
		ConfigFloat m_SphereRadius;
		ConfigFloat m_Speed;

		ConfigFloat m_OffsetHeight;
		ConfigFloat m_OffsetForward;
		ConfigVector3 m_PrismBaseHeightDepth;

		ConfigVFXDef m_CastVFX;
		ConfigVFXDef m_TravelVFX;
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
		public Def.ProjectileTravelType GetTravelType() => m_TravelType.GetValue();
		public Def.ProjectileTravelCollider GetTravelCollider() => m_TravelColliderType.GetValue();
		public Def.ProjectileAreaDamageType GetAreaDamageType() => m_AreaDamageType.GetValue();
		public Def.SpellHitMask GetHitMask() => m_HitMask.GetValue();
		public Def.SpellDamageMask GetDamageMask() => m_DamageMask.GetValue();
		public float GetMaxTravelDistance() => m_MaxTravelDistance.GetValue();
		public bool GetEndOnSelectedPos() => m_EndOnSelectedPos.GetValue();
		public bool GetFadeAway() => m_FadeAway.GetValue();
		public void SetFadeAway(bool fadeAway) => m_FadeAway.SetValue(fadeAway);
		public float GetSphereRadius() => m_SphereRadius.GetValue();
		public int GetProjectileCount() => m_ProjectileCount.GetValue();
		public int GetBurstCount() => m_BurstCount.GetValue();
		public int GetShotAngleOffset() => m_ShotAngleOffset.GetValue();
		public int GetBurstAngleOffset() => m_BurstShotAngleOffset.GetValue();
		public float GetProjectileShotDelay() => m_ProjectileShotDelay.GetValue();
		public float GetProjectileArcSize() => m_ProjectileArcSize.GetValue();
		public Def.VFXPlanes GetVFXPlanes() => m_Planes.GetValue();
		public float GetSpeed() => m_Speed.GetValue();
		public float GetOffsetHeight() => m_OffsetHeight.GetValue();
		public float GetOffsetForward() => m_OffsetForward.GetValue();
		public Vector3 GetPrismBaseHeightDepht() => m_PrismBaseHeightDepth.GetValue();
		public float GetSpriteAngleOffset() => m_SpriteAngleOffset.GetValue();
		public VFXDef GetCastVFX() => m_CastVFX.GetValue();
		public VFXDef GetTravelVFX() => m_TravelVFX.GetValue();
		public VFXDef GetOnHitVFX() => m_OnHitVFX.GetValue();

		public ProjectileSpell()
			:base("ProjectileSpell")
		{
			m_TravelType = new ConfigEnum<Def.ProjectileTravelType>("TravelType", Def.ProjectileTravelType.LINEAR);
			m_TravelColliderType = new ConfigEnum<Def.ProjectileTravelCollider>("TravelColliderType", Def.ProjectileTravelCollider.Sphere);
			m_AreaDamageType = new ConfigEnum<Def.ProjectileAreaDamageType>("AreaDamageType", Def.ProjectileAreaDamageType.CIRC3D);
			m_HitMask = new ConfigEnum<Def.SpellHitMask>("HitMask", Def.SpellHitMask.ALL);
			m_DamageMask = new ConfigEnum<Def.SpellDamageMask>("DamageMask", Def.SpellDamageMask.ALL);
			m_MaxTravelDistance = new ConfigFloat("MaxTravelDistance", -1f);
			m_SpriteAngleOffset = new ConfigFloat("SpriteAngleOffset", 0f);
			m_EndOnSelectedPos = new ConfigBoolean("EndOnSelectedPos", false);
			m_FadeAway = new ConfigBoolean("FadeAway", false);
			m_SphereRadius = new ConfigFloat("SphereRadius", 0f);
			m_Speed = new ConfigFloat("Speed", 1f);
			m_OffsetHeight = new ConfigFloat("OffsetHeight", 0f);
			m_OffsetForward = new ConfigFloat("OffsetForward", 0f);
			m_PrismBaseHeightDepth = new ConfigVector3("PrismBaseHeightDepth", Vector3.zero);
			m_ProjectileCount = new ConfigInteger("ProjectileCount", 1);
			m_BurstCount = new ConfigInteger("BurstCount", 1);
			m_ShotAngleOffset = new ConfigInteger("ShotAngleOffset", 0);
			m_BurstShotAngleOffset = new ConfigInteger("BurstShotAngleOffset", 0);
			m_ProjectileShotDelay = new ConfigFloat("ShotDelay", 0f);
			m_ProjectileArcSize = new ConfigFloat("ArcSize", 0f);
			m_Planes = new ConfigEnum<Def.VFXPlanes>("Planes", Def.VFXPlanes.ONE);

			m_CastVFX = new ConfigVFXDef()
			{
				VFXTarget = new ConfigEnum<Def.VFXTarget>("Cast_VFXTarget", Def.VFXTarget.MONSTER),
				VFXTypeName = new ConfigString("Cast_VFXTypeName", ""),
				VFXType = new ConfigEnum<Def.VFXType>("Cast_VFXType", Def.VFXType.CAST),
				VFXVersion = new ConfigInteger("Cast_VFXVersion", 0),
				VFXFacing = new ConfigEnum<Def.VFXFacing>("Cast_VFXFacing", Def.VFXFacing.DontFaceAnything),
				VFXEnd = new ConfigEnum<Def.VFXEnd>("Cast_VFXEnd", Def.VFXEnd.Repeat),
				VFXScale = new ConfigVector2("Cast_VFXScale", Vector2.one),
				FPSOverride = new ConfigFloat("Cast_FPSOverride", 0f)
			};
			m_TravelVFX = new ConfigVFXDef()
			{
				VFXTarget = new ConfigEnum<Def.VFXTarget>("Travel_VFXTarget", Def.VFXTarget.MONSTER),
				VFXTypeName = new ConfigString("Travel_VFXTypeName", ""),
				VFXType = new ConfigEnum<Def.VFXType>("Travel_VFXType", Def.VFXType.TRAVEL),
				VFXVersion = new ConfigInteger("Travel_VFXVersion", 0),
				VFXFacing = new ConfigEnum<Def.VFXFacing>("Travel_VFXFacing", Def.VFXFacing.DontFaceAnything),
				VFXEnd = new ConfigEnum<Def.VFXEnd>("Travel_VFXEnd", Def.VFXEnd.Repeat),
				VFXScale = new ConfigVector2("Travel_VFXScale", Vector2.one),
				FPSOverride = new ConfigFloat("Travel_FPSOverride", 0f)
			};
			m_OnHitVFX = new ConfigVFXDef()
			{
				VFXTarget = new ConfigEnum<Def.VFXTarget>("OnHit_VFXTarget", Def.VFXTarget.MONSTER),
				VFXTypeName = new ConfigString("OnHit_VFXTypeName", ""),
				VFXType = new ConfigEnum<Def.VFXType>("OnHit_VFXType", Def.VFXType.ONHIT),
				VFXVersion = new ConfigInteger("OnHit_VFXVersion", 0),
				VFXFacing = new ConfigEnum<Def.VFXFacing>("OnHit_VFXFacing", Def.VFXFacing.DontFaceAnything),
				VFXEnd = new ConfigEnum<Def.VFXEnd>("OnHit_VFXEnd", Def.VFXEnd.Repeat),
				VFXScale = new ConfigVector2("OnHit_VFXScale", Vector2.one),
				FPSOverride = new ConfigFloat("OnHit_FPSOverride", 0f)
			};

			m_Config.Add(m_TravelType);
			m_Config.Add(m_TravelColliderType);
			m_Config.Add(m_AreaDamageType);
			m_Config.Add(m_HitMask);
			m_Config.Add(m_DamageMask);
			m_Config.Add(m_MaxTravelDistance);
			m_Config.Add(m_SpriteAngleOffset);
			m_Config.Add(m_EndOnSelectedPos);
			m_Config.Add(m_FadeAway);
			m_Config.Add(m_SphereRadius);
			m_Config.Add(m_ProjectileCount);
			m_Config.Add(m_BurstCount);
			m_Config.Add(m_ShotAngleOffset);
			m_Config.Add(m_BurstShotAngleOffset);
			m_Config.Add(m_ProjectileShotDelay);
			m_Config.Add(m_ProjectileArcSize);
			m_Config.Add(m_Planes);
			m_Config.Add(m_Speed);
			m_Config.Add(m_OffsetHeight);
			m_Config.Add(m_OffsetForward);
			m_Config.Add(m_PrismBaseHeightDepth);
			m_CastVFX.AddToConfig(m_Config);
			m_TravelVFX.AddToConfig(m_Config);
			m_OnHitVFX.AddToConfig(m_Config);
		}
		protected override void PerformAttack(CLivingEntity entity, Vector3 pos)
		{
			if(GetProjectileCount() > 1)
			{
				var spawner = new GameObject("ProjectileSpawner").AddComponent<CProjectileSpawner>();
				spawner.transform.SetParent(m_CasterLE.transform);
				spawner.transform.localPosition = Vector3.zero;
				spawner.transform.localRotation = Quaternion.identity;
				spawner.Init(this, entity, pos);

				return;
			}
			var projectile = new GameObject("Projectile").AddComponent<CProjectile>();
			projectile.transform.localPosition = Vector3.zero;
			var mov = m_CasterLE.gameObject.GetComponent<CMovableEntity>();
			var xzPos = new Vector2(m_CasterLE.transform.position.x, m_CasterLE.transform.position.z) + mov.GetDirection() * m_OffsetForward.GetValue();
			var yPos = m_CasterLE.transform.position.y + m_OffsetHeight.GetValue();
			projectile.transform.position = new Vector3(xzPos.x, yPos, xzPos.y);

			projectile.SetProjectile(this, entity, pos);
		}
		protected override void PerformCast(CLivingEntity entity, Vector3 pos)
		{
			var castVFXConf = m_CastVFX.GetValue();
			if (castVFXConf.IsValid())
			{
				var castVFX = new GameObject("Projectile_Cast").AddComponent<CVFX>();
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
