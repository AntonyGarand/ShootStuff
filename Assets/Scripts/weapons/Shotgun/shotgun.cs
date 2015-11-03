using UnityEngine;
using System.Collections;

public class shotgun : Weapon {

    public int projectileCount;
    [Range(0,1)]
    public float shootAngleVariation;
    public float bulletSpeed;
    public float bulletDamage;
    public float bulletLifetime;


    protected override void Shoot(){
        if (Time.time > nextShotTime)
        {
            muzzleFlash.Activate();
            foreach (Transform muzzle in projectileSpawn)
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    Projectile newProjectile = Instantiate(projectile, muzzle.position, Quaternion.Euler(muzzle.rotation.eulerAngles.x, Random.Range(-shootAngleVariation * 360, shootAngleVariation * 360) + muzzle.rotation.eulerAngles.y, muzzle.rotation.eulerAngles.z)) as Projectile;
                    newProjectile.speed = bulletSpeed;
                    newProjectile.damage = bulletDamage;
                    newProjectile.lifeTime = bulletLifetime;
                    newProjectile.setSpeed(muzzleVelocity);
                }
            }
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position);
            nextShotTime = Time.time + msBetweenShots / 1000;
            Instantiate(shell, shellEjector.position, shellEjector.rotation);
        }
    }

}
