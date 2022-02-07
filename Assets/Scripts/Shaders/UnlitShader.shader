// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnlitShader"
{
    Properties
    {
        // Color property for material inspector, default to white
        _Color("Main Color", Color) = (1,1,1,1)
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

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            // vertex shader
            v2f vert(appdata_base v)
			{
				v2f o;
                
                // setting up for VR
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				return o;
			}
            

            // fragment shader
            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // just returning colour
                col = col * _Color;
				return col;
			}

        ENDCG
        }
    }
}
