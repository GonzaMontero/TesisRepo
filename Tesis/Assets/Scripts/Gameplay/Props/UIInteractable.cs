using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class UIInteractable : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] GameObject interactedPopUp;
        [SerializeField] TextMeshProUGUI interactedText;
        [SerializeField] float popUpDuration;
        [Header("Runtime Values")]
        [SerializeField] List<InteractableController> interactables;
        [SerializeField] float popUpTimer;

        //Unity Events
        void Start()
        {
            //Get all interactables
            interactables = new List<InteractableController>();
            interactables.AddRange(FindObjectsOfType<InteractableController>());

            for (int i = 0; i < interactables.Count; i++)
            {
                interactables[i].Interacted += OnAnyInteracted;
            }
        }
        void Update()
        {
            if (popUpTimer > 0)
            {
                popUpTimer -= Time.deltaTime;
            }
            else if (interactedPopUp.activeSelf)
            {
                interactedPopUp.SetActive(false);
            }
        }

        //Methods
        
        //Event Receivers
        void OnAnyInteracted(string message)
        {
            interactedText.text = message;
            interactedPopUp.SetActive(true);
            popUpTimer = popUpDuration;
        }
    }
}