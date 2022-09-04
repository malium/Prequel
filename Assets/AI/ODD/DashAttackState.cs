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
	//public class DashAttackState : IOddState
	//{
	//	static readonly int AnimationHash = Animator.StringToHash("BaseLayer.Dash_Attack");
	//	public override string StateName => "Dash Attack";
	//	public override OddState State => OddState.DashAttack;
	//	const float AnimationLength = 0.292f;
	//	const float AnimationSpeed = 1f;
	//	public const float AnimationTime = AnimationLength / AnimationSpeed;
	//	const float ImpulseDurationPCT = DashState.AnimationOutTime / AnimationTime;

	//	Timer m_AnimationTimer;
	//	OddState m_NextState;
	//	Vector2 m_DashDir;
	//	Vector2 m_NextDirection;

	//	public DashAttackState()
	//	{
	//		m_AnimationTimer = new Timer(AnimationTime, true, false);
	//		m_AnimationTimer.OnTimerTrigger += OnAnimationEnd;
	//	}
	//	public override void OnFixedUpdate()
	//	{
	//		base.OnFixedUpdate();
	//	}
	//	public override void OnLateUpdate()
	//	{
	//		base.OnLateUpdate();
	//	}
	//	public override void OnStart()
	//	{
	//		base.OnStart();
	//		Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0f, 0);
	//		m_AnimationTimer.Reset(AnimationTime);
	//		m_NextState = OddState.AttackRecovering;
	//		m_DashDir = Controller.ME.GetDirection();
	//		var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
	//		weapon.OnAttackBegin(Def.OddAttackType.Dash);
	//		Controller.Odd.CurrentAttack = Def.OddAttackType.Dash;
	//	}
	//	public override void OnStop()
	//	{
	//		base.OnStop();
	//		if (m_NextState != OddState.AttackRecovering)
	//			Controller.GetWeapons()[0].OnAttackEnd();
	//	}
	//	void OnAnimationEnd()
	//	{
	//		if (m_NextState == OddState.AttackIn)
	//			Controller.ME.SetDirection(m_NextDirection);
	//		Controller.ChangeState(m_NextState);
	//		Controller.CurrentStateController.OnUpdate();
	//	}
	//	public override void OnUpdate()
	//	{
	//		base.OnUpdate();

	//		if (Input.GetMouseButtonDown(0))
	//		{
	//			m_NextState = OddState.AttackIn;
	//			var cam = CameraManager.Mgr.Camera;
	//			if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
	//			{
	//				m_NextDirection = new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z);
	//			}
	//		}

	//		var oldPos = Controller.transform.position;
	//		var remainingPCT = m_AnimationTimer.GetRemainingPct();
	//		if(remainingPCT <= ImpulseDurationPCT)
	//			Controller.ME.Impulse(m_DashDir, 10f);
	//		Controller.LE.UpdateStruc();

	//		var struc = Controller.LE.GetCurrentStruc();
	//		if (struc == null)
	//		{
	//			Controller.LE.UpdateStruc();
	//			struc = Controller.LE.GetCurrentStruc();
	//		}
	//		if (Controller.LE.GetCurrentBlock() == null && struc != null)
	//		{
	//			Controller.LE.UpdateBlock();
	//		}

	//		var movXZ = Controller.ME.UpdateMovement();
	//		var mov = new Vector3(movXZ.x, -1f, movXZ.y);
	//		mov *= Time.deltaTime;

	//		var cc = Controller.ME.GetController();
	//		var collision = cc.Move(mov);
	//		if (collision.HasFlag(CollisionFlags.CollidedSides))
	//			Controller.ME.OnCollision();

	//		var tempBlock = Controller.LE.ComputeCurrentBlock() as CBlock;
	//		if (Controller.LE.GetCurrentBlock() != null && tempBlock == null)
	//		{
	//			Controller.transform.position = oldPos;
	//			Controller.ME.OnCollision();
	//		}
	//		else
	//		{
	//			Controller.LE.SetCurrentBlock(tempBlock);
	//		}

	//		m_AnimationTimer.Update();
	//	}
	//}
	public class DashAttackState : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.Dash_Attack_End");
		public override string StateName => "Dash Attack";
		public override OddState State => OddState.DashAttack;

		const float AnimationLength = 0.292f;
		const float AnimationSpeed = 1f;
		public const float AnimationTime = AnimationLength / AnimationSpeed;
		const float AnimationFrames = 24f * AnimationLength;


		Timer m_AnimationTimer;
		OddState m_NextState;
		Vector2 m_NextDirection;

		public DashAttackState()
		{
			m_AnimationTimer = new Timer(AnimationTime, true, false);
			m_AnimationTimer.OnTimerTrigger += OnAnimationEnd;
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
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0f, 0);
			m_AnimationTimer.Reset(AnimationTime);
			m_NextState = OddState.AttackRecovering;
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.OnAttackBegin(Def.OddAttackType.Dash);
			Controller.SetCurrentAttack(Def.OddAttackType.Dash);

			var tar = new COddController.TargetArmRotationInfo()
			{
				Angle = 0f,
				Duration = AnimationTime,
				StartAxis = Controller.StartAxis[(int)Def.OddAttackType.Dash], //	Vector3.forward,
				EndAxis = Controller.EndAxis[(int)Def.OddAttackType.Dash], //Vector3.right,
				StartFadeInPCT = 0f,
				EndFadeInPCT = 2f / AnimationFrames,
				StartFadeOutPCT = (AnimationFrames - 2f) / AnimationFrames,
				EndFadeOutPCT = 1f
			};
			//var targetEntity = Controller.Odd.GetTargetEntity();
			//if (targetEntity != null)
			//{
			//	tar.Angle = Controller.GetArmRotationForTarget(targetEntity);
			//}
			Controller.SetTargetArmRotation(tar);
		}
		public override void OnStop()
		{
			base.OnStop();
			if (m_NextState != OddState.AttackRecovering)
				Controller.GetWeapons()[0].OnAttackEnd();
		}
		void OnAnimationEnd()
		{
			if (m_NextState == OddState.AttackIn)
				Controller.ME.SetDirection(m_NextDirection);
			Controller.ChangeState(m_NextState);
			Controller.CurrentStateController.OnUpdate();
		}
		public override void OnUpdate()
		{
			base.OnUpdate();
			//UnityEditor.EditorApplication.isPaused = true;
			if (Input.GetMouseButtonDown(0))
			{
				m_NextState = OddState.AttackIn;
				var cam = CameraManager.Mgr.Camera;
				if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
				{
					m_NextDirection = new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z);
				}
			}

			var oldPos = Controller.transform.position;
			//var remainingPCT = m_AnimationTimer.GetRemainingPct();
			//if (remainingPCT <= ImpulseDurationPCT)
			//	Controller.ME.Impulse(m_DashDir, 10f);
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
