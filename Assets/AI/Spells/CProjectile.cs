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
	public class CProjectile : MonoBehaviour
	{
		const float FadeAwayTime = 0.5f;
		ProjectileSpell m_Spell;
		CVFX[] m_TravelVFXs;
		//CVFX m_TravelVFX;
		CLivingEntity m_TargetLE;
		Impl.IProjectileUpdate m_Update;
		Action m_FadeAwayFN = () => { };
		Collider m_Collider;
		float m_StartTime;

		bool m_BlockCollide;
		bool m_PropCollide;
		bool m_MonsterCollide;
		bool m_BlockDamage;
		bool m_PropDamage;
		bool m_MonsterDamage;

		bool m_TargetReached;
		bool m_FadingAway;
		float m_FadingStart;

		public ProjectileSpell GetSpell() => m_Spell;
		public Collider GetCollider() => m_Collider;
		public float GetStartingTime() => m_StartTime;
		public void SetProjectile(ProjectileSpell spell, CLivingEntity target, Vector3 targetPos)
		{
			m_Spell = spell;

			gameObject.layer = Def.RCLayerProjectile;

			//var castVFXConf = m_Spell.GetCastVFX();
			//if (castVFXConf.IsValid())
			//{
			//	var castVFX = new GameObject("Projectile_Cast").AddComponent<CVFX>();
			//	castVFX.transform.position = transform.position;
			//	castVFX.Set(castVFXConf);
			//}

			var travelVFXConf = m_Spell.GetTravelVFX();
			if (travelVFXConf.IsValid())
			{
				var vfxPlanes = m_Spell.GetVFXPlanes();
				var planeNum = vfxPlanes != Def.VFXPlanes.ONE ? 2 : 1;
				m_TravelVFXs = new CVFX[planeNum];
				for (int i = 0; i < planeNum; ++i)
				{
					var vfx = new GameObject("Projectile_Travel_" + i.ToString()).AddComponent<CVFX>();
					vfx.transform.SetParent(transform);
					vfx.transform.localPosition = Vector3.zero;
					vfx.Set(travelVFXConf);
					vfx.GetSprite().GetRenderer().gameObject.layer = Def.RCLayerProjectile;
					m_TravelVFXs[i] = vfx;
				}
				//var casterView = m_Spell.GetCaster().gameObject.GetComponent<CMovableEntity>().GetDirection();
				//var angle = GameUtils.AngleBetween2D(new Vector2(0, -1f), casterView) * Mathf.Rad2Deg;
				//angle *= -1f;
				//castVFX.transform.Rotate(Vector3.up, angle, Space.World);
				//castVFX.transform.Rotate(Vector3.right, -90f, Space.Self);
			}

			var travelCollider = m_Spell.GetTravelCollider();
			switch (travelCollider)
			{
				case Def.ProjectileTravelCollider.Sphere:
					{
						var sphere = gameObject.AddComponent<SphereCollider>();
						sphere.radius = m_Spell.GetSphereRadius();
						m_Collider = sphere;
					}
					break;
				case Def.ProjectileTravelCollider.Prism:
					{
						var box = gameObject.AddComponent<BoxCollider>();
						var prism = m_Spell.GetPrismBaseHeightDepht();
						box.size = prism;
						m_Collider = box;
					}
					break;
				default:
					Debug.LogError("Unhandled TravelCollider " + travelCollider.ToString());
					{
						var sphere = gameObject.AddComponent<SphereCollider>();
						sphere.radius = 1f;
						m_Collider = sphere;
					}
					break;
			}
			m_Collider.isTrigger = true;
			var rb = gameObject.AddComponent<Rigidbody>();
			rb.useGravity = false;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			m_TargetLE = target;


			Vector3 tPos;
			if(m_TargetLE != null)
			{
				tPos = m_TargetLE.transform.position + new Vector3(0f, m_TargetLE.GetHeight() * 0.5f, 0f);
			}
			else
			{
				tPos = targetPos;
			}
			var tPosXZ = new Vector2(tPos.x, tPos.z);
			var posXZ = new Vector2(transform.position.x, transform.position.z);
			var tdist = Vector2.Distance(posXZ, tPosXZ);
			var dir = (tPosXZ - posXZ).normalized;
			var dist = Mathf.Max(tdist, m_Spell.GetMaxTravelDistance());
			tPosXZ = posXZ + dist * dir;
			tPos.Set(tPosXZ.x, tPos.y, tPosXZ.y);
			var casterView = m_Spell.GetCasterLE().GetComponent<CMovableEntity>().GetDirection();
			var angle = GameUtils.AngleBetween2D(new Vector2(0, -1f), casterView) * Mathf.Rad2Deg;
			angle *= -1f;
			transform.Rotate(Vector3.up, angle + m_Spell.GetSpriteAngleOffset(), Space.World);
			if(m_TravelVFXs != null)
			{
				for(int i = 0; i < 1; ++i)
					m_TravelVFXs[i].transform.Rotate(Vector3.right, -90f, Space.Self);
			}
			//Debug.Log("Projectile angle " + angle.ToString());
			m_StartTime = Time.time;


			switch (m_Spell.GetTravelType())
			{
				case Def.ProjectileTravelType.LINEAR:
					m_Update = new Impl.LinearProjectile(this, tPos);
					break;
				case Def.ProjectileTravelType.PARABOLIC:
					m_Update = new Impl.ParabolicProjectile(this, tPos);
					break;
				case Def.ProjectileTravelType.TARGETTED:
					if (target != null)
						m_Update = new Impl.TargettedProjectile(this, target);
					else
						m_Update = new Impl.LinearProjectile(this, tPos);
					break;
				default:
					Debug.LogWarning("Unhandled travel type: " + m_Spell.GetTravelType().ToString());
					m_Update = new Impl.LinearProjectile(this, tPos);
					break;
			}
			var hitMask = m_Spell.GetHitMask();
			var damageMask = m_Spell.GetDamageMask();

			m_BlockCollide = (hitMask & Def.SpellHitMask.BLOCKS) == Def.SpellHitMask.BLOCKS;
			m_PropCollide = (hitMask & Def.SpellHitMask.PROPS) == Def.SpellHitMask.PROPS;
			m_MonsterCollide = (hitMask & Def.SpellHitMask.MONSTERS) == Def.SpellHitMask.MONSTERS;

			m_BlockDamage = (damageMask & Def.SpellDamageMask.BLOCKS) == Def.SpellDamageMask.BLOCKS;
			m_PropDamage = (damageMask & Def.SpellDamageMask.PROPS) == Def.SpellDamageMask.PROPS;
			m_MonsterDamage = (damageMask & Def.SpellDamageMask.MONSTERS) == Def.SpellDamageMask.MONSTERS;
		}

		void ApplyOnHitBridge(BridgeComponent bridge)
		{
			// TODO: deal damage
			m_Spell.TriggerOnBridgeHit(bridge);
		}
		void ApplyOnHitBlock(CBlock block)
		{
			// TODO: deal damage
			m_Spell.TriggerOnBlockHit(block);
		}
		void ApplyOnHitLE(CLivingEntity le)
		{
			le.TriggerDamageFX(transform.position, m_Update.GetDirection());
			m_Spell.TriggerOnEntityHit(le);
		}
		public bool OnMaxDistance()
		{
			if (m_FadingAway)
				return false;

			if(m_Spell.GetMaxTravelDistance() >= 0f)
			{
				if (m_Spell.GetFadeAway())
				{
					OnFadeAway();
				}
				else
				{
					OnCollision();
					return true;
				}
			}
			return false;
		}
		public bool OnTargetReached()
		{
			if (m_TargetReached)
				return false;
			
			m_TargetReached = true;

			if (m_Spell.GetEndOnSelectedPos())
			{
				if (m_Spell.GetFadeAway())
				{
					OnFadeAway();
				}
				else
				{
					OnCollision();
					return true;
				}
			}

			return false;
		}
		public void OnFadeAway()
		{
			m_FadingAway = true;
			m_Collider.enabled = false;
			m_FadeAwayFN = FadeAwayAnim;
			m_FadingStart = Time.time;
		}
		void FadeAwayAnim()
		{
			for (int i = 0; i < m_TravelVFXs.Length; ++i)
			{
				var vfx = m_TravelVFXs[i];
				var color = vfx.GetSprite().GetColor();
				color.a = Mathf.Lerp(1f, 0f, (Time.time - m_FadingStart) / FadeAwayTime);
				if (color.a <= 0f)
				{
					// Destroy projectile
					GameUtils.DeleteGameobject(gameObject);
				}
				vfx.GetSprite().SetColor(color);
			}
		}
		void OnCollision()
		{
			// OnHit VFX
			var onHitVFXConf = m_Spell.GetOnHitVFX();
			if (onHitVFXConf.IsValid())
			{
				var onHitVFX = new GameObject("Projectile_OnHit").AddComponent<CVFX>();
				onHitVFX.transform.position = transform.position;
				onHitVFX.transform.rotation = transform.rotation;
				onHitVFX.transform.Rotate(Vector3.right, -90f, Space.Self);
				onHitVFX.Set(onHitVFXConf);
				onHitVFX.GetSprite().GetRenderer().gameObject.layer = Def.RCLayerProjectile;
			}
			// Apply damage area

			// Destroy projectile
			GameUtils.DeleteGameobject(gameObject);
		}
		void OnPlaneUpdate()
		{
			if (m_TravelVFXs == null || m_TravelVFXs.Length < 2)
				return;
			var cam = CameraManager.Mgr;
			if (transform.position.x < cam.transform.position.x)
				m_TravelVFXs[1].GetSprite().Flip(false, true);
			else
				m_TravelVFXs[1].GetSprite().Flip(false, false);
		}
		private void OnTriggerEnter(Collider other)
		{
			switch (other.gameObject.layer)
			{
				case Def.RCLayerBlock:
					if(m_BlockCollide)
					{
						OnCollision();
					}
					if(m_BlockDamage)
					{
						if (!other.gameObject.TryGetComponent(out CBlock block))
							other.transform.parent.gameObject.TryGetComponent(out block);

						if(block != null)
							ApplyOnHitBlock(block);
					}
					break;
				case Def.RCLayerBridge:
					if (m_BlockCollide)
					{
						OnCollision();
					}
					if (m_BlockDamage)
					{
						if (!other.gameObject.TryGetComponent(out BridgeComponent bridge))
							other.transform.parent.gameObject.TryGetComponent(out bridge);
						if (bridge != null)
							ApplyOnHitBridge(bridge);
					}
					break;
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
										if (!m_Spell.DamagesCaster() || Time.time < (m_StartTime + Def.SelfHitTimeDelay))
											return;
									}
									if (m_MonsterCollide)
									{
										OnCollision();
									}
									if (m_MonsterDamage)
									{
										ApplyOnHitLE(le);
									}
									break;
								case Def.LivingEntityType.Prop:
									if (m_PropCollide)
									{
										OnCollision();
									}
									if (m_PropDamage)
									{
										ApplyOnHitLE(le);
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

			OnPlaneUpdate();
			m_FadeAwayFN();
			if(m_Update.Update())
			{
				m_TargetReached = true;
				OnCollision();
			}
		}
		private void LateUpdate()
		{
			m_Update.LateUpdate();
		}
		//private void OnDrawGizmos()
		//{
		//	Gizmos.DrawSphere(m_Target, 0.25f);
		//}
	}
	namespace Impl
	{
		interface IProjectileUpdate
		{
			Vector3 GetDirection();
			bool Update();
			void LateUpdate();
		}
		class LinearProjectile : IProjectileUpdate
		{
			CProjectile m_Proj;
			Vector3 m_Direction;
			Vector3 m_Start;
			float m_StartingDistance;

			public LinearProjectile(CProjectile projectile, Vector3 targetPos)
			{
				m_Proj = projectile;
				m_Start = m_Proj.transform.position;
				m_Direction = (targetPos - m_Proj.transform.position).normalized;
				m_StartingDistance = Vector3.Distance(targetPos, m_Start);
			}
			public bool Update()
			{
				var curDist = Vector3.Distance(m_Proj.transform.position, m_Start);
				if (curDist >= m_Proj.GetSpell().GetMaxTravelDistance())
				{
					if (m_Proj.OnMaxDistance())
						return true;
				}
				if (curDist >= m_StartingDistance)
				{
					if (m_Proj.OnTargetReached())
						return true;
				}
				var movement = m_Proj.GetSpell().GetSpeed() * Time.deltaTime * m_Direction;
				m_Proj.transform.Translate(movement, Space.World);
				return false;
			}
			public void LateUpdate()
			{

			}
			public Vector3 GetDirection() => m_Direction;
		}
		class ParabolicProjectile : IProjectileUpdate
		{
			CProjectile m_Proj;
			Vector3 m_Direction;
			Vector3 m_Start;
			float m_StartingDistance;
			float m_Voy;
			Quaternion m_StartingAngle;
			public Vector3 GetDirection() => m_Direction;

			public ParabolicProjectile(CProjectile projectile, Vector3 targetPos)
			{
				m_Proj = projectile;
				m_Direction = (targetPos - m_Proj.transform.position).normalized;
				m_Start = m_Proj.transform.position;
				m_StartingDistance = Vector2.Distance(new Vector2(m_Proj.transform.position.x, m_Proj.transform.position.z), new Vector2(targetPos.x, targetPos.z));
				m_StartingAngle = m_Proj.transform.rotation;

				var vox = m_Proj.GetSpell().GetSpeed();
				//var voy = vo.y;
				var xTime = m_StartingDistance / vox;

				m_Voy = (targetPos.y - m_Proj.transform.position.y) / xTime - 0.5f * -9.81f * xTime;
			}
			public bool Update()
			{
				var curDist = Vector2.Distance(new Vector2(m_Proj.transform.position.x, m_Proj.transform.position.z), new Vector2(m_Start.x, m_Start.z));
				if (curDist >= m_Proj.GetSpell().GetMaxTravelDistance())
				{
					if (m_Proj.OnMaxDistance())
						return true;
				}
				if (curDist >= m_StartingDistance)
				{
					if (m_Proj.OnTargetReached())
						return true;
				}
				var movementXZ = m_Proj.GetSpell().GetSpeed() * Time.deltaTime * new Vector2(m_Direction.x, m_Direction.z);
				var t = Time.time - m_Proj.GetStartingTime();
				var h = m_Voy * t + 0.5f * -9.81f * t * t + m_Start.y;

				//var yPos = -0.5f * 9.81f * (t * t) + m_Proj.GetSpell().GetSpeed().y * t + m_Start.y;
				var movementY = h - m_Proj.transform.position.y;

				var movement = new Vector3(movementXZ.x, movementY, movementXZ.y);
				m_Proj.transform.Translate(movement, Space.World);
				if (m_Proj.transform.position.y <= -50f)
					return true;
				return false;
			}
			public void LateUpdate()
			{
				var t = Time.time - m_Proj.GetStartingTime();
				var vy = m_Voy + -9.81f*t;
				var dir = new Vector2(m_Proj.GetSpell().GetSpeed(), vy).normalized;
				var angle = GameUtils.AngleFromDir2D(dir) * Mathf.Rad2Deg;

				//var cam = CameraManager.Mgr;
				//Vector3 v = cam.transform.position - m_Proj.transform.position;
				//v.y = v.z = 0f;
				//dir = cam.transform.position - v;
				//m_Proj.transform.LookAt(dir);

				m_Proj.transform.rotation = /*m_Proj.transform.rotation * */ m_StartingAngle * Quaternion.Euler(0, 0f, angle + 60f);
				//m_Proj.transform.Rotate(Vector3.forward, angle, Space.World);
				//Debug.Log("Vy: " + vy.ToString() + " Angle: " + angle.ToString());
			}
		}
		class TargettedProjectile : IProjectileUpdate
		{
			CProjectile m_Proj;
			CLivingEntity m_Target;
			Vector3 m_Direction;

			public TargettedProjectile(CProjectile projectile, CLivingEntity target)
			{
				m_Proj = projectile;
				m_Target = target;
			}
			public bool Update()
			{
				if (m_Target == null || m_Target.GetCurrentHealth() <= 0f)
					return true;

				m_Direction  = (m_Target.transform.position - m_Proj.transform.position).normalized;

				var movement = m_Proj.GetSpell().GetSpeed() * Time.deltaTime * m_Direction;
				var dist = Vector3.Distance(m_Target.transform.position, m_Proj.transform.position);
				var movDist = Vector3.Distance(m_Proj.transform.position + movement, m_Target.transform.position);
				m_Proj.transform.Translate(movement, Space.World);
				if (movDist > dist)
					return true;

				return false;
			}
			public void LateUpdate()
			{

			}
			public Vector3 GetDirection() => m_Direction;
		}
	}
}
