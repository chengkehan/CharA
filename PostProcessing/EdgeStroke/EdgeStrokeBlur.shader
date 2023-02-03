Shader "Hidden/EdgeStroke/Blur"  
{  
    Properties  
    {  
        _MainTex("Base (RGB)", 2D) = "white" {}  
    }  
  
    CGINCLUDE  
  
    #include "UnityCG.cginc"  
  
    sampler2D _MainTex;  
    uniform half4 _MainTex_TexelSize; 
  
    struct VertexInput  
    {  
        float4 vertex : POSITION;  
        half2 texcoord : TEXCOORD0;  
    };  
  
    struct VertexOutput_DownSmpl  
    {  
        float4 pos : SV_POSITION;  
        half2 uv20 : TEXCOORD0;  
        half2 uv21 : TEXCOORD1;  
        half2 uv22 : TEXCOORD2;  
        half2 uv23 : TEXCOORD3;  
        half2 uv : TEXCOORD4;
    };
  
    VertexOutput_DownSmpl vert_DownSmpl(VertexInput v)  
    {  
        VertexOutput_DownSmpl o;
        UNITY_INITIALIZE_OUTPUT(VertexOutput_DownSmpl, o);
  
        o.pos = UnityObjectToClipPos(v.vertex);  
        o.uv20 = v.texcoord + _MainTex_TexelSize.xy* half2(0.5h, 0.5h);;  
        o.uv21 = v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h, -0.5h);  
        o.uv22 = v.texcoord + _MainTex_TexelSize.xy * half2(0.5h, -0.5h);  
        o.uv23 = v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h, 0.5h);  
  
        return o;  
    }  
  
    fixed4 frag_DownSmpl(VertexOutput_DownSmpl i) : SV_Target  
    {  
        fixed4 color = fixed4(0,0,0,0);  
  
        color += tex2D(_MainTex, i.uv20);  
        color += tex2D(_MainTex, i.uv21);  
        color += tex2D(_MainTex, i.uv22);  
        color += tex2D(_MainTex, i.uv23);  
  
        color /=4;

        return color;  
    }  
  
    ENDCG  

    SubShader  
    {    
        Pass // 0
        {  
            Cull Off ZWrite Off ZTest Always
            CGPROGRAM  
            #pragma vertex vert_DownSmpl  
            #pragma fragment frag_DownSmpl  
            ENDCG  
  
        }  
    } 
}  