using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class ObjectPool
    {
        Queue<GameObject> pool = new Queue<GameObject>();

        public void Init(GameObject go, int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject obj = GameObject.Instantiate(go);
                Enqueue(obj, null);
            }
        }

        public void Init(GameObject go, int count, Transform parent)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject obj = GameObject.Instantiate(go, parent);
                Enqueue(obj, null);
            }
        }


        public GameObject Dequeue(Action<GameObject>? preactivationAction)
        {
            GameObject obj = pool.Dequeue();
            preactivationAction?.Invoke(obj);
            obj.SetActive(true);
            return obj;
        }
        public void Enqueue(GameObject go, Action<GameObject>? predeactivationAction)
        {
            predeactivationAction?.Invoke(go);
            go.SetActive(false);
            pool.Enqueue(go);
        }

        public void Clear()
        {
            pool.Clear();
        }
    }
}
