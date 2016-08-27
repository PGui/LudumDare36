Shader "Unlit/Sh_Sprite_Pixelize"
{
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 100

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
				float ppix = 96.0f;
				float2 uv2 = floor(uv * ppix + 0.5f) / ppix;
				
				float ed = 1.0f-saturate(dot(saturate((abs(uv - uv2)*ppix-(1-Blend)*0.5f)*1000.0f),1.0f));
				float ed2 = 1.0f - saturate(dot(saturate((abs(uv - uv2)*ppix - (1 - 0.2f)*0.5f)*1000.0f), 1.0f));

				float av = 0.5f;
				fixed4 col = tex2Dlod(_MainTex, float4(uv,0,0));
				fixed4 col2 = tex2Dlod(_MainTex, float4(uv2, 0, 0));
				col2 += tex2Dlod(_MainTex, float4(uv2 + float2(av / ppix,0), 0, 0));
				col2 += tex2Dlod(_MainTex, float4(uv2 + float2(0,av / ppix), 0, 0));
				col2 /= 3;
				
				col = lerp(col, col2, ed);
				//col = ed;

				float avgcol = dot(col, 0.333);
				avgcol = saturate((avgcol - 0.7f)*1000.0f);

				float2 CenterDir = (i.screen - 0.5f) * float2(2.2f,2.0f);
				float Assom = 1.0f - pow(saturate(dot(CenterDir, CenterDir)-0.8f),1.0f);

				float3 retro = lerp(float3(0.16,0.25,0), float3(0.6,0.77,0), avgcol);
				retro *= Assom;
				//retro *= ed2;
				col.rgb = lerp(retro,col.rgb, Blend);

				clip(col.a - _Cutoff);
				UNITY_APPLY_FOG(i.fogCoord, col);

				//col.xyz = col2.xyz;

				return col;
			}
		ENDCG
	}
}

}
