using Unity.Entities;
using Unity.Scenes;

namespace StartGame.Utils
{
    public static class NetworkSceneLoader
    {
        public static void LoadMenuSubScene(World world)
        {
            if (world is not { IsCreated: true })
            {
                return;
            }

            SceneSystem.LoadSceneAsync(world.Unmanaged, NetworkSceneConfig.Instance.MenuSubScene, new SceneSystem.LoadParameters
            {
                Flags = SceneLoadFlags.BlockOnStreamIn
            });
        }
    }
}
