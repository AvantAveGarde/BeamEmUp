// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ToonSmear"
{
    Properties 
    {

		[MaterialToggle(_TEX_ON)] _DetailTex ("Enable Detail texture", Float) = 0 	//1
		_MainTex ("Detail", 2D) = "white" {}        								//2
		_ToonShade ("Shade", 2D) = "white" {}  										//3
		[MaterialToggle(_COLOR_ON)] _TintColor ("Enable Color Tint", Float) = 0 	//4
		_Color ("Base Color", Color) = (1,1,1,1)									//5	
		[MaterialToggle(_VCOLOR_ON)] _VertexColor ("Enable Vertex Color", Float) = 0//6        
		_Brightness ("Brightness 1 = neutral", Float) = 1.0							//7	
		_OutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1.0)					//10
		_Outline ("Outline width", Float) = 0.01									//11

		_Position("Position", Vector) = (0, 0, 0, 0)
		_PrevPosition("Prev Position", Vector) = (0, 0, 0, 0)
		_NoiseScale("Noise Scale", Float) = 15
		_NoiseHeight("Noise Height", Float) = 1.3

    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		LOD 250 
        Lighting Off
        Fog { Mode Off }
        
        UsePass "TSF/Base1/BASE"
        	
        Pass
        {
            Cull Front
            ZWrite On
            CGPROGRAM
			#include "UnityCG.cginc"
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
            #pragma vertex vert
 			#pragma fragment frag
			
            struct appdata_t 
            {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f 
			{
				float4 pos : SV_POSITION;
			};

            fixed _Outline;

			float hash(float n)
			{
				return frac(sin(n)*43758.5453);
			}
	
			float noise(float3 x)
			{
				// The noise function returns a value in the range -1.0f -> 1.0f
	
				float3 p = floor(x);
				float3 f = frac(x);
	
				f = f*f*(3.0 - 2.0*f);
				float n = p.x + p.y*57.0 + 113.0*p.z;
	
				return lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
					lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y),
					lerp(lerp(hash(n + 113.0), hash(n + 114.0), f.x),
						lerp(hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
			}

			fixed4 _PrevPosition;
			fixed4 _Position;
	
			half _NoiseScale;
			half _NoiseHeight;
            
            v2f vert (appdata_t v) 
            {
                v2f o;
				
				fixed4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				fixed3 worldOffset = _Position.xyz - _PrevPosition.xyz;
				fixed3 localOffset = worldPos.xyz - _Position.xyz;
				float dirDot = dot(normalize(worldOffset), normalize(localOffset));
				fixed3 unitVec = fixed3(1, 1, 1) * _NoiseHeight;
				worldOffset = clamp(worldOffset, unitVec * -1, unitVec);
				worldOffset *= -clamp(dirDot, -1, 0) * lerp(1, 0, step(length(worldOffset), 0));
				fixed3 smearOffset = -worldOffset.xyz * lerp(1, noise(worldPos * _NoiseScale), step(0, _NoiseScale));
				worldPos.xyz += smearOffset;
				v.vertex = mul(unity_WorldToObject, worldPos);

			    o.pos = v.vertex;
			    o.pos.xyz += normalize(v.normal.xyz) *_Outline*0.01;
			    o.pos = UnityObjectToClipPos(o.pos);
			    return o;
            }
            
            fixed4 _OutlineColor;
            
            fixed4 frag(v2f i) :COLOR 
			{
		    	return _OutlineColor;
			}
            
            ENDCG
        }
    }
Fallback "Legacy Shaders/Diffuse"
}