/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI
{
    public class CImageElement : MonoBehaviour
    {
        public UnityEngine.UI.Image ImageSprite;
        public UnityEngine.UI.Text ImageText;
        public CSlider Slider;
        public UnityEngine.UI.Button CrossBttn;
        public CImageView ImageView;
        public RectTransform Transform;

        CImageSelectorUI.ElementInfo m_Info;

        public void SetElement(CImageSelectorUI.ElementInfo info, ushort probability)
        {
            m_Info = info;
            float prob = (float)probability * 0.01f;
            Slider.SetMinValue(0f);
            Slider.SetMaxValue(100f);
            Slider.SetValue(prob);
            Slider.SetCallback(OnProbChange);
            ImageSprite.sprite = m_Info.Image;
            ImageText.text = m_Info.Name;
            CrossBttn.onClick.AddListener(OnRemove);
        }

        public void OnProbChange(int id, float _2)
        {
            ImageView.OnProbChange(id);
        }

        public void OnRemove()
        {
            ImageView.OnElementRemove(m_Info.Name);
        }
    }
}