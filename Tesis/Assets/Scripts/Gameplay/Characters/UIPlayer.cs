using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TimeDistortion.Gameplay.Characters
{
    public class UIPlayer : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] PlayerController controller;
        [SerializeField] Slider healthBar;
        [SerializeField] TextMeshProUGUI healthText;
        [Header("Runtime Values")]
        [SerializeField] int baseHealth;
        [SerializeField] int currentHealth;

        //Unity Events
        private void Start()
        {
            if (!controller)
            {
                controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            }

            //Link Actions
            controller.Hitted += OnHitted;
            controller.Died += OnHitted;

            //Set UI Values
            baseHealth = controller.publicData.baseStats.health;
            OnHitted();
        }
        private void OnDestroy()
        {
            //UnLink Actions
            controller.Hitted -= OnHitted;
            controller.Died -= OnHitted;
        }


        //Methods
        void UpdateHealthBar()
        {
            if (healthBar == null) return;
            healthBar.value = (float)currentHealth / (float)baseHealth;
        }
        void UpdateHealthText()
        {
            if (healthText == null) return;
            healthText.text = currentHealth + "/" + baseHealth;
        }

        //Event Receivers
        void OnHitted()
        {
            currentHealth = controller.publicData.currentStats.health;
            UpdateHealthBar();
            UpdateHealthText();
        }
    }
}