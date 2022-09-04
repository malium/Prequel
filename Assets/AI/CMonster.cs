/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AI
{
	public class CMonster : MonoBehaviour
	{
		public const float DefaultFrameWait = 0.5f;
		public const float DefaultAngularSpeed = 10f;
		const int HitFrameCount = 10;
		static int MonsterID = 0;
		static VFXDef[] m_BloodTrails;
		static float[] m_BloodTrailScales;

		enum SpriteVFacing
		{
			FRONT,
			BACK
		}

		[SerializeReference] CLivingEntity m_LE;
		[SerializeReference] CMovableEntity m_ME;
		[SerializeReference] CSpellCaster m_SpellCaster;
		[SerializeField] SpriteBackendSQuad m_Sprite;
		[SerializeField] SpriteBackendSprite m_Shadow;
		[SerializeField] MonsterFamily m_Family;
		[SerializeField] int m_CurrentFrame;
		[SerializeField] int m_LastFrameIDX;
		[SerializeField] float m_FrameChangeWait;
		[SerializeField] float m_NextFrameChange;
		[SerializeField] SpriteVFacing m_Facing;
		[SerializeReference] SquashStrechAnimation m_SquashStrechAnimation;

		public CLivingEntity GetLE() => m_LE;
		public CMovableEntity GetME() => m_ME;
		public CSpellCaster GetSpellCaster() => m_SpellCaster;
		public ISpriteBackend GetSprite() => m_Sprite;
		public ISpriteBackend GetShadow() => m_Shadow;
		public MonsterFamily GetFamily() => m_Family;
		public void SetFrameChangeWait(float wait) => m_FrameChangeWait = wait;
		public void SetMonster(MonsterFamily family)
		{
			gameObject.name = "Monster_" + (MonsterID++).ToString();
			m_Family = family;

			m_FrameChangeWait = DefaultFrameWait;
			m_CurrentFrame = 0;
			m_LastFrameIDX = 0;
			m_NextFrameChange = Time.time + UnityEngine.Random.value;

			m_Sprite = SpriteUtils.AddSprite(
				new GameObject(gameObject.name + "_Sprite"),
				SpriteBackendType.SQUAD,
				m_Family.Frames[0]) as SpriteBackendSQuad;
			m_Sprite.transform.SetParent(transform);
			m_Sprite.gameObject.layer = Def.RCLayerLE;
			m_Sprite.GetRenderer().gameObject.layer = Def.RCLayerLE;
			
			m_Sprite.transform.localScale = new Vector3(m_Family.Info.SpriteScale, m_Family.Info.SpriteScale, 1f);
			gameObject.layer = Def.RCLayerLE;

			var texSize = m_Sprite.GetSprite().texture.width;
			float scale = texSize / m_Sprite.GetSprite().pixelsPerUnit;
			var pivot = m_Sprite.GetSprite().pivot / m_Sprite.GetSprite().pixelsPerUnit;
			Vector2 minmaxHeight = new Vector2(float.MaxValue, float.MinValue);
			float nRadius = 0f;
			for (int i = 0; i < (int)Def.MonsterFrame.FACE_ATTACK; ++i)
			{
				var rect = m_Family.VisibleRect[i];
				if (minmaxHeight.x > rect.yMin)
					minmaxHeight.Set(rect.yMin, minmaxHeight.y);
				if (minmaxHeight.y < rect.yMax)
					minmaxHeight.Set(minmaxHeight.x, rect.yMax);
				var radius = rect.width * 0.5f;
				radius /= m_Family.Frames[i].pixelsPerUnit;
				if (nRadius < radius)
					nRadius = radius;
			}
			nRadius *= 0.75f;
			nRadius *= m_Family.Info.SpriteScale;
			minmaxHeight /= m_Family.Frames[0].pixelsPerUnit;
			float vHeight = minmaxHeight.y - minmaxHeight.x;
			float height = vHeight * scale * m_Family.Info.SpriteScale * 0.75f;
			float halfHeight = height * 0.5f;
			float centerY = halfHeight + minmaxHeight.x * m_Family.Info.SpriteScale;

			m_Shadow = SpriteUtils.AddSprite(
				new GameObject(gameObject.name + "_Shadow"),
				SpriteBackendType.SPRITE,
				Manager.Mgr.SpriteShadow) as SpriteBackendSprite;
			m_Shadow.transform.SetParent(transform);
			m_Shadow.gameObject.layer = Def.RCLayerLE;
			
			var shadowSize = m_Shadow.GetSprite().texture.width;
			var shadowOffset = 0.5f + shadowSize * 0.001f;
			m_Shadow.transform.localPosition = new Vector3(-shadowOffset, 0.01f, -shadowOffset);
			m_Shadow.transform.Rotate(Vector3.right, 90f, Space.Self);

			m_LE = gameObject.AddComponent<CLivingEntity>();
			m_LE.Init(Def.LivingEntityType.Monster, 1f, 0f, 1f, 0f, null, gameObject.AddComponent<CharacterController>(), nRadius, height, m_Family.Name);
			m_LE.OnDamageFX += OnDamageFX;
			m_ME = gameObject.AddComponent<CMovableEntity>();
			m_ME.SetME(new MovableInfo()
			{
				MaxSpeed = m_Family.Info.BaseSpeed,
				AngularSpeed = DefaultAngularSpeed,
				ControllerCenter = new Vector3(0f, centerY, 0f),
				MaxStep = m_Family.Info.StepDistance > height ? height : m_Family.Info.StepDistance,
				Weight = m_Family.Info.Weight,
				MaxSightMovDir = 0f,
				SightSpeed = DefaultAngularSpeed
			});
			m_SpellCaster = gameObject.AddComponent<CSpellCaster>();
			if (m_BloodTrails == null)
			{
				var vfxFamilyIdx = VFXs.FamilyDict[(int)Def.VFXTarget.GENERAL]["Blood"];
				var vfxFamily = VFXs.VFXFamilies[(int)Def.VFXTarget.GENERAL][vfxFamilyIdx];

				m_BloodTrails = new VFXDef[vfxFamily.CastVFX.Length];
				m_BloodTrailScales = new float[m_BloodTrails.Length];
				for (int i = 0; i < m_BloodTrails.Length; ++i)
				{
					m_BloodTrails[i] = new VFXDef(Def.VFXTarget.GENERAL, "Blood", Def.VFXType.CAST, i, Def.VFXFacing.DontFaceAnything, Def.VFXEnd.SelfDestroy, 12f);
					m_BloodTrailScales[i] = 1f;
				}
				m_BloodTrailScales[1] = 0.5f;
			}

			m_LE.OnReceiveDamage += OnReceiveDamage;

			OnFamilyUpdated();
		}

		private void OnReceiveDamage(CLivingEntity caster, CLivingEntity receiver, Def.DamageType type, float damageAmout)
		{
			if (type == Def.DamageType.UNAVOIDABLE)
				return;

			m_Sprite.SetColor(Color.red);

			if (m_SquashStrechAnimation == null)
				m_SquashStrechAnimation = gameObject.AddComponent<SquashStrechAnimation>();

			m_SquashStrechAnimation.Set(transform, Vector3.one, 0.33f, true, SquashStrechAnimation.RoundsType.Monsters, false);
			m_SquashStrechAnimation.enabled = true;
		}

		public void OnFamilyUpdated()
		{
			float previousScale = m_Sprite.transform.localScale.x;
			float scaleMultiplier = m_Family.Info.SpriteScale / previousScale;

			m_LE.OnInfoUpdated(m_Family.Info.BaseHealth, m_Family.Info.HealthRegen,
				m_Family.Info.BaseSoulness, m_Family.Info.SoulnessRegen,
				new Resistance[]
				{
					new Resistance(){Type = Def.ResistanceType.PHYSICAL, Value = m_Family.Info.PhysicalResistance },
					new Resistance(){Type = Def.ResistanceType.ELEMENTAL, Value = m_Family.Info.ElementalResistance },
					new Resistance(){Type = Def.ResistanceType.ULTIMATE, Value = m_Family.Info.UltimateResistance },
					new Resistance(){Type = Def.ResistanceType.SOUL, Value = m_Family.Info.SoulResistance },
					new Resistance(){Type = Def.ResistanceType.POISON, Value = m_Family.Info.PoisonResistance },
				},
				m_LE.GetRadius() * scaleMultiplier,
				m_LE.GetHeight() * scaleMultiplier);

			m_ME.SetMaxSpeed(m_Family.Info.BaseSpeed);
			m_ME.SetWeight(m_Family.Info.Weight);
			m_Sprite.transform.localScale = new Vector3(m_Family.Info.SpriteScale, m_Family.Info.SpriteScale, 1f);
			var controller = m_ME.GetController();
			controller.height = controller.height * scaleMultiplier;
			controller.radius = controller.radius * scaleMultiplier;
			controller.stepOffset = m_Family.Info.StepDistance > controller.height ? controller.height : m_Family.Info.StepDistance;
			//controller.enableOverlapRecovery = false;
			controller.center = new Vector3(controller.center.x, controller.center.y * scaleMultiplier, controller.center.z);
			// Spells
			m_SpellCaster.Init(m_LE, m_ME, new List<AttackInfo>(m_Family.Info.Attacks));
			//for (int i = 0; i < m_Spells.Length; ++i)
			//{
			//	if (m_Spells[i] != null)
			//		m_Spells[i].DestroySpell();
			//	m_Spells[i] = null;
			//}
			//for (int i = 0; i < m_Family.Info.Attacks.Length; ++i)
			//{
			//	var attackInfo = m_Family.Info.Attacks[i];
			//	if (attackInfo == null)
			//		continue;
			//	var spell = Spells.SpellManager.CreateSpell(attackInfo.AttackName);
			//	for (int j = 0; j < attackInfo.Configuration.Count; ++j)
			//	{
			//		var cur = attackInfo.Configuration[j];
			//		IConfig enumConf = null;
			//		if (cur.ConfigType == Def.ConfigType.ENUM)
			//		{
			//			for (int k = 0; k < spell.GetConfig().Count; ++k)
			//			{
			//				var config = spell.GetConfig()[k];
			//				if (config.GetConfigName() == cur.Name)
			//				{
			//					enumConf = config;
			//					break;
			//				}
			//			}
			//		}
			//		spell.SetConfig(cur.Create(enumConf));
			//	}
			//	for (int j = 0; j < attackInfo.OnHitConfiguration.Count; ++j)
			//	{
			//		var cur = attackInfo.OnHitConfiguration[j];
			//		spell.AddOnHit(cur);
			//	}
			//	spell.SetCaster(m_LE);
			//	spell.InitSpell();
			//	m_Spells[i] = spell;
			//}
			//if(m_Spells[(int)Def.MonsterSpellSlots.AUTO] == null)
			//{
			//	m_Spells[(int)Def.MonsterSpellSlots.AUTO] = Spells.SpellManager.CreateSpell("NullSpell");
			//}
		}
		void OnDamageFX(Vector3 pos, Vector3 dir)
		{
			// TODO Cast blood trails
		}
		void UpdateFacing()
		{
			var cam = CameraManager.Mgr;
			//var camPos = cam.transform.position;
			var camDirXZ = new Vector2(cam.transform.forward.x, cam.transform.forward.z);
			
			float angleBetween = Mathf.Rad2Deg * GameUtils.AngleBetween2D(camDirXZ, m_ME.GetDirection());
			float angleAbs = Mathf.Abs(angleBetween);

			if (angleAbs > 5f && angleAbs < 175f)
			{
				m_Sprite.Flip(angleBetween > 0f, false);
			}
			if (angleAbs >= 0f && angleAbs < 87.5f)
				m_Facing = SpriteVFacing.BACK;
			else if (angleAbs > 92.5f && angleAbs <= 180f)
				m_Facing = SpriteVFacing.FRONT;
		}
		int GetCurrentFrameIndex()
		{
			if (m_SpellCaster.IsAttacking())
			{
				return (int)(m_Facing == SpriteVFacing.FRONT ?
						Def.MonsterFrame.FACE_ATTACK :
						Def.MonsterFrame.BACK_ATTACK);
			}
			return m_Facing == SpriteVFacing.FRONT ? m_CurrentFrame : (int)Def.MonsterFrame.BACK_1 + m_CurrentFrame;
		}
		public void FrameUpdate()
		{
			if(m_NextFrameChange < Time.time)
			{
				m_CurrentFrame = m_CurrentFrame == 0 ? 1 : 0;

				m_NextFrameChange = Time.time + m_FrameChangeWait;
			}
			bool wasFlipped = m_Sprite.IsHorizontalFlip();
			UpdateFacing();

			int curIdx = GetCurrentFrameIndex();
			if(m_LastFrameIDX != curIdx || (wasFlipped != m_Sprite.IsHorizontalFlip()))
			{
				var sprite = m_Family.Frames[curIdx];
				m_Sprite.ChangeSprite(sprite);
				m_LastFrameIDX = curIdx;
			}
		}
		private void FixedUpdate()
		{
			var hitDiff = Time.frameCount - m_LE.GetLastHitFrame();
			if (hitDiff > HitFrameCount)
			{
				var color = m_Sprite.GetColor();
				color.r = 1f;
				color.g = 1f;
				color.b = 1f;
				m_Sprite.SetColor(color);
			}
		}
		private void OnEnable()
		{
			if (m_Sprite != null)
				m_Sprite.SetEnabled(true);
			if (m_Shadow != null)
				m_Shadow.SetEnabled(true);
			//if (m_StatusBars != null)
			//	m_StatusBars.gameObject.SetActive(true);
		}
		private void OnDisable()
		{
			if (m_Sprite != null)
				m_Sprite.SetEnabled(false);
			if (m_Shadow != null)
				m_Shadow.SetEnabled(false);
			//if (m_StatusBars != null)
			//	m_StatusBars.gameObject.SetActive(false);
		}
		private void LateUpdate()
		{
			var posY = transform.position.y;
			var camPos = CameraManager.Mgr.transform.position;
			m_Sprite.transform.LookAt(new Vector3(camPos.x, posY + 1f, camPos.z));
		}
	}
}