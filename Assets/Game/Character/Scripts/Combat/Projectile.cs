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
    Vector3 targetPoint;

    void Update()
    {
        MoveProjectile();
    }

    private void MoveProjectile()
    {
        if(target != null && isHoming && target.IsAlive())
            transform.LookAt(GetAimLocation());



        transform.Translate(Vector3.forward*speed*Time.deltaTime);
    }

    private Vector3 GetAimLocation()
    {
        if(target == null)
        {
            return targetPoint;
        }
        CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
        if (targetCapsule == null) return target.transform.position;
        return target.transform.position + Vector3.up * targetCapsule.height / 2;
    }

    public void SetTarget(Health target, GameObject instigator, float damage)
    {
        SetTarget(instigator,damage,target);
    }

    public void SetTarget(Vector3 targetPoint, GameObject instigator, float damage)
    {
        SetTarget(instigator, damage, null, targetPoint);
    }

    public void SetTarget(GameObject instigator, float damage, Health target = null, Vector3 targetPoint = default)
    {
        this.damage = damage;
        this.targetPoint = targetPoint;
        this.target = target;
        this.instigator = instigator;
        transform.LookAt(GetAimLocation());
        Destroy(gameObject, maxLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (target != null && health != target) return;
        if (health == null || !health.IsAlive()) return;
        if (other.gameObject == instigator) return;

        speed = 0;
        OnHit?.Invoke();
        health.TakeDamage(instigator, damage);
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
