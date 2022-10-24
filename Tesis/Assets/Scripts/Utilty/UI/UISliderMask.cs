using UnityEngine;
using UnityEngine.UI;

namespace Universal.UI
{
    public class UISliderMask : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Slider slider;
        [SerializeField] RectTransform mask;
        [SerializeField] RectTransform sourceImage;
        [SerializeField] Vector3 fixedHundredPos;
        [SerializeField] Vector3 fixedZeroPos;
        [SerializeField] bool useFixedPos;
        [Header("Runtime Values")]
        [SerializeField] Vector3 sourcePos;
        [SerializeField] Vector3 hundredPerPos;
        [SerializeField] Vector3 zeroPerPos;
        [Header("Editor Values")]
        [SerializeField] bool executeInEdit; 
        [SerializeField] bool ogPosSetted; 


    
        private void Start()
        {
            if (!sourceImage)
            {
                Destroy(this);
            }
            if (!slider)
            {
                slider = GetComponent<Slider>();
            }
            if (!mask)
            {
                mask = transform.GetComponentInChildren<Mask>().rectTransform;
            }

            GetPositions();
        }

        private void Update()
        {
            MoveImage();
        }

#if UNITY_EDITOR
        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            if (!executeInEdit) return;

            //Set positions
            if (!ogPosSetted && !Application.isPlaying)
            {
                //Get positions
                GetPositions();
                ogPosSetted = true;
            }

            //Move image with mask
            MoveImage();
        }
#endif

        
        void GetPositions()
        {
            sourcePos = sourceImage.position;

            if (useFixedPos)
            {
                zeroPerPos = fixedZeroPos;
                hundredPerPos = fixedHundredPos;
                return;
            }

            hundredPerPos = mask.position;
            zeroPerPos = mask.position;
            zeroPerPos.x -= mask.rect.width / 2;
        }
        void MoveImage()
        {
            //Move Mask
            Vector3 newPos;
            newPos = Vector3.Lerp(zeroPerPos, hundredPerPos, slider.normalizedValue);
            mask.position = newPos;

            //Move Source Image to original position
            sourceImage.position = sourcePos;
        }
    }
}