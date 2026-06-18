using UnityEngine;

namespace Lyrith.Navigation
{
    public struct NavWaypoint
    {
        public Vector3 Position;
        public float distanceFromStart;
        public float NormalizedPositionAlongPath;
        public Vector3 dirToNextWaypoint;

        public static implicit operator Vector3(NavWaypoint wp) => wp.Position;
    }
}