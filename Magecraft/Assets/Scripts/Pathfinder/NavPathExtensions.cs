using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavPathExtensions
{
    public static NavPath SmoothPath(this NavPath path)
    {
        Vector3[] raw = path.Points;
        if (raw == null || raw.Length < 3) return raw;

        List<Vector3> smooth = new() { raw[0] };

        float maxCornerTightness = 1f;
        int maxSamplesPerCorner = 8;

        for (int i = 1; i < raw.Length - 1; i++)
        {
            Vector3 A = raw[i - 1];
            Vector3 B = raw[i];
            Vector3 C = raw[i + 1];

            float distAB = Vector3.Distance(A, B);
            float distCB = Vector3.Distance(C, B);

            // Degenerate corner — points coincide, nothing to smooth, just keep B.
            if (distAB < 0.0001f || distCB < 0.0001f)
            {
                smooth.Add(B);
                continue;
            }

            // Clamp tightness to half the shorter adjacent segment so control
            // points never overshoot past A or C.
            float cornerTightness = Mathf.Min(maxCornerTightness, distAB * 0.5f, distCB * 0.5f);
            int samplesPerCorner = Mathf.RoundToInt((float)maxSamplesPerCorner / (maxCornerTightness / cornerTightness));

            Vector3 dirAB = (A - B) / distAB;
            Vector3 dirCB = (C - B) / distCB;

            Vector3 p1 = B + dirAB * cornerTightness;
            Vector3 p2 = B + dirCB * cornerTightness;

            for (int s = 1; s <= samplesPerCorner; s++)
            {
                float t = s / (float)samplesPerCorner;
                Vector3 p = Bezier(p1, B, p2, t);
                if (NavMesh.SamplePosition(p, out NavMeshHit sampleHit, 1, NavMesh.AllAreas)) p = sampleHit.position;
                smooth.Add(p);
            }
        }

        smooth.Add(raw[^1]);
        return smooth.ToArray();
    }
    public static NavPath CleanPath(this NavPath path)
    {
        Vector3[] raw = path.Points;
        if (raw == null || raw.Length < 2) return raw;

        List<Vector3> clean = new() { raw[0] };
        int anchor = 0;

        for (int i = 2; i < raw.Length; i++)
        {
            bool closeEnough = Vector3.Distance(raw[anchor], raw[i]) < 1f;

            if (!closeEnough && !HasClearCorridor(raw[anchor], raw[i]))
            {
                clean.Add(raw[i - 1]);
                anchor = i - 1;
            }
        }

        clean.Add(raw[^1]);
        return clean.ToArray();
    }
    public static NavPath PushAwayFromWalls(this NavPath path, float desiredClearance = 2f)
    {
        Vector3[] raw = path.Points;
        if (raw == null || raw.Length == 0) return raw;

        Vector3[] result = new Vector3[raw.Length];
        result[0] = raw[0]; // anchor: never push the start point

        int lastIndex = raw.Length - 1;

        for (int i = 1; i < raw.Length; i++)
        {
            if (i == lastIndex)
            {
                result[i] = raw[i]; // never push the goal point either
                continue;
            }

            Vector3 original = raw[i];
            Vector3 finalPoint = TryPushPoint(original, desiredClearance);

            // Confirm a clear corridor from the previous RESULT point to this one.
            // If even the fallback isn't safely reachable, keep the original point.
            if (!HasClearCorridor(result[i - 1], finalPoint)) finalPoint = original;

            result[i] = finalPoint;
        }

        return result;
    }

    private static Vector3 TryPushPoint(Vector3 original, float desiredClearance)
    {
        if (!NavMesh.FindClosestEdge(original, out NavMeshHit nearestEdge, NavMesh.AllAreas)) return original;

        float dist = nearestEdge.distance;
        if (dist >= desiredClearance) return original;

        float pushAmount = desiredClearance - dist;
        Vector3 candidate = original + nearestEdge.normal * pushAmount;

        // 1. Try snapping the pushed candidate onto the navmesh.
        if (NavMesh.SamplePosition(candidate, out NavMeshHit sampleHit, desiredClearance, NavMesh.AllAreas)) return sampleHit.position;

        // 2. Push overshot off-mesh — find the edge nearest the (off-mesh) candidate,
        //    which represents the "opposite wall" we pushed too close to or past.
        if (NavMesh.FindClosestEdge(candidate, out NavMeshHit oppositeEdge, NavMesh.AllAreas))
        {
            Vector3 midpoint = (nearestEdge.position + oppositeEdge.position) * 0.5f;

            if (NavMesh.SamplePosition(midpoint, out NavMeshHit midSample, desiredClearance, NavMesh.AllAreas))
                return midSample.position;
        }

        // 3. Nothing worked — give up and keep the original point.
        return original;
    }

    private static bool HasClearCorridor(Vector3 a, Vector3 b)
    {
        NavMeshQueryFilter filter = new()
        {
            areaMask = NavMesh.AllAreas
        };

        Vector3 aUp = a + Vector3.up * 0.05f;
        Vector3 bUp = b + Vector3.up * 0.05f;

        return !NavMesh.Raycast(aUp, bUp, out _, filter);
    }
    private static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1f - t;
        return u * u * p0 + 2f * u * t * p1 + t * t * p2;
    }
}