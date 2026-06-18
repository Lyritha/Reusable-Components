using Lyrith.Inspector.DynamicDropdown;
using Lyrith.Inspector.Fold;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Lyrith.Navigation
{
    [Serializable, Fold("Pathfinding agent")]
    public partial class NavAgentPathfinder
    {
        private static readonly List<NavAgentObstacle> navAgentObstacles = new();
        private static int navAgentObstaclesIter = 0;

        private NavAgentObstacle navAgentObstacle;

        [SerializeField, DynamicDropdown(nameof(GetAgentIDNamePairs))]
        private int agent = 0;

        [Header("Path settings"), SerializeField, Tooltip("Desired clearance from walls when pushing path points away from them.")]
        private float wallClearance = 2f;
        [SerializeField, Tooltip("Points closer together than this may be merged during cleanup.")]
        private float mergeDistance = 1f;
        [SerializeField, Tooltip("Spacing between sample points.")]
        private float sampleDistance = 0.25f;
        [SerializeField, Tooltip("Points closer together than this may be merged during cleanup.")]
        private int sampleDepth = 1;

        // public access to serialized values
        public int AgentTypeID => agent;
        public float WallClearance => wallClearance;
        public float MergeDistance => mergeDistance;
        public float SampleDistance => sampleDistance;
        public int SampleDepth => sampleDepth;


        private NavMeshPath pathData;

        public NavPath Path { get; private set; } = new();

        public bool TrySetPath(Vector3 start, Vector3 end)
        {
            NavMeshQueryFilter filter = new()
            {
                agentTypeID = agent,
                areaMask = NavMesh.AllAreas
            };

            pathData ??= new();
            bool succeeded = NavMesh.CalculatePath(start, end, filter, pathData);

            if (succeeded)
            {
                int selfID = navAgentObstacle?.id ?? -1;
                Path = pathData.corners.ToList()
                    .PushAwayFromWalls(wallClearance)
                    .Clean(mergeDistance)
                    .SmoothResample(sampleDistance, sampleDepth)
                    .PushAwayFromObstacles(navAgentObstacles, selfID)
                    .Build();
            }
            else
            {
                Path = new NavPath();
            }

            return succeeded && Path.HasValidPath;
        }

        public void RegisterNavAgentObstacle(Vector3 pos)
        {
            NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(AgentTypeID);

            navAgentObstacle = new()
            {
                position = pos,
                radius = settings.agentRadius,
                height = settings.agentHeight,
                id = navAgentObstaclesIter++
            };

            navAgentObstacles.Add(navAgentObstacle);
        }
        public void DeregisterNavAgentObstacle()
        {
            if (navAgentObstacle != null)
            {
                navAgentObstacles.RemoveAll(o => o.id == navAgentObstacle.id);
                navAgentObstacle = null;
            }
        }
        public void UpdateNavAgentObstacle(Vector3 pos)
        {
            if (navAgentObstacle != null) navAgentObstacle.position = pos;
        }


        private static (int, string)[] GetAgentIDNamePairs()
        {
            List<(int, string)> idNamePairs = new();

            int count = NavMesh.GetSettingsCount();

            for (int i = 0; i < count; i++)
            {
                NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(i);
                string name = NavMesh.GetSettingsNameFromID(settings.agentTypeID);

                idNamePairs.Add((settings.agentTypeID, name));
            }

            return idNamePairs.ToArray();
        }
    }
}