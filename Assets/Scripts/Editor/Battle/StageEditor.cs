#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace AtentsPro
{
    [CustomEditor(typeof(Stage), editorForChildClasses: true)]
    public class StageEditor : Editor
    {
        private void OnSceneGUI()
        {
            Texture spawnTexture = Resources.Load("Development/EditorGUI/StageSpawnPoint") as Texture;
            Texture substageTexture = Resources.Load("Development/EditorGUI/Stage") as Texture;

            Handles.color = Color.magenta;
            Stage stage = (Stage)target;

            EditorGUI.BeginChangeCheck();

            Vector3[] newPos = new Vector3[stage.spawnPos.Length];
            Vector3 pos_sum = Vector3.zero;
            int pos_count = 0;
            for (int i = 0; i < newPos.Length; i++)
            {
                newPos[i] = Handles.PositionHandle(stage.spawnPos[i], Quaternion.identity);
                pos_sum += newPos[i];
                pos_count++;
            }
            Handles.Label(pos_sum / (float)pos_count, spawnTexture);

            List<Vector3[]> newSubstageTeamPos = new List<Vector3[]>();
            List<Quaternion[]> newSubstageTeamQua = new List<Quaternion[]>();
            List<Vector3[]> newSubstageEnemyPos = new List<Vector3[]>();
            List<Quaternion[]> newSubstageEnemyQua = new List<Quaternion[]>();
            for (int i = 0; i < stage.substages.Length; i++)
            {
                pos_sum = Vector3.zero;
                pos_count = 0;

                newSubstageTeamPos.Add(new Vector3[stage.substages[i].teamPos.Length]);
                newSubstageTeamQua.Add(new Quaternion[stage.substages[i].teamPos.Length]);
                for (int j = 0; j < newSubstageTeamPos[i].Length; j++)
                {
                    newSubstageTeamPos[i][j] = Handles.PositionHandle(stage.substages[i].teamPos[j].position, Quaternion.Normalize(stage.substages[i].teamPos[j].rotation));
                    newSubstageTeamQua[i][j] = Quaternion.Normalize(Handles.RotationHandle(stage.substages[i].teamPos[j].rotation, stage.substages[i].teamPos[j].position));
                    pos_sum += newSubstageTeamPos[i][j];
                    pos_count++;
                }

                newSubstageEnemyPos.Add(new Vector3[stage.substages[i].enemyPos.Length]);
                newSubstageEnemyQua.Add(new Quaternion[stage.substages[i].enemyPos.Length]);
                for (int j = 0; j < newSubstageEnemyPos[i].Length; j++)
                {
                    newSubstageEnemyPos[i][j] = Handles.PositionHandle(stage.substages[i].enemyPos[j].position, Quaternion.Normalize(stage.substages[i].enemyPos[j].rotation));
                    newSubstageEnemyQua[i][j] = Quaternion.Normalize(Handles.RotationHandle(stage.substages[i].enemyPos[j].rotation, stage.substages[i].enemyPos[j].position));
                    pos_sum += newSubstageEnemyPos[i][j];
                    pos_count++;
                }
                Handles.Label(pos_sum/(float)pos_count, substageTexture);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(stage, "Changed Stage GUI");
                for (int i = 0; i < newPos.Length; i++)
                {
                    stage.spawnPos[i] = newPos[i];
                }
                for(int i = 0; i < newSubstageTeamPos.Count; i++)
                {
                    for(int j = 0; j < newSubstageTeamPos[i].Length; j++)
                    {
                        stage.substages[i].teamPos[j].position = newSubstageTeamPos[i][j];
                        stage.substages[i].teamPos[j].rotation = newSubstageTeamQua[i][j];
                    }
                    for (int j = 0; j < newSubstageEnemyPos[i].Length; j++)
                    {
                        stage.substages[i].enemyPos[j].position = newSubstageEnemyPos[i][j];
                        stage.substages[i].enemyPos[j].rotation = newSubstageEnemyQua[i][j];
                    }

                }
            }
        }
    }
}
#endif