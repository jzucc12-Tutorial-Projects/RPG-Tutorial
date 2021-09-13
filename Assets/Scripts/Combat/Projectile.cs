using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 0.1f;
    [SerializeField] float damage = 0f;
    Health target = null;
    [SerializeField] bool isHoming = false;
    [SerializeField] GameObject hitEffect = null;
    [SerializeField] float maxLifetime = 4f;
    [SerializeField] UnityEvent OnHit = null;


    GameObject instigator;

    void Update()
    {
        MoveProjectile();
    }

    private void MoveProjectile()
    {
        if (target == null) return;
        if(isHoming && target.IsAlive())
            transform.LookAt(GetAimLocation());
        transform.Translate(Vector3.forward*speed*Time.deltaTime);
    }

    private Vector3 GetAimLocation()
    {
        CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
        if (targetCapsule == null) return target.transform.position;
        return target.transform.position + Vector3.up * targetCapsule.height / 2;
    }

    public void SetTarget(Health target, GameObject instigator, float damage)
    {
        this.damage = damage;
        this.target = target;
        this.instigator = instigator;
        transform.LookAt(GetAimLocation());
        Destroy(gameObject, maxLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health collidedWith = other.GetComponent<Health>();
        if (collidedWith == null || collidedWith != target || !target.IsAlive()) return;
        speed = 0;
        OnHit?.Invoke();
        collidedWith.TakeDamage(instigator,damage);
        if (hitEffect != null)
            Instantiate(hitEffect, GetAimLocation(), transform.rotation);
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
