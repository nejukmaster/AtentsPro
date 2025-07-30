using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    [System.Serializable]
    public class CameraDictionarySerializable
    {
        public string key;
        public CameraController cameraController;
    }
    public class CameraSystem : MonoBehaviour
    {
        public static CameraSystem instance;
        public static CameraSystem GetInstance() { return instance; }

        [SerializeField] List<CameraDictionarySerializable> _cameraDictionary = new List<CameraDictionarySerializable>();
        Dictionary<string, CameraController> cameraDictionary = new Dictionary<string, CameraController> ();

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            for(int i = 0; i <  _cameraDictionary.Count; i++)
            {
                cameraDictionary.Add(_cameraDictionary[i].key, _cameraDictionary[i].cameraController);
            }
        }

        public CameraController GetCamera(string key)
        {
            if(cameraDictionary.ContainsKey(key))
                return cameraDictionary[key];
            else return null;
        }
    }
}
