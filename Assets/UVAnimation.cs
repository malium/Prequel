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
	public class UVAnimation : MonoBehaviour
	{
		public Renderer Rnd;
		public float Speed;
		public Vector2 UV;

		private void Update()
		{
			var offset = Rnd.material.mainTextureOffset;
			float speed = Speed * Time.deltaTime;
			offset = new Vector2(offset.x + UV.x * speed, offset.y + UV.y * speed);
			Rnd.material.mainTextureOffset = offset;
		}
	}
}