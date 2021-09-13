using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] GameObject go = null;

    public void DestroyObject()
    {
        Destroy(go);
    }
}
