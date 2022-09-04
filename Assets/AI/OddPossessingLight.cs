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

namespace Assets
{
    public class OddPossessingLight : MonoBehaviour
    {
        [SerializeField]
        Light m_Light;
        public Light Light
        {
            get
            {
                return m_Light;
            }
        }
        float m_StartTime;

        const float MaxIntensity = 2f;

        public void Set()
        {
            m_Light = gameObject.AddComponent<Light>();
            m_Light.type = LightType.Point;
            m_Light.range = 10f;
            m_Light.intensity = 0f;
            m_Light.color = new Color(0f, 1f, 96f / 255f);
            m_StartTime = Time.time;
        }

        private void Update()
        {
            var t = Time.time - m_StartTime;
            if(t < 0.1f)
            {
                t = t / 0.1f;
                m_Light.intensity = (1f - t) * 0f + t * MaxIntensity;
            }
            else if(t < 0.3f)
            {
                t = t - 0.1f;
                t = t / 0.2f;
                m_Light.intensity = (1f - t) * MaxIntensity + t * 0.5f * MaxIntensity;
            }
            else if(t < 0.4f)
            {
                t = t - 0.3f;
                t = t / 0.1f;
                m_Light.intensity = (1f - t) * 0.5f * MaxIntensity + t * 0.75f * MaxIntensity;
            }
            else if(t < 0.7f)
            {
                t = t - 0.4f;
                t = t / 0.3f;
                m_Light.intensity = (1f - t) * 0.75f * MaxIntensity;
            }
            else
            {
                GameUtils.DeleteGameobject(gameObject);
            }
        }
    }
}
