/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using UnityEngine;

namespace Assets
{
	public struct BlockDef
	{
        public BoxCollider BlockBC;
        public MeshRenderer TopMR;
        public MeshRenderer MidMR;
        public AI.CProp Prop;
        public AI.CMonster Monster;
        public MaterialFamily MaterialFamily;
        public MaterialPart TopMaterialPart;
        public MaterialPart MidMaterialPart;
        public int Layer;
        public float Length;
        public float Height;
        public float MicroHeight;
        public float BlockFloatOffset;
        public float BlockFloatSpeed;
        public CPilar Pilar;
        public AntComponent[] Ants;
        public SpriteBackendSprite[] Deco;
        public int[] StackedBlocksIdx;
        //public IBlock[] StackedBlocks;
        public Def.BlockType BlockType;
        public Def.StairType StairType;
        public Def.RotationState Rotation;
    }
}