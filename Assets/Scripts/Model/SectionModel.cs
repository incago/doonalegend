/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System;
using System.Collections;
using System.Collections.Generic;
using INCAGO_TMX;

namespace DoonaLegend
{
    [Serializable]
    public class SectionModel
    {
        public int sectionId;
        public SectionType sectionType;
        public Direction direction; //right, up
        public Node origin;
        public int width, height;
        public int[,] terrains;
        // public int[,] objects;
        public Dictionary<Node, TiledObject> objects;

        public SectionModel(int sectionId, SectionType sectionType, Direction direction, Node origin, int width, int height,
        int[,] terrains = null,
        //int[,] objects = null,
        Dictionary<Node, TiledObject> objects = null)
        {
            this.sectionId = sectionId;
            this.sectionType = sectionType;
            this.direction = direction;
            this.origin = origin;
            this.width = width;
            this.height = height;

            this.terrains = terrains;
            // this.objects = objects;
            this.objects = objects;
        }
    }

    public enum SectionType
    {
        straight = 0, corner = 1
    }
}
