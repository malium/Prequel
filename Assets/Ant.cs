/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public class AntComponent : MonoBehaviour
    {
        [SerializeField]
        AntDef m_Def;
        [SerializeField]
        SpriteRenderer m_Renderer;
        public SpriteRenderer Renderer
        {
            get
            {
                return m_Renderer;
            }
        }
        int m_CurrentFrame;
        int m_AntState;
        public int AntState
        {
            get
            {
                return m_AntState;
            }
        }

        public AntDef Def
        {
            get
            {
                return m_Def;
            }
        }

        public void SetAnt(AntDef def, int antState)
        {
            m_Def = def;
            m_AntState = antState;
            m_CurrentFrame = AntManager.Mgr.GetCurrentFrame();
            m_Renderer = gameObject.AddComponent<SpriteRenderer>();
            //m_Renderer.flipX = Manager.Mgr.SpawnRNG.NextDouble() > 0.5;
            m_Renderer.sprite = m_Def.Frames[m_CurrentFrame];
        }

        private void Update()
        {
            var frame = AntManager.Mgr.GetCurrentFrame();
            if (m_CurrentFrame != frame)
            {
                m_CurrentFrame = frame;
                m_Renderer.sprite = m_Def.Frames[frame];
            }
        }
    }
}
