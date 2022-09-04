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
//    public class ArcherGoburin : MonsterScript
//    {
//        static int MonsterTypeID = 0;
//        static int AttackVFXTypeID = 0;
//        static float AttackHeight = -1.0f;
//        static ProjectileDef DefaultProjectile;

//        public override void InitMonster()
//        {
//            if (MonsterTypeID < 1)
//            {
//                MonsterTypeID = Monsters.FamilyDict["ArcherGoburin"];
//                var monster = Monsters.MonsterFamilies[MonsterTypeID];
                
//                AttackVFXTypeID = VFXs.FamilyDict[(int)Def.VFXTarget.MONSTER]["GoblinArrow"];
//                DefaultProjectile = new ProjectileDef
//                {
//                    AreaDamageType = ProjectileAreaDamageType.CIRCULAR,
//                    CircularRadius = 0.1f,
//                    Damage = 23.0f,
//                    DmgType = Def.DamageType.CUT,
//                    CastVFX = new VFXDef(Def.VFXTarget.COUNT, -1, Def.VFXType.COUNT, -1),
//                    HitMask = (int)ProjectileHitMask.ALL,
//                    OnHitVFX = new VFXDef(Def.VFXTarget.COUNT, -1, Def.VFXType.COUNT, -1),
//                    TravelType = ProjectileTravelType.PARABOLIC,
//                    TravelVFX = new VFXDef(Def.VFXTarget.MONSTER, AttackVFXTypeID, Def.VFXType.TRAVEL, 0, Def.VFXFacing.FaceCameraFull, Def.VFXEnd.Repeat),
//                    AvoidMonsterTeams = new List<MonsterTeam>(1)
//                    {
//                        //monster.Team
//                    },
//                    AvoidMonsterFamilies = new List<int>(3)
//                    {
//                        MonsterTypeID,
//                        Monsters.FamilyDict["Goburin"],
//                        Monsters.FamilyDict["RiderGoburin"]
//                    }
//                };
//            }

//            SetMonster(MonsterTypeID);

//            if (AttackHeight < 0.0f)
//            {
//                AttackHeight = (m_Info.Frames[0].rect.height / m_Info.Frames[0].pixelsPerUnit) * m_Info.SpriteScale;
//            }
//        }

//        public override void Attack(LivingEntity target, Vector3 targetPos)
//        {
//            ProjectileDef pDef =  DefaultProjectile;

//            if(target == null)
//            {
//                pDef.FinalPosition = targetPos;
//            }
//            else
//            {
//                pDef.FinalPosition = target.transform.position;
//            }
//            pDef.FinalPosition.y = pDef.FinalPosition.y + AttackHeight;
//            pDef.InitialPosition = transform.position + new Vector3(0f, AttackHeight, 0f);

//            float dist = Vector2.Distance(new Vector2(pDef.FinalPosition.x, pDef.FinalPosition.z), new Vector2(transform.position.x, transform.position.z));
//            pDef.Speed = new Vector2(3.0f, dist);

//            float A = -0.5f * 9.81f;
//            float B = pDef.Speed.y;
//            float C = Mathf.Abs(pDef.InitialPosition.y - pDef.FinalPosition.y);

//            var sqrtPart = Mathf.Sqrt(B * B - 4f * A * C);
//            Vector2 equation = new Vector2(-B + sqrtPart, -B - sqrtPart);
//            equation /= (2f * A);

//            if (equation.x > 0f)
//                pDef.Speed.Set(dist / equation.x, pDef.Speed.y);
//            else
//                pDef.Speed.Set(dist / equation.y, pDef.Speed.y);
            
//            pDef.Caster = this;
//            pDef.Enemies = m_EnemyList;
//            ////pDef.AvoidMonsterTeams[0] = m_Info.Team;

//            var projectileGO = new GameObject("Projectile");
//            var projectile = projectileGO.AddComponent<ProjectileComponent>();
//            projectile.SetProjectile(pDef);
//            var dir = (pDef.InitialPosition - pDef.FinalPosition).normalized;
//            if (dir.z < 0.0f)
//                projectile.Renderer.flipX = true;
//        }
//    }
//}
