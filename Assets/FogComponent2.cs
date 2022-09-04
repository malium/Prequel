/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FogComponent2 : MonoBehaviour
{
    VisualEffect Effect;
    static readonly int FogDistanceID = Shader.PropertyToID("FogDistance");
    const float FogThreshold = 25.0f;

    float m_FogDistance;
    public float FogDistance
    {
        get
        {
            return m_FogDistance;
        }
        set
        {
            if (m_FogDistance == value)
                return;

            if(value > FogThreshold)
            {
                Effect.enabled = false;
                m_FogDistance = value;
                return;
            }
            else
            {
                Effect.enabled = true;
            }

            var curve = Effect.GetAnimationCurve(FogDistanceID);
            for (int i = 0; i < curve.keys.Length; ++i)
                curve.keys[i].value = value;

            Effect.SetAnimationCurve(FogDistanceID, curve);
            m_FogDistance = value;
        }
    }

    private void Awake()
    {
        Effect = gameObject.GetComponent<VisualEffect>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
