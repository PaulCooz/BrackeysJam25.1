using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JamSpace
{
    public class FishingMechanics : MonoBehaviour
    {
        [SerializeField]
        private RectTransform catchMarker;
        [SerializeField]
        private RectTransform sectorsContainer;
        [SerializeField]
        private FishingSectorView sectorPrefab;
        [SerializeField]
        private CanvasGroup canvasGroup;

        private readonly List<FishingSectorView> _sectorInstances = new ();

        private float   _markerBorderValue;
        private Vector2 _markerCurrentDirection;
        private float   _markerSpeed;

        private void Awake()
        {
            canvasGroup.alpha          = 0;
            canvasGroup.blocksRaycasts = false;

            sectorPrefab.gameObject.SetActive(false);
            _markerBorderValue      = Math.Abs((catchMarker.transform.parent as RectTransform)!.rect.x);
            _markerCurrentDirection = Vector2.right;
        }

        public async UniTask<FishingResult> Run(Func<bool> catchSector)
        {
            var (speed, sectors) = GetRandomSpeedAndSectors();
            Setup(speed, sectors);

            await canvasGroup.DOFade(1, 0.3f).OnComplete(() => canvasGroup.blocksRaycasts = true);

            var currentSector = _sectorInstances.First();
            do
            {
                await UniTask.NextFrame();

                catchMarker.anchoredPosition += _markerCurrentDirection * (_markerSpeed * Time.deltaTime);
                if (Math.Abs(catchMarker.anchoredPosition.x) > _markerBorderValue)
                {
                    _markerCurrentDirection = -_markerCurrentDirection;
                }

                var markerRect = catchMarker.GetWorldRect();
                foreach (var sector in _sectorInstances)
                {
                    var newRect = ((RectTransform)sector.transform).GetWorldRect();
                    var oldRect = ((RectTransform)currentSector.transform).GetWorldRect();

                    var newDist = Math.Abs(markerRect.center.x - newRect.center.x);
                    var oldDist = Math.Abs(markerRect.center.x - oldRect.center.x);
                    if (newDist < oldDist)
                    {
                        currentSector = sector;
                    }
                }
            } while (!catchSector());

            await canvasGroup.DOFade(0, 0.3f).OnComplete(() => canvasGroup.blocksRaycasts = false);
            Reset();

            return new FishingResult(currentSector.PointsToAdd);
        }

        private void Setup(float markerSpeed, List<(int weight, int pointsToAdd)> sectors)
        {
            _markerSpeed = markerSpeed;

            foreach (var sectorInfo in sectors)
            {
                var sector = Instantiate(sectorPrefab, sectorsContainer);

                sector.Initialize(weight: sectorInfo.weight, pointsToAdd: sectorInfo.pointsToAdd);

                _sectorInstances.Add(sector);
            }

            catchMarker.anchoredPosition =
                new Vector2(Random.Range(-_markerBorderValue * 0.8f, _markerBorderValue * 0.8f), 0f);
        }

        private (float speed, List<(int weight, int pointsToAdd)> sectors) GetRandomSpeedAndSectors()
        {
            var  sectorsInfo         = new List<(int weight, int pointsToAdd)>();
            var  sectorsCount        = Random.Range(3, 8);
            int? previousPointsToAdd = null;

            for (var i = 0; i < sectorsCount; i++)
            {
                var pointsToAdd = Random.Range(-3, 3);
                var weight      = Random.Range(30, 100);

                if (previousPointsToAdd is > 0 && pointsToAdd > 0 || previousPointsToAdd is < 0 && pointsToAdd < 0)
                {
                    pointsToAdd = -pointsToAdd;
                }

                sectorsInfo.Add((weight, pointsToAdd));

                previousPointsToAdd = pointsToAdd;
            }

            var markerSpeed = Random.Range(100f, 1000f);

            return (markerSpeed, sectorsInfo);
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

    public readonly struct FishingResult
    {
        public readonly int PointsToAdd;

        public FishingResult(int pointsToAdd)
        {
            PointsToAdd = pointsToAdd;
        }
    }
}