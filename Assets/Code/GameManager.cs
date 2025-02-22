using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JamSpace
{
    public sealed class GameManager : MonoBehaviour
    {
        private List<LevelSettings> _levels;

        [SerializeField]
        public Rect gameWorldRect;

        public bool     Running { get; private set; }
        public GameData Data    { get; private set; }

        public LevelSettings LevelSettings { get; private set; }

        private static GameManager _instance;
        public static  GameManager Instance => _instance ??= FindAnyObjectByType<GameManager>();

        public void Start()
        {
            _levels = new List<LevelSettings>();
            for (var i = 0;; i++)
            {
                var level = Resources.Load<LevelSettings>($"Level_{i}");
                if (level is null)
                    break;
                _levels.Add(level);
            }

            Data          = new GameData();
            LevelSettings = _levels[Data.Level];
            Data.Setup(LevelSettings);

            Running = true;
            Post<IGameStart>(l => l.GameStart());
        }

        public void Finish(bool isWin)
        {
            if (!Running)
                return;
            Running = false;

            var levelResult = new LevelResult(isWin, Data.Level);
            Post<ILevelFinish>(l => l.LevelFinish(levelResult));
        }

        public void Post<T>(Action<T> action)
        {
            var all = GetAll<T>();
            foreach (var listener in all)
                action(listener);
        }

        private T[] GetAll<T>()
        {
            return FindObjectsByType<Object>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OfType<T>()
                .ToArray();
        }

        public interface IGameStart
        {
            void GameStart();
        }

        public interface ILevelFinish
        {
            void LevelFinish(LevelResult result);
        }

        public readonly struct LevelResult
        {
            public readonly bool IsWin;
            public readonly int  Level;

            public LevelResult(bool isWin, int level)
            {
                IsWin = isWin;
                Level = level;
            }
        }
    }

    public sealed class GameData
    {
        public void Setup(LevelSettings settings)
        {
            FishCount     = 0;
            FishToCollect = settings.fishToCollect;

            TimerToGameOver = TimeSpan.FromMinutes(settings.timerMinutes);
        }

        public int Level
        {
            get => PlayerPrefs.GetInt("level", 0);
            set => PlayerPrefs.SetInt("level", value);
        }

        private int _fishCount;
        public int FishCount
        {
            get => _fishCount;
            set => _fishCount = Math.Clamp(value, 0, FishToCollect);
        }
        public int FishToCollect { get; set; }

        private TimeSpan _timerToGameOver;
        public TimeSpan TimerToGameOver
        {
            get => _timerToGameOver;
            set => _timerToGameOver = value < TimeSpan.Zero ? TimeSpan.Zero : value;
        }
    }
}