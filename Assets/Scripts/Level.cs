using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour
{
    private const float PipeBodyWidth = 10f;
    private const float PipeHeadHeight = 15f;
    private const float CameraOrthoSize = 50f;
    private const float PipeMoveSpeed = 30f;
    private const float PipeDestroyXPosition = -200f;
    private const float PipeSpawnXPosition = 150f;
    private const float GroundDestroyXPosition = -250f;
    private const float CloudDestroyXPosition = -160f;
    private const float CloudSpawnXPosition = +80f;
    private const float CloudSpawnYPosition = +30f;
    private const float BirdXPosition = 0f;
    private const float GroundWidth = 269f;
    private const float CloudWidth = 60f;

    private static Level _instance;
    private List<Transform> _cloudList;
    private float _cloudSpawnTimer;
    private float _gapSize;

    private List<Transform> _groundList;
    private List<Pipe> _pipesList;
    private int _pipesPassedCount;
    private float _pipeSpawnTimer;
    private float _pipeSpawnTimerMax;
    private int _pipesSpawned;
    private State _state;

    private void Awake()
    {
        _instance = this;
        _pipesList = new List<Pipe>();
        // SpawnInitialClouds();
        SpawnInitialGround();
        _pipeSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
        _state = State.WaitingToStart;
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStartPlaying += Bird_OnStartPlaying;
    }

    private void Update()
    {
        if (_state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
            HandleGround();
            // HandleClouds();
        }
    }

    public static Level GetInstance()
    {
        return _instance;
    }

    private void Bird_OnStartPlaying(object sender, EventArgs e)
    {
        _state = State.Playing;
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        _state = State.BirdDead;
    }

    private enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        BirdDead
    } // ReSharper disable Unity.PerformanceAnalysis
    private void HandlePipeSpawning()
    {
        _pipeSpawnTimer -= Time.deltaTime;
        if (_pipeSpawnTimer < 0)
        {
            // Time to spawn another pipe
            _pipeSpawnTimer += _pipeSpawnTimerMax;

            var height = CalcRandomHeight();
            CreateGapPipes(height, _gapSize, PipeSpawnXPosition);
            _pipesSpawned++;
            SetDifficulty(GetDifficulty());
        }
    }

    private float CalcRandomHeight()
    {
        const float heightEdgeLimit = 10f;
        var minHeight = _gapSize * 0.5f + heightEdgeLimit;
        const float totalHeight = CameraOrthoSize * 2f;
        var maxHeight = totalHeight - _gapSize * 0.5f - heightEdgeLimit;

        return Random.Range(minHeight, maxHeight);
    }

    private void HandlePipeMovement()
    {
        for (var index = 0; index < _pipesList.Count; index++)
        {
            Pipe pipe = _pipesList[index];
            var isToTheRightOfBird = pipe.GetXPosition() > BirdXPosition;
            pipe.Move();
            if (isToTheRightOfBird && pipe.GetXPosition() <= BirdXPosition)
                // Pipe passed bird
                if (pipe.IsBottom())
                {
                    _pipesPassedCount++;
                    SoundManager.PlaySound(SoundManager.Sound.Score);
                }

            if (pipe.GetXPosition() < PipeDestroyXPosition)
            {
                _pipesList.Remove(pipe);
                pipe.DestroySelf();
                index--;
            }
        }
    }

    private void SpawnInitialClouds()
    {
        _cloudList = new List<Transform>();

        const float cloudStartY = -70f;

        Transform cloudTransform = Instantiate(GetCloudPrefabTransform(),
            new Vector3(cloudStartY, CloudSpawnYPosition, 0), quaternion.identity);
        _cloudList.Add(cloudTransform);
        cloudTransform = Instantiate(GetCloudPrefabTransform(),
            new Vector3(cloudStartY + CloudWidth, CloudSpawnYPosition, 0), quaternion.identity);
        _cloudList.Add(cloudTransform);
        cloudTransform = Instantiate(GetCloudPrefabTransform(),
            new Vector3(cloudStartY + CloudWidth * 2f, CloudSpawnYPosition, 0), quaternion.identity);
        _cloudList.Add(cloudTransform);
    }

    private void SpawnInitialGround()
    {
        _groundList = new List<Transform>();

        const float groundY = -50f;
        Transform groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(0, groundY, 0),
            quaternion.identity);
        _groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(GroundWidth, groundY, 0),
            quaternion.identity);
        _groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(GroundWidth * 2f, groundY, 0),
            quaternion.identity);
        _groundList.Add(groundTransform);
    }

    private Transform GetCloudPrefabTransform()
    {
        var random = Random.Range(0, 3);
        return random switch
        {
            1 => GameAssets.GetInstance().pfCloud1,
            2 => GameAssets.GetInstance().pfCloud2,
            _ => GameAssets.GetInstance().pfCloud3
        };
    }

    private Transform GetPipeHeadPrefabTransform()
    {
        var random = Random.Range(0, 2);
        return random switch
        {
            1 => GameAssets.GetInstance().pfPipeHead1,
            _ => GameAssets.GetInstance().pfPipeHead2
        };
    }

    private void HandleClouds()
    {
        // Handle Cloud Spawning
        _cloudSpawnTimer -= Time.deltaTime;
        if (_cloudSpawnTimer < 0)
        {
            // Time to spawn another cloud
            const float cloudSpawnTimerMax = 3f;
            _cloudSpawnTimer = cloudSpawnTimerMax;
            Transform groundTransform = Instantiate(GameAssets.GetInstance().pfCloud1,
                new Vector3(CloudSpawnXPosition, CloudSpawnYPosition, 0), quaternion.identity);
            _cloudList.Add(groundTransform);
        }

        // Handle Cloud Moving
        for (var i = 0; i < _cloudList.Count; i++)
        {
            Transform cloudTransform = _cloudList[i];
            // move clouds with less speed for parralax effect
            cloudTransform.position += new Vector3(-1, 0, 0) * (PipeMoveSpeed * Time.deltaTime * 0.7f);

            if (!(cloudTransform.position.x < CloudDestroyXPosition)) continue;
            // Cloud past destroy point, destroy self
            Destroy(cloudTransform.gameObject);
            _cloudList.RemoveAt(i);
            i--;
        }
    }


    private void HandleGround()
    {
        foreach (Transform groundTransform in _groundList)
        {
            groundTransform.position += new Vector3(-1, 0, 0) * (PipeMoveSpeed * Time.deltaTime);

            if (!(groundTransform.position.x < GroundDestroyXPosition)) continue;
            // Ground passed the left side go relocate on the right side

            // Find the right most x position
            var rightMostXPosition = -100f;
            foreach (Transform t in _groundList)
                if (t.position.x > rightMostXPosition)
                    rightMostXPosition = t.position.x;

            // Place ground on the right most position
            Vector3 position = groundTransform.position;
            position = new Vector3(rightMostXPosition + GroundWidth, position.y,
                position.z);
            groundTransform.position = position;
        }
    }


    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                _gapSize = 40f;
                _pipeSpawnTimerMax = 1.4f;
                break;
            case Difficulty.Medium:
                _gapSize = 40f;
                _pipeSpawnTimerMax = 1.3f;
                break;
            case Difficulty.Hard:
                _gapSize = 35f;
                _pipeSpawnTimerMax = 1.1f;
                break;
            case Difficulty.Impossible:
                _gapSize = 25f;
                _pipeSpawnTimerMax = 1f;
                break;
        }
    }

    private Difficulty GetDifficulty()
    {
        switch (_pipesSpawned)
        {
            case >= 30:
                return Difficulty.Impossible;
            case >= 20:
                return Difficulty.Hard;
            case >= 10:
                return Difficulty.Medium;
            default:
                return Difficulty.Easy;
        }
    }

    // ReSharper disable once ParameterHidesMember
    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * 0.5f, xPosition, true);
        CreatePipe(CameraOrthoSize * 2f - gapY - gapSize * 0.5f, xPosition, false);
    }

    private void CreatePipe(float height, float xPosition, bool createBottom)
    {
        Transform pipeHead = CreatePipeHead(height, xPosition, createBottom);
        Transform pipeBody = CreatePipeBody(height, xPosition, createBottom);

        var pipe = new Pipe(pipeHead, pipeBody, createBottom);

        _pipesList.Add(pipe);
    }

    private Transform CreatePipeHead(float height, float xPosition, bool createBottom)
    {
        Transform pipeHead = Instantiate(GetPipeHeadPrefabTransform());

        float pipeHeadYPosition;

        if (createBottom)
        {
            pipeHeadYPosition = -CameraOrthoSize + height - PipeHeadHeight * 0.5f;
        }
        else
        {
            pipeHeadYPosition = +CameraOrthoSize - height + PipeHeadHeight * 0.5f;
            pipeHead.Rotate(0f, 0f, 180f);
        }

        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);

        return pipeHead;
    }


    private Transform CreatePipeBody(float height, float xPosition, bool createBottom)
    {
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);

        float pipeBodyYPosition;

        if (createBottom)
        {
            pipeBodyYPosition = -CameraOrthoSize;
        }
        else
        {
            pipeBodyYPosition = +CameraOrthoSize;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }

        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        var pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PipeBodyWidth, height);

        var pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PipeBodyWidth, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * 0.5f);

        return pipeBody;
    }

    public int GetPipeSpawn()
    {
        return _pipesSpawned;
    }

    public int GetPipesPassedCount()
    {
        return _pipesPassedCount;
    }

    /**
     * Represents a single Pipe
     */
    private class Pipe
    {
        private readonly bool _isBottom;
        private readonly Transform _pipeBodyTransform;
        private readonly Transform _pipeHeadTransform;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom)
        {
            this._pipeHeadTransform = pipeHeadTransform;
            this._pipeBodyTransform = pipeBodyTransform;
            this._isBottom = isBottom;
        }

        public void Move()
        {
            _pipeHeadTransform.position += Vector3.left * (PipeMoveSpeed * Time.deltaTime);
            _pipeBodyTransform.position += Vector3.left * (PipeMoveSpeed * Time.deltaTime);
        }

        public float GetXPosition()
        {
            return _pipeHeadTransform.position.x;
        }

        public bool IsBottom()
        {
            return _isBottom;
        }

        public void DestroySelf()
        {
            Destroy(_pipeHeadTransform.gameObject);
            Destroy(_pipeBodyTransform.gameObject);
        }
    }
}