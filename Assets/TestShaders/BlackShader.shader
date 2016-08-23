Shader "Unlit/BlackShader"
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

				col = tex2D(_MainTex, i.uv);		

				float percentage = (_ScreenParams.y - i.vertex.y) / _ScreenParams.y;

				if(percentage < 0.05)
				{
					float blackValue = (0.05 - percentage) / 0.05;
					blackValue /= 3;
					col -= fixed4(blackValue, blackValue, blackValue, blackValue);
				}

				return col;
			}
			ENDCG
		}
	}
}
