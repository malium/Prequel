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
	public class CMelee : MonoBehaviour
	{
		MeleeSpell m_Spell;
		CMovableEntity m_CasterMov;
		CVFX m_UnderVFX;
		SpriteBackendSprite m_WeaponSprite;
		BoxCollider m_Collider;
		Impl.IMeleeUpdate m_Update;
		bool m_BlockCollide;
		bool m_PropCollide;
		bool m_MonsterCollide;
		bool m_BlockDamage;
		bool m_PropDamage;
		bool m_MonsterDamage;

		public MeleeSpell GetSpell() => m_Spell;
		public CVFX GetVFX() => m_UnderVFX;
		public SpriteBackendSprite GetWeaponSprite() => m_WeaponSprite;
		public BoxCollider GetCollider() => m_Collider;

		public void SetMelee(MeleeSpell spell)
		{
			m_Spell = spell;
			m_CasterMov = spell.GetCasterLE().gameObject.GetComponent<CMovableEntity>();

			gameObject.layer = Def.RCLayerProjectile;
			transform.SetParent(spell.GetCasterLE().transform);
			transform.localPosition = Vector3.zero;

			var hitMask = m_Spell.GetHitMask();
			var damageMask = m_Spell.GetDamageMask();

			m_BlockCollide = (hitMask & Def.SpellHitMask.BLOCKS) == Def.SpellHitMask.BLOCKS;
			m_PropCollide = (hitMask & Def.SpellHitMask.PROPS) == Def.SpellHitMask.PROPS;
			m_MonsterCollide = (hitMask & Def.SpellHitMask.MONSTERS) == Def.SpellHitMask.MONSTERS;

			m_BlockDamage = (damageMask & Def.SpellDamageMask.BLOCKS) == Def.SpellDamageMask.BLOCKS;
			m_PropDamage = (damageMask & Def.SpellDamageMask.PROPS) == Def.SpellDamageMask.PROPS;
			m_MonsterDamage = (damageMask & Def.SpellDamageMask.MONSTERS) == Def.SpellDamageMask.MONSTERS;

			var weaponSpriteIdx = SpellWeapons.Dict[spell.GetSpellWeapon()];
			var weaponSprite = SpellWeapons.List[weaponSpriteIdx];

			m_WeaponSprite = SpriteUtils.AddSprite(new GameObject("Melee_WeaponSprite"), 
				SpriteBackendType.SPRITE, weaponSprite) as SpriteBackendSprite;
			m_WeaponSprite.Flip(m_Spell.GetFlipWeaponX(), m_Spell.GetFlipWeaponY());
			m_WeaponSprite.gameObject.layer = Def.RCLayerProjectile;
			m_WeaponSprite.transform.SetParent(transform);
			m_WeaponSprite.transform.localPosition = Vector3.zero;
			var weaponScale = m_Spell.GetWeaponScale();
			m_WeaponSprite.transform.localScale = new Vector3(weaponScale.x, weaponScale.y, 1f);
			m_WeaponSprite.SetOnTriggerEnter(OnTriggerEnter);

			m_Collider = m_WeaponSprite.gameObject.AddComponent<BoxCollider>();
			m_Collider.size = m_Spell.GetPrismBaseHeightDepth();
			m_Collider.center = m_Spell.GetColliderOffset();
			m_Collider.isTrigger = true;
			var rb = m_WeaponSprite.gameObject.AddComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.useGravity = false;

			var underVFXConf = m_Spell.GetUnderWeaponEffect();
			if (underVFXConf.IsValid())
			{
				m_UnderVFX = new GameObject("Melee_WeaponUnderVFX").AddComponent<CVFX>();
				m_UnderVFX.Set(underVFXConf);
				m_UnderVFX.gameObject.layer = Def.RCLayerProjectile;
				m_UnderVFX.transform.SetParent(transform);
				var uFWD = m_Spell.GetUnderEffectOffsetForward();
				m_UnderVFX.transform.localPosition = new Vector3(0f, uFWD, 0f);
			}

			var attackType = m_Spell.GetAttackType();

			switch (attackType)
			{
				case Def.MeleeAttackType.Piercing:
					m_Update = new Impl.PierceMelee(this);
					break;
				case Def.MeleeAttackType.Slash:
					if (m_Spell.GetSlashFromOffset())
						m_Update = new Impl.SlashFromOffsetMelee(this);
					else
						m_Update = new Impl.SlashMelee(this, m_CasterMov);
					break;
				default:
					Debug.LogWarning("Unhandled MeleeSpell type " + attackType.ToString());
					break;
			}
			m_Update.Translate();
			UpdateRotation();
		}
		void ApplyDamageBridge(BridgeComponent bridge)
		{
			// TODO: deal damage
			m_Spell.TriggerOnBridgeHit(bridge);
		}
		void ApplyDamageBlock(CBlock block)
		{
			// TODO: deal damage
			m_Spell.TriggerOnBlockHit(block);
		}
		void ApplyDamageLE(CLivingEntity le)
		{
			le.TriggerDamageFX(transform.position, (le.transform.position - transform.position).normalized);
			m_Spell.TriggerOnEntityHit(le);
		}
		void OnCollision(Collider col)
		{
			var onHitVFXConf = m_Spell.GetOnHitVFX();
			if (onHitVFXConf.IsValid())
			{
				var onHitVFX = new GameObject("Melee_OnHit").AddComponent<CVFX>();
				onHitVFX.transform.position = col.transform.position +
					new Vector3(0f, m_Spell.GetOffsetHeight(), 0f);
				onHitVFX.transform.rotation = transform.rotation;
				onHitVFX.transform.Rotate(Vector3.right, -90f, Space.Self);
				onHitVFX.Set(onHitVFXConf);
				onHitVFX.GetSprite().GetRenderer().gameObject.layer = Def.RCLayerProjectile;
			}
		}
		private void OnTriggerEnter(Collider other)
		{
			switch (other.gameObject.layer)
			{
				case Def.RCLayerBlock:
					if (m_BlockCollide)
					{
						OnCollision(other);
					}
					if (m_BlockDamage)
					{
						if (!other.gameObject.TryGetComponent(out CBlock block))
							other.transform.parent.gameObject.TryGetComponent(out block);

						if (block != null)
							ApplyDamageBlock(block);
					}
					break;
				case Def.RCLayerBridge:
					if (m_BlockCollide)
					{
						OnCollision(other);
					}
					if (m_BlockDamage)
					{
						if (!other.gameObject.TryGetComponent(out BridgeComponent bridge))
							other.transform.parent.gameObject.TryGetComponent(out bridge);
						if (bridge != null)
							ApplyDamageBridge(bridge);
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
										return;
									}
									if (m_MonsterCollide)
									{
										OnCollision(other);
									}
									if (m_MonsterDamage)
									{
										ApplyDamageLE(le);
									}
									break;
								case Def.LivingEntityType.Prop:
									if (m_PropCollide)
									{
										OnCollision(other);
									}
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
		void UpdateRotation()
		{
			var casterView = m_CasterMov.GetDirection();
			var forward = casterView * m_Spell.GetOffsetForward();
			var height = m_Spell.GetOffsetHeight();
			var angle = GameUtils.AngleBetween2D(new Vector2(0, -1f), casterView) * Mathf.Rad2Deg;
			angle *= -1f;
			transform.localRotation = Quaternion.Euler(-90f, angle, 0f);
			transform.localPosition = new Vector3(forward.x, height, forward.y);
			//if(m_UnderVFX != null)
			//{
			//	var underFWD = casterView * m_Spell.GetUnderEffectOffsetForward();
			//	m_UnderVFX.transform.localPosition = new Vector3(underFWD.x, height, underFWD.y);
			//}
		}
		private void Update()
		{
			if (Manager.Mgr.IsPaused)
				return;

			UpdateRotation();

			if (m_Update.Update())
			{
				if (m_Update is Impl.PierceMelee)
				{
					m_Update = new Impl.InversePierceMelee(this);
					m_Collider.enabled = false;
				}
				else if(m_Update is Impl.SlashMelee)
				{
					m_Update = new Impl.SlashFadeOutMelee(this, m_CasterMov);
					m_Collider.enabled = false;
				}
				//else if(m_Update is Impl.SlashVerticalMelee)
				//{
				//	m_Update = new Impl.SlashVerticalFadeOutMelee(this, m_CasterMov);
				//	m_Collider.enabled = false;
				//}
				else if (m_Update is Impl.SlashFromOffsetMelee)
				{
					m_Update = new Impl.SlashFromOffsetFadeOutMelee(this);
					m_Collider.enabled = false;
				}
				else
				{
					GameUtils.DeleteGameobject(gameObject);
				}
			}
		}
	}
	namespace Impl
	{
		interface IMeleeUpdate
		{
			bool Update();
			void Translate();
		}
		class PierceMelee : IMeleeUpdate
		{
			CMelee m_Melee;
			float m_Distance;
			float m_Speed;
			SpriteBackendSprite m_WeaponSprite;

			public PierceMelee(CMelee melee)
			{
				m_Melee = melee;
				m_Distance = m_Melee.GetSpell().GetPierceDistance();
				m_Speed = m_Melee.GetSpell().GetSpeed();
				m_WeaponSprite = m_Melee.GetWeaponSprite();
			}
			public void Translate()
			{
				m_WeaponSprite.transform.localPosition = Vector3.zero;
			}
			public bool Update()
			{
				float movement = m_Speed * Time.deltaTime;
				m_WeaponSprite.transform.localPosition =
					m_WeaponSprite.transform.localPosition +
					new Vector3(0f, movement, 0f);

				if (m_WeaponSprite.transform.localPosition.y >= m_Distance)
				{
					m_WeaponSprite.transform.localPosition = new Vector3(
						m_WeaponSprite.transform.localPosition.x,
						m_Distance,
						m_WeaponSprite.transform.localPosition.z);
					return true;
				}
				return false;
			}
		}
		class InversePierceMelee : IMeleeUpdate
		{
			CMelee m_Melee;
			float m_Distance;
			float m_Speed;
			SpriteBackendSprite m_WeaponSprite;

			public InversePierceMelee(CMelee melee)
			{
				m_Melee = melee;
				m_Distance = m_Melee.GetSpell().GetPierceDistance();
				m_Speed = m_Melee.GetSpell().GetPierceReturnSpeed();
				m_WeaponSprite = m_Melee.GetWeaponSprite();
			}
			public void Translate()
			{

			}
			public bool Update()
			{
				float movement = m_Speed * Time.deltaTime;
				m_WeaponSprite.transform.localPosition =
					m_WeaponSprite.transform.localPosition -
					new Vector3(0f, movement, 0f);

				var dist = m_WeaponSprite.transform.localPosition.y;
				var alpha = dist / m_Distance;
				var rnd = m_WeaponSprite.GetRenderer() as SpriteRenderer;
				var color = rnd.color;
				color.a = alpha;
				rnd.color = color;

				if (dist <= 0f)
				{
					m_WeaponSprite.transform.localPosition = new Vector3(
						m_WeaponSprite.transform.localPosition.x,
						m_Distance,
						m_WeaponSprite.transform.localPosition.z);
					return true;
				}
				return false;
			}
		}
		class SlashMelee : IMeleeUpdate
		{
			CMelee m_Melee;
			float m_Arc;
			float m_Speed;
			float m_CurrentAngle;
			float m_TargetAngle;
			float m_ForwardOffset;
			float m_Direction;
			SpriteBackendSprite m_WeaponSprite;
			CMovableEntity m_CasterMov;
			Vector3 m_Angle;

			public SlashMelee(CMelee melee, CMovableEntity casterMov)
			{
				m_Melee = melee;
				m_CasterMov = casterMov;
				m_Arc = m_Melee.GetSpell().GetArcSize();
				m_Speed = m_Melee.GetSpell().GetSpeed();
				m_ForwardOffset = m_Melee.GetSpell().GetOffsetForward();
				m_WeaponSprite = m_Melee.GetWeaponSprite();
				m_Direction = m_Melee.GetSpell().GetFlip() ? -1f : 1f;
				float angleOffset = m_Melee.GetSpell().GetStartingOffset();
				var halfArc = m_Arc * 0.5f;
				m_CurrentAngle = halfArc * m_Direction + angleOffset;
				m_TargetAngle = halfArc * -m_Direction + angleOffset;
				if (m_Melee.GetSpell().GetVertical())
					m_Angle = Vector3.right;
				else
					m_Angle = Vector3.forward;
			}
			public void Translate()
			{
				var casterXZ = new Vector2(m_CasterMov.transform.position.x, m_CasterMov.transform.position.z);
				m_WeaponSprite.transform.position = new Vector3(casterXZ.x, m_WeaponSprite.transform.position.y,
					casterXZ.y);

				m_WeaponSprite.transform.localRotation = Quaternion.Euler(m_Angle * m_CurrentAngle);


				//var dirXZ = m_CasterMov.GetDirection();
				//var nPosXZ = casterXZ + dirXZ * m_ForwardOffset;
				//m_WeaponSprite.transform.position = new Vector3(nPosXZ.x, m_WeaponSprite.transform.position.y, nPosXZ.y);
				m_WeaponSprite.transform.position = m_WeaponSprite.transform.position +
					m_WeaponSprite.transform.up * m_ForwardOffset; // Vertical cannot have forward offset atm
			}
			public bool Update()
			{
				var movement = m_Speed * Time.deltaTime;
				m_CurrentAngle += -m_Direction * movement;

				Translate();

				bool done = false;
				if((m_Direction > 0f && m_CurrentAngle <= m_TargetAngle) || (m_Direction < 0f && m_CurrentAngle >= m_TargetAngle))
				//if((m_TargetAngle < 0f && m_CurrentAngle < m_TargetAngle) || (m_TargetAngle > 0f && m_CurrentAngle > m_TargetAngle) || (m_TargetAngle == 0f && Mathf.Approximately(m_CurrentAngle, 0f)))
				{
					done = true;
					m_CurrentAngle = m_TargetAngle;
				}

				return done;
			}
		}
		class SlashFadeOutMelee : IMeleeUpdate
		{
			CMelee m_Melee;
			float m_Arc;
			float m_Speed;
			float m_CurrentAngle;
			float m_TargetAngle;
			float m_FadeOutAngle;
			float m_Offset;
			float m_Direction;
			Vector3 m_Angle;
			SpriteBackendSprite m_WeaponSprite;
			CMovableEntity m_CasterMov;

			public SlashFadeOutMelee(CMelee melee, CMovableEntity casterMov)
			{
				m_Melee = melee;
				m_CasterMov = casterMov;
				m_Arc = m_Melee.GetSpell().GetArcSize();
				m_Speed = m_Melee.GetSpell().GetSpeed();
				m_Offset = m_Melee.GetSpell().GetOffsetForward();
				m_WeaponSprite = m_Melee.GetWeaponSprite();
				m_Direction = m_Melee.GetSpell().GetFlip() ? -1f : 1f;
				float angleOffset = m_Melee.GetSpell().GetStartingOffset();
				m_CurrentAngle = m_Arc * 0.5f * -m_Direction + angleOffset;
				m_FadeOutAngle = m_Melee.GetSpell().GetSlashFadeOutAngle();
				m_TargetAngle = m_CurrentAngle + m_FadeOutAngle * -m_Direction;
				if (m_Melee.GetSpell().GetVertical())
					m_Angle = Vector3.right;
				else
					m_Angle = Vector3.forward;
			}
			public void Translate()
			{

			}
			public bool Update()
			{
				var movement = m_Speed * Time.deltaTime;
				m_CurrentAngle += -m_Direction * movement;

				var casterXZ = new Vector2(m_CasterMov.transform.position.x, m_CasterMov.transform.position.z);
				m_WeaponSprite.transform.position = new Vector3(casterXZ.x, m_WeaponSprite.transform.position.y,
					casterXZ.y);
				m_WeaponSprite.transform.localRotation = Quaternion.Euler(m_Angle * m_CurrentAngle);

				m_WeaponSprite.transform.position = m_WeaponSprite.transform.position +
					m_WeaponSprite.transform.up * m_Offset;


				var curOffset = Mathf.Abs(m_TargetAngle - m_CurrentAngle);
				var alpha = curOffset / m_FadeOutAngle;
				var rnd = m_WeaponSprite.GetRenderer() as SpriteRenderer;
				var color = rnd.color;
				color.a = alpha;
				rnd.color = color;

				bool done = false;

				if (Mathf.Abs(m_CurrentAngle) > Mathf.Abs(m_TargetAngle))
				//if (nAngle <= m_TargetAngle)
				{
					done = true;
					m_CurrentAngle = m_TargetAngle;
				}
				//m_WeaponSprite.transform.RotateAround(m_Melee/*.GetSpell().GetCaster()*/.transform.position, Vector3.down, movement);

				return done;
			}
		}
		class SlashFromOffsetMelee : IMeleeUpdate
		{
			CMelee m_Melee;
			float m_Arc;
			float m_Speed;
			float m_CurrentAngle;
			Vector3 m_Angle;
			SpriteBackendSprite m_WeaponSprite;

			public SlashFromOffsetMelee(CMelee melee)
			{
				m_Melee = melee;
				m_Arc = m_Melee.GetSpell().GetArcSize();
				m_Speed = m_Melee.GetSpell().GetSpeed();
				m_WeaponSprite = m_Melee.GetWeaponSprite();
				m_CurrentAngle = m_Arc * 0.5f;
				if (m_Melee.GetSpell().GetVertical())
					m_Angle = Vector3.right;
				else
					m_Angle = Vector3.forward;
				m_WeaponSprite.transform.localRotation = Quaternion.Euler(m_Angle * m_Arc * 0.5f);
			}
			public void Translate()
			{
				m_WeaponSprite.transform.localRotation = Quaternion.Euler(m_Angle * m_CurrentAngle);
			}
			public bool Update()
			{
				m_CurrentAngle -= m_Speed * Time.deltaTime;

				bool done = false;
				if(m_CurrentAngle <= (-m_Arc * 0.5f))
				{
					done = true;
					m_CurrentAngle = -m_Arc * 0.5f;
				}
				m_WeaponSprite.transform.localRotation = Quaternion.Euler(m_Angle * m_CurrentAngle);
				
				return done;
			}
		}
		class SlashFromOffsetFadeOutMelee : IMeleeUpdate
		{
			CMelee m_Melee;
			float m_Arc;
			float m_Speed;
			float m_CurrentAngle;
			float m_FadeOutAngle;
			float m_TargetAngle;
			Vector3 m_Angle;
			SpriteBackendSprite m_WeaponSprite;

			public SlashFromOffsetFadeOutMelee(CMelee melee)
			{
				m_Melee = melee;
				m_Arc = m_Melee.GetSpell().GetArcSize();
				m_Speed = m_Melee.GetSpell().GetSpeed();
				m_WeaponSprite = m_Melee.GetWeaponSprite();
				m_CurrentAngle = m_Arc * 0.5f;
				m_FadeOutAngle = m_Melee.GetSpell().GetSlashFadeOutAngle();
				m_TargetAngle = m_CurrentAngle - m_FadeOutAngle;
				if (m_Melee.GetSpell().GetVertical())
					m_Angle = Vector3.right;
				else
					m_Angle = Vector3.forward;
				m_WeaponSprite.transform.localRotation = Quaternion.Euler(m_Angle * m_Arc * 0.5f);
			}
			public void Translate()
			{

			}
			public bool Update()
			{
				var nAngle = m_CurrentAngle - m_Speed * Time.deltaTime;

				var curOffset = Mathf.Abs(m_TargetAngle - m_CurrentAngle);
				var alpha = curOffset / m_FadeOutAngle;
				var rnd = m_WeaponSprite.GetRenderer() as SpriteRenderer;
				var color = rnd.color;
				color.a = alpha;
				rnd.color = color;

				bool done = false;
				if (nAngle <= m_TargetAngle)
				{
					done = true;
					nAngle = m_TargetAngle;
				}
				m_CurrentAngle = nAngle;
				m_WeaponSprite.transform.localRotation = Quaternion.Euler(m_Angle * nAngle);

				return done;
			}
		}
	}
}
