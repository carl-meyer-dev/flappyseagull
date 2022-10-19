using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Background : MonoBehaviour
{
    private const float BackgroundSpeed = 10f;
    private const float buildingsFarDestroyXPosition = -270f;
    private const float buildingsFarWidth = 400f;
    private const float buildingsFrontDestroyXPosition = -300f;
    private const float buildingsFrontWidth = 400f;

    private List<Transform> buildingsFarList;

    private List<Transform> buildingsFrontList;

    private List<Transform> layer2Buildings;

    private State state;


    private void Awake()
    {
        SpawnInitialBuildingsFar();
        SpawnInitialBuildingsFront();
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStartPlaying += Bird_OnStartPlaying;
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == State.Playing)
        {
            HandleBuildingsFarMovement();
            HandleBuildingsFrontMovement();
        }
    }

    private void SpawnInitialBuildingsFar()
    {
        buildingsFarList = new List<Transform>();

        const float buildingsFarY = -14f;

        Transform groundTransform = Instantiate(GameAssets.GetInstance().pfBuildingsFarBackground,
            new Vector3(0, buildingsFarY, 0), quaternion.identity);
        buildingsFarList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfBuildingsFarBackground,
            new Vector3(buildingsFarWidth, buildingsFarY, 0), quaternion.identity);
        buildingsFarList.Add(groundTransform);
    }

    private void SpawnInitialBuildingsFront()
    {
        buildingsFrontList = new List<Transform>();

        const float buildingsFrontY = -15f;

        Transform groundTransform = Instantiate(GameAssets.GetInstance().pfBuildingsFrontBackground,
            new Vector3(0, buildingsFrontY, 0), quaternion.identity);
        buildingsFrontList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfBuildingsFrontBackground,
            new Vector3(buildingsFrontWidth, buildingsFrontY, 0), quaternion.identity);
        buildingsFrontList.Add(groundTransform);
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        state = State.BirdDead;
    }

    private void Bird_OnStartPlaying(object sender, EventArgs e)
    {
        state = State.Playing;
    }

    private void HandleBuildingsFrontMovement()
    {
        const float parallaxEffectMultiplayer = .75f;

        foreach (Transform buildingsFarGroup in buildingsFrontList)
        {
            buildingsFarGroup.position +=
                new Vector3(-1, 0, 0) * (BackgroundSpeed * Time.deltaTime * parallaxEffectMultiplayer);

            if (!(buildingsFarGroup.position.x < buildingsFrontDestroyXPosition)) continue;
            // Buildings Group passed the left side go relocate on the right side

            // Find the right most x position
            var rightMostXPosition = -100f;
            foreach (Transform t in buildingsFrontList)
                if (t.position.x > rightMostXPosition)
                    rightMostXPosition = t.position.x;

            // Place ground on the right most position
            Vector3 position = buildingsFarGroup.position;
            position = new Vector3(rightMostXPosition + buildingsFrontWidth, position.y,
                position.z);
            buildingsFarGroup.position = position;
        }
    }

    private void HandleBuildingsFarMovement()
    {
        const float parallaxEffectMultiplayer = .5f;

        foreach (Transform buildingsFarGroup in buildingsFarList)
        {
            buildingsFarGroup.position +=
                new Vector3(-1, 0, 0) * (BackgroundSpeed * Time.deltaTime * parallaxEffectMultiplayer);

            if (!(buildingsFarGroup.position.x < buildingsFarDestroyXPosition)) continue;
            // Buildings Group passed the left side go relocate on the right side

            // Find the right most x position
            var rightMostXPosition = -100f;
            foreach (Transform t in buildingsFarList)
                if (t.position.x > rightMostXPosition)
                    rightMostXPosition = t.position.x;

            // Place ground on the right most position
            Vector3 position = buildingsFarGroup.position;
            position = new Vector3(rightMostXPosition + buildingsFarWidth, position.y,
                position.z);
            buildingsFarGroup.position = position;
        }
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        BirdDead
    }
}