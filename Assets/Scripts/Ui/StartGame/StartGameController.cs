using System.Net;
using System.Net.Sockets;
using StartGame.Components;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.StartGame
{
    public class StartGameController : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _closeJoinPopupButton;
        [SerializeField] private Button _enterIpButton;
        [SerializeField] private Button _acceptNameButton;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _startGameButton;

        [SerializeField] private TMP_InputField _enterIpAddressInputField;
        [SerializeField] private TMP_InputField _enterNameInputField;

        [SerializeField] private GameObject _menuContainer;
        [SerializeField] private GameObject _joinPopupContainer;
        [SerializeField] private GameObject _lobbyContainer;
        [SerializeField] private GameObject _blockJoinPopupObject;
        
        [SerializeField] private Transform _playerListRoot;
        [SerializeField] private PlayerListElement _playerListElementprefab;
        
        private EntityManager _entityManager;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
            _closeJoinPopupButton.onClick.AddListener(OnCloseJoinPopupClicked);
            _enterIpButton.onClick.AddListener(() => OnIpEntered(null));
            _acceptNameButton.onClick.AddListener(OnAcceptNameClicked);
            _readyButton.onClick.AddListener(OnReadyClicked);
            _startGameButton.onClick.AddListener(OnStartGameClicked);
            
            _enterIpAddressInputField.onEndEdit.AddListener(OnIpEntered);
        }

        private void OnHostClicked()
        {
            
        }
        
        private void OnJoinClicked()
        {
            _joinPopupContainer.SetActive(true);
        }

        private void OnCloseJoinPopupClicked()
        {
            _joinPopupContainer.SetActive(false);
        }
        
        private void OnIpEntered(string enteredIpAddress)
        {
            enteredIpAddress = "127.0.0.1";//DEBUG!!!
            enteredIpAddress = string.IsNullOrWhiteSpace(enteredIpAddress)
                ? _enterIpAddressInputField.text.Trim()
                : enteredIpAddress.Trim();
            
            var isValidIpv4 = enteredIpAddress.Split('.').Length == 4 
                              && IPAddress.TryParse(enteredIpAddress, out var address) 
                              && address.AddressFamily == AddressFamily.InterNetwork;
            
            if (!isValidIpv4)
            {
                Debug.LogWarning("Введите корректный IPv4, например 127.0.0.1");
                return;
            }

            _blockJoinPopupObject.SetActive(true);
            
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new JoinRequestComponent
            {
                EnteredIpAddress = enteredIpAddress
            });
        }

        private void OnAcceptNameClicked()
        {
            
        }
        
        private void OnReadyClicked()
        {
        }

        private void OnStartGameClicked()
        {
            
        }
        
        private void OnDestroy()
        {
            _hostButton.onClick.RemoveAllListeners();
            _joinButton.onClick.RemoveAllListeners();
            _closeJoinPopupButton.onClick.RemoveAllListeners();
            _enterIpButton.onClick.RemoveAllListeners();
            _acceptNameButton.onClick.RemoveAllListeners();
            _readyButton.onClick.RemoveAllListeners();
            _startGameButton.onClick.RemoveAllListeners();
        }
    }
}