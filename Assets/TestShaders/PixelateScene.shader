Shader "Hidden/PixelateScene"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PixelSize ("PixelSize", float) = 0.005
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

			uniform float _PixelSize;

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

				newUV.x = i.uv.x + (_PixelSize - (i.uv.x % _PixelSize));
				newUV.y = i.uv.y + (_PixelSize - (i.uv.y % _PixelSize));
			    final = tex2D(_MainTex,  newUV);

				return final;
			}

			ENDCG
		}
	}
}
