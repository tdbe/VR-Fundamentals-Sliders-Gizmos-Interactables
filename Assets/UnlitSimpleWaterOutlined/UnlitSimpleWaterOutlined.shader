Shader "Custom/UnlitSimpleWaterOutlined"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_ColorFoam("Foam Color", Color) = (1,1,1,1)
		_MaskTex("Mask Texture", 2D) = "white" {}
		_MaskStrength("Mask Strength", float) = 2
		_Mask("Mask", Range(0,1)) = 0

		_MainTex("Main Texture", 2D) = "white" {}
		_TexRamp ("Texture Ramp", 2D) = "white" {}
		_InvFadeParemeter("Edge blend parameters", Vector) = (0.15 ,0.15, 0.5, 1.0)
		//_Test("_Test", Vector) = (1 ,1, 0, 0)
		//_VertexParems("_VertexParems", Vector) = (1 ,1, 1, 1.0)
		//_SunReflectionAmount("SunReflectionAmount", float) = 1

			
	}
	SubShader
	{
		//Tags{ "Queue" = "3000" "RenderType" = "Transparent" }
		Tags{ "ForceNoShadowCasting" = "True" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{

			LOD 100
			Lighting Off
			ZWrite Off //
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
			//Blend SrcColor OneMinusSrcColor
			Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog
			//#pragma PC_EDITOR
			#pragma multi_compile PC_EDITOR MOBILE_VR
			//#pragma shader_feature PC_EDITOR MOBILE_VR
			#include "UnityCG.cginc"




			uniform float4 _Color;
			uniform float4 _ColorFoam;

			sampler2D _MaskTex;
			uniform float4 _MaskTex_ST;
			float _MaskStrength;
			float _Mask;

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _MainTex_TexelSize;
			sampler2D _TexRamp;
			uniform float4 _TexRamp_ST;
			
			sampler2D _CameraDepthTexture;
			// edge & shore fading
			uniform float4 _InvFadeParemeter;
			//uniform float4 _VertexParems;

			//float4 _Test;

			uniform float3 _SunDir;
			uniform float3 _CamDir;
			uniform float3 _SpecPos;

	
			//uniform float _SunReflectionAmount;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 worldDirection : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPosition : TEXCOORD3;
				//float depth : DEPTH;
			};


			
			v2f vert (appdata v)
			{
				v2f o;

				o.uv = v.uv;//TRANSFORM_TEX(v.uv, _MainTex);
				
				//if (_MainTex_TexelSize.y < 0)
				//	o.uv.y = 1 - o.uv.y;

				// Subtract camera position from vertex position in world
				// to get a ray pointing from the camera to this vertex.
				o.worldDirection = mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				//o.worldDirection.x = o.worldDirection.x * _VertexParems.x + _VertexParems.z;
				//o.worldDirection.z = o.worldDirection.z * _VertexParems.y + _VertexParems.w;

				// Typical boilerplate.
				o.vertex = UnityObjectToClipPos(v.vertex);

				// Save the clip space position so we can use it later.
				// (There are more efficient ways to do this in SM 3.0+, 
				o.screenPosition = o.vertex;
				//o.screenPosition.xy = ((o.vertex.xy / o.vertex.w) + 1)/2;
				//o.screenPosition.y = 1 - o.screenPosition.y;
				//o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z *_ProjectionParams.w;

				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				// Compute projective scaling factor...
				float perspectiveDivide = 1.0f / i.screenPosition.w;

				// Scale our view ray to unit depth.
				float3 direction = i.worldDirection * perspectiveDivide;
				
				// Calculate our UV within the screen (for reading depth buffer)
				

				
				//uncomment this to use the editor, comment it to have it work in VR
				//screenUV.y = 1 - screenUV.y;
				
#if defined(PC_EDITOR)
				float2 screenUV = (i.screenPosition.xy * perspectiveDivide) * 0.5f + 0.5f;
				screenUV.y = 1 - screenUV.y;
#elif defined(MOBILE_VR)
				float2 screenUV = (i.screenPosition.xy * perspectiveDivide);// * 0.5f + 0.5f;
				//float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
				//screenUV = i.uv * perspectiveDivide * scaleOffset.xy+scaleOffset.zw;
				//float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
    			//screenUV = (screenUV - scaleOffset.zw) / scaleOffset.xy;
				//screenUV = UnityStereoScreenSpaceUVAdjust(i.screenPosition.xy, _MainTex_ST);
				
				if(unity_StereoEyeIndex==0){
					screenUV = screenUV*(float2(.25,.5))+float2(.25,.5);
				}
				else{
					screenUV = screenUV *(float2(.25,.5))+float2(.75,.5);
				}

#endif


				// Read depth, linearizing into worldspace units.    
				float depth = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, screenUV)));
				//float depth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, screenUV)));
				//depth = _InvFadeParemeter.z * pow(depth, _InvFadeParemeter.y);
				
				//float depth = LinearEyeDepth((tex2D(_CameraDepthTexture, i.screenPosition)));


				// Advance by depth along our view ray from the camera position.
				// This is the worldspace coordinate of the corresponding fragment
				// we retrieved from the depth buffer.
				float3 worldspace = direction * depth + _WorldSpaceCameraPos;


				// sample the texture
				float scale = 0.75;
				fixed4 col = tex2D(_MainTex, i.uv * _MainTex_ST.xy + float2(_SinTime.y, _MainTex_ST.z)*scale);
				fixed4 col2 = (tex2D(_MainTex, i.uv* _MainTex_ST.xy + float2(_MainTex_ST.z, _CosTime.y)*scale));//- _MainTex_ST.zw*.5
				//col.a = col.r;
				//col2.a = col2.r;
				col.r *= col2.r;
				//col.b *= col2.b;
				col.a = col.r;//(col.r+col.g+col.b)/3;

				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);

				/*
				float edgeBlendFactors = 1;// half4(1.0, 0.0, 0.0, 0.0);
				//half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
				//depth = LinearEyeDepth(depth);
				float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.screenPos);
				float depth = Linear01Depth(rawDepth);
				*/


				//float3 normal = float3(0, 1, 0)+ worldspace;// can be bumped
				//float3 lightRefl = reflect(-_SunDir, normal);
				//float distToCam = distance(_WorldSpaceCameraPos, worldspace);
				//float specular = pow(max(0.0, dot(lightRefl, _CamDir)), _InvFadeParemeter.z) * _InvFadeParemeter.y;// +_InvFadeParemeter.y * distToCam;
				//float specular = max(0.0, dot(lightRefl, _CamDir)) * _InvFadeParemeter.z;
				float specular = _InvFadeParemeter.z;



				//float edgeBlendFactors = saturate(_InvFadeParemeter * (depth - i.screenPosition.z));
				//float dist = distance(worldspace, i.worldPos) + (_InvFadeParemeter.z * pow(depth, _InvFadeParemeter.y))*_InvFadeParemeter.y;
				float3 specPos = _SpecPos;
				float dist = 
					//min(
						//specular*_TexRamp_ST.w,
						//distance(specPos, i.worldPos)*_TexRamp_ST.w,
						distance(worldspace, i.worldPos)
					//)
					;
				//float distanceBasedCompensation = _InvFadeParemeter.z * (1 - depth + _InvFadeParemeter.y)*_InvFadeParemeter.y;
				//float edgeBlendFactors = saturate(_InvFadeParemeter.x * dist + distanceBasedCompensation);

				
				float edgeBlendFactors = saturate(_InvFadeParemeter.x * dist );
				
			
				//col.a = _Color.a;
				float foamWobble = (col.a)*_InvFadeParemeter.w;
				//col = lerp(lerp(_ColorFoam * ((foamWobble* edgeBlendFactors) % (_InvFadeParemeter.z*(1 - dist))), _Color, col.a*_InvFadeParemeter.w), _Color, edgeBlendFactors);
				float texRamp = tex2D(_TexRamp, (edgeBlendFactors.xx * _TexRamp_ST.x + _TexRamp_ST.z)*foamWobble).x;
				col = lerp(lerp(_ColorFoam * texRamp, _Color, col.a*_InvFadeParemeter.w), _Color, edgeBlendFactors );
				
				texRamp = tex2D(_TexRamp, (specular * _TexRamp_ST.x + _TexRamp_ST.z)*foamWobble).x;
				float4 speccedCol = float4(col.x * texRamp, col.y * texRamp, col.z * texRamp, col.a);
				col = lerp(col, speccedCol, col.a * edgeBlendFactors - _InvFadeParemeter.y);

				//col = float4(i.screenPosition.x, i.screenPosition.y, i.screenPosition.z, 1);
				//col = float4(depth, depth, depth, 1);
				//col = float4(worldspace.x, worldspace.y, worldspace.z, 1);
				//col = float4(screenUV.x, screenUV.y, 0, 1);

				float mask = tex2D(_MaskTex, i.uv*_MaskTex_ST.xy+_MaskTex_ST.zw).a;
				col.a -= (1-saturate(((mask)* _MaskStrength)*_Mask));
				//col.rgb = depth;
				//col.a = 1;
				return col;
			}
			ENDCG
		}


	}
	//Fallback "Diffuse"
}
