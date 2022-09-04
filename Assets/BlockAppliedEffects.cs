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
    public static class BlockAppliedEffects
    {
        public const float PoisonWaitTime = 1f;
        public const float PoisonDamage = 10f;
        static float LastPoison = 0f;

        public static Action<AI.CLivingEntity>[] Effects = new Action<AI.CLivingEntity>[Def.BlockEffectCount]
        {
            // Poison
            (AI.CLivingEntity target) => 
            {
                if(LastPoison != Time.time &&
                (LastPoison + PoisonWaitTime) > Time.time)
                    return;

                target.ReceiveDamage(null, Def.DamageType.POISON, PoisonDamage);
                LastPoison = Time.time;
            },
            // Slowness
            (AI.CLivingEntity target) =>
            {
                // speed modifier
            },
        };
    }
}