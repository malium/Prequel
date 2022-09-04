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

namespace Assets
{
	public enum SpriteBackendType
	{
		SPRITE,
		MIX,
		SQUAD,
		SQUAD_DS,
		DQUAD,
		COUNT
	}
	public interface ISpriteBackend
	{
		SpriteBackendType GetBackendType();
		bool IsHorizontalFlip();
		bool IsVerticalFlip();
		void Flip(bool horizontal, bool vertical);
		void SetColor(Color color);
		Color GetColor();
		void SetOnTriggerEnter(Action<Collider> onTriggerEnter);
		Renderer GetRenderer();
		void ChangeSprite(Sprite sprite);
		void SetSprite(Sprite sprite);
		Sprite GetSprite();
		void SetEnabled(bool enable);
	}
}
