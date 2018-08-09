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
        public ObjectProperty properties;
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
    public class ObjectProperty
    {
        public string objectId;
        public string objectType; //thingType이 될수도 unitType이 될수도. 상황에 맞추어 특정타입으로 파싱하여 사용한다.
        public Direction direction;
        public string color;
        public string contents;
        public int quantity;
        // public int durability;
        public string itemOption;

        public string title;
        public string message;

        public bool isObstacle;
        public bool isDestroyed;
        public bool isOn;
        public bool isLocked;
        public bool isBoss;
        public bool isOpponentUnit;

        public string camp;
        public bool isDead;
        public bool isRevived;
        public int life; //이것은 current health를 의미한다.
        public int level; //타일드를 이용하여 유닛을 생성할때 레벨정보가 된다.
        public float delay;
        // public int coin;
        public int exp;
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