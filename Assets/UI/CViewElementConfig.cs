/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Assets.UI
{
	public class ViewElementConfigInfo : ViewElementInfo
	{
		public IConfig Config;
	}
	public class CViewElementConfig : CViewElement
	{
		// Boolean
		public UnityEngine.UI.Toggle ValueToggle;
		// Float, Int, String
		public UnityEngine.UI.InputField ValueIF;
		// Vector2, Vector3
		public UnityEngine.UI.Text[] ValueTextV3;
		public UnityEngine.UI.InputField[] ValueV3IF;
		// Enum
		public UnityEngine.UI.Dropdown ValueDropdown;
		public UnityEngine.UI.Button AddEntityButton;
		public CView EntityView;
		public CImageSelectorUI ImageSelectorUI;

		public IConfig Config;

		List<CImageSelectorUI.ElementInfo> m_AllEntities;
		List<CImageSelectorUI.ElementInfo> m_AvailableEntities;
		List<CImageSelectorUI.ElementInfo> m_AvailableWeapons;

		public override void _OnAwake()
		{
			base._OnAwake();
			ValueToggle.onValueChanged.AddListener(OnToggle);
			ValueIF.onEndEdit.AddListener(OnInputField);
			ValueV3IF[0].onEndEdit.AddListener(OnInputFieldV1);
			ValueV3IF[1].onEndEdit.AddListener(OnInputFieldV2);
			ValueV3IF[2].onEndEdit.AddListener(OnInputFieldV3);
			ValueDropdown.onValueChanged.AddListener(OnDropdown);
			if (AddEntityButton != null)
			{
				AddEntityButton.onClick.AddListener(OnAddEntityButton);
				EntityView.SetOnElementRemove(OnEntityRemoved2);
			}
		}
		void PrepareEntityList()
		{
			EntityView.Clear();
			if(m_AllEntities == null)
			{
				m_AllEntities = new List<CImageSelectorUI.ElementInfo>();
				m_AvailableEntities = new List<CImageSelectorUI.ElementInfo>();
				m_AllEntities.AddRange(new CImageSelectorUI.ElementInfo[]
				{
					new CImageSelectorUI.ElementInfo()
					{
						Image = null,
						Name = "ODD"
					},
					new CImageSelectorUI.ElementInfo()
					{
						Image = null,
						Name = "Target"
					},
					new CImageSelectorUI.ElementInfo()
					{
						Image = null,
						Name = "Self"
					},
					new CImageSelectorUI.ElementInfo()
					{
						Image = null,
						Name = "LastHitter"
					},
					new CImageSelectorUI.ElementInfo()
					{
						Image = null,
						Name = "Enemies"
					},
					new CImageSelectorUI.ElementInfo()
					{
						Image = null,
						Name = "Friends"
					},
				});
				m_AllEntities.Capacity = m_AllEntities.Capacity + Monsters.FamilyTags.Count
					+ Monsters.MonsterFamilies.Count + Props.FamilyTags.Count + Props.PropFamilies.Count;
				// All monster tags
				for(int i = 0; i < Monsters.FamilyTags.Count; ++i)
				{
					m_AllEntities.Add(new CImageSelectorUI.ElementInfo()
					{
						Image = null,
						Name = Monsters.FamilyTags[i].TagName
					});
				}
				// All prop tags
				for(int i = 0; i < Props.FamilyTags.Count; ++i)
				{
					m_AllEntities.Add(new CImageSelectorUI.ElementInfo()
					{
						Image = null,
						Name = Props.FamilyTags[i].TagName
					});
				}
				// All monsters
				for(int i = 1; i < Monsters.MonsterFamilies.Count; ++i)
				{
					var family = Monsters.MonsterFamilies[i];
					m_AllEntities.Add(new CImageSelectorUI.ElementInfo()
					{
						Image = family.Frames[0],
						Name = family.Name
					});
				}
				// All props
				for(int i = 1; i < Props.PropFamilies.Count; ++i)
				{
					var family = Props.PropFamilies[i];
					m_AllEntities.Add(new CImageSelectorUI.ElementInfo()
					{
						Image = family.Props[0].PropSprite,
						Name = family.FamilyName
					});
				}
			}
			m_AvailableEntities.Clear();
			m_AvailableEntities.AddRange(m_AllEntities);
		}
		void PrepareWeaponList()
		{
			EntityView.Clear();
			if(m_AvailableWeapons == null)
			{
				m_AvailableWeapons = new List<CImageSelectorUI.ElementInfo>(SpellWeapons.List.Length);
				for(int i = 0; i < SpellWeapons.List.Length; ++i)
				{
					var weapon = SpellWeapons.List[i];
					m_AvailableWeapons.Add(new CImageSelectorUI.ElementInfo()
					{
						Name = weapon.name,
						Image = weapon
					});
				}
			}
		}
		public override void ElementInit(ViewElementInfo info, CView view)
		{
			base.ElementInit(info, view);
			Config = (info as ViewElementConfigInfo).Config;
			int cur;
			List<UnityEngine.UI.Dropdown.OptionData> options;
			switch (Config.GetConfigType())
			{
				case Def.ConfigType.STRING:
				case Def.ConfigType.INTEGER:
				case Def.ConfigType.FLOAT:
					ValueToggle.gameObject.SetActive(false);
					ValueIF.gameObject.SetActive(true);
					for (int i = 0; i < 3; ++i)
					{
						ValueTextV3[i].gameObject.SetActive(false);
						ValueV3IF[i].gameObject.SetActive(false);
					}
					ValueDropdown.gameObject.SetActive(false);
					if (AddEntityButton != null)
					{
						AddEntityButton.gameObject.SetActive(false);
						EntityView.gameObject.SetActive(false);
					}
					ValueIF.text = Config.GetValueString();
					break;
				case Def.ConfigType.BOOLEAN:
					ValueToggle.gameObject.SetActive(true);
					ValueIF.gameObject.SetActive(false);
					for (int i = 0; i < 3; ++i)
					{
						ValueTextV3[i].gameObject.SetActive(false);
						ValueV3IF[i].gameObject.SetActive(false);
					}
					ValueDropdown.gameObject.SetActive(false);
					if (AddEntityButton != null)
					{
						AddEntityButton.gameObject.SetActive(false);
						EntityView.gameObject.SetActive(false);
					}
					ValueToggle.isOn = (Config as ConfigBoolean).GetValue();
					break;
				case Def.ConfigType.VECTOR2:
					ValueToggle.gameObject.SetActive(false);
					ValueIF.gameObject.SetActive(false);
					for (int i = 0; i < 3; ++i)
					{
						ValueTextV3[i].gameObject.SetActive(i < 2);
						ValueV3IF[i].gameObject.SetActive(i < 2);
					}
					ValueDropdown.gameObject.SetActive(false);
					if (AddEntityButton != null)
					{
						AddEntityButton.gameObject.SetActive(false);
						EntityView.gameObject.SetActive(false);
					}
					var v2 = (Config as ConfigVector2).GetValue();
					ValueV3IF[0].text = v2.x.ToString();
					ValueV3IF[1].text = v2.y.ToString();
					break;
				case Def.ConfigType.VECTOR3:
					ValueToggle.gameObject.SetActive(false);
					ValueIF.gameObject.SetActive(false);
					for (int i = 0; i < 3; ++i)
					{
						ValueTextV3[i].gameObject.SetActive(true);
						ValueV3IF[i].gameObject.SetActive(true);
					}
					ValueDropdown.gameObject.SetActive(false);
					if (AddEntityButton != null)
					{
						AddEntityButton.gameObject.SetActive(false);
						EntityView.gameObject.SetActive(false);
					}
					var v3 = (Config as ConfigVector3).GetValue();
					ValueV3IF[0].text = v3.x.ToString();
					ValueV3IF[1].text = v3.y.ToString();
					ValueV3IF[2].text = v3.z.ToString();
					break;
				case Def.ConfigType.ENUM:
					ValueToggle.gameObject.SetActive(false);
					ValueIF.gameObject.SetActive(false);
					for (int i = 0; i < 3; ++i)
					{
						ValueTextV3[i].gameObject.SetActive(false);
						ValueV3IF[i].gameObject.SetActive(false);
					}
					ValueDropdown.gameObject.SetActive(true);
					if (AddEntityButton != null)
					{
						AddEntityButton.gameObject.SetActive(false);
						EntityView.gameObject.SetActive(false);
					}
					var names = Enum.GetNames(Config.GetBaseType());
					cur = -1;
					options = new List<UnityEngine.UI.Dropdown.OptionData>(names.Length);
					for (int i = 0; i < names.Length; ++i)
					{
						var name = names[i];
						options.Add(new UnityEngine.UI.Dropdown.OptionData(name));
						if (cur < 0)
						{
							if (name == Config.GetValueString())
							{
								cur = i;
							}
						}
					}
					if (cur < 0)
						cur = 0;
					ValueDropdown.ClearOptions();
					ValueDropdown.AddOptions(options);
					ValueDropdown.value = cur;
					ValueDropdown.RefreshShownValue();
					break;
				case Def.ConfigType.ENTITYLIST:
					ValueToggle.gameObject.SetActive(false);
					ValueIF.gameObject.SetActive(false);
					for (int i = 0; i < 3; ++i)
					{
						ValueTextV3[i].gameObject.SetActive(false);
						ValueV3IF[i].gameObject.SetActive(false);
					}
					ValueDropdown.gameObject.SetActive(false);
					AddEntityButton.gameObject.SetActive(true);
					AddEntityButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "Add Entity";
					EntityView.gameObject.SetActive(true);
					var entityList = (Config as ConfigEntityList).GetValue();
					PrepareEntityList();
					for (int i = 0; i < entityList.Count; ++i)
					{
						var entity = entityList[i];
						bool set = false;
						for (int j = 0; j < m_AvailableEntities.Count; ++j)
						{
							var elem = m_AvailableEntities[j];
							if (elem.Name == entity)
							{
								EntityView.AddElement(new ViewElementInfo()
								{
									Image = elem.Image,
									Text = elem.Name
								});
								m_AvailableEntities.RemoveAt(j);
								set = true;
								break;
							}
						}
						if (!set)
						{
							Debug.LogWarning("Couldn't found entity: " + entity);
						}
					}
					break;
				case Def.ConfigType.PARTICLE_TEXTURE:
					ValueToggle.gameObject.SetActive(false);
					ValueIF.gameObject.SetActive(false);
					for (int i = 0; i < 3; ++i)
					{
						ValueTextV3[i].gameObject.SetActive(false);
						ValueV3IF[i].gameObject.SetActive(false);
					}
					ValueDropdown.gameObject.SetActive(true);
					if (AddEntityButton != null)
					{
						AddEntityButton.gameObject.SetActive(false);
						EntityView.gameObject.SetActive(false);
					}
					var particleFamilies = Particles.ParticleFamilies;
					cur = -1;
					options = new List<UnityEngine.UI.Dropdown.OptionData>(particleFamilies.Count + 1)
					{
						new UnityEngine.UI.Dropdown.OptionData("NONE")
					};
					for (int i = 0; i < particleFamilies.Count; ++i)
					{
						var name = particleFamilies[i].FamilyName;
						options.Add(new UnityEngine.UI.Dropdown.OptionData(name));
						if (cur < 0)
						{
							if (name == Config.GetValueString())
							{
								cur = i + 1;
							}
						}
					}
					if (cur < 0)
						cur = 0;
					ValueDropdown.ClearOptions();
					ValueDropdown.AddOptions(options);
					ValueDropdown.value = cur;
					ValueDropdown.RefreshShownValue();
					break;
				case Def.ConfigType.SPELL_WEAPON:
					ValueToggle.gameObject.SetActive(false);
					ValueIF.gameObject.SetActive(false);
					for (int i = 0; i < 3; ++i)
					{
						ValueTextV3[i].gameObject.SetActive(false);
						ValueV3IF[i].gameObject.SetActive(false);
					}
					ValueDropdown.gameObject.SetActive(false);
					AddEntityButton.gameObject.SetActive(true);
					AddEntityButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "Set Weapon";
					EntityView.gameObject.SetActive(true);
					var weapon = (Config as ConfigSpellWeapon).GetValue();
					PrepareWeaponList();
					if (SpellWeapons.Dict.ContainsKey(weapon))
					{
						for (int i = 0; i < m_AvailableWeapons.Count; ++i)
						{
							var current = m_AvailableWeapons[i];
							if (current.Name == weapon)
							{
								EntityView.AddElement(new ViewElementInfo()
								{
									Image = current.Image,
									Text = current.Name
								});
								m_AvailableWeapons.RemoveAt(i);
								break;
							}
						}
					}

					break;
				default:
					Debug.LogError("Unhandled IConfig type: " + Config.GetConfigType().ToString());
					break;
			}
		}
		void OnInputFieldV1(string value)
		{
			if(Config.GetConfigType() == Def.ConfigType.VECTOR2)
			{
				if(float.TryParse(value, out float x))
				{
					var qv2 = Config as ConfigVector2;
					var v2 = qv2.GetValue();
					qv2.SetValue(new Vector2(x, v2.y));
				}
			}
			else if(Config.GetConfigType() == Def.ConfigType.VECTOR3)
			{
				if (float.TryParse(value, out float x))
				{
					var qv3 = Config as ConfigVector3;
					var v3 = qv3.GetValue();
					qv3.SetValue(new Vector3(x, v3.y, v3.z));
				}
			}
		}
		void OnInputFieldV2(string value)
		{
			if (Config.GetConfigType() == Def.ConfigType.VECTOR2)
			{
				if (float.TryParse(value, out float y))
				{
					var qv2 = Config as ConfigVector2;
					var v2 = qv2.GetValue();
					qv2.SetValue(new Vector2(v2.x, y));
				}
			}
			else if (Config.GetConfigType() == Def.ConfigType.VECTOR3)
			{
				if (float.TryParse(value, out float y))
				{
					var qv3 = Config as ConfigVector3;
					var v3 = qv3.GetValue();
					qv3.SetValue(new Vector3(v3.x, y, v3.z));
				}
			}
		}
		void OnInputFieldV3(string value)
		{
			if (float.TryParse(value, out float z))
			{
				var qv3 = Config as ConfigVector3;
				var v3 = qv3.GetValue();
				qv3.SetValue(new Vector3(v3.x, v3.y, z));
			}
		}
		void OnDropdown(int idx)
		{
			var option = ValueDropdown.options[idx];
			Config.FromString(option.text);
		}
		void OnInputField(string value)
		{
			Config.FromString(value);
		}
		void OnToggle(bool value)
		{
			Config.FromString(value.ToString());
		}
		void OnAddEntityButton()
		{
			enabled = false;
			ImageSelectorUI.gameObject.SetActive(true);
			switch(Config.GetConfigType())
			{
				case Def.ConfigType.ENTITYLIST:
					ImageSelectorUI.Init(m_AvailableEntities, true, OnAddEntityButtonEnd, Def.ImageSelectorPosition.Center);
					break;
				case Def.ConfigType.SPELL_WEAPON:
					ImageSelectorUI.Init(m_AvailableWeapons, false, OnAddEntityButtonEnd, Def.ImageSelectorPosition.Center);
					break;
			}
		}
		void OnAddEntityButtonEnd()
		{
			enabled = true;
			ImageSelectorUI.gameObject.SetActive(false);
			var selected = ImageSelectorUI.GetSelected();
			switch(Config.GetConfigType())
			{
				case Def.ConfigType.ENTITYLIST:
					{
						var entityConfig = Config as ConfigEntityList;
						var entityList = entityConfig.GetValue();
						for (int i = 0; i < selected.Count; ++i)
						{
							var selName = selected[i];
							if (entityList.Contains(selName))
								continue;
							entityList.Add(selName);

							var elemInfo = new CImageSelectorUI.ElementInfo();
							for (int j = 0; j < m_AllEntities.Count; ++j)
							{
								var elem = m_AllEntities[j];
								if (elem.Name == selName)
								{
									elemInfo = elem;
									break;
								}
							}
							if (elemInfo.Name.Length == 0)
								Debug.LogWarning("Couldn't find " + selName);

							EntityView.AddElement(new ViewElementInfo()
							{
								Image = elemInfo.Image,
								Text = elemInfo.Name
							});
						}
					}
					break;
				case Def.ConfigType.SPELL_WEAPON:
					{
						if (selected.Count != 1)
							return;
						var weaponConfig = Config as ConfigSpellWeapon;
						var prevWeapon = weaponConfig.GetValue();
						var newWeapon = selected[0];
						for (int j = 0; j < m_AvailableWeapons.Count; ++j)
						{
							if (m_AvailableWeapons[j].Name == newWeapon)
							{
								m_AvailableWeapons.RemoveAt(j);
								break;
							}
						}
						bool prevDone = false, newDone = false;
						for(int i = 0; i < SpellWeapons.List.Length; ++i)
						{
							var cur = SpellWeapons.List[i];
							if(!prevDone)
							{
								if(cur.name == prevWeapon)
								{
									m_AvailableWeapons.Add(new CImageSelectorUI.ElementInfo()
									{
										Image = cur,
										Name = cur.name
									});
									prevDone = true;
								}
							}
							if(!newDone)
							{
								if(cur.name == newWeapon)
								{
									EntityView.Clear();
									EntityView.AddElement(new ViewElementInfo()
									{
										Image = cur,
										Text = cur.name
									});
									weaponConfig.SetValue(newWeapon);
									newDone = true;
								}
							}
							if (prevDone && newDone)
								break;
						}
					}
					break;
			}
			
		}
		void OnEntityRemoved2(CViewElement elem)
		{
			var name = elem.NameText != null ? elem.NameText.text : elem.TMPNameText != null ? elem.TMPNameText.text : "";
			switch (Config.GetConfigType())
			{
				case Def.ConfigType.ENTITYLIST:
					{
						var configEntity = Config as ConfigEntityList;
						var entityList = configEntity.GetValue();
						entityList.Remove(name);
						configEntity.SetValue(entityList);
						for (int i = 0; i < m_AllEntities.Count; ++i)
						{
							var ent = m_AllEntities[i];
							if (ent.Name == name)
							{
								m_AvailableEntities.Add(ent);
								break;
							}
						}
					}
					break;
				case Def.ConfigType.SPELL_WEAPON:
					{
						var configWeapon = Config as ConfigSpellWeapon;
						configWeapon.SetValue("");
					}
					break;
			}
		}
		void OnEntityRemoved(string element)
		{
			switch(Config.GetConfigType())
			{
				case Def.ConfigType.ENTITYLIST:
					{
						var configEntity = Config as ConfigEntityList;
						var entityList = configEntity.GetValue();
						entityList.Remove(element);
						configEntity.SetValue(entityList);
						for (int i = 0; i < m_AllEntities.Count; ++i)
						{
							var elem = m_AllEntities[i];
							if (elem.Name == element)
							{
								m_AvailableEntities.Add(elem);
								break;
							}
						}
					}
					break;
				case Def.ConfigType.SPELL_WEAPON:
					{
						var configWeapon = Config as ConfigSpellWeapon;
						configWeapon.SetValue("");
					}
					break;
			}
		}
	}
}