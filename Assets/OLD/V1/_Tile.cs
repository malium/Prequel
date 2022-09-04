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
//    public enum TileType
//    {
//        NORMAL,
//        WIDE,
//        STAIRS,

//        COUNT
//    }

//    public class Tile
//    {
//        public static string TileTag = "TILE";
//        public static Shader TileShader = null;
//        private static Vector3[][] TileVerties = new Vector3[(int)TileType.COUNT][]
//        {
//            new Vector3[]
//            {
//                new Vector3(-0.1f, 0.0f, 0.9f),
//                new Vector3(-0.1f, -0.029289f, 0.970711f),
//                new Vector3(-0.029289f, -0.029289f, 0.9f),
//                new Vector3(-0.1f, 0.0f, 0.1f),
//                new Vector3(-0.029289f, -0.029289f, 0.1f),
//                new Vector3(-0.1f, -0.029289f, 0.029289f),
//                new Vector3(-0.9f, 0.0f, 0.9f),
//                new Vector3(-0.970711f, -0.029289f, 0.9f),
//                new Vector3(-0.9f, -0.029289f, 0.970711f),
//                new Vector3(-0.9f, 0.0f, 0.1f),
//                new Vector3(-0.9f, -0.029289f, 0.029289f),
//                new Vector3(-0.970711f, -0.029289f, 0.1f),
//                new Vector3(-0.1f, 0.0f, 0.1f),
//                new Vector3(-0.9f, 0.0f, 0.1f),
//                new Vector3(-0.9f, 0.0f, 0.9f),
//                new Vector3(-0.1f, 0.0f, 0.9f),
//                new Vector3(-0.1f, -0.1f, 1.0f),
//                new Vector3(0.0f, -0.1f, 0.9f),
//                new Vector3(-0.029289f, -0.029289f, 0.9f),
//                new Vector3(-0.1f, -0.029289f, 0.970711f),
//                new Vector3(-0.029289f, -0.029289f, 0.1f),
//                new Vector3(0.0f, -0.1f, 0.1f),
//                new Vector3(-0.1f, -0.1f, 0.0f),
//                new Vector3(-0.1f, -0.029289f, 0.029289f),
//                new Vector3(-0.970711f, -0.029289f, 0.9f),
//                new Vector3(-1.0f, -0.1f, 0.9f),
//                new Vector3(-0.9f, -0.1f, 1.0f),
//                new Vector3(-0.9f, -0.029289f, 0.970711f),
//                new Vector3(-0.9f, -0.1f, 0.0f),
//                new Vector3(-1.0f, -0.1f, 0.1f),
//                new Vector3(-0.970711f, -0.029289f, 0.1f),
//                new Vector3(-0.9f, -0.029289f, 0.029289f),
//                new Vector3(-0.1f, 0.0f, 0.1f),
//                new Vector3(-0.1f, 0.0f, 0.9f),
//                new Vector3(-0.029289f, -0.029289f, 0.9f),
//                new Vector3(-0.029289f, -0.029289f, 0.1f),
//                new Vector3(-0.029289f, -0.029289f, 0.1f),
//                new Vector3(-0.029289f, -0.029289f, 0.9f),
//                new Vector3(0.0f, -0.1f, 0.9f),
//                new Vector3(0.0f, -0.1f, 0.1f),
//                new Vector3(-0.9f, 0.0f, 0.1f),
//                new Vector3(-0.1f, 0.0f, 0.1f),
//                new Vector3(-0.1f, -0.029289f, 0.029289f),
//                new Vector3(-0.9f, -0.029289f, 0.029289f),
//                new Vector3(-0.9f, -0.029289f, 0.029289f),
//                new Vector3(-0.1f, -0.029289f, 0.029289f),
//                new Vector3(-0.1f, -0.1f, 0.0f),
//                new Vector3(-0.9f, -0.1f, 0.0f),
//                new Vector3(-0.9f, 0.0f, 0.9f),
//                new Vector3(-0.9f, 0.0f, 0.1f),
//                new Vector3(-0.970711f, -0.029289f, 0.1f),
//                new Vector3(-0.970711f, -0.029289f, 0.9f),
//                new Vector3(-0.970711f, -0.029289f, 0.9f),
//                new Vector3(-0.970711f, -0.029289f, 0.1f),
//                new Vector3(-1.0f, -0.1f, 0.1f),
//                new Vector3(-1.0f, -0.1f, 0.9f),
//                new Vector3(-0.1f, 0.0f, 0.9f),
//                new Vector3(-0.9f, 0.0f, 0.9f),
//                new Vector3(-0.9f, -0.029289f, 0.970711f),
//                new Vector3(-0.1f, -0.029289f, 0.970711f),
//                new Vector3(-0.1f, -0.029289f, 0.970711f),
//                new Vector3(-0.9f, -0.029289f, 0.970711f),
//                new Vector3(-0.9f, -0.1f, 1.0f),
//                new Vector3(-0.1f, -0.1f, 1.0f),
//            },
//            new Vector3[]
//            {
//                new Vector3(-0.1f, 0.0f, 1.9f),
//                new Vector3(-0.1f, -0.029289f, 1.97071f),
//                new Vector3(-0.029289f, -0.029289f, 1.9f),
//                new Vector3(-0.1f, 0.0f, 0.1f),
//                new Vector3(-0.029289f, -0.029289f, 0.1f),
//                new Vector3(-0.1f, -0.029289f, 0.029289f),
//                new Vector3(-1.9f, 0.0f, 1.9f),
//                new Vector3(-1.970711f, -0.029289f, 1.9f),
//                new Vector3(-1.9f, -0.029289f, 1.97071f),
//                new Vector3(-1.9f, 0.0f, 0.1f),
//                new Vector3(-1.9f, -0.029289f, 0.029289f),
//                new Vector3(-1.970711f, -0.029289f, 0.1f),
//                new Vector3(-1.9f, 0.0f, 0.1f),
//                new Vector3(-1.9f, 0.0f, 1.9f),
//                new Vector3(-0.1f, 0.0f, 1.9f),
//                new Vector3(-0.1f, 0.0f, 0.1f),
//                new Vector3(0.0f, -0.1f, 1.9f),
//                new Vector3(-0.029289f, -0.029289f, 1.9f),
//                new Vector3(-0.1f, -0.029289f, 1.97071f),
//                new Vector3(-0.1f, -0.1f, 2f),
//                new Vector3(0.0f, -0.1f, 0.1f),
//                new Vector3(-0.1f, -0.1f, 0.0f),
//                new Vector3(-0.1f, -0.029289f, 0.029289f),
//                new Vector3(-0.029289f, -0.029289f, 0.1f),
//                new Vector3(-2f, -0.1f, 1.9f),
//                new Vector3(-1.9f, -0.1f, 2f),
//                new Vector3(-1.9f, -0.029289f, 1.97071f),
//                new Vector3(-1.970711f, -0.029289f, 1.9f),
//                new Vector3(-1.9f, -0.1f, 0.0f),
//                new Vector3(-2f, -0.1f, 0.1f),
//                new Vector3(-1.970711f, -0.029289f, 0.1f),
//                new Vector3(-1.9f, -0.029289f, 0.029289f),
//                new Vector3(-0.1f, 0.0f, 1.9f),
//                new Vector3(-0.029289f, -0.029289f, 1.9f),
//                new Vector3(-0.029289f, -0.029289f, 0.1f),
//                new Vector3(-0.1f, 0.0f, 0.1f),
//                new Vector3(-0.029289f, -0.029289f, 1.9f),
//                new Vector3(0.0f, -0.1f, 1.9f),
//                new Vector3(0.0f, -0.1f, 0.1f),
//                new Vector3(-0.029289f, -0.029289f, 0.1f),
//                new Vector3(-0.1f, 0.0f, 0.1f),
//                new Vector3(-0.1f, -0.029289f, 0.029289f),
//                new Vector3(-1.9f, -0.029289f, 0.029289f),
//                new Vector3(-1.9f, 0.0f, 0.1f),
//                new Vector3(-0.1f, -0.029289f, 0.029289f),
//                new Vector3(-0.1f, -0.1f, 0.0f),
//                new Vector3(-1.9f, -0.1f, 0.0f),
//                new Vector3(-1.9f, -0.029289f, 0.029289f),
//                new Vector3(-1.9f, 0.0f, 0.1f),
//                new Vector3(-1.970711f, -0.029289f, 0.1f),
//                new Vector3(-1.970711f, -0.029289f, 1.9f),
//                new Vector3(-1.9f, 0.0f, 1.9f),
//                new Vector3(-1.970711f, -0.029289f, 0.1f),
//                new Vector3(-2f, -0.1f, 0.1f),
//                new Vector3(-2f, -0.1f, 1.9f),
//                new Vector3(-1.970711f, -0.029289f, 1.9f),
//                new Vector3(-1.9f, 0.0f, 1.9f),
//                new Vector3(-1.9f, -0.029289f, 1.97071f),
//                new Vector3(-0.1f, -0.029289f, 1.97071f),
//                new Vector3(-0.1f, 0.0f, 1.9f),
//                new Vector3(-1.9f, -0.029289f, 1.97071f),
//                new Vector3(-1.9f, -0.1f, 2f),
//                new Vector3(-0.1f, -0.1f, 2f),
//                new Vector3(-0.1f, -0.029289f, 1.97071f),
//            },
//            new Vector3[]
//            { }
//        };
//        private static Vector2[][] TileUVs = new Vector2[(int)TileType.COUNT][]
//        {
//            new Vector2[]
//            {
//                new Vector2(0.072281f, 0.075187f),
//                new Vector2(0.060137f, 0.03832f),
//                new Vector2(0.036492f, 0.079232f),
//                new Vector2(0.075268f, 0.927719f),
//                new Vector2(0.038172f, 0.941436f),
//                new Vector2(0.080323f, 0.96354f),
//                new Vector2(0.924813f, 0.072201f),
//                new Vector2(0.961909f, 0.058484f),
//                new Vector2(0.919758f, 0.036379f),
//                new Vector2(0.927799f, 0.924732f),
//                new Vector2(0.941516f, 0.961828f),
//                new Vector2(0.963621f, 0.919677f),
//                new Vector2(0.075268f, 0.927719f),
//                new Vector2(0.927799f, 0.924732f),
//                new Vector2(0.924813f, 0.072201f),
//                new Vector2(0.072281f, 0.075187f),
//                new Vector2(0.031504f, 0.011476f),
//                new Vector2(0.0001f, 0.075233f),
//                new Vector2(0.036492f, 0.079232f),
//                new Vector2(0.060137f, 0.03832f),
//                new Vector2(0.038172f, 0.941436f),
//                new Vector2(0.013197f, 0.969351f),
//                new Vector2(0.075341f, 0.999819f),
//                new Vector2(0.080323f, 0.96354f),
//                new Vector2(0.961909f, 0.058484f),
//                new Vector2(0.986883f, 0.030568f),
//                new Vector2(0.92474f, 0.0001f),
//                new Vector2(0.919758f, 0.036379f),
//                new Vector2(0.969432f, 0.986803f),
//                new Vector2(0.9999f, 0.924659f),
//                new Vector2(0.963621f, 0.919677f),
//                new Vector2(0.941516f, 0.961828f),
//                new Vector2(0.075268f, 0.927719f),
//                new Vector2(0.072281f, 0.075187f),
//                new Vector2(0.036492f, 0.079232f),
//                new Vector2(0.036419f, 0.927793f),
//                new Vector2(0.036419f, 0.927793f),
//                new Vector2(0.036492f, 0.079232f),
//                new Vector2(0.0001f, 0.075233f),
//                new Vector2(0.000168f, 0.927796f),
//                new Vector2(0.927799f, 0.924732f),
//                new Vector2(0.075268f, 0.927719f),
//                new Vector2(0.080323f, 0.96354f),
//                new Vector2(0.927871f, 0.963569f),
//                new Vector2(0.927871f, 0.963569f),
//                new Vector2(0.080323f, 0.96354f),
//                new Vector2(0.075341f, 0.999819f),
//                new Vector2(0.927871f, 0.999819f),
//                new Vector2(0.924813f, 0.072201f),
//                new Vector2(0.927799f, 0.924732f),
//                new Vector2(0.963621f, 0.919677f),
//                new Vector2(0.96365f, 0.072129f),
//                new Vector2(0.96365f, 0.072129f),
//                new Vector2(0.963621f, 0.919677f),
//                new Vector2(0.9999f, 0.924659f),
//                new Vector2(0.9999f, 0.072129f),
//                new Vector2(0.072281f, 0.075187f),
//                new Vector2(0.924813f, 0.072201f),
//                new Vector2(0.919758f, 0.036379f),
//                new Vector2(0.07221f, 0.03635f),
//                new Vector2(0.07221f, 0.03635f),
//                new Vector2(0.919758f, 0.036379f),
//                new Vector2(0.92474f, 0.0001f),
//                new Vector2(0.07221f, 0.0001f),
//            },
//            new Vector2[]
//            {
//                new Vector2(0.072281f, 0.075187f),
//                new Vector2(0.060137f, 0.03832f),
//                new Vector2(0.036492f, 0.079232f),
//                new Vector2(0.075268f, 0.927719f),
//                new Vector2(0.038172f, 0.941436f),
//                new Vector2(0.080323f, 0.96354f),
//                new Vector2(0.924813f, 0.072201f),
//                new Vector2(0.961909f, 0.058484f),
//                new Vector2(0.919758f, 0.036379f),
//                new Vector2(0.927799f, 0.924732f),
//                new Vector2(0.941516f, 0.961828f),
//                new Vector2(0.963621f, 0.919677f),
//                new Vector2(0.927799f, 0.924732f),
//                new Vector2(0.924813f, 0.072201f),
//                new Vector2(0.072281f, 0.075187f),
//                new Vector2(0.075268f, 0.927719f),
//                new Vector2(0.0001f, 0.075233f),
//                new Vector2(0.036492f, 0.079232f),
//                new Vector2(0.060137f, 0.03832f),
//                new Vector2(0.031504f, 0.011476f),
//                new Vector2(0.013197f, 0.969351f),
//                new Vector2(0.075341f, 0.999819f),
//                new Vector2(0.080323f, 0.96354f),
//                new Vector2(0.038172f, 0.941436f),
//                new Vector2(0.986883f, 0.030568f),
//                new Vector2(0.92474f, 0.0001f),
//                new Vector2(0.919758f, 0.036379f),
//                new Vector2(0.961909f, 0.058484f),
//                new Vector2(0.969432f, 0.986803f),
//                new Vector2(0.9999f, 0.924659f),
//                new Vector2(0.963621f, 0.919677f),
//                new Vector2(0.941516f, 0.961828f),
//                new Vector2(0.072281f, 0.075187f),
//                new Vector2(0.036492f, 0.079232f),
//                new Vector2(0.036419f, 0.927793f),
//                new Vector2(0.075268f, 0.927719f),
//                new Vector2(0.036492f, 0.079232f),
//                new Vector2(0.0001f, 0.075233f),
//                new Vector2(0.000168f, 0.927796f),
//                new Vector2(0.036419f, 0.927793f),
//                new Vector2(0.075268f, 0.927719f),
//                new Vector2(0.080323f, 0.96354f),
//                new Vector2(0.927871f, 0.963569f),
//                new Vector2(0.927799f, 0.924732f),
//                new Vector2(0.080323f, 0.96354f),
//                new Vector2(0.075341f, 0.999819f),
//                new Vector2(0.927871f, 0.999819f),
//                new Vector2(0.927871f, 0.963569f),
//                new Vector2(0.927799f, 0.924732f),
//                new Vector2(0.963621f, 0.919677f),
//                new Vector2(0.96365f, 0.072129f),
//                new Vector2(0.924813f, 0.072201f),
//                new Vector2(0.963621f, 0.919677f),
//                new Vector2(0.9999f, 0.924659f),
//                new Vector2(0.9999f, 0.072129f),
//                new Vector2(0.96365f, 0.072129f),
//                new Vector2(0.924813f, 0.072201f),
//                new Vector2(0.919758f, 0.036379f),
//                new Vector2(0.07221f, 0.03635f),
//                new Vector2(0.072281f, 0.075187f),
//                new Vector2(0.919758f, 0.036379f),
//                new Vector2(0.92474f, 0.0001f),
//                new Vector2(0.07221f, 0.0001f),
//                new Vector2(0.07221f, 0.03635f),
//            },
//            new Vector2[]
//            { },
//        };
//        private static Vector3[][] TileNormals = new Vector3[(int)TileType.COUNT][]
//        {
//            new Vector3[]
//            {
//                new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(0.0f, 1.0f, 0.0f),
//                new Vector3(0.0f, 1.0f, 0.0f),
//                new Vector3(0.0f, 1.0f, 0.0f),
//                new Vector3(0.0f, 1.0f, 0.0f),
//                new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(0.0f, 0.9238766f, -0.3826903f),
//                new Vector3(0.0f, 0.9238766f, -0.3826903f),
//                new Vector3(0.0f, 0.9238766f, -0.3826903f),
//                new Vector3(0.0f, 0.9238766f, -0.3826903f),
//                new Vector3(0.0f, 0.3826903f, -0.9238766f),
//                new Vector3(0.0f, 0.3826903f, -0.9238766f),
//                new Vector3(0.0f, 0.3826903f, -0.9238766f),
//                new Vector3(0.0f, 0.3826903f, -0.9238766f),
//                new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(0.0f, 0.9238766f, 0.3826903f),
//                new Vector3(0.0f, 0.9238766f, 0.3826903f),
//                new Vector3(0.0f, 0.9238766f, 0.3826903f),
//                new Vector3(0.0f, 0.9238766f, 0.3826903f),
//                new Vector3(0.0f, 0.3826903f, 0.9238766f),
//                new Vector3(0.0f, 0.3826903f, 0.9238766f),
//                new Vector3(0.0f, 0.3826903f, 0.9238766f),
//                new Vector3(0.0f, 0.3826903f, 0.9238766f),
//            },
//            new Vector3[]
//            {
//                new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, 0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(-0.3573882f, 0.8628716f, -0.3573882f),
//                new Vector3(0.0f, 1.0f, 0.0f),
//                new Vector3(0.0f, 1.0f, 0.0f),
//                new Vector3(0.0f, 1.0f, 0.0f),
//                new Vector3(0.0f, 1.0f, 0.0f),
//                new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, 0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(-0.6785955f, 0.2810982f, -0.6785955f),
//                new Vector3(0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(0.0f, 0.9238766f, -0.3826903f),
//                new Vector3(0.0f, 0.9238766f, -0.3826903f),
//                new Vector3(0.0f, 0.9238766f, -0.3826903f),
//                new Vector3(0.0f, 0.9238766f, -0.3826903f),
//                new Vector3(0.0f, 0.3826903f, -0.9238766f),
//                new Vector3(0.0f, 0.3826903f, -0.9238766f),
//                new Vector3(0.0f, 0.3826903f, -0.9238766f),
//                new Vector3(0.0f, 0.3826903f, -0.9238766f),
//                new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(-0.3826903f, 0.9238766f, 0.0f),
//                new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(-0.9238766f, 0.3826903f, 0.0f),
//                new Vector3(0.0f, 0.9238766f, 0.3826903f),
//                new Vector3(0.0f, 0.9238766f, 0.3826903f),
//                new Vector3(0.0f, 0.9238766f, 0.3826903f),
//                new Vector3(0.0f, 0.9238766f, 0.3826903f),
//                new Vector3(0.0f, 0.3826903f, 0.9238766f),
//                new Vector3(0.0f, 0.3826903f, 0.9238766f),
//                new Vector3(0.0f, 0.3826903f, 0.9238766f),
//                new Vector3(0.0f, 0.3826903f, 0.9238766f),
//            },
//            new Vector3[]
//            { },
//        };
//        private static int[][] TileIndices = new int[(int)TileType.COUNT][]
//        {
//            new int[]
//            {
//                0, 1, 2,
//                3, 4, 5,
//                6, 7, 8,
//                9, 10, 11,
//                12, 13, 14,
//                12, 14, 15,
//                16, 17, 18,
//                16, 18, 19,
//                20, 21, 22,
//                20, 22, 23,
//                24, 25, 26,
//                24, 26, 27,
//                28, 29, 30,
//                28, 30, 31,
//                32, 33, 34,
//                32, 34, 35,
//                36, 37, 38,
//                36, 38, 39,
//                40, 41, 42,
//                40, 42, 43,
//                44, 45, 46,
//                44, 46, 47,
//                48, 49, 50,
//                48, 50, 51,
//                52, 53, 54,
//                52, 54, 55,
//                56, 57, 58,
//                56, 58, 59,
//                60, 61, 62,
//                60, 62, 63,
//            },
//            new int[]
//            {
//                0, 1, 2,
//                3, 4, 5,
//                6, 7, 8,
//                9, 10, 11,
//                12, 13, 14,
//                12, 14, 15,
//                16, 17, 18,
//                16, 18, 19,
//                20, 21, 22,
//                20, 22, 23,
//                24, 25, 26,
//                24, 26, 27,
//                28, 29, 30,
//                28, 30, 31,
//                32, 33, 34,
//                32, 34, 35,
//                36, 37, 38,
//                36, 38, 39,
//                40, 41, 42,
//                40, 42, 43,
//                44, 45, 46,
//                44, 46, 47,
//                48, 49, 50,
//                48, 50, 51,
//                52, 53, 54,
//                52, 54, 55,
//                56, 57, 58,
//                56, 58, 59,
//                60, 61, 62,
//                60, 62, 63,
//            },
//            new int[]
//            {  },
//        };
//        private static Vector3[][] TileBoxes = new Vector3[(int)TileType.COUNT][]
//        {
//            new Vector3[2]{ new Vector3(1.0f, 0.1f, 1.0f), new Vector3(-0.5f, -0.05f, 0.5f) },
//            new Vector3[2]{ new Vector3(), new Vector3() },
//            new Vector3[2]{ new Vector3(), new Vector3() },
//        };
//        private static Mesh[] TileMeshes = new Mesh[(int)TileType.COUNT]
//        {
//            null,
//            null,
//            null,
//        };

//        private Map m_Map = null;

//        private string m_Name;
//        public string Name
//        {
//            get { return m_Name; }
//        }
//        public Vector3 Pos { get; } = new Vector3();

//        private bool m_Visible = false;
//        public void SetVisible(bool visible)
//        {
//            if(m_Visible && !visible)
//            {
//                UnityEngine.GameObject.Destroy(m_Object);
//            }
//            if(!m_Visible && visible)
//            {
//                m_Object = new GameObject(m_Name);
//                m_Object.transform.SetParent(m_Map.GO.transform);
//                m_Object.transform.Translate(Pos);
//                m_Object.tag = TileTag;
//                var box = m_Object.AddComponent<BoxCollider>();
//                if (TileBoxes[(int)m_Type][0] != new Vector3())
//                {
//                    box.size = TileBoxes[(int)m_Type][0];
//                    box.center = TileBoxes[(int)m_Type][1];
//                }
//                SetMesh();
//                SetMaterial();
//            }
//            m_Visible = visible;
//        }
//        public bool GetVisible()
//        {
//            return m_Visible;
//        }

//        private float m_Length = 1.0f;
//        public float Length
//        {
//            get { return m_Length; }
//        }

//        private GameObject m_Object = null;
//        public GameObject GO
//        {
//            get { return m_Object; }
//        }

//        private TileType m_Type = TileType.NORMAL;
//        public void SetType(TileType type)
//        {
//            m_Type = type;
//        }
//        public TileType GetType()
//        {
//            return m_Type;
//        }

//        private Mesh m_Mesh = null;
//        public Mesh Mesh
//        {
//            get { return m_Mesh; }
//        }

//        public Tile(Map map, Vector3 position, TileType type, float length)
//        {
//            m_Map = map;
//            m_Name = map.GO.name + $"_tile{(int)position.x * Map.MapTileSize.x}x{(int)position.z * Map.MapTileSize.y}y";
//            m_Type = type;
//            Pos = position;
//            m_Length = length;
//            SetVisible(true);
//        }
        
//        private void SetMesh()
//        {
//            MeshRenderer meshRenderer = null;
//            MeshFilter meshFilter = null;
//            meshRenderer = m_Object.GetComponent<MeshRenderer>();
//            meshFilter = m_Object.GetComponent<MeshFilter>();
//            if(meshRenderer == null)
//                meshRenderer = m_Object.AddComponent<MeshRenderer>();
//            if(meshFilter == null)
//                meshFilter = m_Object.AddComponent<MeshFilter>();

//            m_Mesh = TileMeshes[(int)m_Type];
//            if(m_Mesh == null)
//            {
//                m_Mesh = new Mesh();
//                m_Mesh.vertices = TileVerties[(int)m_Type];
//                m_Mesh.uv = TileUVs[(int)m_Type];
//                m_Mesh.normals = TileNormals[(int)m_Type];
//                m_Mesh.triangles = TileIndices[(int)m_Type];
//                m_Mesh.RecalculateTangents();
//                m_Mesh.Optimize();
//                TileMeshes[(int)m_Type] = m_Mesh;
//            }
//            meshFilter.mesh = m_Mesh;
//            meshRenderer.receiveShadows = true;
//            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
//            meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
//        }

//        private void SetMaterial()
//        {
//            var material = new Material(TileShader);
//            material.color = new Color(0.75f, 0.0f, 0.25f);
//            material.enableInstancing = true;
//            m_Object.GetComponent<Renderer>().material = material;
//        }
//    }
//}
