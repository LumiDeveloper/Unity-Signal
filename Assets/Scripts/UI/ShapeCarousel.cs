using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SignalRoguelite
{
    public class ShapeCarousel : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        
        [Header("Display")]
        [SerializeField] private Image displayImage;
        [SerializeField] private TMP_Text displayText;
        
        [Header("Sprites")]
        [SerializeField] private Sprite squareSprite;
        [SerializeField] private Sprite triangleSprite;
        [SerializeField] private Sprite circleSprite;
        
        private ShapeType[] shapes = new ShapeType[] 
        { 
            ShapeType.Square, 
            ShapeType.Triangle, 
            ShapeType.Circle 
        };
        
        private int currentIndex = 0;
        
        public ShapeType CurrentShape => shapes[currentIndex];
        public event System.Action<ShapeType> OnShapeChanged;
        
        void Start()
        {
            if (leftButton != null)
                leftButton.onClick.AddListener(Previous);
            
            if (rightButton != null)
                rightButton.onClick.AddListener(Next);
            
            UpdateUI();
        }
        
        public void Next()
        {
            currentIndex = (currentIndex + 1) % shapes.Length;
            UpdateUI();
            OnShapeChanged?.Invoke(CurrentShape);
        }
        
        public void Previous()
        {
            currentIndex = (currentIndex - 1 + shapes.Length) % shapes.Length;
            UpdateUI();
            OnShapeChanged?.Invoke(CurrentShape);
        }
        
        private void UpdateUI()
        {
            if (displayImage != null)
            {
                switch (CurrentShape)
                {
                    case ShapeType.Square:
                        displayImage.sprite = squareSprite;
                        break;
                    case ShapeType.Triangle:
                        displayImage.sprite = triangleSprite;
                        break;
                    case ShapeType.Circle:
                        displayImage.sprite = circleSprite;
                        break;
                }
            }
            
            if (displayText != null)
            {
                switch (CurrentShape)
                {
                    case ShapeType.Square:
                        displayText.text = "SQUARE";
                        break;
                    case ShapeType.Triangle:
                        displayText.text = "TRIANGLE";
                        break;
                    case ShapeType.Circle:
                        displayText.text = "CIRCLE";
                        break;
                }
            }
        }
        
        public void SetShape(ShapeType shape)
        {
            for (int i = 0; i < shapes.Length; i++)
            {
                if (shapes[i] == shape)
                {
                    currentIndex = i;
                    UpdateUI();
                    break;
                }
            }
        }
    }
}