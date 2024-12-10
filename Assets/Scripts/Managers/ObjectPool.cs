using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    private List<GameObject> pooledProjObjects = new List<GameObject>();
    public int amtProjToPool = 20;

    private List<GameObject> pooledBulletObjects = new List<GameObject>();
    public int amtBulletsToPool = 8;

    private List<AudioSource> pooledAudioSrcs = new List<AudioSource>();
    public int amtAudioToPool = 20;

    private List<GameObject> pooledProjAdvObjects = new List<GameObject>();
    public int amtProjAdvToPool = 30;

    private List<GameObject> pooledEnemyDisplay = new List<GameObject>();
    public int amtDisplayToPool = 20;

    [SerializeField] GameObject prefab;
    [SerializeField] AudioSource audioPrefab;
    [SerializeField] GameObject projPrefab;
    [SerializeField] GameObject projAdvPrefab;
    [SerializeField] GameObject hitDisplayPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //move into delegates and <T> later
    //this is so ugly i know
    void Start()
    {
        for(int i = 0; i < amtBulletsToPool; i++)
        {
            GameObject obj = Instantiate(prefab, gameObject.transform);
            obj.SetActive(false);
            pooledBulletObjects.Add(obj);
        }

        for (int i = 0; i < amtAudioToPool; i++)
        {
            AudioSource obj = Instantiate(audioPrefab, gameObject.transform);
            obj.gameObject.SetActive(false);
            pooledAudioSrcs.Add(obj);
        }

        for (int i = 0; i < amtProjToPool; i++)
        {
            GameObject obj = Instantiate(projPrefab, gameObject.transform);
            obj.SetActive(false);
            pooledProjObjects.Add(obj);
        }

        for (int i = 0; i < amtProjAdvToPool; i++)
        {
            GameObject obj = Instantiate(projAdvPrefab, gameObject.transform);
            obj.SetActive(false);
            pooledProjAdvObjects.Add(obj);
        }

        for (int i = 0; i < amtDisplayToPool; i++)
        {
            GameObject obj = Instantiate(hitDisplayPrefab, gameObject.transform);
            obj.SetActive(false);
            pooledEnemyDisplay.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        //Debug.LogError("Got pooled obj ");
        for (int i = 0; i < pooledBulletObjects.Count; i++)
        {
            if (!pooledBulletObjects[i].activeInHierarchy)
            {
                //Debug.LogError(" returning -> " + pooledBulletObjects[i].name);
                return pooledBulletObjects[i];
            }
        }

        //if you got here, then the pool is full. So, instantiate a new object and return it.
        GameObject inst = Instantiate(prefab);
        inst.SetActive(false);
        pooledBulletObjects.Add(inst);
        return inst;
    }

    public AudioSource GetPooledAudioSource()
    {
        for(int i = 0; i < pooledAudioSrcs.Count; i++)
        {
            if (!pooledAudioSrcs[i].gameObject.activeInHierarchy)
            {
                return pooledAudioSrcs[i];
            }
        }

        //if you got here, then the pool is full. So, instantiate a new object and return it.
        AudioSource inst = Instantiate(audioPrefab);
        inst.gameObject.SetActive(false);
        pooledAudioSrcs.Add(inst);
        return inst;
    }

    public GameObject GetPooledProjObject()
    {
       // Debug.LogError("Got pooled obj ");
        for (int i = 0; i < pooledProjObjects.Count; i++)
        {
            if (!pooledProjObjects[i].activeInHierarchy)
            {
                //Debug.LogError(" returning -> " + pooledProjObjects[i].name);
                return pooledProjObjects[i];
            }
        }

        //if you got here, then the pool is full. So, instantiate a new object and return it.
        GameObject inst = Instantiate(projPrefab);
        inst.SetActive(false);
        pooledProjObjects.Add(inst);
        return inst;
    }

    public GameObject GetPooledProjAdvObject()
    {
        for (int i = 0; i < pooledProjAdvObjects.Count; i++)
        {
            if (!pooledProjAdvObjects[i].activeInHierarchy)
            {
                //Debug.LogError(" returning -> " + pooledProjObjects[i].name);
                return pooledProjAdvObjects[i];
            }
        }

        //if you got here, then the pool is full. So, instantiate a new object and return it.
        GameObject inst = Instantiate(projAdvPrefab);
        inst.SetActive(false);
        pooledProjAdvObjects.Add(inst);
        return inst;
    }

    public GameObject GetPooledDisplayHit()
    {
        //Debug.LogError("Got pooled obj ");
        for (int i = 0; i < pooledEnemyDisplay.Count; i++)
        {
            if (!pooledEnemyDisplay[i].activeInHierarchy)
            {
                //Debug.LogError(" returning -> " + pooledBulletObjects[i].name);
                return pooledEnemyDisplay[i];
            }
        }

        //if you got here, then the pool is full. So, instantiate a new object and return it.
        GameObject inst = Instantiate(hitDisplayPrefab);
        inst.SetActive(false);
        pooledEnemyDisplay.Add(inst);
        return inst;
    }
}
