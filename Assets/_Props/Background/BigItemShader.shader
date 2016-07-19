Shader "Unlit/BigItemShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="TransparentCutout" }
		LOD 100

		Pass {
			CGPROGRAM

			#pragma vertex vert             
			#pragma fragment frag

			sampler2D _MainTex;


			struct vertInput {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};  

			struct vertOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			vertOutput vert(vertInput input) {
				vertOutput o;
				o.pos = mul(UNITY_MATRIX_MVP, input.pos);
				o.uv = input.uv;
				return o;
			}

			half4 frag(vertOutput output) : COLOR {
				half4 final;
				float2 newUV;
	
				float time = _Time[1] % 0.9;
				if(time < 0.1)
				{
					newUV.x = output.uv.x / 3;
					newUV.y = output.uv.y / 3;
				}
				else if(time < 0.2)
				{
					newUV.x = output.uv.x / 3 + 0.3333;
					newUV.y = output.uv.y / 3;
				}
				else if(time < 0.3)
				{
					newUV.x = output.uv.x / 3 + 0.6666;
					newUV.y = output.uv.y / 3;
				}
				else if(time < 0.4)
				{
					newUV.x = output.uv.x / 3;
					newUV.y = output.uv.y / 3 + 0.3333;
				}
				else if(time < 0.5)
				{
					newUV.x = output.uv.x / 3 + 0.3333;
					newUV.y = output.uv.y / 3 + 0.3333;
				}
				else if(time < 0.6)
				{
					newUV.x = output.uv.x / 3 + 0.6666;
					newUV.y = output.uv.y / 3 + 0.3333;
				}
				else if(time < 0.7)
				{
					newUV.x = output.uv.x / 3;
					newUV.y = output.uv.y / 3 + 0.6666;
				}
				else if(time < 0.8)
				{
					newUV.x = output.uv.x / 3 + 0.3333;
					newUV.y = output.uv.y / 3 + 0.6666;
				}
				else
				{
					newUV.x = output.uv.x / 3 + 0.6666;
					newUV.y = output.uv.y / 3 + 0.6666;
				}

				final = tex2D(_MainTex,  newUV);

				return final;

			}
			ENDCG
		}
	}
}

