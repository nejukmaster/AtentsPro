using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    [System.Serializable]
    public struct UINavigateParam
    {
        public int pInt;
        public string pString;
        public float pFloat;
        public double pDouble;
        public bool pBool;
    }

    public abstract class UINavigatable : MonoBehaviour
    {
        public abstract GameObject GetGameObject();
        public abstract void StateOn(UINavigateParam param);
        public abstract void StateOff();

        public virtual void Init()
        {

        }
    }
}
