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
	public abstract class IOnHit
	{
		public abstract Def.OnHitType GetOnHitType();
		public string GetName() => GetOnHitType().ToString();
		public abstract float GetChance();
		public abstract void SetChance(float chance);
		public abstract string GetValueString();
		public abstract void FromString(string value);
	}

	public class OnHitDisplacement : IOnHit
	{
		float m_Chance;
		float m_MinForce;
		float m_MaxForce;
		bool m_IgnoreWeight;
		bool m_ExpansiveDisplacement;

		public override Def.OnHitType GetOnHitType() => Def.OnHitType.Displacement;
		public void SetMinForce(float force) => m_MinForce = force;
		public void SetMaxForce(float force) => m_MaxForce = force;
		public void SetIgnoreWeight(bool ignore) => m_IgnoreWeight = ignore;
		public float GetMinForce() => m_MinForce;
		public float GetMaxForce() => m_MaxForce;
		public bool IgnoreWeight() => m_IgnoreWeight;
		public void SetExpansiveDisplacemente(bool expansive) => m_ExpansiveDisplacement = expansive;
		public bool IsExpansiveDisplacement() => m_ExpansiveDisplacement;

		public override void SetChance(float chance) => m_Chance = chance;
		public override float GetChance() => m_Chance;

		public override string GetValueString()
		{
			return m_Chance.ToString() + ',' + m_MinForce.ToString() + ','
				+ m_MaxForce.ToString() + ',' + m_IgnoreWeight.ToString() + ','
				+ m_ExpansiveDisplacement.ToString();
		}
		public override void FromString(string value)
		{
			void checkParse(bool ok)
			{
				if(!ok)
				{
					Debug.LogWarning("Error while parsing !OnHitDisplacement");
				}
			}
			var split = value.Split(',');
			if (split.Length != 5)
				Debug.LogError("Trying to parse OnHitDisplacement, but it didnt have enough spaces.");
			checkParse(float.TryParse(split[0], out m_Chance));
			checkParse(float.TryParse(split[1], out m_MinForce));
			checkParse(float.TryParse(split[2], out m_MaxForce));
			checkParse(bool.TryParse(split[3], out m_IgnoreWeight));
			checkParse(bool.TryParse(split[4], out m_ExpansiveDisplacement));
		}
	}

	public class OnHitStatusEffect : IOnHit
	{
		float m_Chance;
		Def.StatusEffect m_StatusEffect;

		public override Def.OnHitType GetOnHitType() => Def.OnHitType.StatusEffect;

		public void SetStatusEffect(Def.StatusEffect statusEffect) => m_StatusEffect = statusEffect;
		public Def.StatusEffect GetStatusEffect() => m_StatusEffect;

		public override void SetChance(float chance) => m_Chance = chance;
		public override float GetChance() => m_Chance;

		public override string GetValueString()
		{
			return m_Chance.ToString() + ',' + m_StatusEffect.ToString();
		}
		public override void FromString(string value)
		{
			void checkParse(bool ok)
			{
				if (!ok)
				{
					Debug.LogWarning("Error while parsing !OnHitStatusEffect");
				}
			}
			var split = value.Split(',');
			if (split.Length != 2)
				Debug.LogError("Trying to parse OnHitStatusEffect, but it didnt have enough spaces.");
			checkParse(float.TryParse(split[0], out m_Chance));
			checkParse(Enum.TryParse(split[1], out m_StatusEffect));
		}
	}

	public class OnHitDamage : IOnHit
	{
		float m_Chance;
		Def.DamageType m_DamageType;
		float m_DamageAmount;

		public override Def.OnHitType GetOnHitType() => Def.OnHitType.Damage;

		public Def.DamageType GetDamageType() => m_DamageType;
		public float GetDamageAmount() => m_DamageAmount;
		public void SetDamageType(Def.DamageType damageType) => m_DamageType = damageType;
		public void SetDamageAmount(float damageAmount) => m_DamageAmount = damageAmount;

		public override void SetChance(float chance) => m_Chance = chance;
		public override float GetChance() => m_Chance;

		public override string GetValueString()
		{
			return m_Chance.ToString() + ',' + m_DamageType.ToString() + ','
				+ m_DamageAmount.ToString();
		}
		public override void FromString(string value)
		{
			void checkParse(bool ok)
			{
				if (!ok)
				{
					Debug.LogWarning("Error while parsing !OnHitDamage");
				}
			}
			var split = value.Split(',');
			if (split.Length != 3)
				Debug.LogError("Trying to parse OnHitDamage, but it didnt have enough spaces.");
			checkParse(float.TryParse(split[0], out m_Chance));
			checkParse(Enum.TryParse(split[1], out m_DamageType));
			checkParse(float.TryParse(split[2], out m_DamageAmount));
		}
	}
}
