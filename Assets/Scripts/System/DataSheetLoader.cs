using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AtentsPro
{
    [System.Serializable]
    public struct ItemInfo
    {
        public string name;
        public string description;
        public bool canUse;
    }

    [System.Serializable]
    public struct SkillInfo
    {
        public string name;
        public string description;
        public float cost;
        public float cooltime;
    }

    public struct StageInfo
    {
        public string name;
        public string description;
        public string[] enemies;
    }
    public class DataSheetLoader : MonoBehaviour
    {
        public static DataSheetLoader Instance;
        public static DataSheetLoader GetInstance() { return Instance; }

        public Dictionary<string,ItemInfo> itemTable = new Dictionary<string, ItemInfo>();
        public Dictionary<string,SkillInfo> skillTable = new Dictionary<string, SkillInfo>();
        public Dictionary<string, StageInfo> stageTable = new Dictionary<string, StageInfo>();
        // Start is called before the first frame update
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                Init();
            }
            else
            {
                Destroy(Instance);
            }
        }

        void Init()
        {
            LoadItemTable();
            LoadSkillTable();
            LoadStageTable();
        }

        void LoadItemTable()
        {
            List<Dictionary<string, object>> data = CSVReader.Read(AtentsProGlobalSettings.Instance.dataSheetPath + "ItemTable");
            for(int i = 0; i < data.Count; i++)
            {
                ItemInfo entry = new ItemInfo
                {
                    name = data[i]["item_name"].ToString(),
                    description = data[i]["item_description"].ToString(),
                    canUse = (data[i]["can_use"].ToString() == "TRUE")
                };
                itemTable.Add(data[i]["item_id"].ToString(), entry);
            }
        }

        void LoadSkillTable()
        {
            List<Dictionary<string, object>> data = CSVReader.Read(AtentsProGlobalSettings.Instance.dataSheetPath + "SkillTable");
            for (int i = 0; i < data.Count; i++)
            {
                SkillInfo entry = new SkillInfo
                {
                    name = data[i]["skill_name"].ToString(),
                    description = data[i]["skill_description"].ToString(),
                    cost = float.Parse(data[i]["skill_cost"].ToString()),
                    cooltime = float.Parse(data[i]["skill_cooltime"].ToString())
                };
                skillTable.Add(data[i]["skill_id"].ToString(), entry);
            }
        }

        void LoadStageTable()
        {
            List<Dictionary<string, object>> data = CSVReader.Read(AtentsProGlobalSettings.Instance.dataSheetPath + "StageTable");
            for (int i = 0; i < data.Count; i++)
            {
                StageInfo entry = new StageInfo
                {
                    name = data[i]["stage_name"].ToString(),
                    description = data[i]["stage_description"].ToString(),
                    enemies = data[i]["stage_enemies"].ToString().Split(",")
                };
                stageTable.Add(data[i]["stage_id"].ToString(), entry);
            }
        }
    }

    //CSV 파싱용 클래스
    public class CSVReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static List<Dictionary<string, object>> Read(string file)
        {
            var list = new List<Dictionary<string, object>>();
            TextAsset data = Resources.Load(file) as TextAsset;

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);
            for (var i = 1; i < lines.Length; i++)
            {

                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    object finalvalue = value;
                    int n;
                    float f;
                    if (int.TryParse(value, out n))
                    {
                        finalvalue = n;
                    }
                    else if (float.TryParse(value, out f))
                    {
                        finalvalue = f;
                    }
                    entry[header[j]] = finalvalue;
                }
                list.Add(entry);
            }
            return list;
        }
    }
}
