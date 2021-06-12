Shader "Custom/StencilObject"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;

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

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Albedo comes from a texture tinted by color
                fixed4 c = tex2D(_MainTex, i.uv_MainTex) * _Color;
                // Metallic and smoothness come from slider variables
                c.Metallic = _Metallic;
                c.Smoothness = _Glossiness;
                return c;
            }

        ENDCG
        }
    }
}
