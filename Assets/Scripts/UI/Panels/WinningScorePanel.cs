using UnityEngine;

namespace UI.Panels
{
    public static class WinningScorePanel
    {
        private static WinningScore _winningScorePanelPrefab;

        private static WinningScore _instance;
        
        public static bool Initialized { get; private set; }
        
        public static bool IsActive => _instance != null;
        
        public static Vector3 position = new(0, -100, 0);
        
        public static int Result { get; private set; } = 31;

        public static bool Completed { get; private set; } = false;
        
        private static void Initialize()
        {
            if (Initialized) return;
            
            _winningScorePanelPrefab = Resources.Load<WinningScore>("Prefabs/Panel_WinningScore");
            
            Initialized = _winningScorePanelPrefab != null;
        }

        public static void GetWinningScore()
        {
            if (!Initialized) Initialize();
            
            Completed = false;
            
            if (_instance != null) return;
            
            var canvas = Object.FindObjectOfType<Canvas>().transform;
            
            _instance = Object.Instantiate(_winningScorePanelPrefab, canvas);
            
            if (_instance == null)
            {
                Debug.LogError("Panel instance is null.");
                return;
            }
            
            _instance.transform.localPosition = position;
            
            _instance.OnConfirm += SetWinningScore;
        }
        
        private static void SetWinningScore(int score)
        {
            _instance.OnConfirm -= SetWinningScore;
            Result = score;
            Object.Destroy(_instance.gameObject);
            _instance = null;
            Completed = true;
        }
    }
}