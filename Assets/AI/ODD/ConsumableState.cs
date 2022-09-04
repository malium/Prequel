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
	public class ConsumableState : IOddState
	{
		static readonly int AnimationFastHash = Animator.StringToHash("BaseLayer.Consumable_Fast");
		static readonly int AnimationEatingHash = Animator.StringToHash("BaseLayer.Consumable_Eating");
		public override string StateName => "Consumable";
		public override OddState State => OddState.Consumable;
		const float AnimationFastLength = 0.5f;
		const float AnimationFastSpeed = 1f;
		public const float AnimationFastTime = AnimationFastLength / AnimationFastSpeed;

		const float AnimationEatingLength = 1f;
		const float AnimationEatingSpeed = 1f;
		public const float AnimationEatingTime = AnimationEatingLength / AnimationEatingSpeed;

		Timer m_AnimationTimer;
		public ConsumableState()
		{
			m_AnimationTimer = new Timer(true, false);
			m_AnimationTimer.OnTimerTrigger += OnAnimationFinish;
		}
		void OnAnimationFinish()
		{
			var sc = Controller.SpellCaster;
			var quickItemSlot = sc.GetTriggeredQuickItemSlot();
			var itemSlot = sc.GetItemSlot(sc.GetQuickItems()[(int)quickItemSlot]);
			itemSlot.Item.Activate(sc);

			sc.EnableUseQuickItems(true);
			Controller.ChangeState(OddState.Idle);
			Controller.Odd.GetItemRenderer().gameObject.SetActive(false);
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
			
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(false);
			//Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed * 0.5f);
			//Controller.ME.SetMaxSpeed(WalkingState.OddMaxSpeed * 0.5f);
			Controller.SetCurrentAttack(Def.OddAttackType.COUNT);
			Controller.ME.Impulse(Controller.ME.GetDirection(), 0f);
			var sc = Controller.SpellCaster;
			sc.EnableUseQuickItems(false);
			var quickItemSlot = sc.GetTriggeredQuickItemSlot();
			var itemSlot = sc.GetItemSlot(sc.GetQuickItems()[(int)quickItemSlot]);

			float time = 0f;
			int hash = AnimationFastHash;

			var item = itemSlot.Item;
			var itemRnd = Controller.Odd.GetItemRenderer();
			Sprite img;
			if (Items.ItemLoader.ItemSpriteDict.TryGetValue(item.GetImageName(), out int imageIdx))
				img = Items.ItemLoader.ItemSprites[imageIdx];
			else
				img = Items.ItemLoader.InvalidItem;
			itemRnd.sprite = img;
			itemRnd.gameObject.SetActive(true);
			switch (item.GetConsumableType())
			{
				case Def.ItemConsumableType.Fast:
					hash = AnimationFastHash;
					time = AnimationFastTime;
					break;
				case Def.ItemConsumableType.Eating:
					hash = AnimationEatingHash;
					time = AnimationEatingTime;
					break;
				default:
					Debug.LogWarning("Unhandled ConsumableType " + item.GetConsumableType().ToString());
					break;
			}

			Controller.Odd.GetItemRenderer().transform.localRotation = Quaternion.identity;
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(hash, 0.2f, 0);
			m_AnimationTimer.Reset(time);
		}
		public override void OnStop()
		{
			base.OnStop();
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(true);
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

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

			// Item look at camera
			var current = Controller.Odd.GetItemRenderer().transform.rotation;
			var cam = CameraManager.Mgr.Camera;
			Controller.Odd.GetItemRenderer().transform.LookAt(cam.transform);
			
			Controller.Odd.GetItemRenderer().transform.rotation = Quaternion.Slerp(current, Controller.Odd.GetItemRenderer().transform.rotation, 4 * Time.deltaTime);

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
