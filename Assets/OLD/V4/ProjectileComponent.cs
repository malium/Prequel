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
//    [Flags]
//    public enum ProjectileHitMask : byte
//    {
//        NONE = 0,
//        BLOCKS = 1,
//        PROPS = 2,
//        MONSTERS = 4,

//        LIVINIG_ENTITIES = (PROPS | MONSTERS),
//        ALL = (LIVINIG_ENTITIES | BLOCKS),
//    }

//    public enum ProjectileAreaDamageType
//    {
//        RECTANGULAR,
//        CIRCULAR,
//        CONIC,

//        COUNT
//    }

//    public enum ProjectileTravelType
//    {
//        LINEAR,
//        PARABOLIC,
//        TARGETTED,

//        COUNT
//    }

//    [Serializable]
//    public struct ProjectileDef
//    {
//        public Vector3 InitialPosition;
//        public Vector3 FinalPosition; // Linear and Parabolic
//        public ProjectileTravelType TravelType;
//        public ProjectileAreaDamageType AreaDamageType;
//        public float Damage;
//        public Def.DamageType DmgType;
//        public Rect RectangularArea;
//        public float CircularRadius;
//        public float ConicArcAngle;
//        public float ConicArcLength;
//        public int HitMask;
//        public Vector2 Speed; // (XZ, Y)
//        public LivingEntity Target; // Targetted
//        public LivingEntity Caster;
//        public List<int> AvoidMonsterFamilies;
//        public List<MonsterTeam> AvoidMonsterTeams;
//        public List<LivingEntity> Enemies;
//        public VFXDef CastVFX;
//        public VFXDef TravelVFX;
//        public VFXDef OnHitVFX;
//    }

//    class ProjectileComponent : VFXComponent
//    {
//        const float SelfHitTimeDelay = 0.5f;

//        [SerializeField]
//        ProjectileDef m_Def;
//        Func<bool> m_UpdateFN;
//        Func<List<LivingEntity>> m_AreaHitTestFN;
//        VFXComponent m_CastVFX;
//        public VFXComponent CastVFX
//        {
//            get
//            {
//                return m_CastVFX;
//            }
//        }
//        VFXComponent m_OnHitVFX;
//        public VFXComponent OnHitVFX
//        {
//            get
//            {
//                return m_OnHitVFX;
//            }
//        }
//        Action m_OnTravelVFXUpdate;
//        Vector3 m_Direction;
//        float m_StartTime;
//        IStruc[] m_NearStrucs;

//        [SerializeField]
//        SphereCollider m_Collider;

//        public SphereCollider Collider
//        {
//            get
//            {
//                return m_Collider;
//            }
//        }

//        bool LinearUpdate()
//        {
//            if (m_Direction == Vector3.zero)
//                return true;
//            var movement = m_Direction * m_Def.Speed.x * Time.deltaTime;
//            var dist = Vector3.Distance(transform.position, m_Def.FinalPosition);
//            var movDist = Vector3.Distance(transform.position + movement, m_Def.FinalPosition);
//            transform.Translate(movement, Space.World);
//            if (movDist > dist)
//                return true;
//            return false;
//        }

//        bool ParabolicUpdate()
//        {
//            var movementXZ = new Vector2(m_Direction.x, m_Direction.z) * m_Def.Speed.x * Time.deltaTime;
//            var t = Time.time - m_StartTime;
//            var yPos = -0.5f * 9.81f * (t * t) + m_Def.Speed.y * t + m_Def.InitialPosition.y;
//            var movementY = yPos - transform.position.y;

//            var movement = new Vector3(movementXZ.x, movementY, movementXZ.y);

//            //var dist = Vector3.Distance(transform.position, m_Def.FinalPosition);
//            //var movDist = Vector3.Distance(transform.position, m_Def.FinalPosition);
//            transform.Translate(movement, Space.World);
//            if (transform.position.y <= -50)
//                return true;
//            //if (movDist > dist)
//            //    return true;
//            return false;
//        }

//        bool TargettedUpate()
//        {
//            if (m_Def.Target == null)
//                return true;

//            m_Direction = (m_Def.Target.transform.position - transform.position).normalized;

//            var movement = m_Direction * m_Def.Speed.x * Time.deltaTime;
//            var dist = Vector3.Distance(transform.position, m_Def.FinalPosition);
//            var movDist = Vector3.Distance(transform.position + movement, m_Def.FinalPosition);
//            transform.Translate(movement, Space.World);
//            if (movDist > dist)
//                return true;
//            return false;
//        }

//        bool OnCollisionTest(Collider other, out LivingEntity hit)
//        {
//            bool blockCollide = (m_Def.HitMask & (int)ProjectileHitMask.BLOCKS) == (int)ProjectileHitMask.BLOCKS;
//            bool monsterCollide = (m_Def.HitMask & (int)ProjectileHitMask.MONSTERS) == (int)ProjectileHitMask.MONSTERS;
//            bool propCollide = (m_Def.HitMask & (int)ProjectileHitMask.PROPS) == (int)ProjectileHitMask.PROPS;
//            hit = null;
//            if (blockCollide)
//            {
//                if(other.gameObject.layer == Def.RCLayerBlock)
//                {
//                    return true;
//                }
//            }
//            /*if(monsterCollide || propCollide)
//            {
//                if(other.gameObject.tag == OddWeapon.TAG)
//                {
//                    if (monsterCollide && m_Def.Caster.GetLEType() != Def.LivingEntityType.ODD)
//                        return true;
//                }
//                else if(other.gameObject.layer == Def.RCLayerLE)
//                {
//                    var le = other.gameObject.transform.parent.gameObject.GetComponent<LivingEntity>();
//                    hit = le;
//                    switch (le.GetLEType())
//                    {
//                        case Def.LivingEntityType.ODD:
//                            if (monsterCollide && Manager.Mgr.OddScript.State != ODD_STATE.POSSESSED)
//                            {
//                                return true;
//                            }
//                            break;
//                        case Def.LivingEntityType.Monster:
//                            if (monsterCollide)
//                            {
//                                bool avoidCaster = Time.time < (m_StartTime + SelfHitTimeDelay);
//                                var mon = (MonsterScript)le;
//                                if (avoidCaster && mon == m_Def.Caster)
//                                    return false;
//                                if (m_Def.Enemies.Contains(le))
//                                {
//                                    return true;
//                                }
//                                var familyID = Monsters.FamilyDict[mon.Info.Name];
//                                //var familyID = mon.Info.MonsterID;
//                                //var team = mon.Info.Team;
//                                if (m_Def.AvoidMonsterFamilies.Contains(familyID))
//                                    return false;
//                                //if (m_Def.AvoidMonsterTeams.Contains(team))
//                                    //return false;

//                                return true;
//                            }
//                            break;
//                        case Def.LivingEntityType.Prop:
//                            if (propCollide)
//                            {
//                                return true;
//                            }
//                            break;
//                    }
//                }
//            }*/

//            return false;
//        }

//        List<LivingEntity> RectangularTargetTest()
//        {
//            return new List<LivingEntity>();
//        }

//        List<LivingEntity> CircularTargetTest()
//        {
//            List<LivingEntity> hits = new List<LivingEntity>();
//            //var pos = new Vector2(transform.position.x, transform.position.z);
//            ////Manager.GetStrucsNoAlloc(ref m_NearStrucs, pos);
//            ////var structs = Manager.GetStrucs(pos);
//            //for (int i = 0; i < m_NearStrucs.Length; ++i)
//            //{
//            //    var struc = m_NearStrucs[i];
//            //    if (struc == null)
//            //        break;
//            //    for (int j = 0; j < struc.GetLivingEntities().Count; ++j)
//            //    {
//            //        var le = struc.GetLivingEntities()[j];
//            //        if (le == null)
//            //            continue;
//            //        bool hit = GameUtils.SphericalCheck(transform.position, m_Def.CircularRadius, le.transform.position, le.GetHeight(), le.GetRadius());
//            //        if (hit)
//            //        {
//            //            hits.Add(le);
//            //            //le.ReceiveDamage(m_Def.DmgType, m_Def.Damage);
//            //        }
//            //    }
//            //}
//            //// OddCheck
//            ////var odd = Manager.Mgr.OddScript;
//            ////if(odd.State != ODD_STATE.POSSESSED)
//            ////{
//            //
//            ////    bool hit = GameUtils.SphericalCheck(transform.position, m_Def.CircularRadius, odd.Position, odd.GetHeight(), odd.GetRadius());
//            ////    if (hit)
//            ////    {
//            ////        hits.Add(odd);
//            ////        //odd.ReceiveDamage(m_Def.DmgType, m_Def.Damage);
//            ////    }
//            ////}
//            return hits;
//        }

//        List<LivingEntity> ConicTargetTest()
//        {
//            return new List<LivingEntity>();
//        }

//        void FilterLE(ref List<LivingEntity> hits)
//        {
//            //bool removeCaster = Time.time < (m_StartTime + SelfHitTimeDelay);
//            //for (int i = 0; i < hits.Count; )
//            //{
//            //    var cur = hits[i];
//            //    if(removeCaster && cur == m_Def.Caster)
//            //    {
//            //        hits.RemoveAt(i);
//            //        continue;
//            //    }
//            //    if(m_Def.Enemies.Contains(cur))
//            //    {
//            //        ++i;
//            //        continue;
//            //    }

//            //    if(cur.GetLEType() == Def.LivingEntityType.Monster)
//            //    {
//            //        var mon = (MonsterScript)cur;
//            //        //var team = mon.Info.Team;
//            //        var familyID = Monsters.FamilyDict[mon.Info.Name];
//            //        if (m_Def.AvoidMonsterFamilies.Contains(familyID))// && m_Def.AvoidMonsterTeams.Contains(team))
//            //        {
//            //            hits.RemoveAt(i);
//            //            continue;
//            //        }
//            //    }

//            //    ++i;
//            //}
//        }

//        void DamageLE(List<LivingEntity> hits)
//        {
//            //foreach (LivingEntity le in hits)
//            //{
//            //    le.ReceiveDamage(m_Def.DmgType, m_Def.Damage);
//            //    var pos = new Vector3(le.transform.position.x, transform.position.y, le.transform.position.z);
//            //    var dir = (le.transform.position - transform.position).normalized;
//            //    switch (le.GetLEType())
//            //    {
//            //        case Def.LivingEntityType.Monster:
//            //            var mon = le as MonsterScript;
//            //            mon.SetEnemy(m_Def.Caster);
//            //            mon.CastBloodTrails(pos, dir);
//            //            break;
//            //        case Def.LivingEntityType.Prop:
//            //            var prop = le as PropScript;
//            //            prop.CastDamageParticles(pos, dir);
//            //            break;
//            //    }
//            //}
//        }

//        void OnTargetReached(LivingEntity hit = null)
//        {
//            var hits = m_AreaHitTestFN();
//            if (hit != null && !hits.Contains(hit))
//                hits.Add(hit);
//            FilterLE(ref hits);
//            DamageLE(hits);

//            if(m_OnHitVFX != null)
//            {
//                m_OnHitVFX.transform.Translate(transform.position, Space.World);
//                m_OnHitVFX.ResetVFX();
//            }
//            //if(m_Def.OnHitVFX.IsValid())
//            //{

//            //    var onHitGO = new GameObject(gameObject.name + "OnHit");
//            //    onHitGO.transform.Translate(transform.position, Space.World);
//            //    onHitGO.transform.localScale = new Vector3(m_Def.OnHitSize, m_Def.OnHitSize, 1.0f);
//            //    var onHitVFX = onHitGO.AddComponent<VFXComponent>();
//            //    onHitVFX.SetVFX(m_Def.OnHitVFX);
//            //    onHitVFX.ResetVFX();
//            //}

//            GameObject.Destroy(gameObject);
//        }

//        private void Awake()
//        {
//            //m_UpdateFN = () => { return false; };
//            //m_OnDamageFN = () => { };
//            //m_OnTravelVFXUpdate = () => { };
//        }

//        public void SetProjectile(ProjectileDef definition)
//        {
//            m_Def = definition;
//            transform.Translate(m_Def.InitialPosition - transform.position, Space.World);
//            switch (m_Def.TravelType)
//            {
//                case ProjectileTravelType.LINEAR:
//                    m_UpdateFN = LinearUpdate;
//                    break;
//                case ProjectileTravelType.PARABOLIC:
//                    m_UpdateFN = ParabolicUpdate;
//                    break;
//                case ProjectileTravelType.TARGETTED:
//                    m_UpdateFN = TargettedUpate;
//                    if (m_Def.Target == null)
//                        throw new Exception("Trying to create a Targetted projectile, but the target was null");
//                    break;
//            }
//            switch (m_Def.AreaDamageType)
//            {
//                case ProjectileAreaDamageType.RECTANGULAR:
//                    m_AreaHitTestFN = RectangularTargetTest;
//                    break;
//                case ProjectileAreaDamageType.CIRCULAR:
//                    m_AreaHitTestFN = CircularTargetTest;
//                    m_NearStrucs = new CStruc[10];
//                    break;
//                case ProjectileAreaDamageType.CONIC:
//                    m_AreaHitTestFN = ConicTargetTest;
//                    break;
//            }
//            if (m_Def.CastVFX.IsValid())
//            {
//                var castGO = new GameObject(gameObject.name + "_CAST");
//                castGO.transform.Translate(transform.position);
//                m_CastVFX = castGO.AddComponent<VFXComponent>();
//                m_CastVFX.SetVFX(m_Def.CastVFX);
//                m_CastVFX.ResetVFX();
//            }
//            if(m_Def.TravelVFX.IsValid())
//            {
//                SetVFX(m_Def.TravelVFX);
//                m_OnTravelVFXUpdate = OnUpdate;
//            }
//            else
//            {
//                m_OnTravelVFXUpdate = () => { };
//            }
//            if(m_Def.OnHitVFX.IsValid())
//            {
//                var onHitGO = new GameObject(gameObject.name + "_ONHIT");
//                m_OnHitVFX = onHitGO.AddComponent<VFXComponent>();
//                m_OnHitVFX.SetVFX(m_Def.OnHitVFX);
//                m_OnHitVFX.ResetVFX(20000.0f);
//            }

//            if(m_Def.HitMask != (int)ProjectileHitMask.NONE)
//            {
//                m_Collider = gameObject.AddComponent<SphereCollider>();
//                m_Collider.isTrigger = true;
//                var rb = gameObject.AddComponent<Rigidbody>();
//                rb.useGravity = false;
//                rb.constraints = RigidbodyConstraints.FreezePosition;
//            }

//            m_Direction = (m_Def.FinalPosition - m_Def.InitialPosition).normalized;
//            m_StartTime = Time.time;
//        }

//        private void OnTriggerEnter(Collider other)
//        {
//            if(OnCollisionTest(other, out LivingEntity hit))
//            {
//                OnTargetReached(hit);
//            }
//        }

//        private void Update()
//        {
//            if (m_UpdateFN())
//            {
//                OnTargetReached();
//                return;
//            }
//            m_OnTravelVFXUpdate();
//        }
//    }
//}
