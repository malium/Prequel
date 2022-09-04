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
//    [Serializable]
//    public class OddMonsterController : IOddController
//    {
//        const float DepossesionTimeOffset = 1.0f;
//        const float StructureCheckDelay = 0.1f;
//        OddScript Odd;
//        //[SerializeField]
//        //ControllerDef m_Def;
//        //[SerializeField]
//        //MonsterScript m_Monster;
//        [SerializeField]
//        Vector2 m_CurDirection;
//        [SerializeField]
//        float m_DepossessionTime;
//        float m_NextStructureCheck;
//        bool m_IsFenrir;
//        bool m_IsJumping;

//        public override void FixedUpdate()
//        {
//            //m_Monster._SpriteFrameUpdate();
//            Odd.PossessedMonster._SpriteFrameUpdate();
//        }

//        public override OddControllerType GetControllerType()
//        {
//            return OddControllerType.MONSTER;
//        }

//        //public override ControllerDef GetControllerDef()
//        //{
//        //    return m_Def;
//        //}

//        public override void LateUpdate()
//        {
//            var camPos = Manager.Mgr.m_Camera.transform.position;
//            Odd.PossessedMonster._FaceCamera();
//            //m_Monster._FaceCamera();
//        }

//        public override void MoveTo(Vector2 pos)
//        {
//            var curPos = new Vector2(Odd.Position.x, Odd.Position.z);
//            var dist = Vector2.Distance(curPos, pos);
//            if (dist == 0.0f)
//                return;
//            m_CurDirection = (pos - curPos).normalized;
//            Odd._TargetPos = new Vector3(pos.x, 0.0f, pos.y);
//        }

//        public override void OnDisable()
//        {

//        }

//        public override void OnEnable()
//        {

//        }

//        public override void OnGUI()
//        {

//        }

//        public override void OnReceiveDamage()
//        {

//        }

//        public override void Attack(Vector3 pos)
//        {
//            if (Odd.PossessedMonster._NextAttackTime < Time.time)
//            {
//                Odd.PossessedMonster._NextAttackTime = Time.time + Odd.PossessedMonster.Info.AttackRate;

//                var curPos = new Vector2(Odd.Position.x, Odd.Position.z);
//                var attackPos = new Vector2(pos.x, pos.z);
//                m_CurDirection = (attackPos - curPos).normalized;
//                var dist = Vector2.Distance(curPos, attackPos);
//                if (dist > Odd.PossessedMonster.Info.AttackRange)
//                {
//                    attackPos = curPos + m_CurDirection * Odd.PossessedMonster.Info.AttackRange;
//                }
//                Odd.PossessedMonster.Attack(null, new Vector3(attackPos.x, pos.y, attackPos.y));
//                // Is Fenrir and Jumping
//                if (m_IsFenrir && Odd.PossessedMonster.AI.GetCurrentState() == AI.AIState.IDLE)
//                {
//                    m_IsJumping = true;
//                    Odd.PossessedMonster.Direction = m_CurDirection;
//                    Odd.PossessedMonster._FacingUpdate();
//                }
//            }
//        }

//        public override void SecondaryAction(Vector3 pos)
//        {
//            //if(Odd.PossessedMonster._NextAttackTime < Time.time)
//            //{
//            //    Odd.PossessedMonster._NextAttackTime = Time.time + Odd.PossessedMonster.Info.AttackRate;

//            //    var curPos = new Vector2(Odd.Position.x, Odd.Position.z);
//            //    var attackPos = new Vector2(pos.x, pos.z);
//            //    m_CurDirection = (attackPos - curPos).normalized;
//            //    var dist = Vector2.Distance(curPos, attackPos);
//            //    if (dist > Odd.PossessedMonster.Info.AttackRange)
//            //    {
//            //        attackPos = curPos + m_CurDirection * Odd.PossessedMonster.Info.AttackRange;
//            //    }
//            //    Odd.PossessedMonster.Attack(null, new Vector3(attackPos.x, pos.y, attackPos.y));
//            //    // Is Fenrir and Jumping
//            //    if(m_IsFenrir && Odd.PossessedMonster.AI.GetCurrentState() == AI.AIState.IDLE)
//            //    {
//            //        m_IsJumping = true;
//            //        Odd.PossessedMonster.Direction = m_CurDirection;
//            //        Odd.PossessedMonster._FacingUpdate();
//            //    }
//            //}
//        }

//        public override void SetController(LivingEntityDef controller, OddScript oddScript)
//        {
//            //m_Def = controller;
//            //Odd = oddScript;
//            //Odd._AngularSpeed = m_Def.AngularSpeed;
//            //Odd._MaxJump = m_Def.WalkingJumpHeight;
//            //Odd._Radius = m_Def.Radius;
//            //Odd._Speed = m_Def.Speed;
//            Odd = oddScript;
//            Odd._AngularSpeed = controller.AngularSpeed;
//            Odd._Speed = controller.Speed;
//            Odd._Radius = controller.Radius;
//            Odd._MaxJump = controller.MaxJump;
//            Odd._FlySpeed = controller.FlySpeed;
//            Odd._FallSpeed = controller.FallSpeed;
//            Odd._Height = controller.Height;
//            Odd.State = ODD_STATE.POSSESSED;

//            Odd.PossessedMonster.enabled = false;
//            //m_Monster = m_Def.PossessedMonster;
//            //m_Monster.enabled = false;
//            Odd.MeshRenderer.enabled = false;
//            Odd.FaceRenderer.enabled = false;
//            Odd.Weapon.Renderer.enabled = false;
//            Odd.Collider.enabled = false;

//            Odd.PossessedMonster.SpriteSR.enabled = true;
//            Odd.PossessedMonster.ShadowSR.enabled = true;
//            var dir = (Odd.Position - Odd.PossessedMonster.transform.position).normalized;
//            m_CurDirection.Set(dir.x, dir.z);
//            Odd.QuickInventory.enabled = false;
//            Odd.PossessedMonster.AI.ChangeStateTo(AI.AIState.AGRESSIVE);
//            m_DepossessionTime = Time.time + DepossesionTimeOffset;
//            m_IsJumping = false;
//            m_IsFenrir = false;// Odd.PossessedMonster is AI.Fenrir;
//        }

//        bool IsFalling(IBlock currentBlock, BridgeComponent currentBridge)
//        {
//            return (/*(currentBlock != null && currentBlock.Layer == 0) || */currentBlock == null) && currentBridge == null;
//        }

//        void WhileFalling()
//        {
//            float fallSpeed = Mathf.Min(-9.81f * Mathf.Abs(((Odd.Position.y) * 0.1f)), -9.81f);
//            fallSpeed *= Time.deltaTime;
//            var posOffset = new Vector3(0.0f, fallSpeed, 0.0f);
//            Odd.Position = Odd.Position + posOffset;
//            Odd.transform.Translate(posOffset, Space.World);
//        }

//        public override void Update()
//        {
//            bool monsterDead = Odd.PossessedMonster.GetCurrentHealth() <= 0.0f;
//            if ((Input.GetKey(KeyCode.Space) && Time.time > m_DepossessionTime) || monsterDead)
//            {
//                //ControllerDef def;
//                //def.AttackRange = Odd.Weapon.AttackRange;
//                //def.FallSpeed = 2.5f;
//                //def.UpSpeed = 5.0f;
//                //def.PossessedMonster = null;
//                //def.Radius = Odd.MeshRenderer.bounds.extents.x > Odd.MeshRenderer.bounds.extents.z ? Odd.MeshRenderer.bounds.extents.x : Odd.MeshRenderer.bounds.extents.z;
//                //def.Speed = 3.0f;
//                //def.WalkingJumpHeight = 0.5f;
//                //def.AngularSpeed = 10.0f;
                
//                Odd.SetController(OddControllerType.ODD, new LivingEntityDef()
//                {
//                    AngularSpeed = OddScript.OddAngularSpeed,
//                    FallSpeed = OddScript.OddFallSpeed,
//                    FlySpeed = OddScript.OddFlySpeed,
//                    Height = Odd.Collider.size.y,
//                    MaxJump = OddScript.OddMaxJump,
//                    Radius = Odd.MeshRenderer.bounds.extents.x > Odd.MeshRenderer.bounds.extents.z ? 
//                        Odd.MeshRenderer.bounds.extents.x : Odd.MeshRenderer.bounds.extents.z,
//                    Speed = OddScript.OddSpeed,
//                });
//                if (monsterDead)
//                    Odd.Animator.Play(OddScript.AnimHashes.Idle);
                
//                return;
//            }
//            Odd.PossessedMonster._OnReceiveDamageUpdate();
//            if (m_IsJumping)
//            {
//                var monPos = Odd.PossessedMonster.transform.position;
//                Odd.PossessedMonster.AI.Update();
//                var posOff = Odd.PossessedMonster.transform.position - monPos;
//                Odd.Position += posOff;
//                Odd.transform.Translate(posOff, Space.World);
//                Odd._TargetPos = Odd.Position;
//                if(Odd.PossessedMonster.AI.GetCurrentState() != AI.AIState.IDLE)
//                {
//                    m_IsJumping = false;
//                    return;
//                }
//            }
//            else
//            {
//                Odd.PossessedMonster.transform.Translate(Odd.Position - Odd.PossessedMonster.transform.position, Space.World);
//                Odd.PossessedMonster.Direction = m_CurDirection;
//                Odd.PossessedMonster._FacingUpdate();

//                var positionXZ = new Vector2(Odd.Position.x, Odd.Position.z);
//                Vector2 movDir = Vector2.zero;
//                var camera = Manager.Mgr.m_Camera;
//                bool fwd = false;
//                if (Input.GetKey(KeyCode.W))
//                {
//                    //movDir.Set(-1f, 0f);
//                    movDir.Set(camera.transform.forward.x, camera.transform.forward.z);
//                    fwd = true;
//                }
//                else if (Input.GetKey(KeyCode.S))
//                {
//                    //movDir.Set(1f, 0f);
//                    movDir.Set(-camera.transform.forward.x, -camera.transform.forward.z);
//                    fwd = true;
//                }
//                if (Input.GetKey(KeyCode.A))
//                {
//                    //movDir.Set(movDir.x, -1f);
//                    if (fwd)
//                        movDir.Set((movDir.x + -camera.transform.right.x) * 0.5f, (movDir.y + -camera.transform.right.z) * 0.5f);
//                    else
//                        movDir.Set(-camera.transform.right.x, -camera.transform.right.z);
//                }
//                else if (Input.GetKey(KeyCode.D))
//                {
//                    //movDir.Set(movDir.x, 1f);
//                    if (fwd)
//                        movDir.Set((movDir.x + camera.transform.right.x) * 0.5f, (movDir.y + camera.transform.right.z) * 0.5f);
//                    else
//                        movDir.Set(camera.transform.right.x, camera.transform.right.z);

//                }
//                movDir.Normalize();
//                var wasdMovement = movDir * Odd.GetSpeed() * Time.deltaTime;
//                var nextPosXZ = positionXZ + wasdMovement;
//                Odd._TargetPos = new Vector3(positionXZ.x + 3f * wasdMovement.x, Odd.Position.y, positionXZ.y + 3f * wasdMovement.y);

//                if (movDir != Vector2.zero)
//                    m_CurDirection = movDir;

//                bool onGround = Odd.UpdateMovement(Odd.JumpToVoid < 2, Odd.Position.y, Time.deltaTime, out Vector3 movement,
//                    out IBlock currentBlock, out BridgeComponent currentBridge);

//                ////bool onGround = GameUtils.PFMovement(Odd.Position, new Vector2(Odd.GetTargetPosition().x, Odd.GetTargetPosition().z), Odd.PossessedMonster.Info.BaseSpeed,
//                ////    6.0f, 3.0f, Odd.PossessedMonster.GetRadius(), Odd.PossessedMonster.GetMaxJump(), Time.deltaTime, true, out Vector3 movement, out BlockComponent currentBlock,
//                ////    out BridgeComponent currentBridge);

//                Odd.Position += movement;
//                Odd.transform.Translate(movement, Space.World);

//                Odd.PossessedMonster.RegisterLE(Odd.Position - Odd.PossessedMonster.transform.position != Vector3.zero);

//                //if (m_NextStructureCheck < Time.time)
//                //{
//                //    var strucs = Manager.GetStrucs(new Vector2(Odd.Position.x, Odd.Position.z));
//                //    if (strucs.Count > 0)
//                //    {
//                //        if(Odd.PossessedMonster.Struc != null && strucs[0] != Odd.PossessedMonster.Struc)
//                //        {
//                //            if (Odd.PossessedMonster.Struc.LivingEntities.Contains(Odd.PossessedMonster))
//                //                Odd.PossessedMonster.Struc.LivingEntities.Remove(Odd.PossessedMonster);
//                //            Odd.PossessedMonster.Struc = strucs[0];
//                //            Odd.PossessedMonster.Struc.LivingEntities.Add(Odd.PossessedMonster);
//                //        }
//                //        m_NextStructureCheck = Time.time + StructureCheckDelay;
//                //    }
//                //}

//                if (IsFalling(currentBlock, currentBridge) && Manager.Mgr.HideInfo)
//                {
//                    WhileFalling();
//                    return;
//                }
//            }

//            Odd.PossessedMonster._ElmUpdate();
//        }

//        public override void StopController()
//        {
//            Odd.State = ODD_STATE.COUNT;
//            Odd.MeshRenderer.enabled = true;
//            Odd.FaceRenderer.enabled = true;
//            Odd.Weapon.Renderer.enabled = false;
//            Odd.Collider.enabled = true;
//            if (Odd.PossessedMonster != null)
//            {
//                Odd.PossessedMonster.enabled = true;
//                Odd.PossessedMonster._TargetPos = Odd.Position;
//                Odd.PossessedMonster.AI.ChangeStateTo(AI.AIState.ROAMING);
//                Odd.PossessedMonster = null;
//            }
//            Odd.QuickInventory.enabled = true;
//        }
//    }
//}
