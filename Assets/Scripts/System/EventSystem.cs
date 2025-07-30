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
            // ���� ���콺 ��ư Ŭ�� ����
            if (Input.GetMouseButtonDown(0))
            {
                // ȭ����� ���콺 ��ǥ �� ���� ����
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // ���� ���� Collider�� ������
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
