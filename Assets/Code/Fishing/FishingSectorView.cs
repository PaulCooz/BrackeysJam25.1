using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public class FishingSectorView : MonoBehaviour
    {
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image background;
        [SerializeField] private Color backgroundNoPointColor;
        [SerializeField] private Color backgroundPositiveColor;
        [SerializeField] private Color backgroundNegativeColor;
        
        private Action _onCatch;

        public void Initialize(int weight, int pointsToAdd, Action onCatch)
        {
            _onCatch = onCatch;
            
            layoutElement.flexibleWidth = weight;
            
            var isNoPoints = pointsToAdd == 0;
            var labelText = isNoPoints ? string.Empty : pointsToAdd.ToString();
            
            label.text = labelText;
            
            label.gameObject.SetActive(!string.IsNullOrEmpty(labelText));
            
            background.color = isNoPoints 
                ? backgroundNoPointColor 
                : pointsToAdd > 0 
                    ? backgroundPositiveColor 
                    : backgroundNegativeColor;
            
            gameObject.SetActive(true);
        }

        public void Catch()
        {
            _onCatch?.Invoke();
        }
    }
}