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
	public class CMonsterEditorUI : MonoBehaviour
	{
		public UnityEngine.UI.Button CrossButton;
		public UnityEngine.UI.Button ResetButton;

		public UnityEngine.UI.Image MonsterImage;

		public CImageSelectorUI Selector;

		public UnityEngine.UI.InputField BaseHealthIF;
		public UnityEngine.UI.InputField HealthRegenIF;
		public UnityEngine.UI.InputField BaseSoulnessIF;
		public UnityEngine.UI.InputField SoulnessRegenIF;
		public UnityEngine.UI.InputField BaseSpeedIF;
		public UnityEngine.UI.InputField WeightIF;
		public UnityEngine.UI.InputField SightRangeIF;
		public CSlider SightAngleSlider;
		public UnityEngine.UI.InputField HearingRangeIF;
		public UnityEngine.UI.InputField ScaleIF;
		public UnityEngine.UI.InputField StepIF;
		public UnityEngine.UI.InputField PhysicalResistanceIF;
		public UnityEngine.UI.Toggle PhysicalResistanceToggle;
		public UnityEngine.UI.InputField ElementalResistanceIF;
		public UnityEngine.UI.Toggle ElementalResistanceToggle;
		public UnityEngine.UI.InputField UltimateResistanceIF;
		public UnityEngine.UI.Toggle UltimateResistanceToggle;
		public UnityEngine.UI.InputField SoulResistanceIF;
		public UnityEngine.UI.Toggle SoulResistanceToggle;
		public UnityEngine.UI.InputField PoisonResistanceIF;
		public UnityEngine.UI.Toggle PoisonResistanceToggle;
		public UnityEngine.UI.InputField WarningTimeIF;
		public UnityEngine.UI.InputField AlertTimeIF;

		public CView FriendshipView;
		public UnityEngine.UI.Button AddFamilyExceptionButton;
		public CView QuirkView;
		public UnityEngine.UI.Button AddQuirkButton;
		public CMonsterSpell[] MonsterSpells;

		MonsterFamily m_Monster;
		Action m_OnEnd;
		List<CImageSelectorUI.ElementInfo> m_AllMonsters;
		List<CImageSelectorUI.ElementInfo> m_AvailableMonsters;
		List<CImageSelectorUI.ElementInfo> m_AllQuirks;

		void DumpToInfo()
		{
			void OnParseError(bool ok, string item)
			{
				if(!ok)
					Debug.LogWarning("Error while parsing " + item + " !CMonsterEditorUI.DumpToInfo()");
			}
			bool parseOK;
			
			parseOK = float.TryParse(BaseHealthIF.text, out m_Monster.Info.BaseHealth);
			OnParseError(parseOK, "BaseHealth");
			
			parseOK = float.TryParse(HealthRegenIF.text, out m_Monster.Info.HealthRegen);
			OnParseError(parseOK, "HealthRegen");
			
			parseOK = float.TryParse(BaseSoulnessIF.text, out m_Monster.Info.BaseSoulness);
			OnParseError(parseOK, "BaseSoulness");
			
			parseOK = float.TryParse(SoulnessRegenIF.text, out m_Monster.Info.SoulnessRegen);
			OnParseError(parseOK, "SoulnessRegen");
			
			parseOK = float.TryParse(BaseSpeedIF.text, out m_Monster.Info.BaseSpeed);
			OnParseError(parseOK, "BaseSpeed");
			
			parseOK = float.TryParse(WeightIF.text, out m_Monster.Info.Weight);
			OnParseError(parseOK, "Weight");
			
			parseOK = float.TryParse(SightRangeIF.text, out m_Monster.Info.SightRange);
			OnParseError(parseOK, "BaseHealth");
			
			m_Monster.Info.SightAngle = SightAngleSlider.Slider.value;
			
			parseOK = float.TryParse(HearingRangeIF.text, out m_Monster.Info.HearingRange);
			OnParseError(parseOK, "HearingRange");

			parseOK = float.TryParse(ScaleIF.text, out m_Monster.Info.SpriteScale);
			OnParseError(parseOK, "SpriteScale");

			parseOK = float.TryParse(StepIF.text, out m_Monster.Info.StepDistance);
			OnParseError(parseOK, "StepDistance");
			
			parseOK = float.TryParse(PhysicalResistanceIF.text, out m_Monster.Info.PhysicalResistance);
			OnParseError(parseOK, "PhysicalResistance");
			m_Monster.Info.PhysicalHealing = PhysicalResistanceToggle.isOn;

			parseOK = float.TryParse(ElementalResistanceIF.text, out m_Monster.Info.ElementalResistance);
			OnParseError(parseOK, "ElementalResistance");
			m_Monster.Info.ElementalHealing = ElementalResistanceToggle.isOn;

			parseOK = float.TryParse(UltimateResistanceIF.text, out m_Monster.Info.UltimateResistance);
			OnParseError(parseOK, "UltimateResistance");
			m_Monster.Info.UltimateHealing = UltimateResistanceToggle.isOn;

			parseOK = float.TryParse(SoulResistanceIF.text, out m_Monster.Info.SoulResistance);
			OnParseError(parseOK, "SoulResistance");
			m_Monster.Info.SoulHealing = SoulResistanceToggle.isOn;

			parseOK = float.TryParse(PoisonResistanceIF.text, out m_Monster.Info.PoisonResistance);
			OnParseError(parseOK, "PoisonResistance");
			m_Monster.Info.PoisonHealing = PoisonResistanceToggle.isOn;

			parseOK = float.TryParse(WarningTimeIF.text, out m_Monster.Info.WarnTime);
			OnParseError(parseOK, "WarnTime");

			parseOK = float.TryParse(AlertTimeIF.text, out m_Monster.Info.AlertTime);
			OnParseError(parseOK, "AlertTime");

			m_Monster.Info.Friendships.Clear();
			for(int i = 0; i < FriendshipView.GetElements().Count; ++i)
			{
				var elem = FriendshipView.GetElements()[i] as CViewElementFrienship;
				m_Monster.Info.Friendships.Add(new AI.FamilyFriendship()
				{
					FamilyName = elem.NameText.text,
					Friend = !elem.FriendButton.interactable
				});
			}

			m_Monster.Info.Quirks.Clear();
			for(int i = 0; i < QuirkView.GetElements().Count; ++i)
			{
				var elem = QuirkView.GetElements()[i] as CViewElementQuirk;
				var quirkConf = new List<ConfigInfo>(elem.Quirk.GetConfiguration().Count);
				for(int j = 0; j < elem.Quirk.GetConfiguration().Count; ++j)
				{
					var qConf = elem.Quirk.GetConfiguration()[j];
					quirkConf.Add(new ConfigInfo()
					{
						Name = qConf.GetConfigName(),
						ConfigType = qConf.GetConfigType(),
						Value = qConf.GetValueString()
					});
				}
				var triggers = new List<AI.TriggerInfo>(elem.Triggers.Count);
				for(int j = 0; j < elem.Triggers.Count; ++j)
				{
					var trigger = elem.Triggers[j];

					var triggerConf = new List<ConfigInfo>(trigger.GetConfig().Count);
					for(int k = 0; k < trigger.GetConfig().Count; ++k)
					{
						var tConf = trigger.GetConfig()[k];
						triggerConf.Add(new ConfigInfo()
						{
							ConfigType = tConf.GetConfigType(),
							Name = tConf.GetConfigName(),
							Value = tConf.GetValueString()
						});
					}

					triggers.Add(new AI.TriggerInfo()
					{
						TriggerType = trigger.GetTriggerType(),
						Configuration = triggerConf,
						Inverted = trigger.IsInverted()
					});
				}

				parseOK = int.TryParse(elem.PriorityIF.text, out int priority);
				OnParseError(parseOK, "Quirk priority");
				m_Monster.Info.Quirks.Add(new AI.QuirkInfo()
				{
					Configuration = quirkConf,
					Priority = priority,
					QuirkName = elem.Quirk.GetName(),
					Triggers = triggers
				});
			}
			
			for(int i = 0; i < Def.MonsterSpellSlotsCount; ++i)
			{
				var attack = MonsterSpells[i];
				var spell = attack.GetSpell();
				if (spell == null || spell.GetName() == "NullSpell")
					continue;

				AI.AttackInfo info = new AI.AttackInfo()
				{
					AttackName = spell.GetName(),
					Configuration = new List<ConfigInfo>(spell.GetConfig().Count),
					OnHitConfiguration = new List<AI.OnHitConfig>()
				};
				for(int j = 0; j < spell.GetConfig().Count; ++j)
				{
					var conf = spell.GetConfig()[j];
					info.Configuration.Add(new ConfigInfo()
					{
						ConfigType = conf.GetConfigType(),
						Name = conf.GetConfigName(),
						Value = conf.GetValueString()
					});
				}
				
				for(int j = 0; j < Def.OnHitTypeCount; ++j)
				{
					var onHitList = spell.GetOnHit()[(Def.OnHitType)j];

					for(int k = 0; k < onHitList.Count; ++k)
					{
						info.OnHitConfiguration.Add(new AI.OnHitConfig()
						{
							OnHitType = (Def.OnHitType)j,
							Configuration = onHitList[k].GetValueString()
						});
					}
				}

				m_Monster.Info.Attacks[i] = info;
			}
		}
		void SaveAndApplyInfo()
		{
			Monsters.SaveMonsterInfo(Monsters.FamilyDict[m_Monster.Name]);
			var world = World.World.gWorld;
			for(int i = 0; i < world.GetStrucs().Length; ++i)
			{
				var struc = world.GetStrucs()[i];
				if (struc == null || struc.GetLES() == null)
					continue;
				for(int j = 0; j < struc.GetLES().Count; ++j)
				{
					var le = struc.GetLES()[j];
					if (le == null)
						continue;

					if (le.GetLEType() != Def.LivingEntityType.Monster)
						continue;

					var mon = le.gameObject.GetComponent<AI.CMonster>();
					if (mon.GetFamily().Name != m_Monster.Name)
						continue;
					mon.OnFamilyUpdated();
					if (mon.TryGetComponent(out AI.CMonsterController controller))
						controller.OnFamilyUpdated();
				}
			}
			//var map = Map.GetCurrent();
			//map.ForEachStruc((CStruc struc) =>
			//{
			//	for(int i = 0; i < struc.GetLES().Count; ++i)
			//	{
			//		var le = struc.GetLES()[i];
			//		if (le == null)
			//			continue;
			//		if (le.GetLEType() != Def.LivingEntityType.Monster)
			//			continue;
			//		var mon = le.gameObject.GetComponent<AI.CMonster>();
			//		if (mon.GetFamily().Name != m_Monster.Name)
			//			continue;
			//		mon.OnFamilyUpdated();
			//		if(mon.TryGetComponent(out AI.CMonsterController controller))
			//		{
			//			controller.OnFamilyUpdated();
			//		}
			//	}
			//});
		}
		void OnCrossButton()
		{
			DumpToInfo();
			SaveAndApplyInfo();
			m_OnEnd();
		}
		void OnResetButton()
		{
			MonsterImage.sprite = m_Monster.Frames[0];
			BaseHealthIF.text = m_Monster.Info.BaseHealth.ToString();
			HealthRegenIF.text = m_Monster.Info.HealthRegen.ToString();
			BaseSoulnessIF.text = m_Monster.Info.BaseSoulness.ToString();
			SoulnessRegenIF.text = m_Monster.Info.SoulnessRegen.ToString();
			BaseSpeedIF.text = m_Monster.Info.BaseSpeed.ToString();
			WeightIF.text = m_Monster.Info.Weight.ToString();
			SightRangeIF.text = m_Monster.Info.SightRange.ToString();
			SightAngleSlider.SetValue(m_Monster.Info.SightAngle);
			HearingRangeIF.text = m_Monster.Info.HearingRange.ToString();
			ScaleIF.text = m_Monster.Info.SpriteScale.ToString();
			StepIF.text = m_Monster.Info.StepDistance.ToString();
			PhysicalResistanceIF.text = m_Monster.Info.PhysicalResistance.ToString();
			PhysicalResistanceToggle.isOn = m_Monster.Info.PhysicalHealing;
			ElementalResistanceIF.text = m_Monster.Info.ElementalResistance.ToString();
			ElementalResistanceToggle.isOn = m_Monster.Info.ElementalHealing;
			UltimateResistanceIF.text = m_Monster.Info.UltimateResistance.ToString();
			UltimateResistanceToggle.isOn = m_Monster.Info.UltimateHealing;
			PoisonResistanceIF.text = m_Monster.Info.PoisonResistance.ToString();
			PoisonResistanceToggle.isOn = m_Monster.Info.PoisonHealing;
			WarningTimeIF.text = m_Monster.Info.WarnTime.ToString();
			AlertTimeIF.text = m_Monster.Info.AlertTime.ToString();
			FriendshipView.Clear();
			for(int i = 0; i < m_Monster.Info.Friendships.Count; ++i)
			{
				var curFriend = m_Monster.Info.Friendships[i];
				FriendshipViewElementInfo elem = null;
				if(curFriend.FamilyName == "ODD")
				{
					elem = new FriendshipViewElementInfo()
					{
						Image = null,
						Text = curFriend.FamilyName,
						OnFriendshipChange = OnFriendshipChange,
						IsFriend = curFriend.Friend
					};
				}
				else
				{
					elem = new FriendshipViewElementInfo()
					{
						Image = Monsters.MonsterFamilies[Monsters.FamilyDict[curFriend.FamilyName]].Frames[0],
						Text = curFriend.FamilyName,
						OnFriendshipChange = OnFriendshipChange,
						IsFriend = curFriend.Friend
					};
				}
				FriendshipView.AddElement(elem);
			}
			if(m_AllMonsters != null)
			{
				m_AllMonsters.Clear();
				m_AvailableMonsters.Clear();
			}
			else
			{
				m_AllMonsters = new List<CImageSelectorUI.ElementInfo>(Monsters.MonsterFamilies.Count);
				m_AvailableMonsters = new List<CImageSelectorUI.ElementInfo>(Monsters.MonsterFamilies.Count);
			}
			var oddElem = new CImageSelectorUI.ElementInfo()
			{
				Image = null,
				Name = "ODD"
			};
			m_AllMonsters.Add(oddElem);
			m_AvailableMonsters.Add(oddElem);
			for (int i = 1; i < Monsters.MonsterFamilies.Count; ++i)
			{
				var family = Monsters.MonsterFamilies[i];
				var elem = new CImageSelectorUI.ElementInfo()
				{
					Image = family.Frames[0],
					Name = family.Name
				};
				m_AllMonsters.Add(elem);
				m_AvailableMonsters.Add(elem);
			}
			if(m_AllQuirks == null)
			{
				m_AllQuirks = new List<CImageSelectorUI.ElementInfo>(AI.Quirks.QuirkManager.Quirks.Count);
				for(int i = 0; i < AI.Quirks.QuirkManager.Quirks.Count; ++i)
				{
					var quirk = AI.Quirks.QuirkManager.Quirks[i];
					m_AllQuirks.Add(new CImageSelectorUI.ElementInfo()
					{
						Image = null,
						Name = quirk.GetName()
					});
				}
			}
			// Init Quirks
			QuirkView.Clear();
			void ConfigTrigger(AI.Quirks.IQuirkTrigger trigger, List<ConfigInfo> configs)
			{
				for (int i = 0; i < configs.Count; ++i)
				{
					var conf = configs[i];
					IConfig enumConfig = null;
					if (conf.ConfigType == Def.ConfigType.ENUM)
					{
						for (int k = 0; k < trigger.GetConfig().Count; ++k)
						{
							var config = trigger.GetConfig()[k];
							if (config.GetConfigName() == conf.Name)
							{
								enumConfig = config;
								break;
							}
						}
					}
					trigger.SetConfig(conf.Create(enumConfig));
				}
			}
			void ConfigQuirk(AI.Quirks.IQuirk quirk, List<ConfigInfo> configs)
			{
				for (int i = 0; i < configs.Count; ++i)
				{
					var conf = configs[i];
					IConfig enumConfig = null;
					if (conf.ConfigType == Def.ConfigType.ENUM)
					{
						for (int k = 0; k < quirk.GetConfiguration().Count; ++k)
						{
							var config = quirk.GetConfiguration()[k];
							if (config.GetConfigName() == conf.Name)
							{
								enumConfig = config;
								break;
							}
						}
					}
					quirk.SetConfiguration(conf.Create(enumConfig));
				}
			}
			for (int i = 0; i < m_Monster.Info.Quirks.Count; ++i)
			{
				var quirkInfo = m_Monster.Info.Quirks[i];
				var triggers = new List<AI.Quirks.IQuirkTrigger>(quirkInfo.Triggers.Count);
				for(int j = 0; j < quirkInfo.Triggers.Count; ++j)
				{
					var triggerInfo = quirkInfo.Triggers[j];
					var trigger = AI.Quirks.QuirkManager.CreateTrigger(triggerInfo.TriggerType);
					ConfigTrigger(trigger, triggerInfo.Configuration);
					trigger.SetInverted(triggerInfo.Inverted);
					triggers.Add(trigger);
				}
				var elemInfo = new ViewElementQuirkInfo()
				{
					Image = null,
					QuirkInfo = new AI.Quirks.QuirkInfo()
					{
						Priority = quirkInfo.Priority,
						Quirk = AI.Quirks.QuirkManager.CreateQuirk(quirkInfo.QuirkName),
						Triggers = triggers
					},
					Text = quirkInfo.QuirkName,
				};
				ConfigQuirk(elemInfo.QuirkInfo.Quirk, quirkInfo.Configuration);
				QuirkView.AddElement(elemInfo);
			}

			for(int i = 0; i < Def.MonsterSpellSlotsCount; ++i)
			{
				var attack = m_Monster.Info.Attacks[i];
				if(attack == null)
				{
					MonsterSpells[i].Init(null);
				}
				else
				{
					var spell = AI.Spells.SpellManager.CreateSpell(attack.AttackName);
					for(int j = 0; j < attack.Configuration.Count; ++j)
					{
						var conf = attack.Configuration[j];
						IConfig enumConfig = null;
						if (conf.ConfigType == Def.ConfigType.ENUM)
						{
							for (int k = 0; k < spell.GetConfig().Count; ++k)
							{
								var config = spell.GetConfig()[k];
								if (config.GetConfigName() == conf.Name)
								{
									enumConfig = config;
									break;
								}
							}
						}

						spell.SetConfig(conf.Create(enumConfig));
					}
					for(int j = 0; j < attack.OnHitConfiguration.Count; ++j)
					{
						spell.AddOnHit(attack.OnHitConfiguration[j]);
					}
					MonsterSpells[i].Init(spell);
				}
			}
		}
		void OnAddQuirk()
		{
			enabled = false;
			Selector.gameObject.SetActive(true);
			Selector.Init(m_AllQuirks, false, OnAddQuirkEnd, Def.ImageSelectorPosition.Center);
		}
		void OnAddQuirkEnd()
		{
			Selector.gameObject.SetActive(false);
			enabled = true;
			var selected = Selector.GetSelected();
			if(selected.Count > 0)
			{
				var quirkName = selected[0];
				var quirk = AI.Quirks.QuirkManager.CreateQuirk(quirkName);
				QuirkView.AddElement(new ViewElementQuirkInfo()
				{
					Image = null,
					Text = quirk.GetName(),
					QuirkInfo = new AI.Quirks.QuirkInfo()
					{
						Priority = 1,
						Quirk = quirk,
						Triggers = new List<AI.Quirks.IQuirkTrigger>()
					}
				});
			}
		}
		void OnFriendshipChange(string name, bool friend)
		{
			//for(int i = 0; i < m_Monster.Info.Friendships.Count; ++i)
			//{
			//	var curFriend = m_Monster.Info.Friendships[i];
			//	if (curFriend.FamilyName == name)
			//	{
			//		m_Monster.Info.Friendships[i] = new AI.FamilyFriendship()
			//		{
			//			FamilyName = curFriend.FamilyName,
			//			Friend = friend
			//		};
			//		break;
			//	}
			//}
		}
		void OnFriendRemoved2(CViewElement elem)
		{
			var name = elem.NameText != null ? elem.NameText.text : elem.TMPNameText != null ? elem.TMPNameText.text : "";
			for (int i = 0; i < m_Monster.Info.Friendships.Count; ++i)
			{
				for (int j = 0; j < m_AllMonsters.Count; ++j)
				{
					if (m_AllMonsters[j].Name == name)
					{
						m_AvailableMonsters.Add(m_AllMonsters[j]);
						break;
					}
				}
			}
		}
		void OnFriendRemoved(string name)
		{
			for(int i = 0; i < m_Monster.Info.Friendships.Count; ++i)
			{
				for(int j = 0; j < m_AllMonsters.Count; ++j)
				{
					if(m_AllMonsters[j].Name == name)
					{
						m_AvailableMonsters.Add(m_AllMonsters[j]);
						break;
					}
				}
				//var curFriend = m_Monster.Info.Friendships[i];
				//if (curFriend.FamilyName == name)
				//{
				//	m_Monster.Info.Friendships.RemoveAt(i);
				//	break;
				//}
			}
		}
		void OnAddExceptionButton()
		{
			enabled = false;
			Selector.gameObject.SetActive(true);
			Selector.Init(m_AvailableMonsters, true, OnAddExceptionButtonEnd, Def.ImageSelectorPosition.Center);
		}
		void OnAddExceptionButtonEnd()
		{
			Selector.gameObject.SetActive(false);
			enabled = true;
			var selected = Selector.GetSelected();
			if (selected.Count == 0)
				return;

			for(int i = 0; i < selected.Count; ++i)
			{
				var sel = selected[i];
				for(int j = 0; j < m_AvailableMonsters.Count; ++j)
				{
					if(m_AvailableMonsters[j].Name == sel)
					{
						m_AvailableMonsters.RemoveAt(j);
						break;
					}
				}

				FriendshipViewElementInfo elem = null;
				if(sel == "ODD")
				{
					elem = new FriendshipViewElementInfo()
					{
						Image = null,
						Text = sel,
						OnFriendshipChange = OnFriendshipChange,
						IsFriend = false
					};
				}
				else
				{
					elem = new FriendshipViewElementInfo()
					{
						Image = Monsters.MonsterFamilies[Monsters.FamilyDict[selected[i]]].Frames[0],
						Text = selected[i],
						OnFriendshipChange = OnFriendshipChange,
						IsFriend = false,
					};
				}
				FriendshipView.AddElement(elem);
				//m_Monster.Info.Friendships.Add(new AI.FamilyFriendship()
				//{
				//	FamilyName = selected[i],
				//	Friend = false
				//});
			}
		}
		public void Init(MonsterFamily family, Action onEditEnd)
		{
			m_OnEnd = onEditEnd;
			m_Monster = family;

			OnResetButton();
		}
		private void Awake()
		{
			CrossButton.onClick.AddListener(OnCrossButton);
			ResetButton.onClick.AddListener(OnResetButton);
			FriendshipView.SetOnElementRemove(OnFriendRemoved2);
			AddFamilyExceptionButton.onClick.AddListener(OnAddExceptionButton);
			AddQuirkButton.onClick.AddListener(OnAddQuirk);
		}
		private void Update()
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				OnCrossButton();
			}
		}
	}
}