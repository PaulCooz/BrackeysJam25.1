using System;
using System.Collections.Generic;

namespace JamSpace
{
    [Serializable]
    public class FishingInfo
    {
        public readonly float MarkerSpeed;
        public readonly List<FishingSectorInfo> Sectors;

        public FishingInfo(float markerSpeed, List<FishingSectorInfo> sectors)
        {
            MarkerSpeed = markerSpeed;
            Sectors = sectors;
        }
    }
}