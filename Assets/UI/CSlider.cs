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
    public class CSlider : MonoBehaviour
    {
        public UnityEngine.UI.Slider Slider;
        public UnityEngine.UI.InputField Input;

        public int ID = 0;
        Action<int, float> m_OnValueChanged;
        bool m_SettingValue = false;
        Action<string> m_OnTextChange = null;

        private void Awake()
        {
            if (Slider != null)
            {
                Slider.onValueChanged.AddListener(OnSliderChange);
            }
            if (Input != null)
            {
                //Input.onValueChanged.AddListener(OnInputChange);
                Input.onEndEdit.AddListener(OnInputChange);
            }
            if (m_OnValueChanged == null)
                m_OnValueChanged = (int _1, float _2) => { };
        }
        private void OnEnable()
        {
            if (Slider != null)
                Slider.interactable = true;
            if (Input != null)
                Input.interactable = true;
        }
        private void OnDisable()
        {
            if (Slider != null)
                Slider.interactable = false;
            if (Input != null)
                Input.interactable = false;
        }
        public void OnInputChange(string value)
        {
            if (m_SettingValue)
                return;
            if(m_OnTextChange != null)
            {
                m_OnTextChange(value);
                return;
            }    
            var parseOK = float.TryParse(value, out float nVal);
            if (!parseOK)
                return;
            m_SettingValue = true;
            bool nValChanged = false;
            if (nVal > Slider.maxValue)
            {
                nVal = Slider.maxValue;
                nValChanged = true;
            }
            if (nVal < Slider.minValue)
            {
                nVal = Slider.minValue;
                nValChanged = true;
            }
            if (nVal != Slider.value)
            {
                Slider.value = nVal;
                m_OnValueChanged(ID, nVal);
            }
            if (nValChanged)
                Input.text = nVal.ToString();
            m_SettingValue = false;
        }
        public void OnSliderChange(float value)
        {
            if (m_SettingValue)
                return;
            m_SettingValue = true;
            if(m_OnTextChange == null)
                Input.text = value.ToString();
            m_OnValueChanged(ID, value);
            m_SettingValue = false;
        }
        public void SetValue(float value)
        {
            m_SettingValue = true;
            Slider.value = value;
            if(m_OnTextChange == null)
                Input.text = value.ToString();
            m_SettingValue = false;
        }
        public float GetMinValue() => Slider.minValue;
        public float GetMaxValue() => Slider.maxValue;
        public void SetMinValue(float minValue)
        {
            var val = Slider.value;
            Slider.minValue = minValue;
            if (val < minValue)
            {
                Slider.value = minValue;
                Input.text = minValue.ToString();
                m_OnValueChanged(ID, minValue);
            }
        }
        public void SetMaxValue(float maxValue)
        {
            var val = Slider.value;
            Slider.maxValue = maxValue;
            if(val > maxValue)
            {
                Slider.value = maxValue;
                Input.text = maxValue.ToString();
                m_OnValueChanged(ID, maxValue);
            }
        }
        public void SetInteractable(bool interactable)
		{
            Slider.interactable = interactable;
            Input.interactable = interactable;
        }
        public void SetID(int id) => ID = id;
        public int GetID() => ID;
        public void SetCallback(Action<int, float> cb, Action<string> onTextChange = null)
        {
            m_OnValueChanged = cb;
            m_OnTextChange = onTextChange;
        }
    }
}
