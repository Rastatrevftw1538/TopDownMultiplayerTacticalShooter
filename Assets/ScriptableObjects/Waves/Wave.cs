using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "Wave", menuName = "Wave")]
public class Wave : ScriptableObject
{
    //public GameObject _spawnAreas;
    [NonReorderable] public List<WaveEnemy> _enemies;
    public float _waveValue;
    private List<GameObject> enemiesToSpawn;
    [HideInInspector] public int _amtEnemies;
    private float _amtSpawnAreas;

    public void GenerateEnemies(GameObject spawnAreas)
    {
        float tempWaveValue = _waveValue;
        enemiesToSpawn.Clear();
        _waveValue = tempWaveValue;
        _amtEnemies = 0;

        if (spawnAreas == null)
        {
            Debug.LogError("Cannot find spawnpoint for enemies...");
            return;
        }

        List<GameObject> _generatedEnemies = new List<GameObject>();
        while (_waveValue > 0)
        {
            int randEnemyId = Random.Range(0, _enemies.Count);
            int randEnemyCost = _enemies[randEnemyId].cost;

            if (_waveValue - randEnemyCost >= 0)
            {
                int rando = Random.Range(0, spawnAreas.transform.childCount);
                GameObject currentSpawn = spawnAreas.transform.GetChild(rando).gameObject;
                //_spawnedEnemies.Add(Instantiate(_enemies[randEnemyId].enemyPrefab, currentSpawn.transform));
                _generatedEnemies.Add(_enemies[randEnemyId].enemyPrefab);
                this._amtEnemies++;
                //Debug.LogError("there are now" + _amtEnemies);
                _waveValue -= randEnemyCost;
                //Debug.LogError("wave value now at" + _waveValue);
            }
            else
            {
                break;
            }
        }

        _waveValue = tempWaveValue;
        enemiesToSpawn = _generatedEnemies;
        SpawnEnemies(spawnAreas);
    }

    public void SpawnEnemies(GameObject spawnArea)
    {
        foreach(GameObject enemy in enemiesToSpawn)
        {
            int rando = Random.Range(0, spawnArea.transform.childCount);
            
            //this ensures enemies dont spawn pixel perfectly on top of each other
            float randomOffset = Random.Range(0, 0.5f);
            GameObject currentSpawn = spawnArea.transform.GetChild(rando).gameObject;
            Vector2 minorDiff = new Vector2(currentSpawn.transform.position.x, currentSpawn.transform.position.y + randomOffset);

            Instantiate(enemy, spawnArea.transform.GetChild(rando));
        }
    }

    public float AmtEnemies()
    {
        //return _spawnedEnemies.Count;
        return _amtEnemies;
    }

    public void OnApplicationQuit()
    {
        enemiesToSpawn.Clear();
    }
}

[System.Serializable]
public struct WaveEnemy
{
    public GameObject enemyPrefab;
    public int cost;
}
