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
	[Serializable]
	public abstract class ISpell
	{
		protected List<IConfig> m_Config;

		[SerializeField] protected string m_Name;
		protected ConfigFloat m_MinCastRange;
		protected ConfigFloat m_MaxCastRange;
		protected ConfigFloat m_CastTime;
		protected ConfigFloat m_ReleaseTime;
		protected ConfigFloat m_CooldownTime;
		[SerializeField] protected Timer m_AttackTimer;
		[SerializeField] protected Def.SpellState m_State;
		protected ConfigEnum<Def.MonsterAttackFrameTrigger> m_AttackFrameTrigger;
		[SerializeReference] protected CLivingEntity m_CasterLE;
		protected ConfigBoolean m_DamagesCaster;
		[SerializeReference] CSpellCaster m_Caster;
		[SerializeReference] CLivingEntity m_TargetEntity;
		[SerializeField] Vector3 m_TargetPos;

		protected Dictionary<Def.OnHitType, List<IOnHit>> m_OnHit;

		public float GetMinCastRange() => m_MinCastRange.GetValue();
		public float GetMaxCastRange() => m_MaxCastRange.GetValue();
		public float GetCastTime() => m_CastTime.GetValue();
		public float GetReleaseTime() => m_ReleaseTime.GetValue();
		public float GetCooldownTime() => m_CooldownTime.GetValue();
		public Def.SpellState GetCurrentState() => m_State;
		public Def.MonsterAttackFrameTrigger GetAttackFrameTrigger() => m_AttackFrameTrigger.GetValue();
		public bool CanAttack() => m_State == Def.SpellState.IDLE;
		public bool DamagesCaster() => m_DamagesCaster.GetValue();
		public CLivingEntity GetCasterLE() => m_CasterLE;
		public string GetName() => m_Name;
		public List<IConfig> GetConfig() => m_Config;
		public void SetConfig(IConfig config)
		{
			for(int i = 0; i < m_Config.Count; ++i)
			{
				var conf = m_Config[i];
				if(conf.GetConfigName() == config.GetConfigName())
				{
					conf.FromString(config.GetValueString());
					break;
				}
			}
		}
		public IConfig GetConfig(string name)
		{
			for(int i = 0; i < m_Config.Count; ++i)
			{
				var conf = m_Config[i];
				if (conf.GetConfigName() == name)
					return conf;
			}
			return null;
		}
		public Dictionary<Def.OnHitType, List<IOnHit>> GetOnHit() => m_OnHit;
		public void AddOnHit(OnHitConfig config)
		{
			var onHit = SpellManager.CreateOnHit(config.OnHitType);
			onHit.FromString(config.Configuration);
			AddOnHit(onHit);
		}
		public void AddOnHit(IOnHit onHit)
		{
			if (m_OnHit[onHit.GetOnHitType()].Contains(onHit))
				return; // no duplicates
			m_OnHit[onHit.GetOnHitType()].Add(onHit);
		}
		protected ISpell(string name)
		{
			m_Name = name;
			m_MinCastRange = new ConfigFloat("MinCastRange", 0f);
			m_MaxCastRange = new ConfigFloat("MaxCastRange", 0f);
			m_CastTime = new ConfigFloat("CastTime", 0f);
			m_ReleaseTime = new ConfigFloat("ReleaseTime", 0f);
			m_CooldownTime = new ConfigFloat("CooldownTime", 0f);
			m_AttackFrameTrigger = new ConfigEnum<Def.MonsterAttackFrameTrigger>("AttackFrameTrigger", Def.MonsterAttackFrameTrigger.CAST);
			m_DamagesCaster = new ConfigBoolean("DamagesCaster", false);
			m_AttackTimer = new Timer();

			m_Config = new List<IConfig>()
			{
				m_MinCastRange,
				m_MaxCastRange,
				m_CastTime,
				m_ReleaseTime,
				m_CooldownTime,
				m_AttackFrameTrigger,
				m_DamagesCaster
			};

			m_OnHit = new Dictionary<Def.OnHitType, List<IOnHit>>()
			{
				{ Def.OnHitType.Displacement, new List<IOnHit>() },
				{ Def.OnHitType.Damage, new List<IOnHit>() },
				{ Def.OnHitType.StatusEffect, new List<IOnHit>() }
			};
			if (m_OnHit.Count != Def.OnHitTypeCount)
				Debug.LogWarning("OnHitType mismatch! !ISpell");
		}
		public void SetCaster(CLivingEntity caster)
		{
			m_CasterLE = caster;
			m_Caster = m_CasterLE.gameObject.GetComponent<CSpellCaster>();
		}
		public void Attack(CLivingEntity entity = null, Vector3 pos = default)
		{
			if (m_State != Def.SpellState.IDLE)
				return;
			m_TargetEntity = entity;
			m_TargetPos = pos;

			OnCastBegin();
		}
		public bool CancelAttack()
		{
			if (m_State != Def.SpellState.CASTING)
				return false;

			m_AttackTimer.Stop();
			m_AttackTimer.OnTimerTrigger -= OnCastEnd;

			return true;
		}
		protected abstract void PerformCast(CLivingEntity entity, Vector3 pos);
		protected abstract void PerformAttack(CLivingEntity entity, Vector3 pos);
		protected virtual void PerformCooldown()
		{

		}
		protected virtual void PerformIdle()
		{

		}
		public virtual void InitSpell()
		{

		}
		public virtual void DestroySpell()
		{

		}
		void OnCastBegin()
		{
			if (m_Caster != null && GetAttackFrameTrigger() == Def.MonsterAttackFrameTrigger.CAST)
				m_Caster.SetIsAttacking(true);
			m_State = Def.SpellState.CASTING;
			PerformCast(m_TargetEntity, m_TargetPos);
			float castTime = m_CastTime.GetValue();
			if(castTime > 0f)
			{
				m_AttackTimer.Reset(castTime);
				m_AttackTimer.OnTimerTrigger += OnCastEnd;
			}
			else
			{
				OnReleaseBegin();
			}
		}
		void OnCastEnd()
		{
			if (m_Caster != null && GetAttackFrameTrigger() == Def.MonsterAttackFrameTrigger.CAST)
				m_Caster.SetIsAttacking(false);
			m_AttackTimer.OnTimerTrigger -= OnCastEnd;
			OnReleaseBegin();
		}
		void OnReleaseBegin()
		{
			if (m_Caster != null && GetAttackFrameTrigger() == Def.MonsterAttackFrameTrigger.RELEASE)
				m_Caster.SetIsAttacking(true);
			m_State = Def.SpellState.RELEASING;
			PerformAttack(m_TargetEntity, m_TargetPos);
			float releaseTime = m_ReleaseTime.GetValue();
			
			if (releaseTime > 0f)
			{
				m_AttackTimer.Reset(releaseTime);
				m_AttackTimer.OnTimerTrigger += OnReleaseEnd;
			}
			else
			{
				OnCooldownBegin();
			}
		}
		void OnReleaseEnd()
		{
			if (m_Caster != null && GetAttackFrameTrigger() == Def.MonsterAttackFrameTrigger.RELEASE)
				m_Caster.SetIsAttacking(false);
			m_AttackTimer.OnTimerTrigger -= OnReleaseEnd;
			OnCooldownBegin();
		}
		void OnCooldownBegin()
		{
			m_State = Def.SpellState.COOLDOWN;
			PerformCooldown();
			float cooldownTime = m_CooldownTime.GetValue();
			if(cooldownTime > 0f)
			{
				m_AttackTimer.Reset(cooldownTime);
				m_AttackTimer.OnTimerTrigger += OnCooldownEnd;
			}
			else
			{
				m_State = Def.SpellState.IDLE;
				PerformIdle();
			}
		}
		void OnCooldownEnd()
		{
			m_AttackTimer.OnTimerTrigger -= OnCooldownEnd;
			m_State = Def.SpellState.IDLE;
			PerformIdle();
		}
		protected void ApplyOnHitEffects(CLivingEntity target)
		{
			float chance = UnityEngine.Random.value * 100f;
			for(int i = 0; i < m_OnHit.Count; ++i)
			{
				var pair = m_OnHit.ElementAt(i);
				switch (pair.Key)
				{
					case Def.OnHitType.Displacement:
						break;
					case Def.OnHitType.StatusEffect:
						break;
					case Def.OnHitType.Damage:
						for(int j = 0; j < pair.Value.Count; ++j)
						{
							var dmg = pair.Value[j] as OnHitDamage;
							if (dmg.GetChance() <= chance)
								continue;
							target.ReceiveDamage(m_CasterLE, dmg.GetDamageType(), dmg.GetDamageAmount());
						}
						break;
					default:
						Debug.LogWarning("Unhandled OnHitType " + pair.Key.ToString());
						break;
				}
			}
		}
		protected void ApplyOnHitEffects(CBlock target)
		{

		}
		protected void ApplyOnHitEffects(BridgeComponent target)
		{

		}
		public virtual void OnUpdate()
		{
			m_AttackTimer.Update();
		}
	}
	public struct HitEntity
	{
		public CLivingEntity Entity;
		public float Time;
	}
}
