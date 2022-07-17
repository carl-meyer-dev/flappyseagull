using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
   private const float PIPE_BODY_WIDTH = 7.8f;
   private const float PIPE_HEAD_HEIGHT = 3.75f;
   
   private void Start()
   {
      CreatePipe(50f, 0f);
      CreatePipe(50f, 20f);
   }

   public void CreatePipe(float height, float xPosition)
   {
      Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
      pipeHead.position = new Vector3(xPosition, height - PIPE_HEAD_HEIGHT * 0.5f);
      
      Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
      pipeBody.position = new Vector3(xPosition, 0f);
      SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
      pipeBodySpriteRenderer.size = new Vector2(PIPE_BODY_WIDTH, height);
   }
}
