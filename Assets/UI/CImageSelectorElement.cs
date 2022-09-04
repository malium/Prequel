/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI
{
    public class CImageSelectorElement : MonoBehaviour
    {
        public UnityEngine.UI.Toggle SelectionToggle;
        public UnityEngine.UI.Button SelectionButton;
        public UnityEngine.UI.Image SpriteImage;
        public UnityEngine.UI.Text SpriteText;
        public RectTransform Transform;

        public CImageSelectorUI SelectorUI;

        CImageSelectorUI.ElementInfo m_Info;

        public void SetElement(CImageSelectorUI.ElementInfo info)
        {
            m_Info = info;
            SpriteImage.sprite = m_Info.Image;
            SpriteText.text = m_Info.Name;
            if(SelectionToggle != null)
            {
                SelectionToggle.onValueChanged.AddListener(OnToggleSelect);
            }
            else if(SelectionButton != null)
            {
                SelectionButton.onClick.AddListener(OnSelected);
            }
        }

        void OnSelected()
        {
            SelectorUI.OnElementSelected(m_Info.Name);
        }
        void OnDeselected()
        {
            SelectorUI.OnElementDeselected(m_Info.Name);
        }

        public void OnToggleSelect(bool val)
        {
            if (val)
                OnSelected();
            else
                OnDeselected();
        }
    }
}