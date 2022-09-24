using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TimeDistortion.Gameplay.TimePhys
{
    public class UITimeManager : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] TimeChanger manager;
        [SerializeField] Animator slowMoScopeAnimator;
        [SerializeField] GameObject slowMoScope;
        [SerializeField] List<Image> slowMoScopeParts;
        [SerializeField] Color cooldownScopeColor;
        [SerializeField] Color readyScopeColor;
        [Header("Runtime Values")]
        [SerializeField] int cooldownStage;

        //Unity Events
        private void Start()
        {
            if (!manager)
            {
                manager = TimeChanger.Get();
            }

            manager.SlowMoReady += OnSlowMoReady;
            manager.TargetInScope += OnTargetInScope;
        }
        private void Update()
        {
            //if (!slowMoScope.activeSelf) return;
            
            float currentStage = 4 * (manager.publicCharge);
            if (cooldownStage != currentStage)
            {
                cooldownStage = (int)currentStage;
                CooldownUIUpdater(cooldownStage - 1);

                slowMoScopeAnimator.SetBool("SlowMoActive", cooldownStage < 4);
            }
        }

        //Methods
        void CooldownUIUpdater(int newPart)
        {
            if (newPart < 0)
            {
                foreach (var part in slowMoScopeParts)
                {
                    part.color = cooldownScopeColor;
                }
                return;
            }
            slowMoScopeParts[newPart].color = readyScopeColor;
        }

        //Event Receivers
        void OnSlowMoReady(bool isReady)
        {
            slowMoScope.SetActive(isReady);
        }
        void OnTargetInScope(bool isInScope)
        {
            slowMoScopeAnimator.SetBool("TargetFound", isInScope);
        }
    }
}