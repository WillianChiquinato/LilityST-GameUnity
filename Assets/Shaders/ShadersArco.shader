Shader "Custom/OverlayIgnoreLayers"
{
    Properties
    {
        _Color("Color", Color) = (0.8,0.8,0.8,0.5)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
                Pass Keep
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGBA
            SetTexture [_MainTex] { combine primary }
        }
    }
}
