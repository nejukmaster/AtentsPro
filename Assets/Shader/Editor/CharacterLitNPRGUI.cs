using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using static UnityEditor.Rendering.Universal.ShaderGUI.LitGUI;

namespace AtentsPro.ShaderGUI
{
    /// <summary>
    /// Editor script for the Lit material inspector.
    /// </summary>
    public static class CharacterLitNPRGUI
    {

        public static class Styles
        {
            public static readonly GUIContent nprInput = EditorGUIUtility.TrTextContent("NPR Inputs", "NPR 설정.");

            public static string useShadowMaskText = "Use Shadow Caster Mask";
            public static readonly GUIContent shadowMaskText = EditorGUIUtility.TrTextContent("Shadow Caster Mask", "텍스쳐에서 0.5이하의 부분은 ShadowCaster에서 랜더링하지 않습니다.");
            public static string shadowColorText = "Shadow Color";
            public static string shadowThresholdText = "Shadow Threshold";
            public static string shadowSmoothText = "Shadow Smooth";
            public static string useShadowSpherizingText = "Use Shadow Spherizing";
            public static string shadowSpherizingText = "Shadow Spherizing";

            public static string fresnelColorText = "Fresnel Color";
            public static string fresnelThresholdText = "Fresnel Threshold";
            public static string fresnelSmoothText = "Fresnel Smooth";
            public static string useFresnelSpherizingText = "Use Fresnel Spherizing";
            public static string fresnelSpherizingText = "Fresnel Spherizing";

            public static string specularColorText = "Specular Color";
            public static string specularThresholdText = "Specular Threshold";
            public static string specularSmoothText = "Specular Smooth";
            public static string useSpecularSpherizingText = "Use Specular Spherizing";
            public static string specularSpherizingText = "Specular Spherizing";

            public static string environmentLightsText = "environment Lights";

            public static string giIntensityText = "GI Intensity";
            public static string giTresholdText = "GI Threshold";
            public static string giSmoothText = "GI Smooth";
        }

        /// <summary>
        /// Container for the properties used in the <c>LitGUI</c> editor script.
        /// </summary>
        public struct NPRProperties
        {
            public MaterialProperty useShadowCasterMask;
            public MaterialProperty shadowCasterMask;

            public MaterialProperty shadowColor;
            public MaterialProperty shadowThreshold;
            public MaterialProperty shadowSmooth;
            public MaterialProperty useShadowSpherizing;
            public MaterialProperty shadowSpherizing;

            public MaterialProperty fresnelColor;
            public MaterialProperty fresnelThreshold;
            public MaterialProperty fresnelSmooth;
            public MaterialProperty useFresnelSpherizing;
            public MaterialProperty fresnelSpherizing;

            public MaterialProperty specularColor;
            public MaterialProperty specularThreshold;
            public MaterialProperty specularSmooth;
            public MaterialProperty useSpecularSpherizing;
            public MaterialProperty specularSpherizing;

            public MaterialProperty environmentLights;

            public MaterialProperty giIntensity;
            public MaterialProperty giThreshold;
            public MaterialProperty giSmooth;

            public NPRProperties(MaterialProperty[] properties)
            {
                useShadowCasterMask = BaseShaderGUI.FindProperty("_UseShadowCasterMask", properties, false);
                shadowCasterMask = BaseShaderGUI.FindProperty("_ShadowCasterMask", properties, false);
                shadowColor = BaseShaderGUI.FindProperty("_ShadowColor", properties, false);
                shadowThreshold = BaseShaderGUI.FindProperty("_ShadowThreshold", properties, false);
                shadowSmooth = BaseShaderGUI.FindProperty("_ShadowSmooth", properties, false);
                useShadowSpherizing = BaseShaderGUI.FindProperty("_UseShadowSpherizing", properties, false);
                shadowSpherizing = BaseShaderGUI.FindProperty("_ShadowSpherizing", properties, false);

                fresnelColor = BaseShaderGUI.FindProperty("_FresnelColor", properties, false);
                fresnelThreshold = BaseShaderGUI.FindProperty("_FresnelThreshold", properties, false);
                fresnelSmooth = BaseShaderGUI.FindProperty("_FresnelSmooth", properties, false);
                useFresnelSpherizing = BaseShaderGUI.FindProperty("_UseFresnelSpherizing", properties, false);
                fresnelSpherizing = BaseShaderGUI.FindProperty("_FresnelSpherizing", properties, false);

                specularColor = BaseShaderGUI.FindProperty("_SpecularColor", properties, false);
                specularThreshold = BaseShaderGUI.FindProperty("_SpecularThreshold", properties, false);
                specularSmooth = BaseShaderGUI.FindProperty("_SpecularSmooth", properties, false);
                useSpecularSpherizing = BaseShaderGUI.FindProperty("_UseSpecularSpherizing", properties, false);
                specularSpherizing = BaseShaderGUI.FindProperty("_SpecularSpherizing", properties, false);

                environmentLights = BaseShaderGUI.FindProperty("_EnvironmentLights", properties, false);

                giIntensity = BaseShaderGUI.FindProperty("_GIIntensity", properties, false);
                giThreshold = BaseShaderGUI.FindProperty("_GIThreshold", properties, false);
                giSmooth = BaseShaderGUI.FindProperty("_GISmooth", properties, false);
            }
        }

        public static void DoDetailArea(NPRProperties properties, MaterialEditor materialEditor)
        {
            materialEditor.ShaderProperty(properties.useShadowCasterMask, Styles.useShadowMaskText);
            if(properties.useShadowCasterMask.floatValue == 1.0f)
                materialEditor.TexturePropertySingleLine(Styles.shadowMaskText, properties.shadowCasterMask);
            EditorGUILayout.Space(30);
            materialEditor.ColorProperty(properties.shadowColor, Styles.shadowColorText);
            materialEditor.ShaderProperty(properties.shadowThreshold, Styles.shadowThresholdText);
            materialEditor.ShaderProperty(properties.shadowSmooth, Styles.shadowSmoothText);
            materialEditor.ShaderProperty(properties.useShadowSpherizing, Styles.useShadowSpherizingText);
            if(properties.useShadowSpherizing.floatValue == 1.0f)
                materialEditor.ShaderProperty(properties.shadowSpherizing, Styles.shadowSpherizingText);
            EditorGUILayout.Space(30);
            materialEditor.ColorProperty(properties.fresnelColor, Styles.fresnelColorText);
            materialEditor.ShaderProperty(properties.fresnelThreshold, Styles.fresnelThresholdText);
            materialEditor.ShaderProperty(properties.fresnelSmooth, Styles.fresnelSmoothText);
            materialEditor.ShaderProperty(properties.useFresnelSpherizing, Styles.useFresnelSpherizingText);
            if (properties.useFresnelSpherizing.floatValue == 1.0f)
                materialEditor.ShaderProperty(properties.fresnelSpherizing, Styles.fresnelSpherizingText);
            EditorGUILayout.Space(30);
            materialEditor.ColorProperty(properties.specularColor, Styles.specularColorText);
            materialEditor.ShaderProperty(properties.specularThreshold, Styles.specularThresholdText);
            materialEditor.ShaderProperty(properties.specularSmooth, Styles.specularSmoothText);
            materialEditor.ShaderProperty(properties.useSpecularSpherizing, Styles.useSpecularSpherizingText);
            if (properties.useSpecularSpherizing.floatValue == 1.0f)
                materialEditor.ShaderProperty(properties.specularSpherizing, Styles.specularSpherizingText);
            EditorGUILayout.Space(30);
            materialEditor.ShaderProperty(properties.environmentLights, Styles.environmentLightsText);
            EditorGUILayout.Space(30);
            materialEditor.ShaderProperty(properties.giIntensity, Styles.giIntensityText);
            materialEditor.ShaderProperty(properties.giThreshold, Styles.giTresholdText);
            materialEditor.ShaderProperty(properties.giSmooth, Styles.giSmoothText);
        }

        public static void SetMaterialKeywords(Material material)
        {
            CoreUtils.SetKeyword(material, "_USESHADOWCASTERMASK", material.GetFloat("_UseShadowCasterMask") == 1.0f);
            CoreUtils.SetKeyword(material, "_USESHADOWSPHERIZING", material.GetFloat("_UseShadowSpherizing") == 1.0f);
            CoreUtils.SetKeyword(material, "_USEFRESNELSPHERIZING", material.GetFloat("_UseFresnelSpherizing") == 1.0f);
            CoreUtils.SetKeyword(material, "_USESPECULARSPHERIZING", material.GetFloat("_UseSpecularSpherizing") == 1.0f);
        }
    }
}
