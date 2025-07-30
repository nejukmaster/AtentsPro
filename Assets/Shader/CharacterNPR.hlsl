#ifndef ATENTS_CHARACTER_NPR_INCLUDED
#define ATENTS_CHARACTER_NPR_INCLUDED

struct NPRFactor {
	half3 shadowColor;
	float shadowThreshold;
	float shadowSmooth;
	float shadowSpherizing;

	half3 fresnelColor;
	float fresnelThreshold;
	float fresnelSmooth;
	float fresnelSpherizing;

	half3 specularColor;
	float specularThreshold;
	float specularSmooth;
	float specularSpherizing;

	float environmentLights;

	float giIntensity;
	float giThreshold;
	float giSmooth;

	float3 vertexAveragePos;
};

#define MIN_REFLECTIVITY 0.04

float OneMinusReflectivity(float metallic) {
	float range = 1.0 - MIN_REFLECTIVITY;
	return range - metallic * range;
}

float Square(float x) {
	return x * x;
}

float GetSpecular(InputData inputData, BRDFData brdf, Light light, float3 normal) {
	float3 h = SafeNormalize(light.direction + inputData.viewDirectionWS);
	float nh2 = Square(saturate(dot(normal, h)));
	float lh2 = Square(saturate(dot(light.direction, h)));
	float r2 = Square(brdf.roughness);
	float d2 = Square(nh2 * (r2 - 1.0) + 1.00001);
	float normalization = brdf.roughness * 4.0 + 2.0;
	return r2 / (d2 * max(0.1, lh2) * normalization);
}

half3 SampleEnvironment(InputData input, BRDFData brdf) {
	float3 uvw = reflect(-input.viewDirectionWS, input.normalWS);
	float mip = PerceptualRoughnessToMipmapLevel(brdf.perceptualRoughness);
	float4 environment = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, uvw, mip);
	return DecodeHDREnvironment(environment, unity_SpecCube0_HDR);
}

half3 GetNPR(InputData inputData, SurfaceData surfaceData, BRDFData brdf, NPRFactor factors, half3 environment, Light light) {
	float3 centerToSurface = normalize(inputData.positionWS - factors.vertexAveragePos);
#ifdef _USESHADOWSPHERIZING
	float3 shadowNormal = lerp(inputData.normalWS, centerToSurface, factors.shadowSpherizing);
#else
	float3 shadowNormal = inputData.normalWS;
#endif
	half3 color = surfaceData.albedo + environment/(brdf.roughness2 + 1.0) * factors.environmentLights;

	float ndl = dot(shadowNormal, light.direction);
	float ndv = dot(shadowNormal, inputData.viewDirectionWS);
	float halfLambert = saturate(ndl*light.shadowAttenuation) * 0.5 + 0.5;

	//ndv값이 0보다 작을경우 매시 뒷면으로 판단하고 그림자값을 최대로 고정
	half shadow = ndv >= 0? 1 - smoothstep(factors.shadowThreshold - factors.shadowSmooth, factors.shadowThreshold + factors.shadowSmooth, halfLambert) : 1;

	color *= lerp(light.color, factors.shadowColor, shadow);

#ifdef _USEFRESNELSPHERIZING
	float3 fresnelNormal = lerp(inputData.normalWS, centerToSurface, factors.fresnelSpherizing);
#else
	float3 fresnelNormal = inputData.normalWS;
#endif

	float fresnelStrength = Pow4(1.0 - saturate(dot(fresnelNormal, inputData.viewDirectionWS))) * (1 - shadow);
	float smoothFresnel = smoothstep(factors.fresnelThreshold - factors.fresnelSmooth, factors.fresnelThreshold + factors.fresnelSmooth, fresnelStrength);

	color = lerp(color, factors.fresnelColor, smoothFresnel);

	half3 gi = 0;
	gi.r = smoothstep(factors.giThreshold - factors.giSmooth, factors.giThreshold + factors.giSmooth, inputData.bakedGI.r);
	gi.g = smoothstep(factors.giThreshold - factors.giSmooth, factors.giThreshold + factors.giSmooth, inputData.bakedGI.g);
	gi.b = smoothstep(factors.giThreshold - factors.giSmooth, factors.giThreshold + factors.giSmooth, inputData.bakedGI.b);

	color += saturate(gi * factors.giIntensity);

#ifdef _USESPECULARSPHERIZING
	float3 specularNormal = lerp(inputData.normalWS, centerToSurface, factors.specularSpherizing);
#else
	float3 specularNormal = inputData.normalWS;
#endif
	float specular = GetSpecular(inputData, brdf, light, specularNormal) * (1-shadow);
	float smoothSpecular = smoothstep(factors.specularThreshold - factors.specularSmooth, factors.specularThreshold + factors.specularSmooth, specular);

	color = lerp(color, factors.specularColor, smoothSpecular);

	return color;
}

half4 CharacterFragmentNPR(InputData inputData, SurfaceData surfaceData, NPRFactor factors) {
	half4 shadowMask = CalculateShadowMask(inputData);
	AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData, surfaceData);
	Light mainLight = GetMainLight(inputData, shadowMask, aoFactor);

	BRDFData brdf;
	InitializeBRDFData(surfaceData, brdf);

	half3 environment = SampleEnvironment(inputData, brdf);

	half3 color = GetNPR(inputData, surfaceData, brdf, factors, environment, mainLight);

	uint pixelLightCount = GetAdditionalLightsCount();
	LIGHT_LOOP_BEGIN(pixelLightCount)
		Light additionalLight = GetAdditionalLight(lightIndex, inputData.positionWS, half4(1, 1, 1, 1));
		color +=  GetNPR(inputData, surfaceData, brdf, factors, environment, additionalLight);
	LIGHT_LOOP_END

	half4 outcolor = half4(color, surfaceData.alpha);
	return outcolor;
}

#endif
