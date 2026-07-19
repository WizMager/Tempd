using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.StartGame
{
    public class PlayerListElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private Image _readyImage;
        
        private bool _isReady;
        
        public void ChangePlayerName(string playerName)
        {
            if (_playerNameText.text == playerName)
                return;
            
            _playerNameText.text = playerName;
        }

        public void ChangeReadyStatus(bool ready)
        {
            if (_isReady == ready)
                return;
            
            _isReady = ready;

            _readyImage.color = ready ? Color.green : Color.red;
        }
    }
}