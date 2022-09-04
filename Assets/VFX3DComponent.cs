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
    [Serializable]
    public struct VFX3DDef
    {
        public int VFXTypeID;
        public Def.VFXType VFXType;
        public int VFXVersion;
        public GameObject Object3D;
        public Def.VFXEnd OnEnd;
        public float FPSOverride;

        public VFX3DDef(string typeName, Def.VFXType type, int vfxVersion, GameObject object3D, Def.VFXEnd onEnd = Def.VFXEnd.SelfDestroy, float fpsOverride = 0)
        {
            VFXTypeID = VFX3Ds.FamilyDict[typeName];
            VFXType = type;
            VFXVersion = vfxVersion;
            Object3D = object3D;
            OnEnd = onEnd;
            FPSOverride = fpsOverride;
        }

        public VFX3DDef(int VFXTypeID, Def.VFXType type, int vfxVersion, GameObject object3D, Def.VFXEnd onEnd = Def.VFXEnd.SelfDestroy, float fpsOverride = 0)
        {
            this.VFXTypeID = VFXTypeID;
            VFXType = type;
            VFXVersion = vfxVersion;
            Object3D = object3D;
            OnEnd = onEnd;
            FPSOverride = fpsOverride;
        }

        public bool IsValid()
        {
            if (VFXTypeID < 0)
                return false;

            if (VFXType == Def.VFXType.COUNT)
                return false;

            if (VFXVersion < 0)
                return false;

            if (Object3D == null)
                return false;

            return true;
        }
    }

    class VFX3DComponent : MonoBehaviour
    {
        [SerializeField]
        VFX3DInfoDef m_VFXInfo;
        [SerializeField]
        VFX3DDef m_VFXDef;
        [SerializeField]
        MeshRenderer m_Renderer;
        [SerializeField]
        MeshFilter m_Mesh;
        [SerializeField]
        protected float m_WaitTime;
        [SerializeField]

        protected int m_CurrentFrame;
        protected bool m_Stopped;
        float m_LastFrameChange;

        public float WaitTime
        {
            get
            {
                return m_WaitTime;
            }
        }
        public bool Stopped
        {
            get
            {
                return m_Stopped;
            }
        }
        public VFX3DInfoDef VFXInfo
        {
            get
            {
                return m_VFXInfo;
            }
        }
        public VFX3DDef Definition
        {
            get
            {
                return m_VFXDef;
            }
        }
        public MeshRenderer Renderer
        {
            get
            {
                return m_Renderer;
            }
        }

        public void SetVFX(VFX3DDef vfxDef)
        {
            m_VFXDef = vfxDef;

            var objRenderer = m_VFXDef.Object3D.GetComponent<MeshRenderer>();
            var objMesh = m_VFXDef.Object3D.GetComponent<MeshFilter>();


            m_Renderer = gameObject.GetComponent<MeshRenderer>();
            if (m_Renderer == null)
                m_Renderer = gameObject.AddComponent<MeshRenderer>();
            m_Mesh = gameObject.GetComponent<MeshFilter>();
            if (m_Mesh == null)
                m_Mesh = gameObject.AddComponent<MeshFilter>();
            m_Mesh.mesh = objMesh.mesh;
            m_Renderer.material = new Material(objRenderer.material);

            var vfxFamily = VFX3Ds.VFXFamilies[m_VFXDef.VFXTypeID];
            switch (m_VFXDef.VFXType)
            {
                case Def.VFXType.CAST:
                    m_VFXInfo = vfxFamily.CastVFX[m_VFXDef.VFXVersion];
                    break;
                case Def.VFXType.TRAVEL:
                    m_VFXInfo = vfxFamily.TravelVFX[m_VFXDef.VFXVersion];
                    break;
                case Def.VFXType.ONHIT:
                    m_VFXInfo = vfxFamily.OnHitVFX[m_VFXDef.VFXVersion];
                    break;
                case Def.VFXType.COUNT:
                    throw new Exception("Trying to set a VFXComponent with an invalid Def.VFXType.");
            }

            //var typeInfo = VFX3Ds.VFXFamilies[m_VFXDef.Def.VFXTypeID];
            //int vfxID = -1;
            //switch (m_VFXDef.Def.VFXType)
            //{
            //    case Def.VFXType.CAST:
            //        vfxID = typeInfo.CastVFX[m_VFXDef.VFXVersion];
            //        break;
            //    case Def.VFXType.ONHIT:
            //        vfxID = typeInfo.OnHitVFX[m_VFXDef.VFXVersion];
            //        break;
            //    case Def.VFXType.TRAVEL:
            //        vfxID = typeInfo.TravelVFX[m_VFXDef.VFXVersion];
            //        break;
            //}
            //m_VFXInfo = VFX3Ds.VFXInfos[vfxID];

            m_Renderer.material.SetTexture(Def.MaterialTextureID, m_VFXInfo.Frames[0]);

            m_CurrentFrame = 0;
            if (m_VFXDef.FPSOverride > 0.0f)
            {
                m_WaitTime = 1.0f / m_VFXDef.FPSOverride;
            }
            else
            {
                m_WaitTime = 1.0f / m_VFXInfo.FramesPerSec;
            }
            m_LastFrameChange = 0.0f;
        }

        public void StopVFX()
        {
            m_CurrentFrame = 0;
            m_LastFrameChange = Time.time + 30000.0f;
            m_Renderer.enabled = false;
            m_Stopped = true;
        }

        public void ResetVFX(float offsetTime = 0.0f)
        {
            m_CurrentFrame = 0;
            m_LastFrameChange = Time.time + offsetTime;
            m_Renderer.material.SetTexture(Def.MaterialTextureID, m_VFXInfo.Frames[m_CurrentFrame]);
            m_Renderer.enabled = false;
            m_Stopped = false;
        }

        protected void OnUpdate()
        {
            if (m_LastFrameChange < Time.time)
            {
                m_Renderer.enabled = true;
                ++m_CurrentFrame;
                if (m_CurrentFrame == m_VFXInfo.Frames.Length)
                {
                    switch (m_VFXDef.OnEnd)
                    {
                        case Def.VFXEnd.Stop:
                            StopVFX();
                            break;
                        case Def.VFXEnd.SelfDestroy:
                            GameObject.Destroy(gameObject);
                            break;
                        case Def.VFXEnd.Repeat:
                            ResetVFX();
                            break;
                    }
                    return;
                }
                m_Renderer.material.SetTexture(Def.MaterialTextureID, m_VFXInfo.Frames[m_CurrentFrame]);
                m_LastFrameChange = Time.time + m_WaitTime;
            }
        }

        private void Update()
        {
            OnUpdate();
        }
    }
}
