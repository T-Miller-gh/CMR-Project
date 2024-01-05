Shader "Hidden/Polyphoria/OpacityBaker"
{
    Properties
    {
        _MapA ("Texture", 2D) = "white" {}
        _MapB ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        ZWrite Off
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
                float2 uvA : TEXCOORD0;
                float2 uvB : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uvA : TEXCOORD0;
                float2 uvB : TEXCOORD1;
            };

            sampler2D _MapA;
            float4 _MapA_ST;
            sampler2D _MapB;
            float4 _MapB_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uvA = TRANSFORM_TEX(v.uvA, _MapA);
                o.uvB = TRANSFORM_TEX(v.uvB, _MapB);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 colA = tex2D(_MapA, i.uvA);
                fixed4 colB = tex2D(_MapB, i.uvB);
                return colA * colB;
            }
            ENDCG
        }
    }
}
