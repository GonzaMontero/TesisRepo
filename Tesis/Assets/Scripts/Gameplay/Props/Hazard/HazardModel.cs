using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class HazardModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] HazardController controller;
        [SerializeField] GameObject hitVFX;
        [SerializeField] float hitVFXCooldown;
        [Header("Runtime Values")]
        [SerializeField] float hitVFXTimer;


        //Unity Events
        void Start()
        {
            if(!hitVFX) Destroy(this);
            
            if (!controller)
            {
                controller = GetComponent<HazardController>();
                if(!controller) return;
            }
            
            controller.HittedSomething += OnHittedSomething;
        }

        void Update()
        {
            if (hitVFXTimer > 0)
            {
                hitVFXTimer -= Time.deltaTime;
            }
        }

        //Methods
        void OnHittedSomething(Collision something)
        {
            if(hitVFXTimer > 0) return;
            
            GameObject vfx = Instantiate(hitVFX);
            vfx.transform.position = something.GetContact(0).point; //playerHitter.collision.point;
            hitVFXTimer = hitVFXCooldown;
        }
    }
}