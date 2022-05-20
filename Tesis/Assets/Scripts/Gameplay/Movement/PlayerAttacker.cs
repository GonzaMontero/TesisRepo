using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeDistortion.Gameplay.Handler;

public class PlayerAttacker : MonoBehaviour
{
    [SerializeField] GameObject attackCollider;
    [SerializeField] float attackDuration;

    public void HandleLightAttack()
    {
        if (attackCollider.activeSelf == false)
        {
            attackCollider.SetActive(true);
            Invoke("StopLightAttack", attackDuration * Time.timeScale);
        }    
    }

    public void StopLightAttack()
    {
        if (attackCollider.activeSelf == true)
        {
            attackCollider.SetActive(false);
        }
    }
}
