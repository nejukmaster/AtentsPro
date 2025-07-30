using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AtentsPro
{
    [System.Serializable]
    public struct UIState
    {
        public string stateName;
        public string[] linkedStates;
    }

    [CreateAssetMenu(menuName = "AtentsPro/UINavStatemachine")]
    public class UINavStatemachine : ScriptableObject
    {
        public UIState[] states;
    }
}
