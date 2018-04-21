using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour {
    public float minObjectSpawnTime;
    public float maxObjectSpawnTime;
    public int maxItemsToSpawn;

    private float objectSpawnTimer;
    public LayerMask collisionsMask;
    public RaycastHit hit;
    private Vector3 point;

    public List<GameObject> itemList;

    public float spawnRadius;
    // Use this for initialization
    void Start () {
        objectSpawnTimer = Random.Range(minObjectSpawnTime, maxObjectSpawnTime);
    }
	
	// Update is called once per frame
	void Update () {
        if(maxItemsToSpawn > 0)
        {
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(transform.position, Vector3.down * 5f, out hit, collisionsMask);
            Debug.DrawRay(transform.position, Vector2.down * 5f, Color.red);

            if (objectSpawnTimer < 0)
            {
                point = new Vector3(Random.Range(hit.point.x - spawnRadius, hit.point.x + spawnRadius - 1), hit.point.y + 2.75f, Random.Range(hit.point.z - spawnRadius, hit.point.z + spawnRadius - 1));

                while (Physics.CheckSphere(point, .5f, collisionsMask) == true)
                {
                    point = new Vector3(Random.Range(hit.point.x - spawnRadius, hit.point.x + spawnRadius - 1), hit.point.y + 2.75f, Random.Range(hit.point.z - spawnRadius, hit.point.z + spawnRadius - 1));
                }

                int i = itemList.Count;
                i = Random.Range(0, i);
                Instantiate(itemList[i], point, Quaternion.identity);
                objectSpawnTimer = minObjectSpawnTime;
                maxItemsToSpawn -= 1;
            }

            objectSpawnTimer -= Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Physics.Raycast(transform.position, Vector3.down * 5f, out hit, collisionsMask);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .5f);
        Gizmos.DrawWireSphere(hit.point, spawnRadius);
    }
}
