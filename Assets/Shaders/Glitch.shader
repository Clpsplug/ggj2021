Shader "Custom/Glitch"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _ROffset ("Red Offset", Vector) = (0, 0, 0, 0)
        _GOffset ("Green Offset", Vector) = (0, 0, 0, 0)
        _BOffset ("Blue Offset", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
        LOD 100
		
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _ROffset;
            float2 _GOffset;
            float2 _BOffset;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.color = v.color;
                return o;
            }

            fixed detectEdge(float2 offset) {
                // step(T threshold, T x) when threshold >= x, 0.0; else, 1.0.
                return step(0, offset.x) * step(0, offset.y) * step(offset.x, 1) * step(offset.y, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float rate = 0.9;
                // TODO: Programatically calculate this value
                float2 center = float2(0.05, 0.05);
            
                // sample the texture
                float2 offset = i.uv / rate - center;
                float2 r_offset = (i.uv + _ROffset)/rate - center;
                float2 g_offset = (i.uv + _GOffset)/rate - center;
                float2 b_offset = (i.uv + _BOffset)/rate - center;
                
                
                fixed4 origin = tex2D(_MainTex, offset) * detectEdge(offset);
                fixed4 red = tex2D(_MainTex, r_offset) * detectEdge(r_offset);
                fixed4 green = tex2D(_MainTex, g_offset) * detectEdge(g_offset);
                fixed4 blue = tex2D(_MainTex, b_offset) * detectEdge(b_offset);
                return fixed4 (
                    clamp(origin.r / 2 + red.r / 2, 0, 1) * i.color.r,
                    clamp(origin.g / 2 + green.g / 2, 0, 1) * i.color.g,
                    clamp(origin.b / 2 + blue.b / 2, 0, 1) * i.color.b,
                    // If any of RGB is present, then apply the alpha for that one
                    clamp(origin.a + (red.a + green.a + blue.a) / 3, 0, 1) * i.color.a
                );
            }
            
            ENDCG
        }
    }
}
