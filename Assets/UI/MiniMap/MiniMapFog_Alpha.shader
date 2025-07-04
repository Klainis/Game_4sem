Shader "Custom/MiniMapFog_Alpha"
{
    Properties
    {
        _MainTex ("MiniMap Texture", 2D) = "white" {}
        _FogTex ("Fog Texture", 2D) = "white" {}
        _ExploredTex ("Explored Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0,0,0,1)
        _ExploredColor ("Explored Color", Color) = (0.3,0.3,0.3,0.5)
        _Transparency ("Map Transparency", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _FogTex;
            sampler2D _ExploredTex;
            fixed4 _FogColor;
            fixed4 _ExploredColor;
            float _Transparency;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 map = tex2D(_MainTex, i.uv);
                fixed fog = tex2D(_FogTex, i.uv).r;
                fixed explored = tex2D(_ExploredTex, i.uv).r;

                // Основная логика видимости
                if(fog > 0.1) 
                {
                    map.a = _Transparency; // Видимые зоны с прозрачностью
                    return map;
                }
                else if(explored > 0.1)
                {
                    fixed4 col = lerp(map * _ExploredColor, map, 0.5);
                    col.a = _Transparency * 0.7; // Исследованные зоны более прозрачны
                    return col;
                }
                else
                {
                    _FogColor.a = 1.0; // Неизведанные зоны полностью непрозрачны
                    return _FogColor;
                }
            }
            ENDCG
        }
    }
}