using UnityEngine;

namespace JamSpace
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "Game/LevelSettings", order = 0)]
    public sealed class LevelSettings : ScriptableObject
    {
        [SerializeField]
        public int fishToCollect;
        [SerializeField]
        public double timerMinutes;
    }
}