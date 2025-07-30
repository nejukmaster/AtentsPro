using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AtentsPro
{ 
    public class Marker : Selectable, IPointerClickHandler
    {
        public string key;

        public Action<GameObject> clickAction;

        private GameObject markedObj;
        private Func<GameObject, Vector3> trackingFunc;

        public virtual void Update()
        {
            if(trackingFunc != null)
            {
                Vector2 newPos = trackingFunc(markedObj);
                GetComponent<RectTransform>().position = newPos;
            }
            if( markedObj == null || !markedObj.activeSelf)
            {
                MarkerSystem.GetInstance().EnqueueMarker(this);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            clickAction?.Invoke(markedObj);
        }

        public void SetMarkedObj(GameObject obj)
        {
            this.markedObj = obj;
        }

        public void SetTracking(Func<GameObject, Vector3>? trackingFunc)
        {
            this.trackingFunc = trackingFunc;
        }

        public void Init()
        {
            markedObj = null;
            trackingFunc = null;
        }
    }
}
