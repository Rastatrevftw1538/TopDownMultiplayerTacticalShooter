using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Assertions.Must;
using System.Diagnostics.Eventing.Reader;

[CreateAssetMenu(fileName = "Wave", menuName = "Levels and Waves/Wave")]
public class Wave : ScriptableObject
{
    [NonReorderable] public List<WaveEnemy> _enemies;
    public float _waveValue;

    private List<GameObject> enemiesToSpawn = new List<GameObject>();
    [HideInInspector] public int _amtEnemies;
    private float tempWaveValue;
    private bool isComplete;

    public void GenerateEnemies(GameObject spawnAreas)
    {
        tempWaveValue = _waveValue;
        ResetData();

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

    public void ResetData()
    {
        isComplete = false;
        _waveValue = tempWaveValue;
        enemiesToSpawn.Clear();
        _waveValue = tempWaveValue;
        _amtEnemies = 0;
    }

    public void SpawnEnemies(GameObject spawnArea)
    {
        foreach(GameObject enemy in enemiesToSpawn)
        {
            //this ensures enemies dont spawn pixel perfectly on top of each other
            int rando = Random.Range(0, spawnArea.transform.childCount);
            float randomOffset = Random.Range(0, 0.5f);
            GameObject currentSpawn = spawnArea.transform.GetChild(rando).gameObject;
            Vector3 minorDiff = new Vector3(currentSpawn.transform.position.x, currentSpawn.transform.position.y + randomOffset, currentSpawn.transform.position.z);
            
            //Ensures enemy spawns on the correct axis
            Quaternion quaternion = new Quaternion();
            quaternion.x = 0;
            enemy.transform.rotation = quaternion;

            Instantiate(enemy, minorDiff, Quaternion.identity);
            _amtEnemies++;
        }
    }

    public float AmtEnemies()
    {
        //return _spawnedEnemies.Count;
        return _amtEnemies;
    }

    public void MarkComplete(bool set)
    {
        Debug.LogError(this.name + " has been set to: " + set);
        isComplete = set;
    }

    public bool IsComplete()
    {
        return isComplete;
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
