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

namespace Assets.AI
{
	public class CProp : MonoBehaviour
	{
		static int PropID = 0;

		[SerializeReference] CLivingEntity m_LE;
		[SerializeField] SpriteBackendSQuad m_Sprite;
		[SerializeField] SpriteBackendSprite m_Shadow;
		[SerializeField] Light m_Light;
		[SerializeField] PropInfo m_Info;

		[SerializeField] SquashStrechAnimation m_OnDamageAnimation;

		static Collider[] SpawnColliders;

		public CLivingEntity GetLE() => m_LE;
		public ISpriteBackend GetSprite() => m_Sprite;
		public ISpriteBackend GetShadow() => m_Shadow;
		public Light GetLight() => m_Light;
		public PropInfo GetInfo() => m_Info;
		bool CheckSpawnCollision()
		{
			var box = m_LE.GetCollider() as BoxCollider;
			box.enabled = false;

			if (SpawnColliders == null)
				SpawnColliders = new Collider[32];

			var contactCount = Physics.OverlapBoxNonAlloc(box.center + box.transform.position, box.size * 0.5f, SpawnColliders, box.transform.rotation, 1 << Def.RCLayerBlock);

			if (contactCount < 2)
			{
				box.enabled = true;
				return false;
			}

			int count = 0;
			var pilar = m_LE.GetCurrentBlock().GetPilar(); //m_Block.GetPilar();
			var id = pilar.GetStructureID();
			if (pilar.GetStruc() == null)
				return false;
			var ppos = pilar.GetStruc().VPosFromPilarID(id);
			for(int i = 0; i < contactCount; ++i)
			{
				var cur = SpawnColliders[i];
				if(!cur.gameObject.TryGetComponent(out IBlock b))
				{
					if (!cur.gameObject.transform.parent.gameObject.TryGetComponent(out b))
						continue;
				}
				
				var bPilar = b.GetPilar();
				if(bPilar == pilar)
				{
					++count;
				}
				else if(b.GetBlockType() == Def.BlockType.WIDE)
				{
					var bid = bPilar.GetStructureID();
					var bpos = bPilar.GetStruc().VPosFromPilarID(bid);
					if(((ppos.x == (bpos.x + 1)) && ppos.y == bpos.y) || (ppos.x == bpos.x && (ppos.y == (bpos.y + 1))) || ((ppos.x == (bpos.x + 1)) && ppos.y == (bpos.y + 1)))
					{
						++count;
					}
				}
				if (count > 1)
					break;
			}

			box.enabled = true;

			return count > 1;
		}
		public void SetProp(int familyID, int propID)
		{
			gameObject.name = "Prop_" + (PropID++).ToString();
			gameObject.layer = Def.RCLayerLE;
			m_Info = Props.PropFamilies[familyID].Props[propID];

			m_Sprite = SpriteUtils.AddSprite(
				new GameObject(gameObject.name + "_Sprite"),
				SpriteBackendType.SQUAD,
				m_Info.PropSprite) as SpriteBackendSQuad;
			m_Sprite.transform.SetParent(transform);
			m_Sprite.gameObject.layer = Def.RCLayerLE;
			m_Sprite.GetRenderer().gameObject.layer = Def.RCLayerLE;

			var texSize = m_Info.PropSprite.texture.width;
			float scale = texSize / m_Info.PropSprite.pixelsPerUnit;
			var pivot = m_Info.PropSprite.pivot / m_Info.PropSprite.pixelsPerUnit;

			m_Sprite.transform.localScale = new Vector3(m_Info.SpriteScale, m_Info.SpriteScale, 1f);
			
			var collider = gameObject.AddComponent<BoxCollider>();
			collider.size = new Vector3(m_Info.GetBoxWidth() * scale, m_Info.GetBoxSize().y, m_Info.GetBoxWidth() * scale);
			collider.center = new Vector3(0f, m_Info.PropSprite.bounds.center.y + m_Info.GetBoxCenterOffset().y, 0f);

			if (collider.size.y < 1f)
			{
				var halfDiff = (1f - collider.size.y) * 0.5f;
				var center = collider.center;
				center.y += halfDiff;
				collider.center = center;
				var size = collider.size;
				size.y = 1f;
				collider.size = size;
			}

			m_LE = gameObject.AddComponent<CLivingEntity>();
			m_LE.Init(
				Def.LivingEntityType.Prop,
				m_Info.BaseHealth, 0f, 0f, 0f,
				new Resistance[]
				{
					new Resistance(){ Type = Def.ResistanceType.PHYSICAL,	Value = m_Info.PhysicalResistance * 0.01f,	HealIfNegative = false },
					new Resistance(){ Type = Def.ResistanceType.ELEMENTAL,	Value = m_Info.ElementalResistance * 0.01f,	HealIfNegative = false },
					new Resistance(){ Type = Def.ResistanceType.ULTIMATE,	Value = m_Info.UltimateResistance * 0.01f,	HealIfNegative = false },
					new Resistance(){ Type = Def.ResistanceType.SOUL,		Value = m_Info.SoulResistance * 0.01f,		HealIfNegative = false },
					new Resistance(){ Type = Def.ResistanceType.POISON,		Value = m_Info.PoisonResistance * 0.01f,	HealIfNegative = false },
				},
				collider,
				(m_Info.GetVisibleRect().width * 0.5f) / m_Info.PropSprite.pixelsPerUnit,
				collider.size.y,
				m_Info.FamilyName
			);

			m_LE.OnReceiveDamage += OnReceiveDamage;
			m_LE.OnReceiveElement += OnReceiveElement;

			m_LE.OnDamageFX += OnDamageFX;

			if(m_Info.HasShadow)
			{
				m_Shadow = SpriteUtils.AddSprite(
					new GameObject(gameObject.name + "_Shadow"),
					SpriteBackendType.SPRITE,
					Manager.Mgr.SpriteShadow) as SpriteBackendSprite;

				m_Shadow.gameObject.layer = Def.RCLayerLE;
				m_Shadow.transform.SetParent(transform);
				var shadowSize = Manager.Mgr.SpriteShadow.texture.width;
				var shadowOffset = 0.5f + shadowSize * 0.001f;
				m_Shadow.transform.localPosition = new Vector3(-shadowOffset, 0.01f, -shadowOffset);
				m_Shadow.transform.Rotate(Vector3.right, 90f, Space.Self);
			}
			if(m_Info.HasLighting)
			{
				m_Light = new GameObject(gameObject.name + "_Light").AddComponent<Light>();
				m_Light.transform.SetPositionAndRotation(gameObject.transform.position, Quaternion.identity);
				m_Light.transform.SetParent(transform);
				m_Light.transform.Translate(new Vector3(0f, m_Info.LightHeight, 0f), Space.Self);
				m_Light.intensity = m_Info.LightIntensity;
				m_Light.color = m_Info.LightColor;
				m_Light.range = m_Info.LightRange;
				m_Light.shadows = LightShadows.None;
			}
		}
		private void OnDamageFX(Vector3 pos, Vector3 dir)
		{
			const int particleMin = 3;
			const int particleMax = 4;
			var particleCount = UnityEngine.Random.Range(particleMin, particleMax + 1);
			var colors = m_Info.GetDamageColors();
			var colorProbs = m_Info.GetDamageColorProbs();
			for (int i = 0; i < particleCount; ++i)
			{
				var particle = new GameObject(gameObject.name + "_Particle").AddComponent<PropDamageParticle>();
				particle.transform.position = pos;
				var dirOffset = new Vector3(
					UnityEngine.Random.value * 0.4f - 0.2f,
					UnityEngine.Random.value * 0.4f - 0.2f,
					UnityEngine.Random.value * 0.4f - 0.2f);
				var particleDir = (dir + dirOffset).normalized;
				particle.Set(new Color32[]
					{
						colors[GameUtils.GetRandomFromProbs(colorProbs)],
						colors[GameUtils.GetRandomFromProbs(colorProbs)],
						colors[GameUtils.GetRandomFromProbs(colorProbs)],
						colors[GameUtils.GetRandomFromProbs(colorProbs)]
					}, particleDir);
				particle.GetSprite().GetRenderer().gameObject.layer = Def.RCLayerLE;
			}
			if (m_LE.GetCurrentHealth() <= 0f)
			{
				particleCount = (int)Mathf.Ceil(particleCount * (UnityEngine.Random.value + 3f)) - particleCount;
				for (int i = 0; i < particleCount; ++i)
				{
					var particle = new GameObject(gameObject.name + "_Particle").AddComponent<PropDamageParticle>();
					particle.transform.position = pos;
					var particleDir = new Vector3(
						UnityEngine.Random.value * 2f - 1f,
						dir.y + UnityEngine.Random.value * 0.4f - 0.2f,
						UnityEngine.Random.value * 2f - 1f);
					particle.Set(new Color32[]
					{
						colors[GameUtils.GetRandomFromProbs(colorProbs)],
						colors[GameUtils.GetRandomFromProbs(colorProbs)],
						colors[GameUtils.GetRandomFromProbs(colorProbs)],
						colors[GameUtils.GetRandomFromProbs(colorProbs)]
					}, particleDir.normalized);
					particle.GetSprite().GetRenderer().gameObject.layer = Def.RCLayerLE;
				}
			}
		}
		void ApplyDamageAnimation()
		{
			if(m_OnDamageAnimation == null)
			{
				m_OnDamageAnimation = gameObject.AddComponent<SquashStrechAnimation>();
			}
			m_OnDamageAnimation.Set(m_Sprite.transform, new Vector3(m_Info.SpriteScale, m_Info.SpriteScale, 1f), 1f, true, SquashStrechAnimation.RoundsType.Props, false);
			m_OnDamageAnimation.enabled = true;
		}
		bool CheckHealth(bool castAnim = true)
		{
			if(m_LE.GetCurrentHealth() <= 0f)
			{
				m_LE.GetCurrentBlock().SetProp(null);
				m_LE.GetCollider().enabled = false;
				//if(m_LE.GetCurrentBlock().GetPilar().GetStruc().GetLES().Contains(m_LE))
				//{
				//	m_LE.GetCurrentBlock().GetPilar().GetStruc().GetLES().Remove(m_LE);
				//}
				enabled = false;
				m_Sprite.SetEnabled(true);
				if(m_Shadow != null)
					m_Shadow.SetEnabled(true);
				if(m_Light != null)
					m_Light.enabled = true;
				if (castAnim)
				{
					var fadeAnim = gameObject.AddComponent<FadeoutAnimation>();
					fadeAnim.Set(
							() =>
							{
								return m_Sprite.GetColor();
							},
							(Color color) =>
							{
								m_Sprite.SetColor(color);
								if (m_Shadow != null)
								{
									var sColor = m_Shadow.GetColor();
									sColor.a = color.a;
									m_Shadow.SetColor(sColor);
								}
								if (m_Light != null)
								{
									m_Light.intensity = m_Info.LightIntensity * color.a;
								}
							},
							1f, false,
							() =>
							{
								GameUtils.DeleteGameobject(gameObject);
							});
				}
				else
				{
					GameUtils.DeleteGameobject(gameObject);
				}
				return false;
			}
			return true;
		}
		void OnReceiveElement(CLivingEntity receiver, Def.ElementType type, float damage)
		{
			CheckHealth();
		}
		void OnReceiveDamage(CLivingEntity caster, CLivingEntity receiver, Def.DamageType type, float damageAmout)
		{
			if (CheckHealth(type != Def.DamageType.UNAVOIDABLE))
				ApplyDamageAnimation();
		}
		private void Update()
		{
			if (Manager.Mgr.IsPaused)
				return;
			
			m_LE.UpdateElements();
		}
		private void FixedUpdate()
		{
			if (Manager.Mgr.IsPaused)
				return;

			if (m_LE.GetCurrentBlock() == null)
			{
				Debug.LogWarning(gameObject.name + " not attached to a block!");
				return;
			}
			var pilarPos = m_LE.GetCurrentBlock().GetPilar().transform.position;
			transform.position = new Vector3(pilarPos.x + 0.5f, m_LE.GetCurrentBlock().transform.position.y, pilarPos.z + 0.5f);
			if (CheckSpawnCollision())
			{
				m_LE.ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_LE.GetCurrentHealth());
			}
			m_LE.UpdateStruc();
		}
		private void LateUpdate()
		{
			var posY = transform.position.y;
			var camPos = CameraManager.Mgr.transform.position;
			m_Sprite.transform.LookAt(new Vector3(camPos.x, posY + 1f, camPos.z));
		}
		private void OnDrawGizmos()
		{
			//var box = m_LE.GetCollider() as BoxCollider;
			//Gizmos.DrawCube(transform.position + box.center, box.size);
		}
		private void OnEnable()
		{
			if (m_Sprite != null)
				m_Sprite.enabled = true;
			if (m_Shadow != null)
				m_Shadow.enabled = true;
			if (m_Light != null)
				m_Light.enabled = true;
		}
		private void OnDisable()
		{
			if (m_Sprite != null)
				m_Sprite.enabled = false;
			if (m_Shadow != null)
				m_Shadow.enabled = false;
			if (m_Light != null)
				m_Light.enabled = false;
		}
	}
}
