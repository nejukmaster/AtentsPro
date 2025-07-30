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

        // UIElements 초기화
        public void CreateGUI()
        {
            // 1) Select Asset 버튼
            var selectBtn = new Button(SelectAsset) { text = "Select UINavigationSystem Asset" };
            rootVisualElement.Add(selectBtn);

            // 2) GraphView
            graphView = new UINavStatemachineGraphView();
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            // 3) GraphView 변경 콜백 연결
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
            // from → to 로 연결된 edge가 추가될 때 실행될 로직
            var fromNode = edge.output.node as UINavStatemachineNode;
            var toNode = edge.input.node as UINavStatemachineNode;

            // SerializedProperty 로 linkedStates 배열 가져오기
            var arrProp = statesProp
                .GetArrayElementAtIndex(fromNode.stateIndex)
                .FindPropertyRelative("linkedStates");

            // 배열에 toNode.title(=stateName) 추가
            arrProp.arraySize++;
            arrProp
                .GetArrayElementAtIndex(arrProp.arraySize - 1)
                .stringValue = toNode.title;
        }

        private void HandleEdgeRemoved(Edge edge)
        {
            // 예) serializedObject, statesProp 등으로 linkedStates에서 제거
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

        // 에디터 종료 시 정리
        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        // HandleEdgeAdded/Removed … (생략)
    }
}
#endif