﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGrab : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "gun")
        {
            print("Trigger");
        }
    }
}
