Shader "Hidden/EdgeStroke/Sharpen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DEBUG
			
			#include "UnityCG.cginc"

			sampler2D _SharpenBlurRT;
			float _SharpenIntensity;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			inline half3 hard_light(half3 a, half3 b)
			{
				// return a < 0.5 ? (2 * a * b) : (1 - 2 * (1 - a) * (1 - b)); =>

				half3 c = 1 - step(a, half3(0.5, 0.5, 0.5));
				half3 v = 4 * a*b*c - 2 * a*b - 2 * a*c - 2 * b*c + 2 * a + 2 * b + c - 1; // using fma to optimize
				return v;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			half4 frag (v2f i) : SV_Target
			{
				half4 color = tex2D(_MainTex, i.uv);
				
				half4 highPassFilterC = tex2D(_SharpenBlurRT, i.uv);
				half3 cHP = color.rgb - highPassFilterC.rgb + 0.5;
				cHP = (cHP - 0.5) * _SharpenIntensity + 0.5;

				#if defined(DEBUG)
				return half4(cHP, 1);
				#endif

				color.rgb = hard_light(cHP.rgb, color.rgb);

				return color;
			}
			ENDCG
		}
	}
}
