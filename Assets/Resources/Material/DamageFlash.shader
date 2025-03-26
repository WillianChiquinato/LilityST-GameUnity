Shader "Unlit/DamageFlash"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _HitColor ("Hit Color", Color) = (0, 0, 1, 1)
        _HitIntensity ("Hit Intensity", Range(0, 1)) = 0.513
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha    
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _HitColor;
            float _HitIntensity;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Obtém a cor do sprite original
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = lerp(texColor, _HitColor, _HitIntensity);
                finalColor.a = texColor.a;

                return finalColor;
            }
            ENDCG
        }
    }
}