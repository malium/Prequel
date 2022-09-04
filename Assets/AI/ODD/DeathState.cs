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
	public class DeathState : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.Death");
		public override string StateName => "Death";
		public override OddState State => OddState.Death;
		const float AnimationLength = 0.792f;
		const float AnimationSpeed = 1f;
		public const float AnimationTime = AnimationLength / AnimationSpeed;

		Timer m_AnimationTimer;

		public DeathState()
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
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0.2f, 0);
			Controller.Odd.GetMeshRenderer().material = Controller.Odd.DeathMaterial;
			Controller.Odd.GetOutline().enabled = false;
			m_AnimationTimer.Reset(AnimationTime);
		}
		public override void OnStop()
		{
			base.OnStop();
		}
		void OnAnimationFinish()
		{
			GameUtils.DeleteGameobject(Controller.gameObject);
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			Color SetAlpha(Color c, float a) => new Color(c.r, c.g, c.b, a);

			float alpha = 1f - m_AnimationTimer.GetRemainingPct();
			Controller.Odd.GetMeshRenderer().material.color = SetAlpha(Controller.Odd.GetMeshRenderer().material.color, alpha);
			Controller.Odd.GetFaceRenderer().color = SetAlpha(Controller.Odd.GetFaceRenderer().color, alpha);
			Controller.GetWeapons()[0].GetRenderer().material.color = SetAlpha(Controller.GetWeapons()[0].GetRenderer().material.color, alpha);
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
