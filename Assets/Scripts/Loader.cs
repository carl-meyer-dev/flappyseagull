using UnityEngine.SceneManagement;

public static class Loader 
{
   public enum Scene
   {
      GameScene,
      LoadingScene
   }

   private static Scene _targetScene;
   public static void Load(Scene scene)
   {
      SceneManager.LoadScene(Scene.LoadingScene.ToString());

      _targetScene = scene;
   }

   public static void LoadTargetScene()
   {
      SceneManager.LoadScene(_targetScene.ToString());

   }
}
