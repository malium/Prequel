/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AI.Spells
{
	public class SpellManager
	{
		public static List<ISpell> Spells;
		public static Dictionary<string, int> SpellDict;

		public static List<IOnHit> OnHits;
		public static Dictionary<string, int> OnHitDict;
		public static Dictionary<Def.OnHitType, int> OnHitTypeDict;

		public static ISpell CreateSpell(string spellName)
		{
			if (!SpellDict.ContainsKey(spellName))
				return null;

			var defSpell = Spells[SpellDict[spellName]];
			return Activator.CreateInstance(defSpell.GetType()) as ISpell;
		}

		public static IOnHit CreateOnHit(string onHitName)
		{
			if (!OnHitDict.ContainsKey(onHitName))
				return null;

			var defOnHit = OnHits[OnHitDict[onHitName]];
			return Activator.CreateInstance(defOnHit.GetType()) as IOnHit;
		}
		public static IOnHit CreateOnHit(Def.OnHitType onHitType)
		{
			if (!OnHitTypeDict.ContainsKey(onHitType))
				return null;

			var defOnHit = OnHits[OnHitTypeDict[onHitType]];
			return Activator.CreateInstance(defOnHit.GetType()) as IOnHit;
		}

		public static void Prepare()
		{
			Spells = new List<ISpell>()
			{
				new NullSpell(),
				new ProjectileSpell(),
				new SpraySpell(),
				new MeleeSpell(),
				// Add your spells here...

			};

			OnHits = new List<IOnHit>()
			{
				new OnHitDisplacement(),
				new OnHitStatusEffect(),
				new OnHitDamage(),
				// Add your OnHit effects here..

			};

			SpellDict = new Dictionary<string, int>(Spells.Count);
			for (int i = 0; i < Spells.Count; ++i)
				SpellDict.Add(Spells[i].GetName(), i);

			OnHitDict = new Dictionary<string, int>(OnHits.Count);
			for (int i = 0; i < OnHits.Count; ++i)
				OnHitDict.Add(OnHits[i].GetName(), i);
			OnHitTypeDict = new Dictionary<Def.OnHitType, int>(OnHits.Count);
			for (int i = 0; i < OnHits.Count; ++i)
				OnHitTypeDict.Add(OnHits[i].GetOnHitType(), i);
		}
	}
}
