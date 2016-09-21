Shader "Unlit/explosiomn"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
           
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 worldPosition : TEXCOORD1;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
           
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                o.worldPosition = mul(_Object2World, v.vertex).xyz;
                return o;
            }
 
            [maxvertexcount(3)]
            void geom(triangle v2f input[3], inout TriangleStream<v2f> OutputStream)
            {

				float time = 5.0;
				time -= _Time[2];
				if(time < 0)
					time = 0;

                v2f test = (v2f)0;
				float3 normal = (input[0].normal + input[1].normal + input[2].normal)/3.0;
				float4 pos = float4(time*normal.xyz, 1.0);

				float3x3 rot = float3x3(float3(cos(time),sin(time),0),float3(-sin(time),cos(time),0),float3(0,0,1));

				float3 pC = (input[0].vertex + input[1].vertex + input[2].vertex)/3.0;
                for(int i = 0; i < 3; i++)
                {
					float3 p1 = input[i].vertex.xyz - pC;
					float3 pR = mul(rot, p1);
					p1 = pR+pC;

                    test.normal = normal;
                    test.vertex = mul(UNITY_MATRIX_MVP, float4(float4(p1,0.0)+pos));
                    test.uv = input[i].uv;
                    OutputStream.Append(test);
                }
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}