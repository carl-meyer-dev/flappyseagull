using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Background : MonoBehaviour
{
    private const float BackgroundSpeed = 10f;
    private const float BuildingsFarDestroyXPosition = -270f;
    private const float BuildingsFarWidth = 400f;
    private const float BuildingsFrontDestroyXPosition = -300f;
    private const float BuildingsFrontWidth = 400f;

    private List<Transform> _buildingsFarList;

    private List<Transform> _buildingsFrontList;

    private List<Transform> _layer2Buildings;

    private State _state;


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
        if (_state == State.Playing)
        {
            HandleBuildingsFarMovement();
            HandleBuildingsFrontMovement();
        }
    }

    private void SpawnInitialBuildingsFar()
    {
        _buildingsFarList = new List<Transform>();

        const float buildingsFarY = -14f;

        Transform groundTransform = Instantiate(GameAssets.GetInstance().pfBuildingsFarBackground,
            new Vector3(0, buildingsFarY, 0), quaternion.identity);
        _buildingsFarList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfBuildingsFarBackground,
            new Vector3(BuildingsFarWidth, buildingsFarY, 0), quaternion.identity);
        _buildingsFarList.Add(groundTransform);
    }

    private void SpawnInitialBuildingsFront()
    {
        _buildingsFrontList = new List<Transform>();

        const float buildingsFrontY = -15f;

        Transform groundTransform = Instantiate(GameAssets.GetInstance().pfBuildingsFrontBackground,
            new Vector3(0, buildingsFrontY, 0), quaternion.identity);
        _buildingsFrontList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfBuildingsFrontBackground,
            new Vector3(BuildingsFrontWidth, buildingsFrontY, 0), quaternion.identity);
        _buildingsFrontList.Add(groundTransform);
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        _state = State.BirdDead;
    }

    private void Bird_OnStartPlaying(object sender, EventArgs e)
    {
        _state = State.Playing;
    }

    private void HandleBuildingsFrontMovement()
    {
        const float parallaxEffectMultiplayer = .75f;

        foreach (Transform buildingsFarGroup in _buildingsFrontList)
        {
            buildingsFarGroup.position +=
                new Vector3(-1, 0, 0) * (BackgroundSpeed * Time.deltaTime * parallaxEffectMultiplayer);

            if (!(buildingsFarGroup.position.x < BuildingsFrontDestroyXPosition)) continue;
            // Buildings Group passed the left side go relocate on the right side

            // Find the right most x position
            var rightMostXPosition = -100f;
            foreach (Transform t in _buildingsFrontList)
                if (t.position.x > rightMostXPosition)
                    rightMostXPosition = t.position.x;

            // Place ground on the right most position
            Vector3 position = buildingsFarGroup.position;
            position = new Vector3(rightMostXPosition + BuildingsFrontWidth, position.y,
                position.z);
            buildingsFarGroup.position = position;
        }
    }

    private void HandleBuildingsFarMovement()
    {
        const float parallaxEffectMultiplayer = .5f;

        foreach (Transform buildingsFarGroup in _buildingsFarList)
        {
            buildingsFarGroup.position +=
                new Vector3(-1, 0, 0) * (BackgroundSpeed * Time.deltaTime * parallaxEffectMultiplayer);

            if (!(buildingsFarGroup.position.x < BuildingsFarDestroyXPosition)) continue;
            // Buildings Group passed the left side go relocate on the right side

            // Find the right most x position
            var rightMostXPosition = -100f;
            foreach (Transform t in _buildingsFarList)
                if (t.position.x > rightMostXPosition)
                    rightMostXPosition = t.position.x;

            // Place ground on the right most position
            Vector3 position = buildingsFarGroup.position;
            position = new Vector3(rightMostXPosition + BuildingsFarWidth, position.y,
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