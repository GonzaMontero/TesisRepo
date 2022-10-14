using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class BlockerModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] BlockerController controller;
        [SerializeField] Animator animator;
        //[Header("Runtime Values")]


        //Unity Events
        void Start()
        {
            if (!controller)
            {
                controller = GetComponent<BlockerController>();
            }
            if (!animator)
            {
                animator = GetComponent<Animator>();
            }

            controller.Hitted += OnHit;
            controller.Destroyed += OnDestroyed;
        }
        
        //Methods
        
        //Event Receivers
        void OnHit()
        {
            animator.SetTrigger("Hitted");
        }
        void OnDestroyed()
        {
            Destroy(animator.gameObject);
        }
    }
}