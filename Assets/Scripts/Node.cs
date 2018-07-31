/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System;
using UnityEngine;

namespace DoonaLegend
{
    [Serializable]
    public struct Node
    {
        public int x;
        public int y;

        public Node(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public string ToString()
        {
            return "(" + x.ToString() + "," + y.ToString() + ")";
        }

        public static bool operator ==(Node a, Node b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Node a, Node b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static Node operator +(Node a, Node b)
        {
            return new Node(a.x + b.x, a.y + b.y);
        }

        public static Node operator -(Node a, Node b)
        {
            return new Node(a.x - b.x, a.y - b.y);
        }
    }
}