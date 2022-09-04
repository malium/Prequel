/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Assets;
////using cakeslice;
//using System;

//namespace Assets
//{
//    public enum ODD_STATE
//    {
//        IDLE,
//        WALKING,
//        ATTACKING,
//        RECEIVING_DAMAGE,
//        JUMPING,
//        BUILDING,
//        POSSESSING,
//        DEATH,

//        POSSESSED,

//        COUNT
//    }

//    enum ATTACK_TYPE
//    {
//        TYPE_1,
//        TYPE_2
//    }

//    public class OddScript : LivingEntity
//    {
//        public const float OddSpeed = 2.5f;
//        public const float OddAngularSpeed = 10f;
//        public const float OddFlySpeed = 5f;
//        public const float OddFallSpeed = 2.5f;
//        public const float OddMaxJump = 0.5f;

//        [SerializeField]
//        IOddController m_CurrentController;
//        [SerializeField]
//        IOddController[] m_Controllers;

//        public IOddController GetController(OddControllerType type)
//        {
//            return m_Controllers[(int)type];
//        }
//        public void SetController(OddControllerType type, LivingEntityDef def)
//        {
//            if (m_CurrentController.GetControllerType() == type)
//                return;
//            //try
//            //{
//                m_CurrentController.StopController();
//                bool wasMonster = m_CurrentController.GetControllerType() == OddControllerType.MONSTER;
//                m_CurrentController = GetController(type);
//                m_CurrentController.SetController(def, this);
//                if (wasMonster)
//                {
//                    m_CurrentController.Update();
//                }
//            //}
//            //catch (Exception e)
//            //{
//            //    Manager.ThrowException(e.Message);
//            //}
//        }

//        //[SerializeField]
//        //OddWeapon m_Weapon = null;
//        [SerializeField]
//        SkinnedMeshRenderer m_MeshRenderer = null;
//        [SerializeField]
//        SpriteRenderer m_FaceRenderer = null;
//        [SerializeField]
//        Vector3 m_Position;
//        [SerializeField]
//        ODD_STATE m_State;
//        [SerializeField]
//        Animator m_Animator = null;
//        [SerializeField]
//        FogManager m_FogMgr = null;
//        [SerializeField]
//        int m_JumpToVoid;
//        [SerializeField]
//        OddQuickInventory m_QuickInventory = null;
//        [SerializeField]
//        BoxCollider m_Collider = null;
//        VFXComponent m_SwordSlashVFX_1 = null;
//        VFXComponent m_SwordSlashVFX_2 = null;

//        //public OddWeapon Weapon
//        //{
//        //    get
//        //    {
//        //        return m_Weapon;
//        //    }
//        //}
//        public SkinnedMeshRenderer MeshRenderer
//        {
//            get
//            {
//                return m_MeshRenderer;
//            }
//        }
//        public SpriteRenderer FaceRenderer
//        {
//            get
//            {
//                return m_FaceRenderer;
//            }
//        }
//        public Vector3 Position
//        {
//            get
//            {
//                return m_Position;
//            }
//            set
//            {
//                m_Position = value;
//            }
//        }
//        public ODD_STATE State
//        {
//            get
//            {
//                return m_State;
//            }
//            set
//            {
//                m_State = value;
//            }
//        }
//        public Animator Animator
//        {
//            get
//            {
//                return m_Animator;
//            }
//        }
//        public FogManager FogMgr
//        {
//            get
//            {
//                return m_FogMgr;
//            }
//        }
//        public int JumpToVoid
//        {
//            get
//            {
//                return m_JumpToVoid;
//            }
//            set
//            {
//                m_JumpToVoid = value;
//            }
//        }
//        public OddQuickInventory QuickInventory
//        {
//            get
//            {
//                return m_QuickInventory;
//            }
//        }
//        public InvItemType CurrentInvItem
//        {
//            get
//            {
//                return m_QuickInventory.Items[m_QuickInventory.CurItem].Type;
//            }
//        }
//        public BoxCollider Collider
//        {
//            get
//            {
//                return m_Collider;
//            }
//        }
//        public VFXComponent SwordSlashVFX1
//        {
//            get
//            {
//                return m_SwordSlashVFX_1;
//            }
//        }
//        public VFXComponent SwordSlashVFX2
//        {
//            get
//            {
//                return m_SwordSlashVFX_2;
//            }
//        }
//        //public MonsterScript PossessedMonster;

//        public struct AnimHashes
//        {
//            public static readonly int Idle = Animator.StringToHash("Layer.Idle");
//            public static readonly int Attack1 = Animator.StringToHash("Layer.Attack1");
//            public static readonly int Attack2 = Animator.StringToHash("Layer.Attack2");
//            public static readonly int ReceiveDamage = Animator.StringToHash("Layer.ReceiveDamage");
//            public static readonly int Walk = Animator.StringToHash("Layer.Walk");
//            public static readonly int Jump = Animator.StringToHash("Layer.Jump");
//            public static readonly int Build = Animator.StringToHash("Layer.Build");
//            public static readonly int Possess = Animator.StringToHash("Layer.Possess");
//            public static readonly int Death = Animator.StringToHash("Layer.Death");
//        }

//        public new void OnControllerChange(IGameController controller)
//        {
//            if (controller.GetGameState() == GameState.PLAY)
//            {
//                m_QuickInventory.enabled = true;
//                m_FogMgr.gameObject.SetActive(true);
//                m_FogMgr.enabled = true;
//                SetController(OddControllerType.ODD, new LivingEntityDef()
//                {
//                    Speed = OddSpeed,
//                    AngularSpeed = OddAngularSpeed,
//                    MaxJump = OddMaxJump,
//                    Radius = m_MeshRenderer.bounds.extents.x > m_MeshRenderer.bounds.extents.z ? m_MeshRenderer.bounds.extents.x : m_MeshRenderer.bounds.extents.z,
//                    Height = Collider.size.y,
//                    FallSpeed = OddFallSpeed,
//                    FlySpeed = OddFlySpeed,
//                });
//                //PossessedMonster = null;
//            }
//            else
//            {
//                m_QuickInventory.enabled = false;
//                m_FogMgr.enabled = false;
//                m_FogMgr.gameObject.SetActive(false);
//                SetController(OddControllerType.NULL, new LivingEntityDef());
//            }
//        }

//        public OddScript()
//            :base(new LivingEntityDef()
//            {
//                TotalHealth = 1e8f,
//                Speed = OddSpeed,
//                AngularSpeed = OddAngularSpeed,
//                Radius = 1f,
//                MaxJump = OddMaxJump,
//                FlySpeed = OddFlySpeed,
//                FallSpeed = OddFallSpeed,
//                LEType = Def.LivingEntityType.ODD
//            })
//        {

//        }

//        public override Collider GetCollider()
//        {
//            return m_Collider;
//        }

//        protected override void OnReceiveDamage()
//        {
//            m_CurrentController.OnReceiveDamage();
//        }

//        protected override void OnReceiveElement()
//        {

//        }

//        private void Awake()
//        {
//            m_Controllers = new IOddController[(int)OddControllerType.COUNT];
//            m_CurrentController = new OddNullController();
//            m_Controllers[(int)OddControllerType.NULL] = m_CurrentController;
            
//            //m_PossessedFunctions = new AI.IAIFunction[(int)AI.AIState.COUNT];
//        }

//        public static void Init()
//        {
//            var odd = Manager.Mgr.OddGO.GetComponent<OddScript>();
//            odd.m_TargetPosition = new Vector3(odd.transform.position.x, 0.0f, odd.transform.position.z);
//            odd.Position = odd.transform.position;
//            odd.m_TargetOrientation = odd.transform.rotation;
//            odd.m_State = ODD_STATE.IDLE;
//            odd.m_Radius = odd.m_MeshRenderer.bounds.extents.x > odd.m_MeshRenderer.bounds.extents.z ? odd.m_MeshRenderer.bounds.extents.x : odd.m_MeshRenderer.bounds.extents.z;

//            var fogGO = new GameObject("FogManager");
//            odd.m_FogMgr = fogGO.AddComponent<FogManager>();

//            var swordSlashGO = new GameObject("Odd_Weapon_VFX_1");
//            swordSlashGO.transform.localScale = new Vector3(1.75f, 1.75f, 1.0f);
//            odd.m_SwordSlashVFX_1 = swordSlashGO.AddComponent<VFXComponent>();

//            var swordSlash2GO = Instantiate(swordSlashGO);
//            swordSlash2GO.name = "Odd_Weapon_VFX_2";
//            odd.m_SwordSlashVFX_2 = swordSlash2GO.GetComponent<VFXComponent>();

//            VFXDef vfxDef = new VFXDef(Def.VFXTarget.ODD, "Slash", Def.VFXType.CAST, 1, Def.VFXFacing.DontFaceAnything, Def.VFXEnd.Stop);

//            odd.m_SwordSlashVFX_1.SetVFX(vfxDef);
//            odd.m_SwordSlashVFX_1.StopVFX();
//            odd.m_SwordSlashVFX_1.Renderer.flipX = true;

//            odd.m_SwordSlashVFX_2.SetVFX(vfxDef);
//            odd.m_SwordSlashVFX_2.StopVFX();

//            odd.m_QuickInventory = new OddQuickInventory();
//            odd.m_QuickInventory.Start();
            
//            odd.JumpToVoid = 0;

//            odd.m_Height = odd.Collider.size.y;

//            odd.m_Controllers[(int)OddControllerType.ODD] = new OddController();
//            //odd.m_Controllers[(int)OddControllerType.MONSTER] = new OddMonsterController();
//        }

//        private void OnEnable()
//        {
//            //try
//            //{
//                m_CurrentController.OnEnable();
//            //}
//            //catch (Exception e)
//            //{
//            //    Manager.ThrowException(e.Message);
//            //}
//        }

//        private void OnDisable()
//        {
//            //try
//            //{
//                m_CurrentController.OnDisable();
//            //}
//            //catch (Exception e)
//            //{
//            //    Manager.ThrowException(e.Message);
//            //}
//        }
        
//        // Update is called once per frame
//        void Update()
//        {
//            //try
//            //{
//                m_CurrentController.Update();
//            //}
//            //catch (Exception e)
//            //{
//            //    Manager.ThrowException(e.Message);
//            //}
//        }

//        private void FixedUpdate()
//        {
//            //try
//            //{
//                m_CurrentController.FixedUpdate();
//            //}
//            //catch (Exception e)
//            //{
//            //    Manager.ThrowException(e.Message);
//            //}
//        }

//        private void OnGUI()
//        {
//            //try
//            //{
//                m_CurrentController.OnGUI();
//            //}
//            //catch (Exception e)
//            //{
//            //    Manager.ThrowException(e.Message);
//            //}
//        }

//        public void Attack(Vector3 pos)
//        {
//            m_CurrentController.Attack(pos);
//        }

//        public void SecondaryAction(Vector3 pos)
//        {
//            //try
//            //{
//                m_CurrentController.SecondaryAction(pos);
//            //}
//            //catch (Exception e)
//            //{
//            //    Manager.ThrowException(e.Message);
//            //}
//        }

//        public void MoveTo(Vector2 pos)
//        {
//            //try
//            //{
//                m_CurrentController.MoveTo(pos);
//            //}
//            //catch (Exception e)
//            //{
//            //    Manager.ThrowException(e.Message);
//            //}
//        }

//        //public void TakeDamage(float amount)
//        //{
//        //    if (amount < 0.0f)
//        //        amount = 0.0f;
//        //    m_Health -= amount;
//        //}

//        private void LateUpdate()
//        {
//            //try
//            //{
//                m_CurrentController.LateUpdate();
//            //}
//            //catch (Exception e)
//            //{
//            //    Manager.ThrowException(e.Message);
//            //}
//        }
//    }
//}