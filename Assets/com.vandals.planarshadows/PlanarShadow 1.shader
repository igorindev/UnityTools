Shader "Custom/PlanarShadow" 
{
	Properties {
		_ShadowColor ("Shadow Color", Color) = (0,0,0,1)
		_PlaneHeight ("Plane Height", Float) = 0
		[CurvedWorldBendSettings] _CurvedWorldBendSettings("0,10|1,4|1", Vector) = (0, 0, 0, 0)
	}

	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

		// shadow color
		Pass {   
			
			ZWrite On
			ZTest LEqual 
			Blend SrcAlpha OneMinusSrcAlpha
			
			Stencil {
				Ref 0
				Comp Equal
				Pass IncrWrap
				ZFail Keep
			}
			
			CGPROGRAM 
			#include "PlanarShadowBase.cginc"     

            #pragma vertex vert
            #pragma fragment frag 
  
            #include "UnityCG.cginc"     

            struct v2f 
            {
				float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_full v)
            {
                v2f o;
	
				o.vertex = vertPlanarShadow(v).pos;
				
				return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                return fragPlanarShadow();
            }

            ENDCG

		}
	}
}
