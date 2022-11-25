using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TimeDistortion.Gameplay.TimePhys
{
    public class UITimeChanger : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] TimeChanger controller;
        [SerializeField] Animator slowMoScopeAnimator;
        [SerializeField] GameObject slowMoScope;
        [SerializeField] float minChargeMod;
        [Header("Runtime Values")]
        [SerializeField] bool charging;

        //Unity Events
        private void Start()
        {
            if (!controller)
            {
                controller = TimeChanger.Get();
            }

            controller.ActivatingCharge += OnCharging;
            controller.ReleasedCharge += OnChargeReleased;
            controller.TargetInScope += OnTargetInScope;
        }
        private void Update()
        {
            if(charging)
            {
                slowMoScope.SetActive(controller.publicCharge >= minChargeMod);
            }
        }

        //Event Receivers
        void OnCharging()
        {
            charging = true;
        }
        void OnChargeReleased()
        {
            charging = false;
            slowMoScope.SetActive(false);
        }
        void OnTargetInScope(bool isInScope)
        {
            slowMoScopeAnimator.SetBool("TargetFound", isInScope);
        }
    }
}