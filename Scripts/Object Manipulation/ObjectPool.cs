using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    private const string GOName = "ObjectPool";
    private Dictionary<string, Transform> poolContainers;
    private static ObjectPool _instance;
    private static ObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = GameObject.Find(GOName);
            }
            return _instance;
        }
    }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        name = GOName;

        DontDestroyOnLoad(gameObject);
        _instance = this;
        poolContainers = new Dictionary<string, Transform>();
    }

    public static GameObject OPGetObject(string prefabPath)
    {
        var g = Instance.cGetObject(prefabPath);
        return g;
    }
    public static void OPDestroyObject(GameObject go)
    {
        Instance.cDestroyObject(go);
    }
    public static void OPPreloadObjects(string prefabPath, int count)
    {
        //Debug.Log("Preloading " + prefabPath + " " + count + " times");

        Transform poolContainer = Instance.GetPoolContainer(prefabPath);

        for (int i = 0; i < count; i++)
        {
            GameObject go = (GameObject)Instantiate(Resources.Load(prefabPath));
            go.transform.parent = poolContainer;
            go.transform.rotation = Quaternion.identity;
            go.transform.localPosition = Vector3.zero;

            ObjectPoolItem opi = go.AddComponent<ObjectPoolItem>();
            opi.PrefabPath = prefabPath;

            go.SetActive(false);
        }
    }

    private GameObject cGetObject(string prefabPath)
    {
        Transform poolContainer = GetPoolContainer(prefabPath);

        GameObject newObj;
        ObjectPoolItem opi;
        if (poolContainer.childCount > 0)
        {
            newObj = poolContainer.GetChild(0).gameObject;
            newObj.gameObject.SetActive(true);
            newObj.BroadcastMessage("Awake", SendMessageOptions.DontRequireReceiver);
            newObj.BroadcastMessage("Start", SendMessageOptions.DontRequireReceiver);
            opi = newObj.GetComponent<ObjectPoolItem>();
            if (opi == null)
            {
                //Debug.LogError("no object pool item");
            }
        }
        else
        {
            //Debug.Log("New Object");
            newObj = (GameObject)Instantiate(Resources.Load(prefabPath));
            opi = newObj.AddComponent<ObjectPoolItem>();
        }

        var rb = newObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        newObj.transform.parent = null;
        newObj.BroadcastMessage("OPStart", SendMessageOptions.DontRequireReceiver);
        opi.PrefabPath = prefabPath;

        return newObj;
    }
    private void cDestroyObject(GameObject go)
    {
        //Debug.Break();
        const string error = "Object was not created with object pool";

        ObjectPoolItem opi = go.GetComponent<ObjectPoolItem>();
        if (opi == null)
        {
            Debug.LogError(error);
            return;
        }

        string prefabPath = opi.PrefabPath;
        if (!poolContainers.ContainsKey(prefabPath))
        {
            Debug.LogError(error);
            return;
        }

        Transform poolContainer = GetPoolContainer(prefabPath);
        go.transform.parent = poolContainer;
        go.BroadcastMessage("OPDestroy", SendMessageOptions.DontRequireReceiver);
        go.SetActive(false);
    }
    private Transform GetPoolContainer(string prefabPath)
    {
        Transform container;
        if (!poolContainers.ContainsKey(prefabPath))
        {
            container = new GameObject().transform;
            container.name = prefabPath;
            container.parent = this.transform;
            poolContainers.Add(prefabPath, container);
            //Debug.Log("");
        }
        //Debug.Log("Pool container " + prefabPath);
        //Debug.Log(poolContainers[prefabPath]);
        return poolContainers[prefabPath];
    }
}
