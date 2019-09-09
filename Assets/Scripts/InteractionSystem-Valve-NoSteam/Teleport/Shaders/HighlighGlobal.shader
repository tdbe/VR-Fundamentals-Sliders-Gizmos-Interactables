﻿//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Used for the teleport markers
//
//=============================================================================
// UNITY_SHADER_NO_UPGRADE
Shader "Valve/VR/HighlightPlanar"
{
	Properties
	{
		_TintColor( "Tint Color", Color ) = ( 1, 1, 1, 1 )
		_SeeThru( "SeeThru", Range( 0.0, 1.0 ) ) = 0.25
		_Darken( "Darken", Range( 0.0, 1.0 ) ) = 0.0
		_Scl("Mapping Scale", Float) = 2.0
		_MainTex( "MainTex", 2D ) = "white" {}
	}

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	CGINCLUDE
		
		// Pragmas --------------------------------------------------------------------------------------------------------------------------------------------------
		#pragma target 5.0
		#pragma only_renderers d3d11 vulkan glcore
		#pragma exclude_renderers gles

		// Includes -------------------------------------------------------------------------------------------------------------------------------------------------
		#include "UnityCG.cginc"

		// Structs --------------------------------------------------------------------------------------------------------------------------------------------------
		struct VertexInput
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			fixed4 color : COLOR;
		};
		
		struct VertexOutput
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
			fixed4 color : COLOR;
			float3 pos : TEXCOORD1;
		};

		// Globals --------------------------------------------------------------------------------------------------------------------------------------------------
		sampler2D _MainTex;
		float4 _MainTex_ST;
		float4 _TintColor;
		float _SeeThru;
		float _Darken;
		float _Scl;
				
		// MainVs ---------------------------------------------------------------------------------------------------------------------------------------------------
		VertexOutput MainVS( VertexInput i )
		{
			VertexOutput o;
#if UNITY_VERSION >= 540
			o.vertex = UnityObjectToClipPos(i.vertex);
#else
			o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
#endif
			o.uv = TRANSFORM_TEX( i.uv, _MainTex );
			o.pos = mul(unity_ObjectToWorld, i.vertex);
			o.color = i.color;
			
			return o;
		}
		
		// MainPs ---------------------------------------------------------------------------------------------------------------------------------------------------
		float4 MainPS( VertexOutput i ) : SV_Target
		{
			//float4 vTexel = tex2D( _MainTex, (i.pos.xz +i.pos.y) / _Scl).rgba;
			float4 vTexel1 = tex2D( _MainTex, (i.pos.xz) / _Scl).rgba;
			float4 vTexel2 = tex2D( _MainTex, (i.pos.xy) / _Scl).rgba;
			float4 vTexel3 = tex2D( _MainTex, (i.pos.yz) / _Scl).rgba;
			float4 vTexel = vTexel1;
			float t = frac(abs(i.pos.y));
			if( t > 0.14 & t < 0.86 )
				if(vTexel2.a > vTexel1.a && vTexel2.a > vTexel3.a){
					vTexel = vTexel2;
				}
				else if(vTexel3.a > vTexel1.a && vTexel3.a > vTexel2.a){
					vTexel = vTexel3;
				}

				if (length(i.uv)==0) {
					vTexel = float4(1, 1, 1, 1);
				}

			float4 vColor = vTexel.rgba * _TintColor.rgba * i.color.rgba;
			vColor.rgba = saturate( 2.0 * vColor.rgba );
			float flAlpha = vColor.a;

			vColor.rgb *= vColor.a;
			vColor.a = lerp( 0.0, _Darken, flAlpha );

			return vColor.rgba;
		}

		// MainPs ---------------------------------------------------------------------------------------------------------------------------------------------------
		float4 SeeThruPS( VertexOutput i ) : SV_Target
		{
			//float4 vTexel = tex2D( _MainTex, (i.pos.xz +i.pos.y) /_Scl).rgba;
			float4 vTexel1 = tex2D( _MainTex, (i.pos.xz) / _Scl).rgba;
			float4 vTexel2 = tex2D( _MainTex, (i.pos.xy) / _Scl).rgba;
			float4 vTexel3 = tex2D( _MainTex, (i.pos.yz) / _Scl).rgba;
			float4 vTexel = vTexel1;
			float t = frac(abs(i.pos.y));
			if( t > 0.14 & t < 0.86 )
				if(vTexel2.a > vTexel1.a && vTexel2.a > vTexel3.a){
					vTexel = vTexel2;
				}
				else if(vTexel3.a > vTexel1.a && vTexel3.a > vTexel2.a){
					vTexel = vTexel3;
				}

				if (length(i.uv)==0) {
					vTexel = float4(1, 1, 1, 1);
				}

			float4 vColor = vTexel.rgba * _TintColor.rgba * i.color.rgba * _SeeThru;
			vColor.rgba = saturate( 2.0 * vColor.rgba );
			float flAlpha = vColor.a;

			vColor.rgb *= vColor.a;
			vColor.a = lerp( 0.0, _Darken, flAlpha * _SeeThru );

			return vColor.rgba;
		}

	ENDCG

	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		// Behind Geometry ---------------------------------------------------------------------------------------------------------------------------------------------------
		Pass
		{
			// Render State ---------------------------------------------------------------------------------------------------------------------------------------------
			Blend One OneMinusSrcAlpha
			Cull Off
			ZWrite Off
			ZTest Greater

			CGPROGRAM
				#pragma vertex MainVS
				#pragma fragment SeeThruPS
			ENDCG
		}

		Pass
		{
			// Render State ---------------------------------------------------------------------------------------------------------------------------------------------
			Blend One OneMinusSrcAlpha
			Cull Off
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
				#pragma vertex MainVS
				#pragma fragment MainPS
			ENDCG
		}
	}
}
