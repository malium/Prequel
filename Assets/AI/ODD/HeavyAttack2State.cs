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
	public class HeavyAttack2State : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.HeavyAttack2_Out");
		public override string StateName => "HeavyAttack2";
		public override OddState State => OddState.AttackingHeavy2;
		const float AnimationLength = 0.375f;
		const float AnimationSpeed = 1f;
		public const float AnimationTime = AnimationLength / AnimationSpeed;
		const float AnimationFrames = 9f;

		Timer m_AnimationTimer;

		public HeavyAttack2State()
		{
			m_AnimationTimer = new Timer(AnimationTime, true, false);
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
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0f);
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.GetTrailRenderer().gameObject.SetActive(false);
			weapon.OnAttackBegin(Def.OddAttackType.Heavy2);
			m_AnimationTimer.Reset(AnimationTime);
			Controller.SetCurrentAttack(Def.OddAttackType.Heavy2);
			//Controller.ME.SetAngularSpeed(0f);
			Controller.ME.SetMaxSpeed(RunningState.OddMaxSpeed);
			Controller.ME.Impulse(Controller.ME.GetDirection(), 20f);

			var tar = new COddController.TargetArmRotationInfo()
			{
				Angle = 0f,
				Duration = AnimationTime,
				StartAxis = Controller.StartAxis[(int)Def.OddAttackType.Heavy2], //	Vector3.forward,
				EndAxis = Controller.EndAxis[(int)Def.OddAttackType.Heavy2], //Vector3.right,
				StartFadeInPCT = 5f / AnimationFrames,
				EndFadeInPCT = 7f / AnimationFrames,
				StartFadeOutPCT = 9f / AnimationFrames,
				EndFadeOutPCT = 1f
			};
			Controller.SetTargetArmRotation(tar);
		}
		public override void OnStop()
		{
			base.OnStop();
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.OnAttackEnd();
			weapon.LookAtCamera(true);
		}
		void OnAnimationFinish()
		{
			//Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed);
			Controller.ChangeState(OddState.Idle);
			Controller.CurrentStateController.OnUpdate();
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			var movXZ = Controller.ME.UpdateMovement();
			var mov = new Vector3(movXZ.x, GameUtils.Gravity, movXZ.y);
			mov *= Time.deltaTime;

			var cc = Controller.ME.GetController();
			var collision = cc.Move(mov);
			if (collision.HasFlag(CollisionFlags.CollidedSides))
				Controller.ME.OnCollision();

			m_AnimationTimer.Update();
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
