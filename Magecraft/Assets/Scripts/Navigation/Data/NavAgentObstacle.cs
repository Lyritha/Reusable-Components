using UnityEngine;

namespace Lyrith.Navigation
{
    public record NavAgentObstacle
    {
        public Vector3 position;
        public float radius;
        public float height;
        public int id;
    }
}
