Shader "Custom/UIBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0, 20)) = 10
        _Tint ("Tint Color", Color) = (1,1,1,0.5)
    }

    Category
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]

        SubShader
        {
            // First pass - downsample
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                    float4 color : COLOR;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 texcoord : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    float4 color : COLOR;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _BlurAmount;
                float4 _ClipRect;
                float4 _Tint;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.worldPosition = v.vertex;
                    o.vertex = UnityObjectToClipPos(o.worldPosition);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.color = v.color * _Tint;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float4 color = i.color;
                    
                    // Simple box blur
                    float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y) * _BlurAmount;
                    float4 blur = float4(0, 0, 0, 0);
                    
                    // 3x3 kernel
                    for(int x = -1; x <= 1; x++) 
                    {
                        for(int y = -1; y <= 1; y++) 
                        {
                            float2 offset = float2(x, y) * texelSize;
                            blur += tex2D(_MainTex, i.texcoord + offset);
                        }
                    }
                    
                    blur /= 9.0;
                    
                    return blur * color;
                }
                ENDCG
            }
        }
    }
    
    Fallback "UI/Default"
}