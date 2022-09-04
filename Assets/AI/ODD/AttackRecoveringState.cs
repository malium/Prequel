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
	public class AttackRecoveringState : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.Idle");
		public override string StateName => "AttackRecovering";
		public override OddState State => OddState.AttackRecovering;
		const float AttackRecoverPCT = 0.4f;
		const float HoldDelay = 1f / 8f;

		Timer m_AnimationTimer;
		Timer m_HoldAnimationTimer;
		OddState m_NextState;
		public AttackRecoveringState()
		{
			m_AnimationTimer = new Timer(true, false);
			m_AnimationTimer.OnTimerTrigger += OnAnimationFinish;
			m_HoldAnimationTimer = new Timer(true, false);
			m_HoldAnimationTimer.OnTimerTrigger += OnHoldAnimationEnd;
		}
		private void OnHoldAnimationEnd()
		{
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0.4f);
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(true);
			weapon.GetTrailRenderer().gameObject.SetActive(false);
			weapon.OnAttackEnd();
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
			//Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0.4f);
			float recoverTime;
			switch (Controller.PreviousState)
			{
				case OddState.Attacking1:
				case OddState.Attacking2:
				case OddState.Attacking3:
				case OddState.Attacking4:
				case OddState.AttackingHeavy1:
				case OddState.AttackingHeavy2:
				case OddState.DashAttack:
					recoverTime = Attack1State.AnimationTime * AttackRecoverPCT;
					break;
				default:
					recoverTime = 0.3f;
					break;
			}
			m_AnimationTimer.Reset(recoverTime);
			m_HoldAnimationTimer.Reset(HoldDelay);

			if (Controller.LE.GetCurrentStruc() == null)
			{
				Controller.LE.UpdateStruc();
				if (Controller.LE.GetCurrentStruc() != null)
					Controller.LE.UpdateBlock();
			}
			//Controller.SetTargetArmRotation(0f, 0f);
			m_NextState = OddState.Idle;
		}
		public override void OnStop()
		{
			base.OnStop();
		}
		void OnAnimationFinish()
		{
			//var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			//weapon.LookAtCamera(false);
			//weapon.OnAttackEnd();
			//Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed);
			Controller.ChangeState(m_NextState);
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			m_HoldAnimationTimer.Update();
			m_AnimationTimer.Update();
			if (Controller.CurrentState != OddState.AttackRecovering)
				return;

			if(Input.GetMouseButton(1))
			{
				m_NextState = OddState.Covering;
			}
			else
			{
				m_NextState = OddState.Idle;
				if (Input.GetMouseButtonDown(0))
				{
					var cam = CameraManager.Mgr.Camera;
					if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
					{
						Controller.ME.SetDirection(new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z));
					}
					var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
					weapon.LookAtCamera(false);
					weapon.GetTrailRenderer().gameObject.SetActive(true);
					weapon.OnAttackEnd();
					//Controller.GetWeapons()[0].OnAttackEnd();
					//Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed);
					Controller.ChangeState(OddState.AttackIn);
					Controller.CurrentStateController.OnUpdate();
					return;
				}
			}

			var movDir = Controller.ME.ComputeKeyboardMoveDirectionRelativeToCamera(CameraManager.Mgr.Camera, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D);

			if (movDir != Vector2.zero)
			{
				var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
				weapon.LookAtCamera(true);
				weapon.GetTrailRenderer().gameObject.SetActive(false);
				weapon.OnAttackEnd();
				//Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed);
				Controller.ChangeState(OddState.Walking);
				Controller.CurrentStateController.OnUpdate();
				return;
			}

			var oldPos = Controller.transform.position;

			var movXZ = Controller.ME.UpdateMovement();
			var mov = new Vector3(movXZ.x, GameUtils.Gravity, movXZ.y);
			mov *= Time.deltaTime;
			var cc = Controller.ME.GetController();
			var collision = cc.Move(mov);
			if (collision.HasFlag(CollisionFlags.CollidedSides))
				Controller.ME.OnCollision();

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
