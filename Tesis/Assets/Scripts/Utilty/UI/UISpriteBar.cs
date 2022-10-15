using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Universal.UI
{
    public class UISpriteBar : MonoBehaviour
    {
        [Header("Set Values")] 
        [SerializeField] GameObject spritePrefab;
        [SerializeField] RectTransform rectT;
        [SerializeField] Sprite filledImage;
        [SerializeField] Sprite emptyImage;
        [SerializeField] float spriteSpacing;
        [SerializeField] int spriteQuantity;
        [Header("Runtime Values")]
        [SerializeField] List<Image> sprites;
        [SerializeField] int filledSprites;
        [Header("Editor Values")]
        [SerializeField] bool runOnEditMode;
        [SerializeField] bool resetSprites;
        
        public int publicFilledSprites { set { filledSprites = value; UpdateSprites(); } }
        public int publicSpriteQuantity { set { spriteQuantity = value; ResetImages(); } }

        //Unity Events
        void Start()
        {
            if (!rectT)
            {
                rectT = GetComponent<RectTransform>();
            }

            SetImages();
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!runOnEditMode) return;
            if (resetSprites)
            {
                ResetImages();
                resetSprites = false;
                Debug.Log("Resetted " + gameObject.name + " sprites!");
            }
        }
#endif
        
        //Methods
        void SetImages()
        {
            //Instantiate Sprites
            for (int i = 0; i < spriteQuantity; i++)
            {
                //Instantiatate GO
                sprites.Add(Instantiate(spritePrefab, transform).GetComponent<Image>());
                
                //Set name
                sprites[i].name = spritePrefab.name + " " + (i + 1);
            }
            
            //Get sizes
            float rectWidth = rectT.rect.width;
            float totalSpacing = spriteSpacing * spriteQuantity;
            float totalSpriteWidth = rectWidth - totalSpacing;
            float spriteWidth = totalSpriteWidth / spriteQuantity;
            
            for (int i = 0; i < spriteQuantity; i++)
            {
                SetImage(i, spriteWidth, rectT.rect.xMin);
            }
            
            UpdateSprites();
        }
        void SetImage(int imageIndex, float width, float rectLeft)
        {
            //Get components
            Image image = sprites[imageIndex];
            RectTransform imageRectT = image.rectTransform;

            //Calculate Position
            float pos = rectLeft + width * 0.55f;
            pos += width * imageIndex + spriteSpacing * imageIndex;
            
            //Set size & position
            imageRectT.sizeDelta = Vector2.one * width;
            imageRectT.anchoredPosition = Vector2.right * pos + Vector2.up * rectT.rect.center.y;
        }
        void ResetImages()
        {
            while (sprites.Count > 0)
            {
                //Destroy gameobject if still there
                if (sprites[0])
                {
                    DestroyImmediate(sprites[0].gameObject);
                }

                //Remove from list
                sprites.RemoveAt(0);
            }
            
            //Set all images again
            SetImages();
        }
        void UpdateSprites()
        {
            for (int i = 0; i < spriteQuantity; i++)
            {
                sprites[i].sprite = i < filledSprites ? filledImage : emptyImage;
            }
        }
    }
}