using System;
using System.Net;
using System.Net.Sockets;
using StartGame.Components;
using StartGame.Utils;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
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
        [SerializeField] private Button _exitButton;

        [SerializeField] private TMP_InputField _enterIpAddressInputField;
        [SerializeField] private TMP_InputField _enterNameInputField;

        [SerializeField] private GameObject _menuContainer;
        [SerializeField] private GameObject _joinPopupContainer;
        [SerializeField] private GameObject _lobbyContainer;
        [SerializeField] private GameObject _blockInputObject;
        
        [SerializeField] private Transform _playerListRoot;
        [SerializeField] private PlayerListElement _playerListElementprefab;
        
        private EntityManager _entityManager;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            ConnectionStatusNotifier.OnConnectionStatusChanged += OnConnectionStatusChanged;
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
            _closeJoinPopupButton.onClick.AddListener(OnCloseJoinPopupClicked);
            _enterIpButton.onClick.AddListener(() => OnIpEntered(null));
            _acceptNameButton.onClick.AddListener(OnAcceptNameClicked);
            _readyButton.onClick.AddListener(OnReadyClicked);
            _startGameButton.onClick.AddListener(OnStartGameClicked);
            _exitButton.onClick.AddListener(OnExitClicked);
            
            _enterIpAddressInputField.onEndEdit.AddListener(OnIpEntered);
        }

        private void OnConnectionStatusChanged(ConnectionState.State state, NetworkStreamDisconnectReason disconnectReason)
        {
            Debug.Log($"State: {state}");
            if (state == ConnectionState.State.Disconnected)
            {
                Debug.Log($"Disconnect reason: {disconnectReason}");
            }

            switch (state)
            {
                case ConnectionState.State.Disconnected:
                    ShowMenu();
                    break;
                case ConnectionState.State.Connected:
                    ShowLobby();
                    break;
            }
        }

        private void ShowMenu()
        {
            _blockInputObject.SetActive(false);
            
            _menuContainer.SetActive(true);
            _joinPopupContainer.SetActive(false);
            _lobbyContainer.SetActive(false);
        }

        private void ShowLobby()
        {
            _blockInputObject.SetActive(false);
            
            _menuContainer.SetActive(false);
            _lobbyContainer.SetActive(true);
        }

        private void OnHostClicked()
        {
            _blockInputObject.SetActive(true);
            _entityManager.CreateEntity(typeof(HostRequestComponent));
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

            _blockInputObject.SetActive(true);
            
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
        
        private void OnExitClicked()
        {
            _blockInputObject.SetActive(true);
            _entityManager.CreateEntity(typeof(LeaveRequestComponent));
        }
        
        private void OnDestroy()
        {
            ConnectionStatusNotifier.OnConnectionStatusChanged -= OnConnectionStatusChanged;
            
            _hostButton.onClick.RemoveAllListeners();
            _joinButton.onClick.RemoveAllListeners();
            _closeJoinPopupButton.onClick.RemoveAllListeners();
            _enterIpButton.onClick.RemoveAllListeners();
            _acceptNameButton.onClick.RemoveAllListeners();
            _readyButton.onClick.RemoveAllListeners();
            _startGameButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
            
            _enterIpAddressInputField.onEndEdit.RemoveAllListeners();
        }
    }
}