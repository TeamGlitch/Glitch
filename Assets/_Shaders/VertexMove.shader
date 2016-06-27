Shader "Unlit/VertexMove"
{
	Properties
	{
		_MainTex("texture", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				if (_Time[1] % 0.2 >= 0.15)
				{
					o.vertex.x += 0.5;
				}
				else if (_Time[1] % 0.2 >= 0.1)
				{
					o.vertex.x -= 0.5;
				}
				else if (_Time[1] % 0.2 >= 0.05)
				{
					o.vertex.x += 0.5;
				}
				else
				{
					o.vertex.x -= 0.5;
				}

				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.texcoord);
			}
				ENDCG
		}
	}
}
