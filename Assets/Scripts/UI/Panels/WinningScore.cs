using System;
using UnityEngine;

namespace UI.Panels
{
    public class WinningScore : MonoBehaviour
    {
        private int _winningScore = 31;

        public event Action<int> OnConfirm;
        
        public void Confirm()
        {
            OnConfirm?.Invoke(_winningScore);
        }
        
        public void SetWinningScore(int score)
        {
            _winningScore = score;
        }
    }
}