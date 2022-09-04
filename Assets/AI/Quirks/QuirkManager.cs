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

namespace Assets.AI.Quirks
{
	public class QuirkManager
	{
		public static List<IQuirk> Quirks;
		public static Dictionary<string, int> QuirkDict;
		public static List<IQuirkTrigger> Triggers;
		public static Dictionary<string, int> TriggerNameDict;
		public static Dictionary<Def.QuirkTriggerType, int> TriggerDict;
		public static IQuirk CreateQuirk(string quirkName)
		{
			if (!QuirkDict.ContainsKey(quirkName))
				return null;
			var defQuirk = Quirks[QuirkDict[quirkName]];
			return Activator.CreateInstance(defQuirk.GetType()) as IQuirk;
		}
		public static IQuirkTrigger CreateTrigger(string triggerName)
		{
			if (!TriggerNameDict.ContainsKey(triggerName))
				return null;
			var defTrigger = Triggers[TriggerNameDict[triggerName]];
			return Activator.CreateInstance(defTrigger.GetType()) as IQuirkTrigger;
		}
		public static IQuirkTrigger CreateTrigger(Def.QuirkTriggerType triggerType)
		{
			if (!TriggerDict.ContainsKey(triggerType))
				return null;
			var defTrigger = Triggers[TriggerDict[triggerType]];
			return Activator.CreateInstance(defTrigger.GetType()) as IQuirkTrigger;
		}
		public static void Prepare()
		{
			Quirks = new List<IQuirk>()
			{
				new QuirkStatic(),
				new QuirkRoaming(),
				new QuirkLookingAtHeardEnemies(),
				new QuirkAlertOnFirstSight(),
				new QuirkStareAtTarget(),
				new QuirkSniper(),
				new QuirkHunter(),
				new QuirkCoward(),
				new QuirkChangeTarget(),
				//new RoamingQuirk(),
				//new SniperQuirk(),
				//new HunterQuirk()
				// Add your quirks ...

			};
			Triggers = new List<IQuirkTrigger>()
			{
				new TriggerAwareness(),
				new TriggerEntitesHeard(),
				new TriggerEntitesOnSight(),
				new TriggerEntitiesNear(),
				new TriggerSpellState(),
				new TriggerTargetNotNull(),
				new TriggerTimerReady(),
				new TriggerHasAttacked(),
				new TriggerTargetReached(),
				new TriggerStatus(),
				new TriggerTargetStatus(),
				new TriggerOnHit(),
				new TriggerTargetIs(),
				// Add your triggers...
			};
			QuirkDict = new Dictionary<string, int>(Quirks.Count);
			for(int i = 0; i < Quirks.Count; ++i)
			{
				QuirkDict.Add(Quirks[i].GetName(), i);
			}
			TriggerNameDict = new Dictionary<string, int>(Triggers.Count);
			for(int i = 0; i < Triggers.Count; ++i)
			{
				TriggerNameDict.Add(Triggers[i].GetName(), i);
			}
			TriggerDict = new Dictionary<Def.QuirkTriggerType, int>(Triggers.Count);
			for(int i = 0; i < Triggers.Count; ++i)
			{
				TriggerDict.Add(Triggers[i].GetTriggerType(), i);
			}
		}
	}
}
