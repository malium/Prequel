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
//    public struct HitEntity
//    {
//        public LivingEntity LE;
//        public float NextHitTime;
//    }

//    public class OddWeapon : MonoBehaviour
//    {
//        public const string TAG = "WEAPON";

//        //List<HitEntity> HitEntities;
//        public OddScript Odd;
//        public Vector2 AttackDir;
//        public SpriteRenderer Renderer;
//        public BoxCollider Collider;
//        public float AttackRange
//        {
//            get
//            {
//                return 1.2f;
//            }
//        }
//        Vector3 m_Position;
//        public float AttackStartTime;
//        public float AttackEndTime;

//        private void Awake()
//        {
//            //HitEntities = new List<HitEntity>();
//        }

//        void Start()
//        {
//            m_Position = transform.localPosition;
//        }

//        void Update()
//        {
//            //for(int i = 0; i < HitEntities.Count;)
//            //{
//            //    if(HitEntities[i].LE == null || HitEntities[i].NextHitTime < Time.time || (HitEntities[i].LE != null && HitEntities[i].LE.GetCurrentHealth() <= 0))
//            //    {
//            //        HitEntities.RemoveAt(i);
//            //        continue;
//            //    }
//            //    ++i;
//            //}
//            if(Collider.enabled)
//            {
//                if (AttackEndTime < Time.time)
//                    Collider.enabled = false;
//            }
//            else
//            {
//                if (AttackStartTime <= Time.time && AttackEndTime >= Time.time)
//                    Collider.enabled = true;
//            }
//        }

//        private void OnTriggerEnter(Collider other)
//        {
//            //if (Odd == null)
//            //    return;
//            if (Time.time < AttackStartTime || Time.time > AttackEndTime)
//                return;
//            //if (other.gameObject == Odd.gameObject)
//            //    return;
//            if (Odd.State != ODD_STATE.ATTACKING)
//                return;
//            if (other.gameObject.tag != LivingEntity.TAG)
//                return;
//            //for(int i = 0; i < HitEntities.Count; ++i)
//            //{
//            //    if (HitEntities[i].LE.gameObject == other.gameObject)
//            //        return; // already hit
//            //}
//            var le = other.transform.parent.gameObject.GetComponent<LivingEntity>();
//            if (le.GetLEType() == Def.LivingEntityType.ODD)
//                return;
//            //float nextHitTime = Odd.NextActionTime;


//            //        var enemyDir = (propPos - m_TargetPos).normalized;

//            //        var dotDir = Vector2.Dot(curDir, enemyDir);
//            //        var cosDotDir = Mathf.Acos(dotDir);
//            //        if (cosDotDir < 0.6f)
//            //        {
//            //            sprop.TakeDamage(10.0f);
//            //        }
//            //    }
//            //var dir = new Vector2(Odd.Position.x - other.transform.parent.position.x, Odd.Position.z - other.transform.parent.position.z);
//            //dir.Normalize();
//            //var dotDir = Vector2.Dot(AttackDir, dir);
//            ////var cosDotDir = Mathf.Acos(dotDir);
//            ////if (cosDotDir > 0.6f)
//            ////{
//            ////    Debug.Log($"Dot:{dotDir},CosDotDir:{cosDotDir}");
//            ////    return;
//            ////}
//            //if (dotDir >= -0.3f || dotDir <= -1.0f)
//            //    return;
//            var heightDiff = other.gameObject.transform.position.y - Odd.transform.position.y;
//            if (heightDiff > 1.0f || heightDiff < -0.25f)
//                return;

//            if (Input.GetKey(KeyCode.LeftShift))
//                le.ReceiveDamage(Def.DamageType.UNAVOIDABLE, le.GetTotalHealth());
//            else
//                le.ReceiveDamage(Def.DamageType.CUT, 40.0f);
//            var pos = new Vector3(le.transform.position.x, transform.position.y, le.transform.position.z);
//            var dir = (le.transform.position - transform.position).normalized;
//            switch (le.GetLEType())
//            {
//                case Def.LivingEntityType.Monster:
//                    var mon = le as MonsterScript;
//                    mon.SetEnemy(Odd);
//                    mon.CastBloodTrails(pos, dir);
//                    break;
//                case Def.LivingEntityType.Prop:
//                    var prop = le as PropScript;
//                    prop.CastDamageParticles(pos, dir);
//                    break;
//            }
//            //HitEntity he;
//            //he.NextHitTime = nextHitTime + 0.2f;
//            //he.LE = le;
//            //HitEntities.Add(he);
//        }

//        private void LateUpdate()
//        {
//            if (Manager.Mgr.CurrentControllerSel != (int)GameState.PLAY)
//                return;
//            var cam = Manager.Mgr.m_Camera;
//            var odd = Manager.Mgr.OddScript;
//            if (odd.State == ODD_STATE.IDLE || odd.State == ODD_STATE.WALKING)
//            {
//                Vector3 v = cam.transform.position - transform.position;
//                v.x = v.z = 0.0f;
//                var dir = cam.transform.position - v;
//                Quaternion current = transform.rotation;
//                transform.LookAt(dir);
//                Quaternion next = transform.rotation;
//                var slerp = Quaternion.Slerp(current, next, 6.0f * Time.deltaTime);
//                transform.rotation = slerp;
//                transform.localPosition = m_Position;
//            }
//        }
//    }
//}
