Shader "Unlit/Sh_Sprite_Distortion"
{
Properties{
	_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_DistortTex("Base (RGB)", 2D) = "white" {}
}
	SubShader{
	Tags{ "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
	LOD 100

	GrabPass
	{

	}

	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	Lighting Off

	Pass{
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
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		sampler2D _DistortTex;
		float4 _DistortTex_ST;
		sampler2D _GrabTexture : register(s0);

		v2f vert(appdata_t v)
		{
			v2f o;

			o.vertex = UnityObjectToClipPos(v.vertex);
			o.texcoord = v.texcoord;
			o.screen = 0.0f;
			o.screen = ComputeGrabScreenPos(o.vertex);
			o.color = v.color;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			float4 nois = tex2D(_DistortTex, i.screen.xy*float2(2.0f,1.0f)*1.0f + _Time.x*30.0f);

			float2 uv = i.texcoord.xy;

			uv += (nois.xy-0.5f)*0.3f;

			float2 Dir = uv - 0.5f;
			float Dist = sqrt(dot(Dir, Dir));

			float Fact = max(0.0f, sin(Dist*30.0f - _Time.x*300.0f) - 0.1)*0.5f;
			Fact *= saturate(1.0f-Dist*2.5f) * i.color.a;

			float4 refrColor = tex2D(_GrabTexture, i.screen.xy + Dir * Fact);

			refrColor.rgb = lerp(refrColor.rgb, i.color.rgb, Fact);

			//col.xyz = col.xyz;
			//refrColor = Dist;// float4(Dir, 0, 0);
			//refrColor.xy = Dir.xy;

			clip(0.25f-Dist);
			
			return float4(refrColor.rgb,1.0f);
		}
			ENDCG
		}
	}
}
