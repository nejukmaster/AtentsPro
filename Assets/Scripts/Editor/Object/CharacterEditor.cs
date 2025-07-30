#if UNITY_EDITOR
using Codice.Client.BaseCommands.Merge.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AtentsPro
{
    [CustomEditor(typeof(Character))]
    public class CharacterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Character asset = (Character)target;
            Status status = asset.GetStatus();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("HP: " + status.hp + "/" + status.maxHp);
        }
    }
}
#endif
