using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : NetworkBehaviour
{
    private List<LobbyPlayerPanel> playerPanels;

    public GameObject playerScrollContent;
    public TMP_Text txtPlayerNumber;
    public Button btnStart;
    public Button btnReady;
    public LobbyPlayerPanel playerPanelPrefab;
    public Button btnSubmitName = null;
    [SerializeField] public GameObject areYouSurePanel;
    [SerializeField] public GameObject characterSelected;
    [SerializeField] public TMP_Text playerNameText;


    public void Awake() {
        playerPanels = new List<LobbyPlayerPanel>();
        btnSubmitName = GameObject.FindGameObjectWithTag("SubmitButton").GetComponent<Button>();

        if (btnSubmitName != null)
        {
            btnSubmitName.onClick.AddListener(GameData.Instance.SubmitButtonClicked);
        }
    }

    public void Start() {
        if (IsHost) {
            RefreshPlayerPanels();
            btnStart.onClick.AddListener(HostOnBtnStartClick);
        }

        if (IsClient) {
            btnReady.onClick.AddListener(ClientOnReadyClicked);
        }
    }

    public override void OnNetworkSpawn() {
        if (IsHost) {
            NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected;
            btnReady.gameObject.SetActive(false);
        }

        if (IsClient && !IsHost) {
            btnStart.gameObject.SetActive(false);
        }

        txtPlayerNumber.text = $"Player #{NetworkManager.LocalClientId}";
        GameData.Instance.allPlayers.OnListChanged += ClientOnAllPlayersChanged;
        EnableStartIfAllReady();
    }

    public override void OnDestroy()
    {
        GameData.Instance.allPlayers.OnListChanged -= ClientOnAllPlayersChanged;
    }
    
    private void AddPlayerPanel(PlayerInfo info) {
        LobbyPlayerPanel newPanel = Instantiate(playerPanelPrefab);
        newPanel.transform.SetParent(playerScrollContent.transform, false);
        newPanel.SetName(info.m_PlayerName);
        newPanel.SetColor(info.color);
        newPanel.SetReady(info.isReady);
        playerPanels.Add(newPanel);
    }

    private void RefreshPlayerPanels() {
        foreach (LobbyPlayerPanel panel in playerPanels) {
            Destroy(panel.gameObject);
        }
        playerPanels.Clear();

        foreach (PlayerInfo pi in GameData.Instance.allPlayers) {
            AddPlayerPanel(pi);
        }
    }

    public void StartGame()
    {
        var scene = NetworkManager.SceneManager.LoadScene("Arena1", LoadSceneMode.Single);
        btnStart.enabled = false;

    }

    private void EnableStartIfAllReady() {
        int readyCount = 0;
        foreach (PlayerInfo readyInfo in GameData.Instance.allPlayers) {
            if (readyInfo.isReady) {
                readyCount += 1;
            }
        }

        // btnStart.enabled = readyCount == GameData.Instance.allPlayers.Count;
        // if (btnStart.enabled) {
        //     btnStart.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        // } else {
        //     btnStart.GetComponentInChildren<TextMeshProUGUI>().text = "Waiting for Ready";
        // }
    }
    
    private void ClientOnAllPlayersChanged(NetworkListEvent<PlayerInfo> changeEvent) {
        RefreshPlayerPanels();
    }

    private void HostOnBtnStartClick() {
        Debug.Log("Start Game");
        StartGame();
    }

    private void HostOnClientConnected(ulong clientId) {
        EnableStartIfAllReady();
    }

    private void ClientOnReadyClicked() {
        ToggleReadyServerRpc();
    }
    
    /// <summary>
    /// POPUP
    /// </summary>
    /// <param name="pName"></param>
    /// <param name="currMesh"></param>
    public void ActivateAreYouSurePopUp(string pName, int currMesh)
    {
        string playerName = pName;
        areYouSurePanel.SetActive(true);
        Sprite[] listOfImages = GameData.Instance.spawnedPlayer.GetComponent<PlayerHUDManager>().listOfSprites;
        playerNameText.text = playerName;
        Sprite selectedChar = listOfImages[currMesh - 1];
        characterSelected.GetComponent<Image>().sprite = selectedChar;
        Debug.Log($"{pName} has mesh {currMesh}");
    }

    public void BackPopUp()
    {
        areYouSurePanel.SetActive(false);
        GameData.Instance.leftSelectorBut.gameObject.SetActive(true);
        GameData.Instance.rightSelectorBut.gameObject.SetActive(true);
        GameData.Instance.selectPlayMeshBut.gameObject.SetActive(true);
    }

    public void ContinueButton()
    {
        ContinueButtonValues(GameData.Instance.playerName, GameData.Instance.intOfCurrentMesh);
    }
    public void ContinueButtonValues(string pName, int currMesh)
    {
        GameObject.Find("AreYouSurePanel").SetActive(false);
        //GameData.Instance.SendPNameServerRpc(pName, currMesh);
        Debug.Log($"{pName} has mesh {currMesh}");
    }

    
    [ServerRpc(RequireOwnership = false)]
    public void ToggleReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        int playerIndex = GameData.Instance.FindPlayerIndex(clientId);
        PlayerInfo info = GameData.Instance.allPlayers[playerIndex];

        info.isReady = !info.isReady;
        GameData.Instance.allPlayers[playerIndex] = info;

        EnableStartIfAllReady();
    }

}
