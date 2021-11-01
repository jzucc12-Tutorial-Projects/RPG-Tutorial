using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwitcher : MonoBehaviour
{
    [SerializeField] GameObject entryPoint = null;
    
    private void Start() 
    {
        SwitchTo(entryPoint);
    }

    public void SwitchTo(GameObject display)
    {
        if(display.transform.parent != transform) return;

        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(child.gameObject == display);
        }
    }
}
