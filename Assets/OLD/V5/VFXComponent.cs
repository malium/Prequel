/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets
//{
//    public class VFXComponent : MonoBehaviour
//    {
//        //[SerializeField]
//        //VFXInfo m_VFXInfo;
//        [SerializeField]
//        VFXInfoDef m_VFXInfo;
//        [SerializeField]
//        VFXDef m_VFXDef;
//        [SerializeField]
//        SpriteRenderer m_Renderer;
//        [SerializeField]
//        protected float m_WaitTime;
//        [SerializeField]

//        protected int m_CurrentFrame;
//        protected bool m_Stopped;
//        float m_LastFrameChange;

//        public float WaitTime
//        {
//            get
//            {
//                return m_WaitTime;
//            }
//        }
//        public bool Stopped
//        {
//            get
//            {
//                return m_Stopped;
//            }
//        }
//        public VFXInfoDef VFXInfo
//        {
//            get
//            {
//                return m_VFXInfo;
//            }
//        }
//        //public VFXInfo VFXInfo
//        //{
//        //    get
//        //    {
//        //        return m_VFXInfo;
//        //    }
//        //}
//        public VFXDef Definition
//        {
//            get
//            {
//                return m_VFXDef;
//            }
//        }
//        public SpriteRenderer Renderer
//        {
//            get
//            {
//                return m_Renderer;
//            }
//        }

//        public void SetVFX(VFXDef vfxDef)
//        {
//            m_VFXDef = vfxDef;

//            m_Renderer = gameObject.GetComponent<SpriteRenderer>();
//            if (m_Renderer == null)
//                m_Renderer = gameObject.AddComponent<SpriteRenderer>();
//            m_Renderer.material = Materials.GetMaterial(Def.Materials.Sprite);
//            var vfxFamily = VFXs.VFXFamilies[(int)m_VFXDef.VfxTarget][m_VFXDef.VFXTypeID];
//            switch (m_VFXDef.VFXType)
//            {
//                case Def.VFXType.CAST:
//                    m_VFXInfo = vfxFamily.CastVFX[m_VFXDef.VFXVersion];
//                    break;
//                case Def.VFXType.TRAVEL:
//                    m_VFXInfo = vfxFamily.TravelVFX[m_VFXDef.VFXVersion];
//                    break;
//                case Def.VFXType.ONHIT:
//                    m_VFXInfo = vfxFamily.OnHitVFX[m_VFXDef.VFXVersion];
//                    break;
//                case Def.VFXType.COUNT:
//                    throw new Exception("Trying to set a VFXComponent with an invalid Def.VFXType.");
//            }
//            //var typeInfo = VFXs.Def.VFXTypeInfos[(int)m_VFXDef.VfxTarget][m_VFXDef.Def.VFXTypeID];
//            //int vfxID = -1;
//            //switch (m_VFXDef.Def.VFXType)
//            //{
//            //    case Def.VFXType.CAST:
//            //        vfxID = typeInfo.CastVFX[m_VFXDef.VFXVersion];
//            //        break;
//            //    case Def.VFXType.ONHIT:
//            //        vfxID = typeInfo.OnHitVFX[m_VFXDef.VFXVersion];
//            //        break;
//            //    case Def.VFXType.TRAVEL:
//            //        vfxID = typeInfo.TravelVFX[m_VFXDef.VFXVersion];
//            //        break;
//            //}
//            //m_VFXInfo = VFXs.VFXInfos[(int)m_VFXDef.VfxTarget][vfxID];
//            m_Renderer.sprite = m_VFXInfo.Frames[0];
//            m_CurrentFrame = 0;
//            if (m_VFXDef.FPSOverride > 0.0f)
//            {
//                m_WaitTime = 1.0f / m_VFXDef.FPSOverride;
//            }
//            else
//            {
//                m_WaitTime = 1.0f / m_VFXInfo.FramesPerSec;
//            }
//            m_LastFrameChange = 0.0f;
//        }

//        public void StopVFX()
//        {
//            m_CurrentFrame = 0;
//            m_LastFrameChange = Time.time + 30000.0f;
//            m_Renderer.enabled = false;
//            m_Stopped = true;
//        }

//        public void ResetVFX(float offsetTime = 0.0f)
//        {
//            m_CurrentFrame = 0;
//            m_LastFrameChange = Time.time + offsetTime;
//            m_Renderer.sprite = m_VFXInfo.Frames[m_CurrentFrame];
//            m_Renderer.enabled = false;
//            m_Stopped = false;
//        }

//        protected void OnUpdate()
//        {
//            if (m_LastFrameChange < Time.time)
//            {
//                m_Renderer.enabled = true;
//                ++m_CurrentFrame;
//                if (m_CurrentFrame == m_VFXInfo.Frames.Length)
//                {
//                    switch (m_VFXDef.OnEnd)
//                    {
//                        case Def.VFXEnd.Stop:
//                            StopVFX();
//                            break;
//                        case Def.VFXEnd.SelfDestroy:
//                            GameObject.Destroy(gameObject);
//                            break;
//                        case Def.VFXEnd.Repeat:
//                            ResetVFX();
//                            break;
//                    }
//                    return;
//                }
//                m_Renderer.sprite = m_VFXInfo.Frames[m_CurrentFrame];
//                m_LastFrameChange = Time.time + m_WaitTime;
//            }
//            if (transform.position.y <= -50.0f)
//                GameObject.Destroy(gameObject);
//        }

//        private void Update()
//        {
//            OnUpdate();
//        }

//        private void LateUpdate()
//        {
//            var cam = CameraManager.Mgr;
//            Vector3 v;
//            Vector3 dir;
//            switch (m_VFXDef.Facing)
//            {
//                case Def.VFXFacing.FaceCameraFull:
//                    transform.LookAt(cam.transform);
//                    break;
//                case Def.VFXFacing.FaceCameraFreezeX:
//                    v = cam.transform.position - transform.position;
//                    v.x = 0.0f;
//                    dir = cam.transform.position - v;
//                    transform.LookAt(dir);
//                    break;
//                case Def.VFXFacing.FaceCameraFreezeY:
//                    v = cam.transform.position - transform.position;
//                    v.y = 0.0f;
//                    dir = cam.transform.position - v;
//                    transform.LookAt(dir);
//                    break;
//                case Def.VFXFacing.FaceCameraFreezeZ:
//                    v = cam.transform.position - transform.position;
//                    v.z = 0.0f;
//                    dir = cam.transform.position - v;
//                    transform.LookAt(dir);
//                    break;
//                case Def.VFXFacing.FaceCameraFreezeXY:
//                    v = cam.transform.position - transform.position;
//                    v.x = 0.0f;
//                    v.y = 0.0f;
//                    dir = cam.transform.position - v;
//                    transform.LookAt(dir);
//                    break;
//                case Def.VFXFacing.FaceCameraFreezeXZ:
//                    v = cam.transform.position - transform.position;
//                    v.x = 0.0f;
//                    v.z = 0.0f;
//                    dir = cam.transform.position - v;
//                    transform.LookAt(dir);
//                    break;
//                case Def.VFXFacing.FaceCameraFreezeYZ:
//                    v = cam.transform.position - transform.position;
//                    v.y = 0.0f;
//                    v.z = 0.0f;
//                    dir = cam.transform.position - v;
//                    transform.LookAt(dir);
//                    break;
//                case Def.VFXFacing.FaceXUp:
//                    dir = transform.position + new Vector3(100f, 0f, 0f);
//                    transform.LookAt(dir);
//                    break;
//                case Def.VFXFacing.FaceXDown:
//                    dir = transform.position + new Vector3(-100f, 0f, 0f);
//                    transform.LookAt(dir);
//                    break;
//                case Def.VFXFacing.FaceYUp:
//                    dir = transform.position + new Vector3(0f, 100f, 0f);
//                    transform.LookAt(dir);
//                    break;
//                case Def.VFXFacing.FaceYDown:
//                    dir = transform.position + new Vector3(0f, -100f, 0f);
//                    transform.LookAt(dir);
//                    break;
//                case Def.VFXFacing.FaceZUp:
//                    dir = transform.position + new Vector3(0f, 0f, 100f);
//                    transform.LookAt(dir);
//                    break;
//                case Def.VFXFacing.FaceZDown:
//                    dir = transform.position + new Vector3(0f, 0f, -100f);
//                    transform.LookAt(dir);
//                    break;
//            }
//        }
//    }
//}
