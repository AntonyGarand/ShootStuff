using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// BUG LIST: 
/// 1. Laser won't aim directly at the mouse. Related to the player rotation script
/// 2. When killing an ennemy, the blood will explode from the aiming direction, not the laser position. 
/// </summary>

public class Laser : Weapon {

    public float damage;

    public float laserLength;
    public Color laserColor;
    public float laserStartWidth;
    public float laserEndWidth;
    public float laserChargeTime;
    public float laserCooldown;

    //LineRenderer laser;

    bool isShooting;
    int obstacleMask;
    int enemyMask;

    public override void Start()
    {
        base.Start();
        obstacleMask = (int)Mathf.Pow(2, LayerMask.NameToLayer("Obstacle"));
        enemyMask = (int)Mathf.Pow(2, LayerMask.NameToLayer("Enemy"));
        isShooting = false;
    }

    protected override void Shoot() {
        if (!isShooting && Time.time > nextShotTime)
        {
            isShooting = true;
            //Failsafe stopcoroutine, although there was one case where the beam didn't dissapear. Potential cause?
            StopCoroutine("FireLaser");
            StartCoroutine("FireLaser");
        }
        /*if (Time.time > nextShotTime)
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
        }*/
    }

    public override void OnTriggerHold()
    {
        base.OnTriggerHold();

    }
    public override void OnTriggerRelease()
    {
        isShooting = false;
        base.OnTriggerRelease();
    }

    // First plan: Create a single linerenderer with X vertex
    //     Failed as the linerenderer doens't line angles of less than 90 degrees, which made the tip of the lasers way too slim.
    // Fix: Create a list of gameobject with a line renderer per object.
    IEnumerator FireLaser()
    {
        float startTime = Time.time;
        float percentDone = 0;

        GameObject[] lasers = new GameObject[0];
        Material laserMaterial = new Material(Shader.Find("Particles/Additive"));
        bool shootNow = false;

        while (isShooting && !shootNow)
        {
            //Destroying the previous objects
            for (int i = 0; i < lasers.Length; i++) Destroy(lasers[i]);

            List<Vector3> positions = new List<Vector3>();
            positions.Add(projectileSpawn[0].transform.position);

            percentDone = (Time.time - startTime) / laserChargeTime;
            shootNow = percentDone >= 1;
            if (shootNow) { 
            nextShotTime = Time.time + laserCooldown;
            }

            reflect(30f, projectileSpawn[0].transform.position, projectileSpawn[0].transform.forward, ref positions, shootNow);
            Vector3[] coords = positions.ToArray();

            lasers = new GameObject[coords.Length];

            print(shootNow);
            float width = Mathf.Lerp(laserStartWidth, laserEndWidth, percentDone);
            Vector3 lastCoord = coords[0];

            for (int i = 1; i < coords.Length; i++)
            {
                GameObject lineRendererObject = new GameObject("Line");
                lasers[i] = lineRendererObject;
                lineRendererObject.transform.parent = transform;
                LineRenderer lRendLaser = lineRendererObject.AddComponent<LineRenderer>();
                lRendLaser.SetVertexCount(2);
                lRendLaser.SetPosition(0, lastCoord);
                lRendLaser.SetPosition(1, coords[i]);
                lRendLaser.SetColors(laserColor, laserColor);
                lRendLaser.material = laserMaterial;
                lastCoord = coords[i];
                lRendLaser.SetWidth(width, width);
            }
            yield return null;
        }
        //Destroy the final lasers
        for (int i = 0; i < lasers.Length; i++) Destroy(lasers[i]);
    }

    void reflect(float distanceRemaining, Vector3 position, Vector3 direction, ref List<Vector3> positions, bool hitTargets)
    {
        int mask = obstacleMask ^ (hitTargets ? enemyMask : 0);
        bool hitEnemy = false;
        float colliderDist = 0f;
        //Potential upgrade: 
        //    Use a break when hitting a wall instead of the recursive from inside the loop. 
        do
        {
            //0.01f is the padding required to prevent an infinite loop when shooting, which currently appear at "random" times.
            //Problem: Stacked ennemies won't die, shouldn't be a problem if they move though.
            //TODO: Investigate the infinite loop
            position = hitEnemy ? position + (direction * 0.01f) : position;
            Ray ray = new Ray(position, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, distanceRemaining, mask))
            {
                //Checks if the collider's layer is contained in the enemy layer
                int colliderLayer = (int)Mathf.Pow(2, (hit.collider.gameObject.layer));
                hitEnemy = (colliderLayer & enemyMask) == enemyMask;

                //If we hit a wall, reflect and return
                if (!hitEnemy)
                {
                    float newDist = distanceRemaining - hit.distance;
                    Vector3 newDirection = Vector3.Reflect(direction, hit.normal);
                    positions.Add(hit.point);
                    reflect(newDist, hit.point, newDirection, ref positions, hitTargets);
                    return;
                }
                //If we hit an enemy, damage it and keep going
                else
                {
                    position = hit.point;
                    damageEnemy(hit.collider, hit.point);
                }
            }
            else
            {
                positions.Add(ray.GetPoint(distanceRemaining));
                hitEnemy = false;
            }
            //Loop until you find something that is NOT an enemy (An obstacle)
        } while (hitEnemy); 
    }

    void damageEnemy(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.Takehit(damage, hitPoint, transform.forward);
        }
    }

}
