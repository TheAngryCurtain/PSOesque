using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCrateType { Common, Rare, VeryRare };

public class CrateSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] m_CratePrefabs;
    [SerializeField] private int[] m_CrateProbabilities = new int[3];

    private bool m_Spawned = false;
    private int m_RoomID;

    private int m_ProbabilitySum = 0;
    private float m_RoomWidth;
    private float m_RoomLength;

    private void Awake()
    {
        Debug.Log("<!> EnemySpawner and CrateSpawner can be refactored to use a BaseSpawner because there is so much common functionality!");

        VSEventManager.Instance.AddListener<GameEvents.PlayerEnteredRoomEvent>(OnPlayerEnteredRoom);

        for (int i = 0; i < m_CrateProbabilities.Length; i++)
        {
            m_ProbabilitySum += m_CrateProbabilities[i];
        }
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
        if (m_RoomID == e.RoomID)
        {
            if (!m_Spawned)
            {
                SpawnCrates(e.Player);
                m_Spawned = true;
            }
        }
    }

    private void SpawnCrates(Transform playerTransform)
    {
        Vector3 spawnPos = transform.position;
        float roomHalfWidth = m_RoomWidth / 2f;
        float roomHalfLength = m_RoomLength / 2f;

        float roomMinX = spawnPos.x - roomHalfWidth;
        float roomMaxX = spawnPos.x + roomHalfWidth;
        float roomMinZ = spawnPos.z - roomHalfLength;
        float roomMaxZ = spawnPos.z + roomHalfLength;

        // TODO check difficulty or something to determine how many of each we need to spawn

        int numCrates = 3;
        for (int i = 0; i < numCrates; i++)
        {
            float randX = UnityEngine.Random.Range(roomMinX, roomMaxX);
            float randZ = UnityEngine.Random.Range(roomMinZ, roomMaxZ);

            Vector3 randPos = new Vector3(randX, 0.5f, randZ);
            Quaternion randRot = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up);
            GameObject crateObj = null;

            int crateTypeIndex = Utils.WeightedRandom(m_CrateProbabilities);
            Debug.LogFormat(">> Crate is {0}", (eCrateType)crateTypeIndex);

            crateObj = (GameObject)Instantiate(m_CratePrefabs[crateTypeIndex], randPos, randRot);
        }
    }
}
