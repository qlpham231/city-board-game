Shader "Custom/SmogOverlay"
{
    Properties
    {
        _FogColor("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogIntensity("Fog Intensity", Range(0, 1)) = 0.5
        _FogHeight("Fog Height Falloff", Float) = 20.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            fixed4 _FogColor;
            float _FogIntensity;
            float _FogHeight;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Fog more intense near ground
                //float heightFactor = saturate(1 - (i.worldPos.y / _FogHeight));
                float heightFactor = saturate(i.worldPos.y / _FogHeight); // top-down smog
                float finalAlpha = heightFactor * _FogIntensity;

                return fixed4(_FogColor.rgb, finalAlpha);
                //return fixed4(1, 0, 0, 1); // bright red
            }
            ENDCG
        }
    }
}
