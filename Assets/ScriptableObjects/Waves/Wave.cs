using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Wave")]
public class Wave : ScriptableObject
{
    public GameObject _spawnAreas;
    [NonReorderable] public List<WaveEnemy> _enemies;
    public float _waveValue;
    public List<GameObject> enemiesToSpawn;
    [HideInInspector] public int _amtEnemies;
    private float _amtSpawnAreas;

    public void GenerateEnemies()
    {
        List<GameObject> _generatedEnemies = new List<GameObject>();
        while (_waveValue > 0)
        {
            int randEnemyId = Random.Range(0, _enemies.Count);
            int randEnemyCost = _enemies[randEnemyId].cost;

            if (_waveValue - randEnemyCost >= 0)
            {
                int rando = Random.Range(0, _spawnAreas.transform.childCount);
                GameObject currentSpawn = _spawnAreas.transform.GetChild(rando).gameObject;
                //_spawnedEnemies.Add(Instantiate(_enemies[randEnemyId].enemyPrefab, currentSpawn.transform));
                _generatedEnemies.Add(_enemies[randEnemyId].enemyPrefab);

                //this._amtEnemies++;
                //Debug.LogError("there are now" + _amtEnemies);
                _waveValue -= randEnemyCost;
                //Debug.LogError("wave value now at" + _waveValue);
            }
            else
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = _generatedEnemies;
    }

    public float AmtEnemies()
    {
        //return _spawnedEnemies.Count;
        return _amtEnemies;
    }

    private int GetSpawnAreaCount()
    {
        return _spawnAreas.transform.childCount;
    }
}
