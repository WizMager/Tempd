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
        
        private EntityManager _entityManager;
        private bool _isHandleEnteredIp;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
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

        private void OnIpEntered(string enteredIpAddress)
        {
            if (_isHandleEnteredIp)
                return;
            
            if (string.IsNullOrEmpty(enteredIpAddress))
            {
                enteredIpAddress = _enterIpAddressInputField.text;
            }
            
            _isHandleEnteredIp = true;
            
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
            _enterIpButton.onClick.RemoveAllListeners();
            _acceptNameButton.onClick.RemoveAllListeners();
            _readyButton.onClick.RemoveAllListeners();
            _startGameButton.onClick.RemoveAllListeners();
        }
    }
}