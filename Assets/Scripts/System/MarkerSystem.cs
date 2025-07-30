using AtentsPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class MarkerSystem : MonoBehaviour
    {
        public static MarkerSystem instance;
        public static MarkerSystem GetInstance() { return instance; }

        [SerializeField] Transform markerContainer;
        Dictionary<string, ObjectPool> markerPoolDic = new Dictionary<string, ObjectPool>();

        public void Awake()
        {
            instance = this;
        }

        public void InitMarker(string key, GameObject markerPrefab, int poolingCount)
        {
            if (markerPrefab.GetComponent<Marker>() == null) return;

            if (markerPoolDic.ContainsKey(key))
                markerPoolDic[key].Clear();
            else
                markerPoolDic.Add(key, new ObjectPool());
            markerPoolDic[key].Init(markerPrefab, poolingCount, markerContainer);
        }

        public Marker DequeueMarker(string key, Vector2 rectPos)
        {
            GameObject obj = markerPoolDic[key].Dequeue(null);
            obj.GetComponent<RectTransform>().position = rectPos;
            return obj.GetComponent<Marker>();
        }

        public void EnqueueMarker(Marker marker)
        {
            marker.Init();
            markerPoolDic[marker.key]?.Enqueue(marker.gameObject,null);
        }
    }
}
