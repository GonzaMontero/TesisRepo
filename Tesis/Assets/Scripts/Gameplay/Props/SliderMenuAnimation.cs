using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SliderMenuAnimation : MonoBehaviour
{

    [SerializeField] GameObject panelMenu;
    [SerializeField] PlayerControls inputActions;

    public void OnOpenMenuInput(InputAction.CallbackContext context)
    {
        if (panelMenu != null)
        {
            Animator animator = panelMenu.GetComponent<Animator>();
            if (animator != null)
            {
                bool isOpen = animator.GetBool("ShowControls");
                animator.SetBool("ShowControls", !isOpen);
            }
        }
    }
}
