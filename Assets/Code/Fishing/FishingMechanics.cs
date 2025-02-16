using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JamSpace
{
    public class FishingMechanics : MonoBehaviour
    {
        [SerializeField] private bool onEnableRandomSetup;
        [SerializeField] private FishingScoreView scoreView;
        [SerializeField] private RectTransform catchMarker;
        [SerializeField] private RectTransform sectorsContainer;
        [SerializeField] private FishingSectorView sectorPrefab;

        private readonly List<FishingSectorView> _sectorInstances = new();
        private float _markerBorderValue;
        private Vector2 _markerCurrentDirection;
        private float _markerSpeed;

        private void Awake()
        {
            sectorPrefab.gameObject.SetActive(false);
            _markerBorderValue = Math.Abs((catchMarker.transform.parent as RectTransform)!.rect.x);
            _markerCurrentDirection = Vector2.right;
        }

        private void OnEnable()
        {
            if (onEnableRandomSetup)
            {
                RandomSetup();
                return;
            }
        }

        private void OnDisable()
        {
            Reset();
        }

        private void Update()
        {
            catchMarker.anchoredPosition += _markerCurrentDirection * (_markerSpeed * Time.deltaTime);

            if (Math.Abs(catchMarker.anchoredPosition.x) > _markerBorderValue)
            {
                _markerCurrentDirection = -_markerCurrentDirection;
            }
        }

        public void Setup(FishingInfo fishingInfo)
        {
            _markerSpeed = fishingInfo.MarkerSpeed;
            
            foreach (var sectorInfo in fishingInfo.Sectors)
            {
                var sector = Instantiate(sectorPrefab, sectorsContainer);
                
                sector.Initialize(
                    weight: sectorInfo.Weight,
                    pointsToAdd: sectorInfo.PointsToAdd,
                    onCatch: () => scoreView.IncreaseScore(sectorInfo.PointsToAdd));
                
                _sectorInstances.Add(sector);
            }
            
            catchMarker.anchoredPosition = new Vector2(Random.Range(-_markerBorderValue * 0.8f, _markerBorderValue * 0.8f), 0f);
        }

        private void RandomSetup()
        {
            var sectorsInfo = new List<FishingSectorInfo>();
            var sectorsCount = Random.Range(3, 8);
            int? previousPointsToAdd = null;

            for (var i = 0; i < sectorsCount; i++)
            {
                var pointsToAdd = Random.Range(-3, 3);
                var weight = Random.Range(30, 100);

                if (previousPointsToAdd is > 0 && pointsToAdd > 0 || previousPointsToAdd is < 0 && pointsToAdd < 0)
                {
                    pointsToAdd = -pointsToAdd;
                }
                
                sectorsInfo.Add(new FishingSectorInfo(weight, pointsToAdd));

                previousPointsToAdd = pointsToAdd;
            }
            
            var markerSpeed = Random.Range(100f, 500f);
            
            Setup(new FishingInfo(markerSpeed, sectorsInfo));
        }
        
        private void Reset()
        {
            foreach (var sector in _sectorInstances)
            {
                Destroy(sector.gameObject);
            }
            
            _sectorInstances.Clear();
        }
    }
}