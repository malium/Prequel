/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets
//{
//    public enum MapGeneration
//    {
//        SQUARE,
//        CIRCLE,

//    }

//    public class Map
//    {
//        public static Vector2Int DefaultMapSize = new Vector2Int(16, 16);
//        public static Vector2Int MapSize = DefaultMapSize;
//        public static Vector2 DefaultTileSize = new Vector2(1.0f, 1.0f);
//        public static Vector2 MapTileSize = DefaultTileSize;
        
//        private GameObject MapGO;
//        public GameObject GO
//        {
//            get { return MapGO; }
//        }

//        private List<Tile> MapTiles;
//        public List<Tile> Tiles
//        {
//            get { return MapTiles; }
//        }

//        public Map(Vector3 position)
//        {
//            MapGO = new GameObject($"map{(int)position.x}x{(int)position.y}y{(int)position.z}z");
//            MapGO.transform.Translate(position);
//            MapTiles = new List<Tile>();
//            for (int y = 0; y < MapSize.y; ++y)
//            {
//                for (int x = 0; x < MapSize.x; ++x)
//                {
//                    MapTiles.Add(new Tile(this, new Vector3(x * MapTileSize.x, 0.0f, y * MapTileSize.y), TileType.NORMAL, 1.0f));
//                }
//            }
//        }

//        private float Gaussain2D(Vector2 pos, Vector2 mean, Vector2 sigma)
//        {
//            float a = 1.0f / (2.0f * Mathf.PI * Mathf.Pow(sigma.x, 2.0f) * Mathf.Pow(sigma.y, 2.0f));
//            float b = Mathf.Exp(-Mathf.Pow(pos.x - mean.x, 2.0f)/(2.0f*Mathf.Pow(sigma.x, 2.0f)));
//            float c = Mathf.Exp(-Mathf.Pow(pos.y - mean.y, 2.0f) / (2.0f * Mathf.Pow(sigma.y, 2.0f)));
//            return a * b * c;
//        }

//        public void ClearMap()
//        {
//            for(int i = 0; i < MapTiles.Count; ++i)
//            {
//                MapTiles[i].SetVisible(true);
//            }
//        }

//        private TileType GetType(bool random)
//        {
//            if (!random)
//                return TileType.NORMAL;

//            var prob = UnityEngine.Random.Range(0.0f, 1.0f);
//            if (prob > (1.0f / 8.0f))
//                return TileType.NORMAL;
//            else
//                return TileType.WIDE;
//        }
        

//        public void Generate(MapGeneration generation, int seed, Vector2 mean, Vector2 sigma, bool randomInGeneration)
//        {
//            UnityEngine.Random.InitState(seed);
//            if (generation == MapGeneration.SQUARE)
//            {
//                int left = (int)(mean.x - sigma.x * 0.5f);
//                int right = (int)(mean.x + sigma.x * 0.5f);
//                int top = (int)(mean.y - sigma.y * 0.5f);
//                int bottom = (int)(mean.y + sigma.y * 0.5f);
//                if(left > right)
//                {
//                    var tmp = left;
//                    left = right;
//                    right = tmp;
//                }
//                if(top > bottom)
//                {
//                    var tmp = top;
//                    top = bottom;
//                    bottom = tmp;
//                }

//                if (left < 0)
//                    left = 0;
//                if (top < 0)
//                    top = 0;

//                if (right >= MapSize.x)
//                    right = MapSize.x - 1;

//                if (bottom >= MapSize.y)
//                    bottom = MapSize.y - 1;

//                for (int y = 0; y < top; ++y)
//                {
//                    for (int x = 0; x < left; ++x)
//                    {
//                        MapTiles[MapUtils.GetIDX(x, y, DefaultMapSize.x)].SetVisible(false);
//                    }
//                }
//                for (int y = bottom; y < DefaultMapSize.y; ++y)
//                {
//                    for (int x = right; x < DefaultMapSize.x; ++x)
//                    {
//                        MapTiles[MapUtils.GetIDX(x, y, DefaultMapSize.x)].SetVisible(false);
//                    }
//                }
//                for (int y = top; y < bottom; ++y)
//                {
//                    for (int x = left; x < right; ++x)
//                    {
//                        var idx = MapUtils.GetIDX(x, y, DefaultMapSize.x);
//                        if (!MapTiles[idx].GetVisible())
//                            continue;

//                        bool visible = randomInGeneration ? UnityEngine.Random.Range(0.0f, 1.0f) > 0.3f : true;

//                        TileType type = TileType.NORMAL;
//                        if ((x < (MapSize.x - 1) && y != 0) && visible)
//                            type = GetType(false);

//                        if (type == TileType.WIDE)
//                        {
//                            MapTiles[MapUtils.GetIDX(x - 1, y, DefaultMapSize.x)].SetVisible(false);
//                            MapTiles[MapUtils.GetIDX(x, y - 1, DefaultMapSize.x)].SetVisible(false);
//                            MapTiles[MapUtils.GetIDX(x - 1, y - 1, DefaultMapSize.x)].SetVisible(false);
//                        }

//                        MapTiles[idx].SetVisible(false);
//                        MapTiles[idx].SetType(type);
//                        MapTiles[idx].SetVisible(visible);
//                    }
//                }
//            }
//            else if(generation == MapGeneration.CIRCLE)
//            {
                
//                float[] gauss = new float[MapSize.x * MapSize.y];
//                for (int y = 0; y < DefaultMapSize.y; ++y)
//                {
//                    for (int x = 0; x < DefaultMapSize.x; ++x)
//                    {
//                        gauss[x + y * DefaultMapSize.x] = Gaussain2D(new Vector2(x, y), mean, sigma);
//                    }
//                }
//                var centerProb = gauss[(int)mean.x + (int)mean.y * MapSize.y];
//                float mult = 0.988f / centerProb;
//                for (int i = 0; i < DefaultMapSize.x * DefaultMapSize.y; ++i)
//                    gauss[i] = gauss[i] * mult;

//                for (int y = 0; y < MapSize.y; ++y)
//                {
//                    for (int x = 0; x < MapSize.x; ++x)
//                    {
//                        var idx = MapUtils.GetIDX(x, y, DefaultMapSize.x);
//                        if (!MapTiles[idx].GetVisible())
//                            continue;

//                        float pDiscard = 0.0f;
//                        if (randomInGeneration)
//                            pDiscard = UnityEngine.Random.Range(0.0f, 1.0f);
//                        else
//                            pDiscard = 0.3f;
//                        float prob = gauss[x + y * MapSize.x];

//                        bool visible = prob > pDiscard ? true : false;

//                        TileType type = TileType.NORMAL;
//                        if ((x < (MapSize.x - 1) && y != 0) && visible)
//                            type = GetType(false);

//                        if (type == TileType.WIDE)
//                        {
//                            MapTiles[MapUtils.GetIDX(x - 1, y, DefaultMapSize.x)].SetVisible(false);
//                            MapTiles[MapUtils.GetIDX(x, y - 1, DefaultMapSize.x)].SetVisible(false);
//                            MapTiles[MapUtils.GetIDX(x - 1, y - 1, DefaultMapSize.x)].SetVisible(false);
//                        }

//                        MapTiles[idx].SetVisible(false);
//                        MapTiles[idx].SetType(type);
//                        MapTiles[idx].SetVisible(visible);
//                    }
//                }
//            }
//        }
//    }
//}
