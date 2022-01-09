Shader "Custom/Portal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LeftEyeTexture ("Texture", 2D) = "white" {}
        _RightEyeTexture("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 screenPos : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _LeftEyeTexture;
            sampler2D _RightEyeTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex); // use the screen position coordinates of the portal to sample the render texture (which is our screen)
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w; // clip space -> normalized texture (?)
                //uv = UnityStereoTransformScreenSpaceTex(uv);

                // sample the texture
//#if UNITY_SINGLE_PASS_STEREO
//                fixed4 col = tex2D(_LeftEyeTexture, uv);
//#else
                fixed4 col = unity_StereoEyeIndex == 0 ? tex2D(_LeftEyeTexture, uv) : tex2D(_RightEyeTexture, uv);
//#endif 
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
