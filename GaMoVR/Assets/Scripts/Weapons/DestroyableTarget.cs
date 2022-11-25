using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DestroyableTarget : Health
{
    public IObjectPool<DestroyableTarget> Pool;

    public GallowsManager GallowsManager;

    public AudioSource TargetDestroyedAudioSource;

    public new void Start()
    {
        base.Start();
        _initialHealth = 10;
        m_health = _initialHealth;
    }

    public override void Hit(float damage, WeaponTypeTag weaponTypeTag)
    {
        m_health -= damage;
        if (m_health <= 0)
        {
            TargetDestroyedAudioSource?.Play();
            GallowsManager.IncreaseTimer(20);
            Pool.Release(this);
        }
    }
}
