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
	public class DashState : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.Dash");
		//static readonly int AnimationInHash = Animator.StringToHash("BaseLayer.Dash_In");
		//static readonly int AnimationOutHash = Animator.StringToHash("BaseLayer.Dash_Out");
		public override string StateName => "Dash";
		public override OddState State => OddState.Dashing;
		const float AnimationLength = 0.5f;
		const float AnimationSpeed = 1f;
		public const float AnimationTime = AnimationLength / AnimationSpeed;

		//const float AnimationInLength = 0.333f;
		//const float AnimationInSpeed = 1f;
		//public const float AnimationInTime = AnimationInLength / AnimationInSpeed;

		//const float AnimationOutLength = 0.167f;
		//const float AnimationOutSpeed = 1f;
		//public const float AnimationOutTime = AnimationOutLength / AnimationOutSpeed;

		const float AnticipationTime = 3f / 24f;
		Timer m_DashTimer;
		//Timer m_AnimationTimer;
		Timer m_AnticipationTimer;
		Vector2 m_DashDir;
		UnityEngine.Rendering.Universal.MotionBlur m_Blur;
		OddState m_NextState;

		public DashState()
		{
			m_DashTimer = new Timer(AnimationTime, true, false);
			m_DashTimer.OnTimerTrigger += OnDashEnd;
			m_AnticipationTimer = new Timer(AnticipationTime, true, false);
			m_AnticipationTimer.OnTimerTrigger += OnAnticipationEnd;
			//m_AnimationTimer = new Timer(AnimationInTime, true, false);
			//m_AnimationTimer.OnTimerTrigger += OnAnimationEnd;
		}
		public override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
		}
		public override void OnLateUpdate()
		{
			base.OnLateUpdate();
		}
		void OnAnticipationEnd()
		{
			Controller.ME.SetDirectionInstantly(m_DashDir);
			Controller.ME.SetMaxSpeed(9f);
			
			if (m_Blur != null)
				m_Blur.intensity.Interp(m_Blur.intensity.min, m_Blur.intensity.max, 1f);

			var trail = Controller.Odd.GetDashTrailRenderer();
			if (trail != null)
			{
				trail.gameObject.SetActive(true);
				trail.material.SetFloat(COdd.DashTrailAlphaMultiplyID, 1f);
			}
		}
		public override void OnStart()
		{
			base.OnStart();
			if (!AssetLoader.PospoVolume.TryGet(out m_Blur))
				m_Blur = null;

			if(m_Blur != null)
			{
				m_Blur.active = true;
				m_Blur.intensity.Interp(m_Blur.intensity.min, m_Blur.intensity.max, 0f);
			}

			//Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationInHash, 0.2f, 0);
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0.2f, 0);
			var mgr = Manager.Mgr;
			if (mgr.GameInputControl == Def.GameInputControl.Mouse)
			{
				var cam = CameraManager.Mgr.Camera;
				if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
				{
					m_DashDir = (new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z)).normalized;
					Controller.ME.SetDirection(m_DashDir);
				}
			}
			else if(mgr.GameInputControl == Def.GameInputControl.MouseLikeController)
			{
				m_DashDir = Controller.ME.GetDirection();
				Controller.ME.SetDirection(m_DashDir);
			}
			//m_AnimationTimer.Reset(AnimationInTime);
			m_DashTimer.Reset(AnimationTime);
			m_AnticipationTimer.Reset(AnticipationTime);
			Controller.ME.Impulse(m_DashDir, 0f);
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(false);
			weapon.GetTrailRenderer().gameObject.SetActive(true);
			weapon.GetTrailRenderer().material.SetFloat(COdd.DashTrailAlphaMultiplyID, 0.3f);
			m_NextState = OddState.Running;
		}
		public override void OnStop()
		{
			base.OnStop();
			if(m_Blur != null)
				m_Blur.active = false;
		}
		void OnDashEnd()
		{
			//if (m_NextState == OddState.AttackIn)
			//	Controller.ME.SetDirection(m_NextDirection);
			var trail = Controller.Odd.GetDashTrailRenderer();
			if (trail != null)
				trail.gameObject.SetActive(false);

			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(m_NextState != OddState.DashAttack);
			weapon.GetTrailRenderer().gameObject.SetActive(false);

			Controller.ChangeState(m_NextState);
			Controller.CurrentStateController.OnUpdate();
		}
		//void OnAnimationEnd()
		//{
		//	if(m_AnimationTimer.GetTotalTime() == AnimationInTime)
		//	{
		//		if(m_NextState == OddState.Running) // keep dashing
		//		{
		//			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationOutHash, 0f, 0);
		//			m_AnimationTimer.Reset(AnimationOutTime);
		//		}
		//		else // Dash attack
		//		{
		//			Controller.ChangeState(OddState.DashAttack);
		//		}
		//	}
		//}
		public override void OnUpdate()
		{
			base.OnUpdate();

			m_AnticipationTimer.Update();

			if(m_AnticipationTimer.IsFinished()) // dash
			{
				float remainingTimePCT = m_DashTimer.GetRemainingPct();
				if(remainingTimePCT >= 0.75f)
				{
					float remainingPCT = (remainingTimePCT - 0.75f) * 4f;
					if (m_Blur != null)
						m_Blur.intensity.Interp(m_Blur.intensity.max, m_Blur.intensity.min, remainingPCT);
					var trail = Controller.Odd.GetDashTrailRenderer();
					if (trail != null)
						trail.material.SetFloat(COdd.DashTrailAlphaMultiplyID, 1f - remainingPCT);
				}

				if(/*m_AnimationTimer.GetTotalTime() == AnimationInTime &&*/ Input.GetMouseButtonDown(0))
				{
					m_NextState = OddState.DashAttack;
				}

				var oldPos = Controller.transform.position;
				Controller.ME.Impulse(m_DashDir, 10f);
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
				var mov = new Vector3(movXZ.x, -1f, movXZ.y);
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
			else
			{
				if(m_Blur != null)
					m_Blur.intensity.Interp(m_Blur.intensity.min, m_Blur.intensity.max, m_AnticipationTimer.GetRemainingPct());
			}

			//m_AnimationTimer.Update();
			m_DashTimer.Update();
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
