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
	public class JumpState : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.Jump");
		public override string StateName => "Jumping";
		public override OddState State => OddState.Jumping;
		const float AnimationLength = 0.625f;
		const float AnimationSpeed = 1f;
		const float AnimationTime = AnimationLength / AnimationSpeed;

		Timer m_AnimationTimer;
		public JumpState()
		{
			m_AnimationTimer = new Timer(AnimationTime, true, true);
			m_AnimationTimer.OnTimerTrigger += OnAnimationFinish;
		}
		public override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
		}
		public override void OnLateUpdate()
		{
			base.OnLateUpdate();
		}
		public override void OnStart()
		{
			base.OnStart();
			//Controller.ME.SetMaxSpeed(OddMaxSpeed);
			//Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed);
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0.2f, 0);
			m_AnimationTimer.Reset(AnimationTime);
			Controller.SetCurrentAttack(Def.OddAttackType.COUNT);
		}
		public override void OnStop()
		{
			base.OnStop();
		}
		void OnAnimationFinish()
		{
			Controller.ChangeState(OddState.Idle);
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			m_AnimationTimer.Update();

			var oldPos = Controller.transform.position;

			Controller.ME.Impulse(Controller.ME.GetDirection());
			Controller.LE.UpdateStruc();

			var struc = Controller.LE.GetCurrentStruc();
			if (struc == null)
			{
				Controller.LE.UpdateStruc();
				struc = Controller.LE.GetCurrentStruc();
			}
			if (Controller.LE.GetCurrentBlock() == null && struc != null)
			{
				Controller.LE.UpdateBlock();
			}

			var movXZ = Controller.ME.UpdateMovement();
			var mov = new Vector3(movXZ.x, 1.5f, movXZ.y);
			mov *= Time.deltaTime;

			var cc = Controller.ME.GetController();
			var collision = cc.Move(mov);
			if (collision.HasFlag(CollisionFlags.CollidedSides))
			{
				Controller.ME.OnCollision();
			}

			var tempBlock = Controller.LE.ComputeCurrentBlock() as CBlock;
			if (Controller.LE.GetCurrentBlock() != null && tempBlock == null)
			{
				Controller.transform.position = oldPos;
				Controller.ME.OnCollision();
			}
			else
			{
				Controller.LE.SetCurrentBlock(tempBlock);
			}
		}
		public override void OnGizmos()
		{
			base.OnGizmos();
		}
		public override void OnGUI()
		{
			base.OnGUI();
		}
	}
}
