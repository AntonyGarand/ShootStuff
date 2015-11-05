using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Laser: Weapon {

    public float bulletDamage;
    public float bulletLifetime;
    public float startWidth;
    public float endWidth;
    public float chargeTime;

    LineRenderer laser;

    bool isShooting;
    int obstacleMask;

    public override void Start()
    {
        base.Start();
        laser = GetComponentsInChildren<LineRenderer>()[0];
        obstacleMask = (int)Mathf.Pow(2, LayerMask.NameToLayer("Obstacle"));
        isShooting = false;
    }
    protected override void Shoot(){
        if (!isShooting)
        {
            isShooting = true;
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
        base.OnTriggerRelease();
        StopCoroutine("FireLaser");
        isShooting = false;
        if (laser.enabled)
        {
            laser.enabled = false;
        }

    }

    IEnumerator FireLaser()
    {

        laser.enabled = true;
        float startTime = Time.time;
        float percentDone = 0;

        while (isShooting)
        {
            List<Vector3> positions = new List<Vector3>();
            positions.Add(projectileSpawn[0].transform.position);
            reflect(30f, projectileSpawn[0].transform.position, projectileSpawn[0].transform.forward, ref positions);
            Vector3[] coords = positions.ToArray();
            print(coords.Length);
            laser.SetVertexCount(coords.Length);

            percentDone = (Time.time - startTime) / chargeTime;
            
            float width = Mathf.Lerp(startWidth, endWidth, percentDone);
            for (int i = 0; i < coords.Length; i++)
            {
                laser.SetPosition(i, coords[i]);
                laser.SetWidth(width, width);
                print("Position: " + coords[i]);
            }
            yield return null;///* null;*/ new WaitForSeconds(0.05f);
        }

        laser.enabled = false;

    }

    void reflect(float distanceRemaining, Vector3 position, Vector3 direction, ref List<Vector3> positions)
    {

        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        print(distanceRemaining);
        if(Physics.Raycast(ray, out hit, distanceRemaining, obstacleMask)){
            float newDist = distanceRemaining - hit.distance;

            Vector3 newDirection = Vector3.Reflect(direction, hit.normal);
            positions.Add(hit.point);
            reflect(newDist, hit.point, newDirection, ref positions);
        }
        else
        {
            positions.Add(ray.GetPoint(distanceRemaining));
        }
    }

}
