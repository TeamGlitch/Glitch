﻿Shader "DM/Ripple Shader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("Scale", Range(0.1,500.0)) = 3.0
        _Speed ("Speed", Range(-50,50.0)) = 1.0
		_Position ("Position", Range(-1,1)) = 0.5
    }
    SubShader {
        Tags { "Queue"="Transparent+1" "RenderType"="Opaque" }

        LOD 200
        Cull Off
        CGPROGRAM
        #pragma surface surf Lambert
        #include "UnityCG.cginc"
        
        half4 _Color;
        half _Scale;
        half _Speed;
		half _Position;
        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            half2 uv = (IN.uv_MainTex - _Position) * _Scale;
            half r = sqrt (uv.x*uv.x + uv.y*uv.y);
            half z = sin (r+_Time[1]*_Speed) / r;
            o.Albedo = _Color.rgb * tex2D (_MainTex, IN.uv_MainTex+z).rgb;
            o.Alpha = _Color.a;
            //o.Normal = float3(z, z, z);
        }
        ENDCG
    } 
    FallBack "Diffuse"
}