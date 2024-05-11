Shader "UnderWaterSample/FadeWater"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _RefColor("Reflection Color", Color) = (1, 1, 1, 1)
        _RefPower("Reflection Power", Range(0, 10)) = 5
        _Speed("Scroll Speed", Range(0, 1)) = 0.5
        _Direction("Scroll Direction", Range(0, 1)) = 0.25
        _NormalMap1 ("NormalMap1", 2D) = "bump" {}
        _NormalMap2("NormalMap2", 2D) = "bump" {}
        _Alpha("Transparent", Range(0, 1)) = 1.0
        _Mask("Mask Texture", 2D) = "white" {}
        _Cube("Cube", CUBE) = "" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200
        Cull Off
        CGPROGRAM
        #pragma surface surf Standard alpha:fade nofog

        #pragma target 3.0

        sampler2D _NormalMap1;
        sampler2D _NormalMap2;

        struct Input
        {
            float2 uv_NormalMap1;
            float2 uv_NormalMap2;
            float2 uv_Mask;
            float3 viewDir;
            float3 worldRefl;
            INTERNAL_DATA
        };

        half _Alpha;
        sampler2D _Mask;
        float _RefPower;
        float _Speed;
        float _Direction;
        fixed4 _Color;
        fixed4 _RefColor;
        samplerCUBE _Cube;

        
        UNITY_INSTANCING_BUFFER_START(Props)
        
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {   
            float2 offset = _Time.x * _Speed * float2(cos(_Direction * 2 * 3.141592), sin(_Direction * 2 * 3.141592));
            float3 normal1 = UnpackNormal(tex2D(_NormalMap1, IN.uv_NormalMap1 + offset));
            float3 normal2 = UnpackNormal(tex2D(_NormalMap2, IN.uv_NormalMap2 - offset));
            o.Normal = normalize(float3(normal1.rg + normal2.rg, normal1.b * normal2.b));

            float ref = saturate(dot(o.Normal, normalize(IN.viewDir)));
            ref = pow(ref, _RefPower);

            float mask = tex2D(_Mask, IN.uv_Mask).r;

            o.Albedo = _Color.rgb;
            o.Alpha = mask * _Alpha;
            o.Emission = lerp(texCUBE(_Cube, WorldReflectionVector(IN, o.Normal)), _RefColor.rgb, ref);

        }
        ENDCG
    }
    FallBack "Diffuse"
}
