/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.VFX;

//namespace Assets
//{
	//public enum ShadedMaterial
	//{
	//    Default,
	//    Cutout,
	//    Fade,
	//    Transparent
	//}
	

	//[Serializable]
	//struct AssetHashes
	//{
	//    public int FloorTexturesHash;
	//    public int MonsterSpritesHash;
	//    public int PropSpritesHash;
	//    public int[] TextAssetsHash;
	//    public int OddVFXHash;
	//    public int MonsterVFXHash;
	//    public int PropVFXHash;
	//    public int GeneralVFXHash;
	//    public int TridimensionalVFXHash;
		
	//    public bool UpdateFloorTextures;
	//    public bool UpdateMonsters;
	//    public bool UpdateProps;
	//    public bool UpdateVFX;
	//    public bool Update3DVFX;
	//}

	//public class AssetContainer : MonoBehaviour
	//{
		//public static Material GetMaterial(Def.Materials material)
		//{
		//	return Mgr.Materials[(int)material];
		//}
		//[SerializeField]
		//Material[] Materials = null;

		//[SerializeField]
		//AssetHashes Hashes;

		//public readonly static int MaterialTextureID = Shader.PropertyToID("_BaseMap");
		//public readonly static int ColoredMaterialTextureID = Shader.PropertyToID("Texture2D_556CDA01");
		//public readonly static int ColoredMaterialCSHBID = Shader.PropertyToID("Vector4_34A1AF91");
		//public readonly static int BackgroundColorID = Shader.PropertyToID("ParticleColor");
		//public readonly static int MaterialColorID = Shader.PropertyToID("_BaseColor");
		//public readonly static int FlipShaderID = Shader.PropertyToID("_Flip");

		//public bool UpdateAssets;

		//public GameObject[] Bridges;
		//public Material BackgroundMaterial;
		//public Texture2D[] BackgroundTextures;
		//public Gradient[] BackgroundMistColor;

		//public Mesh[] Assets3D;
		//public Texture2D[] Asset3DTextures;
		//public Sprite[] Weapons;
		//public Texture2D[] InventoryItems;
		//public Sprite[] ItemSprites;
		//public GameObject[] UIGameObjects;
		//public Sprite[] OddVFX;
		//public Sprite[] PropVFX;
		//public Sprite[] MonsterVFX;
		//public Sprite[] GeneralVFX;
		//public Texture2D[] TridimensionalVFX;
		//public GameObject[] TriVFXGameObjects;
		//public VisualEffectAsset[] VisualEffects;

  //      [NonSerialized]
  //      public Texture2D SpriteShadowTex;
		//[NonSerialized]
		//public Sprite SpriteShadow;

		//public static AssetContainer Mgr = null;

		//private void OnValidate()
		//{
		//    if (!UpdateAssets || !Application.isEditor)
		//        return;
		//    int floorHash = FloorTextures.GetHashCode();
		//    int monsterHash = MonsterSprites.GetHashCode();
		//    int propHash = PropSprites.GetHashCode();
		//    int[] textHashes = new int[TextAssets.Length];
		//    for (int i = 0; i < textHashes.Length; ++i) textHashes[i] = TextAssets[i].text.GetHashCode();
		//    int oddVFXHash = OddVFX.GetHashCode();
		//    int propVFXHash = PropVFX.GetHashCode();
		//    int monsterVFXHash = MonsterVFX.GetHashCode();
		//    int generalVFXhash = GeneralVFX.GetHashCode();
		//    int tridimensionalVFXHash = TridimensionalVFX.GetHashCode();
			
		//    if (floorHash != Hashes.FloorTexturesHash)
		//    {
		//        Hashes.UpdateFloorTextures = true;
		//        Hashes.FloorTexturesHash = floorHash;
		//    }
		//    if (monsterHash != Hashes.MonsterSpritesHash)
		//    {
		//        Hashes.UpdateMonsters = true;
		//        Hashes.MonsterSpritesHash = monsterHash;
		//    }
		//    if (propHash != Hashes.PropSpritesHash)
		//    {
		//        Hashes.UpdateProps = true;
		//        Hashes.PropSpritesHash = propHash;
		//    }
		//    if (Hashes.TextAssetsHash == null || (Hashes.TextAssetsHash != null && Hashes.TextAssetsHash.Length != TextAssets.Length))
		//    {
		//        Hashes.TextAssetsHash = new int[TextAssets.Length];
		//        for (int i = 0; i < Hashes.TextAssetsHash.Length; ++i)
		//            Hashes.TextAssetsHash[i] = TextAssets[i].text.GetHashCode();
		//        Hashes.UpdateFloorTextures = true;
		//        Hashes.UpdateMonsters = true;
		//        Hashes.UpdateProps = true;
		//    }
		//    else
		//    {
		//        for (int i = 0; i < Hashes.TextAssetsHash.Length; ++i)
		//        {
		//            if (Hashes.TextAssetsHash[i] != TextAssets[i].text.GetHashCode())
		//            {
		//                var lowerName = TextAssets[i].name.ToLower();
		//                switch (lowerName)
		//                {
		//                    case "materiallist":
		//                        Hashes.UpdateFloorTextures = true;
		//                        break;
		//                    case "proplist":
		//                        Hashes.UpdateProps = true;
		//                        break;
		//                    case "monsterlist":
		//                        Hashes.UpdateProps = true;
		//                        break;
		//                }
		//            }
		//        }
		//    }

		//    if (oddVFXHash != Hashes.OddVFXHash)
		//    {
		//        Hashes.UpdateVFX = true;
		//        Hashes.OddVFXHash = oddVFXHash;
		//    }
		//    if (propVFXHash != Hashes.PropVFXHash)
		//    {
		//        Hashes.UpdateVFX = true;
		//        Hashes.PropVFXHash = propVFXHash;
		//    }
		//    if (monsterVFXHash != Hashes.MonsterVFXHash)
		//    {
		//        Hashes.UpdateVFX = true;
		//        Hashes.MonsterVFXHash = monsterVFXHash;
		//    }
		//    if (generalVFXhash != Hashes.GeneralVFXHash)
		//    {
		//        Hashes.UpdateVFX = true;
		//        Hashes.GeneralVFXHash = generalVFXhash;
		//    }
		//    if (tridimensionalVFXHash != Hashes.TridimensionalVFXHash)
		//    {
		//        Hashes.Update3DVFX = true;
		//        Hashes.TridimensionalVFXHash = tridimensionalVFXHash;
		//    }
		//    //UpdateAssets = false;
		//    EditorInit();
		//}

		//private void Awake()
		//{
		//	Mgr = this;
		//}

		//public void EditorInit()
		//{
		//    if(Hashes.UpdateFloorTextures)
		//    {
		//        //BlockMaterial.EditorInit(BlockMaterials, Def.Materials);
		//        Hashes.UpdateFloorTextures = false;
		//    }
		//    if(Hashes.UpdateMonsters)
		//    {
		//        //Monsters.EditorInit(MonstersInfo);
		//        Hashes.UpdateMonsters = false;
		//    }
		//    if(Hashes.UpdateProps)
		//    {
		//        //Props.EditorInit(PropsInfo);
		//        Hashes.UpdateProps = false;
		//    }
		//    if(Hashes.UpdateVFX)
		//    {
		//        //VFXs.EditorInit(VFXsInfo);
		//        Hashes.UpdateVFX = false;
		//    }
		//    if(Hashes.Update3DVFX)
		//    {
		//        //VFX3Ds.EditorInit(VFX3DsInfo);
		//        Hashes.Update3DVFX = false;
		//    }
		//}

		// Start is called before the first frame update
   //     void Start()
   //     {
   //         //if (BackgroundMistColor.Length != BackgroundTextures.Length)
   //         //    throw new Exception("Background Mist Color amount and Background Textures mismatch");
   //         SpriteShadowTex = new Texture2D(128, 128)
   //         {
   //             name = "Sprite Shadow"
   //         };
   //         Vector2Int center = new Vector2Int(SpriteShadowTex.width / 2, SpriteShadowTex.height / 2);
   //         var maxDist = Mathf.Abs(Vector2Int.Distance(center, new Vector2Int(32, 32)));
   //         //var colors = SpriteShadow.GetPixels();
   //         var colors = new Color32[SpriteShadowTex.width * SpriteShadowTex.height];
   //         for (int y = 0; y < SpriteShadowTex.height; ++y)
   //         {
   //             int yOffset = y * SpriteShadowTex.width;
   //             for (int x = 0; x < SpriteShadowTex.width; ++x)
   //             {
   //                 //var color = colors[x + yOffset];

   //                 var dist = Vector2Int.Distance(center, new Vector2Int(x, y));
   //                 dist = Mathf.Abs(dist);
   //                 float alpha;
   //                 if (dist > maxDist)
   //                 {
   //                     alpha = 0.0f;
   //                 }
   //                 else
   //                 {
   //                     dist /= maxDist;
   //                     alpha = 1.0f - dist;
   //                     alpha *= alpha;
   //                 }
   //                 if (alpha > 0.5f)
   //                     alpha = 0.5f;
   //                 colors[x + yOffset] = new Color32(0, 0, 0, (byte)Mathf.FloorToInt(alpha * 255f));
   //                 //colors[x + yOffset] = new Color(0.0f, 0.0f, 0.0f, alpha);
   //             }
   //         }
			//SpriteShadowTex.SetPixels32(colors);
			////SpriteShadow.alphaIsTransparency = true;
			//SpriteShadowTex.Apply(true, true);
			//SpriteShadow = Sprite.Create(SpriteShadowTex,
			//	new Rect(0f, 0f, SpriteShadowTex.width, SpriteShadowTex.height),
			//	new Vector2(0f, 0f), 100f, 0, SpriteMeshType.FullRect);
   //     }
//	}
//}