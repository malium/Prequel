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
	public class BackDashState : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.BackDash");
		public override string StateName => "BackDash";
		public override OddState State => OddState.BackDashing;
		const float AnimationLength = 0.5f;
		const float AnimationSpeed = 1f;
		public const float AnimationTime = AnimationLength / AnimationSpeed;

		const float AnticipationTime = 3f / 24f;
		Timer m_DashTimer;
		Timer m_AnticipationTimer;
		Vector2 m_DashDir;
		UnityEngine.Rendering.Universal.MotionBlur m_Blur;

		public BackDashState()
		{
			m_DashTimer = new Timer(true, false);
			m_DashTimer.OnTimerTrigger += OnDashEnd;
			m_AnticipationTimer = new Timer(true, false);
			m_AnticipationTimer.OnTimerTrigger += OnAnticipationEnd;
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
			Controller.ME.SetMaxSpeed(5f);

			m_Blur?.intensity.Interp(m_Blur.intensity.min, m_Blur.intensity.max, 1f);

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
			if (AssetLoader.PospoVolume.TryGet(out m_Blur))
			{
				m_Blur.active = true;
				m_Blur.intensity.Interp(m_Blur.intensity.min, m_Blur.intensity.max, 0f);
			}
			else
			{
				m_Blur = null;
			}
			
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0.2f, 0);

			m_DashDir = -Controller.ME.GetSightDirection();
			Controller.ME.SetDirectionInstantly(m_DashDir);

			m_DashTimer.Reset(AnimationTime);
			m_AnticipationTimer.Reset(AnticipationTime);
			Controller.ME.Impulse(m_DashDir, 0f);
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(false);
			weapon.GetTrailRenderer().gameObject.SetActive(true);
			weapon.GetTrailRenderer().material.SetFloat(COdd.DashTrailAlphaMultiplyID, 0.3f);
		}
		public override void OnStop()
		{
			base.OnStop();
			if (m_Blur != null)
				m_Blur.active = false;

			Controller.ME.SetDirectionInstantly(Controller.ME.GetSightDirection());
			Controller.ME.SetSightSpeed(COddController.OddAngularSpeed);
		}
		void OnDashEnd()
		{
			var trail = Controller.Odd.GetDashTrailRenderer();
			if (trail != null)
				trail.gameObject.SetActive(false);

			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.GetTrailRenderer().gameObject.SetActive(false);

			OddState nextState;
			if(Input.GetMouseButton(1))
			{
				nextState = OddState.Covering;
			}
			else
			{
				nextState = OddState.Idle;
				Controller.SetRotateTo(COddController.RotateTo.Movement);
				weapon.LookAtCamera(true);
			}

			Controller.ChangeState(nextState);
			Controller.CurrentStateController.OnUpdate();
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			m_AnticipationTimer.Update();

			if (m_AnticipationTimer.IsFinished()) // dash
			{
				float remainingTimePCT = m_DashTimer.GetRemainingPct();
				if (remainingTimePCT >= 0.75f)
				{
					float remainingPCT = (remainingTimePCT - 0.75f) * 4f;
					if (m_Blur != null)
						m_Blur.intensity.Interp(m_Blur.intensity.max, m_Blur.intensity.min, remainingPCT);
					var trail = Controller.Odd.GetDashTrailRenderer();
					if (trail != null)
						trail.material.SetFloat(COdd.DashTrailAlphaMultiplyID, 1f - remainingPCT);
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
				m_Blur?.intensity.Interp(m_Blur.intensity.min, m_Blur.intensity.max, m_AnticipationTimer.GetRemainingPct());
			}

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
