using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour
{
    public AudioClip shootSound;
    AudioSource soundLocation;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;

    public Transform shell;
    public Transform shellEjector;

    protected MuzzleFlash muzzleFlash;

    protected float nextShotTime;
    protected bool triggerHasBeenReleased;

    public virtual void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    protected abstract void Shoot();

    public virtual void OnTriggerHold()
    {
        Shoot();
        triggerHasBeenReleased = false;
    }

    public virtual void OnTriggerRelease()
    {
        triggerHasBeenReleased = true;
    }

}
