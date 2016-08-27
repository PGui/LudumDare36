Shader "Unlit/Sh_Sprite_Pixelize"
{
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_NoiseTex("Noise (RGBA)", 2D) = "white" {}
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
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				float4 screen : TEXCOORD1;
				float4 color : TEXCOORD02;
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;

			fixed4 _Color;

			v2f vert (appdata_t v)
			{
				v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.screen = ComputeScreenPos(o.vertex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				float4 nois = tex2D(_NoiseTex, i.screen.xy*float2(2.0f,1.0f) - float2(0.0f,_Time.x*1.0f));

				float Blend = saturate((i.screen.x-0.6f + nois.x*0.4)*5.0f);

				float2 uv = i.texcoord;
				float ppix = 96.0f;
				float2 uv2 = floor(uv * ppix + 0.5f) / ppix;
				
				float ed = 1.0f-saturate(dot(saturate((abs(uv - uv2)*ppix-(1-Blend)*0.5f)*1000.0f),1.0f));
				float ed2 = saturate(dot(saturate((abs(uv - uv2)*ppix - (1 - 0.2f)*0.5f)*1000.0f), 1.0f));

				float av = 0.5f;
				fixed4 col = tex2Dlod(_MainTex, float4(uv,0,0));
				fixed4 col2 = tex2Dlod(_MainTex, float4(uv2, 0, 0));
				col2.rgb *= tex2Dlod(_MainTex, float4(uv2 + float2(av / ppix,0), 0, 0)).rgb;
				col2.rgb *= tex2Dlod(_MainTex, float4(uv2 + float2(0,av / ppix), 0, 0)).rgb;
				//col2.rgb /= 3;
				col2.a = saturate((col2.a - 0.01f)*1000.0f);

				float2 CenterDir = (i.screen - 0.5f) * float2(2.2f, 2.1f);

				//float Assom = 1.0f - pow(saturate(dot(CenterDir, CenterDir)-0.8f),1.0f);
				float Assom = 1.0f - saturate(CenterDir.x*CenterDir.x - 0.8f);
				Assom *= 1.0f - saturate(CenterDir.y*CenterDir.y - 0.8f);

				float avgcol = dot(col2.rgb, 0.333);
				avgcol = saturate((avgcol - 0.01f)*1000.0f); // 0.7f
				//avgcol *= (0.8 + ed2*0.2);
				avgcol = max(avgcol, ed2*0.3);

				float3 retro = lerp(float3(0.16, 0.25, 0), float3(0.6, 0.77, 0)*0.5f + 0.5f, avgcol);
				retro *= Assom;
				
				col2.rgb = retro*i.color.rgb;

				col.rgb = lerp(col.rgb, 0.8, 0.5);
								
				col = lerp(col, col2, ed);
				//col = ed;

								
				//clip(col.a - _Cutoff);
				UNITY_APPLY_FOG(i.fogCoord, col);

				//col.xyz = col2.xyz;
				//col.xyz = nois.xyz;

				return col;
			}
		ENDCG
	}
}

}
