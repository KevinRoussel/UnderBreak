﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingManager : MonoBehaviour {

    List<BaseProjectile> _pool;

    [Header("Projectiles pool variables")]
    [Tooltip("Prefab of the projectile to pool")]
    [SerializeField] BaseProjectile _projectile;

    [SerializeField] Transform _bulletRoot;

    [Tooltip("Amount of projectiles to instantiate at the start")]
    [SerializeField] int _initialAmount;

    [Tooltip("If true, the pool can be expanded to fit more than its initial amount")]
    [SerializeField] bool _canExpand;

    void Start () {

        _pool = new List<BaseProjectile>();
        StartCoroutine(StartPool());
        
    }

    IEnumerator StartPool () {
        for (int i = 0; i < _initialAmount; i++) {
            AddProjectileToPool();
            yield return new WaitForEndOfFrame();
        }        
    }

    public BaseProjectile Shoot (Transform shootingTransform, string targetTag, Vector2 spreadRange, int bulletPower, float rightOffset) {

        BaseProjectile projectile = null;

        foreach (BaseProjectile p in _pool) {
            if (p != null && !p.gameObject.activeSelf) {
                projectile = p;
                break;
            }
        }

        if(!projectile && _canExpand)
            projectile = AddProjectileToPool();

        if(projectile)
        {
            Vector3 spreadDir = Quaternion.Euler(0, UnityEngine.Random.Range(spreadRange.x, spreadRange.y), 0) * shootingTransform.forward + shootingTransform.right * rightOffset;
            projectile.Power = bulletPower;
            projectile.transform.SetPositionAndRotation(shootingTransform.position, Quaternion.LookRotation(spreadDir));
            projectile.TargetTag = targetTag;
            projectile.Instigator = shootingTransform.GetComponentInParent<Character>();
            projectile.gameObject.SetActive(true);
        }

        return projectile;

    }

    BaseProjectile AddProjectileToPool() {

        BaseProjectile projectile = Instantiate(_projectile, _bulletRoot);
        _pool.Add(projectile);
        return projectile;

    }

    internal void UpdateBullets()
    {
        foreach (BaseProjectile p in _pool)
            p.transform.position += p.transform.forward * p.Speed * Time.deltaTime;
    }

}
