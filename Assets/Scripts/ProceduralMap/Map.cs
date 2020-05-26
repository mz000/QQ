﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum MapType
    {
        BIG,
        SMALL,
        DEADEND
    }

    public int width, height;
    public GameObject MapOBJ;

    public List<Entry> entries;

    public bool hasLeft, hasRight, hasUp, hasDown;

    public MapType type;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(width / 2, height / 2, 0), new Vector3(width, height, 0));
    }

    // disable if invisible

}
