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
    private const float PipeDestroyXPosition = -100f;
    private const float PipeSpawnXPosition = 100f;
    private const float GroundDestroyXPosition = -200f;
    private const float CloudDestroyXPosition = -160f;
    private const float CloudSpawnXPosition = +80f;
    private const float CloudSpawnYPosition = +30f;
    private const float BirdXPosition = 0f;
    private const float GroundWidth = 180f;
    private const float CloudWidth = 60f;

    private static Level _instance;
    private float gapSize;

    private List<Transform> groundList;
    private List<Transform> cloudList;
    private List<Pipe> pipesList;
    private float cloudSpawnTimer;
    private int pipesPassedCount;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private int pipesSpawned;
    private State state;

    private void Awake()
    {
        _instance = this;
        pipesList = new List<Pipe>();
        // SpawnInitialClouds();
        SpawnInitialGround();
        pipeSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStartPlaying += Bird_OnStartPlaying;
    }

    private void Update()
    {
        if (state == State.Playing)
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
        state = State.Playing;
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        state = State.BirdDead;
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
                // Pipe passed bird
                if (pipe.IsBottom())
                {
                    pipesPassedCount++;
                    SoundManager.PlaySound(SoundManager.Sound.Score);
                }

            if (pipe.GetXPosition() < PipeDestroyXPosition)
            {
                pipesList.Remove(pipe);
                pipe.DestroySelf();
                index--;
            }
        }
    }

    private void SpawnInitialClouds()
    {
        cloudList = new List<Transform>();
    
        const float cloudStartY = -70f;
        
        Transform cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(cloudStartY, CloudSpawnYPosition, 0), quaternion.identity);
        cloudList.Add(cloudTransform);
        cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(cloudStartY + CloudWidth, CloudSpawnYPosition, 0), quaternion.identity);
        cloudList.Add(cloudTransform);
        cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(cloudStartY + (CloudWidth * 2f), CloudSpawnYPosition, 0), quaternion.identity);
        cloudList.Add(cloudTransform);
        
    }

    private void SpawnInitialGround()
    {
        groundList = new List<Transform>();

        const float groundY = -47.5f;
        Transform groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(0, groundY, 0), quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(GroundWidth, groundY, 0), quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(GroundWidth * 2f, groundY, 0), quaternion.identity);
        groundList.Add(groundTransform);
    }

    private Transform GetCloudPrefabTransform()
    {
        var random = Random.Range(0, 3);
        return random switch
        {
            1 => GameAssets.GetInstance().pfCloud_1,
            2 => GameAssets.GetInstance().pfCloud_2,
            _ => GameAssets.GetInstance().pfCloud_3
        };
    }
    
    private Transform GetPipeHeadPrefabTransform()
    {
        var random = Random.Range(0, 2);
        return random switch
        {
            1 => GameAssets.GetInstance().pfPipeHead_1,
            _ => GameAssets.GetInstance().pfPipeHead_2,
        };
    }
    
    private void HandleClouds()
    {
        // Handle Cloud Spawning
        cloudSpawnTimer -= Time.deltaTime;
        if (cloudSpawnTimer < 0)
        {
            // Time to spawn another cloud
            const float cloudSpawnTimerMax = 3f;
            cloudSpawnTimer = cloudSpawnTimerMax;
            Transform groundTransform = Instantiate(GameAssets.GetInstance().pfCloud_1, new Vector3(CloudSpawnXPosition, CloudSpawnYPosition, 0), quaternion.identity);
            cloudList.Add(groundTransform);

        }
        
        // Handle Cloud Moving
        for (var i = 0; i < cloudList.Count; i++)
        {
            Transform cloudTransform = cloudList[i];
            // move clouds with less speed for parralax effect
            cloudTransform.position += new Vector3(-1, 0, 0) * (PipeMoveSpeed * Time.deltaTime * 0.7f);

            if (!(cloudTransform.position.x < CloudDestroyXPosition)) continue;
            // Cloud past destroy point, destroy self
            Destroy(cloudTransform.gameObject);
            cloudList.RemoveAt(i);
            i--;
        }
    }


    private void HandleGround()
    {
        foreach (Transform groundTransform in groundList)
        {
            groundTransform.position += new Vector3(-1, 0, 0) * (PipeMoveSpeed * Time.deltaTime);

            if (!(groundTransform.position.x < GroundDestroyXPosition)) continue;
            // Ground passed the left side go relocate on the right side
                
            // Find the right most x position
            var rightMostXPosition = -100f;
            foreach (Transform t in groundList)
            {
                if (t.position.x > rightMostXPosition)
                {
                    rightMostXPosition = t.position.x;
                }
            }
                
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
                gapSize = 40f;
                pipeSpawnTimerMax = 1.4f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                pipeSpawnTimerMax = 1.3f;
                break;
            case Difficulty.Hard:
                gapSize = 35f;
                pipeSpawnTimerMax = 1.1f;
                break;
            case Difficulty.Impossible:
                gapSize = 25f;
                pipeSpawnTimerMax = 1f;
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

        var pipe = new Pipe(pipeHead, pipeBody, createBottom);

        pipesList.Add(pipe);
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
        private readonly bool isBottom;
        private readonly Transform pipeBodyTransform;
        private readonly Transform pipeHeadTransform;

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