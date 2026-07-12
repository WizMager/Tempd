using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.StartGame
{
    public class StartGameController : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _enterIpButton;
        [SerializeField] private Button _acceptNameButton;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _startGameButton;

        [SerializeField] private TMP_InputField _enterIpAddressInputField;
        [SerializeField] private TMP_InputField _enterNameInputField;

        [SerializeField] private GameObject _menuContainer;
        [SerializeField] private GameObject _joinPopupContainer;
        [SerializeField] private GameObject _lobbyContainer;
        
        [SerializeField] private Transform _playerListRoot;
        [SerializeField] private PlayerListElement _playerListElementprefab;
    }
}