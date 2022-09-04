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
    public class PossessionVFXComponent : MonoBehaviour
    {
        const int BigWaitFrames = 2;
        const int SmallWaitFrames = 6;
        static readonly int AnimHash = Animator.StringToHash("Layer.NormalState");
        //static readonly int StopHash = Animator.StringToHash("Layer.StopState");

        [SerializeField]
        VFX3DComponent m_BigCircle = null;
        [SerializeField]
        VFX3DComponent m_SmallCircle = null;

        [SerializeField]
        Animator m_Animator = null;
        
        public void Set()
        {
            m_BigCircle.SetVFX(new VFX3DDef("CircleBig", Def.VFXType.CAST, 0, m_BigCircle.gameObject, Def.VFXEnd.SelfDestroy, 24.0f));
            m_SmallCircle.SetVFX(new VFX3DDef("CircleSmall", Def.VFXType.CAST, 0, m_SmallCircle.gameObject, Def.VFXEnd.SelfDestroy, 24.0f));
            ResetVFX();
        }

        public void ResetVFX()
        {
            m_BigCircle.ResetVFX(BigWaitFrames * m_BigCircle.WaitTime);
            m_SmallCircle.ResetVFX(SmallWaitFrames * m_SmallCircle.WaitTime);
            m_Animator.Play(AnimHash);
        }

        public void Update()
        {
            if(m_BigCircle == null && m_SmallCircle == null)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
