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
	public class WalkingState : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.Walk");
		public override string StateName => "Walking";
		public override OddState State => OddState.Walking;
		const float AnimationLength = 2f;
		const float AnimationSpeed = 1f;
		const float AnimationTime = AnimationLength / AnimationSpeed;
		public const float OddMaxSpeed = 2f;

		bool m_CollidedSides;
		static Collider[] m_CheckStairColliders;
		Timer m_AnimationTimer;
		public WalkingState()
		{
			m_AnimationTimer = new Timer(AnimationTime, true, true);
			m_AnimationTimer.OnTimerTrigger += OnAnimationFinish;

			if (m_CheckStairColliders == null)
				m_CheckStairColliders = new Collider[4];
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
			Controller.ME.SetMaxSpeed(OddMaxSpeed);
			Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed);
			Controller.ME.SetSightSpeed(Controller.ME.GetAngularSpeed());
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
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0f);
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			m_AnimationTimer.Update();

			if (Input.GetKeyDown(KeyCode.Space))
			{
				if (!Input.GetKey(KeyCode.LeftShift))
					Controller.ChangeState(OddState.Dashing);
				else
					Controller.ChangeState(OddState.Possessing);
				Controller.CurrentStateController.OnUpdate();
				return;
			}

			if(Input.GetMouseButton(1))
			{
				Controller.ChangeState(OddState.Covering);
				Controller.CurrentStateController.OnUpdate();
				return;
			}
			if (Input.GetMouseButtonDown(0))
			{
				var cam = CameraManager.Mgr.Camera;
				if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
				{
					Controller.ME.SetDirection(new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z));
				}
				Controller.ChangeState(OddState.AttackIn);
				Controller.CurrentStateController.OnUpdate();
				return;
			}
			if (Controller.SpellCaster.IsQuickItemTriggered())
			{
				Controller.ChangeState(OddState.Consumable);
				return;
			}

			var oldPos = Controller.transform.position;
			
			var movDir = Controller.ME.ComputeKeyboardMoveDirectionRelativeToCamera(CameraManager.Mgr.Camera, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D);
			if(movDir != Vector2.zero)
			{
				if(Controller.ME.GetCurrentMoment() >= OddMaxSpeed)
				{
					Controller.ChangeState(OddState.Running);
					Controller.CurrentStateController.OnUpdate();
					return;
				}
				Controller.ME.Impulse(movDir);
				Controller.LE.UpdateStruc();
				//Controller.LE.UpdateBlock();
			}
			else
			{
				if(Controller.ME.GetMovementState() == Def.MovementState.Stopped)
				{
					Controller.ChangeState(OddState.Idle);
					return;
				}
			}

			var struc = Controller.LE.GetCurrentStruc();
			if(struc == null)
			{
				Controller.LE.UpdateStruc();
				struc = Controller.LE.GetCurrentStruc();
			}
			if(Controller.LE.GetCurrentBlock() == null && struc != null)
			{
				Controller.LE.UpdateBlock();
			}

			var movXZ = Controller.ME.UpdateMovement();
			var mov = new Vector3(movXZ.x, GameUtils.Gravity, movXZ.y);
			mov *= Time.deltaTime;

			var cc = Controller.ME.GetController();
			var collision = cc.Move(mov);
			m_CollidedSides = false;
			if (collision.HasFlag(CollisionFlags.CollidedSides))
			{
				Controller.ME.OnCollision();
				var count = Physics.OverlapBoxNonAlloc(
					Controller.transform.position + new Vector3(Controller.ME.GetDirection().x * (0.4f + 0.25f), 0.2f, Controller.ME.GetDirection().y * (0.4f + 0.25f)),
					new Vector3(0.25f, 0.25f, 0.25f),
					m_CheckStairColliders, Quaternion.identity, 1 << Def.RCLayerBlock);
				var jumpable = GameUtils.GetJumpableBlock(Controller.transform.position, m_CheckStairColliders, count);
				if (jumpable != null)
				{
					Controller.ChangeState(OddState.Jumping);
					return;
				}
				m_CollidedSides = jumpable != null;
			}

			var tempBlock = Controller.LE.ComputeCurrentBlock() as CBlock;
			if(Controller.LE.GetCurrentBlock() != null && tempBlock == null)
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

			//var pos = Controller.transform.position + new Vector3(Controller.ME.GetDirection().x * (0.4f + 0.25f), 0.2f, Controller.ME.GetDirection().y * (0.4f + 0.25f));
			//Gizmos.DrawCube(pos, new Vector3(0.25f, 0.25f, 0.25f));
		}
		public override void OnGUI()
		{
			base.OnGUI();

			//var collidedContent = new GUIContent("C: " + m_CollidedSides.ToString());
			//var collidedSize = GUI.skin.label.CalcSize(collidedContent);
			//var collidedRect = new Rect(Screen.width * 0.5f - collidedSize.x * 0.5f,
			//	Screen.height * 0.5f - collidedSize.y * 0.5f, collidedSize.x, collidedSize.y);
			//GUI.Label(collidedRect, collidedContent);
		}
	}
}
