Shader"AtentsPro/CharacterOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineWidth("Outline Width", Range(0,50)) = 1
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineCutoff("Outline Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        LOD 300
        Cull Front
        ZTest Less

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vertexOutline
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float _OutlineWidth;
                half4 _OutlineColor;
                float _OutlineCutoff;
            CBUFFER_END

            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                
	            UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS :VAR_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDIr : TEXCOORD1;
                float3 normalWS : NORMAL;
	            UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            VertexOutput vertexOutline(VertexInput input)
            {
                VertexOutput output = (VertexOutput)0;
	            UNITY_SETUP_INSTANCE_ID(input);
	            UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionWS = TransformObjectToWorld(input.vertex.xyz);
                output.positionCS = TransformObjectToHClip(input.vertex.xyz);
    
                float3 clipNormal = TransformObjectToHClip(input.normal);
                float2 offset = normalize(clipNormal.xy) / _ScreenParams.xy * _OutlineWidth * output.positionCS.w;
                output.viewDIr = _WorldSpaceCameraPos.xyz - TransformObjectToWorld(input.vertex.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normal);

                float ndv = dot(normalize(output.normalWS), normalize(output.viewDIr));
                output.positionCS.xy += offset*(1.0-ndv);

                output.uv = input.uv;

                return output;
            }
            half4 frag (VertexOutput input) : SV_Target
            {
	            UNITY_SETUP_INSTANCE_ID(input);
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
