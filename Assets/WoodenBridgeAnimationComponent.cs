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
    class WoodenBridgeAnimationComponent : MonoBehaviour
    {
        public const float AnimationDuration = 0.2f;
        public const float BounceDuration = 0.5f;
        public const float AppearOffset = 0.1f;
        float m_AnimStart;
        float m_AnimEnd;
        float m_Apperance;
        //float m_BounceSpeed;
        Vector3 m_BounceSpeed;
        Vector3 m_TargetPos;
        Quaternion m_TargetOrientation;
        Vector2 m_InitPosXZ;
        Vector3 m_AfterInitPos;
        Vector3 m_BouncePos;
        float m_StartingHeight;
        Vector3 m_BounceTimeAccum;
        Vector3 m_BounceTimes;
        MeshRenderer m_Renderer;
        public float BridgeStartHeight;
        public float BridgeEndHeight;

        private void Awake()
        {
            BridgeStartHeight = BridgeEndHeight = 0.0f;
            m_AnimStart = float.MaxValue;
            m_AnimEnd = float.MaxValue;
            m_Apperance = float.MaxValue;
        }

        private void Start()
        {
            
        }

        public void StartAnimation(float apperance/*, float animationStart*/, float animationEnd, float startingHeight, Def.SpaceDirection direction)
        {
            float angle = -60.0f;
            Vector3 axis = Vector3.zero;
            if (direction == Def.SpaceDirection.WEST || direction == Def.SpaceDirection.SOUTH)
                angle *= -1.0f;
            if (direction == Def.SpaceDirection.EAST || direction == Def.SpaceDirection.WEST)
                axis.Set(1, 0, 0);
            else
                axis.Set(0, 0, 1);

            //m_AnimStart = animationStart;
            m_AnimEnd = animationEnd;
            m_Apperance = apperance;
            m_TargetPos = transform.position;
            m_TargetOrientation = transform.rotation;
            m_StartingHeight = startingHeight;
            //m_BounceSpeed = Mathf.Abs(transform.position.y) / BounceDuration;
            float yPos = startingHeight - transform.position.y;
            m_AfterInitPos = new Vector3(transform.position.x, yPos, transform.position.z);
            switch (direction)
            {
                case Def.SpaceDirection.NORTH:
                    transform.Translate(new Vector3(+0.2f, yPos, 0.0f), Space.World);
                    break;
                case Def.SpaceDirection.SOUTH:
                    transform.Translate(new Vector3(-0.2f, yPos, 0.0f), Space.World);
                    break;
                case Def.SpaceDirection.EAST:
                    transform.Translate(new Vector3(0.0f, yPos, -0.2f), Space.World);
                    break;
                case Def.SpaceDirection.WEST:
                    transform.Translate(new Vector3(0.0f, yPos, +0.2f), Space.World);
                    break;
            }
            m_InitPosXZ = new Vector2(transform.position.x, transform.position.z);
            var bounceOffset = Mathf.Abs(yPos - m_TargetPos.y);
            //var bounceOffset = Mathf.Abs(yPos);
            //if(bounceOffset > 1.0f)
            //{
            //    throw new Exception();
            //}
            //var maximum = bounceOffset * 1.3f + (1.0f - bounceOffset) * 1.0f;
            //var minimum = bounceOffset * 0.9f + (1.0f - bounceOffset) * 1.0f;
            m_BouncePos = new Vector3(bounceOffset * 1.3f, bounceOffset * 0.9f, bounceOffset);
            m_BounceTimes = new Vector3(BounceDuration / 3.0f, BounceDuration / 3.0f, BounceDuration / 3.0f);
            m_BounceTimeAccum = new Vector3(m_BounceTimes.x, m_BounceTimes.x + m_BounceTimes.y, m_BounceTimes.x + m_BounceTimes.y + m_BounceTimes.z);
            m_BounceSpeed = new Vector3(Mathf.Abs(yPos - m_BouncePos.x) / m_BounceTimeAccum.x, Mathf.Abs(yPos - m_BouncePos.y) / m_BounceTimeAccum.y, Mathf.Abs(yPos - m_BouncePos.z) / m_BounceTimeAccum.z);
            transform.Rotate(axis, angle, Space.World);
            m_Renderer = gameObject.GetComponent<MeshRenderer>();
            m_Renderer.enabled = false;
        }

        void Appear()
        {
            m_Renderer.enabled = true;
            m_AnimStart = Time.time;
            //Debug.Log("Bridge: " + transform.parent.GetComponent<BridgeComponent>().Pilar.name + " just appeared");
        }

        void OnAnimation()
        {
            float endTime = m_AnimStart + AnimationDuration;
            float t = endTime - Time.time;
            if(t < 0.0f)
            {
                m_AnimStart = float.MaxValue;
                transform.rotation = m_TargetOrientation;
                return;
            }
            t = AnimationDuration - t;
            t /= AnimationDuration;

            Vector2 targetXZ = new Vector2(m_AfterInitPos.x, m_AfterInitPos.z);

            var nextPosXZ = (1.0f - t) * m_InitPosXZ + t * targetXZ;
            var offsetPos = nextPosXZ - new Vector2(transform.position.x, transform.position.z);
            transform.Translate(new Vector3(offsetPos.x, 0.0f, offsetPos.y), Space.World);

            transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetOrientation, t);
        }

        void OnBounce()
        {
            var endTime = (m_AnimEnd + BounceDuration);
            float t = endTime - Time.time;
            if (t < 0.0f)
            {
                var offset = m_TargetPos - transform.position;
                transform.Translate(offset, Space.World);
                return;
            }
            t = BounceDuration - t;
            float movement = 0.0f;
            float target = 0.0f;
            if(t < m_BounceTimeAccum.x)
            {
                target = (m_StartingHeight - m_TargetPos.y) * 0.3f;
                movement = GameUtils.LinearMovement1D(transform.position.y, m_TargetPos.y - target, Time.deltaTime * m_BounceSpeed.x);
            }
            else if(t < m_BounceTimeAccum.y)
            {
                target = (m_StartingHeight - m_TargetPos.y) * 0.1f;
                movement = GameUtils.LinearMovement1D(transform.position.y, m_TargetPos.y + target, Time.deltaTime * m_BounceSpeed.y);
            }
            else if(t < m_BounceTimeAccum.z)
            {
                movement = GameUtils.LinearMovement1D(transform.position.y, m_TargetPos.y, Time.deltaTime * m_BounceSpeed.z);
            }

            //var movement = GameUtils.LinearMovement1D(transform.position.y, m_TargetPos.y, Time.deltaTime * m_BounceSpeed);
            transform.Translate(new Vector3(0.0f, movement, 0.0f), Space.World);
        }

        private void Update()
        {
            if(m_Apperance < Time.time)
            {
                Appear();
                m_Apperance = float.MaxValue;
            }
            if (m_AnimStart < Time.time)
            {
                OnAnimation();
            }
            if(m_AnimEnd < Time.time)
            {
                OnBounce();
            }
            if(transform.position == m_TargetPos && transform.rotation == m_TargetOrientation)
            {
                var bridge = gameObject.transform.parent.gameObject.GetComponent<BridgeComponent>();
                bridge.SetBridgeHeightInfo(BridgeStartHeight, BridgeEndHeight, bridge.Direction);
                Component.Destroy(this);
            }
        }
    }
}
