using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;
using UnityEngine;

namespace StartGame.Utils
{
    public static class NetworkSceneLoader
    {
        public static void LoadNetworkBootSubScene(World world)
        {
            LoadSubScene(world, config => config.NetworkBootSubScene);
        }

        public static void LoadGameSubScene(World world)
        {
            LoadSubScene(world, config => config.GameSubScene);
        }

        private static void LoadSubScene(World world, System.Func<NetworkSceneConfig, EntitySceneReference> selectScene)
        {
            if (world is not { IsCreated: true })
            {
                return;
            }

            if (NetworkSceneConfig.Instance == null)
            {
                Debug.LogError("NetworkSceneConfig is missing. Ensure NetworkBoot scene is loaded.");
                return;
            }

            SceneSystem.LoadSceneAsync(world.Unmanaged, selectScene(NetworkSceneConfig.Instance), new SceneSystem.LoadParameters
            {
                Flags = SceneLoadFlags.BlockOnStreamIn
            });
        }
    }
}
