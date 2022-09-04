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
//    public enum OddControllerType
//    {
//        NULL,
//        ODD,
//        MONSTER,

//        COUNT
//    }

//    [Serializable]
//    public abstract class IOddController
//    {
//        public abstract void SetController(LivingEntityDef entityDef, OddScript oddScript);

//        public abstract void StopController();

//        //public abstract ControllerDef GetControllerDef();

//        public abstract void Update();

//        public abstract void FixedUpdate();

//        public abstract void OnGUI();

//        public abstract void LateUpdate();

//        public abstract void OnEnable();

//        public abstract void OnDisable();

//        public abstract void OnReceiveDamage();

//        public abstract void SecondaryAction(Vector3 pos);

//        public abstract void Attack(Vector3 pos);

//        public abstract void MoveTo(Vector2 pos);

//        public abstract OddControllerType GetControllerType();
//    }
    
//    [Serializable]
//    public class OddController : IOddController
//    {
//        static readonly Vector2[] JumpPilarCheck = new Vector2[4];
//        const float TPWaitTime = 0.2f;
//        const int BridgeMaxLength = 16;
//        const float DashMoveAmount = 3.0f;
//        const float DashTimeAmount = 0.4f;
//        //const float DashVFXTimeOffset = 0.3f;
//        const float DashSpeed = DashMoveAmount / DashTimeAmount;
//        const float DashStartOffset = 0.3f;
//        const float DashCooldown = 0.5f;
//        const float PossessionRadius = 0.5f;
//        const float PossessionVerticalRange = 1.25f;
        
//        const float DashFallSpeed = 20f;
//        const float DashFlySpeed = 20f;
//        const float DashMaxJump = 0.7f;

//        OddScript Odd;
//        //[SerializeField]
//        //ControllerDef m_Def;
//        [SerializeField]
//        ATTACK_TYPE m_AttackType;
//        [SerializeField]
//        Vector3 m_AfterJumpTarget;
//        [SerializeField]
//        IBlock m_BuildBlock;
//        [SerializeField]
//        InventoryItem m_BuildItem;
//        [SerializeField]
//        Vector2 m_BuildPos;
//        [SerializeField]
//        Vector2 m_BuildDir;
//        Vector2 m_DashDirection;

//        Vector2 m_WasdDirection;

//        float m_DashStartTime;
//        float m_DashStopTime;
//        float m_DashEndTime;
//        float m_NextDashTime;
//        //bool m_DashVFXSpawned;
//        //Vector2 m_DashTargetPos;

//        //PilarComponent m_BridgeBuildPilar;
//        //List<PilarComponent> m_BridgePilars;
//        //SpaceDirection m_BridgeDirection;
//        //float m_BridgeInitialHeight;
//        //float m_BridgeFinalHeight;
//        float m_NextActionTime;

//        InventoryItem m_SecondaryTargetReached;
//        Vector2 m_SecondaryTargetPosition;

//        void WhileJumping()
//        {
//            var timeDiff = m_NextActionTime - Time.time;
//            if (timeDiff < 0.6f)
//            {
//                //GameUtils.PFMovement(Position, new Vector2(m_TargetPosition.x, m_TargetPosition.z), m_Speed, 5.0f, 2.5f, m_Radius, 0.8f, Time.deltaTime, false, out Vector3 jumpMovement, out BlockComponent jumpBlock);
//                var targetPos = Odd.GetTargetPosition();
//                var movXZ = GameUtils.LinearMovement2D(new Vector2(Odd.Position.x, Odd.Position.z), new Vector2(targetPos.x, targetPos.z), 10.0f * Time.deltaTime);
//                var movY = GameUtils.LinearMovement1D(Odd.transform.position.y, targetPos.y, 6.0f * Time.deltaTime);
//                var jumpMovement = new Vector3(movXZ.x, movY, movXZ.y);
//                Odd.Position = Odd.Position + jumpMovement;
//                Odd.transform.Translate(jumpMovement, Space.World);
//            }
//            if (timeDiff > 0.0f)
//            {
//                return;
//            }
//            else if (timeDiff <= 0.0f)
//            {
//                Odd.State = ODD_STATE.COUNT;
//                Odd._TargetPos = m_AfterJumpTarget;
//            }
//        }

//        bool IsFailing(CBlock currentBlock, BridgeComponent currentBridge)
//        {
//            return (/*(currentBlock != null && currentBlock.Layer == 0) ||*/ currentBlock == null) && currentBridge == null;
//        }

//        void WhileFalling()
//        {
//            float fallSpeed = Mathf.Min(-9.81f * Mathf.Abs(((Odd.transform.position.y) * 0.1f)), -9.81f);
//            fallSpeed *= Time.deltaTime;
//            Odd.Position = Odd.Position + new Vector3(0.0f, fallSpeed, 0.0f);
//            Odd.transform.Translate(new Vector3(0.0f, fallSpeed, 0.0f), Space.World);
//        }

//        bool TryJump(Vector2 positionXZ, Vector2 targetXZ, Vector2 directionXZ, CBlock currentBlock)
//        {
//            if (currentBlock == null)
//                return false;
//            Vector2Int dir = Vector2Int.zero;
//            float nextBlockOffset = 0.0f;
//            if (Mathf.Abs(directionXZ.x) > Mathf.Abs(directionXZ.y))
//            {
//                if (directionXZ.x > 0.0f)
//                {
//                    dir.Set(1, 0);
//                    nextBlockOffset = 0.1f;
//                }
//                else if (directionXZ.x < 0.0f)
//                {
//                    dir.Set(-1, 0);
//                    nextBlockOffset = -0.9f;
//                }
//            }
//            else
//            {
//                if (directionXZ.y > 0.0f)
//                {
//                    dir.Set(0, 1);
//                    nextBlockOffset = 0.1f;
//                }
//                else if (directionXZ.y < 0.0f)
//                {
//                    dir.Set(0, -1);
//                    nextBlockOffset = -0.9f;
//                }
//            }
//            if (dir != Vector2Int.zero)
//            {
//                var curBlockPos = GameUtils.PosFromMapID(currentBlock.GetPilar().GetMapID());
//                var nextPilarPos = curBlockPos + dir;
//                var nextPilarID = GameUtils.MapIDFromPos(nextPilarPos);
//                var nextPilar = Manager.Mgr.MapQT.GetPilarWithMapID(nextPilarID);

//                float distToNextPilar = 0.0f;
//                if (dir.x > 0)
//                {
//                    distToNextPilar = Mathf.Abs(Odd.Position.x - nextPilar.transform.position.x);
//                }
//                else if (dir.x < 0)
//                {
//                    distToNextPilar = Mathf.Abs(Odd.Position.x - (nextPilar.transform.position.x + 1.0f));
//                }
//                else if (dir.y > 0)
//                {
//                    distToNextPilar = Mathf.Abs(Odd.Position.z - nextPilar.transform.position.z);
//                }
//                else if (dir.y < 0)
//                {
//                    distToNextPilar = Mathf.Abs(Odd.Position.z - (nextPilar.transform.position.z + 1.0f));
//                }
//                float targetDist = Vector2.Distance(positionXZ, targetXZ);
//                if (distToNextPilar < targetDist && nextPilar.GetBlocks().Count > 0)
//                {
//                    CBlock nextBlock = (CBlock)nextPilar.GetBlocks()[nextPilar.GetBlocks().Count - 1];
//                    float nextGround = nextBlock.GetHeight() + nextBlock.GetMicroHeight();
//                    float minJumpHeight = 0.4f;
//                    if (nextBlock.GetBlockType() == Def.BlockType.STAIRS)
//                    {
//                        minJumpHeight = 0.501f;
//                        switch (nextBlock.GetRotation())
//                        {
//                            case Def.RotationState.Default:
//                                if (dir.x > 0)
//                                    nextGround += 0.5f;
//                                break;
//                            case Def.RotationState.Left:
//                                if (dir.y < 0)
//                                    nextGround += 0.5f;
//                                break;
//                            case Def.RotationState.Half:
//                                if (dir.x < 0)
//                                    nextGround += 0.5f;
//                                break;
//                            case Def.RotationState.Right:
//                                if (dir.y > 0)
//                                    nextGround += 0.5f;
//                                break;
//                        }
//                    }
//                    var heightDiff = nextGround - Odd.Position.y;
//                    if (heightDiff < minJumpHeight || heightDiff > 0.8f)
//                        nextBlock = null;

//                    if (nextBlock != null)
//                    {
//                        Vector2 nextPos = Vector2.zero;
//                        if (dir.x == 0)
//                        {
//                            nextPos.x = Odd.Position.x;
//                            nextPos.y = nextPilar.transform.position.z + nextBlockOffset * dir.y;
//                        }
//                        else
//                        {
//                            nextPos.x = nextPilar.transform.position.x + nextBlockOffset * dir.x;
//                            nextPos.y = Odd.Position.z;
//                        }
//                        Odd.State = ODD_STATE.JUMPING;
//                        m_AfterJumpTarget = Odd.GetTargetPosition();
//                        Odd._TargetPos = new Vector3(nextPos.x, nextGround, nextPos.y);
//                        Odd.Animator.CrossFadeInFixedTime(OddScript.AnimHashes.Jump, 0.2f);
//                        m_NextActionTime = Time.time + 1.0f;
//                        return true;
//                    }
//                }
//            }
//            return false;
//        }

//        void SpiritMovement()
//        {
//            float yOffset = GameUtils.SinMovement1D(0.0f, 0.25f, 0.6f, Time.time);

//            Odd.transform.Translate(new Vector3(0.0f, (yOffset + Odd.Position.y) - Odd.transform.position.y, 0.0f), Space.World);
//        }

//        void BuildingEnd()
//        {
//            if ((m_NextActionTime - Time.time) < 0.35f)
//            {
//                switch (m_BuildItem.Type)
//                {
//                    case InvItemType.BLOCK:
//                        var buildItemFamily = BlockMaterial.MaterialFamilies[m_BuildItem.ID];

//                        if (m_BuildBlock.GetLength() + 0.5f < -BlockMeshDef.MidMesh.VertexHeight[0].y && m_BuildBlock.GetMaterialFamily() == buildItemFamily)
//                        {
//                            m_BuildBlock.SetLength(m_BuildBlock.GetLength() + 0.5f);
//                            m_BuildBlock.SetHeight(m_BuildBlock.GetHeight() + 0.5f);
//                        }
//                        else
//                        {
//                            var lastBlockHeight = m_BuildBlock.GetHeight() + m_BuildBlock.GetMicroHeight();
//                            var block = m_BuildBlock.GetPilar().AddBlock();
//                            //Manager.Mgr.Map.Record(new MapCommand(
//                            //    MapCommandType.PLACED_BLOCK, buildItemFamily.FamilyInfo.FamilyName, m_BuildBlock.GetPilar().GetMapID(), m_BuildBlock.GetPilar().GetBlocks().IndexOf(m_BuildBlock)));
//                            block.SetLayer(m_BuildBlock.GetLayer());
//                            block.GetLayerRnd().enabled = false;
//                            block.SetMaterialFamily(buildItemFamily);
//                            block.SetHeight(lastBlockHeight + 0.5f + ((m_BuildBlock.GetBlockType() == Def.BlockType.STAIRS) ? 0.5f : 0.0f));
//                            block.SetLength(0.5f);
//                            block.SetMicroHeight(0.0f);
//                            block.SetRotation((Def.RotationState)UnityEngine.Random.Range(0, Def.RotationStateCount)); // Manager.Mgr.BuildRNG.Next((int)BlockRotation.COUNT);
//                            Debug.Log("Contructed Block must be converted into CBlock");
//                        }
//                        break;
//                    case InvItemType.BOMB:
//                        var height = m_BuildBlock.GetHeight() + m_BuildBlock.GetMicroHeight();
//                        Vector3 bombPos = new Vector3(m_BuildPos.x, height, m_BuildPos.y);
//                        //Manager.Mgr.Map.Record(new MapCommand(
//                        //    MapCommandType.PLACED_BOMB, $"{bombPos.x}:{bombPos.y}:{bombPos.z}", -1, -1));
//                        var bomb = new GameObject("ClassicBomb").AddComponent<BombComponent>();
//                        bomb.transform.Translate(bombPos, Space.World);
//                        bomb.SetBomb("BombClassic", 2.0f, 2.5f, 100.0f, 5.0f);
//                        break;
//                    case InvItemType.BRIDGE:
//                        //Manager.Mgr.PlaceBridge();
//                        ////PlaceBridge();
//                        break;
//                }
//                Odd.Weapon.Renderer.enabled = true;
//                Odd.State = ODD_STATE.COUNT;
//            }
//        }

//        void AnimationUpdate(Vector3 movement)
//        {
//            if (movement == Vector3.zero)
//            {
//                if (Odd.State != ODD_STATE.IDLE)
//                {
//                    Odd.Animator.CrossFadeInFixedTime(OddScript.AnimHashes.Idle, 0.2f);
//                    Odd.State = ODD_STATE.IDLE;
//                }
//            }
//            else
//            {
//                if (Odd.State != ODD_STATE.WALKING)
//                {
//                    Odd.Animator.CrossFadeInFixedTime(OddScript.AnimHashes.Walk, 0.2f);
//                    Odd.State = ODD_STATE.WALKING;
//                }
//            }
//        }

//        void OnTeleport()
//        {
//            Ray ray = Manager.Mgr.m_Camera.ScreenPointToRay(Input.mousePosition);
//            int mask = (1 << 8);
//            bool blockHit = Physics.Raycast(ray, out RaycastHit mouseHit, 1000f, mask);
//            if(blockHit)
//            {
//                var block = mouseHit.transform.gameObject.GetComponent<CBlock>();
//                var newPos = new Vector3(mouseHit.point.x, block.GetHeight() + block.GetMicroHeight(), mouseHit.point.z);
//                float stairOffset = 0f;
//                if (block.GetBlockType() == Def.BlockType.STAIRS)
//                {
//                    stairOffset = GameUtils.GetStairYOffset(block.transform.position, block.GetRotation(), newPos);
//                }
//                newPos.y += stairOffset;
//                LookAt(new Vector2(newPos.x, newPos.z));
//                Odd._TargetPos = newPos;
//                var posOffset = newPos - Odd.transform.position;
//                Odd.transform.Translate(posOffset, Space.World);
//                Odd.Position = newPos;
//                m_NextActionTime = Time.time + TPWaitTime;
//            }
//            //var targetMapID = GameUtils.MapIDFromPosition(new Vector2(Odd.GetTargetPosition().x, Odd.GetTargetPosition().z));
//            //var pilar = Manager.Mgr.Pilars[targetMapID];
//            //var block = pilar.Blocks[pilar.Blocks.Count - 1];
//            //var newPos = new Vector3(Odd.GetTargetPosition().x, block.Height + block.MicroHeight, Odd.GetTargetPosition().z);
//            //var posOffset = newPos - Odd.transform.position;
//            //Odd.transform.Translate(posOffset, Space.World);
//            //Odd.Position = newPos;
//            //m_NextActionTime = Time.time + TPWaitTime;
//        }

//        void OnDashEnd()
//        {
//            var dashVFX = GameObject.Instantiate(AssetContainer.Mgr.TriVFXGameObjects[0]);
//            dashVFX.SetActive(true);
//            dashVFX.transform.Translate(new Vector3(Odd.transform.position.x, Odd.Position.y, Odd.transform.position.z), Space.World);
//            var pos = dashVFX.GetComponent<PossessionVFXComponent>();
//            pos.Set();

//            Odd._Speed = OddScript.OddSpeed;
//            Odd._FlySpeed = OddScript.OddFlySpeed;
//            Odd._FallSpeed = OddScript.OddFallSpeed;
//            Odd._MaxJump = OddScript.OddMaxJump;

//            m_NextDashTime = Time.time + DashCooldown;
//            Odd._TargetPos = Odd.Position;

//            // Posses near
//            //for (int i = 0; i < Manager.Mgr.Strucs.Count; ++i)
//            //{
//            //    var curStruc = Manager.Mgr.Strucs[i];
//            //    for (int j = 0; j < curStruc.LivingEntities.Count; ++j)
//            //    {
//            //        var curLE = curStruc.LivingEntities[j];
//            //        if (curLE == null)
//            //            continue;
//            //        if (curLE.GetLEType() != Def.LivingEntityType.Monster)
//            //            continue;
//            //        var distXZ = Vector2.Distance(new Vector2(Odd.Position.x, Odd.Position.z), new Vector2(curLE.transform.position.x, curLE.transform.position.z));
//            //        if (distXZ > PossessionRadius)
//            //            continue;
//            //        var distY = Mathf.Abs(curLE.transform.position.y - Odd.Position.y);
//            //        if (distY > PossessionVerticalRange)
//            //            continue;

//            //        var mon = (MonsterScript)curLE;
//            //        //for(int k = 0; k < (int)AI.AIState.COUNT; ++k)
//            //        //{
//            //        //    m_PossessedFunctions[k] = m_Possessed.AI.GetFunction((AI.AIState)k);
//            //        //    m_Possessed.AI.SetFunction(AI.AIFunction.CreateEmpty((AI.AIState)k));
//            //        //}
//            //        //m_Possessed.AI.ChangeStateTo(AI.AIState.IDLE);

//            //        LivingEntityDef def = new LivingEntityDef()
//            //        {
//            //            Speed = mon.Info.BaseSpeed,
//            //            Radius = mon.GetRadius(),
//            //            Height = mon.GetHeight(),
//            //            MaxJump = mon.GetMaxJump(),
//            //            FlySpeed = mon.GetFlySpeed(),
//            //            FallSpeed = mon.GetFallSpeed(),
//            //        };
//            //        Odd.PossessedMonster = mon;
//            //        Odd.State = ODD_STATE.POSSESSED;
//            //        mon.SetEnemy(Odd);
//            //        mon.Info.Team = MonsterTeam.OddTeam;
//            //        Odd.SetController(OddControllerType.MONSTER, def);
//            //        var posesLight = new GameObject("PossessingLight").AddComponent<OddPossessingLight>();
//            //        posesLight.transform.position = Odd.Position + new Vector3(0f, mon.GetHeight() * 0.5f, 0f);
//            //        posesLight.Set();
//            //        return;
//            //        //ControllerDef def;
//            //        //def.AttackRange = mon.Info.AttackRange;
//            //        //def.FallSpeed = 3.0f;
//            //        //def.UpSpeed = 6.0f;
//            //        //def.PossessedMonster = mon;
//            //        //def.Radius = mon.GetRadius();
//            //        //def.Speed = mon.Info.BaseSpeed;
//            //        //def.WalkingJumpHeight = mon._MaxJump;
//            //        //def.AngularSpeed = mon._AngularSpeed;
//            //        //Odd.State = ODD_STATE.POSSESSED;
//            //        //Odd.SetController(OddControllerType.MONSTER, def);
//            //        //mon.SetEnemy(Odd);
//            //        //mon.Info.Team = MonsterTeam.OddTeam;
//            //        //return;



//            //        //Odd.MeshRenderer.enabled = false;
//            //        //Odd.FaceRenderer.enabled = false;
//            //        ////Odd.Weapon.Renderer.enabled = false;

//            //        //m_Def.PossessedMonster.transform.SetParent(Odd.transform);
//            //        //m_Def.PossessedMonster.transform.localPosition = Vector3.zero;
//            //        //m_Def.PossessedMonster.enabled = false;
//            //        //m_Def.PossessedMonster.SpriteCC.enabled = true;
//            //        //m_Def.PossessedMonster.SpriteSR.enabled = true;
//            //        //var posAttribs = m_Def.PossessedMonster.Attributes;
//            //        //posAttribs.Team = MonsterTeam.OddTeam;
//            //        //m_Def.PossessedMonster.Attributes = posAttribs;

//            //        //break;
//            //    }
//            //}


//            //if (Odd.State != ODD_STATE.POSSESSED)
//            //{
//            //    Odd.State = ODD_STATE.COUNT;
//            //    Odd.Weapon.Renderer.enabled = true;
//            //}
//            Odd.State = ODD_STATE.COUNT;
//            Odd.Weapon.Renderer.enabled = true;
//            Odd.Collider.enabled = true;
//        }

//        void WhileDashing()
//        {
//            if (Time.time > m_DashEndTime)
//            {
//                OnDashEnd();
//                return;
//            }
//            else if (Time.time > m_DashStopTime || Time.time < m_DashStartTime)
//            {
//                return;
//            }

//            bool onGround = Odd.UpdateMovement(true, Odd.Position.y, Time.deltaTime, out Vector3 movement, out IBlock cb, out BridgeComponent bc);
//            Odd.Position += movement;
//            Odd.transform.Translate(movement, Space.World);

//            //var onGround = GameUtils.PFMovement(Odd.Position, m_DashTargetPos, DashSpeed, 20.0f, 20.0f, Odd._Radius, 0.7f, Time.deltaTime, true, out Vector3 movement,
//            //    out BlockComponent currentBlock, out BridgeComponent currentBridge);
//            //Odd.Position += movement;
//            //Odd.transform.Translate(movement, Space.World);

//            //if(!m_DashVFXSpawned && Time.time > (m_DashStartTime + DashVFXTimeOffset))
//            //{
//            //    var dashVFX = GameObject.Instantiate(AssetContainer.Mgr.TriVFXGameObjects[0]);
//            //    dashVFX.SetActive(true);
//            //    dashVFX.transform.Translate(Position, Space.World);
//            //    var pos = dashVFX.GetComponent<PossessionVFXComponent>();
//            //    pos.Set();
//            //    m_DashVFXSpawned = true;
//            //}
//        }

//        bool OnPossess()
//        {
//            Ray ray = Manager.Mgr.m_Camera.ScreenPointToRay(Input.mousePosition);
//            const int mask = (1 << Manager.BlockLayer) | (1 << Manager.BridgeLayer);
//            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, mask))
//            {
//                if (!Physics.Raycast(ray, out hit, 1000f, 1 << Manager.RayPlaneLayer))
//                    return false;
//            }

//            Odd.State = ODD_STATE.POSSESSING;
//            var dist = Vector2.Distance(new Vector2(hit.point.x, hit.point.z), new Vector2(Odd.Position.x, Odd.Position.z));
//            m_DashDirection.Set(hit.point.x - Odd.Position.x, hit.point.z - Odd.Position.z);
//            m_DashDirection.Normalize();
//            if (dist > DashMoveAmount)
//            {
//                //m_DashTargetPos.Set(Odd.Position.x + m_DashDirection.x * DashMoveAmount, Odd.Position.z + m_DashDirection.y * DashMoveAmount);
//                Odd._TargetPos = new Vector3(Odd.Position.x + m_DashDirection.x * DashMoveAmount, 0f, Odd.Position.z + m_DashDirection.y * DashMoveAmount);
//            }
//            else
//            {
//                //m_DashTargetPos.Set(rayPlane.point.x, rayPlane.point.z);
//                Odd._TargetPos = hit.point;
//            }
//            //Odd._TargetPos = new Vector3(m_DashTargetPos.x, Odd.GetTargetPosition().y, m_DashTargetPos.y);
//            LookAt(new Vector2(Odd.GetTargetPosition().x, Odd.GetTargetPosition().z));
//            m_DashStartTime = Time.time + DashStartOffset;
//            m_DashStopTime = m_DashStartTime + DashTimeAmount;
//            m_DashEndTime = (Time.time - DashStartOffset) + 0.9f;
//            //m_DashVFXSpawned = false;
//            Odd.Animator.Play(OddScript.AnimHashes.Possess);
//            Odd.Collider.enabled = false;
//            Odd.Weapon.Renderer.enabled = false;
//            Odd._Speed = DashSpeed;
//            Odd._MaxJump = DashMaxJump;
//            Odd._FlySpeed = DashFlySpeed;
//            Odd._FallSpeed = DashFallSpeed;
//            return true;
//        }

//        public override void OnReceiveDamage()
//        {

//        }

//        public override void FixedUpdate()
//        {
//            if (Odd.State == ODD_STATE.ATTACKING)
//            {
//                var timeOffset = m_NextActionTime - Time.time;
//                if (timeOffset < 0.2f)
//                {
//                    Odd.Animator.CrossFadeInFixedTime(OddScript.AnimHashes.Idle, 0.2f);
//                }
//            }
//        }

//        public override OddControllerType GetControllerType()
//        {
//            return OddControllerType.ODD;
//        }

//        //public override ControllerDef GetControllerDef()
//        //{
//        //    return m_Def;
//        //}

//        public override void LateUpdate()
//        {

//        }

//        public override void MoveTo(Vector2 pos)
//        {
//            if (Odd.State == ODD_STATE.JUMPING)
//            {
//                if ((m_NextActionTime - Time.time) < 0.6f)
//                    return;
//                Odd.State = ODD_STATE.COUNT;
//                m_NextActionTime = Time.time;
//                //Animator.CrossFadeInFixedTime(AnimHashes.Walk, 0.2f);
//            }
//            else if (Odd.State == ODD_STATE.POSSESSING)
//            {
//                return;
//            }
//            Odd._TargetPos = new Vector3(pos.x, 0.0f, pos.y);
//            var tempSA = m_SecondaryTargetReached;
//            tempSA.Type = InvItemType.COUNT;
//            m_SecondaryTargetReached = tempSA;
//            LookAt(pos);
//        }

//        public void LookAt(Vector2 pos)
//        {
//            var fwd = (new Vector3(pos.x, Odd.Position.y, pos.y) - Odd.Position).normalized;
//            if (fwd == Vector3.zero)
//                return;
//            Odd._TargetRot = Quaternion.LookRotation(fwd, Vector3.up);
//            //TargetOrientation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);
//        }

//        public void Build(Vector3 buildPos, InventoryItem item)
//        {
//            var buildPosXZ = new Vector2(buildPos.x, buildPos.z);
//            var pilarID = GameUtils.MapIDFromPos(GameUtils.TransformPosition(buildPosXZ));
//            //var pilar = Manager.Mgr.Pilars[pilarID];
//            var pilar = Manager.Mgr.MapQT.GetPilarWithMapID(pilarID);
//            var lastBlock = pilar.GetBlocks()[pilar.GetBlocks().Count - 1];
//            if (lastBlock.GetProp() != null && item.Type == InvItemType.BLOCK)
//                return;

//            //var lastBlockHeight = lastBlock.Height + lastBlock.MicroHeight;
//            //var heightDiff = lastBlockHeight - Position.y;
//            //if (heightDiff > 1.2 || heightDiff < -0.4)
//            //    return;
//            //m_TargetPosition = new Vector3(transform.position.x, 0.0f, transform.position.z);
//            LookAt(buildPosXZ);
//            m_BuildDir.Set(buildPos.x - Odd.Position.x, buildPos.z - Odd.Position.z);
//            m_BuildDir.Normalize();
//            float buildDistance = 1.0f;
//            if (item.Type == InvItemType.BLOCK)
//                buildDistance = 1.6f;
//            var curDistance = Vector3.Distance(buildPos, Odd.Position);
//            if (curDistance > buildDistance)
//            {
//                var nTarget = curDistance - buildDistance * 0.5f;
//                Odd._TargetPos = new Vector3(Odd.Position.x + m_BuildDir.x * nTarget, 0.0f, Odd.Position.z + m_BuildDir.y * nTarget);
//                m_SecondaryTargetReached = Odd.QuickInventory.Items[Odd.QuickInventory.CurItem];
//                m_SecondaryTargetPosition = buildPosXZ;
//                return;
//            }
//            m_BuildPos = buildPosXZ;

//            if (item.Type == InvItemType.BRIDGE)
//            {
//                if (!Manager.Mgr.CanBridgeBePlaced(m_BuildPos, m_BuildDir, Odd.Position.y))
//                    return;
//            }

//            Odd.Animator.CrossFadeInFixedTime(OddScript.AnimHashes.Build, 0.2f);
//            m_NextActionTime = Time.time + 1.0f;
//            Odd.State = ODD_STATE.BUILDING;
//            m_BuildBlock = lastBlock;
//            m_BuildItem = item;
//            Odd.Weapon.Renderer.enabled = false;
//        }

//        public override void Attack(Vector3 attackPos)
//        {
//            var attackPosXZ = new Vector2(attackPos.x, attackPos.z);
//            var curPosXZ = new Vector2(Odd.Position.x, Odd.Position.z);
//            var attackDir = (attackPosXZ - curPosXZ).normalized;
//            LookAt(attackPosXZ);
//            //var dist = Vector2.Distance(curPosXZ, attackPosXZ);
//            //if(dist > Odd.Weapon.AttackRange)
//            //{
//            //    var dir = (attackPosXZ - curPosXZ).normalized;
//            //    var nDist = dist - Odd.Weapon.AttackRange * 0.75f;
//            //    Odd._TargetPos = new Vector3(curPosXZ.x + dir.x * nDist, Odd.GetTargetPosition().y, curPosXZ.y + dir.y * nDist);
//            //    m_SecondaryTargetPosition = attackPosXZ;
//            //    m_SecondaryTargetReached = Odd.QuickInventory.Items[Odd.QuickInventory.CurItem];
//            //    return;
//            //}
//            var attackType = ATTACK_TYPE.TYPE_1;
//            VFXComponent vfxComp = Odd.SwordSlashVFX1;
//            int animHash = OddScript.AnimHashes.Attack1;
//            if (Odd.State == ODD_STATE.ATTACKING)
//            {
//                var timeOffset = m_NextActionTime - Time.time;
//                if (timeOffset >= 0.4f)
//                    return;

//                if (m_AttackType == ATTACK_TYPE.TYPE_1)
//                {
//                    attackType = ATTACK_TYPE.TYPE_2;
//                    vfxComp = Odd.SwordSlashVFX2;
//                    animHash = OddScript.AnimHashes.Attack2;
//                }
//            }
//            else
//            {
//                if (m_NextActionTime >= Time.time)
//                    return;
//            }

//            Odd.State = ODD_STATE.ATTACKING;
//            m_AttackType = attackType;
//            Odd.Animator.CrossFadeInFixedTime(animHash, 0.1f);
//            Odd._TargetPos = Odd.Position;
//            //var targetPosXZ = new Vector2(Odd.transform.position.x, Odd.transform.position.z);
//            //LookAt(attackPos);
//            Odd.Weapon.AttackDir = attackDir;
//            vfxComp.ResetVFX(0.35f);
//            Odd.Weapon.AttackStartTime = Time.time + 0.1f;
//            Odd.Weapon.AttackEndTime = Time.time + 0.8f;

//            var vfxAttack = Odd.Weapon.AttackDir * 0.7f;
//            var targetPos = Odd.Position + new Vector3(vfxAttack.x, 0.5f, vfxAttack.y);
//            vfxComp.transform.SetPositionAndRotation(targetPos, Odd.GetTargetOrientation() * Quaternion.Euler(270.0f, 0.0f, 0.0f));

//            m_NextActionTime = Time.time + 1.0f;
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

//        //bool CanBridgeBePlaced()
//        //{
//        //    Vector2Int dir = Vector2Int.zero;
//        //    var bridgeDir = SpaceDirection.COUNT;
//        //    if (Mathf.Abs(m_BuildDir.x) > Mathf.Abs(m_BuildDir.y))
//        //    {
//        //        if (m_BuildDir.x > 0.0f)
//        //        {
//        //            dir.Set(1, 0);
//        //            bridgeDir = SpaceDirection.SOUTH;
//        //        }
//        //        else
//        //        {
//        //            dir.Set(-1, 0);
//        //            bridgeDir = SpaceDirection.NORTH;
//        //        }
//        //    }
//        //    else
//        //    {
//        //        if (m_BuildDir.y > 0.0f)
//        //        {
//        //            dir.Set(0, 1);
//        //            bridgeDir = SpaceDirection.EAST;
//        //        }
//        //        else
//        //        {
//        //            dir.Set(0, -1);
//        //            bridgeDir = SpaceDirection.WEST;
//        //        }
//        //    }

//        //    var buildMapID = GameUtils.MapIDFromPosition(m_BuildPos);
//        //    var checkPos = GameUtils.PosFromID(buildMapID, Manager.MapWidth, Manager.MapHeight);
//        //    var buildingPilar = Manager.Mgr.Pilars[buildMapID];
//        //    BlockComponent buildBlock = null;
//        //    float closestDist = float.PositiveInfinity;
//        //    float initialHeight = 0f;
//        //    foreach (var bblock in buildingPilar.Blocks)
//        //    {
//        //        initialHeight = bblock.Height + bblock.MicroHeight;
//        //        if (buildBlock.blockType == BlockType.STAIRS)
//        //        {
//        //            switch (bblock.Rotation)
//        //            {
//        //                case BlockRotation.Default:
//        //                    if (bridgeDir == SpaceDirection.NORTH)
//        //                        initialHeight += 0.5f;
//        //                    break;
//        //                case BlockRotation.Left:
//        //                    if (bridgeDir == SpaceDirection.EAST)
//        //                        initialHeight += 0.5f;
//        //                    break;
//        //                case BlockRotation.Half:
//        //                    if (bridgeDir == SpaceDirection.SOUTH)
//        //                        initialHeight += 0.5f;
//        //                    break;
//        //                case BlockRotation.Right:
//        //                    if (bridgeDir == SpaceDirection.WEST)
//        //                        initialHeight += 0.5f;
//        //                    break;
//        //            }
//        //        }
//        //        var dist = Mathf.Abs(initialHeight - Odd.Position.y);
//        //        if (dist < closestDist)
//        //        {
//        //            buildBlock = bblock;
//        //            closestDist = dist;
//        //        }
//        //    }
//        //    int bridgeLength = 0;
//        //    for (int i = 0; i < Manager.BridgeMaxLength; ++i)
//        //    {
//        //        var currentPos = checkPos + dir * (i + 1);

//        //        var currentMapID = GameUtils.IDFromPos(currentPos, Manager.MapWidth, Manager.MapHeight);
//        //        var currentPilar = Manager.Mgr.Pilars[currentMapID];
//        //        if (currentPilar == null || (currentPilar != null && currentPilar.Blocks.Count == 0))
//        //            break;
//        //        Manager.Mgr.BridgeBuildPilars[bridgeLength++] = currentPilar;
//        //    }
//        //    // Blocks that the bridge can be attached to
//        //    BlockComponent TargetBlock = null;
//        //    int targetPilar = -1;
//        //    float heightDiff = 0.0f;
//        //    float finalheight = 0.0f;
//        //    for (int i = 0; i < bridgeLength; ++i)
//        //    {
//        //        var maxDist = (i + 1) * 0.3f;
//        //        var curPilar = Manager.Mgr.BridgeBuildPilars[i];
//        //        for (int j = 0; j < curPilar.Blocks.Count; ++j)
//        //        {
//        //            var curBlock = curPilar.Blocks[j];
//        //            if (curBlock == null)
//        //                continue;

//        //            if (curBlock.Layer == 0)
//        //                continue;

//        //            var blockHeight = curBlock.Height + curBlock.MicroHeight;
//        //            if (curBlock.blockType == BlockType.STAIRS)
//        //            {
//        //                switch (curBlock.Rotation)
//        //                {
//        //                    case BlockRotation.Default:
//        //                        if (bridgeDir == SpaceDirection.SOUTH)
//        //                            blockHeight += 0.5f;
//        //                        break;
//        //                    case BlockRotation.Left:
//        //                        if (bridgeDir == SpaceDirection.WEST)
//        //                            blockHeight += 0.5f;
//        //                        break;
//        //                    case BlockRotation.Half:
//        //                        if (bridgeDir == SpaceDirection.NORTH)
//        //                            blockHeight += 0.5f;
//        //                        break;
//        //                    case BlockRotation.Right:
//        //                        if (bridgeDir == SpaceDirection.EAST)
//        //                            blockHeight += 0.5f;
//        //                        break;
//        //                }
//        //            }
//        //            heightDiff = Mathf.Abs(initialHeight - blockHeight);
//        //            if (heightDiff > maxDist)
//        //                continue;
//        //            TargetBlock = curBlock;
//        //            finalheight = blockHeight;
//        //            break;
//        //        }
//        //        targetPilar = i;
//        //        if (TargetBlock != null)
//        //            break;
//        //    }
//        //    if (TargetBlock == null || targetPilar <= 0)
//        //        return false;

//        //    for (int i = bridgeLength; i < Manager.BridgeMaxLength; ++i)
//        //        Manager.Mgr.BridgeBuildPilars[i] = null;
//        //    Manager.Mgr.SetBridgeInfo(buildBlock, bridgeDir, bridgeLength, initialHeight, finalheight);

//        //    return true;
//        //}

//        //void PlaceBridge()
//        //{
//        //    Vector2 posOffset = Vector2.zero;
//        //    Vector2 initialPos = Vector2.zero;
//        //    BlockRotation rotation = BlockRotation.COUNT;
//        //    bool inverse = false;
//        //    Vector2Int dir = Vector2Int.zero;
//        //    switch (m_BridgeDirection)
//        //    {
//        //        case SpaceDirection.NORTH:
//        //            posOffset.x += (1.0f + StructureComponent.Separation);
//        //            initialPos = new Vector2(m_BridgeBuildPilar.transform.position.x, m_BridgeBuildPilar.transform.position.z);
//        //            inverse = true;
//        //            rotation = BlockRotation.Left;
//        //            dir.Set(-1, 0);
//        //            break;
//        //        case SpaceDirection.SOUTH:
//        //            rotation = BlockRotation.Right;
//        //            initialPos = new Vector2(m_BridgePilars[0].transform.position.x, m_BridgePilars[0].transform.position.z);
//        //            dir.Set(1, 0);
//        //            break;
//        //        case SpaceDirection.EAST:
//        //            rotation = BlockRotation.Default;
//        //            initialPos = new Vector2(m_BridgePilars[0].transform.position.x, m_BridgePilars[0].transform.position.z);
//        //            dir.Set(0, 1);
//        //            break;
//        //        case SpaceDirection.WEST:
//        //            posOffset.y += (1.0f + StructureComponent.Separation);
//        //            initialPos = new Vector2(m_BridgeBuildPilar.transform.position.x, m_BridgeBuildPilar.transform.position.z);
//        //            inverse = true;
//        //            rotation = BlockRotation.Half;
//        //            dir.Set(0, -1);
//        //            break;
//        //    }
//        //    Manager.Mgr.Map.Record(
//        //        new MapCommand(MapCommandType.BRIDGE_PLACED, m_BridgeDirection.ToString(), m_BridgeBuildPilar.MapID, m_BridgeBuildPilar.Blocks.Count - 1));
//        //    //float bridgeLength = Vector2.Distance(initialPos, finalPos);
//        //    float bridgeLength = m_BridgePilars.Count * (1.0f + StructureComponent.Separation);
//        //    //if (dir.x < 0 || dir.y < 0)
//        //    //{
//        //    //    initialPos = new Vector2(buildPilar.transform.position.x, buildPilar.transform.position.z);
//        //    //    inverse = true;
//        //    //}
//        //    //else
//        //    //{
//        //    //    initialPos = new Vector2(foundPilars[0].transform.position.x, foundPilars[0].transform.position.z);
//        //    //}

//        //    //Vector2 initialPos = new Vector2(foundPilars[0].transform.position.x, foundPilars[0].transform.position.z) + posOffset;
//        //    Vector2 finalPos = initialPos + new Vector2(dir.x, dir.y) * bridgeLength;

//        //    //float appearOffset = 0.0f;
//        //    //float startAnimOffset = targetPilar * 2.0f * WoodenBridgeAnimationComponent.AppearOffset;
//        //    float endAnimOffset = WoodenBridgeAnimationComponent.AnimationDuration * 0.5f * m_BridgePilars.Count * 2.0f;

//        //    var bridgeVFXGO = new GameObject("Bridge VFX");
//        //    bridgeVFXGO.transform.Translate(new Vector3(m_BridgePilars[0].transform.position.x + 0.5f, m_BridgeInitialHeight + 0.2f, m_BridgePilars[0].transform.position.z + 0.5f), Space.World);
//        //    var bridgeVFX = bridgeVFXGO.AddComponent<VFXComponent>();

//        //    bridgeVFX.SetVFX(new VFXDef(Def.VFXTarget.GENERAL, "BridgeBombExplosion", Def.VFXType.CAST, 0, Def.VFXFacing.FaceCameraFull, Def.VFXEnd.SelfDestroy, 24.0f));

//        //    BridgeComponent prevBridge = null;
//        //    for (int i = 0; i < m_BridgePilars.Count; ++i)
//        //    {
//        //        var curPilar = m_BridgePilars[i];
//        //        curPilar.DestroyBridge();
//        //        curPilar.AddBridge(0, BridgeType.SMALL, false);
//        //        curPilar.Bridge.Rotation = rotation;
//        //        if (prevBridge != null)
//        //            prevBridge.Next = curPilar.Bridge;
//        //        curPilar.Bridge.Previous = prevBridge;
//        //        prevBridge = curPilar.Bridge;

//        //        //var curBridgePos = new Vector2(foundPilars[i].transform.position.x, foundPilars[i].transform.position.z) + posOffset;
//        //        //var distanceFromInit = Vector2.Distance(initialPos, curBridgePos);
//        //        //float bridgeHeight = GameUtils.BridgeYPosition(bridgeLength, distanceFromInit, oddBlockHeight, lastBlockHeight);
//        //        //curPilar.Bridge.transform.Translate(new Vector3(0.0f, bridgeHeight, 0.0f), Space.World);
//        //        float[] objLH = new float[curPilar.Bridge.Objects.Length];
//        //        for (int j = 0; j < curPilar.Bridge.Objects.Length; ++j)
//        //        {
//        //            var curBridgeObj = curPilar.Bridge.Objects[j];
//        //            var curObjPos = new Vector2(curBridgeObj.transform.position.x, curBridgeObj.transform.position.z);// + posOffset;
//        //            float distanceFromInit = 0.0f;
//        //            if (inverse)
//        //            {
//        //                if (dir.x != 0)
//        //                {
//        //                    distanceFromInit = Mathf.Abs(curObjPos.x - finalPos.x);
//        //                }
//        //                else if (dir.y != 0)
//        //                {
//        //                    distanceFromInit = Mathf.Abs(curObjPos.y - finalPos.y);
//        //                }
//        //                distanceFromInit = bridgeLength - distanceFromInit;
//        //            }
//        //            else
//        //            {
//        //                if (dir.x != 0)
//        //                {
//        //                    distanceFromInit = Mathf.Abs(curObjPos.x - initialPos.x);
//        //                }
//        //                else if (dir.y != 0)
//        //                {
//        //                    distanceFromInit = Mathf.Abs(curObjPos.y - initialPos.y);
//        //                }
//        //            }

//        //            float objHeight = GameUtils.BridgeYPosition(bridgeLength, distanceFromInit, m_BridgeInitialHeight, m_BridgeFinalHeight, out float lh);
//        //            objLH[j] = lh;
//        //            curBridgeObj.transform.Translate(new Vector3(0.0f, objHeight, 0.0f), Space.Self);
//        //        }
//        //        curPilar.Bridge.UpdateCollision();

//        //        float startingDistance = i * (1.0f + StructureComponent.Separation);
//        //        float endingDistance = (i + 1) * (1.0f + StructureComponent.Separation);
//        //        float startingHeight = GameUtils.BridgeYPosition(bridgeLength, startingDistance, m_BridgeInitialHeight, m_BridgeFinalHeight, out float sLh);
//        //        float endingHeight = GameUtils.BridgeYPosition(bridgeLength, endingDistance, m_BridgeInitialHeight, m_BridgeFinalHeight, out float eLh);
//        //        curPilar.Bridge.SetBridgeHeightInfo(startingHeight, endingHeight, m_BridgeDirection);
//        //        float appearOffset = i * WoodenBridgeAnimationComponent.AppearOffset * curPilar.Bridge.Objects.Length;
//        //        curPilar.Bridge.StartAnimation(/*new Vector2(oddBlock.transform.position.x, oddBlock.transform.position.z),*/ appearOffset, endAnimOffset, objLH);
//        //        //appearOffset += curPilar.Bridge.Objects.Length * WoodenBridgeAnimationComponent.AppearOffset;
//        //        //startAnimOffset += curPilar.Bridge.Objects.Length * WoodenBridgeAnimationComponent.AnimationDuration;
//        //    }
//        //}

//        public override void SecondaryAction(Vector3 pos)
//        {
//            if (Odd.State == ODD_STATE.JUMPING)
//            {
//                if ((m_NextActionTime - Time.time) < 0.7)
//                    return;
//                Odd.State = ODD_STATE.WALKING;
//            }
//            else if (Odd.State == ODD_STATE.POSSESSING)
//            {
//                return;
//            }

//            var itemID = Odd.QuickInventory.CurItem;
//            var item = Odd.QuickInventory.Items[itemID];
//            switch (item.Type)
//            {
//                case InvItemType.WEAPON:
//                    Attack(new Vector2(pos.x, pos.z));
//                    break;
//                case InvItemType.BLOCK:
//                case InvItemType.BOMB:
//                case InvItemType.BRIDGE:
//                    Build(pos, item);
//                    break;
//            }
//        }

//        public override void StopController()
//        {

//        }

//        public override void SetController(LivingEntityDef controller, OddScript oddScript)
//        {            
//            //m_Def = controller;
//            Odd = oddScript;
//            Odd._AngularSpeed = controller.AngularSpeed;
//            Odd._MaxJump = controller.MaxJump;
//            Odd._Speed = controller.Speed;
//            Odd._FlySpeed = controller.FlySpeed;
//            Odd._FallSpeed = controller.FallSpeed;
//            Odd._Radius = controller.Radius;

//            //Odd._AngularSpeed = m_Def.AngularSpeed;
//            //Odd._MaxJump = m_Def.WalkingJumpHeight;
//            //Odd._Radius = m_Def.Radius;
//            //Odd._Speed = m_Def.Speed;
//            m_SecondaryTargetReached = new InventoryItem
//            {
//                Type = InvItemType.COUNT
//            };
//        }

//        bool TryJump2(Vector2 posXZ, float ground, Vector2 wasdDir, IBlock currentBlock, BridgeComponent currentBridge)
//        {
//            Vector2Int dir = Vector2Int.zero;
//            float nextBlockOffset = 0.0f;
//            if (Mathf.Abs(wasdDir.x) > Mathf.Abs(wasdDir.y))
//            {
//                if (wasdDir.x > 0.0f)
//                {
//                    dir.Set(1, 0);
//                    nextBlockOffset = 0.1f;
//                }
//                else if (wasdDir.x < 0.0f)
//                {
//                    dir.Set(-1, 0);
//                    nextBlockOffset = -0.9f;
//                }
//            }
//            else
//            {
//                if (wasdDir.y > 0.0f)
//                {
//                    dir.Set(0, 1);
//                    nextBlockOffset = 0.1f;
//                }
//                else if (wasdDir.y < 0.0f)
//                {
//                    dir.Set(0, -1);
//                    nextBlockOffset = -0.9f;
//                }
//            }
//            //Debug.Log($"WASD {wasdDir}, JUMP {dir}");
//            if(dir == Vector2.zero)
//                return false;

//            CPilar pilar = null;
//            if (currentBlock != null)
//                pilar = currentBlock.GetPilar();
//            else
//                pilar = currentBridge.Pilar;
//            Vector2Int curBlockPos = GameUtils.PosFromMapID(pilar.GetMapID());
//            var nextPilarPos = curBlockPos + dir;
//            var nextPilarID = GameUtils.MapIDFromPos(nextPilarPos);
//            var nextPilar = Manager.Mgr.MapQT.GetPilarWithMapID(nextPilarID);
//            const float blockSize = (1f + Def.BlockSeparation);
//            const float blockHalfSize = blockSize * 0.5f;

//            JumpPilarCheck[0] = new Vector2(nextPilar.transform.position.x, nextPilar.transform.position.z);
//            JumpPilarCheck[1] = new Vector2(nextPilar.transform.position.x + blockSize, nextPilar.transform.position.z);
//            JumpPilarCheck[2] = new Vector2(nextPilar.transform.position.x, nextPilar.transform.position.z + blockSize);
//            JumpPilarCheck[3] = new Vector2(nextPilar.transform.position.x + blockSize, nextPilar.transform.position.z + blockSize);

//            bool isNear = false;
//            for(int i = 0; i < JumpPilarCheck.Length; ++i)
//            {
//                float distance = Vector2.Distance(JumpPilarCheck[i], posXZ);
//                if(distance < blockHalfSize)
//                {
//                    isNear = true;
//                    break;
//                }
//            }
//            if (!isNear)
//                return false;

//            IBlock blockToJump = null;
//            float nextBlockGround = 0f;
//            for(int i = 0; i < nextPilar.GetBlocks().Count; ++i)
//            {
//                var nextBlock = nextPilar.GetBlocks()[i];
//                nextBlockGround = nextBlock.GetHeight() + nextBlock.GetMicroHeight();
//                if (nextBlock.GetBlockType() == Def.BlockType.STAIRS)
//                {
//                    switch (nextBlock.GetRotation())
//                    {
//                        case Def.RotationState.Default:
//                            if (dir.x > 0)
//                                nextBlockGround += 0.5f;
//                            break;
//                        case Def.RotationState.Left:
//                            if (dir.y < 0)
//                                nextBlockGround += 0.5f;
//                            break;
//                        case Def.RotationState.Half:
//                            if (dir.x < 0)
//                                nextBlockGround += 0.5f;
//                            break;
//                        case Def.RotationState.Right:
//                            if (dir.y > 0)
//                                nextBlockGround += 0.5f;
//                            break;
//                    }
//                }
//                var heightDiff = nextBlockGround - ground;
//                if (heightDiff > 0.8f)
//                    continue;
//                if (heightDiff < OddScript.OddMaxJump)
//                    continue;
//                blockToJump = nextBlock;
//                break;
//            }
//            if (blockToJump == null)
//                return false;

//            Vector2 nextPos = Vector2.zero;
//            if (dir.x == 0)
//            {
//                nextPos.x = Odd.Position.x;
//                nextPos.y = nextPilar.transform.position.z + nextBlockOffset * dir.y;
//            }
//            else
//            {
//                nextPos.x = nextPilar.transform.position.x + nextBlockOffset * dir.x;
//                nextPos.y = Odd.Position.z;
//            }
//            Odd.State = ODD_STATE.JUMPING;
//            m_AfterJumpTarget = Odd.GetTargetPosition();
//            Odd._TargetPos = new Vector3(nextPos.x, nextBlockGround, nextPos.y);
//            Odd.Animator.CrossFadeInFixedTime(OddScript.AnimHashes.Jump, 0.2f);
//            m_NextActionTime = Time.time + 1.0f;
//            return true;
//        }

//        public override void Update()
//        {
//            Odd._ElmUpdate();
//            //AIUpdate();

//            Odd.transform.rotation = Quaternion.Slerp(Odd.transform.rotation, Odd.GetTargetOrientation(), Time.deltaTime * Odd.GetAngularSpeed());

//            if (Odd.State == ODD_STATE.JUMPING)
//            {
//                WhileJumping();
//                return;
//            }
//            else if (Odd.State == ODD_STATE.POSSESSING)
//            {
//                WhileDashing();
//                return;
//            }
//            if (Input.GetKey(KeyCode.T) && m_NextActionTime < Time.time)
//            {
//                OnTeleport();
//            }

//            var positionXZ = new Vector2(Odd.Position.x, Odd.Position.z);
//            Vector2 movDir = Vector2.zero;
//            var camera = Manager.Mgr.m_Camera;
//            bool fwd = false;
//            if(Input.GetKey(KeyCode.W))
//            {
//                //movDir.Set(-1f, 0f);
//                movDir.Set(camera.transform.forward.x, camera.transform.forward.z);
//                fwd = true;
//            }
//            else if(Input.GetKey(KeyCode.S))
//            {
//                //movDir.Set(1f, 0f);
//                movDir.Set(-camera.transform.forward.x, -camera.transform.forward.z);
//                fwd = true;
//            }
//            if(Input.GetKey(KeyCode.A))
//            {
//                //movDir.Set(movDir.x, -1f);
//                if(fwd)
//                    movDir.Set((movDir.x + -camera.transform.right.x) * 0.5f, (movDir.y + -camera.transform.right.z) * 0.5f);
//                else
//                    movDir.Set(-camera.transform.right.x, -camera.transform.right.z);
//            }
//            else if(Input.GetKey(KeyCode.D))
//            {
//                //movDir.Set(movDir.x, 1f);
//                if (fwd)
//                    movDir.Set((movDir.x + camera.transform.right.x) * 0.5f, (movDir.y + camera.transform.right.z) * 0.5f);
//                else
//                    movDir.Set(camera.transform.right.x, camera.transform.right.z);
//            }
//            movDir.Normalize();
//            var wasdMovement = movDir * Odd.GetSpeed() * Time.deltaTime;
//            var nextPosXZ = positionXZ + wasdMovement;
//            if(Odd.State != ODD_STATE.ATTACKING)
//                LookAt(nextPosXZ);

//            Odd._TargetPos = new Vector3(positionXZ.x + 3f * wasdMovement.x, Odd.Position.y, positionXZ.y + 3f * wasdMovement.y);

//            //Odd.Position += new Vector3(nextPos.x, 0f, nextPos.y);
//            //Odd.transform.Translate(nextPos.x, 0f, nextPos.y, Space.World);


//            var targetXZ = new Vector2(Odd.GetTargetPosition().x, Odd.GetTargetPosition().z);
//            //var directionXZ = (targetXZ - positionXZ).normalized;

//            bool onGround = Odd.UpdateMovement(Odd.JumpToVoid < 2, Odd.Position.y, Time.deltaTime,
//                out Vector3 movement, out IBlock currentBlock, out BridgeComponent currentBridge);

//            ////bool onGround = GameUtils.PFMovement(Odd.Position, targetXZ, Odd._Speed, m_Def.UpSpeed, m_Def.FallSpeed, Odd.GetRadius(), m_Def.WalkingJumpHeight, Time.deltaTime, Odd.JumpToVoid < 2, out Vector3 movement, out BlockComponent currentBlock,
//            ////    out BridgeComponent currentBridge);

//            ////bool onGround = m_Movement.Update(Odd.Position, targetXZ, Odd.JumpToVoid < 2, Time.deltaTime, out Vector3 movement, out BlockComponent currentBlock, out BridgeComponent currentBridge);
//            if (IsFailing((CBlock)currentBlock, currentBridge) && Manager.Mgr.HideInfo)
//            {
//                WhileFalling();
//                return;
//            }

//            if (onGround)
//            {
//                if (Input.GetKey(KeyCode.Space))
//                {
//                    if (Time.time > m_NextDashTime)
//                    {
//                        if (OnPossess())
//                            return;
//                    }
//                }
//                //if (Odd.State == ODD_STATE.WALKING)
                
//                if((Mathf.Abs(movement.x) < 0.01f && Mathf.Abs(movDir.x) > 0.01f) || (Mathf.Abs(movement.z) < 0.01f && Mathf.Abs(movDir.y) > 0.01f))
//                {
//                   // if (TryJump(positionXZ, targetXZ, movDir, currentBlock))
//                   if(TryJump2(positionXZ, Odd.Position.y + movement.y, movDir, currentBlock, currentBridge))
//                        return;
//                }
//                SpiritMovement();
//            }

//            Odd.Position = new Vector3(Odd.Position.x, Odd.Position.y + movement.y, Odd.Position.z);
//            Odd.transform.Translate(new Vector3(0.0f, movement.y, 0.0f), Space.World);

//            if (Odd.State == ODD_STATE.BUILDING)
//                BuildingEnd();

//            if (m_NextActionTime > Time.time)
//                return;

//            var _movement = movement; _movement.y = 0.0f;
//            Odd.Position = Odd.Position + _movement;
//            Odd.transform.Translate(_movement, Space.World);

//            //if(m_SecondaryTargetReached.Type != InvItemType.COUNT)
//            //{
//            //    var dist = Vector2.Distance(new Vector2(Odd.Position.x, Odd.Position.z), new Vector2(Odd.GetTargetPosition().x, Odd.GetTargetPosition().z));
//            //    if(dist < 0.1f)
//            //    {
//            //        Odd.QuickInventory.CurItem = m_SecondaryTargetReached.InventoryPlace;
//            //        SecondaryAction(new Vector3(m_SecondaryTargetPosition.x, Odd.GetTargetPosition().y, m_SecondaryTargetPosition.y));
//            //        var tempSA = m_SecondaryTargetReached;
//            //        tempSA.Type = InvItemType.COUNT;
//            //        m_SecondaryTargetReached = tempSA;
//            //        return;
//            //    }
//            //}

//            if (onGround)
//            {
//                AnimationUpdate(movement);
//            }

//            Odd.RegisterLE(_movement != Vector3.zero);

//            Odd.QuickInventory.Update();
//        }
//    }
//}
