using StartGame.Components;
using Unity.Entities;
using UnityEngine;

namespace StartGame.Authoring
{
    public class LobbyPlayerAuthoring : MonoBehaviour
    {
        private class LobbyPlayerAuthoringBaker : Baker<LobbyPlayerAuthoring>
        {
            public override void Bake(LobbyPlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<LobbyPlayerComponent>(entity);
            }
        }
    }
}