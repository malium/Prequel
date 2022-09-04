/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets
//{
//	public enum BlockMeshType
//	{
//		TOP,
//		MID,
//		BOTTOM,
//		COUNT
//	}

//	public static class BlockMeshDef
//	{
//		//public static Vector3[][] Boxes = new Vector3[(int)BlockType.COUNT][]
//		//    {
//		//        new Vector3[2]{ new Vector3(1.0f, 0.1f, 1.0f), new Vector3(-0.5f, -0.05f, 0.5f) },
//		//        new Vector3[2]{ new Vector3(), new Vector3() },
//		//        new Vector3[2]{ new Vector3(), new Vector3() },
//		//    };

//		public static class TopMesh
//		{
//			public static Vector3[][] Vertices = new Vector3[Def.BlockTypeCount][]
//			{
//				new Vector3[]
//				{
//					new Vector3(-0.1f, 0.0f, 0.9f),
//					new Vector3(-0.1f, -0.029289f, 0.970711f),
//					new Vector3(-0.029289f, -0.029289f, 0.9f),
//					new Vector3(-0.1f, 0.0f, 0.1f),
//					new Vector3(-0.029289f, -0.029289f, 0.1f),
//					new Vector3(-0.1f, -0.029289f, 0.029289f),
//					new Vector3(-0.9f, 0.0f, 0.9f),
//					new Vector3(-0.970711f, -0.029289f, 0.9f),
//					new Vector3(-0.9f, -0.029289f, 0.970711f),
//					new Vector3(-0.9f, 0.0f, 0.1f),
//					new Vector3(-0.9f, -0.029289f, 0.029289f),
//					new Vector3(-0.970711f, -0.029289f, 0.1f),
//					new Vector3(-0.1f, 0.0f, 0.1f),
//					new Vector3(-0.9f, 0.0f, 0.1f),
//					new Vector3(-0.9f, 0.0f, 0.9f),
//					new Vector3(-0.1f, 0.0f, 0.9f),
//					new Vector3(-0.1f, -0.1f, 1.0f),
//					new Vector3(0.0f, -0.1f, 0.9f),
//					new Vector3(-0.029289f, -0.029289f, 0.9f),
//					new Vector3(-0.1f, -0.029289f, 0.970711f),
//					new Vector3(-0.029289f, -0.029289f, 0.1f),
//					new Vector3(0.0f, -0.1f, 0.1f),
//					new Vector3(-0.1f, -0.1f, 0.0f),
//					new Vector3(-0.1f, -0.029289f, 0.029289f),
//					new Vector3(-0.970711f, -0.029289f, 0.9f),
//					new Vector3(-1.0f, -0.1f, 0.9f),
//					new Vector3(-0.9f, -0.1f, 1.0f),
//					new Vector3(-0.9f, -0.029289f, 0.970711f),
//					new Vector3(-0.9f, -0.1f, 0.0f),
//					new Vector3(-1.0f, -0.1f, 0.1f),
//					new Vector3(-0.970711f, -0.029289f, 0.1f),
//					new Vector3(-0.9f, -0.029289f, 0.029289f),
//					new Vector3(-0.1f, 0.0f, 0.1f),
//					new Vector3(-0.1f, 0.0f, 0.9f),
//					new Vector3(-0.029289f, -0.029289f, 0.9f),
//					new Vector3(-0.029289f, -0.029289f, 0.1f),
//					new Vector3(-0.029289f, -0.029289f, 0.1f),
//					new Vector3(-0.029289f, -0.029289f, 0.9f),
//					new Vector3(0.0f, -0.1f, 0.9f),
//					new Vector3(0.0f, -0.1f, 0.1f),
//					new Vector3(-0.9f, 0.0f, 0.1f),
//					new Vector3(-0.1f, 0.0f, 0.1f),
//					new Vector3(-0.1f, -0.029289f, 0.029289f),
//					new Vector3(-0.9f, -0.029289f, 0.029289f),
//					new Vector3(-0.9f, -0.029289f, 0.029289f),
//					new Vector3(-0.1f, -0.029289f, 0.029289f),
//					new Vector3(-0.1f, -0.1f, 0.0f),
//					new Vector3(-0.9f, -0.1f, 0.0f),
//					new Vector3(-0.9f, 0.0f, 0.9f),
//					new Vector3(-0.9f, 0.0f, 0.1f),
//					new Vector3(-0.970711f, -0.029289f, 0.1f),
//					new Vector3(-0.970711f, -0.029289f, 0.9f),
//					new Vector3(-0.970711f, -0.029289f, 0.9f),
//					new Vector3(-0.970711f, -0.029289f, 0.1f),
//					new Vector3(-1.0f, -0.1f, 0.1f),
//					new Vector3(-1.0f, -0.1f, 0.9f),
//					new Vector3(-0.1f, 0.0f, 0.9f),
//					new Vector3(-0.9f, 0.0f, 0.9f),
//					new Vector3(-0.9f, -0.029289f, 0.970711f),
//					new Vector3(-0.1f, -0.029289f, 0.970711f),
//					new Vector3(-0.1f, -0.029289f, 0.970711f),
//					new Vector3(-0.9f, -0.029289f, 0.970711f),
//					new Vector3(-0.9f, -0.1f, 1.0f),
//					new Vector3(-0.1f, -0.1f, 1.0f),
//				},
//				new Vector3[]
//				{
//					new Vector3(-0.1f, 0.0f, 0.9f),
//					new Vector3(-0.1f, -0.029289f, 0.970711f),
//					new Vector3(-0.029289f, -0.029289f, 0.9f),
//					new Vector3(-0.9f, 0.0f, 0.9f),
//					new Vector3(-0.970711f, -0.029289f, 0.9f),
//					new Vector3(-0.9f, -0.029289f, 0.970711f),
//					new Vector3(-0.1f, 0.5f, 0.1f),
//					new Vector3(-0.029289f, 0.470711f, 0.1f),
//					new Vector3(-0.1f, 0.470711f, 0.029289f),
//					new Vector3(-0.9f, 0.5f, 0.1f),
//					new Vector3(-0.9f, 0.470711f, 0.029289f),
//					new Vector3(-0.970711f, 0.470711f, 0.1f),
//					new Vector3(-0.1f, 0.5f, 0.4f),
//					new Vector3(-0.1f, 0.470711f, 0.470711f),
//					new Vector3(-0.029289f, 0.470711f, 0.4f),
//					new Vector3(-0.9f, 0.5f, 0.4f),
//					new Vector3(-0.970711f, 0.470711f, 0.4f),
//					new Vector3(-0.9f, 0.470711f, 0.470711f),
//					new Vector3(-0.1f, 0.0f, 0.6f),
//					new Vector3(-0.029289f, -0.029289f, 0.6f),
//					new Vector3(-0.1f, -0.029289f, 0.529289f),
//					new Vector3(-0.9f, 0.0f, 0.6f),
//					new Vector3(-0.9f, -0.029289f, 0.529289f),
//					new Vector3(-0.970711f, -0.029289f, 0.6f),
//					new Vector3(-0.1f, -0.1f, 0.5f),
//					new Vector3(0.0f, -0.1f, 0.6f),
//					new Vector3(0.0f, -0.1f, 0.4f),
//					new Vector3(-0.9f, -0.1f, 0.5f),
//					new Vector3(-1.0f, -0.1f, 0.4f),
//					new Vector3(-1.0f, -0.1f, 0.6f),
//					new Vector3(0.0f, -0.1f, 0.9f),
//					new Vector3(-0.029289f, -0.029289f, 0.9f),
//					new Vector3(-0.1f, -0.029289f, 0.970711f),
//					new Vector3(-0.1f, -0.1f, 1f),
//					new Vector3(-1.0f, -0.1f, 0.9f),
//					new Vector3(-0.9f, -0.1f, 1f),
//					new Vector3(-0.9f, -0.029289f, 0.970711f),
//					new Vector3(-0.970711f, -0.029289f, 0.9f),
//					new Vector3(-0.9f, 0.0f, 0.9f),
//					new Vector3(-0.9f, -0.029289f, 0.970711f),
//					new Vector3(-0.1f, -0.029289f, 0.970711f),
//					new Vector3(-0.1f, 0.0f, 0.9f),
//					new Vector3(-0.9f, -0.029289f, 0.970711f),
//					new Vector3(-0.9f, -0.1f, 1f),
//					new Vector3(-0.1f, -0.1f, 1f),
//					new Vector3(-0.1f, -0.029289f, 0.970711f),
//					new Vector3(-0.1f, -0.1f, 0.0f),
//					new Vector3(-0.9f, -0.1f, 0.0f),
//					new Vector3(-0.9f, 0.4f, 0.0f),
//					new Vector3(-0.1f, 0.4f, 0.0f),
//					new Vector3(0.0f, 0.4f, 0.1f),
//					new Vector3(-0.1f, 0.4f, 0.0f),
//					new Vector3(-0.1f, 0.470711f, 0.029289f),
//					new Vector3(-0.029289f, 0.470711f, 0.1f),
//					new Vector3(-0.9f, 0.4f, 0.0f),
//					new Vector3(-1.0f, 0.4f, 0.1f),
//					new Vector3(-0.970711f, 0.470711f, 0.1f),
//					new Vector3(-0.9f, 0.470711f, 0.029289f),
//					new Vector3(-0.1f, 0.5f, 0.4f),
//					new Vector3(-0.029289f, 0.470711f, 0.4f),
//					new Vector3(-0.029289f, 0.470711f, 0.1f),
//					new Vector3(-0.1f, 0.5f, 0.1f),
//					new Vector3(-0.029289f, 0.470711f, 0.4f),
//					new Vector3(0.0f, 0.4f, 0.4f),
//					new Vector3(0.0f, 0.4f, 0.1f),
//					new Vector3(-0.029289f, 0.470711f, 0.1f),
//					new Vector3(-0.1f, 0.5f, 0.1f),
//					new Vector3(-0.1f, 0.470711f, 0.029289f),
//					new Vector3(-0.9f, 0.470711f, 0.029289f),
//					new Vector3(-0.9f, 0.5f, 0.1f),
//					new Vector3(-0.1f, 0.470711f, 0.029289f),
//					new Vector3(-0.1f, 0.4f, 0.0f),
//					new Vector3(-0.9f, 0.4f, 0.0f),
//					new Vector3(-0.9f, 0.470711f, 0.029289f),
//					new Vector3(-0.970711f, 0.470711f, 0.1f),
//					new Vector3(-1.0f, 0.4f, 0.1f),
//					new Vector3(-1.0f, 0.4f, 0.4f),
//					new Vector3(-0.970711f, 0.470711f, 0.4f),
//					new Vector3(-0.9f, 0.5f, 0.1f),
//					new Vector3(-0.970711f, 0.470711f, 0.1f),
//					new Vector3(-0.970711f, 0.470711f, 0.4f),
//					new Vector3(-0.9f, 0.5f, 0.4f),
//					new Vector3(-0.9f, 0.5f, 0.1f),
//					new Vector3(-0.9f, 0.5f, 0.4f),
//					new Vector3(-0.1f, 0.5f, 0.4f),
//					new Vector3(-0.1f, 0.5f, 0.1f),
//					new Vector3(0.0f, 0.4f, 0.4f),
//					new Vector3(-0.029289f, 0.470711f, 0.4f),
//					new Vector3(-0.1f, 0.470711f, 0.470711f),
//					new Vector3(-0.1f, 0.4f, 0.5f),
//					new Vector3(-1.0f, 0.4f, 0.4f),
//					new Vector3(-0.9f, 0.4f, 0.5f),
//					new Vector3(-0.9f, 0.470711f, 0.470711f),
//					new Vector3(-0.970711f, 0.470711f, 0.4f),
//					new Vector3(-0.9f, 0.5f, 0.4f),
//					new Vector3(-0.9f, 0.470711f, 0.470711f),
//					new Vector3(-0.1f, 0.470711f, 0.470711f),
//					new Vector3(-0.1f, 0.5f, 0.4f),
//					new Vector3(-0.9f, 0.470711f, 0.470711f),
//					new Vector3(-0.9f, 0.4f, 0.5f),
//					new Vector3(-0.1f, 0.4f, 0.5f),
//					new Vector3(-0.1f, 0.470711f, 0.470711f),
//					new Vector3(-0.9f, -0.1f, 0.5f),
//					new Vector3(-0.1f, -0.1f, 0.5f),
//					new Vector3(-0.1f, 0.4f, 0.5f),
//					new Vector3(-0.9f, 0.4f, 0.5f),
//					new Vector3(0.0f, 0.4f, 0.4f),
//					new Vector3(0.0f, -0.1f, 0.4f),
//					new Vector3(0.0f, -0.1f, 0.1f),
//					new Vector3(0.0f, 0.4f, 0.1f),
//					new Vector3(-1.0f, -0.1f, 0.4f),
//					new Vector3(-1.0f, 0.4f, 0.4f),
//					new Vector3(-1.0f, 0.4f, 0.1f),
//					new Vector3(-1.0f, -0.1f, 0.1f),
//					new Vector3(-0.9f, -0.1f, 0.0f),
//					new Vector3(-1.0f, -0.1f, 0.1f),
//					new Vector3(-1.0f, 0.4f, 0.1f),
//					new Vector3(-0.9f, 0.4f, 0.0f),
//					new Vector3(0.0f, -0.1f, 0.1f),
//					new Vector3(-0.1f, -0.1f, 0.0f),
//					new Vector3(-0.1f, 0.4f, 0.0f),
//					new Vector3(0.0f, 0.4f, 0.1f),
//					new Vector3(-1.0f, 0.4f, 0.4f),
//					new Vector3(-1.0f, -0.1f, 0.4f),
//					new Vector3(-0.9f, -0.1f, 0.5f),
//					new Vector3(-0.9f, 0.4f, 0.5f),
//					new Vector3(-0.1f, 0.4f, 0.5f),
//					new Vector3(-0.1f, -0.1f, 0.5f),
//					new Vector3(0.0f, -0.1f, 0.4f),
//					new Vector3(0.0f, 0.4f, 0.4f),
//					new Vector3(0.0f, -0.1f, 0.6f),
//					new Vector3(-0.1f, -0.1f, 0.5f),
//					new Vector3(-0.1f, -0.029289f, 0.529289f),
//					new Vector3(-0.029289f, -0.029289f, 0.6f),
//					new Vector3(-0.9f, -0.1f, 0.5f),
//					new Vector3(-1.0f, -0.1f, 0.6f),
//					new Vector3(-0.970711f, -0.029289f, 0.6f),
//					new Vector3(-0.9f, -0.029289f, 0.529289f),
//					new Vector3(-0.1f, 0.0f, 0.6f),
//					new Vector3(-0.1f, -0.029289f, 0.529289f),
//					new Vector3(-0.9f, -0.029289f, 0.529289f),
//					new Vector3(-0.9f, 0.0f, 0.6f),
//					new Vector3(-0.1f, -0.029289f, 0.529289f),
//					new Vector3(-0.1f, -0.1f, 0.5f),
//					new Vector3(-0.9f, -0.1f, 0.5f),
//					new Vector3(-0.9f, -0.029289f, 0.529289f),
//					new Vector3(-1.0f, -0.1f, 0.9f),
//					new Vector3(-0.970711f, -0.029289f, 0.9f),
//					new Vector3(-0.970711f, -0.029289f, 0.6f),
//					new Vector3(-1.0f, -0.1f, 0.6f),
//					new Vector3(-0.970711f, -0.029289f, 0.9f),
//					new Vector3(-0.9f, 0.0f, 0.9f),
//					new Vector3(-0.9f, 0.0f, 0.6f),
//					new Vector3(-0.970711f, -0.029289f, 0.6f),
//					new Vector3(-0.9f, 0.0f, 0.9f),
//					new Vector3(-0.1f, 0.0f, 0.9f),
//					new Vector3(-0.1f, 0.0f, 0.6f),
//					new Vector3(-0.9f, 0.0f, 0.6f),
//					new Vector3(-0.1f, 0.0f, 0.9f),
//					new Vector3(-0.029289f, -0.029289f, 0.9f),
//					new Vector3(-0.029289f, -0.029289f, 0.6f),
//					new Vector3(-0.1f, 0.0f, 0.6f),
//					new Vector3(-0.029289f, -0.029289f, 0.9f),
//					new Vector3(0.0f, -0.1f, 0.9f),
//					new Vector3(0.0f, -0.1f, 0.6f),
//					new Vector3(-0.029289f, -0.029289f, 0.6f)
//				},
//				new Vector3[]
//				{
//					new Vector3(-0.1f, 0.0f, 1.9f),
//					new Vector3(-0.1f, -0.029289f, 1.97071f),
//					new Vector3(-0.029289f, -0.029289f, 1.9f),
//					new Vector3(-0.1f, 0.0f, 0.1f),
//					new Vector3(-0.029289f, -0.029289f, 0.1f),
//					new Vector3(-0.1f, -0.029289f, 0.029289f),
//					new Vector3(-1.9f, 0.0f, 1.9f),
//					new Vector3(-1.970711f, -0.029289f, 1.9f),
//					new Vector3(-1.9f, -0.029289f, 1.97071f),
//					new Vector3(-1.9f, 0.0f, 0.1f),
//					new Vector3(-1.9f, -0.029289f, 0.029289f),
//					new Vector3(-1.970711f, -0.029289f, 0.1f),
//					new Vector3(-1.9f, 0.0f, 0.1f),
//					new Vector3(-1.9f, 0.0f, 1.9f),
//					new Vector3(-0.1f, 0.0f, 1.9f),
//					new Vector3(-0.1f, 0.0f, 0.1f),
//					new Vector3(0.0f, -0.1f, 1.9f),
//					new Vector3(-0.029289f, -0.029289f, 1.9f),
//					new Vector3(-0.1f, -0.029289f, 1.97071f),
//					new Vector3(-0.1f, -0.1f, 2f),
//					new Vector3(0.0f, -0.1f, 0.1f),
//					new Vector3(-0.1f, -0.1f, 0.0f),
//					new Vector3(-0.1f, -0.029289f, 0.029289f),
//					new Vector3(-0.029289f, -0.029289f, 0.1f),
//					new Vector3(-2f, -0.1f, 1.9f),
//					new Vector3(-1.9f, -0.1f, 2f),
//					new Vector3(-1.9f, -0.029289f, 1.97071f),
//					new Vector3(-1.970711f, -0.029289f, 1.9f),
//					new Vector3(-1.9f, -0.1f, 0.0f),
//					new Vector3(-2f, -0.1f, 0.1f),
//					new Vector3(-1.970711f, -0.029289f, 0.1f),
//					new Vector3(-1.9f, -0.029289f, 0.029289f),
//					new Vector3(-0.1f, 0.0f, 1.9f),
//					new Vector3(-0.029289f, -0.029289f, 1.9f),
//					new Vector3(-0.029289f, -0.029289f, 0.1f),
//					new Vector3(-0.1f, 0.0f, 0.1f),
//					new Vector3(-0.029289f, -0.029289f, 1.9f),
//					new Vector3(0.0f, -0.1f, 1.9f),
//					new Vector3(0.0f, -0.1f, 0.1f),
//					new Vector3(-0.029289f, -0.029289f, 0.1f),
//					new Vector3(-0.1f, 0.0f, 0.1f),
//					new Vector3(-0.1f, -0.029289f, 0.029289f),
//					new Vector3(-1.9f, -0.029289f, 0.029289f),
//					new Vector3(-1.9f, 0.0f, 0.1f),
//					new Vector3(-0.1f, -0.029289f, 0.029289f),
//					new Vector3(-0.1f, -0.1f, 0.0f),
//					new Vector3(-1.9f, -0.1f, 0.0f),
//					new Vector3(-1.9f, -0.029289f, 0.029289f),
//					new Vector3(-1.9f, 0.0f, 0.1f),
//					new Vector3(-1.970711f, -0.029289f, 0.1f),
//					new Vector3(-1.970711f, -0.029289f, 1.9f),
//					new Vector3(-1.9f, 0.0f, 1.9f),
//					new Vector3(-1.970711f, -0.029289f, 0.1f),
//					new Vector3(-2f, -0.1f, 0.1f),
//					new Vector3(-2f, -0.1f, 1.9f),
//					new Vector3(-1.970711f, -0.029289f, 1.9f),
//					new Vector3(-1.9f, 0.0f, 1.9f),
//					new Vector3(-1.9f, -0.029289f, 1.97071f),
//					new Vector3(-0.1f, -0.029289f, 1.97071f),
//					new Vector3(-0.1f, 0.0f, 1.9f),
//					new Vector3(-1.9f, -0.029289f, 1.97071f),
//					new Vector3(-1.9f, -0.1f, 2f),
//					new Vector3(-0.1f, -0.1f, 2f),
//					new Vector3(-0.1f, -0.029289f, 1.97071f),
//				},
//			};
//			public static Vector2[][] UVs = new Vector2[Def.BlockTypeCount][]
//			{
//				new Vector2[]
//				{
//					new Vector2(0.072281f, 0.075187f),
//					new Vector2(0.060137f, 0.03832f),
//					new Vector2(0.036492f, 0.079232f),
//					new Vector2(0.075268f, 0.927719f),
//					new Vector2(0.038172f, 0.941436f),
//					new Vector2(0.080323f, 0.96354f),
//					new Vector2(0.924813f, 0.072201f),
//					new Vector2(0.961909f, 0.058484f),
//					new Vector2(0.919758f, 0.036379f),
//					new Vector2(0.927799f, 0.924732f),
//					new Vector2(0.941516f, 0.961828f),
//					new Vector2(0.963621f, 0.919677f),
//					new Vector2(0.075268f, 0.927719f),
//					new Vector2(0.927799f, 0.924732f),
//					new Vector2(0.924813f, 0.072201f),
//					new Vector2(0.072281f, 0.075187f),
//					new Vector2(0.031504f, 0.011476f),
//					new Vector2(0.0001f, 0.075233f),
//					new Vector2(0.036492f, 0.079232f),
//					new Vector2(0.060137f, 0.03832f),
//					new Vector2(0.038172f, 0.941436f),
//					new Vector2(0.013197f, 0.969351f),
//					new Vector2(0.075341f, 0.999819f),
//					new Vector2(0.080323f, 0.96354f),
//					new Vector2(0.961909f, 0.058484f),
//					new Vector2(0.986883f, 0.030568f),
//					new Vector2(0.92474f, 0.0001f),
//					new Vector2(0.919758f, 0.036379f),
//					new Vector2(0.969432f, 0.986803f),
//					new Vector2(0.9999f, 0.924659f),
//					new Vector2(0.963621f, 0.919677f),
//					new Vector2(0.941516f, 0.961828f),
//					new Vector2(0.075268f, 0.927719f),
//					new Vector2(0.072281f, 0.075187f),
//					new Vector2(0.036492f, 0.079232f),
//					new Vector2(0.036419f, 0.927793f),
//					new Vector2(0.036419f, 0.927793f),
//					new Vector2(0.036492f, 0.079232f),
//					new Vector2(0.0001f, 0.075233f),
//					new Vector2(0.000168f, 0.927796f),
//					new Vector2(0.927799f, 0.924732f),
//					new Vector2(0.075268f, 0.927719f),
//					new Vector2(0.080323f, 0.96354f),
//					new Vector2(0.927871f, 0.963569f),
//					new Vector2(0.927871f, 0.963569f),
//					new Vector2(0.080323f, 0.96354f),
//					new Vector2(0.075341f, 0.999819f),
//					new Vector2(0.927871f, 0.999819f),
//					new Vector2(0.924813f, 0.072201f),
//					new Vector2(0.927799f, 0.924732f),
//					new Vector2(0.963621f, 0.919677f),
//					new Vector2(0.96365f, 0.072129f),
//					new Vector2(0.96365f, 0.072129f),
//					new Vector2(0.963621f, 0.919677f),
//					new Vector2(0.9999f, 0.924659f),
//					new Vector2(0.9999f, 0.072129f),
//					new Vector2(0.072281f, 0.075187f),
//					new Vector2(0.924813f, 0.072201f),
//					new Vector2(0.919758f, 0.036379f),
//					new Vector2(0.07221f, 0.03635f),
//					new Vector2(0.07221f, 0.03635f),
//					new Vector2(0.919758f, 0.036379f),
//					new Vector2(0.92474f, 0.0001f),
//					new Vector2(0.07221f, 0.0001f),
//				},
//				new Vector2[]
//				{
//					new Vector2(0.5838f, 0.083548f),
//					new Vector2(0.624844f, 0.070619f),
//					new Vector2(0.579556f, 0.044393f),
//					new Vector2(0.583801f, 0.503295f),
//					new Vector2(0.579713f, 0.544097f),
//					new Vector2(0.625455f, 0.51711f),
//					new Vector2(0.750243f, 0.083548f),
//					new Vector2(0.754257f, 0.042711f),
//					new Vector2(0.708542f, 0.069813f),
//					new Vector2(0.750243f, 0.503295f),
//					new Vector2(0.709193f, 0.517875f),
//					new Vector2(0.755513f, 0.542178f),
//					new Vector2(0.907648f, 0.083548f),
//					new Vector2(0.948691f, 0.070619f),
//					new Vector2(0.903404f, 0.044393f),
//					new Vector2(0.907648f, 0.503295f),
//					new Vector2(0.903561f, 0.544097f),
//					new Vector2(0.949302f, 0.51711f),
//					new Vector2(0.426395f, 0.083548f),
//					new Vector2(0.43041f, 0.042711f),
//					new Vector2(0.384694f, 0.069813f),
//					new Vector2(0.426395f, 0.503295f),
//					new Vector2(0.385345f, 0.517876f),
//					new Vector2(0.431666f, 0.542178f),
//					new Vector2(0.077697f, 0.002907f),
//					new Vector2(0.00343f, 0.002907f),
//					new Vector2(0.077697f, 0.077174f),
//					new Vector2(0.423019f, 0.734526f),
//					new Vector2(0.497286f, 0.734526f),
//					new Vector2(0.423019f, 0.66026f),
//					new Vector2(0.583798f, 0.002906f),
//					new Vector2(0.579556f, 0.044393f),
//					new Vector2(0.624844f, 0.070619f),
//					new Vector2(0.655168f, 0.038653f),
//					new Vector2(0.584176f, 0.583803f),
//					new Vector2(0.655387f, 0.549001f),
//					new Vector2(0.625455f, 0.51711f),
//					new Vector2(0.579713f, 0.544097f),
//					new Vector2(0.583801f, 0.503295f),
//					new Vector2(0.623958f, 0.503295f),
//					new Vector2(0.623958f, 0.083548f),
//					new Vector2(0.5838f, 0.083548f),
//					new Vector2(0.623958f, 0.503295f),
//					new Vector2(0.664116f, 0.503295f),
//					new Vector2(0.664116f, 0.083548f),
//					new Vector2(0.623958f, 0.083548f),
//					new Vector2(0.077697f, 0.308983f),
//					new Vector2(0.077697f, 0.729097f),
//					new Vector2(0.340268f, 0.729097f),
//					new Vector2(0.340268f, 0.308983f),
//					new Vector2(0.749687f, 0.00299f),
//					new Vector2(0.678522f, 0.037973f),
//					new Vector2(0.708542f, 0.069813f),
//					new Vector2(0.754257f, 0.042711f),
//					new Vector2(0.681056f, 0.548814f),
//					new Vector2(0.750158f, 0.583395f),
//					new Vector2(0.755513f, 0.542178f),
//					new Vector2(0.709193f, 0.517875f),
//					new Vector2(0.907648f, 0.083548f),
//					new Vector2(0.903404f, 0.044393f),
//					new Vector2(0.754257f, 0.042711f),
//					new Vector2(0.750243f, 0.083548f),
//					new Vector2(0.903404f, 0.044393f),
//					new Vector2(0.907645f, 0.002906f),
//					new Vector2(0.749687f, 0.00299f),
//					new Vector2(0.754257f, 0.042711f),
//					new Vector2(0.750243f, 0.083548f),
//					new Vector2(0.710085f, 0.083548f),
//					new Vector2(0.710085f, 0.503295f),
//					new Vector2(0.750243f, 0.503295f),
//					new Vector2(0.710085f, 0.083548f),
//					new Vector2(0.669927f, 0.083548f),
//					new Vector2(0.669927f, 0.503295f),
//					new Vector2(0.710085f, 0.503295f),
//					new Vector2(0.755513f, 0.542178f),
//					new Vector2(0.750158f, 0.583395f),
//					new Vector2(0.908023f, 0.583803f),
//					new Vector2(0.903561f, 0.544097f),
//					new Vector2(0.750243f, 0.503295f),
//					new Vector2(0.755513f, 0.542178f),
//					new Vector2(0.903561f, 0.544097f),
//					new Vector2(0.907648f, 0.503295f),
//					new Vector2(0.750243f, 0.503295f),
//					new Vector2(0.907648f, 0.503295f),
//					new Vector2(0.907648f, 0.083548f),
//					new Vector2(0.750243f, 0.083548f),
//					new Vector2(0.907645f, 0.002906f),
//					new Vector2(0.903404f, 0.044393f),
//					new Vector2(0.948691f, 0.070619f),
//					new Vector2(0.979015f, 0.038653f),
//					new Vector2(0.908023f, 0.583803f),
//					new Vector2(0.979235f, 0.549001f),
//					new Vector2(0.949302f, 0.51711f),
//					new Vector2(0.903561f, 0.544097f),
//					new Vector2(0.907648f, 0.503295f),
//					new Vector2(0.947806f, 0.503295f),
//					new Vector2(0.947806f, 0.083548f),
//					new Vector2(0.907648f, 0.083548f),
//					new Vector2(0.947806f, 0.503295f),
//					new Vector2(0.987963f, 0.503295f),
//					new Vector2(0.987963f, 0.083548f),
//					new Vector2(0.947806f, 0.083548f),
//					new Vector2(0.423019f, 0.734526f),
//					new Vector2(0.002905f, 0.734527f),
//					new Vector2(0.002906f, 0.997098f),
//					new Vector2(0.42302f, 0.997097f),
//					new Vector2(0.340268f, 0.077174f),
//					new Vector2(0.077697f, 0.077174f),
//					new Vector2(0.077697f, 0.234716f),
//					new Vector2(0.340268f, 0.234716f),
//					new Vector2(0.497286f, 0.734526f),
//					new Vector2(0.497286f, 0.997097f),
//					new Vector2(0.654829f, 0.997097f),
//					new Vector2(0.654828f, 0.734526f),
//					new Vector2(0.729095f, 0.734525f),
//					new Vector2(0.654828f, 0.734526f),
//					new Vector2(0.654829f, 0.997097f),
//					new Vector2(0.729095f, 0.997097f),
//					new Vector2(0.077697f, 0.234716f),
//					new Vector2(0.077697f, 0.308983f),
//					new Vector2(0.340268f, 0.308983f),
//					new Vector2(0.340268f, 0.234716f),
//					new Vector2(0.497286f, 0.997097f),
//					new Vector2(0.497286f, 0.734526f),
//					new Vector2(0.423019f, 0.734526f),
//					new Vector2(0.42302f, 0.997097f),
//					new Vector2(0.340268f, 0.002907f),
//					new Vector2(0.077697f, 0.002907f),
//					new Vector2(0.077697f, 0.077174f),
//					new Vector2(0.340268f, 0.077174f),
//					new Vector2(0.42584f, 0.00299f),
//					new Vector2(0.354675f, 0.037973f),
//					new Vector2(0.384694f, 0.069813f),
//					new Vector2(0.43041f, 0.042711f),
//					new Vector2(0.357208f, 0.548814f),
//					new Vector2(0.42631f, 0.583395f),
//					new Vector2(0.431666f, 0.542178f),
//					new Vector2(0.385345f, 0.517876f),
//					new Vector2(0.426395f, 0.083548f),
//					new Vector2(0.386237f, 0.083548f),
//					new Vector2(0.386238f, 0.503295f),
//					new Vector2(0.426395f, 0.503295f),
//					new Vector2(0.386237f, 0.083548f),
//					new Vector2(0.34608f, 0.083548f),
//					new Vector2(0.34608f, 0.503295f),
//					new Vector2(0.386238f, 0.503295f),
//					new Vector2(0.584176f, 0.583803f),
//					new Vector2(0.579713f, 0.544097f),
//					new Vector2(0.431666f, 0.542178f),
//					new Vector2(0.42631f, 0.583395f),
//					new Vector2(0.579713f, 0.544097f),
//					new Vector2(0.583801f, 0.503295f),
//					new Vector2(0.426395f, 0.503295f),
//					new Vector2(0.431666f, 0.542178f),
//					new Vector2(0.583801f, 0.503295f),
//					new Vector2(0.5838f, 0.083548f),
//					new Vector2(0.426395f, 0.083548f),
//					new Vector2(0.426395f, 0.503295f),
//					new Vector2(0.5838f, 0.083548f),
//					new Vector2(0.579556f, 0.044393f),
//					new Vector2(0.43041f, 0.042711f),
//					new Vector2(0.426395f, 0.083548f),
//					new Vector2(0.579556f, 0.044393f),
//					new Vector2(0.583798f, 0.002906f),
//					new Vector2(0.42584f, 0.00299f),
//					new Vector2(0.43041f, 0.042711f),
//				},
//				new Vector2[]
//				{
//					new Vector2(0.072281f, 0.075187f),
//					new Vector2(0.060137f, 0.03832f),
//					new Vector2(0.036492f, 0.079232f),
//					new Vector2(0.075268f, 0.927719f),
//					new Vector2(0.038172f, 0.941436f),
//					new Vector2(0.080323f, 0.96354f),
//					new Vector2(0.924813f, 0.072201f),
//					new Vector2(0.961909f, 0.058484f),
//					new Vector2(0.919758f, 0.036379f),
//					new Vector2(0.927799f, 0.924732f),
//					new Vector2(0.941516f, 0.961828f),
//					new Vector2(0.963621f, 0.919677f),
//					new Vector2(0.927799f, 0.924732f),
//					new Vector2(0.924813f, 0.072201f),
//					new Vector2(0.072281f, 0.075187f),
//					new Vector2(0.075268f, 0.927719f),
//					new Vector2(0.0001f, 0.075233f),
//					new Vector2(0.036492f, 0.079232f),
//					new Vector2(0.060137f, 0.03832f),
//					new Vector2(0.031504f, 0.011476f),
//					new Vector2(0.013197f, 0.969351f),
//					new Vector2(0.075341f, 0.999819f),
//					new Vector2(0.080323f, 0.96354f),
//					new Vector2(0.038172f, 0.941436f),
//					new Vector2(0.986883f, 0.030568f),
//					new Vector2(0.92474f, 0.0001f),
//					new Vector2(0.919758f, 0.036379f),
//					new Vector2(0.961909f, 0.058484f),
//					new Vector2(0.969432f, 0.986803f),
//					new Vector2(0.9999f, 0.924659f),
//					new Vector2(0.963621f, 0.919677f),
//					new Vector2(0.941516f, 0.961828f),
//					new Vector2(0.072281f, 0.075187f),
//					new Vector2(0.036492f, 0.079232f),
//					new Vector2(0.036419f, 0.927793f),
//					new Vector2(0.075268f, 0.927719f),
//					new Vector2(0.036492f, 0.079232f),
//					new Vector2(0.0001f, 0.075233f),
//					new Vector2(0.000168f, 0.927796f),
//					new Vector2(0.036419f, 0.927793f),
//					new Vector2(0.075268f, 0.927719f),
//					new Vector2(0.080323f, 0.96354f),
//					new Vector2(0.927871f, 0.963569f),
//					new Vector2(0.927799f, 0.924732f),
//					new Vector2(0.080323f, 0.96354f),
//					new Vector2(0.075341f, 0.999819f),
//					new Vector2(0.927871f, 0.999819f),
//					new Vector2(0.927871f, 0.963569f),
//					new Vector2(0.927799f, 0.924732f),
//					new Vector2(0.963621f, 0.919677f),
//					new Vector2(0.96365f, 0.072129f),
//					new Vector2(0.924813f, 0.072201f),
//					new Vector2(0.963621f, 0.919677f),
//					new Vector2(0.9999f, 0.924659f),
//					new Vector2(0.9999f, 0.072129f),
//					new Vector2(0.96365f, 0.072129f),
//					new Vector2(0.924813f, 0.072201f),
//					new Vector2(0.919758f, 0.036379f),
//					new Vector2(0.07221f, 0.03635f),
//					new Vector2(0.072281f, 0.075187f),
//					new Vector2(0.919758f, 0.036379f),
//					new Vector2(0.92474f, 0.0001f),
//					new Vector2(0.07221f, 0.0001f),
//					new Vector2(0.07221f, 0.03635f),
//				},
//			};
//			public static Vector3[][] Normals = new Vector3[Def.BlockTypeCount][]
//			{
//				new Vector3[]
//				{
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//				},
//				new Vector3[]
//				{
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//				},
//				new Vector3[]
//				{
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.0f, 1.0f, 0.0f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.9238766f, -0.3826903f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(0.0f, 0.3826903f, -0.9238766f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.9238766f, 0.3826903f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//					new Vector3(0.0f, 0.3826903f, 0.9238766f),
//				},
//			};
//			public static int[][] Indices = new int[Def.BlockTypeCount][]
//			{
//				new int[]
//				{
//					0, 1, 2,
//					3, 4, 5,
//					6, 7, 8,
//					9, 10, 11,
//					12, 13, 14,
//					12, 14, 15,
//					16, 17, 18,
//					16, 18, 19,
//					20, 21, 22,
//					20, 22, 23,
//					24, 25, 26,
//					24, 26, 27,
//					28, 29, 30,
//					28, 30, 31,
//					32, 33, 34,
//					32, 34, 35,
//					36, 37, 38,
//					36, 38, 39,
//					40, 41, 42,
//					40, 42, 43,
//					44, 45, 46,
//					44, 46, 47,
//					48, 49, 50,
//					48, 50, 51,
//					52, 53, 54,
//					52, 54, 55,
//					56, 57, 58,
//					56, 58, 59,
//					60, 61, 62,
//					60, 62, 63,
//				},
//				new int[]
//				{
//					0, 1, 2,
//					3, 4, 5,
//					6, 7, 8,
//					9, 10, 11,
//					12, 13, 14,
//					15, 16, 17,
//					18, 19, 20,
//					21, 22, 23,
//					24, 25, 26,
//					27, 28, 29,
//					30, 31, 32,
//					30, 32, 33,
//					34, 35, 36,
//					34, 36, 37,
//					38, 39, 40,
//					38, 40, 41,
//					42, 43, 44,
//					42, 44, 45,
//					46, 47, 48,
//					46, 48, 49,
//					50, 51, 52,
//					50, 52, 53,
//					54, 55, 56,
//					54, 56, 57,
//					58, 59, 60,
//					58, 60, 61,
//					62, 63, 64,
//					62, 64, 65,
//					66, 67, 68,
//					66, 68, 69,
//					70, 71, 72,
//					70, 72, 73,
//					74, 75, 76,
//					74, 76, 77,
//					78, 79, 80,
//					78, 80, 81,
//					82, 83, 84,
//					82, 84, 85,
//					86, 87, 88,
//					86, 88, 89,
//					90, 91, 92,
//					90, 92, 93,
//					94, 95, 96,
//					94, 96, 97,
//					98, 99, 100,
//					98, 100, 101,
//					102, 103, 104,
//					102, 104, 105,
//					106, 107, 108,
//					106, 108, 109,
//					110, 111, 112,
//					110, 112, 113,
//					114, 115, 116,
//					114, 116, 117,
//					118, 119, 120,
//					118, 120, 121,
//					122, 123, 124,
//					122, 124, 125,
//					126, 127, 128,
//					126, 128, 129,
//					130, 131, 132,
//					130, 132, 133,
//					134, 135, 136,
//					134, 136, 137,
//					138, 139, 140,
//					138, 140, 141,
//					142, 143, 144,
//					142, 144, 145,
//					146, 147, 148,
//					146, 148, 149,
//					150, 151, 152,
//					150, 152, 153,
//					154, 155, 156,
//					154, 156, 157,
//					158, 159, 160,
//					158, 160, 161,
//					162, 163, 164,
//					162, 164, 165,
//				},
//				new int[]
//				{
//					0, 1, 2,
//					3, 4, 5,
//					6, 7, 8,
//					9, 10, 11,
//					12, 13, 14,
//					12, 14, 15,
//					16, 17, 18,
//					16, 18, 19,
//					20, 21, 22,
//					20, 22, 23,
//					24, 25, 26,
//					24, 26, 27,
//					28, 29, 30,
//					28, 30, 31,
//					32, 33, 34,
//					32, 34, 35,
//					36, 37, 38,
//					36, 38, 39,
//					40, 41, 42,
//					40, 42, 43,
//					44, 45, 46,
//					44, 46, 47,
//					48, 49, 50,
//					48, 50, 51,
//					52, 53, 54,
//					52, 54, 55,
//					56, 57, 58,
//					56, 58, 59,
//					60, 61, 62,
//					60, 62, 63,
//				},
//			};
//			public static Mesh[] Meshes = new Mesh[Def.BlockTypeCount]
//			{
//				null,
//				null,
//				null,
//			};
			
//		}
//		public static class MidMesh
//		{
//			public static Vector3[][] Vertices = new Vector3[Def.BlockTypeCount - 1][]
//			{
//				new Vector3[]
//				{
//					new Vector3(-0.9f, -0.1f, 1.0f),
//					new Vector3(-0.9f, -3.4f, 0.999999f),
//					new Vector3(-0.1f, -3.4f, 0.999999f),
//					new Vector3(-0.1f, -0.1f, 1.0f),
//					new Vector3(-1.0f, -0.1f, 0.1f),
//					new Vector3(-1.0f, -3.4f, 0.099999f),
//					new Vector3(-1.0f, -3.4f, 0.899999f),
//					new Vector3(-1.0f, -0.1f, 0.9f),
//					new Vector3(0.0f, -0.1f, 0.9f),
//					new Vector3(0.0f, -3.4f, 0.899999f),
//					new Vector3(0.0f, -3.4f, 0.099999f),
//					new Vector3(0.0f, -0.1f, 0.1f),
//					new Vector3(-0.1f, -0.1f, 0.0f),
//					new Vector3(-0.1f, -3.4f, -1E-06f),
//					new Vector3(-0.9f, -3.4f, -1E-06f),
//					new Vector3(-0.9f, -0.1f, 0.0f),
//					new Vector3(0.0f, -0.1f, 0.1f),
//					new Vector3(0.0f, -3.4f, 0.099999f),
//					new Vector3(-0.1f, -3.4f, -1E-06f),
//					new Vector3(-0.1f, -0.1f, 0.0f),
//					new Vector3(-0.9f, -0.1f, 0.0f),
//					new Vector3(-0.9f, -3.4f, -1E-06f),
//					new Vector3(-1.0f, -3.4f, 0.099999f),
//					new Vector3(-1.0f, -0.1f, 0.1f),
//					new Vector3(-1.0f, -0.1f, 0.9f),
//					new Vector3(-1.0f, -3.4f, 0.899999f),
//					new Vector3(-0.9f, -3.4f, 0.999999f),
//					new Vector3(-0.9f, -0.1f, 1.0f),
//					new Vector3(0.0f, -3.4f, 0.899999f),
//					new Vector3(0.0f, -0.1f, 0.9f),
//					new Vector3(-0.1f, -0.1f, 1.0f),
//					new Vector3(-0.1f, -3.4f, 0.999999f),
//				},
//				new Vector3[]
//				{
//					new Vector3(-1.9f, -0.1f, 2.0f),
//					new Vector3(-1.9f, -2.5f, 2.0f),
//					new Vector3(-0.1f, -2.5f, 2.0f),
//					new Vector3(-0.1f, -0.1f, 2.0f),
//					new Vector3(-2.0f, -0.1f, 1.9f),
//					new Vector3(-2.0f, -0.1f, 0.1f),
//					new Vector3(-2.0f, -2.5f, 0.1f),
//					new Vector3(-2.0f, -2.5f, 1.9f),
//					new Vector3(0.0f, -0.1f, 0.1f),
//					new Vector3(0.0f, -0.1f, 1.9f),
//					new Vector3(0.0f, -2.5f, 1.9f),
//					new Vector3(0.0f, -2.5f, 0.1f),
//					new Vector3(-0.1f, -0.1f, 0.0f),
//					new Vector3(-0.1f, -2.5f, 0.0f),
//					new Vector3(-1.9f, -2.5f, 0.0f),
//					new Vector3(-1.9f, -0.1f, 0.0f),
//					new Vector3(0.0f, -0.1f, 0.1f),
//					new Vector3(0.0f, -2.5f, 0.1f),
//					new Vector3(-0.1f, -2.5f, 0.0f),
//					new Vector3(-0.1f, -0.1f, 0.0f),
//					new Vector3(-1.9f, -0.1f, 0.0f),
//					new Vector3(-1.9f, -2.5f, 0.0f),
//					new Vector3(-2.0f, -2.5f, 0.1f),
//					new Vector3(-2.0f, -0.1f, 0.1f),
//					new Vector3(-2.0f, -0.1f, 1.9f),
//					new Vector3(-2.0f, -2.5f, 1.9f),
//					new Vector3(-1.9f, -2.5f, 2.0f),
//					new Vector3(-1.9f, -0.1f, 2.0f),
//					new Vector3(0.0f, -2.5f, 1.9f),
//					new Vector3(0.0f, -0.1f, 1.9f),
//					new Vector3(-0.1f, -0.1f, 2.0f),
//					new Vector3(-0.1f, -2.5f, 2.0f),
//				},
//			};
//			public static Vector2[][] UVs = new Vector2[Def.BlockTypeCount - 1][]
//			{
//				new Vector2[]
//				{
//					new Vector2(0.712465f, 0.896312f),
//					new Vector2(0.712465f, 0.020448f),
//					new Vector2(0.500134f, 0.020448f),
//					new Vector2(0.500134f, 0.896312f),
//					new Vector2(0.962331f, 0.896312f),
//					new Vector2(0.962331f, 0.020448f),
//					new Vector2(0.75f, 0.020448f),
//					new Vector2(0.75f, 0.896312f),
//					new Vector2(0.462331f, 0.896312f),
//					new Vector2(0.46233f, 0.020448f),
//					new Vector2(0.25f, 0.020448f),
//					new Vector2(0.250001f, 0.896312f),
//					new Vector2(0.212465f, 0.896312f),
//					new Vector2(0.212464f, 0.020448f),
//					new Vector2(0.000134f, 0.020449f),
//					new Vector2(0.000135f, 0.896313f),
//					new Vector2(0.250001f, 0.896312f),
//					new Vector2(0.25f, 0.020448f),
//					new Vector2(0.212464f, 0.020448f),
//					new Vector2(0.212465f, 0.896312f),
//					new Vector2(0.999866f, 0.896312f),
//					new Vector2(0.999866f, 0.020448f),
//					new Vector2(0.962331f, 0.020448f),
//					new Vector2(0.962331f, 0.896312f),
//					new Vector2(0.75f, 0.896312f),
//					new Vector2(0.75f, 0.020448f),
//					new Vector2(0.712465f, 0.020448f),
//					new Vector2(0.712465f, 0.896312f),
//					new Vector2(0.46233f, 0.020448f),
//					new Vector2(0.462331f, 0.896312f),
//					new Vector2(0.499866f, 0.896312f),
//					new Vector2(0.499865f, 0.020448f),
//				},
//				new Vector2[]
//				{
//					new Vector2(0.363354f, 0.499875f),
//					new Vector2(0.363354f, 0.01557f),
//					new Vector2(0.000125f, 0.01557f),
//					new Vector2(0.000125f, 0.499875f),
//					new Vector2(0.391891f, 0.499875f),
//					new Vector2(0.75512f, 0.499875f),
//					new Vector2(0.75512f, 0.01557f),
//					new Vector2(0.391891f, 0.01557f),
//					new Vector2(0.391891f, 0.999875f),
//					new Vector2(0.75512f, 0.999875f),
//					new Vector2(0.75512f, 0.51557f),
//					new Vector2(0.391891f, 0.51557f),
//					new Vector2(0.363354f, 0.999875f),
//					new Vector2(0.363353f, 0.51557f),
//					new Vector2(0.000125f, 0.51557f),
//					new Vector2(0.000125f, 0.999875f),
//					new Vector2(0.391891f, 0.999875f),
//					new Vector2(0.391891f, 0.51557f),
//					new Vector2(0.363353f, 0.51557f),
//					new Vector2(0.363354f, 0.999875f),
//					new Vector2(0.783658f, 0.499875f),
//					new Vector2(0.783658f, 0.01557f),
//					new Vector2(0.75512f, 0.01557f),
//					new Vector2(0.75512f, 0.499875f),
//					new Vector2(0.391891f, 0.499875f),
//					new Vector2(0.391891f, 0.01557f),
//					new Vector2(0.363354f, 0.01557f),
//					new Vector2(0.363354f, 0.499875f),
//					new Vector2(0.75512f, 0.51557f),
//					new Vector2(0.75512f, 0.999875f),
//					new Vector2(0.783658f, 0.999875f),
//					new Vector2(0.783658f, 0.51557f),
//				},
//			};
//			public static Vector3[][] Normals = new Vector3[Def.BlockTypeCount - 1][]
//			{
//				new Vector3[]
//				{
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//				},
//				new Vector3[]
//				{
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(0.0f, 0.0f, 1.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(-1.0f, 0.0f, 0.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(1.0f, 0.0f, 0.0f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.0f, 0.0f, -1.0f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, -0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(-0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//					new Vector3(0.7071068f, 0.0f, 0.7071068f),
//				},
//			};
//			public static int[][] Indices = new int[Def.BlockTypeCount - 1][]
//			{
//				new int[]
//				{
//					0, 1, 2,
//					0, 2, 3,
//					4, 5, 6,
//					4, 6, 7,
//					8, 9, 10,
//					8, 10, 11,
//					12, 13, 14,
//					12, 14, 15,
//					16, 17, 18,
//					16, 18, 19,
//					20, 21, 22,
//					20, 22, 23,
//					24, 25, 26,
//					24, 26, 27,
//					28, 29, 30,
//					28, 30, 31,
//				},
//				new int[]
//				{
//					0, 1, 2,
//					0, 2, 3,
//					4, 5, 6,
//					4, 6, 7,
//					8, 9, 10,
//					8, 10, 11,
//					12, 13, 14,
//					12, 14, 15,
//					16, 17, 18,
//					16, 18, 19,
//					20, 21, 22,
//					20, 22, 23,
//					24, 25, 26,
//					24, 26, 27,
//					28, 29, 30,
//					28, 30, 31,
//				},
//			};
//			public static Vector2[] VertexHeight = new Vector2[Def.BlockTypeCount - 1]
//			{
//				new Vector2(-0.1f, -3.4f),
//				new Vector2(-0.1f, -2.5f)
//			};
//			public static Vector2 UVHeightNormal = new Vector2(0.02044f, 0.89631f);
//			public static Vector4 UVHeightWide = new Vector4(0.01557f, 0.499875f, 0.51557f, 0.999875f);
//			public static Mesh[] Meshes = new Mesh[Def.BlockTypeCount - 1]
//			{
//				null,
//				null,
//			};
//		}
//		public static class BottomMesh
//		{
//			public static Vector3[][] Vertices = new Vector3[Def.BlockTypeCount - 1][]
//			{
//				new Vector3[]
//				{
//					new Vector3(-0.1f, -3.470711f, 0.029288f),
//					new Vector3(-0.1f, -3.4f, -1E-06f),
//					new Vector3(0.0f, -3.4f, 0.099999f),
//					new Vector3(-0.029289f, -3.470711f, 0.099999f),
//					new Vector3(-0.9f, -3.4f, 0.999999f),
//					new Vector3(-1.0f, -3.4f, 0.899999f),
//					new Vector3(-0.970711f, -3.470711f, 0.899999f),
//					new Vector3(-0.9f, -3.470711f, 0.97071f),
//					new Vector3(-0.970711f, -3.470711f, 0.099999f),
//					new Vector3(-1.0f, -3.4f, 0.099999f),
//					new Vector3(-0.9f, -3.4f, -1E-06f),
//					new Vector3(-0.9f, -3.470711f, 0.029288f),
//					new Vector3(-0.029289f, -3.470711f, 0.899999f),
//					new Vector3(-0.029289f, -3.470711f, 0.099999f),
//					new Vector3(0.0f, -3.4f, 0.099999f),
//					new Vector3(0.0f, -3.4f, 0.899999f),
//					new Vector3(-0.1f, -3.470711f, 0.029288f),
//					new Vector3(-0.9f, -3.470711f, 0.029288f),
//					new Vector3(-0.9f, -3.4f, -1E-06f),
//					new Vector3(-0.1f, -3.4f, -1E-06f),
//					new Vector3(-0.970711f, -3.470711f, 0.099999f),
//					new Vector3(-0.970711f, -3.470711f, 0.899999f),
//					new Vector3(-1.0f, -3.4f, 0.899999f),
//					new Vector3(-1.0f, -3.4f, 0.099999f),
//					new Vector3(-0.9f, -3.470711f, 0.97071f),
//					new Vector3(-0.1f, -3.470711f, 0.97071f),
//					new Vector3(-0.1f, -3.4f, 0.999999f),
//					new Vector3(-0.9f, -3.4f, 0.999999f),
//					new Vector3(0.0f, -3.4f, 0.899999f),
//					new Vector3(-0.1f, -3.4f, 0.999999f),
//					new Vector3(-0.1f, -3.470711f, 0.97071f),
//					new Vector3(-0.029289f, -3.470711f, 0.899999f),
//				},
//				new Vector3[]
//				{
//					new Vector3(-0.1f, -2.570711f, 0.029289f),
//					new Vector3(-0.1f, -2.5f, 0.0f),
//					new Vector3(0.0f, -2.5f, 0.1f),
//					new Vector3(-0.029289f, -2.570711f, 0.1f),
//					new Vector3(-1.9f, -2.570711f, 1.970711f),
//					new Vector3(-1.9f, -2.5f, 2.0f),
//					new Vector3(-2.0f, -2.5f, 1.9f),
//					new Vector3(-1.970711f, -2.570711f, 1.899999f),
//					new Vector3(-2.0f, -2.5f, 0.1f),
//					new Vector3(-1.9f, -2.5f, 0.0f),
//					new Vector3(-1.9f, -2.570711f, 0.029289f),
//					new Vector3(-1.970711f, -2.570711f, 0.1f),
//					new Vector3(-0.029289f, -2.570711f, 1.899999f),
//					new Vector3(-0.029289f, -2.570711f, 0.1f),
//					new Vector3(0.0f, -2.5f, 0.1f),
//					new Vector3(0.0f, -2.5f, 1.9f),
//					new Vector3(-0.1f, -2.570711f, 0.029289f),
//					new Vector3(-1.9f, -2.570711f, 0.029289f),
//					new Vector3(-1.9f, -2.5f, 0.0f),
//					new Vector3(-0.1f, -2.5f, 0.0f),
//					new Vector3(-1.970711f, -2.570711f, 0.1f),
//					new Vector3(-1.970711f, -2.570711f, 1.899999f),
//					new Vector3(-2.0f, -2.5f, 1.9f),
//					new Vector3(-2.0f, -2.5f, 0.1f),
//					new Vector3(-1.9f, -2.570711f, 1.970711f),
//					new Vector3(-0.1f, -2.570711f, 1.970711f),
//					new Vector3(-0.1f, -2.5f, 2.0f),
//					new Vector3(-1.9f, -2.5f, 2.0f),
//					new Vector3(-0.029289f, -2.570711f, 1.899999f),
//					new Vector3(0.0f, -2.5f, 1.9f),
//					new Vector3(-0.1f, -2.5f, 2.0f),
//					new Vector3(-0.1f, -2.570711f, 1.970711f),
//				},
//			};
//			public static Vector2[][] UVs = new Vector2[Def.BlockTypeCount - 1][]
//			{
//				new Vector2[]
//				{
//					new Vector2(0.217961f, 0.000892f),
//					new Vector2(0.212464f, 0.020448f),
//					new Vector2(0.25f, 0.020448f),
//					new Vector2(0.244503f, 0.000892f),
//					new Vector2(0.712465f, 0.020448f),
//					new Vector2(0.75f, 0.020448f),
//					new Vector2(0.744503f, 0.000892f),
//					new Vector2(0.717962f, 0.000892f),
//					new Vector2(0.967828f, 0.000892f),
//					new Vector2(0.962331f, 0.020448f),
//					new Vector2(0.999866f, 0.020448f),
//					new Vector2(0.994369f, 0.000892f),
//					new Vector2(0.46233f, 0.000134f),
//					new Vector2(0.25f, 0.000134f),
//					new Vector2(0.25f, 0.020448f),
//					new Vector2(0.46233f, 0.020448f),
//					new Vector2(0.212464f, 0.000134f),
//					new Vector2(0.000134f, 0.000135f),
//					new Vector2(0.000134f, 0.020449f),
//					new Vector2(0.212464f, 0.020448f),
//					new Vector2(0.962331f, 0.000134f),
//					new Vector2(0.75f, 0.000134f),
//					new Vector2(0.75f, 0.020448f),
//					new Vector2(0.962331f, 0.020448f),
//					new Vector2(0.712465f, 0.000134f),
//					new Vector2(0.500134f, 0.000134f),
//					new Vector2(0.500134f, 0.020448f),
//					new Vector2(0.712465f, 0.020448f),
//					new Vector2(0.46233f, 0.020448f),
//					new Vector2(0.499865f, 0.020448f),
//					new Vector2(0.494368f, 0.000892f),
//					new Vector2(0.467827f, 0.000892f),
//				},
//				new Vector2[]
//				{
//					new Vector2(0.367533f, 0.500702f),
//					new Vector2(0.363353f, 0.51557f),
//					new Vector2(0.391891f, 0.51557f),
//					new Vector2(0.387712f, 0.500702f),
//					new Vector2(0.367533f, 0.000702f),
//					new Vector2(0.363354f, 0.01557f),
//					new Vector2(0.391891f, 0.01557f),
//					new Vector2(0.387712f, 0.000702f),
//					new Vector2(0.75512f, 0.01557f),
//					new Vector2(0.783658f, 0.01557f),
//					new Vector2(0.779478f, 0.000702f),
//					new Vector2(0.759299f, 0.000702f),
//					new Vector2(0.75512f, 0.500126f),
//					new Vector2(0.391891f, 0.500125f),
//					new Vector2(0.391891f, 0.51557f),
//					new Vector2(0.75512f, 0.51557f),
//					new Vector2(0.363353f, 0.500126f),
//					new Vector2(0.000125f, 0.500125f),
//					new Vector2(0.000125f, 0.51557f),
//					new Vector2(0.363353f, 0.51557f),
//					new Vector2(0.75512f, 0.000126f),
//					new Vector2(0.391892f, 0.000125f),
//					new Vector2(0.391891f, 0.01557f),
//					new Vector2(0.75512f, 0.01557f),
//					new Vector2(0.363354f, 0.000126f),
//					new Vector2(0.000125f, 0.000125f),
//					new Vector2(0.000125f, 0.01557f),
//					new Vector2(0.363354f, 0.01557f),
//					new Vector2(0.759299f, 0.500702f),
//					new Vector2(0.75512f, 0.51557f),
//					new Vector2(0.783658f, 0.51557f),
//					new Vector2(0.779478f, 0.500702f),
//				},
//			};
//			public static Vector3[][] Normals = new Vector3[Def.BlockTypeCount - 1][]
//			{
//				new Vector3[]
//				{
//					new Vector3(0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(0.0f, -0.3826903f, -0.9238766f),
//					new Vector3(0.0f, -0.3826903f, -0.9238766f),
//					new Vector3(0.0f, -0.3826903f, -0.9238766f),
//					new Vector3(0.0f, -0.3826903f, -0.9238766f),
//					new Vector3(-0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(0.0f, -0.3826903f, 0.9238766f),
//					new Vector3(0.0f, -0.3826903f, 0.9238766f),
//					new Vector3(0.0f, -0.3826903f, 0.9238766f),
//					new Vector3(0.0f, -0.3826903f, 0.9238766f),
//					new Vector3(0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, 0.6785955f),
//				},
//				new Vector3[]
//				{
//					new Vector3(0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(-0.6785955f, -0.2810982f, -0.6785955f),
//					new Vector3(0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(0.0f, -0.3826903f, -0.9238766f),
//					new Vector3(0.0f, -0.3826903f, -0.9238766f),
//					new Vector3(0.0f, -0.3826903f, -0.9238766f),
//					new Vector3(0.0f, -0.3826903f, -0.9238766f),
//					new Vector3(-0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(-0.9238766f, -0.3826903f, 0.0f),
//					new Vector3(0.0f, -0.3826903f, 0.9238766f),
//					new Vector3(0.0f, -0.3826903f, 0.9238766f),
//					new Vector3(0.0f, -0.3826903f, 0.9238766f),
//					new Vector3(0.0f, -0.3826903f, 0.9238766f),
//					new Vector3(0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, 0.6785955f),
//					new Vector3(0.6785955f, -0.2810982f, 0.6785955f),
//				},
//			};
//			public static int[][] Indices = new int[Def.BlockTypeCount - 1][]
//			{
//				new int[]
//				{
//					0, 1, 2,
//					0, 2, 3,
//					4, 5, 6,
//					4, 6, 7,
//					8, 9, 10,
//					8, 10, 11,
//					12, 13, 14,
//					12, 14, 15,
//					16, 17, 18,
//					16, 18, 19,
//					20, 21, 22,
//					20, 22, 23,
//					24, 25, 26,
//					24, 26, 27,
//					28, 29, 30,
//					28, 30, 31,
//				},
//				new int[]
//				{
//					0, 1, 2,
//					0, 2, 3,
//					4, 5, 6,
//					4, 6, 7,
//					8, 9, 10,
//					8, 10, 11,
//					12, 13, 14,
//					12, 14, 15,
//					16, 17, 18,
//					16, 18, 19,
//					20, 21, 22,
//					20, 22, 23,
//					24, 25, 26,
//					24, 26, 27,
//					28, 29, 30,
//					28, 30, 31,
//				},
//			};
//			public static Mesh[] Meshes = new Mesh[Def.BlockTypeCount - 1]
//			{
//				null,
//				null,
//			};
//		}

//		public static Dictionary<float, Mesh>[] MidMeshes = new Dictionary<float, Mesh>[Def.BlockTypeCount]
//		{
//			new Dictionary<float, Mesh>(),
//			new Dictionary<float, Mesh>(),
//			new Dictionary<float, Mesh>()
//		};

//		//public static Dictionary<int, Dictionary<float, Mesh>>[] CachedMaterialMeshes = new Dictionary<int, Dictionary<float, Mesh>>[(int)BlockType.COUNT]
//		//{
//		//    new Dictionary<int, Dictionary<float, Mesh>>(),
//		//    new Dictionary<int, Dictionary<float, Mesh>>(),
//		//    new Dictionary<int, Dictionary<float, Mesh>>()
//		//};

//		public const float MinHeight = -0.4f;

//		static void InitDefaultMeshes(int type)
//		{
//			// Init top mesh
//			{
//				var mesh = new Mesh();
//				mesh.vertices = TopMesh.Vertices[type];
//				mesh.uv = TopMesh.UVs[type];
//				mesh.normals = TopMesh.Normals[type];
//				mesh.triangles = TopMesh.Indices[type];
//				//mesh.RecalculateTangents();
//				//mesh.Optimize();
//				mesh.Optimize();
//				mesh.RecalculateBounds();
//				mesh.UploadMeshData(false);
//				mesh.name = "TOP" + ((type == (int)Def.BlockType.NORMAL) ? " NORMAL" : (type == (int)Def.BlockType.WIDE) ? " WIDE" : (type == (int)Def.BlockType.WIDE) ? " STAIR" : "");

//				TopMesh.Meshes[type] = mesh;
//			}
//			if (type == (int)Def.BlockType.WIDE)
//				return;
//			// Init Mid mesh
//			{
//				var mesh = new Mesh();
//				mesh.vertices = MidMesh.Vertices[type];
//				mesh.uv = MidMesh.UVs[type];
//				mesh.normals = MidMesh.Normals[type];
//				mesh.triangles = MidMesh.Indices[type];
//				//mesh.RecalculateTangents();
//				//mesh.Optimize();
//				mesh.RecalculateBounds();
//				MidMesh.Meshes[type] = mesh;
//			}
//			// Init Bottom mesh
//			{
//				var mesh = new Mesh();
//				mesh.vertices = BottomMesh.Vertices[type];
//				mesh.uv = BottomMesh.UVs[type];
//				mesh.normals = BottomMesh.Normals[type];
//				mesh.triangles = BottomMesh.Indices[type];
//				//mesh.RecalculateTangents();
//				//mesh.Optimize();
//				mesh.RecalculateBounds();
//				BottomMesh.Meshes[type] = mesh;
//			}
//		}
		
//		static void CopyMesh(Mesh from, ref Mesh to)
//		{
//			var vertices = new Vector3[from.vertices.Length];
//			from.vertices.CopyTo(vertices, 0);
//			to.vertices = vertices;

//			var uvs = new Vector2[from.uv.Length];
//			from.uv.CopyTo(uvs, 0);
//			to.uv = uvs;

//			var normals = new Vector3[from.normals.Length];
//			from.normals.CopyTo(normals, 0);
//			to.normals = normals;

//			var tangents = new Vector4[from.tangents.Length];
//			from.tangents.CopyTo(tangents, 0);
//			to.tangents = tangents;

//			var triangles = new int[from.triangles.Length];
//			from.triangles.CopyTo(triangles, 0);
//			to.triangles = triangles;
//		}

//		static void InitMidMeshes(int type)
//		{
//			var name = type == 0 ? "Normal" : type == 1 ? "Wide" : "";

//			var vertexHeight = MidMesh.VertexHeight[type];
//			float vertexMaxHeight = vertexHeight.y;
//			float currentVertexHeight = vertexMaxHeight;
//			Mesh midDefaultMesh = MidMesh.Meshes[type];
//			Mesh botDefaultMesh = BottomMesh.Meshes[type];

//			var firstMesh = new Mesh()
//			{
//				name = name + $"_{Mathf.Abs(vertexMaxHeight)}",
//			};
//			var combines = new CombineInstance[2] // Mid and Bottom
//			{
//				new CombineInstance()
//				{
//					mesh = midDefaultMesh,
//					subMeshIndex = 0
//				},
//				new CombineInstance()
//				{
//					mesh = botDefaultMesh,
//					subMeshIndex = 0
//				}
//			};
//			firstMesh.CombineMeshes(combines, true, false, false);
//			firstMesh.Optimize();
//			firstMesh.RecalculateBounds();
//			firstMesh.UploadMeshData(false);

//			// Normal and Stair
//			if (type == 0)
//			{
//				MidMeshes[(int)Def.BlockType.NORMAL].Add(vertexMaxHeight, firstMesh);
//				MidMeshes[(int)Def.BlockType.STAIRS].Add(vertexMaxHeight, firstMesh);

//				int lowPart = Mathf.CeilToInt(currentVertexHeight);
//				currentVertexHeight = lowPart;

//				while (currentVertexHeight <= -0.5f)
//				{
//					var currentMesh = new Mesh()
//					{
//						name = name + $"_{Mathf.Abs(currentVertexHeight)}"
//					};
//					var midMesh = new Mesh();
//					var botMesh = new Mesh();
//					CopyMesh(midDefaultMesh, ref midMesh);
//					CopyMesh(botDefaultMesh, ref botMesh);

//					var uvs = midMesh.uv;
//					var lengthPct = 1 - ((Mathf.Abs(currentVertexHeight) - Mathf.Abs(vertexHeight.x)) / (Mathf.Abs(vertexHeight.y) - Mathf.Abs(vertexHeight.x)));

//					var uvHeight = MidMesh.UVHeightNormal;
//					var uvLength = lengthPct * (uvHeight.y - uvHeight.x) + uvHeight.x;
//					//if (uvLength < uvHeight.x)
//					//    uvLength = uvHeight.x;
//					var uvys = new List<UVY>()
//					{
//						new UVY()
//						{
//							Original = uvHeight.x,
//							ToChange = uvLength
//						}
//					};
//					//List<UVY> uvys = new List<UVY>(1);
//					//var uvy = new UVY();
//					//uvy.Original = uvHeight.x;
//					//uvy.ToChange = uvLength;
//					//uvys.Add(uvy);
//					UVYResize(ref uvs, uvys);
//					var midVertices = midMesh.vertices;
//					var botVertices = botMesh.vertices;

//					for (int i = 0; i < midVertices.Length; ++i)
//					{
//						if (midVertices[i].y != vertexHeight.x)
//						{
//							var vertex = midMesh.vertices[i];
//							vertex.y = currentVertexHeight;
//							midVertices[i] = vertex;
//						}
//					}

//					for(int i = 0; i < botVertices.Length; ++i)
//					{
//						var vertex = botVertices[i];
//						vertex.y += (currentVertexHeight - vertexMaxHeight);
//						botVertices[i] = vertex;
//					}

//					botMesh.vertices = botVertices;
//					midMesh.vertices = midVertices;
//					midMesh.uv = uvs;

//					combines = new CombineInstance[2]; // Mid and Bottom
//					combines[0].mesh = midMesh;
//					combines[0].subMeshIndex = 0;
//					combines[1].mesh = botMesh;
//					combines[1].subMeshIndex = 0;

//					currentMesh.CombineMeshes(combines, true, false, false);
//					UnityEngine.Object.Destroy(midMesh);
//					UnityEngine.Object.Destroy(botMesh);

//					currentMesh.Optimize();
//					currentMesh.RecalculateBounds();
//					currentMesh.UploadMeshData(false);
//					MidMeshes[(int)Def.BlockType.NORMAL].Add(currentVertexHeight, currentMesh);
//					MidMeshes[(int)Def.BlockType.STAIRS].Add(currentVertexHeight, currentMesh);

//					currentVertexHeight += 0.5f;
//				}
//			}
//			// WIDE
//			else if(type == 1)
//			{
//				MidMeshes[(int)Def.BlockType.WIDE].Add(vertexMaxHeight, firstMesh);
//				currentVertexHeight += 0.5f;

//				while (currentVertexHeight <= -0.5f)
//				{
//					var currentMesh = new Mesh()
//					{
//						name = name + $"_{Mathf.Abs(currentVertexHeight)}"
//					};
//					var midMesh = new Mesh();
//					var botMesh = new Mesh();
//					CopyMesh(midDefaultMesh, ref midMesh);
//					CopyMesh(botDefaultMesh, ref botMesh);
					
//					var uvs = midMesh.uv;
//					var lengthPct = 1 - ((Mathf.Abs(currentVertexHeight) - Mathf.Abs(vertexHeight.x)) / (Mathf.Abs(vertexHeight.y) - Mathf.Abs(vertexHeight.x)));

//					var uvHeight = MidMesh.UVHeightWide; // Vector4(0.01557f, 0.499875f, 0.51557f, 0.999875f)

//					var uvLength0 = lengthPct * (uvHeight.y - uvHeight.x) + uvHeight.x;
//					var uvLength1 = lengthPct * (uvHeight.w - uvHeight.z) + uvHeight.z;
//					//if (uvLength0 < uvHeight.x)
//					//    uvLength0 = uvHeight.x;
//					//if (uvLength1 < uvHeight.z)
//					//    uvLength1 = uvHeight.z;

//					var uvys = new List<UVY>()
//					{
//						new UVY()
//						{
//							Original = uvHeight.x,
//							ToChange = uvLength0
//						},
//						new UVY()
//						{
//							Original = uvHeight.z,
//							ToChange = uvLength1
//						}
//					};
//					UVYResize(ref uvs, uvys);

//					var midVertices = midMesh.vertices;
//					var botVertices = botMesh.vertices;

//					for (int i = 0; i < midVertices.Length; ++i)
//					{
//						if (midVertices[i].y != vertexHeight.x)
//						{
//							var vertex = midMesh.vertices[i];
//							vertex.y = currentVertexHeight;
//							midVertices[i] = vertex;
//						}
//					}

//					for (int i = 0; i < botVertices.Length; ++i)
//					{
//						var vertex = botVertices[i];
//						vertex.y += (currentVertexHeight - vertexMaxHeight);
//						botVertices[i] = vertex;
//					}

//					botMesh.vertices = botVertices;
//					midMesh.vertices = midVertices;
//					midMesh.uv = uvs;

//					combines = new CombineInstance[2]; // Mid and Bottom
//					combines[0].mesh = midMesh;
//					combines[0].subMeshIndex = 0;
//					//combines[0].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
//					combines[1].mesh = botMesh;
//					combines[1].subMeshIndex = 0;
//					//combines[1].transform = Matrix4x4.TRS(new Vector3(0.0f, currentVertexHeight - vertexMaxHeight, 0.0f), Quaternion.identity, Vector3.one);

//					currentMesh.CombineMeshes(combines, true, false, false);
//					UnityEngine.Object.Destroy(midMesh);
//					UnityEngine.Object.Destroy(botMesh);

//					currentMesh.Optimize();
//					currentMesh.RecalculateBounds();
//					currentMesh.UploadMeshData(false);
//					MidMeshes[(int)Def.BlockType.WIDE].Add(currentVertexHeight, currentMesh);

//					currentVertexHeight += 0.5f;
//				}
//			}
//		}

//		public static void InitBlocks()
//		{
//			for (int i = 0; i < Def.BlockTypeCount; ++i)
//				InitDefaultMeshes(i);

//			for (int i = (int)Def.BlockType.STAIRS; i < Def.BlockTypeCount; ++i)
//				InitMidMeshes(i - 1);
//		}

//		struct UVY
//		{
//			public float Original;
//			public float ToChange;
//		};

//		static void UVYResize(ref Vector2[] uvs, List<UVY> uvy)
//		{
//			for(int i = 0; i < uvs.Length; ++i)
//			{
//				for(int j = 0; j < uvy.Count; ++j)
//				{
//					if(GameUtils.IsNearlyEqual(uvs[i].y, uvy[j].Original))
//					{
//						uvs[i].y = uvy[j].ToChange;
//						break;
//					}
//				}
//			}
//		}

//		public static void SetBlock(GameObject block, Def.BlockType type, Def.BlockMeshType meshType, float blockLength = 0.0f)
//		{
//			MeshRenderer meshRenderer;
//			MeshFilter meshFilter;
//			meshRenderer = block.GetComponent<MeshRenderer>();
//			meshFilter = block.GetComponent<MeshFilter>();
//			if (meshRenderer == null)
//				meshRenderer = block.AddComponent<MeshRenderer>();
//			if (meshFilter == null)
//				meshFilter = block.AddComponent<MeshFilter>();

//			switch(meshType)
//			{
//				case Def.BlockMeshType.TOP:
//					meshFilter.mesh = TopMesh.Meshes[(int)type];
//					break;
//				case Def.BlockMeshType.MID:
//					int idx = type == Def.BlockType.WIDE ? 1 : 0;
//					float length = blockLength == 0.0f ? MidMesh.VertexHeight[idx].y : blockLength;
//					if (length > 0.0f)
//						length = -length;
//					meshFilter.mesh = MidMeshes[(int)type][length];
//					break;
//				default:
//					break;
//			}

//			{
//				//switch(meshType)
//				//{
//				//    case Def.BlockMeshType.TOP:
//				//        meshFilter.mesh = TopMesh.Meshes[(int)type];
//				//        break;
//				//    case Def.BlockMeshType.MID:
//				//        {
//				//            int idx = type == BlockType.WIDE ? 1 : 0;
//				//            var vHeight = MidMesh.VertexHeight[idx];
//				//            if (blockLength == 0.0f)
//				//            {
//				//                blockLength = vHeight.y;
//				//            }
//				//            float length = Mathf.Clamp(Mathf.Abs(blockLength), Mathf.Abs(vHeight.x), Mathf.Abs(vHeight.y));
//				//            var mesh = new Mesh();
//				//            //MidMesh.Vertices.CopyTo(mesh.vertices, 0);
//				//            //MidMesh.UVs.CopyTo(mesh.uv, 0);
//				//            //MidMesh.Normals.CopyTo(mesh.normals, 0);
//				//            //MidMesh.Indices.CopyTo(mesh.triangles, 0);
//				//            mesh.vertices = MidMesh.Vertices[idx];
//				//            mesh.uv = MidMesh.UVs[idx];
//				//            mesh.normals = MidMesh.Normals[idx];
//				//            mesh.triangles = MidMesh.Indices[idx];
//				//
//				//            if (length != -vHeight.y)
//				//            {
//				//                var vertices = mesh.vertices;
//				//                var uvs = mesh.uv;
//				//                var lengthPct = 1 - ((length - Mathf.Abs(vHeight.x)) / (Mathf.Abs(vHeight.y) - Mathf.Abs(vHeight.x)));
//				//                if (type == BlockType.WIDE)
//				//                {
//				//                    var uvHeight = MidMesh.UVHeightWide;
//				//                    var uvLength0 = lengthPct * uvHeight.y;
//				//                    if (uvLength0 < uvHeight.x)
//				//                        uvLength0 = uvHeight.x;
//				//
//				//                    var uvLength1 = lengthPct * uvHeight.w;
//				//                    if (uvLength1 < uvHeight.z)
//				//                        uvLength1 = uvHeight.z;
//				//
//				//                    List<UVY> uvys = new List<UVY>(2);
//				//                    var uvy = new UVY();
//				//                    uvy.Original = uvHeight.x;
//				//                    uvy.ToChange = uvLength0;
//				//                    uvys.Add(uvy);
//				//                    uvy.Original = uvHeight.z;
//				//                    uvy.ToChange = uvLength1;
//				//                    uvys.Add(uvy);
//				//                    UVYResize(ref uvs, uvys);
//				//                }
//				//                else
//				//                {
//				//                    var uvHeight = MidMesh.UVHeightNormal;
//				//                    var uvLength = lengthPct * uvHeight.y;
//				//                    if (uvLength < uvHeight.x)
//				//                        uvLength = uvHeight.x;
//				//                    List<UVY> uvys = new List<UVY>(1);
//				//                    var uvy = new UVY();
//				//                    uvy.Original = uvHeight.x;
//				//                    uvy.ToChange = uvLength;
//				//                    uvys.Add(uvy);
//				//                    UVYResize(ref uvs, uvys);
//				//                }
//				//                for (int i = 0; i < vertices.Length; ++i)
//				//                {
//				//                    if (vertices[i].y != vHeight.x)
//				//                    {
//				//                        var vertex = mesh.vertices[i];
//				//                        vertex.y = -length;
//				//                        vertices[i] = vertex;
//				//                    }
//				//                    //if(GameUtils.IsNearlyEqual(uvs[i].y, uvHeight.x))
//				//                    //{
//				//                    //    var uv = mesh.uv[i];
//				//                    //    uv.y = uvLength;
//				//                    //    uvs[i] = uv;
//				//                    //}                                
//				//                }
//				//                mesh.vertices = vertices;
//				//                mesh.uv = uvs;
//				//            }
//				//            mesh.RecalculateTangents();
//				//            mesh.Optimize();
//				//            mesh.RecalculateBounds();
//				//            meshFilter.mesh = mesh;
//				//        }
//				//        break;
//				//    case Def.BlockMeshType.BOTTOM:
//				//        {
//				//            meshFilter.mesh = BottomMesh.Meshes[type == BlockType.WIDE ? 1 : 0];
//				//        }
//				//        break;
//				//    default:
//				//        break;
//				//}
//			}
//			meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
//			meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
//			meshRenderer.receiveShadows = false;
//			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
//			meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
//		}
//	}
//}
