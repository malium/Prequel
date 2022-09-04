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
    public class BridgeComponent : MonoBehaviour
    {
        public const string BridgeTag = "BRIDGE";
        public const float AnimationDuration = 0.5f;
        public CPilar Pilar;
        // If its the first bridge it will be the block next to it, otherwise it will be the previous Bridge object
        public BridgeComponent Previous;
        // If its the last bridge it will be the block after it, otherwise it will be the next Bridge object
        public BridgeComponent Next;

        public GameObject[] Objects;
        Vector3[] m_LocalPositions;
        BoxCollider[] m_Colliders;
        Outline m_Outline;
        public bool BridgeOutline
        {
            get
            {
                return m_Outline.enabled;
            }
            set
            {
                SetOutline(value);
            }
        }

        public BoxCollider BridgeCollider;

        public int BridgeID
        {
            get
            {
                return m_BridgeID;
            }
        }
        int m_BridgeID;
        public BridgeType Type
        {
            get
            {
                return m_Type;
            }
        }
        BridgeType m_Type;
        
        public Def.RotationState Rotation
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                if (Pilar == null)
                    return;

                float amount = 1.0f;
                if (Type == BridgeType.BIG)
                    amount = 2.0f;

                var pilarPos = Pilar.gameObject.transform.position;
                for(int i = 0; i < Objects.Length; ++i)
                {
                    var curObj = Objects[i];
                    curObj.transform.SetPositionAndRotation(pilarPos, Quaternion.identity);

                    switch (value)
                    {
                        case Def.RotationState.Default:
                            curObj.transform.Translate(new Vector3(amount, 0.0f, 0.0f), Space.Self);
                            break;
                        case Def.RotationState.Right:
                            curObj.transform.Rotate(Vector3.up, 90.0f, Space.Self);
                            break;
                        case Def.RotationState.Half:
                            curObj.transform.Translate(Vector3.forward, Space.Self);
                            curObj.transform.Rotate(Vector3.up, 180.0f, Space.Self);
                            break;
                        case Def.RotationState.Left:
                            curObj.transform.Translate(new Vector3(1.0f, 0.0f, amount), Space.Self);
                            curObj.transform.Rotate(Vector3.up, 270.0f, Space.Self);
                            break;
                    }
                    curObj.transform.Translate(m_LocalPositions[i], Space.Self);
                }


                //transform.SetPositionAndRotation(pilarPos, Quaternion.identity);

                //switch(value)
                //{
                //    case BlockRotation.Default:
                //        gameObject.transform.Translate(new Vector3(amount, 0.0f, 0.0f), Space.Self);
                //        //transform.SetPositionAndRotation(Pilar.transform.position + Vector3.right, Quaternion.identity);
                //        break;
                //    case BlockRotation.Right:
                //        gameObject.transform.Rotate(Vector3.up, 90.0f, Space.Self);
                //        //transform.SetPositionAndRotation(Pilar.transform.position, Quaternion.Euler(0.0f, 90.0f, 0.0f));
                //        break;
                //    case BlockRotation.Half:
                //        gameObject.transform.Translate(Vector3.forward, Space.Self);
                //        gameObject.transform.Rotate(Vector3.up, 180.0f, Space.Self);
                //        //transform.SetPositionAndRotation(Pilar.transform.position + Vector3.forward, Quaternion.Euler(0.0f, 180.0f, 0.0f));
                //        break;
                //    case BlockRotation.Left:
                //        gameObject.transform.Translate(new Vector3(1.0f, 0.0f, amount), Space.Self);
                //        gameObject.transform.Rotate(Vector3.up, 270.0f, Space.Self);
                //        //transform.SetPositionAndRotation(Pilar.transform.position + new Vector3(1.0f, 0.0f, amount), Quaternion.Euler(0.0f, 270.0f, 0.0f));
                //        break;
                //}
                m_Rotation = value;
            }
        }
        Def.RotationState m_Rotation;

        float m_StartingHeight;
        float m_EndingHeight;
        public Def.SpaceDirection Direction
        {
            get
            {
                return m_BridgeDirection;
            }
        }
        Def.SpaceDirection m_BridgeDirection;

        public float GetHeight(Vector2 inPilarOffsetXZ)
        {
            float height = 0.0f;
            const float maxLength = 1.0f + Def.BlockSeparation;
            float amount = 0.0f;
            switch (m_BridgeDirection)
            {
                case Def.SpaceDirection.NORTH:
                    amount = inPilarOffsetXZ.x / maxLength;
                    amount = 1.0f - amount;
                    break;
                case Def.SpaceDirection.SOUTH:
                    amount = inPilarOffsetXZ.x / maxLength;
                    break;
                case Def.SpaceDirection.EAST:
                    amount = inPilarOffsetXZ.y / maxLength;
                    break;
                case Def.SpaceDirection.WEST:
                    amount = inPilarOffsetXZ.y / maxLength;
                    amount = 1.0f - amount;
                    break;
            }
            height = m_StartingHeight * (1.0f - amount) + m_EndingHeight * amount;
            return height;
        }

        public void SetBridge(int bridgeInfoID)
        {
            var bridgeInfo = Bridges.BridgeInfos[bridgeInfoID];
            m_BridgeID = bridgeInfoID;
            m_Type = bridgeInfo.Type;
            if (Pilar == null)
                return;
            m_LocalPositions = new Vector3[Objects.Length];
            m_Colliders = new BoxCollider[Objects.Length];

            for (int i = 0; i < m_LocalPositions.Length; ++i)
            {
                m_LocalPositions[i] = Objects[i].transform.localPosition;
                m_Colliders[i] = Objects[i].GetComponent<BoxCollider>();
                m_Colliders[i].enabled = false;
            }
            BridgeCollider = gameObject.AddComponent<BoxCollider>();
            BridgeCollider.gameObject.layer = 10;
            transform.Translate(Pilar.transform.position, Space.World);
            transform.SetParent(Pilar.transform);
            Rotation = Def.RotationState.Default;
            UpdateCollision();
        }

        public void UpdateCollision()
        {
            // World space
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < m_Colliders.Length; ++i)
            {
                var objCol = m_Colliders[i];
                objCol.enabled = true;

                var bounds = objCol.bounds;
                for(int j = 0; j < 3; ++j)
                {
                    if (min[j] > bounds.min[j])
                        min[j] = bounds.min[j];
                    if (max[j] < bounds.max[j])
                        max[j] = bounds.max[j];
                }
                objCol.enabled = false;
            }
            min = min - transform.position;
            max = max - transform.position;
            BridgeCollider.center = (min + max) * 0.5f;
            BridgeCollider.size = (max - BridgeCollider.center) * 2.0f;
        }

        public void SetBridgeHeightInfo(float startingHeight, float endingHeight, Def.SpaceDirection direction)
        {
            m_StartingHeight = startingHeight;
            m_EndingHeight = endingHeight;
            m_BridgeDirection = direction;
        }

        public void StartAnimation(/*Vector2 initBridgePosXZ,*/ float apperanceOffset, /*float animStartOffset,*/ float animEnd, float[] linearHeight)
        {
            //var tempObjects = new GameObject[Objects.Length];
            //for(int i = 0; i < Objects.Length; ++i)
            //{
            //    float minDistance = float.MaxValue;
            //    GameObject minObject = null;
            //    for(int j = 0; j < Objects.Length; ++j)
            //    {
            //        if (tempObjects.Contains(Objects[j]))
            //            continue;
            //        var distance = Vector2.Distance(initBridgePosXZ, new Vector2(Objects[j].transform.position.x, Objects[j].transform.position.z));
            //        if(distance < minDistance)
            //        {
            //            minDistance = distance;
            //            minObject = Objects[j];
            //        }
            //    }
            //    tempObjects[i] = minObject;
            //}
            for (int i = 0; i < Objects.Length; ++i)
            {
                var anim = Objects[i].AddComponent<WoodenBridgeAnimationComponent>();
                anim.StartAnimation(Time.time + apperanceOffset + WoodenBridgeAnimationComponent.AppearOffset * i,
                    /*Time.time + animStartOffset + WoodenBridgeAnimationComponent.AnimationDuration * 0.5f * i,*/ Time.time + animEnd,
                    linearHeight[i], m_BridgeDirection);
                anim.BridgeStartHeight = m_StartingHeight;
                anim.BridgeEndHeight = m_EndingHeight;
            }
            m_StartingHeight = float.MaxValue;
            m_EndingHeight = float.MaxValue;
        }

        private void Awake()
        {
            Pilar = null;
            Previous = null;
            Next = null;
            m_BridgeID = -1;
            m_Type = BridgeType.COUNT;
            m_Rotation = Def.RotationState.COUNT;
            m_StartingHeight = 0.0f;
            m_EndingHeight = 0.0f;
            m_BridgeDirection = Def.SpaceDirection.COUNT;
            m_Outline = gameObject.AddComponent<Outline>();
            m_Outline.enabled = false;
            m_Outline.OutlineMode = Outline.Mode.OutlineAll;
            m_Outline.OutlineColor = Color.green;
        }

        private void Start()
        {
            //if (Pilar != null)
            //{
            //    transform.SetPositionAndRotation(Pilar.transform.position, Quaternion.identity);
            //    transform.SetParent(Pilar.transform);
            //}
            //else
            //{
            //    transform.SetParent(null);
            //}
        }

        private void Update()
        {
            
        }

        void SetOutline(bool enable, bool prev = false, bool next = false)
        {
            if (!prev && Previous != null)
            {
                Previous.SetOutline(enable, false, true);
            }
            if (!next && Next != null)
            {
                Next.SetOutline(enable, true, false);
            }
            m_Outline.enabled = enable;
        }

        public void Destroy()
        {
            if(Previous != null)
            {
                Previous.Next = null;
                Previous.Destroy();
            }
            if(Next != null)
            {
                Next.Previous = null;
                Next.Destroy();
            }
            Pilar.DestroyBridge();
        }
    }
}
