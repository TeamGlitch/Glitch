Shader "Hidden/GlitchCameraOffsetShader" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_DispTex ("Base (RGB)", 2D) = "bump" {}
	_Intensity ("Glitch Intensity", Range(0.1, 1.0)) = 1
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		#include "UnityCG.cginc"
		
		uniform sampler2D _MainTex;
		uniform sampler2D _DispTex;
		uniform sampler2D _Corr;
		float _Intensity;
		
		struct v2f {
			float4 pos : POSITION;
			float2 uv : TEXCOORD0;
		};
		
		v2f vert( appdata_img v )
		{
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord.xy;

			return o;
		}
		
		half4 frag (v2f i) : COLOR
		{
			//Applies movement and correction textures to
			//the x of the uv and returns the result
			half4 normal = tex2D (_DispTex, i.uv.xy);
			half4 correction = tex2D (_Corr, i.uv.xy);
			i.uv.x += (normal.x - correction.x) * _Intensity;
			half4 final = tex2D(_MainTex,  i.uv.xy);

			return final;
		}
		ENDCG
	}
}

Fallback off

}