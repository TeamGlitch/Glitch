Shader "Unlit/PixelatedShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 newUV;
				fixed4 col;
				if((i.uv.x % 0.025 > 0.0 && i.uv.x % 0.025 < 0.001) || 
					(i.uv.y % 0.025 > 0.0 && i.uv.y % 0.025 < 0.001))
				{
					col = fixed4(0.0,0.0,0.0,0.0);
				}
				else
				{
					newUV.x = i.uv.x + (0.025 - (i.uv.x % 0.025));
					newUV.y = i.uv.y + (0.025 - (i.uv.y % 0.025));
					col = tex2D(_MainTex, newUV);
				}
				return col;
			}
			ENDCG
		}
	}
}
