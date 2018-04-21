using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public LayerMask collisionMask;

    public List<Transform> okayLootSpawns;
    public List<Transform> goodLootSpawns;
    public List<Transform> rareLootSpawns;

    public List<ItemData> okayItems;
    public List<ItemData> goodItems;
    public List<ItemData> rareItems;

    public GameObject itemPrefab;

    public float goodSpawnChance;
    public float rareSpawnChance;

    public int itemsAtStart = 10;
    public int maxItemsToSpawn;
    
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 5f;

    private float spawnTimer;
    private RaycastHit hit;

    public void Awake()
    {
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);

        for (int i=0; i<itemsAtStart; i++)
        {
            SpawnRandomItem();
        }
    }

    public void Update()
    {
        if(maxItemsToSpawn > 0)
        {
            if (spawnTimer < 0)
            {
                SpawnRandomItem();
            }

            spawnTimer -= Time.deltaTime;
        }
    }

    private void SpawnRandomItem()
    {
        if(maxItemsToSpawn > 0)
        {
            float perc = Random.Range(0f, 1f);

            if (perc > rareSpawnChance)
            {
                Transform t = rareLootSpawns[Random.Range(0, rareLootSpawns.Count)];
                ItemData id = rareItems[Random.Range(0, rareItems.Count)];
                SpawnItem(t, id);
                maxItemsToSpawn -= 1;
            }
            else if (perc > goodSpawnChance)
            {
                Transform t = goodLootSpawns[Random.Range(0, goodLootSpawns.Count)];
                ItemData id = goodItems[Random.Range(0, goodItems.Count)];
                SpawnItem(t, id);
                maxItemsToSpawn -= 1;
            }
            else
            {
                Transform t = okayLootSpawns[Random.Range(0, okayLootSpawns.Count)];
                ItemData id = okayItems[Random.Range(0, okayItems.Count)];
                SpawnItem(t, id);
                maxItemsToSpawn -= 1;
            }
        }
        
    }

    private void SpawnItem(Transform t, ItemData itemData)
    {
        if(maxItemsToSpawn > 0)
        {
            Physics.Raycast(t.position, Vector3.down * 5f, out hit, collisionMask);

            Vector3 point;
            do
            {
                float xPos = Random.Range(hit.point.x - t.localScale.x / 2, hit.point.x + t.localScale.x / 2);
                float yPos = hit.point.y + 2.75f;
                float zPos = Random.Range(hit.point.z - t.localScale.x / 2, hit.point.z + t.localScale.x / 2);
                point = new Vector3(xPos, yPos, zPos);
            }
            while (Physics.CheckSphere(point, .75f, collisionMask));

            GameObject go = Instantiate(itemPrefab, transform);
            go.transform.position = point;
            go.GetComponent<GameItem>().itemData = itemData;

            spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
            maxItemsToSpawn -= 1;
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Transform t in goodLootSpawns)
        {
            Physics.Raycast(t.position, Vector3.down, out hit, collisionMask);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(t.position, .5f);
            Gizmos.DrawWireCube(hit.point, Vector3.one * t.localScale.x);
        }

        foreach (Transform t in rareLootSpawns)
        {
            Physics.Raycast(t.position, Vector3.down, out hit, collisionMask);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(t.position, .5f);
            Gizmos.DrawWireCube(hit.point, Vector3.one * t.localScale.x);
        }

        foreach (Transform t in okayLootSpawns)
        {
            Physics.Raycast(t.position, Vector3.down, out hit, collisionMask);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(t.position, .5f);
            Gizmos.DrawWireCube(hit.point, Vector3.one * t.localScale.x);
        }
    }
}
