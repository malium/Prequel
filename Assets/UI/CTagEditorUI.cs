/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.UI
{
	public class CTagEditorUI : MonoBehaviour
	{
		public UnityEngine.UI.Button CrossButton;
		public UnityEngine.UI.Dropdown TagSelector;
		public UnityEngine.UI.Button RemoveTagButton;
		public UnityEngine.UI.InputField NameIF;
		public UnityEngine.UI.Button AddFamilyButton;
		public CView FriendlyFamilies;

		public CImageSelectorUI ImageSelectorUI;

		List<CImageSelectorUI.ElementInfo> m_AllElements;
		List<CImageSelectorUI.ElementInfo> m_AvailableElements;

		Action m_OnClose;
		int m_SelectedOption;
		FamilyTag m_CurrentTag;

		private void Awake()
		{
			CrossButton.onClick.AddListener(OnCrossButton);
			TagSelector.onValueChanged.AddListener(OnTagChange);
			RemoveTagButton.onClick.AddListener(OnTagRemove);
			NameIF.onEndEdit.AddListener(OnNameChange);
			AddFamilyButton.onClick.AddListener(OnAddFamily);
			FriendlyFamilies.SetOnElementRemove(OnElementRemove2);
		}
		void EnableInteraction(bool enable)
		{
			RemoveTagButton.interactable = enable;
			NameIF.interactable = enable;
			AddFamilyButton.interactable = enable;
			FriendlyFamilies.enabled = enable;
		}
		void ResetCurrentTag()
		{
			m_CurrentTag = new FamilyTag()
			{
				TagName = "",
				Friendships = new List<AI.FamilyFriendship>()
			};
			NameIF.text = "";
		}
		public void Init(Action onClose = null)
		{
			var options = new List<UnityEngine.UI.Dropdown.OptionData>
			{
				new UnityEngine.UI.Dropdown.OptionData("Select Tag"),
				new UnityEngine.UI.Dropdown.OptionData("Create new Tag")
			};
			for (int i = 0; i < Monsters.FamilyTags.Count; ++i)
			{
				var curTag = Monsters.FamilyTags[i];
				if (curTag.TagName == "WARRIOR" || curTag.TagName == "HIPPIE")
					continue;
				options.Add(new UnityEngine.UI.Dropdown.OptionData(curTag.TagName));
			}
			TagSelector.options = options;
			TagSelector.value = 0;
			ResetCurrentTag();
			EnableInteraction(false);
			m_SelectedOption = 0;
			m_OnClose = onClose == null ? () => { } : onClose;
			if (m_AllElements != null)
			{
				m_AllElements.Clear();
				m_AvailableElements.Clear();
			}
			else
			{
				m_AllElements = new List<CImageSelectorUI.ElementInfo>(Monsters.MonsterFamilies.Count);
				m_AvailableElements = new List<CImageSelectorUI.ElementInfo>(Monsters.MonsterFamilies.Count);
			}

			var oddElem = new CImageSelectorUI.ElementInfo()
			{
				Image = null,
				Name = "ODD"
			};
			m_AllElements.Add(oddElem);
			m_AvailableElements.Add(oddElem);

			for(int i = 1; i < Monsters.MonsterFamilies.Count; ++i)
			{
				var family = Monsters.MonsterFamilies[i];
				var elem = new CImageSelectorUI.ElementInfo()
				{
					Image = family.Frames[0],
					Name = family.Name
				};
				m_AllElements.Add(elem);
				m_AvailableElements.Add(elem);
			}
			FriendlyFamilies.Clear();
		}
		void OnTagChange(int optionID)
		{
			if (optionID == m_SelectedOption)
				return;
			if(optionID == 0) // null
			{
				EnableInteraction(false);
				m_SelectedOption = optionID;
				FriendlyFamilies.Clear();
				return;
			}
			if(optionID == 1) // create new
			{
				var defName = "unnamed_tag";
				var name = defName;
				int tries = 0;
				while(Monsters.FamilyTagDict.ContainsKey(name))
				{
					name = defName + '_' + (tries++).ToString();
				}

				var options = TagSelector.options;
				options.Add(new UnityEngine.UI.Dropdown.OptionData(name));
				m_SelectedOption = options.Count - 1;
				TagSelector.options = options;
				TagSelector.value = m_SelectedOption;

				Monsters.FamilyTagDict.Add(name, Monsters.FamilyTags.Count);
				m_CurrentTag = new FamilyTag()
				{
					TagName = name,
					Friendships = new List<AI.FamilyFriendship>()
				};
				Monsters.FamilyTags.Add(m_CurrentTag);
				NameIF.text = name;

				Monsters.UpdateUIFamilyTags();
				FriendlyFamilies.Clear();
				m_AvailableElements.Clear();
				m_AvailableElements.AddRange(m_AllElements);
				EnableInteraction(true);
			}
			else // load tag
			{
				m_SelectedOption = optionID;
				var tagName = TagSelector.options[optionID].text;
				var tag = Monsters.FamilyTags[Monsters.FamilyTagDict[tagName]];
				m_CurrentTag = new FamilyTag()
				{
					TagName = tag.TagName,
					Friendships = new List<AI.FamilyFriendship>(tag.Friendships)
				};
				NameIF.text = m_CurrentTag.TagName;
				FriendlyFamilies.Clear();
				m_AvailableElements.Clear();
				m_AvailableElements.AddRange(m_AllElements);
				var viewInfo = new List<ViewElementInfo>(m_CurrentTag.Friendships.Count);
				for(int i = 0; i < m_CurrentTag.Friendships.Count; ++i)
				{
					var friend = m_CurrentTag.Friendships[i];
					if (!friend.Friend)
						continue;
					var friendName = friend.FamilyName;
					

					for(int j = 0; j < m_AvailableElements.Count;)
					{
						if(m_AvailableElements[j].Name == friendName)
						{
							m_AvailableElements.RemoveAt(j);
						}
						else
						{
							++j;
						}
					}

					ViewElementInfo elem;
					if (friendName == "ODD")
					{
						elem = new ViewElementInfo()
						{
							Image = null,
							Text = friendName
						};
					}
					else
					{
						var family = Monsters.MonsterFamilies[Monsters.FamilyDict[friendName]];
						elem = new ViewElementInfo()
						{
							Image = family.Frames[0],
							Text = friendName
						};
					}

					
					viewInfo.Add(elem);
				}
				FriendlyFamilies.AddElement(viewInfo);
				EnableInteraction(true);
			}
		}
		void OnElementRemove2(CViewElement elem)
		{
			var name = elem.NameText != null ? elem.NameText.text : elem.TMPNameText != null ? elem.TMPNameText.text : "";
			for (int i = 0; i < m_CurrentTag.Friendships.Count; ++i)
			{
				var friend = m_CurrentTag.Friendships[i].FamilyName;
				if (friend == name)
				{
					m_CurrentTag.Friendships.RemoveAt(i);
					Monsters.FamilyTags[Monsters.FamilyTagDict[m_CurrentTag.TagName]] = m_CurrentTag;
					break;
				}
			}
			if (name == "ODD")
			{
				m_AvailableElements.Insert(0, m_AllElements[0]);
			}
			else
			{
				int idx = Monsters.FamilyDict[name];
				m_AvailableElements.Add(m_AllElements[idx]);
			}
		}
		void OnElementRemove(string elemName)
		{
			for(int i = 0; i < m_CurrentTag.Friendships.Count; ++i)
			{
				var friend = m_CurrentTag.Friendships[i].FamilyName;
				if(friend == elemName)
				{
					//m_CurrentTag.Friendships[i] = new AI.FamilyFriendship()
					//{
					//	FamilyName = elemName,
					//	Friend = false
					//};
					m_CurrentTag.Friendships.RemoveAt(i);
					Monsters.FamilyTags[Monsters.FamilyTagDict[m_CurrentTag.TagName]] = m_CurrentTag;
					break;
				}
			}
			if(elemName == "ODD")
			{
				m_AvailableElements.Insert(0, m_AllElements[0]);
			}
			else
			{
				int idx = Monsters.FamilyDict[elemName];
				m_AvailableElements.Add(m_AllElements[idx]);
			}
		}
		void OnNameChange(string name)
		{
			if (name == m_CurrentTag.TagName)
				return;
			if (Monsters.FamilyTagDict.ContainsKey(name) || name.Length == 0)
			{
				NameIF.text = m_CurrentTag.TagName;
				return; // AlreadyExists
			}

			int tagIdx = -1;
			if(Monsters.FamilyTagDict.ContainsKey(m_CurrentTag.TagName))
			{
				tagIdx = Monsters.FamilyTagDict[m_CurrentTag.TagName];
				Monsters.FamilyTagDict.Remove(m_CurrentTag.TagName);
			}
			m_CurrentTag.TagName = name;
			if(tagIdx < 0)
			{
				Monsters.FamilyTags.Add(m_CurrentTag);
				tagIdx = Monsters.FamilyTags.Count - 1;
			}
			else
			{
				Monsters.FamilyTags[tagIdx] = m_CurrentTag;
			}
			TagSelector.options[m_SelectedOption].text = name;
			TagSelector.RefreshShownValue();

			Monsters.FamilyTagDict.Add(m_CurrentTag.TagName, tagIdx);
			Monsters.UpdateUIFamilyTags();
		}
		void OnTagRemove()
		{
			var options = TagSelector.options;
			var tagName = options[m_SelectedOption].text;
			options.RemoveAt(m_SelectedOption);
			TagSelector.value = 0;

			if(Monsters.FamilyTagDict.ContainsKey(tagName))
			{
				Monsters.FamilyTags.RemoveAt(Monsters.FamilyTagDict[tagName]);
				Monsters.FamilyTagDict.Clear();
				for(int i = 0; i < Monsters.FamilyTags.Count; ++i)
				{
					Monsters.FamilyTagDict.Add(Monsters.FamilyTags[i].TagName, i);
				}
			}
			m_CurrentTag.TagName = "";
			NameIF.text = "";
			Monsters.UpdateUIFamilyTags();
		}
		void OnCrossButton()
		{
			Monsters.OnFamilyTagsUpdated();
			Monsters.SaveFamilyTags();
			gameObject.SetActive(false);
			m_OnClose();
		}
		void OnAddFamily()
		{
			ImageSelectorUI.gameObject.SetActive(true);
			ImageSelectorUI.Init(m_AvailableElements, true, OnAddFamilyEnd, Def.ImageSelectorPosition.Center);
			enabled = false;
		}
		void OnAddFamilyEnd()
		{
			enabled = true;
			ImageSelectorUI.gameObject.SetActive(false);
			var selected = ImageSelectorUI.GetSelected();
			if (selected.Count == 0)
				return;

			for(int i = 0; i < selected.Count; ++i)
			{
				var sel = selected[i];
				for(int j = 0; j < m_AvailableElements.Count; ++j)
				{
					if(m_AvailableElements[j].Name == sel)
					{
						m_AvailableElements.RemoveAt(j);
						break;
					}
				}

				//for(int j = 0; j < m_CurrentTag.Friendships.Count; ++j)
				//{
				//	if(m_CurrentTag.Friendships[j].FamilyName == selected[i])
				//	{
				//		m_CurrentTag.Friendships[j] = new AI.FamilyFriendship()
				//		{
				//			FamilyName = selected[i],
				//			Friend = true
				//		};
				//		Monsters.FamilyTags[Monsters.FamilyTagDict[m_CurrentTag.TagName]] = m_CurrentTag;
				//		break;
				//	}
				//}
				m_CurrentTag.Friendships.Add(new AI.FamilyFriendship()
				{
					FamilyName = selected[i],
					Friend = true
				});
				Monsters.FamilyTags[Monsters.FamilyTagDict[m_CurrentTag.TagName]] = m_CurrentTag;
				ViewElementInfo elem = null;
				if(sel == "ODD")
				{
					elem = new ViewElementInfo()
					{
						Image = null,
						Text = "ODD"
					};
				}
				else
				{
					elem = new ViewElementInfo()
					{
						Image = Monsters.MonsterFamilies[Monsters.FamilyDict[sel]].Frames[0],
						Text = sel
					};
				}
				FriendlyFamilies.AddElement(elem);
			}
		}
	}
}