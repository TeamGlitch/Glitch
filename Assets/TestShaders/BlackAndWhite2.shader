﻿Shader "Hidden/BackAndWhite2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			half4 frag (v2f i) : COLOR
			{
				float2 newUV;
				half4 final;

				final = tex2D(_MainTex,  i.uv.xy);

				float aux = 1 - (final.x * 0.2989 + final.y * 0.5870 + final.z * 0.1140);
				final.x = final.y = final.z = aux;

				return final;
			}

			ENDCG
		}
	}
}
