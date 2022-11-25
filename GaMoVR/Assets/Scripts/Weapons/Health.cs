using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    protected float _initialHealth = 100;

    protected float m_health;

    public float GetHealth
    {
        get => m_health;
    }

    public void Start()
    {
        m_health = _initialHealth;
    }

    public virtual void Hit(float damage, WeaponTypeTag weaponTypeTag)
    {
        m_health -= damage;
    }

    public void ResetHealth()
    {
        m_health = _initialHealth;
    }
}
