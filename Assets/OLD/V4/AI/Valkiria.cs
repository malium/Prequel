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
//    public class Valkiria : MonsterScript
//    {
//        static int MonsterTypeID = 0;
//        static int AttackVFXTypeID = 0;
//        static float AttackHeight = -1.0f;
//        static ProjectileDef DefaultProjectile;
//        static ProjectileDef SecondaryProjectile;

//        public override void InitMonster()
//        {
//            if (MonsterTypeID < 1)
//            {
//                MonsterTypeID = Monsters.FamilyDict["Valkiria"];
//                var monster = Monsters.MonsterFamilies[MonsterTypeID];
//                AttackVFXTypeID = VFXs.FamilyDict[(int)Def.VFXTarget.MONSTER]["ValkiriaSlash"];
//                DefaultProjectile.AreaDamageType = ProjectileAreaDamageType.CIRCULAR;
//                DefaultProjectile.CircularRadius = 0.6f;
//                DefaultProjectile.Damage = 11.0f;
//                DefaultProjectile.DmgType = Def.DamageType.CUT;
//                DefaultProjectile.Speed = new Vector2(3.0f, 0.0f);
//                DefaultProjectile.CastVFX = new VFXDef(Def.VFXTarget.MONSTER, "ValkiriaSlash", Def.VFXType.CAST, 0, Def.VFXFacing.FaceYDown, Def.VFXEnd.SelfDestroy);
//                DefaultProjectile.HitMask = (int)ProjectileHitMask.NONE;
//                DefaultProjectile.OnHitVFX = new VFXDef(Def.VFXTarget.COUNT, -1, Def.VFXType.COUNT, -1);
//                DefaultProjectile.TravelType = ProjectileTravelType.LINEAR;
//                DefaultProjectile.TravelVFX = new VFXDef(Def.VFXTarget.COUNT, -1, Def.VFXType.COUNT, -1);
//                DefaultProjectile.AvoidMonsterTeams = new List<MonsterTeam>(1)
//                {
//                   //monster.Team
//                };
//                DefaultProjectile.AvoidMonsterFamilies = new List<int>(3)
//                {
//                    MonsterTypeID
//                };
//                //SECONDARY
//                SecondaryProjectile.AreaDamageType = ProjectileAreaDamageType.CIRCULAR;
//                SecondaryProjectile.CircularRadius = 0.6f;
//                SecondaryProjectile.Damage = 39.0f;
//                SecondaryProjectile.DmgType = Def.DamageType.LIGHT;
//                SecondaryProjectile.Speed = new Vector2(1.2f, 0.0f);
//                SecondaryProjectile.CastVFX = new VFXDef(Def.VFXTarget.COUNT, -1, Def.VFXType.COUNT, -1);
//                SecondaryProjectile.HitMask = (int)ProjectileHitMask.NONE;
//                SecondaryProjectile.OnHitVFX = new VFXDef(Def.VFXTarget.MONSTER, "ValkiriaSlash", Def.VFXType.CAST, 0, Def.VFXFacing.FaceCameraFreezeXZ, Def.VFXEnd.SelfDestroy);
//                SecondaryProjectile.TravelType = ProjectileTravelType.LINEAR;
//                SecondaryProjectile.TravelVFX = new VFXDef(Def.VFXTarget.COUNT, -1, Def.VFXType.COUNT, -1);
//                SecondaryProjectile.AvoidMonsterTeams = new List<MonsterTeam>(1)
//                {
//                   //monster.Team
//                };
//                SecondaryProjectile.AvoidMonsterFamilies = new List<int>(3)
//                {
//                    MonsterTypeID
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
//            projectile.CastVFX.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

//            //SECONDARY

//            ProjectileDef sDef = SecondaryProjectile;

//            var distance = 0.5f;

//            if (target == null)
//            {
//                sDef.FinalPosition = targetPos + new Vector3(0.0f, AttackHeight, 0.0f);
//            }
//            else
//            {
//                sDef.FinalPosition = target.transform.position + new Vector3(0.0f, AttackHeight, 0.0f);
//            }
//            sDef.InitialPosition = transform.position + new Vector3(0.0f, AttackHeight, 0.0f);

//            var dir = (sDef.FinalPosition - sDef.InitialPosition).normalized;
//            var fPos = sDef.InitialPosition + dir * distance;

//            sDef.FinalPosition = fPos;
//            sDef.Caster = this;
//            sDef.Enemies = m_EnemyList;
//            //sDef.AvoidMonsterTeams[0] = m_Info.Team;

//            var projectile2GO = new GameObject("Projectile");
//            var projectile2 = projectile2GO.AddComponent<ProjectileComponent>();
//            projectile2.SetProjectile(sDef);
//            projectile2.OnHitVFX.transform.localScale = new Vector3(2f, 2f, 1f);
//        }
//    }
//}
