/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AI
{
    public class FenrirClawComponent : MonoBehaviour
    {
        [SerializeField]
        MonsterFamily m_Fenrir;
        [SerializeField]
        List<AI.CLivingEntity> m_Enemies;
        [SerializeField]
        List<MonsterTeam> m_FriendlyTeams;
        [SerializeField]
        List<int> m_FriendlyFamilies;

        public void Set(MonsterFamily info, List<AI.CLivingEntity> enemies, List<MonsterTeam> avoidTeams, List<int> avoidFamiles)
        {
            m_Fenrir = info;
            m_Enemies = enemies;
            m_FriendlyTeams = avoidTeams;
            m_FriendlyFamilies = avoidFamiles;
        }
        
        bool FilterMonster<T>(T monster)
        {
            ////bool friendlyTeam = m_FriendlyTeams.Contains(monster.Info.Team);
            //bool friendlyFamily = m_FriendlyFamilies.Contains(Monsters.FamilyDict[monster.Info.Name] /*monster.Info.MonsterID*/);
            ////return !(friendlyFamily && friendlyTeam);

            //return !friendlyFamily;
            return false;
        }

        public void OnTriggerEnter(Collider other)
        {
            //if (other.tag != LivingEntity.TAG)
            if(other.gameObject.layer != Def.RCLayerLE)
                return;
            var le = other.transform.parent.gameObject.GetComponent<AI.CLivingEntity>();
            if (le.GetCurrentHealth() <= 0f)
                return;
            bool damage = false;
            bool enemy = m_Enemies.Contains(le);
            if (!enemy)
            {
                switch (le.GetLEType())
                {
                    case Def.LivingEntityType.ODD:
                    case Def.LivingEntityType.Prop:
                        damage = true;
                        break;
                    case Def.LivingEntityType.Monster:
                        damage = false;// FilterMonster((MonsterScript)le);
                        break;
                }
            }
            else
            {
                damage = true;
            }
            if (damage)
                le.ReceiveDamage(null, m_Fenrir.DamageType, m_Fenrir.AttackDamage);
        }
    }
}
