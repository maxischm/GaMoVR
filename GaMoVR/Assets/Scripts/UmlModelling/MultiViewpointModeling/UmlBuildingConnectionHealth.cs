using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmlBuildingConnectionHealth : Health
{
    private WeaponTypeTag acceptedWeaponType = WeaponTypeTag.Sword;

    public new void Start()
    {
        base.Start();
        _initialHealth = 10;
        m_health = _initialHealth;
    }

    public override void Hit(float damage, WeaponTypeTag weaponTypeTag)
    {
        if (weaponTypeTag == acceptedWeaponType)
        {
            base.Hit(damage, weaponTypeTag);
            if (m_health <= 0)
            {
                UmlModel.Instance.RemoveConnectionInUml(gameObject);
            }
        }
    }
}
