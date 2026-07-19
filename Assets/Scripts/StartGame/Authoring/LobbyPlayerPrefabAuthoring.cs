using StartGame.Components;
using Unity.Entities;
using UnityEngine;

namespace StartGame.Authoring
{
    public class LobbyPlayerPrefabAuthoring : MonoBehaviour
    {
        public GameObject lobbyPlayerPrefab;
        
        private class LobbyPlayerAuthoringBaker : Baker<LobbyPlayerPrefabAuthoring>
        {
            public override void Bake(LobbyPlayerPrefabAuthoring prefabAuthoring)
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