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
//    public class Succubus : MonsterScript
//    {
//        static int MonsterTypeID = 0;
//        static int AttackVFXTypeID = 0;
//        static float AttackHeight = -1.0f;
//        static ProjectileDef DefaultProjectile;

//        public override void InitMonster()
//        {
//            if (MonsterTypeID < 1)
//            {
//                MonsterTypeID = Monsters.FamilyDict["Succubus"];
//                var monster = Monsters.MonsterFamilies[MonsterTypeID];
//                AttackVFXTypeID = VFXs.FamilyDict[(int)Def.VFXTarget.MONSTER]["CharmingHeart"];
//                DefaultProjectile = new ProjectileDef
//                {
//                    AreaDamageType = ProjectileAreaDamageType.CIRCULAR,
//                    CircularRadius = 0.3f,
//                    Damage = 23.0f,
//                    DmgType = Def.DamageType.ASPHYXIA,
//                    Speed = new Vector2(3.0f, 0.0f),
//                    CastVFX = new VFXDef(Def.VFXTarget.COUNT, -1, Def.VFXType.COUNT, -1),
//                    HitMask = (int)ProjectileHitMask.ALL,
//                    OnHitVFX = new VFXDef(Def.VFXTarget.MONSTER, AttackVFXTypeID, Def.VFXType.ONHIT, 0, Def.VFXFacing.FaceCameraFull, Def.VFXEnd.SelfDestroy),
//                    TravelType = ProjectileTravelType.LINEAR,
//                    TravelVFX = new VFXDef(Def.VFXTarget.MONSTER, AttackVFXTypeID, Def.VFXType.TRAVEL, 0, Def.VFXFacing.FaceCameraFull, Def.VFXEnd.Repeat),
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

//            if (target == null)
//            {
//                pDef.FinalPosition = targetPos + new Vector3(0.0f, AttackHeight, 0.0f);
//            }
//            else
//            {
//                pDef.FinalPosition = target.transform.position + new Vector3(0.0f, AttackHeight, 0.0f);
//            }
//            pDef.InitialPosition = transform.position + new Vector3(0.0f, AttackHeight, 0.0f);
//            pDef.Caster = this;
//            pDef.Enemies = m_EnemyList;
//            //pDef.AvoidMonsterTeams[0] = m_Info.Team;

//            var projectileGO = new GameObject("Projectile");
//            var projectile = projectileGO.AddComponent<ProjectileComponent>();
//            projectile.SetProjectile(pDef);
//        }
//    }
//}
