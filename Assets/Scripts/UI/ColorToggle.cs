using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SignalRoguelite
{
    public class ColorToggle : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button button;
        [SerializeField] private Image displayImage;
        [SerializeField] private TMP_Text displayText;
        
        [Header("Sprites")]
        [SerializeField] private Sprite redSprite;
        [SerializeField] private Sprite greenSprite;
        
        private SignalColor currentColor = SignalColor.Red;
        
        public SignalColor CurrentColor => currentColor;
        public event System.Action<SignalColor> OnColorChanged;
        
        void Start()
        {
            if (button == null)
                button = GetComponent<Button>();
            
            if (button != null)
                button.onClick.AddListener(ToggleColor);
            
            UpdateUI();
        }
        
        public void ToggleColor()
        {
            currentColor = (currentColor == SignalColor.Red) ? SignalColor.Green : SignalColor.Red;
            UpdateUI();
            OnColorChanged?.Invoke(currentColor);
        }
        
        private void UpdateUI()
        {
            if (displayImage != null)
                displayImage.sprite = currentColor == SignalColor.Red ? redSprite : greenSprite;
            
            if (displayText != null)
                displayText.text = currentColor == SignalColor.Red ? "RED" : "GREEN";
        }
        
        public void SetColor(SignalColor color)
        {
            currentColor = color;
            UpdateUI();
        }
    }
}