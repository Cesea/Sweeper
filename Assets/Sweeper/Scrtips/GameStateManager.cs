using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

using Foundation;
using Utils;

public class GameStateManager : SingletonBase<GameStateManager>
{
    private BoardManager _boardManager;
    private EnemyManager _enemyManager;

    [Header("Prefabs")]
    public GameObject _exclamationPrefab;

    public float _delayTime = 1.0f;

    private List<GameObject> _exclamations;

    [SerializeField]
    private PlayerBoardObject _player;
    public PlayerBoardObject Player { get { return _player; } set { _player = value; } }

    public CameraController _cameraController;

    public Level.LevelCursor _levelCursor;

    private bool _levelStarted = false;
    public bool LevelStarted { get { return _levelStarted; } set { _levelStarted = value; } }
    private bool _isGamePlaying = false;
    public bool IsGamePlaying { get { return _isGamePlaying; } set { _isGamePlaying = value; } }
    private bool _isGameOver = false;
    public bool IsGameOver { get { return _isGameOver; } set { _isGameOver = value; } }
    private bool _levelFinished = false;
    public bool LevelFinished { get { return _levelFinished; } set { _levelFinished = value; } }

    //private bool _playerDied = false;

    public UnityEvent StartLevelEvent;
    public UnityEvent PlayLevelEvent;
    public UnityEvent EndLevelEvent;


    private void OnEnable()
    {
        EventManager.Instance.AddListener<Events.RadarSkillEvent>(OnRadarSkillEvent);
    }

    private void OnDisable()
    {
        EventManager.Instance.AddListener<Events.RadarSkillEvent>(OnRadarSkillEvent);
    }


    private void Start()
    {
        _boardManager = BoardManager.Instance;
        _enemyManager = EnemyManager.Instance;

        _exclamations = new List<GameObject>();

        _levelCursor.gameObject.SetActive(false);

        if (_exclamationPrefab != null &&
            _boardManager != null)
        {
            StartCoroutine("RunGameLoop");
        }
        else
        {
            Debug.LogWarning("Cannot start game");
        }
    }

    private IEnumerator RunGameLoop()
    {
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        yield return StartCoroutine("EndLevelRoutine");
    }

    private IEnumerator StartLevelRoutine()
    {
        _player.CanReceiveCommand = false;

        SetupNextBoard();
        yield return null;        

        SetupPlayer();
        yield return null;        

        SetupEnemy();
        yield return null;        

        while (!_levelStarted)
        {
            yield return null;
        }

        if (StartLevelEvent != null)
        {
            StartLevelEvent.Invoke();
        }

        _levelStarted = false;
    }

    private IEnumerator PlayLevelRoutine()
    {
        _isGamePlaying = true;
        yield return new WaitForSeconds(_delayTime);

        _levelCursor.gameObject.SetActive(true);

        if (PlayLevelEvent != null)
        {
            PlayLevelEvent.Invoke();
        }

        _player.CanReceiveCommand = true;
        while (!_isGameOver)
        {
            //check for game over condition
            //win
            //reach the end
            if (_levelFinished)
            {
                break;
            }

            //lose
            //player dies
            if (!_player.GetComponent<BoardHealth>().Alive)
            {
                break;
            }

            //_isGameOver = true
            yield return null;
        }

        _isGamePlaying = false;
        _isGameOver = false;
    }

    private IEnumerator EndLevelRoutine()
    {
        _player.CanReceiveCommand = false;

        if (EndLevelEvent != null)
        {
            //NOTE : true value is temp
            EndLevelEvent.Invoke();
        }
        //show end screen
        while (!_levelFinished)
        {
            //user presses button to continue
            _levelFinished = true;
            yield return null;
        }

        RestartLevel();
        _levelFinished = false;
    }

    void RestartLevel()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Instance.GetCurrentScene().buildIndex);
    }


    public void PlayLevel()
    {
        _levelStarted = true;
    }

    public void SetupNextBoard()
    {
        _boardManager.BuildNewBoard();
        RemoveExclamations();

        _cameraController.transform.position = new Vector3(
            _boardManager.WorldSize.x / 2.0f,
            _cameraController.transform.position.y,
            _boardManager.WorldSize.z / 2.0f);
    }

    public void SetupPlayer()
    {
        InGameHUD.Show(Player);
        Player.SetSittingNode(_boardManager.CurrentBoard.StartCellCoord, Side.Top);
    }

    public void SetupEnemy()
    {
        _enemyManager.SpawnEnemy(3, 0, 3, Side.Top);
    }

    public void SpawnExclamation(Transform playerTransform)
    {
        GameObject go = Instantiate(_exclamationPrefab, playerTransform.position + Vector3.up, Quaternion.identity);
        go.transform.SetParent(transform);
        _exclamations.Add(go);
    }

    public void RemoveExclamations()
    {
        if (_exclamations.Count > 0)
        {
            for (int i = _exclamations.Count - 1; i >= 0; --i)
            {
                Destroy(_exclamations[i]);
            }
            _exclamations.Clear();
        }
    }


    public void OnRadarSkillEvent(Events.RadarSkillEvent e)
    {
        //Node[] playerAdjacentCells = Player.AdjacentCells;

        //int count = 0;
        //for (int z = 0; z < 3; ++z)
        //{
        //    for (int x = 0; x < 3; ++x)
        //    {
        //        if (x == 1 && z == 1)
        //        {
        //            continue;
        //        }
        //        Node currentNode = playerAdjacentCells[count];
        //        if (currentNode != null)
        //        {
        //            if (currentNode.IsHazard)
        //            {
        //                SpawnExclamation(currentNode.X, currentNode.Y, currentNode.Z);
        //            }
        //        }
        //        count++;
        //    }
        //}
    }

    //public void OnPlayerDied()
    //{
    //    _playerDied = true;        
    //}
}