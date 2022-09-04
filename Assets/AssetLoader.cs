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
	public static class AssetLoader
	{
		static readonly string[] VFXPaths = new string[Def.VFXTargetCount]
		{
			"Odd",
			"Prop",
			"Monster",
			"General"
		};
		static readonly Action[] LoadFunctions = new Action[]
		{
			() => { Materials = Resources.LoadAll<Material>("Materials"); },
			() => { Blocks = Resources.LoadAll<MeshFilter>("Blocks"); },
			() => { MaterialFamilies = Resources.LoadAll<MaterialFamilyInfo>("MaterialFamilies"); },
			() => { FloorTextures = Resources.LoadAll<Texture2D>("Floor"); },
			() => { MonsterFamilies = Resources.LoadAll<MonsterFamily>("MonsterFamilies"); },
			() => { PropInfos = Resources.LoadAll<PropInfo>("PropInfos"); },
			() => { VFXSprites = new Sprite[Def.VFXTargetCount][]; VFXSprites[0] = Resources.LoadAll<Sprite>("VFX/" + VFXPaths[0]); },
			() => { VFXSprites[1] = Resources.LoadAll<Sprite>("VFX/" + VFXPaths[1]); },
			() => { VFXSprites[2] = Resources.LoadAll<Sprite>("VFX/" + VFXPaths[2]); },
			() => { VFXSprites[3] = Resources.LoadAll<Sprite>("VFX/" + VFXPaths[3]); },
			() => { VFX3DTextures = Resources.LoadAll<Texture2D>("VFX/3D"); },
			() => { AssetLoader.Particles = Resources.LoadAll<Texture2D>("Particles");
					ParticleEffect = Resources.Load<GameObject>("Particles/SprayPrefab");
					ConeAreaDamage = Resources.Load<GameObject>("AreaDamages/ConeAreaPrefab"); },
			() => { OddWeaponInfos = Resources.LoadAll<OddWeaponInfo>("OddWeaponInfos"); },
			() => { SpellWeapons = Resources.LoadAll<Sprite>("WeaponsSpell"); },
			() => { BlockDecorations = Resources.LoadAll<Texture2D>("BlockDecoration"); },
			() => { EditorSprites = Resources.LoadAll<Sprite>("EditorSprites"); },
			() => { Bridges = Resources.LoadAll<BridgeComponent>("Bridges"); },
			() => { VisualEffects = Resources.LoadAll<UnityEngine.VFX.VisualEffectAsset>("ParticleVFX"); },
			() => { AntSprites = Resources.LoadAll<Sprite>("Ants"); },
			//() => { InventoryItems = Resources.LoadAll<Sprite>("InventoryItems"); },
			() => { BackgroundInfos = Resources.LoadAll<BackgroundInfo>("Backgrounds"); },
			() => {
				var pospo = GameObject.Find("PospoVolume");
				if(pospo != null)
					PospoVolume = pospo.GetComponent<UnityEngine.Rendering.Volume>().profile;
			},
			LoadPrefabs,
		};
		static void LoadPrefabs()
		{
			StatusBars = Resources.Load<AI.LEStatusBars>("LEBars");
			//StatusBars.Canvas.worldCamera = CameraManager.Mgr.Camera;
			Odd = Resources.Load<AI.ODD.COddController>("Odd");
			LockArrow = Resources.Load<UI.CLockArrow>("UI/TargetArrow");
		}
		static int CurrentLoadFunction = 0;

		static readonly Action[] PrepareFunctions = new Action[]
		{
			Assets.Materials.Prepare,
			Manager.Prepare,
			Assets.Blocks.Prepare,
			//BlockMeshDef.InitBlocks,
			BlockMaterial.Prepare,
			Props.Prepare,
			Monsters.Prepare,
			SpawnerInfoLoader.Prepare,
			AI.Items.ItemLoader.Prepare,
			Structures.Init,
			BiomeLoader.Prepare,
			VFXs.Prepare,
			VFX3Ds.Prepare,
			WeaponLoader.Prepare,
			Assets.SpellWeapons.Prepare,
			Assets.Particles.Prepare,
			Assets.EditorSprites.Prepare,
			Assets.Bridges.Prepare,
			ParticleVFXs.Prepare,
			Backgrounds.Prepare,
			//Assets.InventoryItems.Prepare,
			AI.Quirks.QuirkManager.Prepare,
			AI.Spells.SpellManager.Prepare,
			UI.MessageBoxUI.LoadInit,
			World.WorldPool.Init,
		};
		static int CurrentPrepareFunction = 0;

		public static Material[] Materials;

		public static MaterialFamilyInfo[] MaterialFamilies;
		public static Texture2D[] FloorTextures;
		//public static TextAsset FloorInfoText;

		public static MeshFilter[] Blocks;

		public static BridgeComponent[] Bridges;

		public static MonsterFamily[] MonsterFamilies;

		public static OddWeaponInfo[] OddWeaponInfos;

		public static PropInfo[] PropInfos;

		public static Sprite[][] VFXSprites;
		public static Texture2D[] VFX3DTextures;

		public static Texture2D[] BlockDecorations;

		public static Sprite[] EditorSprites;

		public static Sprite[] SpellWeapons;

		public static Texture2D[] Particles;
		public static GameObject ParticleEffect;
		public static GameObject ConeAreaDamage;

		public static Sprite[] AntSprites;

		public static BackgroundInfo[] BackgroundInfos;

		public static AI.LEStatusBars StatusBars;
		public static UI.CLockArrow LockArrow;

		public static AI.ODD.COddController Odd;

		public static UnityEngine.Rendering.VolumeProfile PospoVolume;

		//public static Sprite[] InventoryItems;

		public static UnityEngine.VFX.VisualEffectAsset[] VisualEffects;

		public static bool LoadNext()
		{
			LoadFunctions[CurrentLoadFunction++]();
			
			return CurrentLoadFunction == LoadFunctions.Length;
		}
		public static int GetCurrentLoadFunctionIdx() => CurrentLoadFunction;
		public static int GetLoadFunctionCount() => LoadFunctions.Length;
		public static bool PrepareNext()
		{
			PrepareFunctions[CurrentPrepareFunction++]();

			return CurrentPrepareFunction == PrepareFunctions.Length;
		}
		public static int GetCurrentPrepareFunctionIdx() => CurrentPrepareFunction;
		public static int GetPrepareFunctionCount() => PrepareFunctions.Length;
		public static void Init()
		{
			CurrentLoadFunction = 0;
			CurrentPrepareFunction = 0;
		}
		public static void Stop()
		{
			CurrentLoadFunction = 0;
			CurrentPrepareFunction = 0;

			void UnloadList<T>(ref T[] list)
			{
				for (int i = 0; i < list.Length; ++i)
					Resources.UnloadAsset(list[i] as UnityEngine.Object);
				list = new T[0];
			}
			UnloadList(ref MaterialFamilies);
			UnloadList(ref FloorTextures);
			UnloadList(ref MonsterFamilies);
			UnloadList(ref PropInfos);
			for (int i = 0; i < VFXSprites.Length; ++i)
				UnloadList(ref VFXSprites[i]);
			UnloadList(ref VFX3DTextures);
			UnloadList(ref SpellWeapons);
			UnloadList(ref BlockDecorations);
			UnloadList(ref EditorSprites);
			UnloadList(ref AntSprites);
			UnloadList(ref BackgroundInfos);
		}
	}
}
