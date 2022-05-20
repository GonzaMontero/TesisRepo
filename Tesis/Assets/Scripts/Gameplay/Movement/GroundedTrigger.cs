using UnityEngine;
using TimeDistortion.Gameplay.Handler;

public class GroundedTrigger : MonoBehaviour
{
    [SerializeField] InputHandler handler;
    [SerializeField] float posY;

    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, posY, transform.localPosition.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Ground")
        {
            handler.SetGrounded(true);
        }
        else
        {
            handler.SetGrounded(false); 
        }
    }

}
