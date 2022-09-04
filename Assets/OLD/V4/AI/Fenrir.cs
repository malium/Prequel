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

//namespace Assets.AI
//{
//    public class Fenrir : MonsterScript
//    {
//        static int MonsterTypeID = 0;
//        //static int AttackVFXTypeID = 0;
//        static float AttackHeight = -1.0f;

//        const float MinAttackDist = 2f;
//        const float JumpDuration = 0.5f;
//        static readonly Vector2[] JumpOffsetDirection = new Vector2[8]
//        {
//            new Vector2(1, 0),
//            new Vector2(0, 1),
//            new Vector2(-1, 0),
//            new Vector2(0, -1),
//            new Vector2(1f / Mathf.Sqrt(2), 1f / Mathf.Sqrt(2)),
//            new Vector2(-1f / Mathf.Sqrt(2), 1f / Mathf.Sqrt(2)),
//            new Vector2(1f / Mathf.Sqrt(2), -1f / Mathf.Sqrt(2)),
//            new Vector2(-1f / Mathf.Sqrt(2), -1f / Mathf.Sqrt(2)),
//        };

//        Vector3 m_JumpTarget;
//        Vector2 m_JumpSpeed;
//        Vector3 m_JumpInitialPos;
//        Vector2 m_JumpDirection;
//        float m_JumpStartTime;

//        public override void InitMonster()
//        {
//            if (MonsterTypeID < 1)
//            {
//                MonsterTypeID = Monsters.FamilyDict["Fenrir"];
//                //var monster = Monsters.MonsterInfos[MonsterTypeID];
//                //AttackVFXTypeID = VFXs.VFXDict[(int)Def.VFXTarget.MONSTER]["CharmingHeart"];
//            }

//            SetMonster(MonsterTypeID);

//            if (AttackHeight < 0.0f)
//            {
//                AttackHeight = (m_Info.Frames[0].rect.height / m_Info.Frames[0].pixelsPerUnit) * m_Info.SpriteScale;
//            }
//        }

//        // Jump To position, launch attack
//        public void JumpState()
//        {
//            var movementXZ = m_JumpDirection * m_JumpSpeed.x * Time.deltaTime;
//            var t = Time.time - m_JumpStartTime;
//            var yPos = -0.5f * 9.81f * (t * t) + m_JumpSpeed.y * t + m_JumpInitialPos.y;
//            var movementY = yPos - transform.position.y;

//            var movement = new Vector3(movementXZ.x, movementY, movementXZ.y);
//            transform.Translate(movement, Space.World);
//            m_TargetPosition = transform.position;
//        }

//        public AIState JumpCheck()
//        {
//            var posXZ = new Vector2(transform.position.x, transform.position.z);
//            var targetXZ = new Vector2(m_JumpTarget.x, m_JumpTarget.z);
//            var dir = (targetXZ - posXZ).normalized;
//            if(!GameUtils.IsNearlyEqual(dir.x, m_JumpDirection.x, 0.1f) 
//                || !GameUtils.IsNearlyEqual(dir.y, m_JumpDirection.y, 0.1f))
//            {
//                m_AI.SetFunction(new AIFunction(AIState.IDLE, () => { }, () => { return AIState.ROAMING; }));
//                Attack(null, m_JumpTarget + new Vector3(m_JumpDirection.x * MinAttackDist, 0f, m_JumpDirection.y * MinAttackDist));
//                return AIState.AGRESSIVE;
//            }
//            //if (Time.time > (m_JumpStartTime + JumpDuration))
//            //{
                
//            //}
//            return AIState.IDLE;
//        }

//        public override void Attack(LivingEntity target, Vector3 targetPos)
//        {
//            if(target != null)
//            {
//                targetPos = target.transform.position;
//            }
//            var targetXZ = new Vector2(targetPos.x, targetPos.z);
//            var posXZ = new Vector2(transform.position.x, transform.position.z);

//            var yDist = Mathf.Abs(targetPos.y - transform.position.y);
//            var xzDist = Vector2.Distance(targetXZ, posXZ);

//            m_JumpDirection = (targetXZ - posXZ).normalized;
//            var invDirection = (posXZ - targetXZ).normalized;

//            void Jump()
//            {
//                float ground = 0f;// GameUtils.GetNearGround(targetPos, out IBlock bk, out BridgeComponent bg);
//                m_JumpTarget = new Vector3(targetXZ.x, ground, targetXZ.y);
//                m_JumpInitialPos = transform.position;
//                m_JumpSpeed.Set(xzDist / JumpDuration, xzDist);
//                m_JumpStartTime = Time.time;

//                float A = -0.5f * 9.81f;
//                float B = m_JumpSpeed.y;
//                float C = yDist;
//                var sqrtpart = Mathf.Sqrt(B * B - 4 * A * C);
//                Vector2 equation = new Vector2(-B + sqrtpart, -B - sqrtpart);
//                equation /= (2f * A);

//                if (equation.x > 0f)
//                    m_JumpSpeed.Set(xzDist / equation.x, m_JumpSpeed.y);
//                else
//                    m_JumpSpeed.Set(xzDist / equation.y, m_JumpSpeed.y);
//                m_AI.ChangeStateTo(AIState.IDLE);
//                m_AI.SetFunction(new AIFunction(AIState.IDLE, JumpState, JumpCheck));
//            }

//            if (yDist < 2f && (xzDist > MinAttackDist && xzDist < 5f))
//            {
//                targetXZ += invDirection * 2f;
//                //targetPos += new Vector3(m_JumpDirection.x * -MinAttackDist, 0f, m_JumpDirection.z * -MinAttackDist);
//                if (GameUtils.CanGoThere(targetPos.y, targetXZ, 0.6f, true))
//                {
//                    Jump();
//                    return;
//                }
//            }
            
//            if(xzDist <= MinAttackDist)
//            {
//                // Jump again with a little probability
//                if(Manager.Mgr.DamageRNG.NextDouble() < 0.25f)
//                {
//                    var offset = JumpOffsetDirection[Manager.Mgr.DamageRNG.Next(JumpOffsetDirection.Length)];
//                    targetXZ += offset * MinAttackDist;
//                    //targetPos += new Vector3(offset.x * MinAttackDist, 0f, offset.y * MinAttackDist);
//                    if (GameUtils.CanGoThere(targetPos.y, targetXZ, 0.6f, true))
//                    {
//                        Jump();
//                        return;
//                    }
//                }

//                var friendlyTeam = new List<MonsterTeam>(1) { /*m_Info.Team*/ };
//                var friendlyFamily = new List<int>(1) { Monsters.FamilyDict[m_Info.Name] /*m_Info.MonsterID*/ };
//                var fenrirVFX_1 = Instantiate(AssetContainer.Mgr.TriVFXGameObjects[2]).GetComponent<FenrirAttackVFX>();
//                var fenrirVFX_2 = Instantiate(AssetContainer.Mgr.TriVFXGameObjects[2]).GetComponent<FenrirAttackVFX>();

//                fenrirVFX_1.gameObject.SetActive(true);
//                fenrirVFX_2.gameObject.SetActive(true);

//                fenrirVFX_1.transform.position = transform.position;
//                fenrirVFX_2.transform.position = transform.position;

//                fenrirVFX_1.Set(m_Info, m_EnemyList, friendlyTeam, friendlyFamily);
//                fenrirVFX_2.Set(m_Info, m_EnemyList, friendlyTeam, friendlyFamily);

//                fenrirVFX_1.transform.localScale *= 0.8f;
//                fenrirVFX_2.transform.localScale *= 0.8f;

//                fenrirVFX_1.transform.forward = new Vector3( m_JumpDirection.x, fenrirVFX_1.transform.forward.y,  m_JumpDirection.y);
//                fenrirVFX_2.transform.forward = new Vector3(-m_JumpDirection.x, fenrirVFX_2.transform.forward.y, -m_JumpDirection.y);
//            }
//            //else
//            //{
//            //    targetXZ 
//            //    targetPos += new Vector3(m_JumpDirection.x * -MinAttackDist, 0f, m_JumpDirection.z * -MinAttackDist);
//            //    Jump();
//            //    return;
//            //}
//        }
//    }
//}
