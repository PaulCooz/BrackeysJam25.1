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

        public GameData Data { get; private set; }

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

            Data = new GameData();
            Data.Setup(_levels[Data.Level]);

            Post<IGameStart>(l => l.GameStart());
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
    }

    public sealed class GameData
    {
        public void Setup(LevelSettings settings)
        {
            FishCount     = 0;
            FishToCollect = settings.fishToCollect;
        }

        public int Level
        {
            get => PlayerPrefs.GetInt("level", 0);
            set => PlayerPrefs.SetInt("level", value);
        }

        public int FishCount     { get; set; }
        public int FishToCollect { get; set; }
    }
}