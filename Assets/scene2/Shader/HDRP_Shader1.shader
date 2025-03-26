Shader "Unlit/HDRP_Shader1"
{
    SubShader
    {
        Tags 
        {
            "RenderType"="Opaque"
            "RenderPipeline"="HDRenderPipeline"
        }
        LOD 100

        Pass
        {
            Tags {"LightMode" = "Forward"}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            // ‰~Žü—¦
            #define PI 3.14159265
            //static const PI = 3.14159265f;

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
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 p = i.uv.xy * 2. - 1.;
                float theta = atan2(p.y, p.x);
                float d = length(p) - .5 + sin(theta * 6. + (PI / 2.)) * .4;
                float b = 0.01 / abs(d);
                float3 col = float3(.5, .0, 1.);
                return float4(b * col, 1);
            }
            ENDHLSL
        }
    }  
}
