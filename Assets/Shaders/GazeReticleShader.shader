Shader "Unlit/GazeReticleSader"
{
	Properties
	{
		_Color ("Main Color", Color) = (1.000000,1.000000,1.000000,1.000000)
		_MainTex ("Texture", 2D) = "white" {}
		_Cutoff ("Alpha Cutoff", Range(-0.2600000,1.00000)) = 0.000000
		_SmoothCutoffFactor("Smooth Cutoff Factor", Range(0.000000,10.000000)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent+1001"}
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Always
		ZWrite Off
		LOD 100
		Cull Off
		

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile _ PIXELSNAP_ON​
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Cutoff;
			float _SmoothCutoffFactor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				#ifdef PIXELSNAP_ON
					o.vertex = UnityPixelSnap(o.vertex);
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = _Color * tex2D(_MainTex, i.uv);
				float cutoff = _Cutoff + 0.26;
				if(col.a < cutoff){
					//col.a = lerp(col.a,0,saturate((_Cutoff-col.a)*_SmoothCutoffFactor));
					//col.a = (col.a*saturate(1-(_Cutoff-col.a)*_SmoothCutoffFactor));
					//col.a = min(col.a,(1-(_Cutoff-col.a) * _SmoothCutoffFactor));
					col.a = lerp(0,1,saturate(1-(cutoff-col.a) * _SmoothCutoffFactor));
				}
				else
					col.a = _Color.a;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
