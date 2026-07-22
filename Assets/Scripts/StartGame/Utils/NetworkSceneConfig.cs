using Unity.Entities.Serialization;
using UnityEngine;

namespace StartGame.Utils
{
    public class NetworkSceneConfig : MonoBehaviour
    {
        [field: SerializeField] public EntitySceneReference NetworkBootSubScene { get; private set; }
        [field: SerializeField] public EntitySceneReference GameSubScene { get; private set; }

        public static NetworkSceneConfig Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
