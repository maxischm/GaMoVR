using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeaponTypeTag
{
    Gun,
    Sword
}

public class UmlElementHealth : Health
{
    public WeaponTypeTag acceptedWeaponType;

    // Start is called before the first frame update
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
                if (UmlHangmanLevelValidator.Instance.ValidateObjectDestruction(gameObject))
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
