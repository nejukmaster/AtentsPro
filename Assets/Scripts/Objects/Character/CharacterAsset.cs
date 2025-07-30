using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AtentsPro
{
    [CreateAssetMenu(fileName = "NewCharacterAsset", menuName = "AtentsPro/CharacterAsset")]
#if UNITY_EDITOR
    [CanEditMultipleObjects]
#endif
    public class CharacterAsset : ScriptableObject
    {
        public static CharacterAsset Load(string characterName)
        {
            return Resources.Load<CharacterAsset>(AtentsProGlobalSettings.Instance.characterAssetPath + characterName);
        }

        public GameObject model;
        public CharacterAnimationInfo animationInfo;
    }
}
