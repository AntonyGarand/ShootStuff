using UnityEngine;
using System.Collections;

public class Sniper: Weapon {

    public float bulletDamage;
    public float bulletLifetime;


    protected override void Shoot(){
        if (Time.time > nextShotTime)
        {
            muzzleFlash.Activate();
            foreach (Transform muzzle in projectileSpawn)
            {
                Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
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
