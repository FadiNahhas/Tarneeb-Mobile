using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public static class DisplayNamePanel
    {
        private static GameObject _displayNamePanel; // Prefab reference
    
        private static Transform _instance; // Panel instance
        private static TMP_InputField _displayNameInputField; // Panel instance input field
        private static Button _displayNameSubmitButton; // Panel instance submit button
    
        public static bool Initialized { get; private set; }
        public static bool IsActive => _instance != null;

        public static string Result { get; private set; } = string.Empty;
        public static Vector3 position = new(0, -100, 0);

        private static void Initialize()
        {
            if (Initialized) return;
        
            _displayNamePanel = Resources.Load<GameObject>("Prefabs/Panel_DisplayName");
        
            Initialized = _displayNamePanel != null;
        }

        public static void GetDisplayName()
        {
            if (!Initialized) Initialize();
            if (_instance != null) return; 
            var canvas = Object.FindObjectOfType<Canvas>().transform;
        
            _instance = Object.Instantiate(_displayNamePanel, canvas).transform;
        
            if (_instance == null)
            {
                Debug.LogError("Panel instance is null.");
                return;
            }
        
            _instance.localPosition = position;
            _displayNameInputField = _instance.GetComponentInChildren<TMP_InputField>();
            _displayNameSubmitButton = _instance.GetComponentInChildren<Button>();
        
            _displayNameSubmitButton.onClick.AddListener(SetDisplayName);
        }

        private static void SetDisplayName()
        {
            Result = _displayNameInputField.text;
            Object.Destroy(_instance.gameObject);
            ResetReferences();
        }

        private static void ResetReferences()
        {
            _instance = null;
            _displayNameInputField = null;
            _displayNameSubmitButton = null;
        }
    }
}
