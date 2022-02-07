// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnlitColor"
{
    Properties
    {
        // Color property for material inspector, default to white
        _Color("Main Color", Color) = (1,1,1,1)
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
                float4 vertex : SV_POSITION;
                // setting up for VR
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;

            // vertex shader
            v2f vert(appdata_base v)
            {
                v2f o;

                // setting up for VR
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // passing data to fragment shader
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }


            // fragment shader
            fixed4 frag(v2f i) : SV_Target
            {
                // just returning colour
                fixed4 col = _Color;
                return col;
            }

            ENDCG
        }
    }
}
