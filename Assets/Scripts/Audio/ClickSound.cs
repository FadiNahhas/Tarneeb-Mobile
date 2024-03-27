using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public class ClickSound : MonoBehaviour
    {
        [SerializeField] private string soundName;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }
    
        private void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick() => AudioManager.Instance.PlaySound(soundName);
    }
}
