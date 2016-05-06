Shader "Sprites/Glitch"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		
		_Color ("Tint", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			// Upgrade NOTE: excluded shader from Xbox360; has structs without semantics (struct v2f members pos)
			#pragma exclude_renderers xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif
				return OUT;
			}

			sampler2D _MainTex;
			uniform sampler2D _DispTex;
			uniform sampler2D _Corr;
			float _Intensity;
			
			fixed4 frag(v2f i) : COLOR
			{
				//Applies movement and correction textures to
				//the x of the uv and returns the result
				half4 normal = tex2D (_DispTex, i.texcoord.xy);
				half4 correction = tex2D (_Corr, i.texcoord.xy);
				i.texcoord.x += (normal.x - correction.x) * _Intensity;
				
				fixed4 final = tex2D(_MainTex, i.texcoord) * i.color;
				final.a *= i.color.a;
				final.rgb *= final.a;
				return final;
			}
		ENDCG
		}
	}
}