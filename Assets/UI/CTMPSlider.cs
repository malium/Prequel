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
    public class CTMPSlider : MonoBehaviour
    {
        public UnityEngine.UI.Slider Slider;
        public TMPro.TMP_InputField Input;
        public float MinValue = 0f;
        public float MaxValue = 100f;
        public float Step = 1f;
        public float DefaultValue = 0f;

        Action<float> m_OnValueChanged;
        bool m_SettingValue = false;

        float Range() => Mathf.Abs(MaxValue - MinValue);
        float StepCount() => Range() / Step;
		private void OnValidate()
		{
            m_SettingValue = true;
            DefaultValue = Mathf.Clamp(DefaultValue, MinValue, MaxValue);
            if (Step == 0f)
                Step = 0.01f;

			if(Slider != null)
			{
                float stepsToZeroMax = Mathf.Abs(MaxValue - 0f) / Step;
                float stepsToZeroMin = Mathf.Abs(MinValue - 0f) / Step;
                Slider.maxValue = stepsToZeroMax;
                if (MinValue < 0f)
                    Slider.minValue = -stepsToZeroMin;
                else
                    Slider.minValue = stepsToZeroMin;
                Slider.wholeNumbers = true;
                Slider.value = (DefaultValue / Range()) * StepCount();
			}

            if(Input != null)
			{
                var value = Slider.value * Step;
                Input.text = value.ToString();
			}

            m_SettingValue = false;
		}
		private void Awake()
        {
            if (Slider != null)
                Slider.onValueChanged.AddListener(OnSliderChange);

            if (Input != null)
                Input.onEndEdit.AddListener(OnInputChange);
            
            if(m_OnValueChanged == null)
                SetCallback(null);
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

            var parseOK = float.TryParse(value, out float nVal);
            if (!parseOK)
                return;

            m_SettingValue = true;

            nVal = Mathf.Clamp(nVal, MinValue, MaxValue);
            var sValue = (nVal / Range()) * StepCount();
            var fValue = Mathf.Floor(sValue);
            var decimals = sValue - fValue;
            sValue = fValue;
            if (decimals > 0.5f)
                sValue += 1f;
            
            bool trigger = sValue != Slider.value;

            var rValue = sValue * Step;
            if(Input != null)
                Input.text = rValue.ToString();
            if(Slider != null)
                Slider.value = sValue;

            m_SettingValue = false;

            if (trigger)
                m_OnValueChanged(rValue);
        }
        public void OnSliderChange(float value)
        {
            if (m_SettingValue)
                return;
            m_SettingValue = true;

            var rValue = value * Step;
            if(Input != null)
                Input.text = rValue.ToString();

            m_SettingValue = false;

            m_OnValueChanged(rValue);
        }
        public float GetValue()
		{
            if(Slider != null)
			{
                return Step * Slider.value;
			}
            if(Input != null)
			{
                var parseOK = float.TryParse(Input.text, out float nVal);
                if (!parseOK)
                    return 0f;
                var sValue = (nVal / Range()) * StepCount();
                var fValue = Mathf.Floor(sValue);
                var decimals = sValue - fValue;
                sValue = fValue;
                if (decimals > 0.5f)
                    sValue += 1f;
                return sValue * Step;
            }
            return 0f;
		}
        public void SetValue(float value)
        {
            OnInputChange(value.ToString());
        }
        public float GetMinValue() => MinValue;
        public float GetMaxValue() => MaxValue;
        public void Set(float minValue, float maxValue, float step, float defaultValue)
		{
            MinValue = minValue;
            MaxValue = maxValue;
            Step = step;
            DefaultValue = defaultValue;
            OnValidate();
        }
        public void SetCallback(Action<float> cb)
        {
            m_OnValueChanged = cb;
            if(m_OnValueChanged == null)
                m_OnValueChanged = (float _) => { };
        }
    }
}
