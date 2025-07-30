using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtentsPro
{
    [Serializable]
    public class StateNavigatablePair
    {
        public string key;
        public UINavigatable value;
    }

    public class UINavigationSystem : MonoBehaviour
    {
        public static UINavigationSystem instance;
        public static UINavigationSystem GetInstance() { return instance; }

        public string currentState;
        public GameObject loadingUI;

        [SerializeField] UINavStatemachine statemachineAsset;
        [SerializeField] StateNavigatablePair[] _uiNavigatables;

        Dictionary<string, string[]> uiStates = new Dictionary<string, string[]>();
        Dictionary<string, List<UINavigatable>> navigatables = new Dictionary<string, List<UINavigatable>> ();

        void Awake()
        {
            instance = this;
            foreach(UIState state in statemachineAsset.states)
            {
                uiStates.Add(state.stateName, state.linkedStates);
            }
            foreach(StateNavigatablePair pair in _uiNavigatables)
            {
                if (!navigatables.ContainsKey(pair.key))
                    navigatables.Add(pair.key, new List<UINavigatable>());
                navigatables[pair.key].Add(pair.value);
            }
        }

        private void Start()
        {
            foreach(var list in navigatables.Values)
            {
                foreach(var ui in list)
                {
                    ui.Init();
                }
            }
            if (navigatables.ContainsKey(currentState))
            {
                foreach (UINavigatable ui in navigatables[currentState])
                {
                    ui.StateOn(new UINavigateParam());
                }
            }
        }

        public bool ChangeState(string targetStateName, UINavigateParam param)
        {
            if (uiStates[currentState].Contains<string>(targetStateName))
            {
                if (navigatables.ContainsKey(currentState))
                {
                    foreach (UINavigatable ui in navigatables[currentState])
                    {
                        ui.StateOff();
                    }
                }
                if (navigatables.ContainsKey(targetStateName))
                {
                    foreach (UINavigatable ui in navigatables[targetStateName])
                    {
                        ui.StateOn(param);
                    }
                }
                currentState = targetStateName;
                return true;
            }
            else
                return false;
        }
    }
}
