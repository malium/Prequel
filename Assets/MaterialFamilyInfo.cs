/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System.Collections;
using UnityEngine;

namespace Assets
{
    [CreateAssetMenu(fileName = "MaterialFamily", menuName = "Assets/MaterialFamily", order = 7)]
    public class MaterialFamilyInfo : ScriptableObject
    {
        [Header("Base Properties")]
        public int ID = -1;
        public string FamilyName = "INVALID";
        public Def.MaterialMode MaterialMode = Def.MaterialMode.Default;
        [Tooltip("Default: (1.4, 1, 0, 1.5)")]
        public Vector4 CSHB = new Vector4(1.4f, 1f, 0f, 1.5f);
        [Header("Material Properties")]
        public Def.BlockPhysicState PhysicalState = Def.BlockPhysicState.SOLID;
        public float SlowdownTime = 1f;
        [Range(0, 5)]
        public int Transparency = 0;
        [Range(1, 5)]
        public int Weight = 3;
        [Range(1, 5)]
        public int Hardness = 3;
        // Drops -> needs items
        public bool CastDamageParticles = false;
        [Range(1,5)]
        public int Fertility = 3;
        public Def.BlockStability Stability = Def.BlockStability.STABLE;
        [Range(1, 5)]
        [Tooltip("Only if Stability is set to 'WEIGHT_OVER_IS_LESS'")]
        public int MaximumWeightOver = 3;
        [Range(0f, 4f)]
        public float LESpeedMultiplier = 1f;
        [Range(-100, 100)]
        public int SoulnessRegeneration = 0;
        [Range(0f, 4f)]
        public float FallDamageMultiplier = 1f;
        [Range(0, 5)]
        public int Glow = 0;
        public Color GlowColor = Color.white;
        public bool Radioactive = false;
        [Range(-5, 5)]
        public int HeatEmission = 0;
        // Spawn monsters/props under certain conditions
        [Range(0, 100)]
        public int SlidePct = 0;
        [Range(0f, 10f)]
        public float FloatSpeed = 1f;
        [Range(0f, 4f)]
        public float RangeOffset = 0f;
        public bool PushHeavierBlocks = false;
        public bool PushLighterBlocks = true;
        // Receive damage by (condition + param + damageAmount)
        public int BlockHP = 10;
        public bool Flamable = false;
        // Conservation -> ex: Flesh will get rotten with time
        // OnTemperature -> ex: metal melts if temperature too high
    }
}