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
	public class CProjectileSpawner : MonoBehaviour
	{
		ProjectileSpell m_Spell;
		CLivingEntity m_TargetEntity;
		Vector3 m_Position;
		float m_Distance;
		Timer m_DelayTimer;
		int m_RemainingProjectiles;
		float m_ArcOffset;
		float m_CurrentAngle;

		void OnTimerFinish()
		{
			FireProjectile();
			if(m_RemainingProjectiles <= 0)
			{
				if (m_DelayTimer != null)
					m_DelayTimer.OnTimerTrigger -= OnTimerFinish;
				GameUtils.DeleteGameobject(gameObject);
			}
		}
		public void Init(ProjectileSpell spell, CLivingEntity entity, Vector3 position)
		{
			m_Spell = spell;
			if(m_Spell.GetTravelType() == Def.ProjectileTravelType.TARGETTED)
				m_TargetEntity = entity;
			m_RemainingProjectiles = m_Spell.GetProjectileCount();
			var delay = m_Spell.GetProjectileShotDelay();
			int remaining = 0;
			if(delay > 0f)
			{
				m_DelayTimer = new Timer(delay, true, true);
				m_DelayTimer.OnTimerTrigger += OnTimerFinish;
				remaining = m_RemainingProjectiles - 1;
			}

			Vector3 pos;
			if (entity != null)
			{
				pos = entity.transform.position + new Vector3(0f, entity.GetHeight() * 0.5f, 0f);
			}
			else
			{
				pos = position;
			}
			m_Position = pos;
			var caster = m_Spell.GetCasterLE();
			var casterXZ = new Vector2(caster.transform.position.x, caster.transform.position.z);
			var posXZ = new Vector2(pos.x, pos.z);
			var temp = posXZ - casterXZ;
			var tempDist = temp.sqrMagnitude;
			var dir = temp.normalized;
			m_Distance = Mathf.Max(tempDist, m_Spell.GetMaxTravelDistance());
			m_Distance = Mathf.Max(m_Distance - m_Spell.GetOffsetForward(), 0f);
			var angle = GameUtils.AngleFromDir2D(dir) * Mathf.Rad2Deg;

			var totalArc = m_Spell.GetProjectileArcSize();
			if(totalArc == 0f)
			{
				m_ArcOffset = 0f;
			}
			else
			{
				m_ArcOffset = totalArc / m_RemainingProjectiles;
			}
			m_CurrentAngle = angle - totalArc * 0.5f + m_Spell.GetShotAngleOffset();

			while (m_RemainingProjectiles > remaining)
				OnTimerFinish();
		}
		void FireProjectile()
		{
			int currentBurst = 0;
			var maxBurst = Mathf.Max(m_Spell.GetBurstCount(), 1);
			while (currentBurst < maxBurst)
			{
				if (m_RemainingProjectiles <= 0)
					return;
				--m_RemainingProjectiles;

				Vector2 curDirection = GameUtils.DirFromAngle2D(m_CurrentAngle * Mathf.Deg2Rad);

				var caster = m_Spell.GetCasterLE();
				var projectile = new GameObject("Projectile").AddComponent<CProjectile>();
				var casterXZ = new Vector2(caster.transform.position.x, caster.transform.position.z);
				var txzpos = casterXZ + curDirection * m_Distance;

				var xzPos = casterXZ + curDirection * m_Spell.GetOffsetForward();
				var yPos = caster.transform.position.y + m_Spell.GetOffsetHeight();
				projectile.transform.position = new Vector3(xzPos.x, yPos, xzPos.y);
				projectile.SetProjectile(m_Spell, null,
					new Vector3(txzpos.x, m_Position.y, txzpos.y));

				m_CurrentAngle += m_ArcOffset;
				++currentBurst;
			}
			if(maxBurst > 1)
			{
				var burstAngleOffset = m_Spell.GetBurstAngleOffset();
				m_CurrentAngle += burstAngleOffset;
			}
		}

		private void Update()
		{
			m_DelayTimer.Update();
		}
	}
}
