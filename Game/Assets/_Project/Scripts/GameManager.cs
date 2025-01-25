using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameManager : LazySingleton<GameManager>
{
    public class BuildsInfo
    {
        public int nbBuildsRemaining;
        public int nbBuilds;
        public int kingsRemaining;
        public int kingsOnGrid;
        public int[] nbKingsAlive = new int[2];
        public KingComponent[][] kingComponents;
    }

    #region Fields

    public Action RefreshSelecter;
    public int currentPlayerIndex = 0;
    public ConstructionState constructionState = ConstructionState.CurrentPlayerConstructed;
    public bool readyToPlay;
    public bool constructionDone = false;
    public bool inEditMode = true;

    [SerializeField] BuildFactory buildFactory;
    [SerializeField] List<Player> players = new();
    [SerializeField] Transform[] posCamera = new Transform[2];
    [SerializeField] int maxNbShoot;
    [SerializeField] Button buttonEndTurn;
    [SerializeField] Transform player1ButtonPosition, player2ButtonPosition;

    int nbCellsInAllGrid;
    int nbTurn;
    BuildsInfo currentBuildsInfo = new BuildsInfo();
    Player currentPlayer;
    Coroutine cameraMovementCoroutine;

    [SerializeField]
    private Transform[] limitsMap;

    //Timer
    TMP_Text constructTimerText;
    [SerializeField]
    float maxConstructionTime = 60.0f;
    float constructTimer = 0.0f;
    Coroutine constructTimerCoroutine;

    #endregion

    #region Enums
    public enum ConstructionState
    {
        CurrentPlayerConstructed,
        NextPlayerConstructed,
    }
    #endregion

    #region Messages

    protected override void Awake()
    {
        foreach (Player player in players)
        {
            player.turnHasEnded += OnPlayerTurnEnded;
        }

        currentPlayerIndex = 0;
        currentPlayer = players[currentPlayerIndex];
        StartPlayerTurn(currentPlayer);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        players[(int)Player.PlayerID.Player1].ScoreText = GameHudManager.Instance.player1ScoreText;
        players[(int)Player.PlayerID.Player2].ScoreText = GameHudManager.Instance.player2ScoreText;
    }

    private void Start()
    {
        buttonEndTurn.GetComponent<Image>().enabled = false;
    }

    private void Update()
    {
        //nbKingsAlive = currentBuildsInfo.nbKingsAlive[currentPlayerIndex];
        if (Input.GetKeyDown(KeyCode.T))
        {
            constructionDone = true;
            NextPlayerTurn();
        }
    }


    private void OnDestroy()
    {
        foreach (Player player in players)
        {
            player.turnHasEnded -= OnPlayerTurnEnded;
        }
    }

    #endregion

    #region PrivateMethods

    //Warning : The player is the precedent, not the next
    private void StartPlayerTurn(Player player)
    {
        // Debug.Log($"Starting turn for Player {player.playerID}");
        GameHudManager.Instance.DisablePlayerTag(player.playerID);
        CannonManager.Instance.PositionCannon = player.transform.Find("CanonPos").position;
        CannonManager.Instance.ResetCannon();

        float halfWidthCamera = Camera.main.aspect * Camera.main.orthographicSize;

        float pos = 0.0f;
        //Debug.Log(canConstruct);
        if (!constructionDone)
        {
            pos = Mathf.Clamp(player.transform.position.x, limitsMap[0].position.x + halfWidthCamera, limitsMap[1].position.x - halfWidthCamera);
            SetCameraPosition(pos, 2.0f);

            constructTimer = maxConstructionTime;
            if (player.playerID == Player.PlayerID.Player1)
            {
                GameHudManager.Instance.player1TimerText.gameObject.SetActive(true);
                GameHudManager.Instance.player2TimerText.gameObject.SetActive(false);
                constructTimerText = GameHudManager.Instance.player1TimerText;
            }
            else
            {
                GameHudManager.Instance.player1TimerText.gameObject.SetActive(false);
                GameHudManager.Instance.player2TimerText.gameObject.SetActive(true);
                constructTimerText = GameHudManager.Instance.player2TimerText;
            }
            if (constructTimerCoroutine != null)
                StopCoroutine(constructTimerCoroutine);

            constructTimerCoroutine = StartCoroutine(ConstructionTimer());

        }
        else
        {
            buttonEndTurn.GetComponent<Image>().enabled = true;
            constructTimerText.enabled = false;
            if (player.playerID == Player.PlayerID.Player1)
            {
                pos = posCamera[(int)Player.PlayerID.Player1].position.x - halfWidthCamera;
                pos = Mathf.Clamp(pos, limitsMap[0].position.x + halfWidthCamera, limitsMap[1].position.x - halfWidthCamera);
                buttonEndTurn.transform.position = player1ButtonPosition.position;
            }
            else
            {
                pos = posCamera[(int)Player.PlayerID.Player2].position.x + halfWidthCamera;
                pos = Mathf.Clamp(pos, limitsMap[0].position.x + halfWidthCamera, limitsMap[1].position.x - halfWidthCamera);
                buttonEndTurn.transform.position = player2ButtonPosition.position;
            }

            SetCameraPosition(pos, 1.0f);
        }

        if (nbTurn > 1)
        {
            players[currentPlayerIndex == 0 ? 1 : 0].GetComponentInChildren<BuildsStack>().LoadPlayMode(); // ReCall when the second player starts their first turn
        }

        nbTurn++;
    }

    private void OnPlayerTurnEnded(Player player)
    {
        NextPlayerTurn();
    }

    public void NextPlayerTurn()
    {
        CannonManager.Instance.destroyAllLaunchedBullets?.Invoke();
        int previousPlayer = (int)currentPlayer.playerID;
        currentPlayer = GetNextPlayer();
        StartPlayerTurn(currentPlayer);
    }

    private Player GetNextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        return players[currentPlayerIndex];
    }

    public void HandleConstructionPhase()
    {
        switch (constructionState)
        {
            case ConstructionState.CurrentPlayerConstructed:
                //Debug.Log($" CurrentPlayer {currentPlayer.playerID} constructs.");
                currentPlayer.GetComponentInChildren<BuildsStack>().ResetBuilds();
                currentPlayer.EndTurn();
                RefreshSelecter?.Invoke();
                constructionState = ConstructionState.NextPlayerConstructed;
                break;
            case ConstructionState.NextPlayerConstructed:
                //Debug.Log($"NextPlayer {currentPlayer.playerID} constructs.");
                HandleEndOfConstruction();
                TouchManager.Instance.canSlide = true;
                currentPlayer.EndTurn();
                BuildCursor.Instance.LeaveBuildMode();
                break;

        }
    }

    private void HandleEndOfConstruction()
    {
        //Debug.Log("Players are done constructing");
        constructionDone = true;
        CannonManager.Instance.swapButton.SetActive(true);

        currentPlayer.ConstructionFinished = true;
        foreach (Player player in players)
        {
            player.GetComponentInChildren<BuildingGrid>().gameObject.SetActive(false);
        }
    }

    private IEnumerator ConstructionTimer()
    {
        bool isFlashing = false;
        while (constructTimer > 0)
        {
            constructTimer -= Time.deltaTime;

            constructTimer = Mathf.Max(constructTimer, 0);

            int minutes = Mathf.FloorToInt(constructTimer / 60);
            int seconds = Mathf.FloorToInt(constructTimer % 60);

            constructTimerText.text = $"{minutes:D2}:{seconds:D2}";

            if (constructTimer <= 15f && !isFlashing)
            {
                isFlashing = true;
                StartCoroutine(FlashTimer());
            }
            yield return null;
        }

        constructTimerText.text = "00:00";
        constructTimerCoroutine = null;
        HandleConstructionPhase();
    }

    private IEnumerator FlashTimer()
    {
        bool isVisible = true;
        while (constructTimer > 0 && constructTimer <= 15f)
        {
            if (currentPlayer.playerID == Player.PlayerID.Player1)
            {
                constructTimerText.color = isVisible ? Color.blue : Color.yellow;
            }
            else
            {
                constructTimerText.color = isVisible ? Color.red : Color.yellow;
            }

            isVisible = !isVisible;
            yield return new WaitForSeconds(0.2f);
        }
        constructTimerText.color = Color.white;
    }
    ///////////\\\\\\//Reminder for me: Once everything is working, this in a class TODO\\\///////////\\\\\\
    private void SetCameraPosition(float targetX, float duration)
    {
        if (cameraMovementCoroutine != null)
        {
            StopCoroutine(cameraMovementCoroutine);
        }
        cameraMovementCoroutine = StartCoroutine(SmoothCameraMovement(targetX, duration));
    }


    private void ResetCameraPosition(float defaultX)
    {
        if (Camera.main != null)
        {
            Vector3 newCameraPosition = Camera.main.transform.position;
            newCameraPosition.x = defaultX;
            Camera.main.transform.position = newCameraPosition;
        }
    }

    private void SetCameraSize(float targetWidth, float targetHeight)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            if (mainCamera.orthographic)
            {
                // Calculate the orthographic size based on the target width and aspect ratio
                float aspectRatio = targetWidth / targetHeight;
                mainCamera.orthographicSize = targetHeight / 2f;
            }
            else
            {
                // For perspective, adjust the field of view based on the desired dimensions
                float aspectRatio = targetWidth / targetHeight;
                mainCamera.aspect = aspectRatio;
                // Field of View remains as is; aspect ratio change affects X-axis length perception
            }
        }
    }


    private IEnumerator SmoothCameraMovement(float targetX, float duration)
    {
        if (Camera.main != null)
        {
            Vector3 startPosition = Camera.main.transform.position;
            Vector3 targetPosition = new Vector3(targetX, startPosition.y, startPosition.z);
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                yield return null;
            }
            Camera.main.transform.position = targetPosition;
        }
    }
    #endregion

    #region Properties/Accessors
    public int NbCellsInAllGrid { get => nbCellsInAllGrid; set => nbCellsInAllGrid = value; }
    public BuildsInfo CurrentBuildsInfo { get => currentBuildsInfo; set => currentBuildsInfo = value; }
    public BuildFactory BuildFactory { get => buildFactory; }
    public int PlayerCount => players.Count;
    public List<Player> Players => players;
    public Player CurrentPlayer => currentPlayer;
    public Button ButtonEndTurn => buttonEndTurn;

    #endregion
}
