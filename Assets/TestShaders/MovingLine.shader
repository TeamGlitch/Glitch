Shader "Hidden/MovingLine"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		xPercentage ("_xPercentage", float) = 0.5
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
			float xPercentage;
			float yPercentage;

			half4 frag (v2f i) : COLOR
			{
				float2 newUV;
				half4 final;

				bool normal = true;

				if(i.uv.x >= xPercentage-0.06 && i.uv.x <= xPercentage-0.055)
				{
					if(i.uv.y >= yPercentage - 0.03 && i.uv.y <= yPercentage - 0.025)
					{
						normal = false;
					}
				}
				else if(i.uv.x > xPercentage-0.055 && i.uv.x <= xPercentage-0.05)
				{
					if(i.uv.y >= yPercentage - 0.025 && i.uv.y <= yPercentage - 0.02)
					{
						normal = false;
					}
				}
				else if(i.uv.x > xPercentage-0.05 && i.uv.x <= xPercentage-0.045)
				{
					if(i.uv.y >= yPercentage - 0.02 && i.uv.y <= yPercentage - 0.015)
					{
						normal = false;
					}
				}
				else if(i.uv.x > xPercentage-0.045 && i.uv.x <= xPercentage-0.04)
				{
					if(i.uv.y >= yPercentage - 0.025 && i.uv.y <= yPercentage - 0.02)
					{
						normal = false;
					}
				}
				else if(i.uv.x > xPercentage-0.04 && i.uv.x <= xPercentage-0.035)
				{
					if(i.uv.y >= yPercentage - 0.03 && i.uv.y <= yPercentage - 0.025)
					{
						normal = false;
					}
				}
				else if(i.uv.x > xPercentage-0.035 && i.uv.x <= xPercentage-0.03)
				{
					if(i.uv.y >= yPercentage - 0.035 && i.uv.y <= yPercentage - 0.03)
					{
						normal = false;
					}
				}
				else if(i.uv.x > xPercentage-0.035 && i.uv.x <= xPercentage-0.03)
				{
					if(i.uv.y >= yPercentage - 0.035 && i.uv.y <= yPercentage - 0.03)
					{
						normal = false;
					}
				}



				final = tex2D(_MainTex,  i.uv.xy);

				if((normal && (i.uv.x <= xPercentage-0.06 || i.uv.x >= xPercentage - 0.03) && i.uv.y >= yPercentage - 0.035 && i.uv.y <= yPercentage - 0.03) || !normal)
				{
					final += half4(0.5,0.5,0.5,1);
				}

				return final;
			}

			ENDCG
		}
	}
}
