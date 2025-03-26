Shader "Unlit/HDRP_Shader1"
{
    SubShader
    {
        //Tags 
        //{
        //    "RenderType"="Opaque"
        //    "RenderPipeline"="HDRenderPipeline"
        //}
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Tags {"LightMode" = "Forward"}
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            // ‰~Žü—¦
            #define PI 3.14159265
            //static const PI = 3.14159265f;

            float _dT = 0;

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
                p = p * 2;
                float theta = atan2(p.y, p.x);
                float t = 4*_dT % 2;
                float r = sqrt(p.x*p.x + p.y*p.y);
                int n1 =7;
                int n2 = 6*t;
                float r_in = 0.5;
                float r_band = 0.4;
                if(t <= 1){
                    r_in *= t;
                    r_band *= t;
                } else {
                    n2 = 6;
                }

                float b = 0;
                for(int i = 0; i < n2; i++){
                    float d_i = length(p) - r_in + sin((theta + 2*PI*i - _dT) * n1 / n2) * r_band;
                    float b_i = 0.01 / abs(d_i);
                    if(b < b_i){
                        b = b_i;
                    }
                }
                if(t <= 1){
                    b *= t;
                } else {
                    b *= (2 - t);
                }

                if(r > .6){
                    b *= 1 - 0.5*r;
                }
                float4 purple = float4(.5, .0, 1., b);
                float4 white = float4(.9, .75, 1., b);
                float col_rate = r;
                float4 col = purple*(1-col_rate) + white*col_rate;
                
                return col;
            }
            ENDHLSL
        }
    }  
}
