/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    [CreateAssetMenu(fileName = "Background", menuName = "Assets/Background", order = 1)]
    public class BackgroundInfo : ScriptableObject
    {
        public Texture2D BackgroundTexture = null;
        public Gradient MistGradient = null;
    }
}
