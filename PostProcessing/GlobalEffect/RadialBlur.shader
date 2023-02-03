
//----------------------------------------------------------------------------------------------------------
// X-PostProcessing Library
// https://github.com/QianMo/X-PostProcessing-Library
// Copyright (C) 2020 QianMo. All rights reserved.
// Licensed under the MIT License 
// You may not use this file except in compliance with the License.You may obtain a copy of the License at
// http://opensource.org/licenses/MIT
//----------------------------------------------------------------------------------------------------------

Shader "Hidden/RadialBlur"
{
	HLSLINCLUDE

	#include "UnityCG.cginc"

	struct VaryingsDefault
	{
		float4 vertex : SV_POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct AttributesDefault
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    float4 _MainTex_ST;

	uniform half4 _Params;
	
	#define _BlurRadius _Params.x
	#define _Iteration _Params.y
	#define _RadialCenter _Params.zw

	float2 TransformTriangleVertexToUV(float2 vertex)
	{
		float2 uv = (vertex + 1.0) * 0.5;
		return uv;
	}
	
	half4 RadialBlur(VaryingsDefault i)
	{
		float2 blurVector = (_RadialCenter - i.texcoord.xy) * _BlurRadius;
		
		half4 acumulateColor = half4(0, 0, 0, 0);
		
		for (int j = 0; j < _Iteration; j ++)
		{
			acumulateColor += tex2D(_MainTex, i.texcoord);
			i.texcoord.xy += blurVector;
		}
		
		return acumulateColor / _Iteration;
	}

	VaryingsDefault VertDefault(AttributesDefault v)
	{
		VaryingsDefault o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = TRANSFORM_TEX(v.uv, _MainTex);

		return o;
	}

	half4 Frag(VaryingsDefault i): SV_Target
	{
		return RadialBlur(i);
	}
	
	ENDHLSL

	Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		
		Pass
		{
			HLSLPROGRAM
			
			#pragma vertex VertDefault
			#pragma fragment Frag
			
			ENDHLSL
		}
	}
}

