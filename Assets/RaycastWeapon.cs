using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
    }


    public bool isFiring = false;
    public ParticleSystem muzzleFlash;
    //Fire rate is how many bullets shot per second
    public int fireRate = 25;
    public float bulletSpeed = 1000.0f;
    public float bulletDrop = 300.0f;
    public ParticleSystem hitEffect;
    public TrailRenderer trailRenderer;
    public Transform raycastOrigin;
    public Transform raycastDestination;


    Ray ray;
    RaycastHit hitInfo;
    float accumluatedTime;
    List<Bullet> bullets = new List<Bullet>();
    float maxLifeTime = 3.0f;

    Vector3 GetPosition(Bullet bullet)
    {
        // p + v*t + 0.5*g*t*t
        Vector3 gravity = Vector3.down * bulletDrop;
        //Respect the orders in math
        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(trailRenderer, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        return bullet;
    }

    //Maybe needed to hande Unity Bug
    //public ParticleSystem[] muzzleFlash;
    public void StartFiring()
    {
        isFiring = true;
        //Instanziate the accumulated time to 0
        accumluatedTime = 0;
        FireBullet();
    }

    public void UpdateFiring(float deltaTime)
    {
        accumluatedTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while(accumluatedTime >= 0)
        {
            FireBullet();
            accumluatedTime -= fireInterval;
        }
    }

    public void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    private void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifeTime);
    }

    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        //Normalisez so we cant take the magnitude
        ray.direction = direction;
        if (Physics.Raycast(ray, out hitInfo))
        {
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
            //Direction
            hitEffect.transform.position = hitInfo.point;
            //Normalize vector
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = maxLifeTime;
        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }

    private void FireBullet()
    {
        muzzleFlash.Emit(1);

        //Maybe needed to hande Unity Bug
        //foreach(var particle in muzzleFlash)
        //{
        //    particle.emit(1);
        //}

        Vector3 velocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;
        Bullet bullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Add(bullet);

    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
