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
	public class HeavyAttack1State : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.HeavyAttack1_Out");
		public override string StateName => "HeavyAttack1";
		public override OddState State => OddState.AttackingHeavy1;
		const float AnimationLength = 0.375f;
		const float AnimationSpeed = 1f;
		public const float AnimationTime = AnimationLength / AnimationSpeed;
		const float AnimationFrames = 9f;

		Timer m_AnimationTimer;
		OddState m_NextState;
		Vector2 m_NextDirection;

		public HeavyAttack1State()
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
			weapon.OnAttackBegin(Def.OddAttackType.Heavy1);
			weapon.GetTrailRenderer().gameObject.SetActive(false);
			m_AnimationTimer.Reset(AnimationTime);
			m_NextState = OddState.AttackRecovering;
			m_NextDirection = Controller.ME.GetDirection();
			Controller.SetCurrentAttack(Def.OddAttackType.Heavy1);
			//Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed * 0.05f);
			Controller.ME.SetMaxSpeed(RunningState.OddMaxSpeed);
			Controller.ME.Impulse(Controller.ME.GetDirection(), 20f);

			var tar = new COddController.TargetArmRotationInfo()
			{
				Angle = 0f,
				Duration = AnimationTime,
				StartAxis = Controller.StartAxis[(int)Def.OddAttackType.Heavy1], //	Vector3.forward,
				EndAxis = Controller.EndAxis[(int)Def.OddAttackType.Heavy1], //Vector3.right,
				StartFadeInPCT = 5f / AnimationFrames,
				EndFadeInPCT = 7f / AnimationFrames,
				StartFadeOutPCT = 9f / AnimationFrames,
				EndFadeOutPCT = 1f
			};
			var targetEntity = Controller.Odd.GetTargetEntity();
			if (targetEntity != null)
			{
				tar.Angle = Controller.GetArmRotationForTarget(targetEntity);
			}
			Controller.SetTargetArmRotation(tar);
		}
		public override void OnStop()
		{
			base.OnStop();
			if (m_NextState != OddState.AttackRecovering)
				Controller.GetWeapons()[0].OnAttackEnd();
		}
		void OnAnimationFinish()
		{
			if (m_NextState == OddState.AttackIn)
				Controller.ME.SetDirection(m_NextDirection);
			Controller.ChangeState(m_NextState);
			Controller.CurrentStateController.OnUpdate();
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			if (Input.GetMouseButtonDown(0))
			{
				m_NextState = OddState.AttackIn;
				var targetEntity = Controller.Odd.GetTargetEntity();
				if (targetEntity == null)
				{
					var mgr = Manager.Mgr;
					if (mgr.GameInputControl == Def.GameInputControl.Mouse)
					{
						var cam = CameraManager.Mgr.Camera;
						if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
						{
							m_NextDirection = new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z);
						}
					}
					else if (mgr.GameInputControl == Def.GameInputControl.MouseLikeController)
					{
						var cam = CameraManager.Mgr.Camera;
						var posXZ = new Vector2(Controller.transform.position.x, Controller.transform.position.z);
						var camXZ = new Vector2(cam.transform.position.x, cam.transform.position.z);
						m_NextDirection = posXZ - camXZ;
					}
				}
				else
				{
					var posXZ = new Vector2(Controller.transform.position.x, Controller.transform.position.z);
					var trgXZ = new Vector2(targetEntity.transform.position.x, targetEntity.transform.position.z);
					m_NextDirection = (trgXZ - posXZ).normalized;
				}
			}

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
