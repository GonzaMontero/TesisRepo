using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlatform : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Platform")
        {
            transform.parent = other.transform.parent;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Platform")
        {
            transform.parent = null;
        }
    }
}
