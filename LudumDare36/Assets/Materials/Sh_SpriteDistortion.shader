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
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			half2 texcoord : TEXCOORD0;
			float4 screen : TEXCOORD1;
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
			//o.screen = ComputeScreenPos(o.vertex);
			o.screen = 0.0f;
			o.screen.xy = o.vertex.xy/ o.vertex.w *float2(0.5f,-0.5f)+0.5f;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			float4 nois = tex2D(_DistortTex, i.screen.xy*float2(2.0f,1.0f)*5.0f);

			float2 Dir = i.texcoord.xy;
			float Dist = sqrt(dot(Dir, Dir));
			//Dir /= Dist;

			float4 refrColor = tex2D(_GrabTexture, i.screen.xy + Dir*0.0f + nois.xy*0.03f);

			//col.xyz = col.xyz;
			//refrColor = 0;
			//refrColor.xy = Dir.xy;

			return float4(refrColor.rgb,1.0f);
		}
			ENDCG
		}
	}
}
