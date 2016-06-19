Shader "Hidden/GlitchScene"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PercentagePixel ("PercentagePixel", float) = 0.2
		_PlayerPercentagePixelX ("PlayerPercentagePixelX", float) = 0.2
		_PlayerPercentagePixelY ("PlayerPercentagePixelY", float) = 0.2
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

			uniform float _PercentagePixel;
			uniform float _PlayerPercentagePixelX;
			uniform float _PlayerPercentagePixelY;
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
				if((((_PlayerPercentagePixelX - i.uv.x) * (_PlayerPercentagePixelX - i.uv.x) / 0.01 + (_PlayerPercentagePixelY - i.uv.y) * (_PlayerPercentagePixelY - i.uv.y) / 0.03) > 1) && 
					((i.uv.x < _PercentagePixel + 0.3 && (i.uv.y < 0.05 || i.uv.y > 0.95)) ||
					(i.uv.x < _PercentagePixel + 0.45 && (i.uv.y < 0.025 || i.uv.y > 0.975)) ||
					(i.uv.x < _PercentagePixel + 0.225 && (i.uv.y < 0.075 || i.uv.y > 0.925)) ||
					(i.uv.x < _PercentagePixel + 0.150 && (i.uv.y < 0.1 || i.uv.y > 0.9)) ||
					(i.uv.x < _PercentagePixel + 0.1125 && (i.uv.y < 0.125 || i.uv.y > 0.875)) ||
					(i.uv.x < _PercentagePixel + 0.075 && (i.uv.y < 0.15 || i.uv.y > 0.85)) ||
					(i.uv.x < _PercentagePixel + 0.05 && (i.uv.y < 0.175 || i.uv.y > 0.825)) ||
					(i.uv.x < _PercentagePixel + 0.025 && (i.uv.y < 0.2 || i.uv.y > 0.8)) ||
					(i.uv.x < _PercentagePixel + 0.012 && (i.uv.y < 0.225 || i.uv.y > 0.775)) ||
					(i.uv.x < _PercentagePixel)))
				{
					newUV.x = i.uv.x + (_PixelSize - (i.uv.x % _PixelSize));
					newUV.y = i.uv.y + (_PixelSize - (i.uv.y % _PixelSize));
				    final = tex2D(_MainTex,  newUV);
				}
				else
				{
					final = tex2D(_MainTex,  i.uv.xy);
				}

				return final;
			}

			ENDCG
		}
	}
}
