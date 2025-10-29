using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Spawner Setting")]
    [SerializeField] private GameObject[] monsterPrefab;
    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private int maxMonster = 0;
    [SerializeField] private float respawnTime = 7f;

    private List<GameObject> liveMonster = new List<GameObject>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for(int i = 0; i<maxMonster; i++)
        {
            SpawnMonster(Random.Range(0, spawnPoint.Length));
        }
    }

    private void SpawnMonster(int index)
    {
        if(spawnPoint.Length == 0 || monsterPrefab.Length == 0)
        {
            return;
        }

        Transform point = spawnPoint[index];
        GameObject prefab = monsterPrefab[Random.Range(0, monsterPrefab.Length)];
        GameObject monster = Instantiate(prefab, point.position, Quaternion.identity);

        MonsterBase baseScript = monster.GetComponent<MonsterBase>();
        if(baseScript != null)
        {
            baseScript.SetGameManager(this);
        }

        liveMonster.Add(monster);
    }

    public void MonsterDeath(GameObject monster)
    {
        if(liveMonster.Contains(monster))
        {
            liveMonster.Remove(monster);
        }
    }
}
