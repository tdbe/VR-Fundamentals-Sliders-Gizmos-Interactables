Shader "Custom/SteamOutlines_TransparentUnlitColor"
{
    Properties
    {
		_TintColor ("Main Color", Color) = (1.000000,1.000000,1.000000,1.000000)
		_TintColor2 ("Backface Color", Color) = (1.000000,1.000000,1.000000,1.000000)
        _MainTex ("Texture", 2D) = "white" {}
        _Darken( "Darken", Range( 0.0, 1.0 ) ) = 1
        _SeeThru( "SeeThru", Range( 0.0, 1.0 ) ) = 0.25
        _Scl("Mapping Scale", Float) = 2.0
        _RimContrast("Rim Contrast", Float) = 1.35
        _MoveSpeed_("_MoveSpeed", Float) = 1
    }

    CGINCLUDE

        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            //UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;
			float3 pos : TEXCOORD1;
            float3 color : COLOR;

        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _TintColor;
        float4 _TintColor2;
        float _Scl;
        float _SeeThru;
        float _Darken;
        float _RimContrast;
        float _MoveSpeed_;


        float4 MainPSValve( v2f i )
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

			float4 vColor = vTexel.rgba * _TintColor.rgba;
			vColor.rgba = saturate( 2.0 * vColor.rgba );
			float flAlpha = vColor.a;

			vColor.rgb *= vColor.a;
			vColor.a = lerp( 0.0, _Darken, flAlpha );

			return vColor.rgba;
		}


        float4 SeeThruPSValve( v2f i ) 
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
        
			float4 vColor = vTexel.rgba * _TintColor.rgba * _SeeThru;
			vColor.rgba = saturate( 2.0 * vColor.rgba );
			float flAlpha = vColor.a;

			vColor.rgb *= vColor.a;
			vColor.a = lerp( 0.0, _Darken, flAlpha * _SeeThru );

			return vColor.rgba;
		}
    ENDCG

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        

        Pass
        {
            //Blend DstColor OneMinusSrcAlpha
            //Blend SrcAlpha OneMinusSrcAlpha
            Blend One One
            //Blend OneMinusDstColor One // Soft Additive
            //Blend DstColor SrcAlpha
            ZWrite Off
            ZTest Greater
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            
            



            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex)-_Time.yz*_MoveSpeed_;
                o.pos = mul(unity_ObjectToWorld, v.vertex);

                 float3 worldScale = float3(
                length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
                length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
                length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
                );
                worldScale = saturate(worldScale);
                float camDist = distance(_WorldSpaceCameraPos, o.pos)*_RimContrast;
                v.vertex.xyz /= worldScale / camDist;

                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                float dotProduct = 1 - dot(v.normal, viewDir);
                float rimWidth = _Scl;
                o.color = smoothstep(1 - rimWidth, 1.0, dotProduct);
                o.color *= _TintColor2;

                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col = _SeeThru*tex.a;
                col.rgb*=i.color.rgb;
                //fixed4 col = SeeThruPSValve(i);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            
            ENDCG
        }

        Pass
        {
            //Blend DstColor OneMinusSrcAlpha
            //Blend SrcAlpha OneMinusSrcAlpha
            Blend One One
            //Blend OneMinusDstColor One // Soft Additive
            //Blend DstColor SrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            
            


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex)-_Time.yz*_MoveSpeed_;
                o.pos = mul(unity_ObjectToWorld, v.vertex);

                float3 worldScale = float3(
                length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
                length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
                length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
                );
                worldScale = saturate(worldScale);
                float camDist = distance(_WorldSpaceCameraPos, o.pos)*_RimContrast;
                v.vertex.xyz /= worldScale / camDist;
                
                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                float dotProduct = 1 - dot(v.normal, viewDir);
                float rimWidth = _Scl;
                o.color = smoothstep(1 - rimWidth, 1.0, dotProduct);
                o.color *= _TintColor;

                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col = _Darken*tex.a;
                col.rgb*=i.color.rgb;
                //fixed4 col = MainPSValve(i);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    
    }
}
