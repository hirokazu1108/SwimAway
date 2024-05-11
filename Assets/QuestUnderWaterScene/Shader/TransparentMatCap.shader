Shader "UnderWaterSample/TransparentMatCap"
{
    Properties
    {
        _Color("Main Color", Color) = (1, 1, 1, 1)
        _MatCap("MatCap", 2D) = "white" {}
        _Alpha("MatCap Transparent", Range(0, 1)) = 1

        _RimColor("RimColor", Color) = (1,1,1,1)
        _RimPower("RimPower", float) = 0.0
        _RimThickness("RimThickness", float) = 0.0
    }

        Subshader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
               #include "UnityCG.cginc"
               #pragma vertex vert
               #pragma fragment frag
               #pragma multi_compile_fog

                fixed4 _Color;
                sampler2D _MatCap;
                float _Alpha;

                fixed4 _RimColor;
                float _RimPower;
                float _RimThickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };


            struct v2f
            {
                float4 vertex  : SV_POSITION;
                half2 uv    : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 worldPos : TEXCOORD3;
                float2 matcapUV : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };

            float2 matcapSample(float3 viewDirection, float3 normalDirection)
            {
                half3 worldUp = float3(0, 1, 0);
                half3 worldViewUp = normalize(worldUp - viewDirection * dot(viewDirection, worldUp));
                half3 worldViewRight = normalize(cross(viewDirection, worldViewUp));
                half2 matcapUV = half2(dot(worldViewRight, normalDirection), dot(worldViewUp, normalDirection)) * 0.5 + 0.5;
                return matcapUV;
            }

            v2f vert(appdata v)
            {
                v2f o; 
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                o.matcapUV = matcapSample(normalize(_WorldSpaceCameraPos - o.worldPos), UnityObjectToWorldNormal(v.normal));

                float4x4 modelMatrix = unity_ObjectToWorld;
                o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                fixed4 col = _Color;
                // カメラから見た法線のxyをそのままUVとして使う
                fixed4 matcap = tex2D(_MatCap, i.matcapUV);

                half rim = 1.0 - abs(dot(i.viewDir, i.normalDir));
                fixed3 emission = _RimColor.rgb * pow(rim, _RimThickness) * _RimPower;

                float mask = lerp(0.1, 1, matcap.a);

                col.rgb *= matcap.rgb;
                col.rgb += emission;
                col.a = _Alpha * mask;

                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }

            ENDCG
        }
    }
}