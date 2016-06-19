Shader "Unlit/ObjectDistorionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			CGPROGRAM

			#pragma vertex vert             
			#pragma fragment frag

			struct vertInput {
				float4 pos : POSITION;
			};  

			struct vertOutput {
				float4 pos : SV_POSITION;
			};

			vertOutput vert(vertInput input) {
				vertOutput o;
				o.pos = mul(UNITY_MATRIX_MVP, input.pos);
				if(_Time[1] % 0.2 >= 0.15)
				{
					o.pos.x += 0.25;
					o.pos.y += 0.25;
				}
				else if(_Time[1] % 0.2 >= 0.1)
				{
					o.pos.x -= 0.25;
					o.pos.y += 0.25;
				}
				else if(_Time[1] % 0.2 >= 0.05)
				{
					o.pos.x += 0.25;
					o.pos.y -= 0.25;
				}
				else
				{
					o.pos.x -= 0.25;
					o.pos.y -= 0.25;
				}
				return o;
			}

			half4 frag(vertOutput output) : COLOR {
				half4 temporalResult;
				if(_Time[1] % 0.2 >= 0.15)
				{
					if(output.pos.y >= 7.5)
						temporalResult = half4(0.5, 0.5, 0.5, 0.5);
					else if(output.pos.y % 4 >= 5.0)
						temporalResult = half4(0.125, 0.125, 0.125, 0.5);
					else if(output.pos.y % 4 >= 2.5)
						temporalResult = half4(0.25, 0.25, 0.25, 0.5);
					else
						temporalResult = half4(0.375, 0.375, 0.375, 0.5);
				}
				else if(_Time[1] % 0.4 >= 0.1)
				{
					if(output.pos.y % 10 >= 7.5)
						temporalResult = half4(0.25, 0.25, 0.25, 0.5);
					else if(output.pos.y % 4 >= 5.0)
						temporalResult = half4(0.375, 0.375, 0.375, 0.5);		
					else if(output.pos.y % 4 >= 2.5)
						temporalResult = half4(0.5, 0.5, 0.5, 0.5);
					else
						temporalResult = half4(0.125, 0.125, 0.125, 0.5);
				}
				else if(_Time[1] % 0.4 >= 0.05)
				{
					if(output.pos.y % 10 >= 7.5)
						temporalResult = half4(0.375, 0.375, 0.375, 0.5);		
					else if(output.pos.y % 4 >= 5.0)
						temporalResult = half4(0.25, 0.25, 0.25, 0.5);
					else if(output.pos.y % 4 >= 2.5)
						temporalResult = half4(0.125, 0.125, 0.125, 0.5);
					else
						temporalResult = half4(0.5, 0.5, 0.5, 0.5);
				}
				else
				{
					if(output.pos.y % 10 >= 7.5)
						temporalResult = half4(0.125, 0.125, 0.125, 0.5);
					else if(output.pos.y % 4 >= 5.0)
						temporalResult = half4(0.5, 0.5, 0.5, 0.5);
					else if(output.pos.y % 4 >= 2.5)
						temporalResult = half4(0.375, 0.375, 0.375, 0.5);		
					else
						temporalResult = half4(0.25, 0.25, 0.25, 0.5);
				}

				return temporalResult;

			}
			ENDCG
		}
	}
}
