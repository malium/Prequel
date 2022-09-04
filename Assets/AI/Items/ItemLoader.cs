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
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.AI.Items
{
	public static class ItemLoader
	{
		public static readonly string ItemPath = Application.streamingAssetsPath + "/Items";

		public static Sprite InvalidItem;

		public static Sprite[] ItemSprites;
		public static Dictionary<string, int> ItemSpriteDict;

		public static List<Item> Items;
		//public static Dictionary<string, int> ItemNameDict;
		public static Dictionary<int, int> ItemIDDict;

		public static List<IItemEffect> ItemEffects;
		public static Dictionary<string, int> ItemEffectNameDict;

		static void Serial_UnknownNode(object sender, XmlNodeEventArgs e)
		{
			Debug.LogWarning("XMLSerializer unknown node." + e.Name);
		}
		static void Serial_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			Debug.LogWarning("XMLSerializer unknown attribute.");
		}
		public static IItemEffect GetItemEffect(Def.ItemEffectType iet)
		{
			int idx = (int)iet;
			if (idx < 0 || idx >= ItemEffects.Count)
				return null;
			return ItemEffects[idx];
		}
		public static void Prepare()
		{
			InvalidItem = Resources.Load<Sprite>("InvalidSprites/Item");
			ItemSprites = Resources.LoadAll<Sprite>("Items");
			ItemSpriteDict = new Dictionary<string, int>(ItemSprites.Length);
			for(int i = 0; i < ItemSprites.Length; ++i)
				ItemSpriteDict.Add(ItemSprites[i].name, i);

			ItemEffects = new List<IItemEffect>()
			{
				new StatUpDown(),
				new Consumable(),

			};

			ItemEffectNameDict = new Dictionary<string, int>(ItemEffects.Count);
			for (int i = 0; i < ItemEffects.Count; ++i)
				ItemEffectNameDict.Add(ItemEffects[i].GetItemEffectType().ToString(), i);

			var di = new DirectoryInfo(ItemPath);
			if (!di.Exists)
				di.Create();

			var serializer = new XmlSerializer(typeof(Item));
			serializer.UnknownNode += Serial_UnknownNode;
			serializer.UnknownAttribute += Serial_UnknownAttribute;

			var files = di.GetFiles("*.xml");
			Items = new List<Item>(files.Length);
			//ItemNameDict = new Dictionary<string, int>(files.Length);
			ItemIDDict = new Dictionary<int, int>(files.Length);
			for(int i = 0; i < files.Length; ++i)
			{
				var fi = files[i];
				var fs = fi.OpenRead();
				var item = (Item)serializer.Deserialize(fs);

				//ItemNameDict.Add(item.GetName(), Items.Count);
				ItemIDDict.Add(item.GetID(), Items.Count);
				Items.Add(item);

				fs.Close();
			}
		}
		public static void SaveItems()
		{
			var serializer = new XmlSerializer(typeof(Item));
			serializer.UnknownNode += Serial_UnknownNode;
			serializer.UnknownAttribute += Serial_UnknownAttribute;

			for(int i = 0; i < Items.Count; ++i)
			{
				var item = Items[i];
				if (item == null)
					continue;

				var fileName = item.GetName() + '_' + item.GetID().ToString() + ".xml";
				var fi = new FileInfo(ItemPath + '/' + fileName);
				if (fi.Exists)
					fi.Delete();
				var fs = fi.Create();
				serializer.Serialize(fs, item);
				fs.Close();
			}
		}
		public static void SaveItem(Item item)
		{
			var serializer = new XmlSerializer(typeof(Item));
			serializer.UnknownNode += Serial_UnknownNode;
			serializer.UnknownAttribute += Serial_UnknownAttribute;

			var fileName = item.GetName() + '_' + item.GetID().ToString() + ".xml";
			var fi = new FileInfo(ItemPath + '/' + fileName);
			if (fi.Exists)
				fi.Delete();
			var fs = fi.Create();
			serializer.Serialize(fs, item);
			fs.Close();
		}
		public static IItemEffect CreateItemEffect(string itemEffectName)
		{
			if(!ItemEffectNameDict.TryGetValue(itemEffectName, out int idx))
				return null;
			var defItemEffect = ItemEffects[idx];
			return Activator.CreateInstance(defItemEffect.GetType()) as IItemEffect;
		}
		public static IItemEffect CreateItemEffect(Def.ItemEffectType itemEffectType)
		{
			var defItemEffect = GetItemEffect(itemEffectType);
			if (defItemEffect == null)
				return null;
			return Activator.CreateInstance(defItemEffect.GetType()) as IItemEffect;
		}
	}
}
