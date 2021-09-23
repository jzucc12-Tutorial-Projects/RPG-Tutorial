using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent OnCollide;
    GameObject player;

    private void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");    
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject != player) return;
        OnCollide?.Invoke();
    }
}
