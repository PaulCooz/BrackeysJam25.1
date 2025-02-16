using TMPro;
using UnityEngine;

namespace JamSpace
{
    public class FishingScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        private int _score;

        private void Awake()
        {
            _score = 0;
        }

        private void Start()
        {
            UpdateView();
        }

        public void IncreaseScore(int value)
        {
            _score += value;
            UpdateView();
        }

        public void UpdateView()
        {
            label.text = _score.ToString();
        }
    }
}