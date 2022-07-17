using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const float PipeBodyWidth = 7.8f;
    private const float PipeHeadHeight = 3.75f;
    private const float CameraOrthoSize = 50f;

    private void Start()
    {
        CreateGapPipes(50f, 20f, 20f);
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * 0.5f, xPosition, true); 
        CreatePipe(CameraOrthoSize * 2f  - gapY - gapSize * 0.5f, xPosition, false); 
    }

    public void CreatePipe(float height, float xPosition, bool createBottom)
    {
        SetupPipeHead(height, xPosition, createBottom);
        SetupPipeBody(height, xPosition, createBottom);
    }

    private void SetupPipeHead(float height, float xPosition, bool createBottom)
    {
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);

        float pipeHeadYPosition;

        if (createBottom)
        {
            pipeHeadYPosition = -CameraOrthoSize + height - PipeHeadHeight * 0.5f;
        }
        else
        {
            pipeHeadYPosition = +CameraOrthoSize - height + PipeHeadHeight * 0.5f;
        }

        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);;
    }


    private void SetupPipeBody(float height, float xPosition, bool createBottom)
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
        
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);;

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PipeBodyWidth, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PipeBodyWidth, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * 0.5f);
    }
}