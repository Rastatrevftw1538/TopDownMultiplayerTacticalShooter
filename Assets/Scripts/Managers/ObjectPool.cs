using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    private List<GameObject> pooledObjects = new List<GameObject>();
    public int amtToPool = 20;

    private List<AudioSource> pooledAudioSrcs = new List<AudioSource>();
    public int amtAudioToPool = 20;

    [SerializeField] GameObject prefab;
    [SerializeField] AudioSource audioPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        for(int i = 0; i < amtToPool; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }

        for (int i = 0; i < amtAudioToPool; i++)
        {
            AudioSource obj = Instantiate(audioPrefab);
            obj.gameObject.SetActive(false);
            pooledAudioSrcs.Add(obj);
        }
    }

    void Update()
    {
        
    }

    public GameObject GetPooledObject()
    {
        Debug.LogError("Got pooled obj ");
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                Debug.LogError(" returning -> " + pooledObjects[i].name);
                return pooledObjects[i];
            }
        }

        //if you got here, then the pool is full. So, instantiate a new object and return it.
        GameObject inst = Instantiate(prefab);
        inst.SetActive(false);
        pooledObjects.Add(inst);
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
}
