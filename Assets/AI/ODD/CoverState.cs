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
	public class CoverState : IOddState
	{
		static readonly int CoverAnimationHash = Animator.StringToHash("BaseLayer.Cover");
		static readonly int IdleAnimationHash = Animator.StringToHash("BaseLayer.Cover_Idle");
		public override string StateName => "Cover";
		public override OddState State => OddState.Covering;
		const float CoverAnimationLength = 0.333f;
		const float CoverAnimationSpeed = 1f;
		public const float CoverAnimationTime = CoverAnimationLength / CoverAnimationSpeed;
		const float CoverStartPct = 5f / 8f;

		const float IdleAnimationLength = 2f;
		const float IdleAnimationSpeed = 1f;
		const float IdleAnimationTime = IdleAnimationLength / IdleAnimationSpeed;

		Timer m_AnimationTimer;
		OddState m_NextState;
		bool m_CoverDone;
		public CoverState()
		{
			m_AnimationTimer = new Timer(true, false);
			m_AnimationTimer.OnTimerTrigger += OnAnimationTimerFinish;
		}
		void OnAnimationTimerFinish()
		{
			m_CoverDone = true;
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(IdleAnimationHash, 0f, 0);
			m_AnimationTimer.Reset(IdleAnimationTime);
			Controller.SetIsCovered(true);
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
			int animationHash;
			float transition;
			if(Controller.PreviousState != OddState.BackDashing)
			{
				Controller.SetIsCovered(false);
				animationHash = CoverAnimationHash;
				transition = 0.2f;
				m_CoverDone = false;
			}
			else
			{
				Controller.SetIsCovered(true);
				animationHash = IdleAnimationHash;
				transition = 0f;
				m_CoverDone = true;
			}
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(animationHash, transition, 0);
			m_AnimationTimer.Reset(CoverAnimationTime);
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(false);
			Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed * 0.5f);
			Controller.ME.SetMaxSpeed(WalkingState.OddMaxSpeed * 0.5f);
			Controller.ME.SetSightSpeed(Controller.ME.GetAngularSpeed());
			Controller.SetCurrentAttack(Def.OddAttackType.COUNT);
			Controller.SetRotateTo(COddController.RotateTo.Sight);
			m_NextState = OddState.Idle;
		}
		public override void OnStop()
		{
			base.OnStop();
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(m_NextState != OddState.BackDashing);
			Controller.SetIsCovered(false);
			if (m_NextState != OddState.BackDashing)
			{
				Controller.SetRotateTo(COddController.RotateTo.Movement);
				Controller.ME.SetSightSpeed(COddController.OddAngularSpeed);
			}
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			var targetEntity = Controller.Odd.GetTargetEntity();
			if (targetEntity == null)
			{
				var mgr = Manager.Mgr;
				if (mgr.GameInputControl == Def.GameInputControl.Mouse)
				{
					var cam = CameraManager.Mgr.Camera;
					if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
					{
						//Controller.ME.SetDirection(new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z));
						Controller.ME.SetSightDirection(new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z));
					}
				}
				else
				{
					var cam = CameraManager.Mgr.Camera;
					var posXZ = new Vector2(Controller.transform.position.x, Controller.transform.position.z);
					var camXZ = new Vector2(cam.transform.position.x, cam.transform.position.z);
					Controller.ME.SetSightDirection(posXZ - camXZ);
				}
			}
			else
			{
				var posXZ = new Vector2(Controller.transform.position.x, Controller.transform.position.z);
				var trgXZ = new Vector2(targetEntity.transform.position.x, targetEntity.transform.position.z);
				//Controller.ME.SetDirection(trgXZ - posXZ);
				Controller.ME.SetSightDirection(trgXZ - posXZ);
			}

			if(!m_CoverDone && !Controller.IsCovered())
			{
				Controller.SetIsCovered(m_AnimationTimer.GetRemainingPct() >= CoverStartPct);
			}

			if (Input.GetKey(KeyCode.Space))
			{
				m_NextState = OddState.BackDashing;
			}

			if (m_CoverDone)
			{
				if(!Input.GetMouseButton(1) || m_NextState == OddState.BackDashing)
				{
					Controller.ChangeState(m_NextState);
					Controller.CurrentStateController.OnUpdate();
					return;
				}
			}

			var movDir = Controller.ME.ComputeKeyboardMoveDirectionRelativeToCamera(CameraManager.Mgr.Camera, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D);
			if (movDir != Vector2.zero)
			{
				Controller.ME.Impulse(movDir);
				Controller.ME.SetDirectionInstantly(movDir);
				Controller.LE.UpdateStruc();
			}

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
