using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;

    public Transform shell;
    public Transform shellEjector;

    protected MuzzleFlash muzzleFlash;

    protected float nextShotTime;

    public virtual void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    public virtual void Shoot(){
        if (Time.time > nextShotTime)
        {
            muzzleFlash.Activate();
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.setSpeed(muzzleVelocity);
            nextShotTime = Time.time + msBetweenShots / 1000;
            Instantiate(shell, shellEjector.position, shellEjector.rotation);
        }
    }

}
