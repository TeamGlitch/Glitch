Shader "Hidden/GlitchScene"
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
				if(i.uv.x < 0.1)
				{
					newUV.x = i.uv.x + (0.02 - (i.uv.x % 0.02));
					newUV.y = i.uv.y + (0.05 - (i.uv.y % 0.05));
				    final = tex2D(_MainTex,  newUV);
				}
				else if(i.uv.x >= 0.1 && i.uv.x < 0.15)
				{
					newUV.x = i.uv.x + (0.01 - (i.uv.x % 0.01));
					newUV.y = i.uv.y + (0.025 - (i.uv.y % 0.025));
				    final = tex2D(_MainTex,  newUV);
				}
				else
				{
					final = tex2D(_MainTex,  i.uv.xy);
				}

				if(i.uv.x < 0.1f)
					final -= half4(0.15,0.15,0.15,0.0);
				else if(i.uv.x >= 0.1f && i.uv.x < 0.15f)
					final -= half4(0.075,0.075,0.075,0.0);
				return final;
			}

			ENDCG
		}
	}
}
