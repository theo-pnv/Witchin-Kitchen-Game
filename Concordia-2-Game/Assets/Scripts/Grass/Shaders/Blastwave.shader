﻿Shader "Unlit/Displacer"
{
    Properties
    {
        _MinRingRadius("Min ring radius", Float) = 0.8
        _MaxRingRadius("Max ring radius", Float) = 1.0
        _Intensity("Intensity", Float) = 1.0
    }
        SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

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

            float _MinRingRadius;
            float _MaxRingRadius;
            float _Intensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.localPos = v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = sqrt(i.localPos.x * i.localPos.x + i.localPos.y * i.localPos.y);
                float x = saturate((dist - _MinRingRadius) / (_MaxRingRadius - _MinRingRadius)) - 0.5f;
                float factor = saturate(-4.0f * x * x + 1.0f) * _Intensity;

                float2 direction = normalize(i.localPos.xy);
                float2 packedDirection = direction + float2(0.5f, 0.5f);

                fixed4 col = fixed4(packedDirection.x, packedDirection.y, factor, factor);
                return col;
            }
            ENDCG
        }
    }
}
