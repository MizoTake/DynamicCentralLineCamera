Shader "Unlit/CentralLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CenterX ("Center X", Range (0, 1)) = 0.5
        _CenterY ("Center Y", Range (0, 1)) = 0.4
        _Central ("Central", float) = 1
        _Line ("Line", float) = 1
        _CentralEdge("Central Edge", float) = 0.4
        _CentralLength("Central Length", float) = 0.84
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

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
            float _CenterX;
            float _CenterY;
            float _Central;
            float _Line;
            float _CentralEdge;
            float _CentralLength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            float2 dir(float2 p)
            {
                p = p % 289;
                float x = frac((34 * ((34 * p.x + 1) * p.x % 289 + p.y) + 1) * ((34 * p.x + 1) * p.x % 289 + p.y) % 289 / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }
            
            float gradientNoise(float2 p)
            {
                p *= 40;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(dir(ip), fp);
                float d01 = dot(dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                float2 polarCoordinates = float2(length(i.uv - float2(_CenterX, _CenterY)) * 2 * _Central, atan2(i.uv.x - _CenterX, i.uv.y - _CenterY) * 1.0/6.28 * _Line);
                float step = smoothstep(_CentralEdge, 0.86, (gradientNoise(polarCoordinates.y) + 0.5) + ((polarCoordinates.x - 0.1) * 0.9 / (_CentralLength - 0.1) * -1));
                if(step != 0) {
                    c.a = 0;
                } else {
                    c.rgb = step;
                }
                return c;
            }
            ENDCG
        }
    }
}
