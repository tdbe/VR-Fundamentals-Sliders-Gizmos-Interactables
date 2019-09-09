// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/InvisibleObjectWithDepthWrite"
{
	Properties
	{
		_Color("Debug Color", Color) = (0, 0, 0, 0)
		//_MainTex("Base (RGB)", 2D) = "white" {}
		//_DepthLevel("Depth Level", Range(1, 3)) = 1
	}
		SubShader
	{
		Pass
	{
		Tags{ "ForceNoShadowCasting" = "True" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite On
		ZTest LEqual
		//ColorMask 0
		Blend One One//SrcAlpha OneMinusSrcAlpha
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
	
	float4 _Color;
	//uniform sampler2D _MainTex;
	//uniform sampler2D _CameraDepthTexture;
	//uniform fixed _DepthLevel;
	//uniform half4 _MainTex_TexelSize;

	struct input
	{
		float4 pos : POSITION;
		//half2 uv : TEXCOORD0;
	};

	struct output
	{
		float4 pos : SV_POSITION;
		//half2 uv : TEXCOORD0;
	};


	output vert(input i)
	{
		output o;
		o.pos = UnityObjectToClipPos(i.pos);
		/*
		o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, i.uv);
		// why do we need this? cause sometimes the image I get is flipped. see: http://docs.unity3d.com/Manual/SL-PlatformDifferences.html
#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1 - o.uv.y;
#endif
*/
		return o;
	}

	fixed4 frag(output o) : COLOR
	{
		//float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, o.uv));
		//depth = pow(Linear01Depth(depth), _DepthLevel);
		//return depth;
		return _Color;
	}

		ENDCG
	}
	}
}