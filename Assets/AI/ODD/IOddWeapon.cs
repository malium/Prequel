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
	public class IOddWeapon : MonoBehaviour
	{
		[SerializeField] protected SpriteRenderer m_Renderer;
		[SerializeField] protected OddWeaponInfo m_Info;
		[SerializeField] protected IWeaponCollider m_Collider;
		[SerializeField] protected COddController m_Odd;

		public OddWeaponInfo WeaponInfo { get => m_Info; }
		public SpriteRenderer GetRenderer() => m_Renderer;
		public IWeaponCollider GetCollider() => m_Collider;
		public COddController GetOdd() => m_Odd;
		public virtual void Init(COddController odd, OddWeaponInfo info)
		{
			m_Odd = odd;
			m_Info = info;
			m_Renderer.sprite = m_Info.WeaponSprite;
		}
		public virtual void OnAttackBegin(Def.OddAttackType attackType)
		{

		}
		public virtual void OnAttackEnd()
		{

		}
	}
}
