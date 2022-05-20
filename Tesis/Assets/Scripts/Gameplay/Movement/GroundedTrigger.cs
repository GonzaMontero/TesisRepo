using UnityEngine;
using TimeDistortion.Gameplay.Handler;

public class GroundedTrigger : MonoBehaviour
{
    [SerializeField] InputHandler handler;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Ground")
        {
            handler.SetGrounded(true);
        }
    }

}
