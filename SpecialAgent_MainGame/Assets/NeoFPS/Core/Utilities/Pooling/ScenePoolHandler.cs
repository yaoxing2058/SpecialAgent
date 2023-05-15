﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NeoSaveGames.Serialization;
using NeoSaveGames;
using UnityEngine.Serialization;

namespace NeoFPS
{
    [HelpURL("https://docs.neofps.com/manual/utilitiesref-mb-scenepoolinfo.html")]
	public class ScenePoolHandler : MonoBehaviour, INeoSerializableComponent
    {
        [SerializeField, Tooltip("The collections of pooled objects to set up at initialisation.")]
        private PooledObjectCollection[] m_Collections = { };

        [SerializeField, FormerlySerializedAs("m_StartingPools"), Tooltip("The pools to set up at initialisation.")]
        private PoolInfo[] m_ScenePools = new PoolInfo[0];

        private bool m_Initialised = false;
        private NeoSerializedGameObject m_Nsgo = null;
        private Dictionary<PooledObject, Pool> m_PoolDictionary = new Dictionary<PooledObject, Pool>();
        private List<Pool> m_PoolList = new List<Pool>();

        class Pool
        {
            public PooledObject prototype = null;
            public Transform poolTransform = null;
            public Transform activeTransform = null;
            public NeoSerializedGameObject poolNsgo = null;
            public NeoSerializedGameObject activeNsgo = null;
            public NeoSerializedGameObject prototypeNsgo = null;
            public int targetSize = 0;

            private int m_Counter = 1;
            // Note: With save system refactor, should remove this

            public int total
            {
                get { return poolTransform.childCount + activeTransform.childCount; }
            }
            
            public void Grow()
            {
                int start = total;
                int toAdd = Math.Min(targetSize - start, PoolManager.poolIncrement);
                if (prototypeNsgo != null)
                {
                    for (int i = 0; i < toAdd; ++i)
                    {
                        var obj = poolNsgo.InstantiatePrefab<PooledObject>(prototypeNsgo.prefabStrongID, m_Counter++);
                        obj.gameObject.SetActive(false);
                        obj.poolTransform = poolTransform;
                    }
                }
                else
                {
                    for (int i = 0; i < toAdd; ++i)
                    {
                        PooledObject obj = Instantiate(prototype);
                        obj.gameObject.SetActive(false);
                        obj.transform.SetParent(poolTransform);
                        obj.poolTransform = poolTransform;
                    }
                }
            }

            public void Grow(int target)
            {
                if (target > targetSize)
                    targetSize = target;
                Grow();
            }

            public Pool(PooledObject proto, int total, int startCount, Transform pt, Transform at)
            {
                prototype = proto;
                poolTransform = pt;
                activeTransform = at;
                poolNsgo = null;
                activeNsgo = null;
                prototypeNsgo = null;                
                targetSize = total;
                for (int i = 0; i < startCount; ++i)
                {
                    PooledObject obj = Instantiate(prototype);
                    obj.gameObject.SetActive(false);
                    obj.transform.SetParent(poolTransform);
                    obj.poolTransform = poolTransform;
                }
            }

            public Pool(PooledObject proto, NeoSerializedGameObject protoNsgo, NeoSerializedGameObject pNsgo, NeoSerializedGameObject aNsgo)
            {
                prototype = proto;
                prototypeNsgo = protoNsgo;
                poolTransform = pNsgo.transform;
                activeTransform = aNsgo.transform;
                poolNsgo = pNsgo;
                activeNsgo = aNsgo;

                // Build hash map of active objects
                int highest = 0;
                HashSet<int> activeObjects = new HashSet<int>();
                for (int i = 0; i < activeTransform.childCount; ++i)
                {
                    int key = activeTransform.GetChild(i).GetComponent<NeoSerializedGameObject>().serializationKey;
                    if (key > highest)
                        highest = key;
                    activeObjects.Add(key);
                }

                // Fill out inactive to count, skipping active
                // Start() will fill out remaining capacity
                for (int i = 1; i < highest; ++i)
                {
                    if (!activeObjects.Contains(i))
                    {
                        var obj = poolNsgo.InstantiatePrefab<PooledObject>(prototypeNsgo.prefabStrongID, i);
                        obj.gameObject.SetActive(false);
                        obj.poolTransform = poolTransform;
                    }
                }

                m_Counter = highest + 1;
                targetSize = highest;
            }

            public Pool(PooledObject proto, int total, int startCount, NeoSerializedGameObject pNsgo, NeoSerializedGameObject aNsgo)
            {
                prototype = proto;
                poolTransform = pNsgo.transform;
                activeTransform = aNsgo.transform;
                targetSize = total;

                prototypeNsgo = proto.GetComponent<NeoSerializedGameObject>();
                if (prototypeNsgo != null)
                {
                    poolNsgo = pNsgo;
                    activeNsgo = aNsgo;
                    
                    for (int i = 0; i < startCount; ++i)
                    {
                        var obj = poolNsgo.InstantiatePrefab<PooledObject>(prototypeNsgo.prefabStrongID, m_Counter++);
                        if (obj != null)
                        {
                            obj.gameObject.SetActive(false);
                            obj.poolTransform = poolTransform;
                        }
                    }
                }
                else
                {
                    poolNsgo = null;
                    activeNsgo = null;

                    for (int i = 0; i < startCount; ++i)
                    {
                        PooledObject obj = Instantiate(prototype);
                        obj.gameObject.SetActive(false);
                        obj.transform.SetParent(poolTransform);
                        obj.poolTransform = poolTransform;
                    }
                }
            }

            public void DestroyPool()
            {
                Destroy(poolTransform.gameObject);
                poolTransform = null;
                Destroy(activeTransform.gameObject);
                activeTransform = null;
                poolNsgo = null;
                activeNsgo = null;
                prototype = null;
                prototypeNsgo = null;
            }

            public T GetObject<T>(bool activate)
            {
                if (poolTransform == null || activeTransform == null)
                    return default(T);

                if (poolTransform.childCount > 0)
                {
                    Transform t = poolTransform.GetChild(poolTransform.childCount - 1);
                    T result = t.GetComponent<T>();

                    if (result != null)
                    {
                        if (prototypeNsgo != null)
                        {
                            var nsgo = t.GetComponent<NeoSerializedGameObject>();
                            nsgo.SetParent(activeNsgo);
                        }
                        else
                        {
                            t.SetParent(activeTransform);
                        }
                    }
                    return result;
                }
                else
                {
                    if (total < targetSize)
                    {
                        Grow();
                        return GetObject<T>(activate);
                    }
                    else
                    {
                        if (activeTransform.childCount > 0)
                        {
                            Transform t = activeTransform.GetChild(0);
                            T result = t.GetComponent<T>();

                            if (result != null)
                            {
                                t.gameObject.SetActive(false);
                                t.SetAsLastSibling();
                            }

                            return result;
                        }
                        else
                        {
                            Debug.LogError("Pooling system attempting to recycle an active pooled object, but none found. This shouldn't be possible");
                            return default(T);
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            for (int i = 0; i < m_ScenePools.Length; ++i)
            {
                if (m_ScenePools[i].count < 1)
                    m_ScenePools[i].count = 1;
            }
        }
#endif

        void Awake()
        {
            PoolManager.SetCurrentScenePoolInfo(this);
        }

        IEnumerator Start ()
        {
            yield return null;
            Initialise();
        }

        void Update()
        {
            for (int i = 0; i < m_PoolList.Count; ++i)
            {
                var total = m_PoolList[i].total;
                if (total < m_PoolList[i].targetSize)
                {
                    m_PoolList[i].Grow();
                    break;
                }
            }
        }
        
        public void Initialise()
        {
            if (!m_Initialised)
            {
                // Get the NeoSerializedGameObject if appropriate
                m_Nsgo = GetComponent<NeoSerializedGameObject>();

                // Create the starting pools
                CreatePools(m_ScenePools);
                for (int i = 0; i < m_Collections.Length; ++i)
                {
                    if (m_Collections[i] != null)
                        CreatePools(m_Collections[i].pooledObjects);
                }

                m_Initialised = true;
            }
        }

        public void CreatePools(PoolInfo[] pools)
        {
            for (int i = 0; i < pools.Length; ++i)
            {
                // Get the prototype
                PooledObject prototype = pools[i].prototype;
                if (prototype != null)
                    CreatePool(prototype, pools[i].count);
            }
        }

        public void CreatePool(PooledObject prototype, int count)
        {
            CreatePool(prototype, count, 0);
        }

        public void CreatePool (PooledObject prototype, int total, int startCount)
		{
            // Check invalid pool size
            if (total < 1)
                total = 1;
            if (startCount < 0)
                startCount = 0;

            if (m_PoolDictionary.ContainsKey (prototype))
			{
                m_PoolDictionary[prototype].Grow (total);
			}
			else
            {
                var prototypeNsgo = prototype.GetComponent<NeoSerializedGameObject>();
                if (m_Nsgo == null || prototypeNsgo == null || !NeoSerializedObjectFactory.IsPrefabRegistered(prototypeNsgo.prefabStrongID))
                {
                    // Create heirachy
                    Transform poolRoot = new GameObject(prototype.name).transform;
                    poolRoot.SetParent(transform);
                    Transform poolTransform = new GameObject("Pool").transform;
                    poolTransform.SetParent(poolRoot);
                    Transform activeTransform = new GameObject("Active").transform;
                    activeTransform.SetParent(poolRoot);

                    // Create and add the pool
                    var pool = new Pool(prototype, total, startCount, poolTransform, activeTransform);
                    m_PoolDictionary.Add(prototype, pool);
                    m_PoolList.Add(pool);
                }
                else
                {
                    // Create heirachy
                    var nsgo = m_Nsgo;
                    NeoSerializedGameObject poolRoot = nsgo.serializedChildren.CreateChildObject(prototype.name, prototypeNsgo.prefabStrongID);
                    NeoSerializedGameObject activeNsgo = poolRoot.serializedChildren.CreateChildObject("Active", 1);
                    NeoSerializedGameObject poolNsgo = poolRoot.serializedChildren.CreateChildObject("Pool", -1);
                    poolRoot.saveName = true;
                    activeNsgo.saveName = true;
                    poolNsgo.saveName = true;

                    // Set pool object not to serialize children
                    poolNsgo.filterChildObjects = NeoSerializationFilter.Include;

                    // Create and add the pool
                    var pool = new Pool(prototype, total, startCount, poolNsgo, activeNsgo);
                    m_PoolDictionary.Add(prototype, pool);
                    m_PoolList.Add(pool);
                }
			}
		}

		public void ReturnObjectToPool (PooledObject obj)
		{
            Pool pool;
			if (m_PoolDictionary.TryGetValue (obj, out pool))
			{
                var nsgo = obj.GetComponent<NeoSerializedGameObject>();
                if (nsgo != null)
                {
                    nsgo.gameObject.SetActive(false);
                    nsgo.SetParent(pool.poolNsgo);
                }
                else
                {
                    obj.gameObject.SetActive(false);
                    obj.transform.SetParent(pool.poolTransform);
                }
			}
			else
				Destroy (obj.gameObject);
		}

		public T GetPooledObject<T> (PooledObject prototype, bool activate = true)
        {
            Pool pool;
			if (m_PoolDictionary.TryGetValue (prototype, out pool))
			{
				T result = pool.GetObject<T> (activate);
                var comp = result as Component;
				if (comp != null)
				{
					Transform t = comp.transform;
					t.position = Vector3.zero;
					t.rotation = Quaternion.identity;
                    if (activate)
                        comp.gameObject.SetActive(true);
                }
                return result;
			}
			else
			{
                CreatePool (prototype, PoolManager.defaultPoolSize);
				return GetPooledObject<T> (prototype);
			}
		}

		public T GetPooledObject<T> (PooledObject prototype, Vector3 position, Quaternion rotation, bool activate = true)
        {
            Pool pool;
			if (m_PoolDictionary.TryGetValue (prototype, out pool))
			{
				T result = pool.GetObject<T> (activate);
                var comp = result as Component;
                if (comp != null)
                {
					Transform t = comp.transform;
					t.position = position;
					t.rotation = rotation;
                    if (activate)
                        comp.gameObject.SetActive(true);
                }
				return result;
			}
			else
			{
                CreatePool (prototype, PoolManager.defaultPoolSize);
				return GetPooledObject<T> (prototype, position, rotation);
			}
		}

        public T GetPooledObject<T>(PooledObject prototype, Vector3 position, Quaternion rotation, Vector3 scale, bool activate = true)
        {
            Pool pool;
            if (m_PoolDictionary.TryGetValue(prototype, out pool))
            {
                T result = pool.GetObject<T>(activate);
                var comp = result as Component;
                if (comp != null)
                {
                    Transform t = comp.transform;
                    t.position = position;
                    t.rotation = rotation;
                    t.localScale = scale;
                    if (activate)
                        comp.gameObject.SetActive(true);
                }
                return result;
            }
            else
            {
                CreatePool(prototype, PoolManager.defaultPoolSize);
                return GetPooledObject<T>(prototype, position, rotation);
            }
        }

        public void WriteProperties(INeoSerializer writer, NeoSerializedGameObject nsgo, SaveMode saveMode)
        {
            // Nothing needs writing - it's all handled by serializing child objects
        }

        public void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
        {
            m_Nsgo = nsgo;

            var childObjects = GetComponentsInChildren<NeoSerializedGameObject>();
            for (int i = 0; i < childObjects.Length; ++i)
            {
                var prefab = NeoSerializedObjectFactory.GetPrefab(childObjects[i].serializationKey);
                if (prefab != null)
                {
                    var pooledObject = prefab.GetComponent<PooledObject>();
                    if (pooledObject != null)
                    {
                        var activeNsgo = childObjects[i].serializedChildren.GetChildObject(1);
                        var inactiveNsgo = childObjects[i].serializedChildren.GetChildObject(-1);
                        if (activeNsgo != null && inactiveNsgo != null)
                        {
                            inactiveNsgo.filterChildObjects = NeoSerializationFilter.Include;
                            var pool = new Pool(pooledObject, prefab, inactiveNsgo, activeNsgo);
                            m_PoolDictionary.Add(pooledObject, pool);
                        }
                    }
                }
            }
        }
    }
}