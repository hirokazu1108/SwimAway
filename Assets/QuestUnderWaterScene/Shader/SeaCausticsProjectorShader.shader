Shader "UnderWaterSample/SeaCausticsProjector"
{
    Properties
    {   
        [HideInInspector] _MainTex("Tex", 2D) = "white" {}

        [Header(Cellular Noise)]_Brightness("Whiteness", float) = 1.0
        _Threshold1("Alpha Threshold1", Range(0, 1)) = 0.5
        _Threshold2("Alpha Threshold2", Range(0, 1)) = 0.5
        [IntRange]_Size("Cell Size", Range(2, 16)) = 2
        _Speed("Cell Speed", Range(0, 1)) = 0.5  
        _randamize("Randamize", float) = 6.2831
        [Space(10)]_Scale("UV Scale", float) = 1
        _Alpha("Transparent", Range(0, 1)) = 1.0


        [Space(20)][Header(HSV)]_Hue("H", Range(0, 1)) = 0.5
        _Saturation("S", Range(0, 1)) = 0.5
        _Value("V", Range(0, 1)) = 0.5

        [Space(20)][Header(Diffuse)]_Diffuse("Diffuse Color", Color) = (0.3,0.3,1,1) // 拡散反射の色
        _Kd("Deffuse Strength", Range(0.01, 1)) = 0.8 // 拡散反射の反射率

        [Space(10)][Header(Specular)]_Specular("Specular Color", Color) = (1,1,1,1) // 鏡面反射の色
        _Ks("Specular Strength", Range(0.01, 1)) = 1.0 // 鏡面反射の反射率
        _Shininess("Specular Shininess", Range(0, 100)) = 10 // ハイライトの鋭さ

        [Space(10)][Header(Frenel)][PowerSlider(0.1)] _F0("Frenel Strength", Range(0.0, 1.0)) = 0.02 // フレネル効果の強さ
        _Fresnel("Fresnel Color", Color) = (0.3,0.3,1,1) // フレネル効果の色

        [Space(10)][Header(Ambient)]_Ka("Ambient Strength", Range(0.01, 1)) = 0.5 // 環境光の反射率
        _RimPower("RimPower", Range(0.0, 10.0)) = 0.3

        [Space(20)][Header(UV Scroll)]_Speed1("Scroll Speed", Range(0, 10)) = 1
        _Direction1("Scroll Direction", Range(0, 1)) = 0.5
        _UVdistortion("UV Distortion", Range(0, 1)) = 0.1
        _NoiseTex("Noise Texture", 2D) = "white" {}

        [Header(Red)]
        _RedX("Offset X", Range(-0.5, 0.5)) = 0.0
        _RedY("Offset Y", Range(-0.5, 0.5)) = 0.0

        [Header(Green)]
        _GreenX("Offset X", Range(-0.5, 0.5)) = 0.0
        _GreenY("Offset Y", Range(-0.5, 0.5)) = 0.0

        [Header(Blue)]
        _BlueX("Offset X", Range(-0.5, 0.5)) = 0.0
        _BlueY("Offset Y", Range(-0.5, 0.5)) = 0.0
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Equal

            Pass
            {
                Tags { "LightMode" = "ForwardBase" }

                Cull Back

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog


                #include "UnityCG.cginc"
                #include "Lighting.cginc"
                

                #define PI 3.141592

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                   
                };

                struct v2f
                {
                    float4 uv : TEXCOORD0;
                    float3 normal : TEXCOORD1;
                    float3 view   : TEXCOORD2;
                    UNITY_FOG_COORDS(3)
                    float4 vertex : SV_POSITION;

                };

                sampler2D _MainTex;

                float _Brightness;
                float _Threshold1;
                float _Threshold2;
                int _Size;
                float _Scale;
                float _Speed;
                float distance;
                float _Alpha;
                float _randamize;
                
                float _Hue;
                float _Saturation;
                float _Value;

                float4 _Diffuse;
                float _Kd;
                float4 _Specular;
                float _Ks;
                float _Shininess;
                float _Ka;
                half _RimPower;
                float _F0;
                float4 _Fresnel;

                float _Speed1;
                float _Direction1;
                float _UVdistortion;
                sampler2D _NoiseTex;

                float _RedX;
                float _RedY;
                float _GreenX;
                float _GreenY;
                float _BlueX;
                float _BlueY;

                float4x4 unity_Projector;
                float4x4 unity_ProjectorClip;

                v2f vert(appdata v)
                {
                    v2f o;
                    float4 scrollVector1 = float4(cos(_Direction1 * 2 * 3.141592), sin(_Direction1 * 2 * 3.141592), 0, 0);
                    float4 offset1 = _Time.x * (_Speed1 / _Scale) * scrollVector1;

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = (mul(unity_Projector, v.vertex) * _Scale * 0.1 + offset1);
                    o.normal = UnityObjectToWorldNormal(v.normal);
                    o.view = normalize(ObjSpaceViewDir(v.vertex));  

                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                float2 random2(float2 st)
                {
                    st = float2(dot(st, float2(127.1, 311.7)),
                                dot(st, float2(269.5, 183.3)));
                    return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
                }


                float cellularnoise(float2 st, float n) {
                    st *= n;

                    float2 ist = floor(st);
                    float2 fst = frac(st);

                    distance = 5;

                    for (int y = -1; y <= 1; y++)
                    for (int x = -1; x <= 1; x++)
                        {
                            float2 neighbor = float2(x, y);
                            float2 p = 0.5 + 0.5 * sin(_Time.y * _Speed + _randamize * random2(ist + neighbor));
                            float2 diff = neighbor + p - fst;
                            distance = min(distance, length(diff));
                        }

                    
                    float color = distance * _Brightness;

                    return color;

                }

                float3 RGBtoHSV(float3 c) {
                    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                    float4 p = c.g < c.b ? float4(c.bg, K.wz) : float4(c.gb, K.xy);
                    float4 q = c.r < p.x ? float4(p.xyw, c.r) : float4(c.r, p.yzx);
                    float d = q.x - min(q.w, q.y);
                    float e = 1.0e-10;
                    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
                }

                float3 HSVtoRGB(float3 c) {
                    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                    return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
                }

                fixed4 frag(v2f i) : SV_Target{


                    float4 uv = UNITY_PROJ_COORD(i.uv);
                    uv += tex2D(_NoiseTex, i.uv).r * _UVdistortion;
                    
                    
                    float2 red_uv = uv.xy + float2(_RedX, _RedY);
                    float2 green_uv = uv.xy + float2(_GreenX, _GreenY);
                    float2 blue_uv = uv.xy + float2(_BlueX, _BlueY);

                    fixed4 col;
                    col = cellularnoise(uv.xy, _Size);
                    
                    float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                    float alpha = smoothstep(_Threshold1 - _Threshold2, _Threshold1 + _Threshold2, gray) * _Alpha;
                    float3 hsv = RGBtoHSV(col.rgb);

                    hsv.x = _Hue;
                    hsv.y = _Saturation;
                    hsv.z = _Value;

                    fixed3 rgb = HSVtoRGB(hsv);
                    col = fixed4(rgb, alpha);

                    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                    float3 NdotL = max(0, dot(i.normal, lightDir));

                    // 反射ベクトル
                    float3 reflect = normalize(-lightDir + 2.0 * i.normal * NdotL);

                    // 拡散反射
                    float3 diffuse = _Kd * col.xyz * NdotL * _Diffuse;

                    // 鏡面反射
                    float3 spec = _Ks * pow(max(0, dot(reflect, i.view)), _Shininess) * _Specular;

                    // 環境光
                    half4 refColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflect, 0);
                    half rim = pow(1.0 - abs(dot(i.view, i.normal)), 2);
                    float3 ambient = col.xyz * _Ka * float3(1, 1, 1) + refColor * rim * _RimPower;

                    // フレネル効果
                    half vdotn = dot(i.view, i.normal.xyz);
                    half3 fresnel = (_F0 + (1.0h - _F0) * pow(1.0h - vdotn, 5)) * _Fresnel;

                    fixed3 c = diffuse + spec + ambient + fresnel;

                    col = fixed4(c, alpha);
                    
                    col.r *= cellularnoise(red_uv, _Size);
                    col.g *= cellularnoise(green_uv, _Size);
                    col.b *= cellularnoise(blue_uv, _Size);

                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                    
                    

                }
                ENDCG
            }
        }
}