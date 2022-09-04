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

namespace Assets.AI.ODD
{
	public class MeleeSpell : Spells.ISpell
	{
		public MeleeSpell()
			: base("OddMeleeSpell")
		{

		}
		public override void DestroySpell()
		{
			base.DestroySpell();
		}
		public override void InitSpell()
		{
			base.InitSpell();
		}
		public override void OnUpdate()
		{
			base.OnUpdate();
		}
		protected override void PerformAttack(CLivingEntity entity, Vector3 pos)
		{

		}
		protected override void PerformCast(CLivingEntity entity, Vector3 pos)
		{

		}
		protected override void PerformCooldown()
		{
			base.PerformCooldown();
		}
		protected override void PerformIdle()
		{
			base.PerformIdle();
		}
	}
}
