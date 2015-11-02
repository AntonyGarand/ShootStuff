using UnityEngine;
using System.Collections;

public class shotgun : Weapon {

    public int projectileCount;
    [Range(0,1)]
    public float shootAngleVariation;
    
    public override void Shoot(){
        print(Time.time + " " + nextShotTime);
        if (Time.time > nextShotTime)
        {
            muzzleFlash.Activate();
            for (int i = 0; i < projectileCount; i++)
            {
                Projectile newProjectile = Instantiate(projectile, muzzle.position, Quaternion.Euler(muzzle.rotation.eulerAngles.x,Random.Range(-shootAngleVariation * 360, shootAngleVariation * 360) + muzzle.rotation.eulerAngles.y,muzzle.rotation.eulerAngles.z)) as Projectile;
                newProjectile.setSpeed(muzzleVelocity);
            }
            nextShotTime = Time.time + msBetweenShots / 1000;
            Instantiate(shell, shellEjector.position, shellEjector.rotation);
        }
    }

}
