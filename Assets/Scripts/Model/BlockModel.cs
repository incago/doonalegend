/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System;

namespace DoonaLegend
{
    [Serializable]
    public class BlockModel
    {
        public int blockId;
        public BlockType blockType;
        public Direction direction;
        public Node origin;
        public int progress;

        public BlockModel(int blockId, BlockType blockType, Direction direction, Node origin, int progress)
        {
            this.blockId = blockId;
            this.blockType = blockType;
            this.direction = direction;
            this.origin = origin;
            this.progress = progress;
        }
    }

    public enum BlockType
    {
        straight = 0, corner = 1, shortcut_start = 2, shortcut_end = 3, edge = 4
    }
}