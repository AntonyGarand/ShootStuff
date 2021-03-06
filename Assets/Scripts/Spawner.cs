﻿using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public bool debugMode;

    public Wave[] waves;
    public Enemy enemy;
    public Material tileMaterial;
    Color tileColor;

    LivingEntity playerEntity;
    Transform playerTransform;

    Wave currentWave;
    int currentWaveNumber;

    int enemyRemauningToSpawn;
    int enemyRemainingToKill;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenMoveCheck = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 lastPlayerPosition;
    bool isCamping;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    void Start()
    {
        tileColor = tileMaterial.color;
        playerEntity = FindObjectOfType<Player>();
        playerTransform = playerEntity.transform;

        nextCampCheckTime = timeBetweenMoveCheck + Time.time;
        lastPlayerPosition = playerTransform.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenMoveCheck;

                isCamping = Vector3.Distance(playerTransform.position, lastPlayerPosition) < campThresholdDistance;
                lastPlayerPosition = playerTransform.position;
            }


            if ((enemyRemauningToSpawn > 0 || currentWave.lastWaveInfinite )&& Time.time > nextSpawnTime)
            {
                enemyRemauningToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");
            }
        }
        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StopCoroutine("SpawnEnemy");
                foreach(Enemy opponent in FindObjectsOfType<Enemy>())
                {
                    Destroy(opponent.gameObject);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpentile();
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerTransform.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(tileColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.damagePerHit, currentWave.health, currentWave.skinColor);


    }
    void OnEnemyDeath()
    {
        enemyRemainingToKill--;
        if (enemyRemainingToKill <= 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerTransform.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    void OnPlayerDeath() {
        enemyRemainingToKill--;
        if (enemyRemainingToKill <= 0)
        {
            NextWave();
        }
        isDisabled = true;
    }

    void NextWave() {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemyRemauningToSpawn = currentWave.enemyCount;
            enemyRemainingToKill = enemyRemauningToSpawn;

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int damagePerHit;
        public float health;
        public Color skinColor;

        public bool lastWaveInfinite;
    }

}
