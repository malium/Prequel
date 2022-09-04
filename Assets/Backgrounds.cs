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
    public static class Backgrounds
    {
        public static List<BackgroundInfo> Infos;
        public static Dictionary<string, int> Dict;

        public static BackgroundInfo GetBackgroundInfo(string backgroundName)
		{
            if (Dict == null || Infos == null)
                throw new Exception("Backgrounds is not ready, trying to get a BackgroundInfo before loading?");
            if (Dict.ContainsKey(backgroundName))
            {
                return Infos[Dict[backgroundName]];
            }
            if (Infos.Count == 0)
                throw new Exception("There are no Backgrounds, something went really wrong.");

            return Infos[0];
        }
        public static void Prepare()
		{
            var backgrounds = AssetLoader.BackgroundInfos;
			Infos = new List<BackgroundInfo>(backgrounds.Length);
			Dict = new Dictionary<string, int>(Infos.Capacity);
			for (int i = 0; i < backgrounds.Length; ++i)
			{
				var info = backgrounds[i];
				Infos.Add(info);
				Dict.Add(info.name, i);
			}
		}
        //public static void Init()
        //{
        //    var backgrounds = Resources.LoadAll<BackgroundInfo>("Backgrounds");
        //    Infos = new List<BackgroundInfo>(backgrounds.Length);
        //    Dict = new Dictionary<string, int>(Infos.Capacity);
        //    for(int i = 0; i < backgrounds.Length; ++i)
        //    {
        //        var info = backgrounds[i];
        //        Infos.Add(info);
        //        Dict.Add(info.name, i);
        //    }
        //}

        //public static void Deinit()
        //{

        //}
    }
}