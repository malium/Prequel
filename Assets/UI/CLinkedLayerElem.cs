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
    public class CLinkedLayerElem : MonoBehaviour
    {
        public CSlider Slider;
        public UnityEngine.UI.Button CrossButton;
        public UnityEngine.UI.Dropdown LayerDropdown;
        public CLinkedLayer LinkedLayer;
        public RectTransform RectTransform;

        public int LayerSlot;
        bool m_IsDropdownChanging;

        private void Awake()
        {
            Slider.SetCallback(OnProbChanged);
            CrossButton.onClick.AddListener(OnRemove);
            m_IsDropdownChanging = false;
        }

        public void OnDropdownChanged()
        {
            if (m_IsDropdownChanging)
                return;
            m_IsDropdownChanging = true;

            var slotStr = LayerDropdown.options[LayerDropdown.value];
            var slotValueStr = slotStr.text.Substring(6);
            int nSlot = int.Parse(slotValueStr);
            LinkedLayer.OnElementChange(this, nSlot);
            LayerSlot = nSlot;
            int cur = LayerDropdown.value;
            m_IsDropdownChanging = false;
        }

        public void OnProbChanged(int id, float value)
        {
            LinkedLayer.OnProbChanged(id);
        }

        public void OnRemove()
        {
            LinkedLayer.OnElementRemove(this);
            GameUtils.DeleteGameobject(gameObject);
        }
    }
}
