using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selfdestroy : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 2);
    }
}
