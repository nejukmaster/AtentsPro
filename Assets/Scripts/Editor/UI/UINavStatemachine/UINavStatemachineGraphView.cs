#if UNITY_EDITOR
using AtentsPro;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AtentsPro
{
    public class UINavStatemachineGraphView : GraphView
    {
        public System.Action<GraphViewChange> onGraphViewChanged;

        public UINavStatemachineGraphView()
        {
            // ��/��/���� ����
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // �׸��� ���
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            graphViewChanged += changes =>
            {
                onGraphViewChanged?.Invoke(changes);
                return changes;
            };
        }

        /// <summary>
        /// �׺���̼� �����͸� �޾Ƽ� ���� ������ ��� �׸��ϴ�.
        /// </summary>
        public void Populate(UINavStatemachine navMachine)
        {
            // ���� ������Ʈ Ŭ����
            DeleteElements(graphElements);

            var nodes = new Dictionary<string, UINavStatemachineNode>();

            // 1) ��� ����
            for (int i = 0; i < navMachine.states.Length; i++)
            {
                var state = navMachine.states[i];
                var node = new UINavStatemachineNode(state.stateName, i);

                // ������ ��ġ (���⼱ ���� ��ġ ����)
                float angle = i * (360f / navMachine.states.Length);
                node.SetPosition(new Rect(
                    200 + 200 * Mathf.Cos(angle * Mathf.Deg2Rad),
                    200 + 200 * Mathf.Sin(angle * Mathf.Deg2Rad),
                    150, 100));

                AddElement(node);
                nodes[state.stateName] = node;
            }

            // 2) ���� ��ũ(����) ����
            for (int i = 0; i < navMachine.states.Length; i++)
            {
                var fromState = navMachine.states[i];
                var fromNode = nodes[fromState.stateName];

                foreach (var toName in fromState.linkedStates)
                {
                    if (nodes.TryGetValue(toName, out var toNode))
                    {
                        var edge = new Edge
                        {
                            output = fromNode.outputPort,
                            input = toNode.inputPort,
                        };
                        fromNode.outputPort.Connect(edge);
                        toNode.inputPort.Connect(edge);
                        Add(edge);
                    }
                }
            }
        }
    }
}
#endif