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
	public class IdleState : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.Idle");
		public override string StateName => "Idle";


		const float AnimationLength = 2f;
		const float AnimationSpeed = 0.6f;
		const float AnimationTime = AnimationLength / AnimationSpeed;
		public override OddState State => OddState.Idle;
		Timer m_AnimationTimer;
		

		public IdleState()
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
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0.2f);
			Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed);
			Controller.ME.SetSightSpeed(Controller.ME.GetAngularSpeed());
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(true);
			m_AnimationTimer.Reset(AnimationTime);
			if(Controller.LE.GetCurrentStruc() == null)
			{
				Controller.LE.UpdateStruc();
				if (Controller.LE.GetCurrentStruc() != null)
					Controller.LE.UpdateBlock();
			}
			Controller.SetCurrentAttack(Def.OddAttackType.COUNT);
		}
		public override void OnStop()
		{
			base.OnStop();
		}
		void OnAnimationFinish()
		{
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0f);
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			m_AnimationTimer.Update();

			if(Input.GetKeyDown(KeyCode.Space))
			{
				if (!Input.GetKey(KeyCode.LeftShift))
					Controller.ChangeState(OddState.Dashing);
				else
					Controller.ChangeState(OddState.Possessing);
				Controller.CurrentStateController.OnUpdate();
				return;
			}
			if(Input.GetMouseButtonDown(1))
			{
				var cam = CameraManager.Mgr.Camera;
				if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
				{
					Controller.ME.SetDirection(new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z));
				}
				Controller.ChangeState(OddState.Covering);
				Controller.CurrentStateController.OnUpdate();
				return;
			}

			if(Input.GetMouseButtonDown(0))
			{
				var cam = CameraManager.Mgr.Camera;
				if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
				{
					Controller.ME.SetDirection(new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z));
				}
				Controller.ChangeState(OddState.AttackIn);
				Controller.CurrentStateController.OnUpdate();
				return;
			}

			if(Controller.SpellCaster.IsQuickItemTriggered())
			{
				Controller.ChangeState(OddState.Consumable);
				return;
			}

			var movDir = Controller.ME.ComputeKeyboardMoveDirectionRelativeToCamera(CameraManager.Mgr.Camera, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D);

			if (movDir != Vector2.zero)
			{
				Controller.ChangeState(OddState.Walking);
				Controller.CurrentStateController.OnUpdate();
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
