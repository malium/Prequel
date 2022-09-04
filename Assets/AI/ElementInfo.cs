/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

namespace Assets.AI
{
	public static class ElementInfo
	{
		public static readonly float[] Duration = new float[Def.ElementTypeCount]
		{
			3f,  // BLEEDING
			2f,  // BURNING
			2f,  // FREEZING
			4f,  // CURSED
			3f,  // POISONED
			4f   // DISEASE
		};
		public static readonly bool[] Stackable = new bool[Def.ElementTypeCount]
		{
			true,   // BLEEDING
			false,  // BURNING
			true,   // FREEZING
			false,  // CURSED
			true,   // POISONED
			false   // DISEASE
		};
		public static readonly bool[] Renewable = new bool[Def.ElementTypeCount]
		{
			true,   // BLEEDING
			false,  // BURNING
			true,   // FREEZING
			false,  // CURSED
			true,   // POISONED
			false   // DISEASE
		};
		public static readonly float[] DamagePerSecond = new float[Def.ElementTypeCount]
		{
			3f,  // BLEEDING
			5f,  // BURNING
			1f,  // FREEZING
			4f,  // CURSED
			6f,  // POISONED
			3f   // DISEASE
		};
		public static readonly Def.DamageType[] DmgType = new Def.DamageType[Def.ElementTypeCount]
		{
			Def.DamageType.CUT,         // BLEEDING
			Def.DamageType.FIRE,        // BURNING
			Def.DamageType.ICE,			// FREEZING
			Def.DamageType.DEPRESSION,  // CURSED
			Def.DamageType.POISON,      // POISONED
			Def.DamageType.DEPRESSION   // DISEASE
		};
		public static Def.ElementType GetElementTypeFromDamageType(Def.DamageType damageType)
		{
			var element = Def.ElementType.COUNT;
			switch (damageType)
			{
				case Def.DamageType.CUT:
					element = Def.ElementType.BLEEDING;
					break;
				case Def.DamageType.FIRE:
				case Def.DamageType.ELECTRICAL:
					element = Def.ElementType.BURNING;
					break;
				case Def.DamageType.ICE:
					element = Def.ElementType.FREEZING;
					break;
				case Def.DamageType.DEPRESSION:
					element = Def.ElementType.CURSED;
					break;
				case Def.DamageType.POISON:
					element = Def.ElementType.POISONED;
					break;
				case Def.DamageType.QUICKSILVER:
					element = Def.ElementType.DISEASE;
					break;
			}
			return element;
		}
	}
}