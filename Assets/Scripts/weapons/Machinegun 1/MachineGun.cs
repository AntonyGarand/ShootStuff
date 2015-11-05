using UnityEngine;
using System.Collections;

public class MachineGun : Weapon {

    [Range(0,1)]
    public float shootAngleVariation;
    public float bulletDamage;
    public float bulletLifetime;

    protected override void Shoot(){
        if (Time.time > nextShotTime)
        {
            muzzleFlash.Activate();
            foreach (Transform muzzle in projectileSpawn)
            {
                Projectile newProjectile = Instantiate(projectile, muzzle.position, Quaternion.Euler(muzzle.rotation.eulerAngles.x, Random.Range(-shootAngleVariation * 360, shootAngleVariation * 360) + muzzle.rotation.eulerAngles.y, muzzle.rotation.eulerAngles.z)) as Projectile;
                newProjectile.setSpeed(bulletSpeed);
                newProjectile.damage = bulletDamage;
                newProjectile.lifeTime = bulletLifetime;
                newProjectile.setSpeed(base.bulletSpeed);
            }
            playSound();
            nextShotTime = Time.time + msBetweenShots / 1000;
            Instantiate(shell, shellEjector.position, shellEjector.rotation);
        }
    }

}
