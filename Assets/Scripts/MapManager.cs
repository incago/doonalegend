/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Newtonsoft.Json;
using INCAGO_TMX;

namespace DoonaLegend
{
    public class MapManager : MonoBehaviour
    {
        #region Variables
        private static MapManager _instance;
        public static MapManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<MapManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        public TextAsset[] unit_2;
        public TextAsset[] unit_3;
        public TextAsset[] unit_4;
        public TextAsset[] unit_5;
        public TextAsset[] tilesets;
        public List<Tileset> tilesetList = new List<Tileset>();
        private Dictionary<int, Tileset> tilesetDictionary = new Dictionary<int, Tileset>();
        private Dictionary<int, TileProperty> tileObjectDictionary = new Dictionary<int, TileProperty>();
        private Dictionary<int, List<SectionContent>> sectionContentList = new Dictionary<int, List<SectionContent>>();
        private Dictionary<int, Queue<SectionContent>> sectionContentQueue = new Dictionary<int, Queue<SectionContent>>();
        public TileProperty GetObjectPropertyByGid(int gid)
        {
            if (!tileObjectDictionary.ContainsKey(gid))
            {
                return null;
            }
            return tileObjectDictionary[gid];
        }
        #endregion

        #region Method
        void Awake()
        {
            if (_instance == null)
            {
                DontDestroyOnLoad(gameObject);
                _instance = this;
                Init();
                // TileProperty op_a = GetObjectPropertyByGid(17);
                // TileProperty op_b = GetObjectPropertyByGid(18);
                // Debug.Log(op_a.objectId);
                // Debug.Log(op_b.objectId);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Init()
        {
            tilesetList.Clear();
            tilesetDictionary.Clear();
            tileObjectDictionary.Clear();
            for (int i = 0; i < tilesets.Length; i++)
            {
                TextAsset textAsset = tilesets[i];
                Tileset tileset = JsonConvert.DeserializeObject<Tileset>(textAsset.text);
                if (tileset.tileproperties != null && tileset.tileproperties.Count > 0)
                {
                    foreach (KeyValuePair<int, TileProperty> kv in tileset.tileproperties)
                    {
                        tileObjectDictionary.Add(tileset.firstgid + kv.Key, kv.Value);
                    }
                }
                tilesetList.Add(tileset);
                tilesetDictionary.Add(i, tileset);
            }

            sectionContentList.Clear();
            sectionContentList.Add(2, GetSectionContents(unit_2, 2));
            sectionContentList.Add(3, GetSectionContents(unit_3, 3));
            sectionContentList.Add(4, GetSectionContents(unit_4, 4));
            sectionContentList.Add(5, GetSectionContents(unit_5, 5));
            sectionContentList[2].Shuffle();
            sectionContentList[3].Shuffle();
            sectionContentList[4].Shuffle();
            sectionContentList[5].Shuffle();

            sectionContentQueue.Clear();
            sectionContentQueue.Add(2, new Queue<SectionContent>(sectionContentList[2]));
            sectionContentQueue.Add(3, new Queue<SectionContent>(sectionContentList[3]));
            sectionContentQueue.Add(4, new Queue<SectionContent>(sectionContentList[4]));
            sectionContentQueue.Add(5, new Queue<SectionContent>(sectionContentList[5]));

            // SectionContent sectionContent = GetSectorData(2);
            // Debug.Log(sectionContent.fileName.ToString());
            // foreach (KeyValuePair<int, Dictionary<int, int>> kv in sectionContent.objects)
            // {
            //     int x = kv.Key;
            //     foreach (KeyValuePair<int, int> kv2 in kv.Value)
            //     {
            //         int y = kv2.Key;
            //         int gid = kv2.Value;
            //         Debug.Log(x.ToString() + "," + y.ToString() + ":" + gid.ToString());
            //     }
            // }
            // Debug.Log(sectionContent.objects.Count.ToString());
        }

        public SectionContent GetSectorContent(int unitLength)
        {
            if (sectionContentQueue[unitLength].Count == 0)
            {
                sectionContentList[unitLength].Shuffle();
                sectionContentQueue[unitLength] = new Queue<SectionContent>(sectionContentList[unitLength]);
            }

            return sectionContentQueue[unitLength].Dequeue();
        }

        public List<SectionContent> GetSectionContents(TextAsset[] textAssets, int unitLength)
        {
            List<SectionContent> returnData = new List<SectionContent>();
            foreach (TextAsset textAsset in textAssets)
            {
                TiledMap tileMap = JsonConvert.DeserializeObject<TiledMap>(textAsset.text);
                //tiledMap을 읽어서 terrain레이어는 그대로 terrains 배열에 넣고
                //object레이어의 각 오브젝트는 읽어서 해당하는 인덱스에 gid값을 집어넣는다
                int[,] terrains = new int[3, 3 * unitLength];
                // Dictionary<int, Dictionary<int, int>> objects = new Dictionary<int, Dictionary<int, int>>();
                Dictionary<Node, TiledObject> objects = new Dictionary<Node, TiledObject>();
                foreach (Layer layer in tileMap.layers)
                {
                    if (layer.name.Equals("terrain"))
                    {
                        // for (int i = 0; i < terrains.GetLength(0); i++)
                        // for (int i = terrains.GetLength(0) - 1; i >= 0; i--)
                        // {
                        //     for (int j = 0; j < terrains.GetLength(1); j++)
                        //     {
                        //         terrains[i, j] = layer.data[(terrains.GetLength(0) - 1 - i) * unitLength * 3 + j];
                        //     }
                        // }
                        for (int i = 0; i < layer.data.Count; i++)
                        {
                            int y = i % terrains.GetLength(1);
                            int x = terrains.GetLength(0) - (i / terrains.GetLength(1)) - 1;
                            terrains[x, y] = layer.data[i];
                        }
                    }
                    else if (layer.name.Equals("object"))
                    {
                        // Debug.Log("layer.objects.Count: " + layer.objects.Count.ToString());
                        foreach (TiledObject tiledObject in layer.objects)
                        {
                            int y = tiledObject.x / 24;
                            int x = (24 * 3 - tiledObject.y) / 24;

                            // Direction direction = Direction.up;
                            // if (tiledObject.properties != null)
                            // {
                            //     direction = tiledObject.properties.direction;
                            // }
                            Node node = new Node(x, y);
                            if (!objects.ContainsKey(node)) objects.Add(node, tiledObject);

                            // if (!objects.ContainsKey(x)) objects.Add(x, new Dictionary<int, int>());
                            // objects[x].Add(y, tiledObject.gid);
                        }
                    }
                }
                SectionContent sectionContent = new SectionContent(unitLength, textAsset.name, terrains, objects);
                returnData.Add(sectionContent);
            }

            return returnData;
        }

        #endregion
    }

    public class SectionContent
    {
        public int unitLength;
        public string fileName;
        public int[,] terrains;
        public Dictionary<Node, TiledObject> objects;

        public SectionContent(int unitLength, string fileName, int[,] terrains, Dictionary<Node, TiledObject> objects)
        {
            this.unitLength = unitLength;
            this.fileName = fileName;
            this.terrains = terrains;

            this.objects = objects;
        }
    }

    //tileset_terrain과 매칭된다.
    public enum TerrainGid
    {
        empty = 1,
        basic_grass = 2,
        basic_dirt = 3,
        water = 4,
        ice = 5,
        cracked = 6,
        magma = 7
    }

    public enum ItemGid
    {
        coin_yellow = 17,
        food_apple = 18,
        potion_sp = 19,
        heart = 20,
        coin_blue = 21,
        coin_red = 25
    }

    public enum TrapGid
    {
        thornfloor = 33,
        axemachine = 34,
        blademachine = 35,
        jawmachine = 36,
        punchmachine = 37,
        crossbow = 38,
        flamemachine = 39
    }

    public enum EnemyGid
    {
        slime = 49
    }
}