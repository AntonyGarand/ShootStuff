using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

    public Transform weaponHold;
    public Weapon startingWeapon;
    Weapon equippedWeapon;

    void Start()
    {
        if(startingWeapon != null) {
            EquipGun(startingWeapon);
        }
    }

    public void EquipGun(Weapon weaponToEquip)
    {
        if (equippedWeapon != null)
        {
            Destroy(equippedWeapon.gameObject);
        }
        equippedWeapon = Instantiate(weaponToEquip, weaponHold.position, weaponHold.rotation) as Weapon;
        equippedWeapon.transform.parent = weaponHold;
    }
    public void Shoot()
    {
        if(equippedWeapon != null)
        {
            equippedWeapon.Shoot();
        }
    }
}
