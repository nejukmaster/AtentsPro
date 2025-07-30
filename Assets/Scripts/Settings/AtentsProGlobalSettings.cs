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
                    // 1. ScriptableObject �ν��Ͻ����� SerializedObject�� �����ɴϴ�.
                    //    �̷��� �ϸ� Undo/Redo�� �����ϰ�, ���� ����Ǿ����� ���� ������ �� �ֽ��ϴ�.
                    var settings = AtentsProGlobalSettings.GetSerializedObject();

                    // 2. UI �ʵ带 �׸��ϴ�. settings.FindProperty()�� ����մϴ�.
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

                    // 3. UI���� ����� ������ �ִٸ� ����(����)�մϴ�.
                    settings.ApplyModifiedProperties();
                },

                keywords = new System.Collections.Generic.HashSet<string>(new[] { "Atents", "Settings" })
            };

            return provider;
        }
    }
#endif
    // ScriptableObject�� �����͸� ��� �����̳� ������ �մϴ�.
    public class AtentsProGlobalSettings : ScriptableObject
    {
        // 1. ScriptableObject ������ ����� ����Դϴ�.
        // ProjectSettings ������ ������Ʈ ��ü ������ �����մϴ�.
        private const string k_settingsPath = "Assets/Resources/Settings/AtentsProGlobalSettings.asset";

        // 2. Ŭ������ ��� �ν��Ͻ� �ʵ�� ���⿡ �����մϴ�. (static ����)
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

        // 3. ���������� ����� �̱��� �ν��Ͻ�
        private static AtentsProGlobalSettings s_instance;

        // 4. �ܺο��� �� ������ �����ϱ� ���� public ������Ƽ
        public static AtentsProGlobalSettings Instance
        {
            get
            {
                if (s_instance == null)
                {
                    // ������ ���� ������ �ִ��� ã�ƺ���, �ִٸ� �ε��մϴ�.
                    s_instance = AssetDatabase.LoadAssetAtPath<AtentsProGlobalSettings>(k_settingsPath);

                    // ���� ���� ������ ���ٸ� ���� �����մϴ�.
                    if (s_instance == null)
                    {
                        s_instance = CreateInstance<AtentsProGlobalSettings>();

                        // ������ ��ο� ���� ���Ϸ� �����մϴ�.
                        AssetDatabase.CreateAsset(s_instance, k_settingsPath);
                        AssetDatabase.SaveAssets();
                    }
                }
                return s_instance;
            }
        }

        // �� Ŭ���� ������ SerializedObject�� �������� ���� �Լ�
        internal static SerializedObject GetSerializedObject()
        {
            return new SerializedObject(Instance);
        }
    }
}
