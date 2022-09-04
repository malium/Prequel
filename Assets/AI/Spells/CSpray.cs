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
	public class CSpray : MonoBehaviour
	{
		SpraySpell m_Spell;
		MeshFilter m_ConeMesh;
		MeshCollider m_Collider;
		List<HitEntity> m_HitEntities;
		bool m_PropDamage;
		bool m_MonsterDamage;

		private void Awake()
		{
			m_HitEntities = new List<HitEntity>();
			var prefab = AssetLoader.ConeAreaDamage;
			m_ConeMesh = gameObject.AddComponent<MeshFilter>();
			m_ConeMesh.mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
		}
		public void SetSpell(SpraySpell spell)
		{
			m_Spell = spell;
		}
		public void StartSpray(float angle)
		{
			m_HitEntities.Clear();
			var heightOffset = m_Spell.GetVFXHeightOffset();
			transform.SetPositionAndRotation(m_Spell.GetCasterLE().transform.position + new Vector3(0f, heightOffset, 0f), Quaternion.identity);
			//var vfxDef = m_Spell.GetSprayVFXDef();
			transform.Rotate(Vector3.up, angle, Space.World); // Horizontal
			var scale = m_Spell.GetAreaConeScale();
			transform.localScale = scale;

			var damageMask = m_Spell.GetDamageMask();
			m_PropDamage = (damageMask & Def.SpellDamageMask.PROPS) == Def.SpellDamageMask.PROPS;
			m_MonsterDamage = (damageMask & Def.SpellDamageMask.MONSTERS) == Def.SpellDamageMask.MONSTERS;

			m_Collider = gameObject.AddComponent<MeshCollider>();
			m_Collider.sharedMesh = m_ConeMesh.mesh;
			m_Collider.convex = true;
			m_Collider.isTrigger = true;
			m_Collider.gameObject.layer = Def.RCLayerProjectile;
			enabled = true;
		}
		public void StopSpray()
		{
			Component.Destroy(m_Collider);
			enabled = false;
		}
		void UpdateHitEntities()
		{
			float delay = m_Spell.GetDamageDelay() * 1.5f;
			for(int i = 0; i < m_HitEntities.Count; )
			{
				var hit = m_HitEntities[i];
				hit.Time += Time.deltaTime;
				
				if(hit.Time > delay)
				{
					m_HitEntities.RemoveAt(i);
					continue;
				}
				m_HitEntities[i] = hit;
				++i;
			}
		}
		void ApplyDamageLE(CLivingEntity le)
		{
			var delay = m_Spell.GetDamageDelay();

			if (delay > 0f)
			{
				bool wasHitPreviously = false;
				for(int i = 0; i < m_HitEntities.Count; ++i)
				{
					var hit = m_HitEntities[i];
					if(hit.Entity == le)
					{
						wasHitPreviously = true;
						if (hit.Time > delay)
						{
							le.TriggerDamageFX(transform.position, (le.transform.position - transform.position).normalized);
							m_Spell.TriggerOnEntityHit(le);
							hit.Time = 0f;
							m_HitEntities[i] = hit;
						}
						break;
					}
				}
				if (!wasHitPreviously)
				{
					m_HitEntities.Add(new HitEntity()
					{
						Entity = le,
						Time = 0f
					});
					le.TriggerDamageFX(transform.position, (le.transform.position - transform.position).normalized);
					m_Spell.TriggerOnEntityHit(le);
				}
			}
			else // TODO: deal instant damage
			{
				le.TriggerDamageFX(transform.position, (le.transform.position - transform.position).normalized);
				m_Spell.TriggerOnEntityHit(le);
			}
		}
		private void OnTriggerStay(Collider other)
		{
			switch (other.gameObject.layer)
			{
				case Def.RCLayerLE:
					{
						if (!other.gameObject.TryGetComponent(out CLivingEntity le))
							other.gameObject.transform.parent.TryGetComponent(out le);
						if (le != null)
						{
							switch (le.GetLEType())
							{
								case Def.LivingEntityType.ODD:
								case Def.LivingEntityType.Monster:
									if (m_Spell.GetCasterLE() == le)
									{
										if (!m_Spell.DamagesCaster())
											return;
									}
									if (m_MonsterDamage)
									{
										ApplyDamageLE(le);
									}
									break;
								case Def.LivingEntityType.Prop:
									if (m_PropDamage)
									{
										ApplyDamageLE(le);
									}
									break;
							}
						}
					}
					break;
			}
		}
		private void OnTriggerEnter(Collider other)
		{
			switch (other.gameObject.layer)
			{
				case Def.RCLayerLE:
					{
						if (!other.gameObject.TryGetComponent(out CLivingEntity le))
							other.gameObject.transform.parent.TryGetComponent(out le);
						if (le != null)
						{
							switch (le.GetLEType())
							{
								case Def.LivingEntityType.ODD:
								case Def.LivingEntityType.Monster:
									if (m_Spell.GetCasterLE() == le)
									{
										if (!m_Spell.DamagesCaster())
											return;
									}
									if (m_MonsterDamage)
									{
										ApplyDamageLE(le);
									}
									break;
								case Def.LivingEntityType.Prop:
									if (m_PropDamage)
									{
										ApplyDamageLE(le);
									}
									break;
							}
						}
					}
					break;
			}
		}
		private void Update()
		{
			if (Manager.Mgr.IsPaused)
				return;
			UpdateHitEntities();
		}
	}
}
