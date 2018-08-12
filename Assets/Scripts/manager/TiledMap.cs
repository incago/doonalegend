using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using DoonaLegend;

namespace INCAGO_TMX
{

    [Serializable]
    public class TiledMap
    {
        public int height;
        public int width;
        public int tilewidth;
        public int tileheight;
        public int version;
        public Orientation orientation;
        public int nextobjectid;
        public List<Layer> layers;
        public List<TilesetRef> tilesets;
    }

    [Serializable]
    public class TilesetRef
    {
        public int firstgid;
        public string source;
    }

    [Serializable]
    public class Tileset
    {
        public int firstgid;
        public int columns;
        public string image;
        public int imageheight;
        public int imagewidth;
        public int margin;
        public string name;
        public int spacing;
        public int tilecount;
        public int tileheight;
        public int tilewidth;
        public Dictionary<int, TileProperty> tileproperties;
        // public Dictionary<int, TileProperty> tilepropertytypes;
    }

    [Serializable]
    public class Layer
    {
        public string name;
        public int width;
        public int height;
        public int layerdepth;
        public bool visible;
        public float opacity;
        public LayerType type;

        public List<int> data;
        public List<TiledObject> objects;
    }

    [Serializable]
    public class TiledObject
    {
        public int gid;
        public int id;
        public int height;
        public int width;
        public bool visible;
        public int x;
        public int y;
        public TileProperty properties;
        public int index
        {
            get
            {
                int gridX = x / 12;
                int gridY = y / 12 - 1;
                return gridY * 10 + gridX;
            }
        }
    }

    [Serializable]
    public class TileProperty
    {
        public string objectId;
        public string objectType; //thingType이 될수도 unitType이 될수도. 상황에 맞추어 특정타입으로 파싱하여 사용한다.
        public string itemType;
        public float value;
        public bool isDead;//for test
    }

    public enum Orientation : byte
    {
        orthogonal, isometric, staggered
    }

    public enum LayerType : byte
    {
        tilelayer, objectgroup
    }
}