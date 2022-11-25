using System;
using TMPro;
using UnityEngine;
using Universal.UI;

namespace TimeDistortion.Gameplay.Characters
{
    public class UIPlayer : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Handler.PlayerController controller;
        [SerializeField] UISpriteBar healthBar;
        [SerializeField] TextMeshProUGUI healthRegenText;
        [SerializeField] TextMeshProUGUI interactText;
        [SerializeField] GameObject interactPopUp;
        [Header("Runtime Values")]
        [SerializeField] string originalInteractText;
        [SerializeField] string interactMessage;
        [SerializeField] int baseHealth;
        [SerializeField] int currentHealth;
        [SerializeField] bool spawned;

        //Unity Events
        private void Start()
        {
            if (!controller)
            {
                controller = GameObject.FindGameObjectWithTag("Player").GetComponent<Handler.PlayerController>();
            }

            //Link Actions
            controller.LifeChanged += OnLifeChanged;
            //controller.Died += OnLifeChanged;

            //Set UI Values
            baseHealth = controller.publicData.baseStats.health;
            healthBar.publicSpriteQuantity = baseHealth;
            healthBar.Set();
            OnLifeChanged(1);
            originalInteractText = interactText.text;
        }

        public void Update()
        {
            //Set player UI only after the player spawned
            if (spawned) return;
            if(controller.isSpawning) return;

            //Set interact pop up
            if (controller.publicCanInteract)
                SetInteractText();
            else
                ClearInteractText();
        }

        private void OnDestroy()
        {
            //UnLink Actions
            //controller.Hitted -= OnHitted;
            //controller.Died -= OnHitted;
        }

        //Methods
        void UpdateHealthBar()
        {
            if (!healthBar) return;
            healthBar.publicFilledSprites = currentHealth;
        }
        void UpdateHealthRegenText()
        {
            if (!healthRegenText) return;
            //healthRegenText.text = currentHealth + "/" + baseHealth;
            healthRegenText.text = controller.regenerators.ToString();
        }
        void SetInteractText()
        {
            //if message is already writen, exit
            if (interactMessage == controller.publicInteractable.interactedMessage) return;
            
            //Get interact message
            interactMessage = controller.publicInteractable.interactedMessage;
            
            //Set interact text
            //interactText.text = originalInteractText + "\n" + interactMessage;
            
            interactPopUp.SetActive(true);
        }
        void ClearInteractText()
        {
            if (!interactPopUp.activeSelf) return;

            interactPopUp.SetActive(false);
            
            //Reset interact message
            interactMessage = "";

            //Reset interact text
            //interactText.text = originalInteractText;
        }

        //Event Receivers
        void OnLifeChanged(int healthDifference)
        {
            currentHealth = controller.publicData.currentStats.health;
            UpdateHealthBar();

            if (healthDifference > 0)
            {
                UpdateHealthRegenText();
            }
        }
    }
}