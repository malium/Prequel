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
	public class Attack4State : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.Attack4");
		public override string StateName => "Attack4";
		public override OddState State => OddState.Attacking4;
		const float AnimationLength = 0.5f;
		const float AnimationSpeed = 1f;
		public const float AnimationTime = AnimationLength / AnimationSpeed;
		const float AnimationFrames = 12f;

		Timer m_AnimationTimer;
		Timer m_MovementTimer;
		OddState m_NextState;
		Vector2 m_NextDirection;
		public Attack4State()
		{
			m_AnimationTimer = new Timer(AnimationTime, true, false);
			m_AnimationTimer.OnTimerTrigger += OnAnimationFinish;
			m_MovementTimer = new Timer(AnimationTime * 0.5f, true, false);
			m_MovementTimer.OnTimerTrigger += OnMovement;
		}
		private void OnMovement()
		{
			Controller.ME.SetMaxSpeed(RunningState.OddMaxSpeed);
			Controller.ME.Impulse(Controller.ME.GetDirection(), 10f);
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
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0.2f, 0);
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			//weapon.SetActivationDelay(0.5f * AnimationTime);
			weapon.OnAttackBegin(Def.OddAttackType.Normal4);
			m_AnimationTimer.Reset(AnimationTime);
			m_MovementTimer.Reset();
			m_NextState = OddState.AttackRecovering;
			m_NextDirection = Controller.ME.GetDirection();
			Controller.SetCurrentAttack(Def.OddAttackType.Normal4);
			//Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed * 0.05f);

			var tar = new COddController.TargetArmRotationInfo()
			{
				Angle = 0f,
				Duration = AnimationTime,
				StartAxis = Controller.StartAxis[(int)Def.OddAttackType.Normal4], //Vector3.forward,
				EndAxis = Controller.EndAxis[(int)Def.OddAttackType.Normal4], //Vector3.right,
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
			if (m_NextState == OddState.AttackIn || m_NextState == OddState.Covering)
				Controller.ME.SetDirection(m_NextDirection);
			Controller.ChangeState(m_NextState);
			Controller.CurrentStateController.OnUpdate();
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			var remainingPCT = m_AnimationTimer.GetRemainingPct();
			bool mbRightDown = Input.GetMouseButton(1);

			if (mbRightDown)
			{
				m_NextState = OddState.Covering;
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
			if (!mbRightDown && Input.GetMouseButtonDown(0))
			{
				if (remainingPCT >= 0.4f)
				{
					m_NextState = OddState.AttackIn;
					var targetEntity = Controller.Odd.GetTargetEntity();
					if (targetEntity == null)
					{
						var cam = CameraManager.Mgr.Camera;
						if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
						{
							m_NextDirection = new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z);
						}
					}
					else
					{
						var posXZ = new Vector2(Controller.transform.position.x, Controller.transform.position.z);
						var trgXZ = new Vector2(targetEntity.transform.position.x, targetEntity.transform.position.z);
						m_NextDirection = (trgXZ - posXZ).normalized;
					}
				}
			}
			if (remainingPCT >= 0.5f)
			{
				if (remainingPCT < 0.8f) // Fade in
				{
					float pct = Mathf.Min(remainingPCT, 0.6f);
					pct -= 0.5f;
					pct *= 10f;
					var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
					weapon.GetTrailRenderer().material.SetFloat(COdd.DashTrailAlphaMultiplyID, pct * 0.3f);
				}
				else // Fade out
				{
					float pct = Mathf.Min(remainingPCT, 0.9f);
					pct -= 0.8f;
					pct *= 10f;
					var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
					weapon.GetTrailRenderer().material.SetFloat(COdd.DashTrailAlphaMultiplyID, (1f - pct) * 0.3f);
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
			m_MovementTimer.Update();
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
