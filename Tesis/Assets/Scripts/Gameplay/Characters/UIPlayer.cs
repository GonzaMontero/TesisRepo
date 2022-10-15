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
            controller.Hitted += OnHitted;
            controller.Died += OnHitted;

            //Set UI Values
            baseHealth = controller.publicData.baseStats.health;
            healthBar.publicSpriteQuantity = baseHealth;
            OnHitted();
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
            if (healthRegenText == null) return;
            healthRegenText.text = currentHealth + "/" + baseHealth;
        }

        //Event Receivers
        void OnHitted()
        {
            currentHealth = controller.publicData.currentStats.health;
            UpdateHealthBar();
        }
        void OnRegenerated()
        {
            UpdateHealthBar();
            UpdateHealthRegenText();
        }
    }
}