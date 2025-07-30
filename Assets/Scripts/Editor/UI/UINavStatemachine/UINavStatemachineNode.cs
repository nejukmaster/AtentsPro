using System.Collections;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AtentsPro
{
    public class UINavStatemachineNode : Node
    {
        public int stateIndex;            // states �迭 �ε���
        public Port inputPort;            // ������� ��Ʈ
        public Port outputPort;           // ���Ẹ�� ��Ʈ

        public UINavStatemachineNode(string title, int index)
        {
            this.stateIndex = index;
            this.title = title;

            // �Է�/��� ��Ʈ ���� (���� ���� ���)
            inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));

            inputContainer.Add(inputPort);
            outputContainer.Add(outputPort);

            // �⺻ ��Ÿ�� ����
            RefreshExpandedState();
            RefreshPorts();
        }
    }
}
#endif