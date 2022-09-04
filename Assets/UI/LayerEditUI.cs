/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI
{
	public class LayerEditUI : MonoBehaviour
	{
		LayerInfo[] m_TempLayers;
		CStrucEdit m_Struc;
		int m_CurrentLayer;
		Action m_OnEditEnd;
		bool m_ValueLock = false;

		public UnityEngine.UI.Dropdown LayerDropdown;
		public UnityEngine.UI.Button ResetLayerBttn;
		public UnityEngine.UI.Button CopyFromBttn;
		public UnityEngine.UI.Button CrossBttn;

		public UnityEngine.UI.Toggle LinkedLayerTggl;
		public CLinkedLayer LinkedLayer;

		public UnityEngine.UI.Dropdown LayerTypeDropdown;
		public UnityEngine.UI.Dropdown RotationDropdown;
		//public CSlider BlockHeightSlider;
		public CSlider MicroHeightMinSlider;
		public CSlider MicroHeightMaxSlider;
		public CSlider BlockLengthMinSlider;
		public CSlider BlockLengthMaxSlider;
		public CImageView MaterialFamiliesView;
		public CSlider SemiVoidChanceSlider;
		public CSlider WideBlockChanceSlider;
		public CSlider StairBlockChanceSlider;
		public CSlider RampChanceSlider;
		public CImageView PropFamiliesView;
		public CSlider PropChanceSlider;
		public CSlider PropSafetyDistanceSlider;
		public CImageView MonsterFamiliesView;
		public CSlider MonsterChanceSlider;
		public UnityEngine.UI.Toggle SpawnZoneMonstersTggl;
		public UnityEngine.UI.Toggle LayerMonsterRespawnTggl;

		public LayerInfo GetCurrentEditingLayer() => m_TempLayers[m_CurrentLayer-1];
		public LayerInfo[] GetLayers() => m_TempLayers;

		private void Awake()
		{
			m_TempLayers = new LayerInfo[Def.MaxLayerSlots];
			for (int i = 0; i < Def.MaxLayerSlots; ++i)
			{
				var layer = LayerInfo.GetDefaultLayer();
				layer.Slot = i + 1;
				layer.LayerType = (i > (int)Def.BiomeLayerType.OTHER) ? Def.BiomeLayerType.OTHER : (Def.BiomeLayerType)i;
				m_TempLayers[i] = layer;
			}
			LayerDropdown.onValueChanged.AddListener(OnLayerChange);
			ResetLayerBttn.onClick.AddListener(OnResetLayer);
			CopyFromBttn.onClick.AddListener(OnCopyLayer);
			CrossBttn.onClick.AddListener(OnCross);
			LinkedLayerTggl.onValueChanged.AddListener(OnLinkedLayer);
			RotationDropdown.onValueChanged.AddListener(OnRotationChange);
			LayerTypeDropdown.onValueChanged.AddListener(OnLayerTypeChange);

			//BlockHeightSlider.SetMinValue(0f);
			//BlockHeightSlider.SetMaxValue(16f);
			//BlockHeightSlider.SetCallback(OnBlockHeightChange, OnBlockHeightTextChange);
			//BlockHeightSlider.SetValue(8f);

			MicroHeightMinSlider.SetMinValue(-5f);
			MicroHeightMinSlider.SetMaxValue(5f);
			MicroHeightMinSlider.SetValue(0f);
			MicroHeightMinSlider.SetCallback(OnMicroHeightMinChange, OnMicroHeightMinTextChange);

			MicroHeightMaxSlider.SetMinValue(-5f);
			MicroHeightMaxSlider.SetMaxValue(5f);
			MicroHeightMaxSlider.SetValue(0f);
			MicroHeightMaxSlider.SetCallback(OnMicroHeightMaxChange, OnMicroHeightMaxTextChange);

			BlockLengthMinSlider.SetMinValue(0f);
			BlockLengthMinSlider.SetMaxValue(6f);
			BlockLengthMinSlider.SetCallback(OnLengthMinChange, OnLengthMinTextChange);
			BlockLengthMinSlider.SetValue(1f);

			BlockLengthMaxSlider.SetMinValue(0f);
			BlockLengthMaxSlider.SetMaxValue(6f);
			BlockLengthMaxSlider.SetCallback(OnLengthMaxChange, OnLengthMaxTextChange);
			BlockLengthMaxSlider.SetValue(1f);

			SemiVoidChanceSlider.SetMinValue(0f);
			SemiVoidChanceSlider.SetMaxValue(100f);
			SemiVoidChanceSlider.SetValue(0f);
			SemiVoidChanceSlider.SetCallback(OnSemiVoidChange);

			WideBlockChanceSlider.SetMinValue(0f);
			WideBlockChanceSlider.SetMaxValue(100f);
			WideBlockChanceSlider.SetValue(25f);
			WideBlockChanceSlider.SetCallback(OnWideBlockChange);

			StairBlockChanceSlider.SetMinValue(0f);
			StairBlockChanceSlider.SetMaxValue(100f);
			StairBlockChanceSlider.SetValue(50f);
			StairBlockChanceSlider.SetCallback(OnStairBlockChange);

			RampChanceSlider.SetMinValue(0f);
			RampChanceSlider.SetMaxValue(100f);
			RampChanceSlider.SetValue(50f);
			RampChanceSlider.SetCallback(OnRampBlockChange);

			PropChanceSlider.SetMinValue(0f);
			PropChanceSlider.SetMaxValue(100f);
			PropChanceSlider.SetValue(0f);
			PropChanceSlider.SetCallback(OnPropChanceChange);

			PropSafetyDistanceSlider.SetMinValue(0f);
			PropSafetyDistanceSlider.SetMaxValue(8f);
			PropSafetyDistanceSlider.SetCallback(OnPropSafetyDistanceChange, OnPropSafetyDistanceTextChange);
			PropSafetyDistanceSlider.SetValue(0f);

			MonsterChanceSlider.SetMinValue(0f);
			MonsterChanceSlider.SetMaxValue(100f);
			MonsterChanceSlider.SetValue(0f);
			MonsterChanceSlider.SetCallback(OnMonsterChanceChange);

			SpawnZoneMonstersTggl.onValueChanged.AddListener(OnSpawnZoneMonster);
			LayerMonsterRespawnTggl.onValueChanged.AddListener(OnLayerMonsterRespawn);
		}

		public void OnRotationChange(int rot)
		{
			var layer = GetCurrentEditingLayer();
			layer.Rotation = (Def.RotationState)rot;
		}
		public void OnLayerChange(int nLayer)
		{
			m_CurrentLayer = nLayer + 1;
			var layer = GetCurrentEditingLayer();
			LinkedLayerTggl.isOn = layer.IsLinkedLayer;
			LinkedLayer.OnLayerChange();
			LayerTypeDropdown.value = (int)layer.LayerType;
			OnLayerTypeChange((int)layer.LayerType);
			RotationDropdown.value = (int)layer.Rotation;
			//OnBlockHeightTextChange(layer.BlockHeight.ToString());
			OnMicroHeightMinTextChange(layer.MicroHeightMin.ToString());
			OnMicroHeightMaxTextChange(layer.MicroHeightMax.ToString());
			OnLengthMinTextChange(layer.BlockLengthMin.ToString());
			OnLengthMaxTextChange(layer.BlockLengthMax.ToString());
			SemiVoidChanceSlider.SetValue(layer.SemiVoidChance * 0.01f);
			WideBlockChanceSlider.SetValue(layer.WideBlockChance * 0.01f);
			StairBlockChanceSlider.SetValue(layer.StairBlockChance * 0.01f);
			RampChanceSlider.SetValue(layer.RampBlockChance * 0.01f);
			MaterialFamiliesView.OnLayerChange();
			PropChanceSlider.SetValue(layer.PropGeneralChance * 0.01f);
			OnPropSafetyDistanceTextChange(layer.PropSafetyDistance.ToString());
			PropFamiliesView.OnLayerChange();
			MonsterChanceSlider.SetValue(layer.MonsterGeneralChance * 0.01f);
			MonsterFamiliesView.OnLayerChange();
			SpawnZoneMonstersTggl.isOn = layer.SpawnZoneMonsters;
			LayerMonsterRespawnTggl.isOn = layer.LayerMonstersRespawn;
		}
		public void OnResetLayer()
		{
			m_TempLayers[m_CurrentLayer - 1] = LayerInfo.GetDefaultLayer();
			var layer = GetCurrentEditingLayer();
			layer.Slot = m_CurrentLayer;
			OnLayerChange(m_CurrentLayer - 1);
		}
		public void OnCopyLayer()
		{
			Debug.Log("Layer copy Not done!");
		}
		public void OnCopyLayerEnd()
		{

		}
		public void OnCross()
		{
			m_OnEditEnd();
		}
		public void OnLinkedLayer(bool enabled)
		{
			if(enabled)
			{
				LinkedLayer.enabled = true;
				LayerTypeDropdown.interactable = false;
				RotationDropdown.interactable = false;
				//BlockHeightSlider.enabled = false;
				MicroHeightMinSlider.enabled = false;
				MicroHeightMaxSlider.enabled = false;
				BlockLengthMinSlider.enabled = false;
				BlockLengthMaxSlider.enabled = false;
				MaterialFamiliesView.enabled = false;
				SemiVoidChanceSlider.enabled = false;
				WideBlockChanceSlider.enabled = false;
				StairBlockChanceSlider.enabled = false;
				PropFamiliesView.enabled = false;
				PropChanceSlider.enabled = false;
				PropSafetyDistanceSlider.enabled = false;
				MonsterFamiliesView.enabled = false;
				MonsterChanceSlider.enabled = false;
				SpawnZoneMonstersTggl.interactable = false;
				LayerMonsterRespawnTggl.interactable = false;
			}
			else
			{
				LinkedLayer.enabled = false;
				LayerTypeDropdown.interactable = true;
				RotationDropdown.interactable = true;
				//BlockHeightSlider.enabled = true;
				MicroHeightMinSlider.enabled = true;
				MicroHeightMaxSlider.enabled = true;
				BlockLengthMinSlider.enabled = true;
				BlockLengthMaxSlider.enabled = true;
				MaterialFamiliesView.enabled = true;
				SemiVoidChanceSlider.enabled = true;
				WideBlockChanceSlider.enabled = true;
				StairBlockChanceSlider.enabled = true;
				PropFamiliesView.enabled = true;
				PropChanceSlider.enabled = true;
				PropSafetyDistanceSlider.enabled = true;
				MonsterFamiliesView.enabled = true;
				MonsterChanceSlider.enabled = true;
				SpawnZoneMonstersTggl.interactable = true;
				LayerMonsterRespawnTggl.interactable = true;
			}
		}
		public void OnLayerTypeChange(int nType)
		{
			var layer = GetCurrentEditingLayer();
			layer.LayerType = (Def.BiomeLayerType)nType;
			bool interactable = layer.LayerType == Def.BiomeLayerType.OTHER;
			MicroHeightMaxSlider.SetInteractable(interactable);
			MicroHeightMinSlider.SetInteractable(interactable);
			SemiVoidChanceSlider.SetInteractable(interactable);
			WideBlockChanceSlider.SetInteractable(interactable);
			StairBlockChanceSlider.SetInteractable(interactable);
			RampChanceSlider.SetInteractable(interactable);
			PropChanceSlider.SetInteractable(interactable);
			PropSafetyDistanceSlider.SetInteractable(interactable);
			MonsterChanceSlider.SetInteractable(interactable);

			MaterialFamiliesView.SetInteractable(interactable);
			PropFamiliesView.SetInteractable(interactable);
			MonsterFamiliesView.SetInteractable(interactable);

			bool isFullVoid = layer.LayerType == Def.BiomeLayerType.FULLVOID;
			RotationDropdown.interactable = !isFullVoid;
			//BlockHeightSlider.SetInteractable(!isFullVoid);
			BlockLengthMaxSlider.SetInteractable(!isFullVoid);
			BlockLengthMinSlider.SetInteractable(!isFullVoid);
			SpawnZoneMonstersTggl.interactable = !isFullVoid;
			LayerMonsterRespawnTggl.interactable = !isFullVoid;
		}
		//public void OnBlockHeightChange(int _id, float value)
		//{
		//	if (m_ValueLock)
		//		return;
		//	var layer = GetCurrentEditingLayer();
		//	float nVal = -4f + value * 0.5f;
		//	m_ValueLock = true;
		//	BlockHeightSlider.Input.text = nVal.ToString();
		//	m_ValueLock = false;
		//	layer.BlockHeight = nVal;
		//}
		//public void OnBlockHeightTextChange(string value)
		//{
		//	if (m_ValueLock)
		//		return;
		//	bool parseOK = float.TryParse(value, out float nVal);
		//	if (!parseOK)
		//		return;
		//	var layer = GetCurrentEditingLayer();
		//	if (nVal < -4f)
		//	{
		//		nVal = -4f;
		//	}
		//	else if (nVal > 4f)
		//	{
		//		nVal = 4f;
		//	}
		//	else
		//	{
		//		float decimals = nVal - Mathf.Floor(nVal);
		//		if (decimals >= 0.66)
		//			nVal = (nVal - decimals) + 1f;
		//		else if (decimals < 0.66 && decimals > 0.33)
		//			nVal = (nVal - decimals) + 0.5f;
		//		else
		//			nVal -= decimals;
		//	}
		//	layer.BlockHeight = nVal;
		//	float sVal = nVal * 2f + 8f;
		//	m_ValueLock = true;
		//	BlockHeightSlider.SetValue(sVal);
		//	BlockHeightSlider.Input.text = nVal.ToString();
		//	m_ValueLock = false;
		//}
		public void OnMicroHeightMinChange(int _id, float value) // -0.25 -0.20 ... 0 0.05 ... 0.20 0.25
		{
			if (m_ValueLock)
				return;
			var layer = GetCurrentEditingLayer();
			float nVal = (0.05f * value);
			if(nVal > layer.MicroHeightMax)
			{
				nVal = layer.MicroHeightMax;
			}
			layer.MicroHeightMin = nVal;
			m_ValueLock = true;
			MicroHeightMinSlider.Input.text = nVal.ToString();
			MicroHeightMinSlider.SetValue(nVal / 0.05f);
			m_ValueLock = false;
		}
		public void OnMicroHeightMinTextChange(string value)
		{
			if (m_ValueLock)
				return;
			bool parseOK = float.TryParse(value, out float nVal);
			if (!parseOK)
				return;

			var layer = GetCurrentEditingLayer();
			if(nVal >= 0.25f)
			{
				nVal = 0.25f;
			}
			else if(nVal <= -0.25f)
			{
				nVal = -0.25f;
			}                
			else
			{
				float decimals = (nVal * 10f) - Mathf.Floor(nVal * 10f);
				if (decimals >= 0.66)
					nVal = (nVal - decimals * 0.1f) + 0.1f;
				else if (decimals < 0.66 && decimals > 0.33)
					nVal = (nVal - decimals * 0.1f) + 0.05f;
				else
					nVal -= decimals * 0.1f;
			}
			if(nVal > layer.MicroHeightMax)
			{
				nVal = layer.MicroHeightMax;
			}
			layer.MicroHeightMin = nVal;
			m_ValueLock = true;
			MicroHeightMinSlider.Input.text = nVal.ToString();
			MicroHeightMinSlider.SetValue(nVal / 0.05f);
			m_ValueLock = false;
		}
		public void OnMicroHeightMaxChange(int _id, float value)
		{
			if (m_ValueLock)
				return;
			var layer = GetCurrentEditingLayer();
			float nVal = (0.05f * value);
			if (nVal < layer.MicroHeightMin)
			{
				nVal = layer.MicroHeightMin;
			}
			layer.MicroHeightMax = nVal;
			m_ValueLock = true;
			MicroHeightMaxSlider.Input.text = nVal.ToString();
			MicroHeightMaxSlider.SetValue(nVal / 0.05f);
			m_ValueLock = false;
		}
		public void OnMicroHeightMaxTextChange(string value)
		{
			if (m_ValueLock)
				return;
			bool parseOK = float.TryParse(value, out float nVal);
			if (!parseOK)
				return;

			var layer = GetCurrentEditingLayer();
			if (nVal >= 0.25f)
			{
				nVal = 0.25f;
			}
			else if (nVal <= -0.25f)
			{
				nVal = -0.25f;
			}
			else
			{
				float decimals = (nVal * 10f) - Mathf.Floor(nVal * 10f);
				if (decimals >= 0.66)
					nVal = (nVal - decimals * 0.1f) + 0.1f;
				else if (decimals < 0.66 && decimals > 0.33)
					nVal = (nVal - decimals * 0.1f) + 0.05f;
				else
					nVal -= decimals * 0.1f;
			}
			if (nVal < layer.MicroHeightMin)
			{
				nVal = layer.MicroHeightMin;
			}
			layer.MicroHeightMax = nVal;
			m_ValueLock = true;
			MicroHeightMaxSlider.Input.text = nVal.ToString();
			MicroHeightMaxSlider.SetValue(nVal / 0.05f);
			m_ValueLock = false;
		}
		public void OnLengthMinChange(int _id, float value) // 0.5 1 1.5 2 2.5 3 3.4
		{
			if (m_ValueLock)
				return;
			var layer = GetCurrentEditingLayer();
			float nVal;

			if (value == 6f)
			{
				nVal = 3.4f;
			}
			else
			{
				nVal = (value + 1) * 0.5f;
			}
			if(layer.BlockLengthMax < nVal)
			{
				nVal = layer.BlockLengthMax;
				value = BlockLengthMaxSlider.Slider.value;
			}
			layer.BlockLengthMin = nVal;
			m_ValueLock = true;
			BlockLengthMinSlider.Input.text = nVal.ToString();
			BlockLengthMinSlider.Slider.value = value;
			m_ValueLock = false;
		}
		public void OnLengthMinTextChange(string value)
		{
			if (m_ValueLock)
				return;
			bool parseOK = float.TryParse(value, out float nVal);
			if (!parseOK)
				return;

			var layer = GetCurrentEditingLayer();
			if(nVal >= 3.2f)
			{
				nVal = 3.4f;
			}
			else if(nVal < 0.5f)
			{
				nVal = 0.5f;
			}
			else
			{
				float decimals = nVal - Mathf.Floor(nVal);
				if (decimals >= 0.66f)
					nVal = (nVal - decimals) + 1f;
				else if (decimals < 0.66f && decimals > 0.33f)
					nVal = (nVal - decimals) + 0.5f;
				else
					nVal -= decimals;
			}
			if(layer.BlockLengthMax < nVal)
			{
				nVal = layer.BlockLengthMax;
			}
			layer.BlockLengthMin = nVal;
			float sVal = nVal == 3.4f ? 6f : (nVal * 2f - 1f);
			m_ValueLock = true;
			BlockLengthMinSlider.Slider.value = sVal;
			BlockLengthMinSlider.Input.text = nVal.ToString();
			m_ValueLock = false;
		}
		public void OnLengthMaxChange(int _id, float value)
		{
			if (m_ValueLock)
				return;
			var layer = GetCurrentEditingLayer();
			float nVal;

			if (value == 6f)
			{
				nVal = 3.4f;
			}
			else
			{
				nVal = (value + 1) * 0.5f;
			}
			if (layer.BlockLengthMin > nVal)
			{
				nVal = layer.BlockLengthMin;
				value = BlockLengthMinSlider.Slider.value;
			}
			layer.BlockLengthMax = nVal;
			m_ValueLock = true;
			BlockLengthMaxSlider.SetValue(value);
			BlockLengthMaxSlider.Input.text = nVal.ToString();
			//BlockLengthMaxSlider.Slider.value = value;
			m_ValueLock = false;
		}
		public void OnLengthMaxTextChange(string value)
		{
			if (m_ValueLock)
				return;
			bool parseOK = float.TryParse(value, out float nVal);
			if (!parseOK)
				return;

			var layer = GetCurrentEditingLayer();
			if (nVal >= 3.2f)
			{
				nVal = 3.4f;
			}
			else if (nVal < 0.5f)
			{
				nVal = 0.5f;
			}
			else
			{
				float decimals = nVal - Mathf.Floor(nVal);
				if (decimals >= 0.66f)
					nVal = (nVal - decimals) + 1f;
				else if (decimals < 0.66f && decimals > 0.33f)
					nVal = (nVal - decimals) + 0.5f;
				else
					nVal -= decimals;
			}
			if (layer.BlockLengthMin > nVal)
			{
				nVal = layer.BlockLengthMin;
			}
			layer.BlockLengthMax = nVal;
			float sVal = nVal == 3.4f ? 6f : (nVal * 2f - 1f);
			m_ValueLock = true;
			//BlockLengthMaxSlider.Slider.value = sVal;
			BlockLengthMaxSlider.SetValue(sVal);
			BlockLengthMaxSlider.Input.text = nVal.ToString();
			m_ValueLock = false;
		}
		public void OnSemiVoidChange(int _id, float value)
		{
			var layer = GetCurrentEditingLayer();
			layer.SemiVoidChance = (ushort)Mathf.FloorToInt(value * 100f);
		}
		public void OnWideBlockChange(int _id, float value)
		{
			var layer = GetCurrentEditingLayer();
			layer.WideBlockChance = (ushort)Mathf.FloorToInt(value * 100f);
		}
		public void OnStairBlockChange(int _id, float value)
		{
			var layer = GetCurrentEditingLayer();
			layer.StairBlockChance = (ushort)Mathf.FloorToInt(value * 100f);
		}
		public void OnRampBlockChange(int _id, float value)
		{
			var layer = GetCurrentEditingLayer();
			layer.RampBlockChance = (ushort)Mathf.FloorToInt(value * 100f);
		}
		public void OnPropChanceChange(int _id, float value)
		{
			var layer = GetCurrentEditingLayer();
			layer.PropGeneralChance = (ushort)Mathf.FloorToInt(value * 100f);
		}
		public void OnPropSafetyDistanceChange(int _id, float value) // 0 0.5 1 1.5 2 2.5 3 3.5 4
		{
			if (m_ValueLock)
				return;
			var layer = GetCurrentEditingLayer();
			var nVal = value * 0.5f;
			layer.PropSafetyDistance = nVal;
			m_ValueLock = true;
			PropSafetyDistanceSlider.Input.text = nVal.ToString();
			m_ValueLock = false;
		}
		public void OnPropSafetyDistanceTextChange(string value)
		{
			if (m_ValueLock)
				return;
			bool parseOK = float.TryParse(value, out float nVal);
			if (!parseOK)
				return;

			if(nVal > 3.66f)
			{
				nVal = 4f;
			}
			else if(nVal < 0f)
			{
				nVal = 0f;
			}
			else
			{
				float decimals = nVal - Mathf.Floor(nVal);
				if (decimals > 0.66f)
				{
					nVal = (nVal - decimals) + 1f;
				}
				else if (decimals > 0.33f)
				{
					nVal = (nVal - decimals) + 0.5f;
				}
				else
				{
					nVal = nVal - decimals;
				}
			}
			var layer = GetCurrentEditingLayer();
			layer.PropSafetyDistance = nVal;
			
			float sVal = nVal * 2f;

			m_ValueLock = true;
			PropSafetyDistanceSlider.Slider.value = sVal;
			PropSafetyDistanceSlider.Input.text = nVal.ToString();
			m_ValueLock = false;
		}
		public void OnMonsterChanceChange(int _id, float value)
		{
			var layer = GetCurrentEditingLayer();
			layer.MonsterGeneralChance = (ushort)Mathf.FloorToInt(value * 100f);
		}
		public void OnSpawnZoneMonster(bool enabled)
		{
			var layer = GetCurrentEditingLayer();
			layer.SpawnZoneMonsters = enabled;
		}
		public void OnLayerMonsterRespawn(bool enabled)
		{
			var layer = GetCurrentEditingLayer();
			layer.LayerMonstersRespawn = enabled;
		}
		public void Init(CStrucEdit struc, Action onEditEnd)
		{
			m_Struc = struc;
			m_OnEditEnd = onEditEnd;
			var sLayers = m_Struc.GetLayers();
			for(int i = 0; i < sLayers.Length; ++i)
			{
				var sLayer = sLayers[i];
				var tLayer = m_TempLayers[i];
				tLayer.CopyFrom(sLayer);
			}
			m_CurrentLayer = 1;
			LayerDropdown.value = m_CurrentLayer - 1;
			MaterialFamiliesView.Init();
			PropFamiliesView.Init();
			MonsterFamiliesView.Init();
			OnLayerChange(m_CurrentLayer - 1);
		}

		private void Update()
		{
			if(Input.GetKey(KeyCode.Escape))
			{
				OnCross();
			}
		}
	}
}