// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnlitColorTexture"
{
    Properties
    {
        // Color property for material inspector, default to white
        _Color("Main Color", Color) = (1,1,1,1)
        // Texture property for material inspector, default to white
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // fragment shader struct
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                // setting up for VR
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // input color
            fixed4 _Color;
            // input texture
            sampler2D _MainTex;
            float4 _MainTex_ST;

            // vertex shader
            v2f vert(appdata_base v)
            {
                v2f o;

                // setting up for VR
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // passing data to fragment shader
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                return o;
            }


            // fragment shader
            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture - assuming b&w
                fixed4 col = tex2D(_MainTex, i.uv);
                // multiply by color
                col = col * _Color;

                if (col.a < 0.1)
                    discard;

                return col;
            }

            ENDCG
        }
    }
}
