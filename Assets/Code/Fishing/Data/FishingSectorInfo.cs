using System;

namespace JamSpace
{
    [Serializable]
    public class FishingSectorInfo
    {
        public readonly int Weight;
        public readonly int PointsToAdd;

        public FishingSectorInfo(int weight, int pointsToAdd)
        {
            Weight = weight;
            PointsToAdd = pointsToAdd;
        }
    }
}