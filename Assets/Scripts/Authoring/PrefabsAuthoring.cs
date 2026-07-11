using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class PrefabsAuthoring : MonoBehaviour
    {
        public GameObject playerPrefab;
        
        private class PrefabsAuthoringBaker : Baker<PrefabsAuthoring>
        {
            public override void Bake(PrefabsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                AddComponent(entity, new PrefabsComponent
                {
                    Player = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}