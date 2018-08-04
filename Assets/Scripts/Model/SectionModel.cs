/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System;

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

        public SectionModel(int sectionId, SectionType sectionType, Direction direction, Node origin, int width, int height)
        {
            this.sectionId = sectionId;
            this.sectionType = sectionType;
            this.direction = direction;
            this.origin = origin;
            this.width = width;
            this.height = height;
        }
    }

    public enum SectionType
    {
        straight = 0, corner = 1
    }
}
