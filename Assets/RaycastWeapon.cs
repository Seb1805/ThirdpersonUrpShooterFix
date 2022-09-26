using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public bool isFiring = false;
    public ParticleSystem muzzleFlash;
    public Transform raycastOrigin;
    public Transform raycastDestination;


    Ray ray;
    RaycastHit hitInfo;

    //Maybe needed to hande Unity Bug
    //public ParticleSystem[] muzzleFlash;
    public void StartFiring()
    {
        isFiring = true;
        muzzleFlash.Emit(1);

        //Maybe needed to hande Unity Bug
        //foreach(var particle in muzzleFlash)
        //{
        //    particle.emit(1);
        //}

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;
        if(Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
        }
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
