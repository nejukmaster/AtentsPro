using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;


namespace AtentsPro
{

    public class EventSystem : MonoBehaviour
    {
        public static EventSystem instance;
        public static EventSystem GetInstance() { return instance; }

        public Action<Character> characterClickedEvent;
        
        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            // 왼쪽 마우스 버튼 클릭 감지
            if (Input.GetMouseButtonDown(0))
            {
                // 화면상의 마우스 좌표 → 레이 생성
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // 레이 쏴서 Collider에 맞으면
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    GameObject clickedObj = hit.collider.gameObject;
                    if (clickedObj.GetComponent<Character>() != null)
                    {
                        characterClickedEvent?.Invoke(clickedObj.GetComponent<Character>());
                    }
                }
            }
        }
    }
}
