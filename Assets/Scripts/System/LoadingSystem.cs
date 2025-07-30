using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class LoadingSystem : MonoBehaviour
    {
        public static LoadingSystem instance;
        public static LoadingSystem GetInstance() { return instance; }

        private void Awake()
        {
            instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            GameManager.GetInstance().sceneLoadParam.actionToLoad?.Invoke();
        }
    }
}
