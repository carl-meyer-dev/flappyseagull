using System;
using System.Collections.Generic;
using CodeMonkey;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour
{
    private const float PipeBodyWidth = 7.8f;
    private const float PipeHeadHeight = 3.75f;
    private const float CameraOrthoSize = 50f;
    private const float PipeMoveSpeed = 30f;
    private const float PipeDestroyXPosition = -100f;
    private const float PipeSpawnXPosition = 100f;
    private const float BirdXPosition = 0f;

    private static Level _instance;

    public static Level GetInstance()
    {
        return _instance;
    }

    private List<Pipe> pipesList;
    private int pipesPassedCount;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;

    private enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }

    private enum State
    {
        Playing,
        BirdDead
    }

    private void Awake()
    {
        _instance = this;
        pipesList = new List<Pipe>();
        pipeSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
        state = State.Playing;
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        state = State.BirdDead;

        FunctionTimer.Create(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }, 1f);
    }

    private void Update()
    {
        if (state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0)
        {
            // Time to spawn another pipe
            pipeSpawnTimer += pipeSpawnTimerMax;

            var height = CalcRandomHeight();
            CreateGapPipes(height, gapSize, PipeSpawnXPosition);
            pipesSpawned++;
            SetDifficulty(GetDifficulty());
        }
    }

    private float CalcRandomHeight()
    {
        const float heightEdgeLimit = 10f;
        var minHeight = gapSize * 0.5f + heightEdgeLimit;
        const float totalHeight = CameraOrthoSize * 2f;
        var maxHeight = totalHeight - gapSize * 0.5f - heightEdgeLimit;

        return Random.Range(minHeight, maxHeight);
    }

    private void HandlePipeMovement()
    {
        for (var index = 0; index < pipesList.Count; index++)
        {
            Pipe pipe = pipesList[index];
            var isToTheRightOfBird = pipe.GetXPosition() > BirdXPosition;
            pipe.Move();
            if (isToTheRightOfBird && pipe.GetXPosition() <= BirdXPosition)
            {
                // Pipe passed bird
                if (pipe.IsBottom())
                {
                    pipesPassedCount++;
                }
            }

            if (pipe.GetXPosition() < PipeDestroyXPosition)
            {
                pipesList.Remove(pipe);
                pipe.DestroySelf();
                index--;
            }
        }
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50f;
                pipeSpawnTimerMax = 1.2f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                pipeSpawnTimerMax = 1.1f;
                break;
            case Difficulty.Hard:
                gapSize = 35f;
                pipeSpawnTimerMax = 1.0f;
                break;
            case Difficulty.Impossible:
                gapSize = 25f;
                pipeSpawnTimerMax = 0.9f;
                break;
        }
    }

    private Difficulty GetDifficulty()
    {
        switch (pipesSpawned)
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

        Pipe pipe = new Pipe(pipeHead, pipeBody, createBottom);

        pipesList.Add(pipe);
    }

    private Transform CreatePipeHead(float height, float xPosition, bool createBottom)
    {
        var pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);

        float pipeHeadYPosition;

        if (createBottom)
        {
            pipeHeadYPosition = -CameraOrthoSize + height - PipeHeadHeight * 0.5f;
        }
        else
        {
            pipeHeadYPosition = +CameraOrthoSize - height + PipeHeadHeight * 0.5f;
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
        return pipesSpawned;
    }

    public int GetPipesPassedCount()
    {
        return pipesPassedCount;
    }

    /**
     * Represents a single Pipe
     */
    private class Pipe
    {
        private readonly Transform pipeHeadTransform;
        private readonly Transform pipeBodyTransform;
        private readonly bool isBottom;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
        }

        public void Move()
        {
            pipeHeadTransform.position += Vector3.left * (PipeMoveSpeed * Time.deltaTime);
            pipeBodyTransform.position += Vector3.left * (PipeMoveSpeed * Time.deltaTime);
        }

        public float GetXPosition()
        {
            return pipeHeadTransform.position.x;
        }

        public bool IsBottom()
        {
            return isBottom;
        }

        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}