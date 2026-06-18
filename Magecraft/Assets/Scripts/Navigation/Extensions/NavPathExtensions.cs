using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Lyrith.Navigation
{
    public static class NavPathBuilderExtensions
    {
        public static NavPath Build(this Vector3[] pts) => pts == null ? new NavPath() : pts.ToList().Build();
        public static NavPath Build(this List<Vector3> pts)
        {
            if (pts == null || pts.Count < 2)
                return new NavPath { Waypoints = Array.Empty<NavWaypoint>() };

            NavWaypoint[] wps = new NavWaypoint[pts.Count];

            float totalDist = 0f;

            for (int i = 0; i < pts.Count; i++)
            {
                if (i > 0)
                    totalDist += Vector3.Distance(pts[i - 1], pts[i]);

                wps[i] = new NavWaypoint
                {
                    Position = pts[i],
                    distanceFromStart = totalDist,
                    NormalizedPositionAlongPath = 0f, // fill later
                    dirToNextWaypoint = (i < pts.Count - 1)
                        ? (pts[i + 1] - pts[i]).normalized
                        : Vector3.zero
                };
            }

            for (int i = 0; i < wps.Length; i++)
                wps[i].NormalizedPositionAlongPath = wps[i].distanceFromStart / totalDist;

            return new NavPath { Waypoints = wps };
        }

        public static List<Vector3> PushAwayFromWalls(this List<Vector3> pts, float clearance = 2f, float snapRadius = 0.3f)
        {
            if (pts == null || pts.Count == 0) return pts;

            List<Vector3> outPts = new(pts.Count);
            outPts.Add(pts[0]);

            for (int i = 1; i < pts.Count - 1; i++)
            {
                Vector3 prev = outPts[i - 1];
                Vector3 pushed = TryPushFromWall(pts[i], clearance, snapRadius);

                outPts.Add(HasClearCorridor(prev, pushed) ? pushed : pts[i]);
            }

            outPts.Add(pts[^1]);
            return outPts;
        }
        public static List<Vector3> Clean(this List<Vector3> pts, float mergeDistance = 1f)
        {
            if (pts == null || pts.Count < 2) return pts;

            List<Vector3> clean = new() { pts[0] };
            int anchor = 0;

            for (int i = 2; i < pts.Count; i++)
            {
                bool close = Vector3.Distance(pts[anchor], pts[i]) < mergeDistance;

                if (!close && !HasClearCorridor(pts[anchor], pts[i]))
                {
                    clean.Add(pts[i - 1]);
                    anchor = i - 1;
                }
            }

            clean.Add(pts[^1]);
            return clean;
        }

        public static List<Vector3> SmoothResample(this List<Vector3> pts, float spacing = 0.25f, int depth = 1, float maxCornerTightness = 1f, float snapRadius = 0.3f)
        {
            if (pts == null || pts.Count < 3) return pts;

            List<Vector3> outPts = new() { pts[0] };
            int maxIndex = Mathf.Min(depth, pts.Count - 2);

            Vector3 last = pts[0];

            for (int i = 1; i <= maxIndex; i++)
            {
                Vector3 A = pts[i - 1];
                Vector3 B = pts[i];
                Vector3 C = pts[i + 1];

                float distAB = Vector3.Distance(A, B);
                float distCB = Vector3.Distance(C, B);

                if (distAB < 1e-5f || distCB < 1e-5f)
                {
                    SampleLine(last, B, spacing, outPts);
                    last = B;
                    continue;
                }

                float tight = Mathf.Min(maxCornerTightness, distAB * 0.5f, distCB * 0.5f);

                Vector3 dirAB = (A - B).normalized;
                Vector3 dirCB = (C - B).normalized;

                Vector3 p1 = B + dirAB * tight;
                Vector3 p2 = B + dirCB * tight;

                SampleLine(last, p1, spacing, outPts);
                outPts.Add(p1);

                const int HIGH = 32;
                Vector3[] dense = new Vector3[HIGH + 1];
                float[] lengths = new float[HIGH + 1];

                dense[0] = Snap(p1, snapRadius);
                lengths[0] = 0f;

                for (int s = 1; s <= HIGH; s++)
                {
                    float t = s / (float)HIGH;
                    dense[s] = Snap(Bezier(p1, B, p2, t), snapRadius);
                    lengths[s] = lengths[s - 1] + Vector3.Distance(dense[s - 1], dense[s]);
                }

                float total = lengths[HIGH];
                float target = spacing;
                int idx = 1;

                while (target < total && idx <= HIGH)
                {
                    while (idx <= HIGH && lengths[idx] < target) idx++;
                    if (idx > HIGH) break;

                    float seg = lengths[idx] - lengths[idx - 1];
                    if (seg > 1e-5f)
                    {
                        float segT = (target - lengths[idx - 1]) / seg;
                        Vector3 p = Vector3.Lerp(dense[idx - 1], dense[idx], segT);
                        outPts.Add(p);
                    }

                    target += spacing;
                }

                outPts.Add(p2);
                last = p2;
            }

            for (int i = maxIndex + 1; i < pts.Count; i++)
                outPts.Add(pts[i]);

            for (int i = 0; i < outPts.Count; i++)
                outPts[i] = Snap(outPts[i], snapRadius);

            return outPts;
        }
        public static List<Vector3> PushAwayFromObstacles(this List<Vector3> pts, List<NavAgentObstacle> obstacles, int selfID, float snapRadius = 0.3f)
        {
            if (pts == null || pts.Count == 0 || selfID < 0) return pts;

            List<Vector3> outPts = new(pts.Count);
            outPts.Add(pts[0]);

            for (int i = 1; i < pts.Count - 1; i++)
            {
                Vector3 prev = outPts[i - 1];
                Vector3 pushed = TryPushFromObstacles(pts[i], obstacles, selfID, snapRadius);

                outPts.Add(HasClearCorridor(prev, pushed) ? pushed : pts[i]);
            }

            outPts.Add(pts[^1]);
            return outPts;
        }



        private static Vector3 TryPushFromWall(Vector3 p, float clearance, float snapRadius)
        {
            if (!NavMesh.FindClosestEdge(p, out NavMeshHit hit, NavMesh.AllAreas))
                return p;

            if (hit.distance >= clearance)
                return p;

            float push = clearance - hit.distance;
            Vector3 candidate = p + hit.normal * push;

            Vector3 snapped = Snap(candidate, snapRadius);
            if (snapped != candidate) return snapped;

            if (NavMesh.FindClosestEdge(candidate, out NavMeshHit opposite, NavMesh.AllAreas))
            {
                Vector3 mid = (hit.position + opposite.position) * 0.5f;
                snapped = Snap(mid, snapRadius);
                if (snapped != mid) return snapped;
            }

            return p;
        }
        private static Vector3 TryPushFromObstacles(Vector3 p, List<NavAgentObstacle> obstacles, int selfID, float snapRadius)
        {
            Vector3 adjusted = p;

            foreach (var obs in obstacles)
            {
                if (obs.id == selfID) continue;

                Vector3 dir = adjusted - obs.position;
                float dist = dir.magnitude;

                if (dist < obs.radius && dist > 0.001f)
                {
                    float push = obs.radius - dist;
                    adjusted += dir.normalized * push;
                }
            }

            return Snap(adjusted, snapRadius);
        }



        private static Vector3 Snap(Vector3 p, float r) => NavMesh.SamplePosition(p, out NavMeshHit hit, r, NavMesh.AllAreas) ? hit.position : p;
        private static bool HasClearCorridor(Vector3 a, Vector3 b)
        {
            NavMeshQueryFilter filter = new() { areaMask = NavMesh.AllAreas };
            return !NavMesh.Raycast(a + Vector3.up * 0.05f, b + Vector3.up * 0.05f, out _, filter);
        }
        private static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            float u = 1f - t;
            return u * u * p0 + 2f * u * t * p1 + t * t * p2;
        }
        private static void SampleLine(Vector3 a, Vector3 b, float spacing, List<Vector3> list)
        {
            float dist = Vector3.Distance(a, b);
            if (dist < 1e-5f)
            {
                list.Add(b);
                return;
            }

            int steps = Mathf.FloorToInt(dist / spacing);

            for (int i = 1; i <= steps; i++)
            {
                float t = (i * spacing) / dist;
                list.Add(Vector3.Lerp(a, b, t));
            }

            list.Add(b);
        }
    }
}
