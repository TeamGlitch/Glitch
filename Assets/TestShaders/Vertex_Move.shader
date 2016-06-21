Shader "ClaseShaders/Vertex_Move"
{
	Properties
	{
		_Amplitude ("Amplitude", Float) = 1.0
		_Frequency ("Frequency", Float) = 1.0
		_MainTex ("texture", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _Amplitude;
			float _Frequency;

			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;

				float res = _Amplitude * (sin(_Frequency * _Time.y) + 1.0);

				float4 newPosition = v.vertex;
				newPosition.xyz += v.normal * res;

				o.vertex = mul(UNITY_MATRIX_MVP, newPosition);

				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D( _MainTex, i.texcoord );
			}
			ENDCG
		}
	}
}
