Shader "Sprites/SpriteMul"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		Brightness("Brightness", Float) = 1.0
		Contrast("Contrast", Color) = (0,0,0,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Overlay" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		GrabPass
		{

		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 screen : TEXCOORD1;
			};
			
			fixed4 _Color;
			float Brightness;
			fixed4 Contrast;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif
				OUT.screen = 0.0f;
				//OUT.screen.xy = OUT.vertex.xy / OUT.vertex.w *float2(0.5f, -0.5f) + 0.5f;
				OUT.screen = ComputeGrabScreenPos(OUT.vertex);

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			sampler2D _GrabTexture : register(s0);

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				c.rgb *= Brightness;

				float4 refrColor = tex2D(_GrabTexture, IN.screen.xy);
				refrColor.rgb = max(0.0f,(refrColor.rgb-Contrast.rgb) *c.rgb);

				return float4(refrColor.rgb, c.a);
				//return float4(1,1,1, c.a);
			}
		ENDCG
		}
	}
}
