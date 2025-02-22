using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JamSpace
{
    public class FishingMechanics : MonoBehaviour, GameManager.IGameStart
    {
        [SerializeField]
        private RectTransform catchMarker;
        [SerializeField]
        private RectTransform sectorsContainer;
        [SerializeField]
        private FishingSectorView sectorPrefab;
        [SerializeField]
        private CanvasGroup canvasGroup;
        [SerializeField] 
        private AudioSource reelingSound, wooshSound, waterSplash;
        

        private readonly List<FishingSectorView> _sectorInstances = new ();

        private float   _markerBorderValue;
        private Vector2 _markerCurrentDirection;
        private float   _markerSpeed;

        private FishingHookSettings _settings;

        private void Awake()
        {
            canvasGroup.alpha          = 0;
            canvasGroup.blocksRaycasts = false;

            sectorPrefab.gameObject.SetActive(false);
            _markerBorderValue      = Math.Abs((catchMarker.transform.parent as RectTransform)!.rect.x);
            _markerCurrentDirection = Vector2.right;
        }

        public void GameStart()
        {
            _settings = GameManager.Instance.LevelSettings.fishingHook;
        }

        public async UniTask<FishingSectorView.FishingSector> Run(float fadeInDur, float fadeOutDur, Func<bool> catchSector)
        {
            var (speed, sectors) = GetRandomSpeedAndSectors();
            Setup(speed, sectors);
            
            wooshSound.Play();
            DOVirtual.DelayedCall(0.5f, () => waterSplash.Play());

            await canvasGroup.DOFade(1, fadeInDur).OnComplete(() => canvasGroup.blocksRaycasts = true);
            
            reelingSound.Play();

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

            canvasGroup
                .DOFade(0, fadeOutDur)
                .OnComplete(() =>
                {
                    canvasGroup.blocksRaycasts = false;
                    Reset();
                })
                .ToUniTask()
                .Forget();

            var manager = GameManager.Instance;
            switch (currentSector.Info.Type)
            {
                case FishingSectorView.SectorType.Fish:
                    manager.Data.FishCount += currentSector.Info.Value;
                    manager.Post<PlayerController.ICaughtFish>(l => l.PlayerCaughtFish());
                    break;
                case FishingSectorView.SectorType.Time:
                    manager.Data.TimerToGameOver += TimeSpan.FromSeconds(currentSector.Info.Value);
                    break;
                case FishingSectorView.SectorType.Speed:
                    manager.PostOrdered<PlayerController.IChangeSpeed>(l => l.PlayerChangeSpeed(currentSector.Info.Value));
                    break;
            }
            reelingSound.Stop();
            return currentSector.Info;
        }

        private void Setup(float markerSpeed, List<FishingSectorView.FishingSector> sectors)
        {
            _markerSpeed = markerSpeed;

            foreach (var sectorInfo in sectors)
            {
                var sector = Instantiate(sectorPrefab, sectorsContainer);

                sector.Initialize(sectorInfo);

                _sectorInstances.Add(sector);
            }

            catchMarker.anchoredPosition =
                new Vector2(Random.Range(-_markerBorderValue * 0.8f, _markerBorderValue * 0.8f), 0f);
        }

        private (float speed, List<FishingSectorView.FishingSector> sectors) GetRandomSpeedAndSectors()
        {
            var sectorsInfo  = new List<FishingSectorView.FishingSector>();
            var sectorsCount = Random.Range(_settings.minMaxSectorCount.x, _settings.minMaxSectorCount.y);
            for (var i = 0; i < sectorsCount; i++)
            {
                var propWithType = new[]
                {
                    (prop: _settings.proportionOfSpace, type: FishingSectorView.SectorType.Space),
                    (prop: _settings.proportionOfFish, type: FishingSectorView.SectorType.Fish),
                    (prop: _settings.proportionOfSpeed, type: FishingSectorView.SectorType.Speed),
                    (prop: _settings.proportionOfTime, type: FishingSectorView.SectorType.Time),
                };
                var c    = Random.Range(0, propWithType.Sum(p => p.prop));
                var type = propWithType.First().type;
                foreach (var p in propWithType)
                {
                    if (c < p.prop)
                    {
                        type = p.type;
                        break;
                    }
                    c -= p.prop;
                }

                var value = type switch
                {
                    FishingSectorView.SectorType.Fish => Random.Range(0, 100) < _settings.chanceOfDecreaseFish ? -1 : +1,
                    FishingSectorView.SectorType.Time => Random.Range(0, 100) < _settings.chanceOfDecreaseTime
                        ? _settings.timeDecIncSec.x
                        : _settings.timeDecIncSec.y,
                    FishingSectorView.SectorType.Speed => Random.Range(0, 100) < _settings.chanceOfDecreaseSpeed ? -1 : +1,
                    _                                  => 0
                };
                var width = Random.Range(_settings.minMaxSectorWidth.x, _settings.minMaxSectorWidth.y);

                sectorsInfo.Add(new FishingSectorView.FishingSector(type, value, width));
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

        [Serializable]
        public struct FishingHookSettings
        {
            public Vector2Int minMaxSectorCount;
            public Vector2Int minMaxSectorWidth;

            public int proportionOfSpace;

            public int proportionOfFish;
            public int chanceOfDecreaseFish;

            public int        proportionOfTime;
            public int        chanceOfDecreaseTime;
            public Vector2Int timeDecIncSec;

            public int proportionOfSpeed;
            public int chanceOfDecreaseSpeed;
        }
    }
}