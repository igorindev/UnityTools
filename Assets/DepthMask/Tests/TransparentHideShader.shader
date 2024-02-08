Shader "Custom/TransparentHideShader"
{
	Properties
	{
		[MainTexture][HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		[HideInInspector] _Color ("Color (RGBA)", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Name "UI Transparent"
		Tags 
		{
			"RenderPipeline" = "UniversalPipeline" 
			"Queue" = "Transparent+2"  
			"IgnoreProjector"="True" 
		}
		Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			HLSLPROGRAM 

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			
			struct appdata
			{
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
				half4 color : COLOR;
			};

			struct v2f
			{
				half2 uv : TEXCOORD0;
				half fogFactor : TEXCOORDn;
				half4 vertex : SV_POSITION;
				half4 color : COLOR;
			};

			half4 _Color;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			CBUFFER_START(UnityPerMaterial)
			half4 _MainTex_ST;
			CBUFFER_END

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.fogFactor = ComputeFogFactor(o.vertex.z);
				o.color = v.color;
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				col = col * i.color;
				col.rgb = MixFog(col.rgb, i.fogFactor);
				return col;
			}
			ENDHLSL 
		}
	}
}