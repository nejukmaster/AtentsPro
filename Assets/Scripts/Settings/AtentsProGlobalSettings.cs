using UnityEditor;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace AtentsPro
{
#if UNITY_EDITOR
    static class AtentsProGlobalSettingsProvider
    {

        [SettingsProvider]
        public static SettingsProvider CreateAtentsProSettingsProvider()
        {
            var provider = new SettingsProvider("Project/AtentsPro", SettingsScope.Project)
            {
                label = "Atents Project Global Settings",
                guiHandler = (searchContext) =>
                {
                    // 1. ScriptableObject 인스턴스에서 SerializedObject를 가져옵니다.
                    //    이렇게 하면 Undo/Redo가 가능하고, 값이 변경되었는지 쉽게 감지할 수 있습니다.
                    var settings = AtentsProGlobalSettings.GetSerializedObject();

                    // 2. UI 필드를 그립니다. settings.FindProperty()를 사용합니다.
                    EditorGUILayout.LabelField("Resources Path", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.characterAssetPath)), new GUIContent("Character Assets"));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.modelFBXsPath)), new GUIContent("Model FBXs Path"));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.prefabsPath)), new GUIContent("Prefabs Path"));

                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField("Texture Path", EditorStyles.boldLabel); 
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.uiTexturePath)), new GUIContent("UI Texture"));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.itemIconPath)), new GUIContent("Item Icon Texture"));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.skillIconPath)), new GUIContent("Skill Icon Texture"));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.profileImagePath)), new GUIContent("Profile Image Texture"));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.characterPortraitPath)), new GUIContent("Character Portrait Texture"));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.characterFullbodyPath)), new GUIContent("Character Fullbody Texture"));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.buffIconPath)), new GUIContent("Buff Icon Texture"));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.eventBannerPath)), new GUIContent("Event Banner"));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.dataSheetPath)), new GUIContent("Data Sheet"));

                    EditorGUILayout.Space(10);

                    EditorGUILayout.LabelField("Scene Path", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(AtentsProGlobalSettings.scenesPath)), new GUIContent("Scenes Path"));

                    // 3. UI에서 변경된 사항이 있다면 적용(저장)합니다.
                    settings.ApplyModifiedProperties();
                },

                keywords = new System.Collections.Generic.HashSet<string>(new[] { "Atents", "Settings" })
            };

            return provider;
        }
    }
#endif
    // ScriptableObject는 데이터를 담는 컨테이너 역할을 합니다.
    public class AtentsProGlobalSettings : ScriptableObject
    {
        // 1. ScriptableObject 에셋이 저장될 경로입니다.
        // ProjectSettings 폴더는 프로젝트 전체 설정에 적합합니다.
        private const string k_settingsPath = "Assets/Resources/Settings/AtentsProGlobalSettings.asset";

        // 2. 클래스의 모든 인스턴스 필드는 여기에 선언합니다. (static 제거)
        public string characterAssetPath;
        public string modelFBXsPath;
        public string prefabsPath;
        public string uiTexturePath;
        public string itemIconPath;
        public string skillIconPath;
        public string profileImagePath;
        public string characterPortraitPath;
        public string characterFullbodyPath;
        public string buffIconPath;
        public string eventBannerPath;
        public string dataSheetPath;
        public string scenesPath;

        // 3. 내부적으로 사용할 싱글톤 인스턴스
        private static AtentsProGlobalSettings s_instance;

        // 4. 외부에서 이 설정에 접근하기 위한 public 프로퍼티
        public static AtentsProGlobalSettings Instance
        {
            get
            {
                if (s_instance == null)
                {
                    // 기존에 에셋 파일이 있는지 찾아보고, 있다면 로드합니다.
                    s_instance = AssetDatabase.LoadAssetAtPath<AtentsProGlobalSettings>(k_settingsPath);

                    // 만약 에셋 파일이 없다면 새로 생성합니다.
                    if (s_instance == null)
                    {
                        s_instance = CreateInstance<AtentsProGlobalSettings>();

                        // 지정된 경로에 에셋 파일로 저장합니다.
                        AssetDatabase.CreateAsset(s_instance, k_settingsPath);
                        AssetDatabase.SaveAssets();
                    }
                }
                return s_instance;
            }
        }

        // 이 클래스 내에서 SerializedObject를 가져오는 헬퍼 함수
        internal static SerializedObject GetSerializedObject()
        {
            return new SerializedObject(Instance);
        }
    }
}
