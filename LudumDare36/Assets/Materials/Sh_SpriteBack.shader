Shader "Unlit/Sh_Sprite_Back"
{
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
SubShader {
	Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
	LOD 100

	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	Lighting Off

	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				float4 screen : TEXCOORD1;
				float4 color : TEXCOORD2;
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Cutoff;

			v2f vert (appdata_t v)
			{
				v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.screen = ComputeScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				float Blend = saturate((i.screen.x-0.4f)*5.0f);

				float2 uv = i.texcoord;
				fixed4 col2 = tex2Dlod(_MainTex, float4(uv,0,0));

				float2 CenterDir = (i.screen - 0.5f) * float2(2.2f, 2.1f);

				//float Assom = 1.0f - pow(saturate(dot(CenterDir, CenterDir)-0.8f),1.0f);
				float Assom = 1.0f - saturate(CenterDir.x*CenterDir.x - 0.8f);
				Assom *= 1.0f - saturate(CenterDir.y*CenterDir.y - 0.8f);

				float avgcol = dot(col2.rgb, 0.333);
				avgcol = saturate((avgcol - 0.01f)*1000.0f); // 0.7f

				col2.rgb = lerp(col2.rgb, 0.8, 0.5);

				float3 retro = lerp(float3(0.16, 0.25, 0), float3(0.6, 0.77, 0)*0.5f+0.5f, avgcol);
				retro *= Assom;
				col2.rgb = lerp(retro, col2.rgb, Blend);
							
								
				//clip(col.a - _Cutoff);
				UNITY_APPLY_FOG(i.fogCoord, col2);

				//col.xyz = col2.xyz;

				return col2;
			}
		ENDCG
	}
}

}
