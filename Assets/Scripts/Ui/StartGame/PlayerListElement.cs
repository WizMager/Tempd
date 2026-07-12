using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.StartGame
{
    public class PlayerListElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private Image _readyImage;

        public void ChangePlayerName(string playerName)
        {
            _playerNameText.text = playerName;
        }

        public void ChangeReadyStatus(bool ready)
        {
            _readyImage.color = ready ? Color.green : Color.red;
        }
    }
}