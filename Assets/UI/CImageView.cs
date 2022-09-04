/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.UI
{
	public class CImageView : MonoBehaviour
	{
		const int ElementWidth = 160;
		const int ElementHeight = 150;
		const int RowElementCount = 3;
		public UnityEngine.UI.Button AddElemBttn;
		public UnityEngine.UI.Button ResetChancesBttn;
		public RectTransform ViewContent;
		public CImageElement ElementToCopy;
		public CImageSelectorUI SelectorUI;
		public LayerEditUI LayerEdit;

		public enum ImageViewType
		{
			Floor,
			Props,
			Monsters,
		}
		public ImageViewType ViewType;
		struct Element
		{
			public Sprite Image;
			public string Name;
			public CImageElement Comp;
		}

		List<CImageSelectorUI.ElementInfo> m_AllElements;
		List<CImageSelectorUI.ElementInfo> m_AvailableElements;
		List<Element> m_CurrentElements;
		int m_RowIdx;
		int m_ColIdx;

		private void OnEnable()
		{
			if (AddElemBttn != null)
				AddElemBttn.interactable = true;
			if (ResetChancesBttn != null)
				ResetChancesBttn.interactable = true;
			if (m_CurrentElements != null)
			{
				for (int i = 0; i < m_CurrentElements.Count; ++i)
				{
					var el = m_CurrentElements[i].Comp;
					el.CrossBttn.interactable = true;
					el.Slider.enabled = true;
				}
			}
		}
		private void OnDisable()
		{
			if (AddElemBttn != null)
				AddElemBttn.interactable = false;
			if (ResetChancesBttn != null)
				ResetChancesBttn.interactable = false;
			if (m_CurrentElements != null)
			{
				for (int i = 0; i < m_CurrentElements.Count; ++i)
				{
					var el = m_CurrentElements[i].Comp;
					el.CrossBttn.interactable = false;
					el.Slider.enabled = false;
				}
			}
		}
		public void Init()
		{
			switch (ViewType)
			{
				case ImageViewType.Floor:
					m_AllElements = new List<CImageSelectorUI.ElementInfo>(BlockMaterial.MaterialFamilies.Count - 1);
					//m_AvailableElements = new List<CImageSelectorUI.ElementInfo>(m_AllElements.Capacity);
					for (int i = 1; i < BlockMaterial.MaterialFamilies.Count; ++i)
					{
						var matFamily = BlockMaterial.MaterialFamilies[i];
						var texture = (Texture2D)matFamily.NormalMaterials[0].TopPart.Mat.GetTexture(Def.MaterialTextureID);
						if (texture == null)
							texture = (Texture2D)matFamily.NormalMaterials[0].TopPart.Mat.GetTexture(Def.ColoredMaterialTextureID);
						var sprite =
							Sprite.Create(texture,
							new Rect(0f, 0f, texture.width, texture.height),
							new Vector2(0f, 0f), 100f, 0, SpriteMeshType.FullRect);
						var elem = new CImageSelectorUI.ElementInfo()
						{
							Image = sprite,
							Name = matFamily.FamilyInfo.FamilyName
						};
						m_AllElements.Add(elem);
						//m_AvailableElements.Add(elem);
					}
					break;
				case ImageViewType.Props:
					m_AllElements = new List<CImageSelectorUI.ElementInfo>(Props.PropFamilies.Count - 1);
					//m_AvailableElements = new List<CImageSelectorUI.ElementInfo>(m_AllElements.Capacity);
					for (int i = 1; i < Props.PropFamilies.Count; ++i)
					{
						var family = Props.PropFamilies[i];
						var elem = new CImageSelectorUI.ElementInfo()
						{
							Image = family.Props[0].PropSprite,
							Name = family.FamilyName
						};
						m_AllElements.Add(elem);
						//m_AvailableElements.Add(elem);
					}
					break;
				case ImageViewType.Monsters:
					m_AllElements = new List<CImageSelectorUI.ElementInfo>(Monsters.MonsterFamilies.Count - 1);
					//m_AvailableElements = new List<CImageSelectorUI.ElementInfo>(m_AllElements.Capacity);
					for (int i = 1; i < Monsters.MonsterFamilies.Count; ++i)
					{
						var info = Monsters.MonsterFamilies[i];
						var elem = new CImageSelectorUI.ElementInfo()
						{
							Image = info.Frames[0],
							Name = info.Name
						};
						m_AllElements.Add(elem);
						//m_AvailableElements.Add(elem);
					}
					break;
			}
			if (m_CurrentElements != null)
			{
				for (int i = 0; i < m_CurrentElements.Count; ++i)
				{
					GameUtils.DeleteGameobject(m_CurrentElements[i].Comp.gameObject);
				}
				m_CurrentElements.Clear();
			}
			else
			{
				m_CurrentElements = new List<Element>();
			}
		}
		public void OnLayerChange()
		{
			if (m_AllElements == null)
				Init();

			for (int i = 0; i < m_CurrentElements.Count; ++i)
			{
				GameUtils.DeleteGameobject(m_CurrentElements[i].Comp.gameObject);
			}
			m_CurrentElements.Clear();
			var layer = LayerEdit.GetCurrentEditingLayer();
			List<NameChance> layerElements = null;
			switch (ViewType)
			{
				case ImageViewType.Floor:
					layerElements = new List<NameChance>(layer.MaterialFamilies.Count);
					for (int i = 0; i < layer.MaterialFamilies.Count; ++i)
					{
						layerElements.Add(new NameChance()
						{
							ID = BlockMaterial.MaterialFamilies[layer.MaterialFamilies[i].ID].FamilyInfo.FamilyName,
							Chance = layer.MaterialFamilies[i].Chance
						});
					}
					break;
				case ImageViewType.Props:
					layerElements = new List<NameChance>(layer.PropFamilies.Count);
					for (int i = 0; i < layer.PropFamilies.Count; ++i)
					{
						layerElements.Add(new NameChance()
						{
							ID = Props.PropFamilies[layer.PropFamilies[i].ID].FamilyName,
							Chance = layer.PropFamilies[i].Chance
						});
					}
					break;
				case ImageViewType.Monsters:
					layerElements = new List<NameChance>(layer.MonsterFamilies.Count);
					for (int i = 0; i < layer.MonsterFamilies.Count; ++i)
					{
						layerElements.Add(new NameChance()
						{
							ID = Monsters.MonsterFamilies[layer.MonsterFamilies[i].ID].Name,
							Chance = layer.MonsterFamilies[i].Chance
						});
					}
					break;
			}

			m_AvailableElements = new List<CImageSelectorUI.ElementInfo>(m_AllElements);

			if (layerElements.Count > m_CurrentElements.Capacity)
				m_CurrentElements.Capacity = layerElements.Count;
			//m_CurrentElements = new List<Element>(layerElements.Count);
			m_RowIdx = 0;
			m_ColIdx = 0;
			for(int i = 0; i < layerElements.Count; ++i)
			{
				var nElem = Instantiate(ElementToCopy);
				CImageSelectorUI.ElementInfo elemInfo = m_AllElements[0];
				for(int j = 0; j < m_AllElements.Count; ++j)
				{
					if(m_AllElements[j].Name == layerElements[i].ID)
					{
						elemInfo = m_AllElements[j];
						m_AvailableElements.Remove(elemInfo);
						break;
					}
				}
				nElem.gameObject.SetActive(true);
				nElem.SetElement(elemInfo, layerElements[i].Chance);
				nElem.transform.SetParent(ElementToCopy.transform.parent);
				if(m_ColIdx == RowElementCount)
				{
					++m_RowIdx;
					m_ColIdx = 0;
					ViewContent.sizeDelta = new Vector2(ViewContent.sizeDelta.x, (1 + m_RowIdx) * ElementHeight);
				}
				nElem.Transform.anchoredPosition = new Vector2(m_ColIdx * ElementWidth, -m_RowIdx * ElementHeight);
				nElem.transform.localScale = new Vector3(1f, 1f, 1f);
				m_CurrentElements.Add(new Element()
				{
					Comp = nElem,
					Image = elemInfo.Image,
					Name = elemInfo.Name
				});
				++m_ColIdx;
			}
		}
		public void OnChanceReset()
		{
			var layer = LayerEdit.GetCurrentEditingLayer();
			List<IDChance> chances = null;
			switch (ViewType)
			{
				case ImageViewType.Floor:
					for(int i = 0; i < layer.MaterialFamilies.Count; ++i)
					{
						var idc = layer.MaterialFamilies[i];
						idc.Chance = (ushort)(10000f / layer.MaterialFamilies.Count);
						layer.MaterialFamilies[i] = idc;
					}
					GameUtils.UpdateChances2(ref layer.MaterialFamilies);
					chances = layer.MaterialFamilies;
					break;
				case ImageViewType.Props:
					for (int i = 0; i < layer.PropFamilies.Count; ++i)
					{
						var idc = layer.PropFamilies[i];
						idc.Chance = (ushort)(10000f / layer.PropFamilies.Count);
						layer.PropFamilies[i] = idc;
					}
					GameUtils.UpdateChances2(ref layer.PropFamilies);
					chances = layer.PropFamilies;
					break;
				case ImageViewType.Monsters:
					for (int i = 0; i < layer.MonsterFamilies.Count; ++i)
					{
						var idc = layer.MonsterFamilies[i];
						idc.Chance = (ushort)(10000f / layer.MonsterFamilies.Count);
						layer.MonsterFamilies[i] = idc;
					}
					GameUtils.UpdateChances2(ref layer.MonsterFamilies);
					chances = layer.MonsterFamilies;
					break;
			}
			for(int i = 0; i < m_CurrentElements.Count; ++i)
			{
				m_CurrentElements[i].Comp.Slider.SetValue(chances[i].Chance * 0.01f);
			}
		}
		public void OnElementAdd()
		{
			SelectorUI.gameObject.SetActive(true);
			SelectorUI.Init(m_AvailableElements, true, OnElementAddEnd, Def.ImageSelectorPosition.Left);
		}
		public void OnElementAddEnd()
		{
			SelectorUI.gameObject.SetActive(false);
			var selected = SelectorUI.GetSelected();
			for(int i = 0; i < selected.Count; ++i)
			{
				var current = new CImageSelectorUI.ElementInfo();
				for(int j = 0; j < m_AvailableElements.Count; ++j)
				{
					if(m_AvailableElements[j].Name == selected[i])
					{
						current = m_AvailableElements[j];
						m_AvailableElements.RemoveAt(j);
						break;
					}
				}
				var elem = Instantiate(ElementToCopy);
				ushort mean = (ushort)Mathf.FloorToInt(100f * (100f / (m_CurrentElements.Count + 1)));
				var layer = LayerEdit.GetCurrentEditingLayer();
				int id;
				switch (ViewType)
				{
					case ImageViewType.Floor:
						id = BlockMaterial.FamilyDict[current.Name];
						layer.MaterialFamilies.Add(new IDChance()
						{
							ID = id,
							Chance = mean,
						});
						break;
					case ImageViewType.Props:
						id = Props.FamilyDict[current.Name];
						layer.PropFamilies.Add(new IDChance()
						{
							ID = id,
							Chance = mean,
						});
						break;
					case ImageViewType.Monsters:
						id = Monsters.FamilyDict[current.Name];
						layer.MonsterFamilies.Add(new IDChance()
						{
							ID = id,
							Chance = mean
						});
						break;
				}
				m_CurrentElements.Add(new Element()
				{
					Comp = elem,
					Image = current.Image,
					Name = current.Name
				});
				elem.gameObject.SetActive(true);
				elem.SetElement(current, mean);
				elem.Slider.SetID(m_CurrentElements.Count - 1);
				elem.transform.SetParent(ElementToCopy.transform.parent);
				if(m_ColIdx == RowElementCount)
				{
					m_ColIdx = 0;
					++m_RowIdx;
					ViewContent.sizeDelta = 
						new Vector2(ViewContent.sizeDelta.x, (1 + m_RowIdx) * ElementHeight);
				}
				elem.Transform.anchoredPosition = 
					new Vector2(m_ColIdx * ElementWidth, -m_RowIdx * ElementHeight);
				elem.transform.localScale = new Vector3(1f, 1f, 1f);
				++m_ColIdx;
			}
			OnProbChange(m_CurrentElements.Count - 1);
		}
		public void OnProbChange(int elemIdx = -1)
		{
			var layer = LayerEdit.GetCurrentEditingLayer();
			for(int i = 0; i < m_CurrentElements.Count; ++i)
			{
				var curEl = m_CurrentElements[i];
				ushort prob = (ushort)Mathf.FloorToInt(curEl.Comp.Slider.Slider.value * 100f);
				switch (ViewType)
				{
					case ImageViewType.Floor:
						layer.MaterialFamilies[i] = new IDChance()
						{
							ID = layer.MaterialFamilies[i].ID,
							Chance = prob,
						};
						break;
					case ImageViewType.Props:
						layer.PropFamilies[i] = new IDChance()
						{
							ID = layer.PropFamilies[i].ID,
							Chance = prob
						};
						break;
					case ImageViewType.Monsters:
						layer.MonsterFamilies[i] = new IDChance()
						{
							ID = layer.MonsterFamilies[i].ID,
							Chance = prob
						};
						break;
				}
			}
			switch (ViewType)
			{
				case ImageViewType.Floor:
					GameUtils.UpdateChances2(ref layer.MaterialFamilies, elemIdx);
					break;
				case ImageViewType.Props:
					GameUtils.UpdateChances2(ref layer.PropFamilies, elemIdx);
					break;
				case ImageViewType.Monsters:
					GameUtils.UpdateChances2(ref layer.MonsterFamilies, elemIdx);
					break;
			}
			for(int i = 0; i < m_CurrentElements.Count; ++i)
			{
				var curEl = m_CurrentElements[i];
				ushort prob = 0;
				switch (ViewType)
				{
					case ImageViewType.Floor:
						prob = layer.MaterialFamilies[i].Chance;
						break;
					case ImageViewType.Props:
						prob = layer.PropFamilies[i].Chance;
						break;
					case ImageViewType.Monsters:
						prob = layer.MonsterFamilies[i].Chance;
						break;
				}
				curEl.Comp.Slider.SetValue(prob * 0.01f);
			}
		}
		public void OnElementRemove(string elemID)
		{
			var layer = LayerEdit.GetCurrentEditingLayer();
			int elemIdx = -1;
			for(int i = 0; i < m_CurrentElements.Count; ++i)
			{
				if (m_CurrentElements[i].Name != elemID)
					continue;

				switch (ViewType)
				{
					case ImageViewType.Floor:
						layer.MaterialFamilies.RemoveAt(i);
						break;
					case ImageViewType.Props:
						layer.PropFamilies.RemoveAt(i);
						break;
					case ImageViewType.Monsters:
						layer.MonsterFamilies.RemoveAt(i);
						break;
				}
				var elem = m_CurrentElements[i];
				var listElem = new CImageSelectorUI.ElementInfo()
				{
					Image = elem.Image,
					Name = elem.Name
				};
				var elemListIdx = m_AllElements.IndexOf(listElem);
				m_AvailableElements.Insert(elemListIdx, listElem);
				GameUtils.DeleteGameobject(elem.Comp.gameObject);
				m_CurrentElements.RemoveAt(i);
				elemIdx = i;
				break;
			}
			m_RowIdx = 0;
			m_ColIdx = 0;
			for(int i = 0; i < m_CurrentElements.Count; ++i)
			{
				var elem = m_CurrentElements[i].Comp;
				if(i >= elemIdx)
				{
					elem.Slider.SetID(i);
				}
				if(m_ColIdx == RowElementCount)
				{
					m_ColIdx = 0;
					++m_RowIdx;
				}
				elem.Transform.anchoredPosition = new Vector2(m_ColIdx * ElementWidth, -m_RowIdx * ElementHeight);
				++m_ColIdx;
			}
			ViewContent.sizeDelta = new Vector2(ViewContent.sizeDelta.x, (1 + m_RowIdx) * ElementHeight);

			switch (ViewType)
			{
				case ImageViewType.Floor:
					GameUtils.UpdateChances2(ref layer.MaterialFamilies);
					break;
				case ImageViewType.Props:
					GameUtils.UpdateChances2(ref layer.PropFamilies);
					break;
				case ImageViewType.Monsters:
					GameUtils.UpdateChances2(ref layer.MonsterFamilies);
					break;
			}
			for(int i = 0; i < m_CurrentElements.Count; ++i)
			{
				var elem = m_CurrentElements[i];
				float prob = 0f;
				switch (ViewType)
				{
					case ImageViewType.Floor:
						prob = layer.MaterialFamilies[i].Chance * 0.01f;
						break;
					case ImageViewType.Props:
						prob = layer.PropFamilies[i].Chance * 0.01f;
						break;
					case ImageViewType.Monsters:
						prob = layer.MonsterFamilies[i].Chance * 0.01f;
						break;
				}
				elem.Comp.Slider.Slider.value = prob;
			}
		}
		public void SetInteractable(bool interactable)
		{
			AddElemBttn.interactable = interactable;
			ResetChancesBttn.interactable = interactable;
			for(int i = 0; i < m_CurrentElements.Count; ++i)
			{
				var elem = m_CurrentElements[i];
				elem.Comp.CrossBttn.interactable = interactable;
				elem.Comp.Slider.SetInteractable(interactable);
			}
		}
	}
}