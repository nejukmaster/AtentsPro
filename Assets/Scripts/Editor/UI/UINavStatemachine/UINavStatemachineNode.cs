using System.Collections;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AtentsPro
{
    public class UINavStatemachineNode : Node
    {
        public int stateIndex;            // states 배열 인덱스
        public Port inputPort;            // 연결받을 포트
        public Port outputPort;           // 연결보낼 포트

        public UINavStatemachineNode(string title, int index)
        {
            this.stateIndex = index;
            this.title = title;

            // 입력/출력 포트 생성 (다중 연결 허용)
            inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));

            inputContainer.Add(inputPort);
            outputContainer.Add(outputPort);

            // 기본 스타일 정리
            RefreshExpandedState();
            RefreshPorts();
        }
    }
}
#endif