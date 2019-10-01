Shader "Unlit/GazeProgressIndicatorShader"
{
	Properties
	{
		_Color ("Main Color", Color) = (1.000000,1.000000,1.000000,1.000000)
		_MainTex ("Texture", 2D) = "white" {}
		_Cutoff ("Alpha Cutoff", Range(0.0,1.00000)) = 0.000000
		_GradientCorrection("Gradient Percent Correction", Range(0.000000,2.000000)) = 0.322
		_BlackOutline ("Black Outline", float) = 10

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
			float _GradientCorrection;
			float _BlackOutline;
			
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
			
			float4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float4 col = _Color;
				float4 texCol = tex2D(_MainTex, i.uv);
				//col.a = texCol.a * _Color.a * texCol.r;
				col.a = pow(texCol.r,_GradientCorrection);
				//float cutoff = (1-_Cutoff) + 0.2;// 0.26;
				float cutoff = (1-_Cutoff);
				
				if(col.a > cutoff){//* (1-texCol.g*_GradientCorrection)){
					//col.a = lerp(col.a,0,saturate((_Cutoff-col.a)*_GradientCorrection));
					//col.a = (col.a*saturate(1-(_Cutoff-col.a)*_GradientCorrection));
					//col.a =min(col.a,(1-(_Cutoff-col.a) * _GradientCorrection));
					
					//col.a = lerp(0,texCol.r,saturate(1-(cutoff-col.a) * _GradientCorrection));
					col.a = texCol.g;//lerp(0,texCol.r,saturate(1-(cutoff-col.a) * _GradientCorrection));
					//col.a = lerp(0,texCol.r,saturate(1-(cutoff) * _GradientCorrection));
					
				}
				else
					col.a = 0;

				float border = texCol.g + texCol.b -1;
				col.rgb -= (texCol.g + texCol.b -1)*_BlackOutline;

				col.a -= texCol.b;//-border;
				col.a *=_Color.a;
				
				//col.a = texCol.a;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
