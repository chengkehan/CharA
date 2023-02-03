Shader "Custom/StylizedScreen"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 scrPos : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};

			sampler2D _MainTex;
			uniform sampler2D _LayeredMaskRT;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.pos);
				o.uv = v.uv;
				return o;
			}

			#define vec2 float2
			#define vec3 float3
			#define vec4 float4
			#define mat2 float2x2
			#define mat3 float3x3
			#define mat4 float4x4
			#define iGlobalTime _Time.y
			#define mod fmod
			#define mix lerp
			#define fract frac
			#define texture tex2D
			#define iResolution _ScreenParams
			#define gl_FragCoord ((_iParam.scrPos.xy/_iParam.scrPos.w) * _ScreenParams.xy)
 
			#define PI2 6.28318530717959

			#define RANGE 16.
			#define STEP 4.
			#define ANGLENUM 5

			// Here's some magic numbers, and two groups of settings that I think looks really nice. 
			// Feel free to play around with them!

			#define MAGIC_GRAD_THRESH 0.01

			// Setting group 2:
			#define MAGIC_SENSITIVITY     5.

			vec4 getCol(vec2 pos)
			{
				vec2 uv = pos / iResolution.xy;
				return texture(_MainTex, uv);
			}

			float getVal(vec2 pos)
			{
				vec4 c=getCol(pos);
				return dot(c.xyz, vec3(0.2126, 0.7152, 0.0722));
			}

			vec2 getGrad(vec2 pos, float eps)
			{
   				vec2 d=vec2(eps,0);
				return vec2(
					getVal(pos+d.xy)-getVal(pos-d.xy),
					getVal(pos+d.yx)-getVal(pos-d.yx)
				)/eps/2.;
			}

			void pR(inout vec2 p, float a) {
				p = cos(a)*p + sin(a)*vec2(p.y, -p.x);
			}
			float absCircular(float t)
			{
				float a = floor(t + 0.5);
				return mod(abs(a - t), 1.0);
			}

			void main(vec2 fragCoord, out vec4 fragColor)
			{
				vec2 pos = fragCoord;
				float weight = 1.0;

				half4 maskC = texture(_LayeredMaskRT, pos / iResolution.xy);

				UNITY_BRANCH
				if (maskC.b > 0.1)
				{
					UNITY_UNROLL
					for (float j = 0.; j < ANGLENUM; j += 1.)
					{
						vec2 dir = vec2(1, 0);
						pR(dir, j * PI2 / (2. * ANGLENUM));
        
						vec2 grad = vec2(-dir.y, dir.x);

						vec2 dirN = normalize(dir);
						vec2 gradN = normalize(grad);

						UNITY_UNROLL
						for (float i = -RANGE; i <= RANGE; i += STEP)
						{
							vec2 pos2 = pos + dirN*i;
            
							// video texture wrap can't be set to anything other than clamp  (-_-)
							if (pos2.y < 0. || pos2.x < 0. || pos2.x > iResolution.x || pos2.y > iResolution.y)
								continue;
            
							vec2 g = getGrad(pos2, 1.);
							if (length(g) < MAGIC_GRAD_THRESH)
								continue;
            
							weight -= pow(abs(dot(gradN, normalize(g))), MAGIC_SENSITIVITY) / floor((2. * RANGE + 1.) / STEP) / ANGLENUM;
						}
					}
				}
				

				vec4 col = getCol(pos);

				float scaledWeight = weight * weight * weight * weight;
				scaledWeight = lerp(1, scaledWeight, maskC.b);
				fragColor = col * scaledWeight;
			}

			float4 frag(v2f _iParam) : SV_Target
			{ 
				vec2 fragCoord = gl_FragCoord;
				vec4 fragColor;
				main(gl_FragCoord, fragColor);
				return fragColor;
			}  

			

			ENDCG
		}
	}
}
