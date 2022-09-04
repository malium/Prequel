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
	public class SpawnerEditorUI : MonoBehaviour
	{
		public UnityEngine.UI.Button CrossButton;
		public TMPro.TMP_Dropdown SpawnerSelector;
		public UnityEngine.UI.Button RemoveSpawnButton;
		public TMPro.TMP_InputField NameIF;
		public UnityEngine.UI.Button CompactButton;
		public UnityEngine.UI.Button NormalButton;
		public UnityEngine.UI.Button LargeButton;
		public TMPro.TMP_InputField DefaultMaxRangeIF;
		public CTMPSlider LastWaveProbabilitySlider;

		public CImageSelectorUI ImageSelectorUI;

		public UnityEngine.UI.Button AddSpawnButton;
		public CView SpawnView;
		
		public UnityEngine.UI.Button AddLastWaveSpawnButton;
		public CView LastWaveSpawnView;

		List<CImageSelectorUI.ElementInfo> m_AllElements;
		int m_DefaultRadius;
		Action m_OnClose;
		int m_SelectedOption;
		SpawnerInfo m_CurrentSpawner;
		bool m_ValueLock;

		private void Awake()
		{
			CrossButton.onClick.AddListener(OnCross);
			SpawnerSelector.onValueChanged.AddListener(OnSpawnerChange);
			RemoveSpawnButton.onClick.AddListener(OnSpawnerDelete);
			NameIF.onEndEdit.AddListener(OnNameChange);
			CompactButton.onClick.AddListener(OnCompactRange);
			NormalButton.onClick.AddListener(OnNormalRange);
			LargeButton.onClick.AddListener(OnLargeRange);
			DefaultMaxRangeIF.onEndEdit.AddListener(OnRangeChange);
			LastWaveProbabilitySlider.Set(0f, 100f, 0.01f, Def.DefaultLastWaveChance * 0.01f);
			LastWaveProbabilitySlider.SetCallback(OnLastWaveProbability);
			AddSpawnButton.onClick.AddListener(OnAddSpawn);
			SpawnView.SetOnElementRemove(OnRemoveSpawn);
			AddLastWaveSpawnButton.onClick.AddListener(OnAddLastWaveSpawn);
			LastWaveSpawnView.SetOnElementRemove(OnRemoveLastWaveSpawn);
		}
		void EnableInteraction(bool enable)
		{
			NameIF.interactable = enable;
			CompactButton.interactable = enable;
			NormalButton.interactable = enable;
			LargeButton.interactable = enable;
			DefaultMaxRangeIF.interactable = enable;
			LastWaveProbabilitySlider.Slider.interactable = enable;
			LastWaveProbabilitySlider.Input.interactable = enable;
			AddSpawnButton.interactable = enable;
			SpawnView.enabled = enable;
			AddLastWaveSpawnButton.interactable = enable;
			LastWaveSpawnView.enabled = enable;
			RemoveSpawnButton.interactable = enable;
		}
		public void Init(Action onClose = null)
		{
			m_OnClose = onClose != null ? onClose : () => { };
			
			m_ValueLock = true;
			SpawnView.Clear();
			LastWaveSpawnView.Clear();
			LastWaveProbabilitySlider.SetValue(Def.DefaultLastWaveChance * 0.01f);
			DefaultMaxRangeIF.text = Def.NormalSpawnRadius.ToString();
			NameIF.text = "";

			var options = new List<TMPro.TMP_Dropdown.OptionData>(2 + SpawnerInfoLoader.Dict.Count)
			{
				new TMPro.TMP_Dropdown.OptionData("Select Spawner"),
				new TMPro.TMP_Dropdown.OptionData("Create new Spawner")
			};
			for(int i = 0; i < SpawnerInfoLoader.Dict.Count; ++i)
			{
				var pair = SpawnerInfoLoader.Dict.ElementAt(i);
				options.Add(new TMPro.TMP_Dropdown.OptionData(pair.Key));
			}
			m_SelectedOption = 0;
			SpawnerSelector.options = options;
			SpawnerSelector.value = 0;
			SpawnerSelector.RefreshShownValue();
			EnableInteraction(false);
			if (m_AllElements == null)
			{
				m_AllElements = new List<CImageSelectorUI.ElementInfo>(Monsters.MonsterFamilies.Count - 1);
				for(int i = 1; i < Monsters.MonsterFamilies.Count; ++i)
				{
					var family = Monsters.MonsterFamilies[i];
					m_AllElements.Add(new CImageSelectorUI.ElementInfo()
					{
						Image = family.Frames[0],
						Name = family.Info.MonsterFamily
					});
				}
			}

			m_ValueLock = false;
		}
		void OnCompactRange()
		{
			m_DefaultRadius = Def.CompactSpawnRadius;
			m_CurrentSpawner.SetDefaultMaxRange((uint)m_DefaultRadius);
			DefaultMaxRangeIF.text = m_DefaultRadius.ToString();
		}
		void OnNormalRange()
		{
			m_DefaultRadius = Def.NormalSpawnRadius;
			m_CurrentSpawner.SetDefaultMaxRange((uint)m_DefaultRadius);
			DefaultMaxRangeIF.text = m_DefaultRadius.ToString();
		}
		void OnLargeRange()
		{
			m_DefaultRadius = Def.LargeSpawnRadius;
			m_CurrentSpawner.SetDefaultMaxRange((uint)m_DefaultRadius);
			DefaultMaxRangeIF.text = m_DefaultRadius.ToString();
		}
		void OnRangeChange(string sRange)
		{
			if (m_ValueLock)
				return;

			if(!int.TryParse(sRange, out int radius))
			{
				m_ValueLock = true;
				DefaultMaxRangeIF.text = m_DefaultRadius.ToString();
				m_ValueLock = false;
				return;
			}
			m_DefaultRadius = radius;
		}
		void OnLastWaveProbability(float value)
		{
			m_CurrentSpawner.SetLastWaveChance((ushort)Mathf.FloorToInt(value * 100f));
		}
		void OnCross()
		{
			SpawnerInfoLoader.SaveSpawnerInfos();
			m_OnClose();
		}
		void OnSpawnerChange(int optionID)
		{
			if (m_SelectedOption == optionID)
				return;

			m_SelectedOption = optionID;
			switch(optionID)
			{
				case 0: // null
					EnableInteraction(false);
					m_ValueLock = true;
					LastWaveProbabilitySlider.SetValue(Def.DefaultLastWaveChance * 0.01f);
					m_CurrentSpawner = null;
					SpawnView.Clear();
					LastWaveSpawnView.Clear();
					m_DefaultRadius = Def.NormalSpawnRadius;
					DefaultMaxRangeIF.text = Def.NormalSpawnRadius.ToString();
					NameIF.text = "";
					m_ValueLock = false;
					break;
				case 1: // create new
					var defName = "unnamed_spawner";
					var name = defName;
					int tries = 0;
					while (SpawnerInfoLoader.Dict.ContainsKey(name))
					{
						name = defName + '_' + (tries++).ToString();
					}
					var options = SpawnerSelector.options;
					options.Add(new TMPro.TMP_Dropdown.OptionData(name));
					m_SelectedOption = options.Count - 1;
					SpawnerSelector.options = options;
					SpawnerSelector.value = m_SelectedOption;

					SpawnerInfoLoader.Dict.Add(name, SpawnerInfoLoader.SpawnerInfos.Count);
					m_CurrentSpawner = new SpawnerInfo();
					m_CurrentSpawner.SetName(name);
					m_CurrentSpawner.SetLastWaveChance(Def.DefaultLastWaveChance);
					m_CurrentSpawner.SetDefaultMaxRange(Def.NormalSpawnRadius);
					SpawnerInfoLoader.SpawnerInfos.Add(m_CurrentSpawner);
					m_DefaultRadius = Def.NormalSpawnRadius;

					m_ValueLock = true;
					NameIF.text = name;
					DefaultMaxRangeIF.text = m_DefaultRadius.ToString();
					SpawnView.Clear();
					LastWaveSpawnView.Clear();
					LastWaveProbabilitySlider.SetValue(Def.DefaultLastWaveChance * 0.01f);
					m_ValueLock = false;
					EnableInteraction(true);
					break;
				default: // load spawner info
					var spawnerName = SpawnerSelector.options[optionID].text;
					if(SpawnerInfoLoader.Dict.TryGetValue(spawnerName, out int spawnIdx))
					{
						EnableInteraction(true);
						m_CurrentSpawner = SpawnerInfoLoader.SpawnerInfos[spawnIdx];
						m_ValueLock = true;
						NameIF.text = spawnerName;
						m_DefaultRadius = (int)m_CurrentSpawner.GetDefaultMaxRange();
						DefaultMaxRangeIF.text = m_DefaultRadius.ToString();
						SpawnView.Clear();
						for(int i = 0; i < m_CurrentSpawner.GetSpawns().Count; ++i)
						{
							var spawn = m_CurrentSpawner.GetSpawns()[i];
							SpawnView.AddElement(new ViewElementSpawnInfo()
							{
								Text = spawn.GetMonsterFamily(),
								Spawn = spawn,
								Image = m_AllElements[Monsters.FamilyDict[spawn.GetMonsterFamily()] - 1].Image
							});
						}
						LastWaveSpawnView.Clear();
						for (int i = 0; i < m_CurrentSpawner.GetLastWaveSpawns().Count; ++i)
						{
							var spawn = m_CurrentSpawner.GetLastWaveSpawns()[i];
							LastWaveSpawnView.AddElement(new ViewElementSpawnInfo()
							{
								Text = spawn.GetMonsterFamily(),
								Spawn = spawn,
								Image = m_AllElements[Monsters.FamilyDict[spawn.GetMonsterFamily()] - 1].Image
							});
						}
						LastWaveProbabilitySlider.SetValue(m_CurrentSpawner.GetLasWaveChance() * 0.01f);
						m_ValueLock = false;
					}
					else
					{
						SpawnerSelector.value = 0;
					}
					break;
			}
		}
		void OnSpawnerDelete()
		{
			var options = SpawnerSelector.options;
			var name = options[m_SelectedOption].text;
			options.RemoveAt(m_SelectedOption);
			SpawnerSelector.value = 0;
			SpawnerInfoLoader.RemoveSpawnerInfo(name);
		}
		void OnNameChange(string name)
		{
			if(m_ValueLock || m_CurrentSpawner == null)
				return;
			
			if (m_CurrentSpawner.GetName() == name)
				return;

			if(SpawnerInfoLoader.Dict.ContainsKey(name))
			{
				m_ValueLock = true;
				NameIF.text = m_CurrentSpawner.GetName();
				m_ValueLock = false;
				return;
			}
			SpawnerInfoLoader.RemoveSpawnerInfo(m_CurrentSpawner.GetName());
			SpawnerInfoLoader.Dict.Add(name, SpawnerInfoLoader.SpawnerInfos.Count);
			SpawnerInfoLoader.SpawnerInfos.Add(m_CurrentSpawner);
			m_CurrentSpawner.SetName(name);
			SpawnerSelector.options[m_SelectedOption].text = name;
			SpawnerSelector.RefreshShownValue();
		}
		void OnAddSpawn()
		{
			ImageSelectorUI.gameObject.SetActive(true);
			ImageSelectorUI.Init(m_AllElements, false, OnAddSpawnEnd, Def.ImageSelectorPosition.Center);
			enabled = false;
		}
		void OnAddSpawnEnd()
		{
			enabled = true;
			ImageSelectorUI.gameObject.SetActive(false);
			var selected = ImageSelectorUI.GetSelected();
			if (selected.Count == 0)
				return;

			var selection = selected[0];
			var spawn = new SpawnInfo();
			spawn.SetMonsterFamily(selection);
			spawn.SetTotalProbability(5000);
			spawn.SetMinRange(0);
			spawn.SetMaxRange(m_DefaultRadius);
			spawn.SetMinAmount(0);
			spawn.SetMaxAmount(1);
			m_CurrentSpawner.GetSpawns().Add(spawn);
			SpawnView.AddElement(new ViewElementSpawnInfo()
			{
				Text = selection,
				Spawn = spawn,
				Image = m_AllElements[Monsters.FamilyDict[spawn.GetMonsterFamily()] - 1].Image
			});
		}
		void OnRemoveSpawn(CViewElement elem)
		{
			var removedSpawn = (elem as CViewElementSpawn).GetSpawnInfo();
			for(int i = 0; i < m_CurrentSpawner.GetSpawns().Count; ++i)
			{
				var spawn = m_CurrentSpawner.GetSpawns()[i];
				if(removedSpawn == spawn)
				{
					m_CurrentSpawner.GetSpawns().RemoveAt(i);
					return;
				}
			}
		}
		void OnAddLastWaveSpawn()
		{
			ImageSelectorUI.gameObject.SetActive(true);
			ImageSelectorUI.Init(m_AllElements, false, OnAddLastWaveSpawnEnd, Def.ImageSelectorPosition.Center);
			enabled = false;
		}
		void OnAddLastWaveSpawnEnd()
		{
			enabled = true;
			ImageSelectorUI.gameObject.SetActive(false);
			var selected = ImageSelectorUI.GetSelected();
			if (selected.Count == 0)
				return;

			var selection = selected[0];
			var spawn = new SpawnInfo();
			spawn.SetMonsterFamily(selection);
			spawn.SetTotalProbability(50);
			spawn.SetMinRange(0);
			spawn.SetMaxRange(m_DefaultRadius);
			spawn.SetMinAmount(0);
			spawn.SetMaxAmount(1);
			m_CurrentSpawner.GetLastWaveSpawns().Add(spawn);
			LastWaveSpawnView.AddElement(new ViewElementSpawnInfo()
			{
				Text = selection,
				Spawn = spawn,
				Image = m_AllElements[Monsters.FamilyDict[spawn.GetMonsterFamily()] - 1].Image
			});
		}
		void OnRemoveLastWaveSpawn(CViewElement elem)
		{
			var removedSpawn = (elem as CViewElementSpawn).GetSpawnInfo();
			for (int i = 0; i < m_CurrentSpawner.GetLastWaveSpawns().Count; ++i)
			{
				var spawn = m_CurrentSpawner.GetLastWaveSpawns()[i];
				if (removedSpawn == spawn)
				{
					m_CurrentSpawner.GetLastWaveSpawns().RemoveAt(i);
					return;
				}
			}
		}
	}
}
