﻿using UnityEngine;
using System.Collections;
using System;

public class machinegun : Weapon {

	public enum FireMode { auto, burst, single }
    public FireMode firemode;

    public int burstCount;
    public float msBetweenBurst;
    public float msBetweenBurstShots;

    int currentShotsFired;

    protected override void Shoot()
    {
        
        if (Time.time > nextShotTime)
        {
            switch (firemode)
            {
                case FireMode.auto:
                    shootBullet();
                    nextShotTime = Time.time + msBetweenShots / 1000;
                    AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position);
                    break;
                case FireMode.burst:
                    if(currentShotsFired >= burstCount)
                    {
                        nextShotTime = Time.time + msBetweenBurst / 1000;
                        currentShotsFired = 0;
                    } else {
                        nextShotTime = Time.time + msBetweenBurstShots / 1000;
                        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position);
                        currentShotsFired++;
                        shootBullet();
                    }
                    break;
                case FireMode.single:
                    if (triggerHasBeenReleased)
                    {
                        shootBullet();
                        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position);
                        nextShotTime = Time.time + msBetweenShots / 1000;
                    }
                    break;
            }
            
        }
    }

    void shootBullet()
    {
        muzzleFlash.Activate();
        foreach (Transform muzzle in projectileSpawn)
        {
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.setSpeed(muzzleVelocity);
        }
        Instantiate(shell, shellEjector.position, shellEjector.rotation);
    }

    public override void OnTriggerRelease()
    {
        if(firemode == FireMode.burst)
        {
            nextShotTime = Time.time + msBetweenBurstShots / 1000;
            currentShotsFired = 0;
        }
        base.OnTriggerRelease();
    }

}
