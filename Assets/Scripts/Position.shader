﻿Shader "Custom/Position" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		cull off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert nolightmap addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _PositionBuffer;
		float2 _BufferOffset;

		struct Input {
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void vert(inout appdata_full v)
		{
			float4 uv = float4(v.texcoord1.xy + _BufferOffset, 0, 0);
			float4 p = tex2Dlod(_PositionBuffer, uv);
			//v.vertex.xyz *= 1.2;
			v.vertex.xyz += p.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
