Shader "Custom/TransparentCoverShader" 
{
	Properties
	{
		[MainTexture][HideInInspector] _MainTex("Base (RGBA)", 2D) = "white" {}
		_Cutoff("Base Alpha cutoff", Range(0.01,.5)) = .5
	}

	SubShader
	{
		Name "UI Covert Transparent"
		Offset 0, -1
		Cull Off

		Tags 
		{
			"RenderPipeline" = "UniversalPipeline" 
			"Queue" = "Transparent+1"  
			"IgnoreProjector"="True" 
		}
	
		Pass
		{
			HLSLPROGRAM 

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct appdata
			{
				half4 positionOS  : POSITION;
				half2 uv : TEXCOORD0;
				half4 color : COLOR;
			};

			struct v2f
			{
				half4 positionHCS  : SV_POSITION;
				half2 uv : TEXCOORD0;
				half4 color : COLOR;
			};

			half4 _Color;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			CBUFFER_START(UnityPerMaterial)
			half4 _MainTex_ST;
			half _Cutoff;
			CBUFFER_END

			v2f vert(appdata v)
			{
			    v2f o;
			    o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
			    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			    o.color = v.color;
			    return o;
			}
			
			half4 frag(v2f i) : SV_Target
			{
			    half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;
				clip(col.a - _Cutoff);
			    return col;
			}
			ENDHLSL 
		}
	}
}
