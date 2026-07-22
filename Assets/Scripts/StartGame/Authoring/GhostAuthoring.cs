using StartGame.Components;
using Unity.Entities;
using UnityEngine;

namespace StartGame.Authoring
{
    public class GhostAuthoring : MonoBehaviour
    {
        private class GhostAuthoringBaker : Baker<GhostAuthoring>
        {
            public override void Bake(GhostAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<LobbyPlayerComponent>(entity);
            }
        }
    }
}