using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public class FishingSectorView : MonoBehaviour
    {
        [SerializeField]
        private LayoutElement layoutElement;
        [SerializeField]
        public TextMeshProUGUI label;
        [SerializeField]
        private Image background;
        [SerializeField]
        private Color backgroundNoPointColor;
        [SerializeField]
        private Color backgroundPositiveColor;
        [SerializeField]
        private Color backgroundNegativeColor;

        public FishingSector Info { get; private set; }

        public void Initialize(FishingSector info)
        {
            Info                        = info;
            layoutElement.flexibleWidth = Info.Width;
            switch (Info.Type)
            {
                case SectorType.Space:
                    layoutElement.flexibleWidth = info.Value;
                    label.gameObject.SetActive(false);
                    background.color = backgroundNoPointColor;
                    break;
                case SectorType.Fish:
                    label.text = GetText(Info.Value, "fish");
                    label.gameObject.SetActive(true);
                    background.color = Info.Value > 0 ? backgroundPositiveColor : backgroundNegativeColor;
                    break;
                case SectorType.Time:
                    label.text = GetText(Info.Value, "stopwatch");
                    label.gameObject.SetActive(true);
                    background.color = Info.Value > 0 ? backgroundPositiveColor : backgroundNegativeColor;
                    break;
                case SectorType.Speed:
                    label.text = GetText(Info.Value, Info.Value >= 0 ? "boot_fast" : "boot_slow");
                    label.gameObject.SetActive(true);
                    background.color = Info.Value > 0 ? backgroundPositiveColor : backgroundNegativeColor;
                    break;
            }

            gameObject.SetActive(true);
        }

        private string GetText(int value, string sprite) => $"{(value > 0 ? "+" : "")}{value} <sprite=\"{sprite}\" index=0>";

        public readonly struct FishingSector
        {
            public readonly SectorType Type;

            public readonly int Value;
            public readonly int Width;

            public FishingSector(SectorType type, int value, int width)
            {
                Type  = type;
                Value = value;
                Width = width;
            }
        }

        public enum SectorType
        {
            Space,
            Fish,
            Time,
            Speed
        }
    }
}