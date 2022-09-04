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
	public class ViewElementSpawnInfo : ViewElementInfo
	{
		public SpawnInfo Spawn;
	}
	public class CViewElementSpawn : CViewElement
	{
		public UnityEngine.UI.Button MonsterButton;

		public UnityEngine.UI.Toggle OneOverProbabilityToggle;
		public CTMPSlider TotalProbabilitySlider;
		public TMPro.TMP_InputField PartialProbabilityIF;
		public GameObject TotalProbabilityGO;
		public GameObject PartialProbabilityGO;

		public TMPro.TMP_InputField MinAmountIF;
		public TMPro.TMP_InputField MaxAmountIF;
		public TMPro.TMP_InputField MinRangeIF;
		public TMPro.TMP_InputField MaxRangeIF;

		public CImageSelectorUI ImageSelector;

		SpawnInfo m_Info;
		bool m_ValueLock;

		public SpawnInfo GetSpawnInfo() => m_Info;
		public override void _OnAwake()
		{
			base._OnAwake();
			if (MonsterButton != null)
				MonsterButton.onClick.AddListener(OnMonsterButton);
			if (OneOverProbabilityToggle != null)
				OneOverProbabilityToggle.onValueChanged.AddListener(OnOneOverProbabiltyToggle);
			if (MinAmountIF != null)
				MinAmountIF.onEndEdit.AddListener(OnMinAmountIF);
			if (MaxAmountIF != null)
				MaxAmountIF.onEndEdit.AddListener(OnMaxAmountIF);
			if (MinRangeIF != null)
				MinRangeIF.onEndEdit.AddListener(OnMinRangeIF);
			if (MaxRangeIF != null)
				MaxRangeIF.onEndEdit.AddListener(OnMaxRangeIF);
			if (TotalProbabilitySlider != null)
				TotalProbabilitySlider.SetCallback(OnTotalProbabilty);
		}
		public override void ElementInit(ViewElementInfo info, CView view)
		{
			base.ElementInit(info, view);

			var vei = info as ViewElementSpawnInfo;

			m_ValueLock = true;
			m_Info = vei.Spawn;
			if(m_Info.IsPartialProbability())
			{
				OneOverProbabilityToggle.isOn = true;
				PartialProbabilityIF.text = m_Info.GetPartialProbability().ToString();
			}
			else
			{
				OneOverProbabilityToggle.isOn = false;
				TotalProbabilitySlider.SetValue(m_Info.GetTotalProbability() * 0.01f);
			}
			MinAmountIF.text = m_Info.GetMinAmount().ToString();
			MaxAmountIF.text = m_Info.GetMaxAmount().ToString();
			MinRangeIF.text = m_Info.GetMinRange().ToString();
			MaxRangeIF.text = m_Info.GetMaxRange().ToString();

			m_ValueLock = false;
		}
		public void OnTotalProbabilty(float value)
		{
			if (m_ValueLock)
				return;

			m_Info.SetTotalProbability((ushort)Mathf.FloorToInt(value * 100f));
		}
		public void OnMonsterButton()
		{
			ImageSelector.gameObject.SetActive(true);
			ImageSelector.Init(Monsters.UIMonsters, false, OnMonsterButtonEnd, Def.ImageSelectorPosition.Center);
		}
		public void OnMonsterButtonEnd()
		{
			var selectList = ImageSelector.GetSelected();
			if (selectList.Count < 1)
				return;
			var selection = selectList[0];
			if(Monsters.FamilyDict.TryGetValue(selection, out int familyIdx))
			{
				if (ElementImage != null)
					ElementImage.sprite = Monsters.UIMonsters[familyIdx].Image;
				m_Info.SetMonsterFamily(selection);
			}
		}
		public void OnOneOverProbabiltyToggle(bool value)
		{
			if (m_ValueLock)
				return;

			PartialProbabilityGO.SetActive(value);
			TotalProbabilityGO.SetActive(!value);
			if (value)
			{
				m_Info.SetPartialProbability(100);
				m_ValueLock = true;
				PartialProbabilityIF.text = m_Info.GetPartialProbability().ToString();
				m_ValueLock = false;
			}
			else
			{
				m_Info.SetTotalProbability(0);
				m_ValueLock = true;
				TotalProbabilitySlider.SetValue(0f);
				m_ValueLock = false;
			}
		}
		public void OnMinAmountIF(string sValue)
		{
			if (m_ValueLock)
				return;

			if(!int.TryParse(sValue, out int value))
			{
				m_ValueLock = true;
				MinAmountIF.text = m_Info.GetMinAmount().ToString();
				m_ValueLock = false;
				return;
			}
			int cvalue = value;
			if(value < 0)
			{
				cvalue = 0;
			}
			if(value > m_Info.GetMaxAmount())
			{
				cvalue = m_Info.GetMaxAmount();
			}
			m_ValueLock = true;
			m_Info.SetMinAmount(cvalue);
			MinAmountIF.text = m_Info.GetMinAmount().ToString();
			m_ValueLock = false;
		}
		public void OnMaxAmountIF(string sValue)
		{
			if (m_ValueLock)
				return;

			if (!int.TryParse(sValue, out int value))
			{
				m_ValueLock = true;
				MaxAmountIF.text = m_Info.GetMaxAmount().ToString();
				m_ValueLock = false;
				return;
			}
			int cvalue = value;
			if (value < 0)
			{
				cvalue = 0;
			}
			if (value < m_Info.GetMinAmount())
			{
				cvalue = m_Info.GetMinAmount();
			}
			m_ValueLock = true;
			m_Info.SetMaxAmount(cvalue);
			MaxAmountIF.text = m_Info.GetMaxAmount().ToString();
			m_ValueLock = false;
		}
		public void OnMinRangeIF(string sValue)
		{
			if (m_ValueLock)
				return;

			if (!int.TryParse(sValue, out int value))
			{
				m_ValueLock = true;
				MinRangeIF.text = m_Info.GetMinRange().ToString();
				m_ValueLock = false;
				return;
			}
			if (value < 0)
			{
				value = 0;
			}
			if (value > m_Info.GetMaxRange())
			{
				value = m_Info.GetMaxRange();
			}
			m_ValueLock = true;
			m_Info.SetMinRange(value);
			MinRangeIF.text = m_Info.GetMinRange().ToString();
			m_ValueLock = false;
		}
		public void OnMaxRangeIF(string sValue)
		{
			if (m_ValueLock)
				return;

			if (!int.TryParse(sValue, out int value))
			{
				m_ValueLock = true;
				MaxRangeIF.text = m_Info.GetMaxRange().ToString();
				m_ValueLock = false;
				return;
			}
			if (value < 0)
			{
				value = 0;
			}
			if (value < m_Info.GetMinRange())
			{
				value = m_Info.GetMinRange();
			}
			m_ValueLock = true;
			m_Info.SetMaxRange(value);
			MaxRangeIF.text = m_Info.GetMaxRange().ToString();
			m_ValueLock = false;
		}
	}
}
