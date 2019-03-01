﻿Shader "Unlit/Displacer"
{
    Properties
    {
        _Radius("Radius", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                o.localPos = v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = sqrt(i.localPos.x * i.localPos.x + i.localPos.y * i.localPos.y);
                float factor = saturate(1.0f - dist / _Radius);

                float2 direction = normalize(i.localPos.xy);
                float2 packedDirection = (direction + float2(1.0f, 1.0f)) / 2.0f;

                fixed4 col = fixed4(direction.x, direction.y, factor, 1.0);
                return col;
            }
            ENDCG
        }
    }
}
