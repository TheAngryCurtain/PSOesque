﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eEnemyType { Low, Medium, High, Boss };

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] m_EnemyPrefabs;

    private int m_RoomID;
    private float m_RoomWidth;
    private float m_RoomLength;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.PlayerEnteredRoomEvent>(OnPlayerEnteredRoom);
    }

    public void SetRoomID(int id)
    {
        m_RoomID = id;
    }

    public void SetRoomBoundaries(float width, float length)
    {
        m_RoomWidth = width;
        m_RoomLength = length;
    }

    private void OnPlayerEnteredRoom(GameEvents.PlayerEnteredRoomEvent e)
    {
        if (m_RoomID == e.roomID)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        Debug.Log("Spawing enemies...");

        Vector3 spawnPos = transform.position;
        float roomHalfWidth = m_RoomWidth / 2f;
        float roomHalfLength = m_RoomLength / 2f;

        float roomMinX = spawnPos.x - roomHalfWidth;
        float roomMaxX = spawnPos.x + roomHalfWidth;
        float roomMinZ = spawnPos.z - roomHalfLength;
        float roomMaxZ = spawnPos.z + roomHalfLength;

        // TODO check difficulty or something to determine how many of each we need to spawn

        int numLow = 3;
        int numMed = 2;
        int numHigh = 1;

        int total = numLow + numMed + numHigh;
        for (int i = 0; i < total; i++)
        {
            float randX = UnityEngine.Random.Range(roomMinX, roomMaxX);
            float randZ = UnityEngine.Random.Range(roomMinZ, roomMaxZ);

            Vector3 randPos = new Vector3(randX, 1f, randZ);

            if (i < numLow)
            {
                Instantiate(m_EnemyPrefabs[(int)eEnemyType.Low], randPos, Quaternion.identity);
            }
            else if (i < numLow + numMed)
            {
                Instantiate(m_EnemyPrefabs[(int)eEnemyType.Medium], randPos, Quaternion.identity);
            }
            else
            {
                Instantiate(m_EnemyPrefabs[(int)eEnemyType.High], randPos, Quaternion.identity);
            }
        }
    }
}
