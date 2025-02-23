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
        [SerializeField]
        public FishZoneSystem.Settings fishZone;
        [SerializeField]
        public FishingMechanics.FishingHookSettings fishingHook;
        [SerializeField]
        public Vector2 minMaxSpawnMeteorInterval;
        [SerializeField]
        public Vector2 minMaxSpawnMeteorSpeed;
    }
}