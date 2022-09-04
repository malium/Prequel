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
	public class SpraySpell : ISpell
	{
		CSprayParticles m_Particles;

		ConfigEnum<Def.SpellDamageMask> m_DamageMask;
		ConfigFloat m_SprayDamageDelay;
		ConfigVector3 m_AreaConeScale;

		ConfigEnum<Def.VFXPlanes> m_Planes;

		ConfigEnum<Def.MonsterAttackFrameTrigger> m_ParticleStart;
		ConfigEnum<Def.SpellState> m_ParticleEnd;
		ConfigFloat m_ParticleHeightOffset;
		ConfigSprayParticles m_ParticlesConfig;

		ConfigFloat m_VFXHeightOffset;
		ConfigFloat m_VFXForwardOffset;
		ConfigVFXDef m_InVFXConfig;
		ConfigVFXDef m_CycleVFXConfig;
		ConfigVFXDef m_OutVFXConfig;

		ConfigVFXDef m_OnHitVFXConfig;

		CSpray m_Cycle;
		CVFX[] m_InVFXs;
		CVFX[] m_CycleVFXs;
		CVFX[] m_OutVFXs;
		float m_Angle;

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
		public Def.SpellDamageMask GetDamageMask() => m_DamageMask.GetValue();
		public float GetDamageDelay() => m_SprayDamageDelay.GetValue();
		public Vector3 GetAreaConeScale() => m_AreaConeScale.GetValue();
		public Def.VFXPlanes GetVFXPlanes() => m_Planes.GetValue();
		public float GetVFXHeightOffset() => m_VFXHeightOffset.GetValue();
		public float GetVFXForwardOffset() => m_VFXForwardOffset.GetValue();
		public VFXDef GetSprayVFXDef() => m_CycleVFXConfig.GetValue();
		public VFXDef GetOnHitVFXDef() => m_OnHitVFXConfig.GetValue();

		public SpraySpell()
			:base("Spray")
		{
			m_DamageMask = new ConfigEnum<Def.SpellDamageMask>("DamageMask", Def.SpellDamageMask.ALL);
			m_SprayDamageDelay = new ConfigFloat("DamageDelay", 0f);
			m_AreaConeScale = new ConfigVector3("AreaConeScale", Vector3.one);

			m_Planes = new ConfigEnum<Def.VFXPlanes>("SprayPlanes", Def.VFXPlanes.ONE);

			m_ParticleStart = new ConfigEnum<Def.MonsterAttackFrameTrigger>("ParticleStart", Def.MonsterAttackFrameTrigger.CAST);
			m_ParticleEnd = new ConfigEnum<Def.SpellState>("ParticleEnd", Def.SpellState.COOLDOWN);

			m_ParticleHeightOffset = new ConfigFloat("ParticleHeightOffset", 0f);

			m_ParticlesConfig = new ConfigSprayParticles()
			{
				ImageRowCount = new ConfigInteger("ImageRowCount", 0),
				LifeTime = new ConfigVector2("LifeTime", Vector2.zero),
				SizeCurve = new ConfigVector2("ParticleSize", Vector2.zero),
				SpawnCenter = new ConfigVector3("SpawnerOffset", Vector3.zero),
				SpawnRate = new ConfigInteger("SpawnRate", 0),
				SpawnSize = new ConfigVector3("SpawnerSize", Vector3.zero),
				Speed = new ConfigVector2("Speed", Vector2.zero),
				Texture = new ConfigParticleTexture("Particle", ""),
				ParticleVersion = new ConfigInteger("ParticleVersion", 0)
			};

			m_VFXHeightOffset = new ConfigFloat("VFXHeightOffset", 0f);
			m_VFXForwardOffset = new ConfigFloat("VFXForwardOffset", 0);

			m_InVFXConfig = new ConfigVFXDef()
			{
				FPSOverride = new ConfigFloat("IN_FPSOverride", 0f),
				VFXEnd = new ConfigEnum<Def.VFXEnd>("IN_VFXEnd", Def.VFXEnd.Stop),
				VFXFacing = new ConfigEnum<Def.VFXFacing>("IN_VFXFacing", Def.VFXFacing.DontFaceAnything),
				VFXScale = new ConfigVector2("IN_VFXScale", new Vector2(1f, 1f)),
				VFXTarget = new ConfigEnum<Def.VFXTarget>("IN_VFXTarget", Def.VFXTarget.MONSTER),
				VFXType = new ConfigEnum<Def.VFXType>("IN_VFXType", Def.VFXType.CAST),
				VFXTypeName = new ConfigString("IN_VFXTypeName", ""),
				VFXVersion = new ConfigInteger("IN_VFXVersion", 0)
			};

			m_CycleVFXConfig = new ConfigVFXDef()
			{
				FPSOverride = new ConfigFloat("CYCLE_FPSOverride", 0f),
				VFXEnd = new ConfigEnum<Def.VFXEnd>("CYCLE_VFXEnd", Def.VFXEnd.Repeat),
				VFXFacing = new ConfigEnum<Def.VFXFacing>("CYCLE_VFXFacing", Def.VFXFacing.DontFaceAnything),
				VFXScale = new ConfigVector2("CYCLE_VFXScale", new Vector2(1f, 1f)),
				VFXTarget = new ConfigEnum<Def.VFXTarget>("CYCLE_VFXTarget", Def.VFXTarget.MONSTER),
				VFXType = new ConfigEnum<Def.VFXType>("CYCLE_VFXType", Def.VFXType.TRAVEL),
				VFXTypeName = new ConfigString("CYCLE_VFXTypeName", ""),
				VFXVersion = new ConfigInteger("CYCLE_VFXVersion", 0)
			};

			m_OutVFXConfig = new ConfigVFXDef()
			{
				FPSOverride = new ConfigFloat("OUT_FPSOverride", 0f),
				VFXEnd = new ConfigEnum<Def.VFXEnd>("OUT_VFXEnd", Def.VFXEnd.Stop),
				VFXFacing = new ConfigEnum<Def.VFXFacing>("OUT_VFXFacing", Def.VFXFacing.DontFaceAnything),
				VFXScale = new ConfigVector2("OUT_VFXScale", new Vector2(1f, 1f)),
				VFXTarget = new ConfigEnum<Def.VFXTarget>("OUT_VFXTarget", Def.VFXTarget.MONSTER),
				VFXType = new ConfigEnum<Def.VFXType>("OUT_VFXType", Def.VFXType.ONHIT),
				VFXTypeName = new ConfigString("OUT_VFXTypeName", ""),
				VFXVersion = new ConfigInteger("OUT_VFXVersion", 0)
			};

			m_OnHitVFXConfig = new ConfigVFXDef()
			{
				FPSOverride = new ConfigFloat("ONHIT_FPSOverride", 0f),
				VFXEnd = new ConfigEnum<Def.VFXEnd>("ONHIT_VFXEnd", Def.VFXEnd.SelfDestroy),
				VFXFacing = new ConfigEnum<Def.VFXFacing>("ONHIT_VFXFacing", Def.VFXFacing.DontFaceAnything),
				VFXScale = new ConfigVector2("ONHIT_VFXScale", new Vector2(1f, 1f)),
				VFXTarget = new ConfigEnum<Def.VFXTarget>("ONHIT_VFXTarget", Def.VFXTarget.MONSTER),
				VFXType = new ConfigEnum<Def.VFXType>("ONHIT_VFXType", Def.VFXType.ONHIT),
				VFXTypeName = new ConfigString("ONHIT_VFXTypeName", ""),
				VFXVersion = new ConfigInteger("ONHIT_VFXVersion", 0)
			};

			m_Config.Add(m_DamageMask);
			m_Config.Add(m_SprayDamageDelay);
			m_Config.Add(m_AreaConeScale);
			m_Config.Add(m_Planes);
			m_Config.Add(m_ParticleStart);
			m_Config.Add(m_ParticleEnd);
			m_Config.Add(m_ParticleHeightOffset);
			m_ParticlesConfig.AddToConfig(m_Config);
			m_Config.Add(m_VFXHeightOffset);
			m_Config.Add(m_VFXForwardOffset);
			m_InVFXConfig.AddToConfig(m_Config);
			m_CycleVFXConfig.AddToConfig(m_Config);
			m_OutVFXConfig.AddToConfig(m_Config);
			m_OnHitVFXConfig.AddToConfig(m_Config);

			m_Config.Remove(m_InVFXConfig.VFXEnd);
			m_Config.Remove(m_CycleVFXConfig.VFXEnd);
			m_Config.Remove(m_OutVFXConfig.VFXEnd);
			m_Config.Remove(m_OnHitVFXConfig.VFXEnd);
		}
		public override void InitSpell()
		{
			m_Particles = GameObject.Instantiate(Particles.DefaultParticle);
			m_Particles.transform.SetParent(m_CasterLE.transform, true);
			m_Cycle = new GameObject("Spray").AddComponent<CSpray>();
			m_Cycle.SetSpell(this);
			m_Cycle.enabled = false;
			m_Cycle.transform.SetParent(m_CasterLE.transform, true);
			var vfxPlanes = GetVFXPlanes();
			var planeNum = vfxPlanes == Def.VFXPlanes.ONE ? 1 : vfxPlanes == Def.VFXPlanes.TWO ? 2 : 4;

			CVFX[] InitVFXPlanes(string name)
			{
				CVFX[] vfxs = new CVFX[planeNum];
				for(int i = 0; i < planeNum; ++i)
				{
					var vfx = new GameObject(name + i.ToString()).AddComponent<CVFX>();
					vfx.enabled = false;
					vfx.transform.SetParent(m_CasterLE.transform, true);
					vfxs[i] = vfx;
				}
				return vfxs;
			}
			m_InVFXs = InitVFXPlanes("Spray_IN_VFX");
			m_CycleVFXs = InitVFXPlanes("Spray_CYCLE_VFX");
			m_OutVFXs = InitVFXPlanes("Spray_OUT_VFX");
		}
		void InitParticles()
		{
			var particleTexture = m_ParticlesConfig.Texture.GetValue();
			if (particleTexture == "NONE" || particleTexture == "")
				return;
			m_Particles.gameObject.SetActive(true);
			m_Particles.gameObject.layer = Def.RCLayerProjectile;
			m_Particles.enabled = true;
			m_Particles.transform.position = m_CasterLE.transform.position + new Vector3(0f, m_ParticleHeightOffset.GetValue(), 0f);
			m_Particles.Set(m_ParticlesConfig);
			m_Particles.StartEffect();

			var casterView = m_CasterLE.GetComponent<CMovableEntity>().GetDirection();
			var angle = GameUtils.AngleBetween2D(new Vector2(0, -1f), casterView) * Mathf.Rad2Deg;
			angle *= -1f;
			m_Particles.transform.rotation = Quaternion.identity;
			m_Particles.transform.Rotate(Vector3.right, -90f, Space.World);
			m_Particles.transform.Rotate(Vector3.up, angle, Space.World);
		}
		void StartCycle(CVFX inVFX = null)
		{
			var cycleConf = m_CycleVFXConfig.GetValue();
			if (cycleConf.IsValid())
			{
				for(int i = 0; i < m_CycleVFXs.Length; ++i)
				{
					var vfx = m_CycleVFXs[i];
					vfx.Set(cycleConf, StartOut);
					vfx.GetSprite().GetRenderer().gameObject.layer = Def.RCLayerProjectile;
				}
			}
			if (inVFX == null)
				return;
			inVFX.enabled = false;
			inVFX.GetSprite().enabled = false;
		}
		void StartOut(CVFX cycleVFX = null)
		{
			var outVFXConf = m_OutVFXConfig.GetValue();
			if (outVFXConf.IsValid())
			{
				for (int i = 0; i < m_OutVFXs.Length; ++i)
				{
					var vfx = m_OutVFXs[i];
					vfx.Set(outVFXConf, EndOut);
					vfx.GetSprite().GetRenderer().gameObject.layer = Def.RCLayerProjectile;
				}
			}
			if (cycleVFX == null)
				return;
			cycleVFX.enabled = false;
			cycleVFX.GetSprite().enabled = false;
		}
		void EndOut(CVFX outVFX = null)
		{
			if (outVFX == null)
				return;
			outVFX.enabled = false;
			outVFX.GetSprite().enabled = false;
		}
		protected override void PerformAttack(CLivingEntity entity, Vector3 pos)
		{
			m_Cycle.StartSpray(m_Angle);
			var inVFXConf = m_InVFXConfig.GetValue();
			if (inVFXConf.IsValid())
			{
				for (int i = 0; i < m_InVFXs.Length; ++i)
				{
					var vfx = m_InVFXs[i];
					vfx.Set(inVFXConf, StartCycle);
					vfx.GetSprite().GetRenderer().gameObject.layer = Def.RCLayerProjectile;
				}
			}
			else
			{
				StartCycle();
			}
			
			if (m_ParticleStart.GetValue() == Def.MonsterAttackFrameTrigger.RELEASE)
			{
				InitParticles();
			}
			if (m_ParticleEnd.GetValue() == Def.SpellState.RELEASING)
				m_Particles.StopEffect();
		}
		protected override void PerformCast(CLivingEntity entity, Vector3 pos)
		{
			var casterView = m_CasterLE.GetComponent<CMovableEntity>().GetDirection();
			m_Angle = GameUtils.AngleBetween2D(new Vector2(0, -1f), casterView) * Mathf.Rad2Deg;
			m_Angle *= -1f;

			var xzPos = new Vector2(m_CasterLE.transform.position.x, m_CasterLE.transform.position.z) + casterView * m_VFXForwardOffset.GetValue();
			var yPos = m_CasterLE.transform.position.y + m_VFXHeightOffset.GetValue();

			var vfxPos = new Vector3(xzPos.x, yPos, xzPos.y);
			for(int i = 0; i < m_InVFXs.Length; ++i)
			{
				m_InVFXs[i].transform.position = vfxPos;
				m_CycleVFXs[i].transform.position = vfxPos;
				m_OutVFXs[i].transform.position = vfxPos;
			}
			//Debug.Log(m_Angle);
			
			var rotation = Quaternion.Euler(-90f, m_Angle, 0f);
			switch (GetVFXPlanes())
			{
				case Def.VFXPlanes.ONE:
					m_InVFXs[0].transform.rotation = rotation;
					m_CycleVFXs[0].transform.rotation = rotation;
					m_OutVFXs[0].transform.rotation = rotation;
					break;
				case Def.VFXPlanes.TWO:
					{
						var rotation2 = Quaternion.Euler(-180f, m_Angle + 90f, -90f);
						m_InVFXs[0].transform.rotation = rotation;
						m_CycleVFXs[0].transform.rotation = rotation;
						m_OutVFXs[0].transform.rotation = rotation;

						m_InVFXs[1].transform.rotation = rotation2;
						m_CycleVFXs[1].transform.rotation = rotation2;
						m_OutVFXs[1].transform.rotation = rotation2;
					}
					break;
				case Def.VFXPlanes.FOUR:
					for (int i = 0; i < m_InVFXs.Length; ++i)
					{
						m_InVFXs[i].transform.rotation = rotation;
						m_CycleVFXs[i].transform.rotation = rotation;
						m_OutVFXs[i].transform.rotation = rotation;
					}
					break;
			}
			
			//m_InVFX.transform.rotation = Quaternion.identity;
			//m_CycleVFX.transform.rotation = Quaternion.identity;
			//m_OutVFX.transform.rotation = Quaternion.identity;
			//m_InVFX.transform.Rotate(Vector3.up, m_Angle, Space.World); // Horizontal
			//m_InVFX.transform.Rotate(Vector3.right, -90f, Space.World); // Vertical
			//m_CycleVFX.transform.Rotate(Vector3.up, m_Angle, Space.World); // Horizontal
			//m_CycleVFX.transform.Rotate(Vector3.right, -90f, Space.World); // Vertical
			//m_OutVFX.transform.Rotate(Vector3.up, m_Angle, Space.World); // Horizontal
			//m_OutVFX.transform.Rotate(Vector3.right, -90f, Space.World); // Vertical

			
			if(m_ParticleStart.GetValue() == Def.MonsterAttackFrameTrigger.CAST)
			{
				InitParticles();
			}
			if (m_ParticleEnd.GetValue() == Def.SpellState.CASTING)
				m_Particles.StopEffect();
		}
		protected override void PerformCooldown()
		{
			// Stop and destroy cycle
			m_Cycle.StopSpray();
			for (int i = 0; i < m_CycleVFXs.Length; ++i)
				m_CycleVFXs[i].GetVFXDef().OnEnd = Def.VFXEnd.Stop;
			
			if (m_ParticleEnd.GetValue() == Def.SpellState.COOLDOWN)
				m_Particles.StopEffect();
		}
		protected override void PerformIdle()
		{
			if (m_ParticleEnd.GetValue() == Def.SpellState.IDLE)
				m_Particles.StopEffect();
			for (int i = 0; i < m_CycleVFXs.Length; ++i)
				m_CycleVFXs[i].enabled = false;
		}
		public override void DestroySpell()
		{
			if (m_Particles != null)
				GameUtils.DeleteGameobject(m_Particles.gameObject);
			
			void DestroyVFX(CVFX[] vfxs)
			{
				if (vfxs == null)
					return;
				for(int i = 0; i < vfxs.Length; ++i)
				{
					if (vfxs[i] == null) continue;
					GameUtils.DeleteGameobject(vfxs[i].gameObject);
				}
			}
			DestroyVFX(m_InVFXs);
			DestroyVFX(m_CycleVFXs);
			DestroyVFX(m_OutVFXs);
			if (m_Cycle != null)
				GameUtils.DeleteGameobject(m_Cycle.gameObject);
		}
	}
}
