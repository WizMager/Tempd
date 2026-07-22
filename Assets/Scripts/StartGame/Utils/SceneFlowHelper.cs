using System;
using UnityEngine.SceneManagement;

namespace StartGame.Utils
{
    public static class SceneFlowHelper
    {
        public const string NetworkBootSceneName = "NetworkBoot";
        public const string MenuSceneName = "Menu";
        public const string GameSceneName = "Game";

        public static event Action GameStarted;
        public static event Action ReturnedToMenu;

        public static void EnsureNetworkBootLoaded()
        {
            if (IsSceneLoaded(NetworkBootSceneName))
            {
                return;
            }

            SceneManager.LoadScene(NetworkBootSceneName, LoadSceneMode.Additive);
        }

        public static void EnsureMenuLoaded()
        {
            if (IsSceneLoaded(MenuSceneName))
            {
                return;
            }

            SceneManager.LoadScene(MenuSceneName, LoadSceneMode.Additive);
        }

        public static void LoadGameAdditive()
        {
            if (!IsSceneLoaded(GameSceneName))
            {
                SceneManager.LoadScene(GameSceneName, LoadSceneMode.Additive);
            }

            GameStarted?.Invoke();
        }

        public static void UnloadMenuIfLoaded()
        {
            if (IsSceneLoaded(MenuSceneName))
            {
                SceneManager.UnloadSceneAsync(MenuSceneName);
            }
        }

        public static void UnloadGameIfLoaded()
        {
            if (IsSceneLoaded(GameSceneName))
            {
                SceneManager.UnloadSceneAsync(GameSceneName);
            }

            EnsureMenuLoaded();
            ReturnedToMenu?.Invoke();
        }

        private static bool IsSceneLoaded(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }
    }
}
