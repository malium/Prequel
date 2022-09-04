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

namespace Assets.UI
{
	public class CBiomeStat : MonoBehaviour
	{
		public CTMPSlider MinSlider;
		public CTMPSlider MaxSlider;
		public TMPro.TMP_Text MinValue;
		public TMPro.TMP_Text MaxValue;
		public Def.BiomeStat BiomeStat;

		public int GetMinValue()
		{
			if (MinSlider == null || MinSlider.Slider == null)
				return 0;
			return (int)MinSlider.Slider.value;
		}
		public int GetMaxValue()
		{
			if (MaxSlider == null || MaxSlider.Slider == null)
				return 0;
			return (int)MaxSlider.Slider.value;
		}
		public void SetMinValue(int value)
		{
			MinValue.text = value.ToString();
			MinSlider.SetValue(value);
		}
		public void SetMaxValue(int value)
		{
			MaxValue.text = value.ToString();
			MaxSlider.SetValue(value);
		}
	}
}
