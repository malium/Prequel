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
//    public class OldLegend : MonsterScript
//    {
//        static int MonsterTypeID = 0;
//        //static int AttackVFXTypeID = 0;
//        static float AttackHeight = -1.0f;

//        public override void InitMonster()
//        {
//            if (MonsterTypeID < 1)
//            {
//                MonsterTypeID = Monsters.FamilyDict["OldLegend"];

//				//var monster = Monsters.MonsterFamilies[MonsterTypeID];
//			}
//			SetMonster(MonsterTypeID);
			
//			if (AttackHeight < 0.0f)
//			{
//				AttackHeight = (m_Info.Frames[0].rect.height / m_Info.Frames[0].pixelsPerUnit) * m_Info.SpriteScale;
//			}
//		}
//		public override void Attack(LivingEntity target, Vector3 targetPos)
//		{

//		}
//	}
//}
