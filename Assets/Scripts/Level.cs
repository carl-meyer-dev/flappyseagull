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
        CreatePipe(50f, 0f, true);
        CreatePipe(40f, 20f, false);
        CreatePipe(30f, 40f, true);
        CreatePipe(20f, 60f, false);
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