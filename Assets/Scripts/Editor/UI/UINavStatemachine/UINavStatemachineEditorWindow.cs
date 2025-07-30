#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using AtentsPro;
using System.Linq;

namespace AtentsPro
{
    public class UINavigationEditorWindow : EditorWindow
    {
        private UINavStatemachineGraphView graphView;
        private UINavStatemachine navSystem;
        private SerializedObject so;
        private SerializedProperty statesProp;

        [MenuItem("Window/UI Navigation Editor")]
        public static void OpenWindow()
        {
            GetWindow<UINavigationEditorWindow>("UI Nav Editor");
        }

        // UIElements �ʱ�ȭ
        public void CreateGUI()
        {
            // 1) Select Asset ��ư
            var selectBtn = new Button(SelectAsset) { text = "Select UINavigationSystem Asset" };
            rootVisualElement.Add(selectBtn);

            // 2) GraphView
            graphView = new UINavStatemachineGraphView();
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            // 3) GraphView ���� �ݹ� ����
            graphView.graphViewChanged += OnGraphChanged;
        }

        private void SelectAsset()
        {
            string path = EditorUtility.OpenFilePanel("Select Asset", "Assets", "asset");
            if (string.IsNullOrEmpty(path)) return;
            path = path.Substring(path.IndexOf("Assets"));
            navSystem = AssetDatabase.LoadAssetAtPath<UINavStatemachine>(path);
            if (navSystem == null) return;

            so = new SerializedObject(navSystem);
            statesProp = so.FindProperty("states");
            graphView.Populate(navSystem);
        }

        private GraphViewChange OnGraphChanged(GraphViewChange changes)
        {
            if (navSystem == null) return changes;

            if (changes.edgesToCreate != null)
                foreach (var edge in changes.edgesToCreate)
                    HandleEdgeAdded(edge);

            if (changes.elementsToRemove != null)
                foreach (var edge in changes.elementsToRemove.OfType<Edge>())
                    HandleEdgeRemoved(edge);

            so.ApplyModifiedProperties();
            return changes;
        }

        private void HandleEdgeAdded(Edge edge)
        {
            // from �� to �� ����� edge�� �߰��� �� ����� ����
            var fromNode = edge.output.node as UINavStatemachineNode;
            var toNode = edge.input.node as UINavStatemachineNode;

            // SerializedProperty �� linkedStates �迭 ��������
            var arrProp = statesProp
                .GetArrayElementAtIndex(fromNode.stateIndex)
                .FindPropertyRelative("linkedStates");

            // �迭�� toNode.title(=stateName) �߰�
            arrProp.arraySize++;
            arrProp
                .GetArrayElementAtIndex(arrProp.arraySize - 1)
                .stringValue = toNode.title;
        }

        private void HandleEdgeRemoved(Edge edge)
        {
            // ��) serializedObject, statesProp ������ linkedStates���� ����
            var fromNode = edge.output.node as UINavStatemachineNode;
            var toNode = edge.input.node as UINavStatemachineNode;
            var arrProp = statesProp
                .GetArrayElementAtIndex(fromNode.stateIndex)
                .FindPropertyRelative("linkedStates");

            for (int i = 0; i < arrProp.arraySize; i++)
            {
                if (arrProp.GetArrayElementAtIndex(i).stringValue == toNode.title)
                {
                    arrProp.DeleteArrayElementAtIndex(i);
                    break;
                }
            }
        }

        // ������ ���� �� ����
        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        // HandleEdgeAdded/Removed �� (����)
    }
}
#endif