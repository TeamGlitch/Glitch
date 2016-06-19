Shader "Hidden/GlitchLine"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		yPercentage ("_yPercentage", float) = 0.5
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
			float yPercentage;

			half4 frag (v2f i) : COLOR
			{
				float2 newUV;
				half4 final;

				final = tex2D(_MainTex,  i.uv.xy);

				if(i.uv.y >= yPercentage && i.uv.y <= yPercentage + 0.05)
				{
					final += half4(0.5,0.5,0.5,1);
				}

				return final;
			}

			ENDCG
		}
	}
}
