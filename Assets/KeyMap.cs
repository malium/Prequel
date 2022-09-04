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
	using KeyBind = System.Collections.Generic.KeyValuePair<UnityEngine.KeyCode, bool>;

	public static class KeyMap
	{
		public struct KeyCombination
		{
			public KeyCode Key;
			public bool Ctrl;
			public bool Shift;
			public bool Alt;

			public KeyCombination(KeyCode key)
			{
				Key = key;
				Ctrl = Shift = Alt = false;
			}
			public KeyCombination(KeyCode key, bool ctrl, bool shift, bool alt)
			{
				Key = key;
				Ctrl = ctrl;
				Shift = shift;
				Alt = alt;
			}
		}

		public static KeyBind[][] StrucEditKeyBind = new KeyBind[][]//Def.StrucEditKeyMapCount]
		{
			new KeyBind[]{ new KeyBind(KeyCode.F1, true), },													// Info
			new KeyBind[]{ new KeyBind(KeyCode.Escape, true), },												// Menu
			new KeyBind[]{ new KeyBind(KeyCode.F9, true), },													// CamEdit
			new KeyBind[]{ new KeyBind(KeyCode.F10, true), },													// CamGame
			new KeyBind[]{ new KeyBind(KeyCode.F11, true), },													// CamFree
			new KeyBind[]{ new KeyBind(KeyCode.F2, true), },													// LayerEdit
			new KeyBind[]{ new KeyBind(KeyCode.Space, true), new KeyBind(KeyCode.LeftControl, true), },			// SelectAll
			new KeyBind[]{ new KeyBind(KeyCode.KeypadPlus, true), new KeyBind(KeyCode.LeftControl, true),
							new KeyBind(KeyCode.LeftShift, false), },											// IncreaseStrucWidth
			new KeyBind[]{ new KeyBind(KeyCode.KeypadMinus, true), new KeyBind(KeyCode.LeftControl, true),
							new KeyBind(KeyCode.LeftShift, false),},											// DecreaseStrucWidth
			new KeyBind[]{ new KeyBind(KeyCode.KeypadPlus, true), new KeyBind(KeyCode.LeftShift, true),
							new KeyBind(KeyCode.LeftControl, false), },											// IncreaseStrucHeigth
			new KeyBind[]{ new KeyBind(KeyCode.KeypadMinus, true), new KeyBind(KeyCode.LeftShift, true),
							new KeyBind(KeyCode.LeftControl, false), },											// DecreaseStrucHeight
			new KeyBind[]{ new KeyBind(KeyCode.F5, true), },													// ReapplyLayers
			new KeyBind[]{ new KeyBind(KeyCode.Backspace, true), },												// ResetLocks
			new KeyBind[]{ new KeyBind(KeyCode.Tab, true), },													// MaterialCycle
			new KeyBind[]{ new KeyBind(KeyCode.E, true), },														// Stair
			new KeyBind[]{ new KeyBind(KeyCode.R, true), },														// Ramp
			new KeyBind[]{ new KeyBind(KeyCode.L, true), },														// Lock
			new KeyBind[]{ new KeyBind(KeyCode.Q, true), },														// Anchor
			new KeyBind[]{ new KeyBind(KeyCode.LeftArrow, true), },												// RotateLeft
			new KeyBind[]{ new KeyBind(KeyCode.RightArrow, true), },											// RotateRight
			new KeyBind[]{ new KeyBind(KeyCode.KeypadPlus, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// LengthUp
			new KeyBind[]{ new KeyBind(KeyCode.KeypadMinus, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// LengthDown
			new KeyBind[]{ new KeyBind(KeyCode.UpArrow, true), },												// HeightUp
			new KeyBind[]{ new KeyBind(KeyCode.DownArrow, true), },												// HeightDown
			new KeyBind[]{ new KeyBind(KeyCode.Delete, true), },												// DestroyBlock
			new KeyBind[]{ new KeyBind(KeyCode.V, true), },														// Void
			new KeyBind[]{ new KeyBind(KeyCode.T, true), },														// StackBlock
			new KeyBind[]{ new KeyBind(KeyCode.H, true), },														// Visibility
			new KeyBind[]{ new KeyBind(KeyCode.Alpha1, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer1
			new KeyBind[]{ new KeyBind(KeyCode.Alpha2, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer2
			new KeyBind[]{ new KeyBind(KeyCode.Alpha3, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer3
			new KeyBind[]{ new KeyBind(KeyCode.Alpha4, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer4
			new KeyBind[]{ new KeyBind(KeyCode.Alpha5, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer5
			new KeyBind[]{ new KeyBind(KeyCode.Alpha6, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer6
			new KeyBind[]{ new KeyBind(KeyCode.Alpha7, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer7
			new KeyBind[]{ new KeyBind(KeyCode.Alpha8, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer8
			new KeyBind[]{ new KeyBind(KeyCode.Alpha9, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer9
			new KeyBind[]{ new KeyBind(KeyCode.Alpha0, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer10
			new KeyBind[]{ new KeyBind(KeyCode.Alpha1, true), new KeyBind(KeyCode.LeftShift, true),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer11
			new KeyBind[]{ new KeyBind(KeyCode.Alpha2, true), new KeyBind(KeyCode.LeftShift, true),
							new KeyBind(KeyCode.LeftControl, false), },											// Layer12

			new KeyBind[]{ new KeyBind(KeyCode.Alpha0, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer0
			new KeyBind[]{ new KeyBind(KeyCode.Alpha1, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer1
			new KeyBind[]{ new KeyBind(KeyCode.Alpha2, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer2
			new KeyBind[]{ new KeyBind(KeyCode.Alpha3, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer3
			new KeyBind[]{ new KeyBind(KeyCode.Alpha4, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer4
			new KeyBind[]{ new KeyBind(KeyCode.Alpha5, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer5
			new KeyBind[]{ new KeyBind(KeyCode.Alpha6, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer6
			new KeyBind[]{ new KeyBind(KeyCode.Alpha7, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer7
			new KeyBind[]{ new KeyBind(KeyCode.Alpha8, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer8
			new KeyBind[]{ new KeyBind(KeyCode.Alpha9, true), new KeyBind(KeyCode.LeftShift, false),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer9
			new KeyBind[]{ new KeyBind(KeyCode.Alpha0, true), new KeyBind(KeyCode.LeftShift, true),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer10
			new KeyBind[]{ new KeyBind(KeyCode.Alpha1, true), new KeyBind(KeyCode.LeftShift, true),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer11
			new KeyBind[]{ new KeyBind(KeyCode.Alpha2, true), new KeyBind(KeyCode.LeftShift, true),
							new KeyBind(KeyCode.LeftControl, true), },											// SelectLayer12
			new KeyBind[]{ new KeyBind(KeyCode.W, true), },														// CamMoveForward
			new KeyBind[]{ new KeyBind(KeyCode.S, true), },														// CamMoveBackward
			new KeyBind[]{ new KeyBind(KeyCode.A, true), },														// CamMoveLeft
			new KeyBind[]{ new KeyBind(KeyCode.D, true), },														// CamMoveRight
			new KeyBind[]{ new KeyBind(KeyCode.X, true), new KeyBind(KeyCode.LeftControl, false), },			// BlockHide
			new KeyBind[]{ new KeyBind(KeyCode.X, true), new KeyBind(KeyCode.LeftControl, true), },				// BlockUnhide
			new KeyBind[]{ new KeyBind(KeyCode.Z, true), new KeyBind(KeyCode.LeftControl, true), },				// Undo
		};

		static bool IsCombinationActive(KeyBind[] combination)
		{
			for(int i = 0; i < combination.Length; ++i)
			{
				if(combination[i].Key == KeyCode.LeftControl)
				{
					if (combination[i].Value != Input.GetKey(KeyCode.LeftControl) &&
						combination[i].Value != Input.GetKey(KeyCode.RightControl))
						return false;
				}
				else if(combination[i].Key == KeyCode.LeftShift)
				{
					if (combination[i].Value != Input.GetKey(KeyCode.LeftShift) &&
						combination[i].Value != Input.GetKey(KeyCode.RightShift))
						return false;
				}
				else if(combination[i].Key == KeyCode.LeftAlt)
				{
					if (combination[i].Value != Input.GetKey(KeyCode.LeftAlt) &&
						combination[i].Value != Input.GetKey(KeyCode.RightAlt))
						return false;
				}
				else
				{
					if (Input.GetKey(combination[i].Key) != combination[i].Value)
						return false;
				}
			}
			//if (!Input.GetKey(comb.Key))
			//	return false;

			//if (comb.Ctrl && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
			//	return false;

			//if (comb.Shift && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
			//	return false;

			//if (comb.Alt && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
			//	return false;

			return true;
		}

		public static bool StrucEditCheck(Def.StrucEditKeyMap key)
		{
			return IsCombinationActive(StrucEditKeyBind[(int)key]);
		}
	}
}
