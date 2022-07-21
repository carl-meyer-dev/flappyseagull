using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const float PipeBodyWidth = 7.8f;
    private const float PipeHeadHeight = 3.75f;
    private const float CameraOrthoSize = 50f;
    private const float PipeMoveSpeed = 30f;
    private const float PipeDestroyXPosition = -100f;
    private const float PipeSpawnXPosition = 100f;

    private List<Pipe> pipesList;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    
    private enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }


    private void Awake()
    {
        pipesList = new List<Pipe>();
        pipeSpawnTimerMax = 1f;
    }

    private void Update()
    {
        HandlePipeMovement();
        HandlePipeSpawning();
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
        }
    }

    private float CalcRandomHeight()
    {
        const float heightEdgeLimit = 10f;
        var minHeight = gapSize * 05f + heightEdgeLimit;
        const float totalHeight = CameraOrthoSize * 2f;
        var maxHeight = totalHeight - gapSize * 0.5f - heightEdgeLimit;

        return Random.Range(minHeight, maxHeight);
    }

    private void HandlePipeMovement()
    {
        for (var index = 0; index < pipesList.Count; index++)
        {
            Pipe pipe = pipesList[index];
            pipe.Move();
            if (pipe.GetXPosition() < PipeDestroyXPosition)
            {
                pipesList.Remove(pipe);
                pipe.DestroySelf();
                index--;
            }
        }
    }

    private void Start()
    {
        CreateGapPipes(50f, 20f, 20f);
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * 0.5f, xPosition, true);
        CreatePipe(CameraOrthoSize * 2f - gapY - gapSize * 0.5f, xPosition, false);
    }

    private void CreatePipe(float height, float xPosition, bool createBottom)
    {
        Transform pipeHead = CreatePipeHead(height, xPosition, createBottom);
        Transform pipeBody = CreatePipeBody(height, xPosition, createBottom);

        Pipe pipe = new Pipe(pipeHead, pipeBody);

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

    /**
     * Represents a single Pipe
     */
    private class Pipe
    {
        private readonly Transform pipeHeadTransform;
        private readonly Transform pipeBodyTransform;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
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

        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}