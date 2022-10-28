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
        [SerializeField] GameObject interactPopUp;
        [Header("Runtime Values")]
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
            OnLifeChanged(1);
        }

        public void Update()
        {
            //Set player UI only after the player spawned
            if (spawned) return;
            if(controller.isSpawning) return;

            //Set health bar
            healthBar.Set();

            //Set interact pop up
            interactPopUp.SetActive(controller.publicCanInteract);
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