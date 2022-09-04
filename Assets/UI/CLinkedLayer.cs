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
	public class CLinkedLayer : MonoBehaviour
	{
		const int ElementHeight = 30;
		
		public LayerEditUI LayerEdit;
		public RectTransform Content;
		public CLinkedLayerElem DefaultElement;
		public UnityEngine.UI.Button AddLayerBttn;
		public UnityEngine.UI.Button ResetChanceBttn;

		List<CLinkedLayerElem> m_Elements;

		private void Awake()
		{
			m_Elements = new List<CLinkedLayerElem>(Def.MaxLayerSlots - 1);
		}
		private void OnEnable()
		{
			if (AddLayerBttn != null)
				AddLayerBttn.interactable = true;
			if (ResetChanceBttn != null)
				ResetChanceBttn.interactable = true;
		}
		private void OnDisable()
		{
			if (AddLayerBttn != null)
				AddLayerBttn.interactable = false;
			if (ResetChanceBttn != null)
				ResetChanceBttn.interactable = false;
		}
		public void OnLayerChange()
		{
			while(m_Elements.Count > 0)
			{
				GameUtils.DeleteGameobject(m_Elements.First().gameObject);
				m_Elements.RemoveAt(0);
			}

			var layer = LayerEdit.GetCurrentEditingLayer();
			if (layer.LinkedLayers.Count == 0)
			{
				enabled = false;
				return;
			}
			var options = new List<UnityEngine.UI.Dropdown.OptionData>(Def.MaxLayerSlots);
			for (int i = 1; i < Def.MaxLayerSlots + 1; ++i)
			{
				if (i == layer.Slot || !LayerEdit.GetLayers()[i - 1].IsValid())
					continue;
				options.Add(new UnityEngine.UI.Dropdown.OptionData($"Layer {i}"));
			}
			Content.sizeDelta = new Vector2(Content.sizeDelta.x, layer.LinkedLayers.Count * ElementHeight);

			for (int i = 0; i < layer.LinkedLayers.Count; ++i)
			{
				var nElem = Instantiate(DefaultElement);
				m_Elements.Add(nElem);
				nElem.gameObject.SetActive(true);
				nElem.Slider.SetMinValue(0f);
				nElem.Slider.SetMaxValue(100f);
				nElem.Slider.SetValue(layer.LinkedLayers[i].Chance * 0.01f);
				nElem.LayerSlot = layer.LinkedLayers[i].ID;
				nElem.transform.SetParent(DefaultElement.transform.parent);
				nElem.RectTransform.anchoredPosition = new Vector2(0f, -(m_Elements.Count - 1) * ElementHeight);
				nElem.transform.localScale = Vector3.one;
				var opt = $"Layer {nElem.LayerSlot}";
				for (int j = 0; j < options.Count; ++j)
				{
					if (options[j].text == opt)
					{
						options.RemoveAt(j);
						break;
					}
				}
			}
			for (int i = 0; i < m_Elements.Count; ++i)
			{
				var opts = new List<UnityEngine.UI.Dropdown.OptionData>(options.Count + 1)
				{
					new UnityEngine.UI.Dropdown.OptionData($"Layer {m_Elements[i].LayerSlot}")
				};
				opts.AddRange(options);
				m_Elements[i].LayerDropdown.options = opts;
			}
		}
		public void OnElementChange(CLinkedLayerElem elem, int cur)
		{
			string nSlot = $"Layer {cur}";
			//string oSlot = $"Layer {elem.LayerSlot}";
			for(int i = 0; i < m_Elements.Count; ++i)
			{
				if (m_Elements[i] == elem)
				{
					//m_Elements[i].LayerDropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData(oSlot));
					continue;
				}
				var opts = m_Elements[i].LayerDropdown.options;
				int idx = m_Elements[i].LayerDropdown.value;
				for(int j = 0; j < opts.Count; ++j)
				{
					if(opts[j].text == nSlot)
					{
						if (j < idx)
							--idx;
						opts.RemoveAt(j);
						break;
					}
				}
				opts.Add(new UnityEngine.UI.Dropdown.OptionData(nSlot));
				m_Elements[i].LayerDropdown.options = opts;
				m_Elements[i].LayerDropdown.value = idx;
			}
			int linkedID = m_Elements.IndexOf(elem);
			var layer = LayerEdit.GetCurrentEditingLayer();
			layer.LinkedLayers[linkedID] = new IDChance()
			{
				ID = cur,
				Chance = layer.LinkedLayers[linkedID].Chance
			};
		}
		public void OnResetProbs()
		{
			var layer = LayerEdit.GetCurrentEditingLayer();
			var mean = (ushort)(10000 / layer.LinkedLayers.Count);
			for(int i = 0; i < layer.LinkedLayers.Count; ++i)
			{
				layer.LinkedLayers[i] = new IDChance()
				{
					ID = layer.LinkedLayers[i].ID,
					Chance = mean
				};
			}
			GameUtils.UpdateChances2(ref layer.LinkedLayers);
			for(int i = 0; i < m_Elements.Count; ++i)
			{
				m_Elements[i].Slider.SetValue(layer.LinkedLayers[i].Chance * 0.01f);
			}
		}
		public void OnElementRemove(CLinkedLayerElem elem)
		{
			var layerSlot = elem.LayerSlot;
			var idx = m_Elements.IndexOf(elem);
			m_Elements.Remove(elem);
			var nData = $"Layer {layerSlot}";

			for (int i = 0; i < m_Elements.Count; ++i)
				m_Elements[i].LayerDropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData(nData));
			Content.sizeDelta = new Vector2(Content.sizeDelta.x, m_Elements.Count * ElementHeight);

			var curLayer = LayerEdit.GetCurrentEditingLayer();
			curLayer.LinkedLayers.RemoveAt(idx);

			for(int i = 0; i < m_Elements.Count; ++i)
			{
				m_Elements[i].RectTransform.anchoredPosition = new Vector2(0f, -(ElementHeight * i));
				if (i >= idx)
					m_Elements[i].Slider.SetID(i);
			}

			OnProbChanged();
		}
		public void AddElement()
		{
			var layers = LayerEdit.GetLayers();
			var options = new List<UnityEngine.UI.Dropdown.OptionData>(layers.Length - 1);
			int firstLayer = 0;
			for(int i = 0; i < layers.Length; ++i)
			{
				if (!layers[i].IsValid())
					continue;
				if (LayerEdit.GetCurrentEditingLayer().Slot == (i + 1))
					continue;

				var cmp = $"Layer {i+1}";
				bool isLinked = false;
				for(int j = 0; j < m_Elements.Count; ++j)
				{
					var curOption = m_Elements[j].LayerDropdown.options[m_Elements[j].LayerDropdown.value];
					if(curOption.text == cmp)
					{
						isLinked = true;
						break;
					}
				}
				if (!isLinked)
				{
					if (firstLayer == 0)
						firstLayer = i + 1;
					options.Add(new UnityEngine.UI.Dropdown.OptionData(cmp));
				}
			}
			if (options.Count == 0)
				return;

			float mean = 100f / (m_Elements.Count + 1);
			var nElem = Instantiate(DefaultElement);
			m_Elements.Add(nElem);
			nElem.gameObject.SetActive(true);
			//nElem.Init();
			nElem.Slider.SetID(m_Elements.Count - 1);
			nElem.Slider.SetMinValue(0f);
			nElem.Slider.SetMaxValue(100f);
			
			nElem.Slider.SetValue(mean);
			nElem.LayerSlot = firstLayer;
			Content.sizeDelta = new Vector2(Content.sizeDelta.x, m_Elements.Count * ElementHeight);
			nElem.transform.SetParent(DefaultElement.transform.parent);
			nElem.RectTransform.anchoredPosition = new Vector2(0f, -(m_Elements.Count - 1) * ElementHeight);
			nElem.transform.localScale = Vector3.one;
			nElem.LayerDropdown.options = options;
			for(int i = 0; i < (m_Elements.Count - 1); ++i)
			{
				for(int j = 0; j < m_Elements[i].LayerDropdown.options.Count; ++j)
				{
					if(m_Elements[i].LayerDropdown.options[j].text == nElem.LayerDropdown.options[0].text)
					{
						m_Elements[i].LayerDropdown.options.RemoveAt(j);
					}
				}
			}
			var curLayer = LayerEdit.GetCurrentEditingLayer();
			curLayer.LinkedLayers.Add(new IDChance()
			{
				ID = firstLayer,
				Chance = 0
			});
			OnProbChanged(m_Elements.Count - 1);
		}
		public void OnProbChanged(int elemIdx = -1)
		{
			var layer = LayerEdit.GetCurrentEditingLayer();
			for(int i = 0; i < m_Elements.Count; ++i)
			{
				layer.LinkedLayers[i] = new IDChance()
				{
					ID = layer.LinkedLayers[i].ID,
					Chance = (ushort)Mathf.FloorToInt(m_Elements[i].Slider.Slider.value * 100f)
				};
			}
			GameUtils.UpdateChances2(ref layer.LinkedLayers, elemIdx);
			for(int i = 0; i < m_Elements.Count; ++i)
			{
				m_Elements[i].Slider.SetValue(layer.LinkedLayers[i].Chance * 0.01f);
			}
		}
	}
}
