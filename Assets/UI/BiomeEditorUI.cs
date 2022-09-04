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
	public class BiomeEditorUI : MonoBehaviour
	{
		public UnityEngine.UI.Button CrossButton;
		public TMPro.TMP_InputField NameIF;
		public TMPro.TMP_Dropdown BiomeLayerDropdown;
		public UnityEngine.UI.Button ResetLayerButton;
		public UnityEngine.UI.Button InvalidateLayerButton;
		public CTMPSlider MinMicroHeightSlider;
		public CTMPSlider MaxMicroHeightSlider;
		public CView MaterialFamiliesView;
		public CTMPSlider SemiVoidChanceSlider;
		public CTMPSlider WideBlockChanceSlider;
		public CTMPSlider StairBlockChanceSlider;
		public CTMPSlider StairIsRampChanceSlider;
		public CView PropFamiliesView;
		public CTMPSlider PropChanceSlider;
		public CTMPSlider PropSafetyDistanceSlider;
		public CView MonsterFamiliesView;
		public CTMPSlider MonsterChanceSlider;
		public CBiomeStat[] BiomeStats;
		public TMPro.TMP_InputField BiomeMinDistanceIF;

		public UnityEngine.UI.Button AddBlockMaterialButton;
		public UnityEngine.UI.Button ResetBlockMaterialChancesButton;
		public UnityEngine.UI.Button AddPropButton;
		public UnityEngine.UI.Button ResetPropChancesButton;
		public UnityEngine.UI.Button AddMonsterButton;
		public UnityEngine.UI.Button ResetMonsterChancesButton;

		public CImageSelectorUI SelectorUI;
		List<CImageSelectorUI.ElementInfo> m_AvailableBlockMaterials;
		List<CImageSelectorUI.ElementInfo> m_AvailableProps;
		List<CImageSelectorUI.ElementInfo> m_AvailableMonsters;

		World.Biome m_Biome;
		World.BiomeLayer[] m_TempLayers;
		int m_CurrentLayer;
		Action m_OnEnd;
		bool m_ValueLock;

		public World.BiomeLayer GetCurrentLayer() => m_TempLayers[m_CurrentLayer];
		public World.BiomeLayer[] GetLayers() => m_TempLayers;
		static Action<float>[] m_BiomeFn;
		void SetOnEnd(Action onEnd = null)
		{
			m_OnEnd = onEnd;
			if (m_OnEnd == null)
				m_OnEnd = () => { };
		}
		BiomeEditorUI()
		{
			m_BiomeFn = new Action<float>[Def.BiomeStatCount * 2]
			{
				(float v) => { OnMinBiomeStatChange(Def.BiomeStat.Density,		v); },
				(float v) => { OnMaxBiomeStatChange(Def.BiomeStat.Density,		v); },
				(float v) => { OnMinBiomeStatChange(Def.BiomeStat.Temperature,	v); },
				(float v) => { OnMaxBiomeStatChange(Def.BiomeStat.Temperature,	v); },
				(float v) => { OnMinBiomeStatChange(Def.BiomeStat.Height,		v); },
				(float v) => { OnMaxBiomeStatChange(Def.BiomeStat.Height,		v); },
				(float v) => { OnMinBiomeStatChange(Def.BiomeStat.Soulness,		v); },
				(float v) => { OnMaxBiomeStatChange(Def.BiomeStat.Soulness,		v); },
				(float v) => { OnMinBiomeStatChange(Def.BiomeStat.Wealth,		v); },
				(float v) => { OnMaxBiomeStatChange(Def.BiomeStat.Wealth,		v); }
			};
		}
		private void Awake()
		{
			m_TempLayers = new World.BiomeLayer[Def.BiomeTypeCount];
			m_ValueLock = false;
			m_AvailableBlockMaterials = new List<CImageSelectorUI.ElementInfo>();
			m_AvailableProps = new List<CImageSelectorUI.ElementInfo>();
			m_AvailableMonsters = new List<CImageSelectorUI.ElementInfo>();
			SetOnEnd();

			CrossButton.onClick.AddListener(OnCross);

			NameIF.onEndEdit.AddListener(OnNameChange);
			
			BiomeLayerDropdown.ClearOptions();
			var layerTypesArr = Enum.GetNames(typeof(Def.BiomeLayerType));
			var layerTypes = new List<string>(layerTypesArr);
			layerTypes.Remove(Def.BiomeLayerType.FULLVOID.ToString());
			layerTypes.Remove(Def.BiomeLayerType.OTHER.ToString());
			layerTypes.Remove(Def.BiomeLayerType.COUNT.ToString());

			BiomeMinDistanceIF.onEndEdit.AddListener(OnBiomeMinDistanceChange);

			BiomeLayerDropdown.AddOptions(layerTypes);
			BiomeLayerDropdown.value = 0;
			BiomeLayerDropdown.RefreshShownValue();
			BiomeLayerDropdown.onValueChanged.AddListener(OnLayerChange);

			ResetLayerButton.onClick.AddListener(OnResetLayer);
			InvalidateLayerButton.onClick.AddListener(OnInvalidateLayer);

			MaxMicroHeightSlider.Set(-0.25f, 0.25f, 0.05f, 0f);
			
			MinMicroHeightSlider.Set(-0.25f, 0.25f, 0.05f, 0f);

			SemiVoidChanceSlider.Set(0f, 100f, 1f, 25f);

			WideBlockChanceSlider.Set(0f, 100f, 1f, 25f);

			StairBlockChanceSlider.Set(0f, 100f, 1f, 50f);

			StairIsRampChanceSlider.Set(0f, 100f, 1f, 50f);

			PropChanceSlider.Set(0f, 100f, 1f, 25f);

			PropSafetyDistanceSlider.Set(0f, 8f, 0.5f, 0.5f);

			MonsterChanceSlider.Set(0f, 100f, 1f, 25f);

			MaterialFamiliesView.SetOnElementRemove(OnBlockMaterialRemoved2);
			PropFamiliesView.SetOnElementRemove(OnPropRemoved2);
			MonsterFamiliesView.SetOnElementRemove(OnMonsterRemoved2);

			AddBlockMaterialButton.onClick.AddListener(OnAddBlockMaterial);
			ResetBlockMaterialChancesButton.onClick.AddListener(OnBlockMaterialChanceReset);
			AddPropButton.onClick.AddListener(OnAddProp);
			ResetPropChancesButton.onClick.AddListener(OnPropChanceReset);
			AddMonsterButton.onClick.AddListener(OnAddMonster);
			ResetMonsterChancesButton.onClick.AddListener(OnMonsterChanceReset);

			if (BiomeStats != null && BiomeStats.Length == Def.BiomeStatCount)
			{
				for (int i = 0; i < Def.BiomeStatCount; ++i)
				{
					var cur = BiomeStats[i];
					cur.MinSlider.SetCallback(m_BiomeFn[i * 2]);
					cur.MaxSlider.SetCallback(m_BiomeFn[i * 2 + 1]);
				}
			}
		}
		void CopyBiomeLayers()
		{
			var layers = m_Biome.GetLayers();
			for(int i = 0; i < layers.Length; ++i)
			{
				m_TempLayers[i] = layers[i].GetACopy();
			}
		}
		void CopyBiomeStat()
		{
			m_ValueLock = true;
			for(int i = 0; i < Def.BiomeStatCount; ++i)
			{
				m_Biome.GetBiomeStat((Def.BiomeStat)i, out int min, out int max);
				var cur = BiomeStats[i];
				cur.SetMaxValue(max);
				cur.SetMinValue(min);
			}
			m_ValueLock = false;
		}
		public void Init(World.Biome biome, Action onEnd)
		{
			m_Biome = biome;
			SetOnEnd(onEnd);
			MaxMicroHeightSlider.SetCallback(OnMaxMicroHeightChange);
			MinMicroHeightSlider.SetCallback(OnMinMicroHeightChange);

			var biomeIE = BiomeLoader.Biomes[m_Biome.IDXIE];
			m_ValueLock = true;
			NameIF.text = biomeIE.GetName();
			BiomeMinDistanceIF.text = biome.GetMinDistance().ToString();
			m_ValueLock = false;
			CopyBiomeStat();
			CopyBiomeLayers();
			m_CurrentLayer = -1;
			BiomeLayerDropdown.value = 0;
			BiomeLayerDropdown.RefreshShownValue();
			OnLayerChange(0);
		}
		void OnNameChange(string name)
		{
			if (m_ValueLock)
				return;
			var biomeIE = BiomeLoader.Biomes[m_Biome.IDXIE];
			var oName = biomeIE.GetName();
			if (oName == name)
				return;

			if(BiomeLoader.Dict.ContainsKey(name))
			{
				m_ValueLock = true;
				NameIF.text = oName;
				m_ValueLock = false;
				return;
			}

			biomeIE.SetName(name);
			if (BiomeLoader.Dict.ContainsKey(oName))
				BiomeLoader.Dict.Remove(oName);
			BiomeLoader.Dict.Add(name, m_Biome.IDXIE);
		}
		void OnLayerChange(int layerIdx)
		{
			if (m_CurrentLayer == layerIdx)
				return;

			if(!CanExitLayer())
			{
				BiomeLayerDropdown.value = m_CurrentLayer;
				return;
			}

			StoreLayerInfo();

			m_AvailableBlockMaterials.Clear();
			m_AvailableBlockMaterials.AddRange(BlockMaterial.UIMaterialFamilies);
			m_AvailableMonsters.Clear();
			m_AvailableMonsters.AddRange(Monsters.UIMonsters);
			m_AvailableProps.Clear();
			m_AvailableProps.AddRange(Props.UIProps);

			m_CurrentLayer = layerIdx;
			var layer = GetCurrentLayer();
			MaxMicroHeightSlider.SetValue(layer.MicroHeightMax);
			MinMicroHeightSlider.SetValue(layer.MicroHeightMin);
			SemiVoidChanceSlider.SetValue(layer.SemiVoidChance * 0.01f);
			WideBlockChanceSlider.SetValue(layer.WideBlockChance * 0.01f);
			StairBlockChanceSlider.SetValue(layer.StairBlockChance * 0.01f);
			StairIsRampChanceSlider.SetValue(layer.RampBlockChance * 0.01f);
			PropChanceSlider.SetValue(layer.PropGeneralChance * 0.01f);
			PropSafetyDistanceSlider.SetValue(layer.PropSafetyDistance);
			MonsterChanceSlider.SetValue(layer.MonsterGeneralChance * 0.01f);

			void UpdateView(CView view, List<IDChance> ids, List<CImageSelectorUI.ElementInfo> allInfos,
				List<CImageSelectorUI.ElementInfo> availableInfos, Action<string, float> onProbChange)
			{
				view.Clear();
				for (int i = 0; i < ids.Count; ++i)
				{
					var idchance = ids[i];

					var info = allInfos[idchance.ID - 1];
					var elem = view.AddElement(new ViewElementProbInfo()
					{
						Image = info.Image,
						Text = info.Name,
						Probability = idchance.Chance * 0.01f
					}) as CViewElementProb;
					elem.SetOnProbChange(onProbChange);
					for (int j = 0; j < availableInfos.Count; ++j)
					{
						if (availableInfos[j].Name == info.Name)
						{
							availableInfos.RemoveAt(j);
							break;
						}
					}
				}
			}
			UpdateView(MaterialFamiliesView, layer.MaterialFamilies, BlockMaterial.UIMaterialFamilies, m_AvailableBlockMaterials, OnBlockMaterialChanceChange);
			UpdateView(PropFamiliesView, layer.PropFamilies, Props.UIProps, m_AvailableProps, OnPropChanceChange);
			UpdateView(MonsterFamiliesView, layer.MonsterFamilies, Monsters.UIMonsters, m_AvailableMonsters, OnMonsterChanceChange);
		}
		void OnResetLayer()
		{
			m_TempLayers[m_CurrentLayer] = m_Biome.GetLayers()[m_CurrentLayer].GetACopy();
			int layerIdx = m_CurrentLayer;
			m_CurrentLayer = -1;
			OnLayerChange(layerIdx);
		}
		void OnInvalidateLayer()
		{
			m_TempLayers[m_CurrentLayer] = new World.BiomeLayer() { LayerType = (Def.BiomeLayerType)m_CurrentLayer };
			int layerIdx = m_CurrentLayer;
			m_CurrentLayer = -1;
			OnLayerChange(layerIdx);
		}
		void StoreLayerInfo()
		{
			if (m_CurrentLayer < 0)
				return;
			var layer = GetCurrentLayer();
			layer.SemiVoidChance = (ushort)Mathf.FloorToInt(SemiVoidChanceSlider.GetValue() * 100f);
			layer.WideBlockChance = (ushort)Mathf.FloorToInt(WideBlockChanceSlider.GetValue() * 100f);
			layer.StairBlockChance = (ushort)Mathf.FloorToInt(StairBlockChanceSlider.GetValue() * 100f);
			layer.RampBlockChance = (ushort)Mathf.FloorToInt(StairIsRampChanceSlider.GetValue() * 100f);
			layer.PropGeneralChance = (ushort)Mathf.FloorToInt(PropChanceSlider.GetValue() * 100f);
			layer.PropSafetyDistance = PropSafetyDistanceSlider.GetValue();
			layer.MonsterGeneralChance = (ushort)Mathf.FloorToInt(MonsterChanceSlider.GetValue() * 100f);
		}
		void SaveToBiomeIE()
		{
			var biomeIE = BiomeLoader.Biomes[m_Biome.IDXIE];
			for(int i = 0; i < Def.BiomeStatCount; ++i)
			{
				//var stat = BiomeStats[i];
				//var min = stat.GetMinValue();
				//var max = stat.GetMaxValue();
				//m_Biome.SetBiomeStat((Def.BiomeStat)i, min, max);
				m_Biome.GetBiomeStat((Def.BiomeStat)i, out int min, out int max);
				biomeIE.SetBiomeStat((Def.BiomeStat)i, min, max);
			}
			biomeIE.SetMinDistance(m_Biome.GetMinDistance());
		}
		bool CanExitLayer()
		{
			if (m_CurrentLayer < 0)
				return true;

			var layer = GetCurrentLayer();
			
			if (!layer.IsValid())
				return true;

			bool materialOK, propOK, monsterOK;

			string msg = "";
			ushort accumProb = 0;
			for (int i = 0; i < layer.MaterialFamilies.Count; ++i)
				accumProb += layer.MaterialFamilies[i].Chance;

			materialOK = accumProb == 10000;
			if (!materialOK)
				msg += "\nMaterial probabilities don't add to 100, " + (accumProb * 0.01f).ToString();

			if (layer.PropFamilies.Count > 0)
			{
				accumProb = 0;
				for (int i = 0; i < layer.PropFamilies.Count; ++i)
					accumProb += layer.PropFamilies[i].Chance;
				propOK = accumProb == 10000;
			}
			else
			{
				propOK = true;
			}
			if(!propOK)
				msg += "\nProp probabilities don't add to 100, " + (accumProb * 0.01f).ToString();

			if (layer.MonsterFamilies.Count > 0)
			{
				accumProb = 0;
				for (int i = 0; i < layer.MonsterFamilies.Count; ++i)
					accumProb += layer.MonsterFamilies[i].Chance;
				monsterOK = accumProb == 10000;
			}
			else
			{
				monsterOK = true;
			}
			if (!monsterOK)
				msg += "\nMonster probabilities don't add to 100, " + (accumProb * 0.01f).ToString();

			if (!materialOK || !propOK || !monsterOK)
			{
				var mb = MessageBoxUI.Create();
				mb.Init(Def.MessageBoxType.YesNo, "Layer Probability error",
					"Some errors have been found" + msg + "\n\nIf you want them to be normalized press YES, if you want to do it manually press NO.",
					null, null, () => // On YES
					{
						if (!materialOK)
						{
							GameUtils.UpdateChances2(ref layer.MaterialFamilies);
							UpdateUIProbs(MaterialFamiliesView, layer.MaterialFamilies);
						}
						if (!propOK)
						{
							GameUtils.UpdateChances2(ref layer.PropFamilies);
							UpdateUIProbs(PropFamiliesView, layer.PropFamilies);
						}
						if (!monsterOK)
						{
							GameUtils.UpdateChances2(ref layer.MonsterFamilies);
							UpdateUIProbs(MonsterFamiliesView, layer.MonsterFamilies);
						}
						GameUtils.DeleteGameobject(mb.gameObject);
					},
					() => // On NO
					{
						GameUtils.DeleteGameobject(mb.gameObject);
					});
				return false;
			}
			return true;
		}
		void OnCross()
		{
			if (!CanExitLayer())
				return;
			StoreLayerInfo();
			SaveToBiomeIE();
			m_OnEnd();
		}
		void OnBiomeMinDistanceChange(string value)
		{
			if (m_ValueLock)
				return;

			if(!int.TryParse(value, out int iVal))
			{
				Debug.LogWarning("Couldn't parse the min distance " + value);
				return;
			}

			var val = Mathf.Clamp(iVal, 1, ushort.MaxValue);
			if(val != iVal)
			{
				m_ValueLock = true;
				BiomeMinDistanceIF.text = val.ToString();
				m_ValueLock = false;
			}
			m_Biome.SetMinDistance(val);
		}
		void OnMaxMicroHeightChange(float value)
		{
			if (m_ValueLock)
				return;
			var layer = GetCurrentLayer();
			if (value < layer.MicroHeightMin)
			{
				value = layer.MicroHeightMin;
				m_ValueLock = true;
				MaxMicroHeightSlider.SetValue(value);
			}
			layer.MicroHeightMax = value;
			m_ValueLock = false;
		}
		void OnMinMicroHeightChange(float value)
		{
			if (m_ValueLock)
				return;
			var layer = GetCurrentLayer();
			if (value > layer.MicroHeightMax)
			{
				value = layer.MicroHeightMax;
				m_ValueLock = true;
				MinMicroHeightSlider.SetValue(value);
			}
			layer.MicroHeightMin = value;
			m_ValueLock = false;
		}
		void OnMaxBiomeStatChange(Def.BiomeStat biomeStat, float value)
		{
			if (m_ValueLock)
				return;

			m_ValueLock = true;

			int ivalue = (int)value;
			m_Biome.GetBiomeStat(biomeStat, out int curMin, out int _);

			if (ivalue < curMin)
			{
				ivalue = curMin;
			}

			m_Biome.SetBiomeStat(biomeStat, curMin, ivalue);
			BiomeStats[(int)biomeStat].SetMaxValue(ivalue);

			m_ValueLock = false;
		}
		void OnMinBiomeStatChange(Def.BiomeStat biomeStat, float value)
		{
			if (m_ValueLock)
				return;

			m_ValueLock = true;
			
			int ivalue = (int)value;
			m_Biome.GetBiomeStat(biomeStat, out int _, out int curMax);

			if (ivalue > curMax)
			{
				ivalue = curMax;
			}
			BiomeStats[(int)biomeStat].SetMinValue(ivalue);
			m_Biome.SetBiomeStat(biomeStat, ivalue, curMax);
			
			m_ValueLock = false;
		}
		void UpdateChances(CView view, List<IDChance> chances, int toProtect = -1)
		{
			ushort lockAmount = 0;
			List<IDChance> nChances = new List<IDChance>(chances.Count);
			var elements = view.GetElements();
			int protectedID = -1;

			for(int i = 0; i < chances.Count; ++i)
			{
				CViewElementProb vElement = null;
				if(elements.Count > i)
					vElement = elements[i] as CViewElementProb;
				var chance = chances[i];
				if(i == toProtect || (vElement != null && vElement.IsLocked()))
				{
					lockAmount += chance.Chance;
				}
				else
				{
					nChances.Add(chance);
				}
			}

			if(lockAmount > 0)
			{
				protectedID = nChances.Count;
				nChances.Add(new IDChance() { ID = -1, Chance = lockAmount });
			}
			GameUtils.UpdateChances2(ref nChances, protectedID);
			int lastChance = 0;
			for(int i = 0; i < chances.Count; ++i)
			{
				CViewElementProb vElement = null;
				if(elements.Count > i)
					vElement = elements[i] as CViewElementProb;
				if (i == toProtect || (vElement != null && vElement.IsLocked()))
					continue;

				chances[i] = new IDChance()
				{
					ID = chances[i].ID,
					Chance = nChances[lastChance++].Chance
				};
			}
		}
		KeyValuePair<int, ushort> GetRemainingChance(CView view, List<IDChance> chances)
		{
			ushort lockedAmount = 0;
			int lockedCount = 0;
			var elements = view.GetElements();

			for(int i = 0; i < chances.Count; ++i)
			{
				CViewElementProb vElement = null;
				if(elements.Count > i)
					vElement = elements[i] as CViewElementProb;
				if (vElement == null || !vElement.IsLocked())
					continue;
				lockedAmount += chances[i].Chance;
				++lockedCount;
			}

			ushort unlockedAmount = (ushort)(10000 - lockedAmount);
			int unlockedCount = chances.Count - lockedCount;

			return new KeyValuePair<int, ushort>(unlockedCount, unlockedAmount);
		}
		void AddElements(CView view, ref List<CImageSelectorUI.ElementInfo> availableInfos, ref List<IDChance> idChances, Dictionary<string, int> familyDict, Action<string, float> onProbChange)
		{
			var selected = SelectorUI.GetSelected();
			var pair = GetRemainingChance(view, idChances);
			if (pair.Value < selected.Count)
			{
				Debug.LogWarning("Not enough unlocked chance.");
				return;
			}
			int nSize = pair.Key + selected.Count; //view.GetElements().Count + selected.Count;
			ushort chance = (ushort)Mathf.FloorToInt(/*100f * */(pair.Value / (float)nSize));

			var totalChance = (ushort)(chance * selected.Count);

			idChances.Add(new IDChance()
			{
				ID = int.MaxValue,
				Chance = totalChance
			});
			UpdateChances(view, idChances, idChances.Count - 1);
			//GameUtils.UpdateChances2(ref idChances, idChances.Count - 1);
			idChances.RemoveAt(idChances.Count - 1);
			for (int i = 0; i < selected.Count; ++i)
			{
				var current = new CImageSelectorUI.ElementInfo();
				for (int j = 0; j < availableInfos.Count; ++j)
				{
					if (availableInfos[j].Name == selected[i])
					{
						current = availableInfos[j];
						availableInfos.RemoveAt(j);
						break;
					}
				}

				var idc = new IDChance()
				{
					ID = familyDict[current.Name],
					Chance = chance
				};
				idChances.Add(idc);

				var elem = view.AddElement(new ViewElementProbInfo()
				{
					Image = current.Image,
					Text = current.Name,
					Probability = chance * 0.01f
				}) as CViewElementProb;
				elem.SetOnProbChange(onProbChange);
			}
			UpdateChances(view, idChances);
			//GameUtils.UpdateChances2(ref idChances);
			UpdateUIProbs(view, idChances);
		}
		void OnAddBlockMaterial()
		{
			enabled = false;
			SelectorUI.gameObject.SetActive(true);
			SelectorUI.Init(m_AvailableBlockMaterials, true, OnAddBlockMaterialEnd, Def.ImageSelectorPosition.Center);
		}
		void OnAddBlockMaterialEnd()
		{
			enabled = true;
			SelectorUI.gameObject.SetActive(false);
			var layer = GetCurrentLayer();
			AddElements(MaterialFamiliesView, ref m_AvailableBlockMaterials, ref layer.MaterialFamilies, BlockMaterial.FamilyDict, OnBlockMaterialChanceChange);
		}
		void OnAddProp()
		{
			enabled = false;
			SelectorUI.gameObject.SetActive(true);
			SelectorUI.Init(m_AvailableProps, true, OnAddPropEnd, Def.ImageSelectorPosition.Center);
		}
		void OnAddPropEnd()
		{
			enabled = true;
			SelectorUI.gameObject.SetActive(false);
			var layer = GetCurrentLayer();
			AddElements(PropFamiliesView, ref m_AvailableProps, ref layer.PropFamilies, Props.FamilyDict, OnPropChanceChange);
		}
		void OnAddMonster()
		{
			enabled = false;
			SelectorUI.gameObject.SetActive(true);
			SelectorUI.Init(m_AvailableMonsters, true, OnAddMonsterEnd, Def.ImageSelectorPosition.Center);
		}
		void OnAddMonsterEnd()
		{
			enabled = true;
			SelectorUI.gameObject.SetActive(false);
			var layer = GetCurrentLayer();
			AddElements(MonsterFamiliesView, ref m_AvailableMonsters, ref layer.MonsterFamilies, Monsters.FamilyDict, OnMonsterChanceChange);
		}
		void OnChanceReset(CView view, ref List<IDChance> list)
		{
			var pair = GetRemainingChance(view, list);
			var elements = view.GetElements();

			ushort nChance = (ushort)(pair.Value / pair.Key);
			for (int i = 0; i < list.Count; ++i)
			{
				CViewElementProb vElement = null;
				if (elements.Count > i)
					vElement = elements[i] as CViewElementProb;
				if (vElement.IsLocked())
					continue;
				var fam = list[i];
				list[i] = new IDChance()
				{
					ID = fam.ID,
					Chance = nChance
				};
			}
			//GameUtils.UpdateChances2(ref list);
		}
		void UpdateUIProbs(CView view, List<IDChance> idc)
		{
			for(int i = 0; i < view.GetElements().Count; ++i)
			{
				var elem = view.GetElements()[i] as CViewElementProb;
				elem.SetProbValue(idc[i].Chance * 0.01f);
			}
		}
		void OnBlockMaterialChanceReset()
		{
			OnChanceReset(MaterialFamiliesView, ref GetCurrentLayer().MaterialFamilies);
			UpdateUIProbs(MaterialFamiliesView, GetCurrentLayer().MaterialFamilies);
		}
		void OnPropChanceReset()
		{
			OnChanceReset(PropFamiliesView, ref GetCurrentLayer().PropFamilies);
			UpdateUIProbs(PropFamiliesView, GetCurrentLayer().PropFamilies);
		}
		void OnMonsterChanceReset()
		{
			OnChanceReset(MonsterFamiliesView, ref GetCurrentLayer().MonsterFamilies);
			UpdateUIProbs(MonsterFamiliesView, GetCurrentLayer().MonsterFamilies);
		}
		void OnElementRemoved(CView view, ref List<IDChance> idc, int id)
		{
			for (int i = 0; i < idc.Count; ++i)
			{
				if (idc[i].ID == id)
				{
					idc.RemoveAt(i);
					break;
				}
			}
			UpdateChances(view, idc);
			//GameUtils.UpdateChances2(ref idc);
		}
		void OnUIElementRemoved(ref List<CImageSelectorUI.ElementInfo> availableElements, List<CImageSelectorUI.ElementInfo> allElements, string name)
		{
			for(int i = 0; i < allElements.Count; ++i)
			{
				if(allElements[i].Name == name)
				{
					if (availableElements.Count > i)
						availableElements.Insert(i, allElements[i]);
					else
						availableElements.Add(allElements[i]);
					break;
				}
			}
			
		}
		void OnBlockMaterialRemoved2(CViewElement elem)
		{
			var name = elem.NameText != null ? elem.NameText.text : elem.TMPNameText != null ? elem.TMPNameText.text : "";
			var id = BlockMaterial.FamilyDict[name];
			var layer = GetCurrentLayer();

			OnElementRemoved(MaterialFamiliesView, ref layer.MaterialFamilies, id);
			UpdateUIProbs(MaterialFamiliesView, GetCurrentLayer().MaterialFamilies);
			OnUIElementRemoved(ref m_AvailableBlockMaterials, BlockMaterial.UIMaterialFamilies, name);
		}
		void OnBlockMaterialRemoved(string name)
		{
			var id = BlockMaterial.FamilyDict[name];
			var layer = GetCurrentLayer();

			OnElementRemoved(MaterialFamiliesView, ref layer.MaterialFamilies, id);
			UpdateUIProbs(MaterialFamiliesView, GetCurrentLayer().MaterialFamilies);
			OnUIElementRemoved(ref m_AvailableBlockMaterials, BlockMaterial.UIMaterialFamilies, name);
		}
		void OnPropRemoved2(CViewElement elem)
		{
			var name = elem.NameText != null ? elem.NameText.text : elem.TMPNameText != null ? elem.TMPNameText.text : "";
			var id = Props.FamilyDict[name];
			var layer = GetCurrentLayer();

			OnElementRemoved(PropFamiliesView, ref layer.PropFamilies, id);
			UpdateUIProbs(PropFamiliesView, GetCurrentLayer().PropFamilies);
			OnUIElementRemoved(ref m_AvailableProps, Props.UIProps, name);
		}
		void OnPropRemoved(string name)
		{
			var id = Props.FamilyDict[name];
			var layer = GetCurrentLayer();

			OnElementRemoved(PropFamiliesView, ref layer.PropFamilies, id);
			UpdateUIProbs(PropFamiliesView, GetCurrentLayer().PropFamilies);
			OnUIElementRemoved(ref m_AvailableProps, Props.UIProps, name);
		}
		void OnMonsterRemoved2(CViewElement elem)
		{
			var name = elem.NameText != null ? elem.NameText.text : elem.TMPNameText != null ? elem.TMPNameText.text : "";
			var id = Monsters.FamilyDict[name];
			var layer = GetCurrentLayer();

			OnElementRemoved(MonsterFamiliesView, ref layer.MonsterFamilies, id);
			UpdateUIProbs(MonsterFamiliesView, GetCurrentLayer().MonsterFamilies);
			OnUIElementRemoved(ref m_AvailableMonsters, Monsters.UIMonsters, name);
		}
		void OnMonsterRemoved(string name)
		{
			var id = Monsters.FamilyDict[name];
			var layer = GetCurrentLayer();

			OnElementRemoved(MonsterFamiliesView, ref layer.MonsterFamilies, id);
			UpdateUIProbs(MonsterFamiliesView, GetCurrentLayer().MonsterFamilies);
			OnUIElementRemoved(ref m_AvailableMonsters, Monsters.UIMonsters, name);
		}
		void OnChanceChange(CView view, ref List<IDChance> idChances, int id, float prob)
		{
			int idx = -1;
			for(int i = 0; i < idChances.Count; ++i)
			{
				var idc = idChances[i];
				if(idc.ID == id)
				{
					idx = i;
					idChances[i] = new IDChance()
					{
						ID = id,
						Chance = (ushort)Mathf.FloorToInt(prob * 100f)
					};
					break;
				}
			}
			if (idx < 0)
				return;
			var pair = GetRemainingChance(view, idChances);
			if (idChances[idx].Chance > pair.Value)
			{
				idChances[idx] = new IDChance()
				{
					ID = id,
					Chance = pair.Value
				};
			}
			UpdateChances(view, idChances, idx);
			//UpdateChances(view, idChances);
			//GameUtils.UpdateChances2(ref idChances, idx);
		}
		void OnBlockMaterialChanceChange(string name, float prob)
		{
			var id = BlockMaterial.FamilyDict[name];
			var layer = GetCurrentLayer();

			OnChanceChange(MaterialFamiliesView, ref layer.MaterialFamilies, id, prob);
			UpdateUIProbs(MaterialFamiliesView, GetCurrentLayer().MaterialFamilies);
		}
		void OnPropChanceChange(string name, float prob)
		{
			var id = Props.FamilyDict[name];
			var layer = GetCurrentLayer();

			OnChanceChange(PropFamiliesView, ref layer.PropFamilies, id, prob);
			UpdateUIProbs(PropFamiliesView, GetCurrentLayer().PropFamilies);
		}
		void OnMonsterChanceChange(string name, float prob)
		{
			var id = Monsters.FamilyDict[name];
			var layer = GetCurrentLayer();

			OnChanceChange(MonsterFamiliesView, ref layer.MonsterFamilies, id, prob);
			UpdateUIProbs(MonsterFamiliesView, GetCurrentLayer().MonsterFamilies);
		}
	}
}
