using StartGame.Components;
using Unity.Entities;
using UnityEngine;

namespace StartGame.Authoring
{
    public class GhostPrefabAuthoring : MonoBehaviour
    {
        public GameObject lobbyPlayerPrefab;
        
        private class GhostPrefabAuthoringBaker : Baker<GhostPrefabAuthoring>
        {
            public override void Bake(GhostPrefabAuthoring prefabAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new LobbyPlayerPrefabComponent
                {
                    Value = GetEntity(prefabAuthoring.lobbyPlayerPrefab, TransformUsageFlags.None)
                });
            }
        }
    }
}