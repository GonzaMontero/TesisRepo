﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Handler;

public class PlayerAttacker : MonoBehaviour
{
    AnimatorHandler handler;

    public void Awake()
    {
        handler = GetComponent<AnimatorHandler>();
    }

    public void HandleLightAttack()
    {
        //animatorhandler play animation
    }
}