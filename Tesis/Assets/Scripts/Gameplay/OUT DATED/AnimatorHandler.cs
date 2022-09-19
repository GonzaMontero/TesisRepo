using UnityEngine;

namespace TimeDistortion.Gameplay.Handler
{
    public class AnimatorHandler : MonoBehaviour
    {
        public Animator anim;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            anim = GetComponent<Animator>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement,float horizontalMovement)
        {
            #region Vertical
            float v = 0;

            if (verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (verticalMovement > 0.55f)
            {
                v = 1;
            }
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                v = -1;
            }
            else
            {
                v = 0;
            }

            #endregion

            #region Horizontal
            float y = 0;

            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                y = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                y = 1;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                y = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                y = -1;
            }
            else
            {
                y = 0;
            }

            #endregion

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, y, 0.1f, Time.deltaTime);
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }
    }
}
