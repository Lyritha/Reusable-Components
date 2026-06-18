using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Lyrith.Navigation
{
    public class NavPath
    {
        public NavWaypoint[] Waypoints;

        public bool HasValidPath => Waypoints != null && Waypoints.Length > 1;

        public int Length => Waypoints?.Length ?? 0;

        public NavWaypoint this[int i] => Waypoints[i];

        public bool HasReachedEnd(Vector3 currentPosition, float threshold = 0.1f)
        {
            if (!HasValidPath) return false;
            if (Waypoints == null || Waypoints.Length == 0) return true;

            Vector3 end = Waypoints[^1].Position;
            return Vector3.Distance(currentPosition, end) <= threshold;
        }

        public Vector3 GetPositionAtDistance(float dist)
        {
            if (Waypoints == null || Waypoints.Length == 0) return Vector3.zero;

            // Clamp to end
            if (dist <= 0) return Waypoints[0].Position;
            if (dist >= Waypoints[^1].distanceFromStart) return Waypoints[^1].Position;

            // Find segment
            for (int i = 0; i < Waypoints.Length - 1; i++)
            {
                float a = Waypoints[i].distanceFromStart;
                float b = Waypoints[i + 1].distanceFromStart;

                if (dist >= a && dist <= b)
                {
                    float t = (dist - a) / (b - a);
                    return Vector3.Lerp(Waypoints[i].Position, Waypoints[i + 1].Position, t);
                }
            }

            return Waypoints[^1].Position;
        }
    }
}
