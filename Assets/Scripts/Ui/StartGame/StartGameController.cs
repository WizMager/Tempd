using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using StartGame.Components;
using StartGame.Utils;
using TMPro;
using Unity.Collections;
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
        
        private EntityManager _localWorldEntityManager;
        private EntityManager _clientWorldEntityManager;
        private bool _hasClientWorldEntityManager;
        private readonly Dictionary<int, PlayerListElement> _playerListElements = new();
        private readonly List<int> _toRemove = new();

        private void Start()
        {
            SceneFlowHelper.EnsureNetworkBootLoaded();

            _localWorldEntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            ConnectionStatusNotifier.OnConnectionStatusChanged += OnConnectionStatusChanged;
            SceneFlowHelper.GameStarted += OnGameStarted;
            SceneFlowHelper.ReturnedToMenu += OnReturnedToMenu;

            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
            _closeJoinPopupButton.onClick.AddListener(OnCloseJoinPopupClicked);
            _enterIpButton.onClick.AddListener(() => OnIpEntered(null));
            _acceptNameButton.onClick.AddListener(OnAcceptNameClicked);
            _readyButton.onClick.AddListener(OnReadyClicked);
            _startGameButton.onClick.AddListener(OnStartGameClicked);
            _exitButton.onClick.AddListener(OnExitClicked);
            
            _enterIpAddressInputField.onEndEdit.AddListener(OnIpEntered);

            _startGameButton.gameObject.SetActive(false);
            ShowMenu();
        }

        private void OnConnectionStatusChanged(ConnectionState.State state, NetworkStreamDisconnectReason disconnectReason)
        {
            if (state == ConnectionState.State.Disconnected)
            {
                Debug.Log($"Disconnect reason: {disconnectReason}");
            }

            switch (state)
            {
                case ConnectionState.State.Disconnected:
                    _hasClientWorldEntityManager = false;
                    SceneFlowHelper.UnloadGameIfLoaded();
                    ShowMenu();
                    break;
                case ConnectionState.State.Connected:
                    _clientWorldEntityManager = ClientServerBootstrap.ClientWorld.EntityManager;
                    _hasClientWorldEntityManager = true;
                    _startGameButton.gameObject.SetActive(ClientServerBootstrap.ServerWorld is { IsCreated: true });
                    
                    ShowLobby();
                    break;
            }
        }

        private void OnGameStarted()
        {
            _blockInputObject.SetActive(false);
            _menuContainer.SetActive(false);
            _joinPopupContainer.SetActive(false);
            _lobbyContainer.SetActive(false);
        }

        private void OnReturnedToMenu()
        {
            ShowMenu();
        }

        private void ShowMenu()
        {
            _blockInputObject.SetActive(false);
            
            _menuContainer.SetActive(true);
            _joinPopupContainer.SetActive(false);
            _lobbyContainer.SetActive(false);
            _startGameButton.gameObject.SetActive(false);
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
            _localWorldEntityManager.CreateEntity(typeof(HostRequestComponent));
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
            
            var entity = _localWorldEntityManager.CreateEntity();
            _localWorldEntityManager.AddComponentData(entity, new JoinRequestComponent
            {
                EnteredIpAddress = enteredIpAddress
            });
        }

        private void OnAcceptNameClicked()
        {
            if (string.IsNullOrEmpty(_enterNameInputField.text))
                return;
            
            var rpcEntity = _clientWorldEntityManager.CreateEntity();
            _clientWorldEntityManager.AddComponentData(rpcEntity, new ChangeNameRpc
            {
                Name = _enterNameInputField.text
            });
            _clientWorldEntityManager.AddComponent<SendRpcCommandRequest>(rpcEntity);
        }
        
        private void OnReadyClicked()
        {
            _clientWorldEntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ReadyStatusChangeRpc));
        }

        private void OnStartGameClicked()
        {
            _clientWorldEntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(StartGameRequestRpc));
        }
        
        private void OnExitClicked()
        {
            _blockInputObject.SetActive(true);
            _localWorldEntityManager.CreateEntity(typeof(LeaveRequestComponent));
        }

        private void Update()
        {
            if (!_hasClientWorldEntityManager)
                return;
            
            using var query = _clientWorldEntityManager.CreateEntityQuery(ComponentType.ReadOnly<LobbyPlayerComponent>());
            var entities = query.ToEntityArray(Allocator.Temp);
            var lobbyPlayer = query.ToComponentDataArray<LobbyPlayerComponent>(Allocator.Temp);
            var alive = new HashSet<int>();

            for (var i = 0; i < entities.Length; i++)
            {
                var index = entities[i].Index;
                alive.Add(index);
                
                if (!_playerListElements.TryGetValue(entities[i].Index, out var player))
                {
                    player = Instantiate(_playerListElementprefab, _playerListRoot).GetComponent<PlayerListElement>();
                    _playerListElements.Add(entities[i].Index, player);
                }
                
                
                player.ChangePlayerName(lobbyPlayer[i].Name.Value);
                player.ChangeReadyStatus(lobbyPlayer[i].IsReady);
            }
            
            _toRemove.Clear();

            foreach (var key in _playerListElements.Keys)
            {
                if (alive.Contains(key))
                    continue;
                
                _toRemove.Add(key);
            }

            foreach (var indexToRemove in _toRemove)
            {
                Destroy(_playerListElements[indexToRemove].gameObject);
                _playerListElements.Remove(indexToRemove);
            }
        }

        private void OnDestroy()
        {
            ConnectionStatusNotifier.OnConnectionStatusChanged -= OnConnectionStatusChanged;
            SceneFlowHelper.GameStarted -= OnGameStarted;
            SceneFlowHelper.ReturnedToMenu -= OnReturnedToMenu;
            
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