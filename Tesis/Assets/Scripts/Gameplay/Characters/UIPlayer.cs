﻿using TMPro;
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
        [Header("Runtime Values")]
        [SerializeField] int baseHealth;
        [SerializeField] int currentHealth;

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
            OnLifeChanged(0);
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
            healthRegenText.text = currentHealth + "/" + baseHealth;
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