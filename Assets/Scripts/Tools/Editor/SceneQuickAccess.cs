using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tools.Editor
{
    public class SceneQuickAccess : OdinEditorWindow
    {
        [MenuItem("Tools/Scene Quick Access")]
        public static void OpenWindow()
        {
            GetWindow<SceneQuickAccess>().Show();
        }
        
        [HorizontalGroup("Buttons"), Button(ButtonSizes.Large)]
        private void MainMenu()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
        
        [HorizontalGroup("Buttons"), Button(ButtonSizes.Large)]
        private void OfflineScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/OfflineTarneeb.unity");
        }
    }
}
