/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AI
{
	[Serializable]
	public struct Resistance
	{
		public Def.ResistanceType Type;
		public float Value;
		public bool HealIfNegative;

		public static Def.ResistanceType GetResistanceTypeFromDamageType(Def.DamageType type)
		{
			switch (type)
			{
				case Def.DamageType.HIT:
				case Def.DamageType.CUT:
					return Def.ResistanceType.PHYSICAL;
				case Def.DamageType.FIRE:
				case Def.DamageType.ICE:
					return Def.ResistanceType.ELEMENTAL;
				case Def.DamageType.LIGHT:
				case Def.DamageType.ELECTRICAL:
					return Def.ResistanceType.ULTIMATE;
				case Def.DamageType.ASPHYXIA:
				case Def.DamageType.DEPRESSION:
					return Def.ResistanceType.SOUL;
				case Def.DamageType.POISON:
				case Def.DamageType.QUICKSILVER:
					return Def.ResistanceType.POISON;
			}
			return Def.ResistanceType.COUNT;
		}
	}
	[Serializable]
	public struct AppliedElment
	{
		public Def.ElementType Type;
		public float Duration;
		public float NextApplyTime;
	}

	public class CLivingEntity : MonoBehaviour
	{
		const bool EnableStatusBars = false;

		[SerializeField] float m_Health;
		[SerializeField] float m_MaxHealth;
		[SerializeField] float m_HealthRegen;
		[SerializeField] float m_Soulness;
		[SerializeField] float m_MaxSoulness;
		[SerializeField] float m_SoulnessRegen;

		[SerializeField] Collider m_Collider;
		[SerializeField] float m_Radius;
		[SerializeField] float m_Height;
		[SerializeReference] IStruc m_CurrentStruc;
		[SerializeReference] IBlock m_CurrentBlock;
		[SerializeField] Resistance[] m_Resistances;
		//[SerializeField] bool[] m_HealResistances;
		[SerializeField] List<AppliedElment> m_CurrentElements;
		[SerializeField] Def.LivingEntityType m_LEType;
		[SerializeField] string m_LEName;
		[SerializeField] int m_LastHitFrame;
		[SerializeField] float m_LastHitTime;
		[SerializeField] CLivingEntity m_LastHitter;
		[SerializeReference] LEStatusBars m_StatusBars;

		[SerializeField] Timer m_RegenerationTimer;

		public float GUIHeight { get; private set; }

		public event StrucChangeCB OnStrucChange;
		public event ReceiveDamageCB OnReceiveDamage;
		public event ReceiveHealingCB OnReceiveHealing;
		public event ReduceSoulnessCB OnReduceSoulness;
		public event ReceiveSoulnessCB OnReceiveSoulness;
		public event ReceiveElementCB OnReceiveElement;
		public event CastDamageFX OnDamageFX;
		public event EntityDeathCB OnEntityDeath;

		
		public CLivingEntity()
		{
			m_CurrentElements = new List<AppliedElment>();
			m_Resistances = new Resistance[Def.ResistanceTypeCount];
			//m_HealResistances = new bool[Def.ResistanceTypeCount];
			m_LEType = Def.LivingEntityType.COUNT;
			m_Health = float.MaxValue;
			m_MaxHealth = float.MaxValue;
			m_RegenerationTimer = new Timer(1f, true, true);
			m_RegenerationTimer.OnTimerTrigger += OnRegen;
		}
		public void Init(Def.LivingEntityType leType, float baseHealth, float healthRegen, float baseSoulness,
			float soulnessRegen, Resistance[] baseResistances, Collider collider, float radius, float height,
			string name)
		{
			m_LEType = leType;
			
			m_Collider = collider;
			m_LEName = name;

#pragma warning disable CS0162
			if (EnableStatusBars)
			{
				m_StatusBars = GameObject.Instantiate(AssetLoader.StatusBars);
				m_StatusBars.Init(this);
				//m_StatusBars.transform.SetParent(CameraManager.Mgr.Canvas.transform);
				m_StatusBars.gameObject.SetActive(true);
			}
#pragma warning restore CS0162

			//m_CurrentElements = new List<AppliedElment>();
			//m_BaseResistances = new Resistance[Def.ResistanceTypeCount];
			//m_CurrentResistances = new Resistance[Def.ResistanceTypeCount];
			OnInfoUpdated(baseHealth, healthRegen, baseSoulness, soulnessRegen, baseResistances, radius, height);
		}
		public void OnInfoUpdated(float maxHealth, float healthRegen, float maxSoulness, float soulnessRegen,
			Resistance[] resistances, float radius, float height)
		{
			m_Health = maxHealth;
			m_MaxHealth = maxHealth;
			m_HealthRegen = healthRegen;
			m_Soulness = maxSoulness;
			m_MaxSoulness = maxSoulness;
			m_SoulnessRegen = soulnessRegen;
			m_Radius = radius;
			m_Height = height;
			//if(m_StatusBars != null)
			//	m_StatusBars.transform.localPosition = new Vector3(0f, m_Height * 1.25f, 0f);
			if (resistances == null || (resistances != null && resistances.Length != Def.ResistanceTypeCount))
			{
				for (int i = 0; i < Def.ResistanceTypeCount; ++i)
				{
					m_Resistances[i] = new Resistance()
					{
						Type = (Def.ResistanceType)i,
						Value = 0.5f,
						HealIfNegative = false
					};
				}
			}
			else
			{
				for (int i = 0; i < Def.ResistanceTypeCount; ++i)
				{
					m_Resistances[i] = new Resistance()
					{
						Type = (Def.ResistanceType)i,
						Value = resistances[i].Value,
						HealIfNegative = resistances[i].HealIfNegative
					};
				}
			}
			//if (healFromResistances == null || (healFromResistances != null && healFromResistances.Length != Def.ResistanceTypeCount))
			//{
			//	for (int i = 0; i < Def.ResistanceTypeCount; ++i)
			//		m_HealResistances[i] = false;
			//}
			//else
			//{
			//	for (int i = 0; i < Def.ResistanceTypeCount; ++i)
			//		m_HealResistances[i] = healFromResistances[i];
			//}
		}
		public void UpdateStruc()
		{
			var world = World.World.gWorld;
			if (world == null)
				return;

			var struc = world.StrucFromWPos(new Vector2(transform.position.x, transform.position.z));
			//var vwPos = GameUtils.TransformPosition(new Vector2(transform.position.x, transform.position.z));
			//var vPos = world.VPosFromVWPos(vwPos);
			//world.vs
			//var vsPos = World.World.VSPosFromVPos(vPos);
			//var strucID = world.StrucIDFromVSPos(vsPos);
			//var struc = world.GetStrucs()[strucID];
			if (struc != m_CurrentStruc)
			{
				if(m_CurrentStruc != null)
				{
					if (m_CurrentStruc.GetLES().Contains(this))
						m_CurrentStruc.GetLES().Remove(this);
				}
				OnStrucChange?.Invoke(m_CurrentStruc, struc);
				m_CurrentStruc = struc;
				if(m_CurrentStruc != null)
				{
					if(!m_CurrentStruc.GetLES().Contains(this))
						m_CurrentStruc.GetLES().Add(this);
				}
			}
		}
		public IBlock ComputeCurrentBlock()
		{
			if (m_CurrentStruc == null)
				return null;

			return World.World.gWorld.GetBlockAtWPos(transform.position);

			//var vStrucPos = GameUtils.TransformPosition(new Vector2(m_CurrentStruc.transform.position.x, m_CurrentStruc.transform.position.z));
			//var vPos = GameUtils.TransformPosition(new Vector2(transform.position.x, transform.position.z));
			//var vPilarPos = vPos - vStrucPos;
			//if (vPilarPos.x < 0 || vPilarPos.x >= CStruc.Width ||
			//	vPilarPos.y < 0 || vPilarPos.y >= CStruc.Height)
			//	return null;

			//var id = m_CurrentStruc.PilarIDFromVPos(vPilarPos);
			//var pilar = m_CurrentStruc.GetPilars()[id];
			//if (pilar != null)
			//	return pilar.GetClosestBlock(transform.position.y);
			//return null;
		}
		public void SetCurrentBlock(IBlock block) => m_CurrentBlock = block;
		public void UpdateElements()
		{
			for(int i = 0; i < m_CurrentElements.Count; )
			{
				var curElem = m_CurrentElements[i];
				if(curElem.NextApplyTime > Time.time)
				{
					++i;
					continue;
				}
				curElem.Duration -= 1f;
				curElem.NextApplyTime = Time.time + 1f;
				m_CurrentElements[i] = curElem;
				ReceiveElementDamage(curElem.Type);
				if (curElem.Duration <= 0f)
					m_CurrentElements.RemoveAt(i);
				else
					++i;
			}
		}
		public void UpdateBlock()
		{
			m_CurrentBlock = ComputeCurrentBlock();
		}
		public float ApplyResistances(Def.DamageType damageType, float amount)
		{
			var resType = Resistance.GetResistanceTypeFromDamageType(damageType);
			if (resType == Def.ResistanceType.COUNT)
				return amount;
			return amount * m_Resistances[(int)resType].Value;
		}
		public void SpawnElement(Def.ElementType elementType)
		{
			float duration = ElementInfo.Duration[(int)elementType];
			if (ElementInfo.Stackable[(int)elementType])
			{
				AppliedElment stacked;
				stacked.Type = elementType;
				stacked.Duration = duration;
				stacked.NextApplyTime = Time.time + 0.75f;
				m_CurrentElements.Add(stacked);
			}
			else
			{
				bool hasCurElement = false;
				for (int i = 0; i < m_CurrentElements.Count; ++i)
				{
					if (m_CurrentElements[i].Type == elementType)
					{
						hasCurElement = true;
						break;
					}
				}
				if (!hasCurElement)
				{
					AppliedElment elem;
					elem.Type = elementType;
					elem.Duration = duration;
					elem.NextApplyTime = Time.time + 0.75f;
					m_CurrentElements.Add(elem);
				}
			}
			if (ElementInfo.Renewable[(int)elementType])
			{
				for (int i = 0; i < m_CurrentElements.Count; ++i)
				{
					var curEl = m_CurrentElements[i];
					if (curEl.Type != elementType)
						continue;

					curEl.Duration = duration;

					m_CurrentElements[i] = curEl;
				}
			}
		}
		public void ReceiveElementDamage(Def.ElementType elementType)
		{
			float damage = ElementInfo.DamagePerSecond[(int)elementType];
			var damageType = ElementInfo.DmgType[(int)elementType];
			damage = ApplyResistances(damageType, damage);
			if (damage <= 0f)
				return; // No damage, no healing

			// Reapply
			//var resisType = Resistance.GetResistanceTypeFromDamageType(damageType);
			//if (resisType != Def.ResistanceTypeCount)
			//{
			//    float reapplyChance = m_Resistances[(int)resisType].Value * 0.25f;
			//    var rng = (float)Manager.Mgr.DamageRNG.NextDouble();
			//    if (rng < reapplyChance)
			//    {
			//        SpawnElement(type);
			//    }
			//}
			m_Health -= damage;
			OnReceiveElement?.Invoke(this, elementType, damage);
			if (m_Health <= 0f)
			{
				if (m_CurrentStruc != null && m_CurrentStruc.GetLES().Contains(this))
					m_CurrentStruc.GetLES().Remove(this);
				OnEntityDeath?.Invoke(this);
			}
		}
		public void TriggerDamageFX(Vector3 pos, Vector3 dir)
		{
			OnDamageFX?.Invoke(pos, dir);
		}
		public void ReceiveDamage(CLivingEntity caster, Def.DamageType damageType, float amount)
		{
			float damage = ApplyResistances(damageType, amount);
			var resistanceType = Resistance.GetResistanceTypeFromDamageType(damageType);
			if(damage < 0f && !m_Resistances[(int)resistanceType].HealIfNegative)
			{
				damage = 0f;
			}
			if (damage == 0f)
				return; // no damage
			if(damage < 0f)
			{
				ReceiveHealing(caster, -damage);
				return; // damage converted into healing
			}
			var elementType = ElementInfo.GetElementTypeFromDamageType(damageType);
			if(elementType != Def.ElementType.COUNT)
			{
				float chance = m_Resistances[(int)resistanceType].Value;
				float rng = UnityEngine.Random.value;
				if(chance > rng)
				{
					SpawnElement(elementType);
				}	
			}
			//OnReceiveDamagePrev.Invoke(caster, this, damageType, damage);
			if (caster != this)
			{
				m_LastHitFrame = Time.frameCount;
				m_LastHitter = caster;
				m_LastHitTime = Time.time;
			}
			m_Health -= damage;
			OnReceiveDamage?.Invoke(caster, this, damageType, damage);
			if (m_Health <= 0f)
			{
				if (m_CurrentStruc != null && m_CurrentStruc.GetLES().Contains(this))
					m_CurrentStruc.GetLES().Remove(this);
				OnEntityDeath?.Invoke(this);
			}
		}
		public void ReceiveHealing(CLivingEntity caster, float amount)
		{
			if (amount <= 0f)
				return; // invalid healing

			var oHealth = m_Health;
			m_Health = Mathf.Min(m_Health + amount, m_MaxHealth);
			if (oHealth == m_Health)
				return; // no healing
			OnReceiveHealing?.Invoke(caster, this, m_Health - oHealth);
		}
		public void ReceiveSoulness(CLivingEntity caster, float amount)
		{
			if (amount <= 0f)
				return; // invalid

			var oSoulness = m_Soulness;
			m_Soulness = Mathf.Min(m_Soulness + amount, m_MaxSoulness);
			if (oSoulness == m_Soulness)
				return; // no healing
			OnReceiveSoulness?.Invoke(caster, this, m_Soulness - oSoulness);
		}
		public void ReduceSoulness(CLivingEntity caster, float amount)
		{
			if (amount <= 0f)
				return; // invalid

			var oSoulness = m_Soulness;
			m_Soulness = Mathf.Max(m_Soulness - amount, 0f);
			if (oSoulness == m_Soulness)
				return; // none
			OnReduceSoulness?.Invoke(caster, this, oSoulness - m_Soulness);
		}
		public float GetMaxHealth() => m_MaxHealth;
		public float GetCurrentHealth() => m_Health;
		public float GetHealthRegen() => m_HealthRegen;
		public Resistance[] GetResistances() => m_Resistances;
		public float GetRadius() => m_Radius;
		public void SetRadius(float radius) => m_Radius = radius;
		public float GetHeight() => m_Height;
		public void SetHeight(float height) => m_Height = height;
		public Def.LivingEntityType GetLEType() => m_LEType;
		public List<AppliedElment> GetCurrentElements() => m_CurrentElements;
		public Collider GetCollider() => m_Collider;
		public string GetName() => m_LEName;
		public IStruc GetCurrentStruc() => m_CurrentStruc;
		public IBlock GetCurrentBlock() => m_CurrentBlock;
		void OnRegen()
		{
			m_Health = Mathf.Min(m_Health + m_HealthRegen, m_MaxHealth);
			m_Soulness = Mathf.Min(m_Soulness + m_SoulnessRegen, m_MaxSoulness);
		}
		public int GetLastHitFrame() => m_LastHitFrame;
		public float GetLastHitTime() => m_LastHitTime;
		public CLivingEntity GetLastHitter() => m_LastHitter;
		public float GetCurrentSoulness() => m_Soulness;
		public float GetMaxSoulness() => m_MaxSoulness;
		public float GetSoulnessRegen() => m_SoulnessRegen;
		public LEStatusBars GetStatusBars() => m_StatusBars;
		private void Update()
		{
			m_RegenerationTimer.Update();
		}
		private void OnGUI()
		{
			if (!Manager.Mgr.DebugStats)
			{
				GUIHeight = 0f;
				return;
			}
			var oldColor = GUI.contentColor;

			var cam = CameraManager.Mgr;

			var wPos = transform.position;
			wPos += new Vector3(0f, m_Height, 0f);
			var sPos = cam.Camera.WorldToScreenPoint(wPos);

			if (sPos.x >= Screen.width || sPos.y >= Screen.height)
			{
				GUIHeight = float.MinValue;
				return; // Fully outside of screen (right || bottom)
			}

			var healthContent	= new GUIContent($"Health   ");
			var curHealthContent = new GUIContent(m_Health.ToString("f2"));
			var totalHealthContent = new GUIContent($"/{m_MaxHealth}");
			var soulnessContent = new GUIContent($"Soulness ");
			var curSoulnessContent = new GUIContent(m_Soulness.ToString("f2"));
			var totalSoulnessContent = new GUIContent($"/{m_MaxSoulness}");

			var healthSize = GUI.skin.label.CalcSize(healthContent);
			var curHealthSize = GUI.skin.label.CalcSize(curHealthContent);
			var totalHealthSize = GUI.skin.label.CalcSize(totalHealthContent);
			var soulnessSize = GUI.skin.label.CalcSize(soulnessContent);
			var curSoulnessSize = GUI.skin.label.CalcSize(curSoulnessContent);
			var totalSoulnessSize = GUI.skin.label.CalcSize(totalSoulnessContent);

			var healthHeight = Mathf.Max(healthSize.y, curHealthSize.y, totalHealthSize.y);
			var soulnessHeight = Mathf.Max(soulnessSize.y, curSoulnessSize.y, totalSoulnessSize.y);

			Color curHealthColor;
			if (m_Health < (m_MaxHealth * 0.33f))
				curHealthColor = Color.red;
			else if (m_Health < (m_MaxHealth * 0.66f))
				curHealthColor = Color.yellow;
			else
				curHealthColor = Color.green;

			Color curSoulnessColor;
			if (m_Soulness < (m_MaxSoulness * 0.33f))
				curSoulnessColor = Color.gray;
			else if (m_Soulness < (m_MaxSoulness * 0.66f))
				curSoulnessColor = Color.cyan;
			else
				curSoulnessColor = Color.blue;

			GUIHeight = healthHeight + soulnessHeight;

			var sPoint = new Vector2(sPos.x, Screen.height - sPos.y);
			var gPoint = GUIUtility.ScreenToGUIPoint(sPoint);

			var healthRect = new Rect(gPoint.x, gPoint.y - GUIHeight, healthSize.x, healthHeight);
			var curHealthRect = new Rect(healthRect.x + healthRect.width, healthRect.y, curHealthSize.x, healthHeight);
			var totalHealthRect = new Rect(curHealthRect.x + curHealthRect.width, healthRect.y, totalHealthSize.x, healthHeight);

			var soulnessRect = new Rect(gPoint.x, healthRect.y + healthHeight, soulnessSize.x, soulnessHeight);
			var curSoulnessRect = new Rect(soulnessRect.x + soulnessRect.width, soulnessRect.y, curSoulnessSize.x, soulnessHeight);
			var totalSoulnessRect = new Rect(curSoulnessRect.x + curSoulnessRect.width, soulnessRect.y, totalSoulnessSize.x, soulnessHeight);

			GUI.contentColor = Color.white;
			GUI.Label(healthRect, healthContent);
			GUI.contentColor = curHealthColor;
			GUI.Label(curHealthRect, curHealthContent);
			GUI.contentColor = Color.white;
			GUI.Label(totalHealthRect, totalHealthContent);
			GUI.Label(soulnessRect, soulnessContent);
			GUI.contentColor = curSoulnessColor;
			GUI.Label(curSoulnessRect, curSoulnessContent);
			GUI.contentColor = Color.white;
			GUI.Label(totalSoulnessRect, totalSoulnessContent);

			GUI.contentColor = oldColor;
		}
		private void OnEnable()
		{
			if (m_StatusBars != null) m_StatusBars.gameObject.SetActive(true);
		}
		private void OnDisable()
		{
			if (m_StatusBars != null) m_StatusBars.gameObject.SetActive(false);
		}
		private void OnDestroy()
		{
			if(m_StatusBars != null)
			{
				GameUtils.DeleteGameobject(m_StatusBars.gameObject);
			}
		}

		public delegate void CastDamageFX(Vector3 pos, Vector3 dir);
		public delegate void StrucChangeCB(IStruc oStruc, CStruc nStruc);
		public delegate void ReceiveDamageCB(CLivingEntity caster, CLivingEntity receiver, Def.DamageType type, float damageAmout);
		public delegate void ReceiveHealingCB(CLivingEntity caster, CLivingEntity receiver, float amount);
		public delegate void ReceiveSoulnessCB(CLivingEntity caster, CLivingEntity receiver, float amount);
		public delegate void ReduceSoulnessCB(CLivingEntity caster, CLivingEntity receiver, float amount);
		public delegate void ReceiveElementCB(CLivingEntity receiver, Def.ElementType type, float damage);
		public delegate void EntityDeathCB(CLivingEntity entity);
	}
}