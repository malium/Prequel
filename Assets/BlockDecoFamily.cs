/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    [CreateAssetMenu(fileName = "DecoFamily", menuName = "Assets/BlockDecoFamily", order = 2)]
    public class BlockDecoFamily : ScriptableObject
    {
        [SerializeReference]
        public BlockDecoElem[] Elements;
        public string FamilyName;
    }
}