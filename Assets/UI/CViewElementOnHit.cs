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

namespace Assets.UI
{
	public class ViewElementOnHitInfo : ViewElementInfo
	{
		public AI.Spells.IOnHit OnHit;
	}
	public class CViewElementOnHit : CViewElement
	{
		public UnityEngine.UI.InputField PctIF;

		public UnityEngine.UI.Text MinDisplacementText;
		public UnityEngine.UI.InputField MinDisplacementIF;
		public UnityEngine.UI.Text MaxDisplacementText;
		public UnityEngine.UI.InputField MaxDisplacementIF;
		public UnityEngine.UI.Text ExpansiveDisplacementText;
		public UnityEngine.UI.Toggle ExpansiveDisplacementToggle;
		public UnityEngine.UI.Text IgnoreWeightText;
		public UnityEngine.UI.Toggle IgnoreWeightToggle;

		public UnityEngine.UI.Dropdown Dropdown;
		public UnityEngine.UI.Text DropdownText;

		public UnityEngine.UI.Text DamageAmountText;
		public UnityEngine.UI.InputField DamageAmountIF;

		public override void _OnAwake()
		{
			base._OnAwake();
			PctIF.onEndEdit.AddListener(OnPCTChange);
			MinDisplacementIF.onEndEdit.AddListener(OnMinDisplacementChange);
			MaxDisplacementIF.onEndEdit.AddListener(OnMaxDisplacementChange);
			ExpansiveDisplacementToggle.onValueChanged.AddListener(OnExpansiveToggleChange);
			IgnoreWeightToggle.onValueChanged.AddListener(OnIgnoreWeightToggleChange);
			Dropdown.onValueChanged.AddListener(OnDropdownChange);
			DamageAmountIF.onEndEdit.AddListener(OnDamageAmountChange);
		}

		void OnPCTChange(string value)
		{
			if (float.TryParse(value, out float r))
				m_OnHit.SetChance(r);
		}
		void OnMinDisplacementChange(string value)
		{
			if(int.TryParse(value, out int res))
			{
				(m_OnHit as AI.Spells.OnHitDisplacement).SetMinForce(res);
			}
		}
		void OnMaxDisplacementChange(string value)
		{
			if (int.TryParse(value, out int res))
			{
				(m_OnHit as AI.Spells.OnHitDisplacement).SetMaxForce(res);
			}
		}
		void OnExpansiveToggleChange(bool value)
		{
			(m_OnHit as AI.Spells.OnHitDisplacement).SetExpansiveDisplacemente(value);
		}
		void OnIgnoreWeightToggleChange(bool value)
		{
			(m_OnHit as AI.Spells.OnHitDisplacement).SetIgnoreWeight(value);
		}
		void OnDropdownChange(int value)
		{
			switch (m_OnHit.GetOnHitType())
			{
				case Def.OnHitType.StatusEffect:
					(m_OnHit as AI.Spells.OnHitStatusEffect).SetStatusEffect((Def.StatusEffect)value);
					break;
				case Def.OnHitType.Damage:
					(m_OnHit as AI.Spells.OnHitDamage).SetDamageType((Def.DamageType)value);
					break;
				default:
					Debug.LogWarning("Unhandled OnHitType: " + m_OnHit.GetOnHitType().ToString());
					break;
			}
		}
		void OnDamageAmountChange(string value)
		{
			if (float.TryParse(value, out float r))
				(m_OnHit as AI.Spells.OnHitDamage).SetDamageAmount(r);
		}

		AI.Spells.IOnHit m_OnHit;
		public AI.Spells.IOnHit GetOnHit() => m_OnHit;

		public override void ElementInit(ViewElementInfo info, CView view)
		{
			base.ElementInit(info, view);

			m_OnHit = (info as ViewElementOnHitInfo).OnHit;

			switch (m_OnHit.GetOnHitType())
			{
				case Def.OnHitType.Displacement:
					PctIF.gameObject.SetActive(true);
					MinDisplacementText.gameObject.SetActive(true);
					MinDisplacementIF.gameObject.SetActive(true);
					MaxDisplacementText.gameObject.SetActive(true);
					MaxDisplacementIF.gameObject.SetActive(true);
					ExpansiveDisplacementText.gameObject.SetActive(true);
					ExpansiveDisplacementToggle.gameObject.SetActive(true);
					IgnoreWeightText.gameObject.SetActive(true);
					IgnoreWeightToggle.gameObject.SetActive(true);
					Dropdown.gameObject.SetActive(false);
					DropdownText.gameObject.SetActive(false);
					DamageAmountIF.gameObject.SetActive(false);
					DamageAmountText.gameObject.SetActive(false);

					{
						var displacement = m_OnHit as AI.Spells.OnHitDisplacement;
						PctIF.text = displacement.GetChance().ToString();
						MinDisplacementIF.text = displacement.GetMinForce().ToString();
						MaxDisplacementIF.text = displacement.GetMaxForce().ToString();
						ExpansiveDisplacementToggle.isOn = displacement.IsExpansiveDisplacement();
						IgnoreWeightToggle.isOn = displacement.IgnoreWeight();
					}
					break;
				case Def.OnHitType.StatusEffect:
					PctIF.gameObject.SetActive(true);
					MinDisplacementText.gameObject.SetActive(false);
					MinDisplacementIF.gameObject.SetActive(false);
					MaxDisplacementText.gameObject.SetActive(false);
					MaxDisplacementIF.gameObject.SetActive(false);
					ExpansiveDisplacementText.gameObject.SetActive(false);
					ExpansiveDisplacementToggle.gameObject.SetActive(false);
					IgnoreWeightText.gameObject.SetActive(false);
					IgnoreWeightToggle.gameObject.SetActive(false);
					Dropdown.gameObject.SetActive(true);
					DropdownText.gameObject.SetActive(true);
					DamageAmountIF.gameObject.SetActive(false);
					DamageAmountText.gameObject.SetActive(false);
					{
						var statusEffect = m_OnHit as AI.Spells.OnHitStatusEffect;
						PctIF.text = statusEffect.GetChance().ToString();
						DropdownText.text = "Status Effect:";
						var names = Enum.GetNames(typeof(Def.StatusEffect));
						int cur = -1;
						var options = new List<UnityEngine.UI.Dropdown.OptionData>(names.Length);
						for(int i = 0; i < names.Length; ++i)
						{
							options.Add(new UnityEngine.UI.Dropdown.OptionData(names[i]));
							if(cur < 0)
							{
								if(names[i] == statusEffect.GetStatusEffect().ToString())
								{
									cur = i;
								}
							}
						}
						if (cur < 0)
							cur = 0;
						Dropdown.ClearOptions();
						Dropdown.AddOptions(options);
						Dropdown.value = cur;
						Dropdown.RefreshShownValue();
					}
					break;
				case Def.OnHitType.Damage:
					PctIF.gameObject.SetActive(true);
					MinDisplacementText.gameObject.SetActive(false);
					MinDisplacementIF.gameObject.SetActive(false);
					MaxDisplacementText.gameObject.SetActive(false);
					MaxDisplacementIF.gameObject.SetActive(false);
					ExpansiveDisplacementText.gameObject.SetActive(false);
					ExpansiveDisplacementToggle.gameObject.SetActive(false);
					IgnoreWeightText.gameObject.SetActive(false);
					IgnoreWeightToggle.gameObject.SetActive(false);
					Dropdown.gameObject.SetActive(true);
					DropdownText.gameObject.SetActive(true);
					DamageAmountIF.gameObject.SetActive(true);
					DamageAmountText.gameObject.SetActive(true);
					{
						var damage = m_OnHit as AI.Spells.OnHitDamage;
						PctIF.text = damage.GetChance().ToString();
						DropdownText.text = "Damage Type:";
						var names = Enum.GetNames(typeof(Def.DamageType));
						int cur = -1;
						var options = new List<UnityEngine.UI.Dropdown.OptionData>(names.Length);
						for (int i = 0; i < names.Length; ++i)
						{
							options.Add(new UnityEngine.UI.Dropdown.OptionData(names[i]));
							if (cur < 0)
							{
								if (names[i] == damage.GetDamageType().ToString())
								{
									cur = i;
								}
							}
						}
						if (cur < 0)
							cur = 0;
						Dropdown.ClearOptions();
						Dropdown.AddOptions(options);
						Dropdown.value = cur;
						Dropdown.RefreshShownValue();
						DamageAmountIF.text = damage.GetDamageAmount().ToString();
					}
					break;
				default:
					Debug.LogWarning("Unhandled OnHitType: " + m_OnHit.GetOnHitType().ToString());
					break;
			}
		}
	}
}
