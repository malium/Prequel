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
//    public class Crier : MonsterScript
//    {
//        static int MonsterTypeID = 0;
//        static int AttackVFXTypeID = 0;
//        static float AttackHeight = -1.0f;
//        static ProjectileDef DefaultProjectile;
//        bool m_AttackVariation = false;

//        public override void InitMonster()
//        {
//            if (MonsterTypeID < 1)
//            {
//                MonsterTypeID = Monsters.FamilyDict["Crier"];
//                var monster = Monsters.MonsterFamilies[MonsterTypeID];
//                AttackVFXTypeID = VFXs.FamilyDict[(int)Def.VFXTarget.MONSTER]["CrierTears"];
//                DefaultProjectile = new ProjectileDef
//                {
//                    AreaDamageType = ProjectileAreaDamageType.CIRCULAR,
//                    CircularRadius = 1.5f,
//                    Damage = 4.0f,
//                    DmgType = Def.DamageType.DEPRESSION,
//                    Speed = new Vector2(3.0f, 0.0f),
//                    CastVFX = new VFXDef(Def.VFXTarget.MONSTER, AttackVFXTypeID, Def.VFXType.CAST, 0, Def.VFXFacing.FaceCameraFull, Def.VFXEnd.SelfDestroy),
//                    HitMask = (int)ProjectileHitMask.NONE,
//                    OnHitVFX = new VFXDef(Def.VFXTarget.MONSTER, "CrierPool", Def.VFXType.ONHIT, 0, Def.VFXFacing.FaceYUp, Def.VFXEnd.SelfDestroy),
//                    TravelType = ProjectileTravelType.LINEAR,
//                    TravelVFX = new VFXDef(Def.VFXTarget.COUNT, -1, Def.VFXType.COUNT, -1),
//                    AvoidMonsterTeams = new List<MonsterTeam>(1)
//                    {
//                        //monster.Team
//                    },
//                        AvoidMonsterFamilies = new List<int>(3)
//                    {
//                        MonsterTypeID
//                    }
//                };
//            }

//            SetMonster(MonsterTypeID);

//            if (AttackHeight < 0.0f)
//            {
//                AttackHeight = (m_Info.Frames[0].rect.height / m_Info.Frames[0].pixelsPerUnit) * m_Info.SpriteScale;
//                AttackHeight *= 0.5f;
//            }
//        }

//        public override void Attack(LivingEntity target, Vector3 targetPos)
//        {
//            ProjectileDef pDef = DefaultProjectile;

//            pDef.InitialPosition = transform.position + new Vector3(0f, 0.2f, 0f);
//            pDef.FinalPosition = pDef.InitialPosition;

//            pDef.Caster = this;
//            pDef.Enemies = m_EnemyList;
//            //pDef.AvoidMonsterTeams[0] = m_Info.Team;
//            pDef.OnHitVFX.VFXVersion = m_AttackVariation ? 1 : 0;
//            m_AttackVariation = !m_AttackVariation;

//            var projectileGO = new GameObject("Projectile");
//            var projectile = projectileGO.AddComponent<ProjectileComponent>();
//            projectile.SetProjectile(pDef);
//            projectile.CastVFX.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
//            projectile.OnHitVFX.transform.localScale = new Vector3(3f, 3f, 1f);
//        }
//    }
//}
