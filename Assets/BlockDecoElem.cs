/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System.Collections;
using UnityEngine;

namespace Assets
{
    [CreateAssetMenu(fileName = "DecoElement", menuName = "Assets/BlockDecoElement", order = 3)]
    public class BlockDecoElem : ScriptableObject
    {
        public Def.DecoSpriteType Type;
        public Texture2D[] Images;
        public float FPS;
        [SerializeReference]
        public BlockDecoFamily Family;
    }
}