using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public class FishingSectorView : MonoBehaviour
    {
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] public TextMeshProUGUI label;
        [SerializeField] private Image background;
        [SerializeField] private Color backgroundNoPointColor;
        [SerializeField] private Color backgroundPositiveColor;
        [SerializeField] private Color backgroundNegativeColor;

        public int PointsToAdd { get; private set; }

        public void Initialize(int weight, int pointsToAdd)
        {
            PointsToAdd = pointsToAdd;

            layoutElement.flexibleWidth = weight;

            var isNoPoints = PointsToAdd == 0;
            var labelText  = isNoPoints ? string.Empty : PointsToAdd.ToString();

            label.text = labelText;
            label.gameObject.SetActive(!string.IsNullOrEmpty(labelText));

            background.color = isNoPoints
                ? backgroundNoPointColor
                : PointsToAdd > 0
                    ? backgroundPositiveColor
                    : backgroundNegativeColor;

            gameObject.SetActive(true);
        }
    }
}