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

namespace Assets.AI
{
    public class FenrirAttackVFX : MonoBehaviour
    {
        static readonly int AnimHash = Animator.StringToHash("Layer.Attack");
        const float ClawsDuration = 0.5f;
        const float ClawsSpan = 180f;
        const float ClawsSpeed = ClawsSpan / ClawsDuration;

        [SerializeField]
        VFX3DComponent m_Claws = null;
        [SerializeField]
        VFX3DComponent m_Trail = null;

        [SerializeField]
        Animator m_Animator = null;
        float m_StartTime;
        Vector3 m_InitAngles;

        public void Set(MonsterFamily info, List<AI.CLivingEntity> enemies, List<MonsterTeam> avoidTeams, List<int> avoidFamiles)
        {
            m_Claws.SetVFX(new VFX3DDef("FenrirClaws", Def.VFXType.CAST, 0, m_Claws.gameObject, Def.VFXEnd.SelfDestroy, 24f));
            m_Trail.SetVFX(new VFX3DDef("FenrirTrail", Def.VFXType.CAST, 0, m_Trail.gameObject, Def.VFXEnd.SelfDestroy, 24f));
            ResetVFX();
            m_InitAngles = m_Claws.transform.localRotation.eulerAngles;
            m_Claws.GetComponent<FenrirClawComponent>().Set(info, enemies, avoidTeams, avoidFamiles);
        }

        public void ResetVFX()
        {
            m_Claws.ResetVFX();
            m_Trail.ResetVFX();
            m_Animator.Play(AnimHash);
            m_StartTime = Time.time;
        }

        public void Update()
        {
            if (m_Claws == null && m_Trail == null)
            {
                GameObject.Destroy(gameObject);
                return;
            }
            if (m_Claws == null)
                return;
            var timeOffset = Time.time - m_StartTime;
            timeOffset /= 0.5f;
            float angle = (1f - timeOffset) * m_InitAngles.y + timeOffset * (m_InitAngles.y + ClawsSpan);
            m_Claws.transform.localRotation = Quaternion.Euler(m_InitAngles.x, angle, m_InitAngles.z);
            //m_Claws.transform.Rotate(0f, Time.deltaTime * ClawsSpeed, 0f);
        }
    }
}
