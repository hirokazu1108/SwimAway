Shader "UnderWaterSample/QuestColorOverlay"
{
    Properties
    {
		_dim("Transparent", range(0, 1))= 0.3
        _distance("オブジェクト中心からの描画距離",float)=5
        _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { 
			"RenderType"="Transparent"
			"Queue"="Transparent+5000" 
            }
		Cull front
        ZWrite Off
        ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
			float _dim,_distance;
            fixed4 _Color;
            fixed4 frag () : SV_Target
            
            {
				float sX = 1/sqrt(pow(unity_WorldToObject[0].x, 2) + pow(unity_WorldToObject[0].y, 2) + pow(unity_WorldToObject[0].z, 2));
				float sY = 1/sqrt(pow(unity_WorldToObject[1].x, 2) + pow(unity_WorldToObject[1].y, 2) + pow(unity_WorldToObject[1].z, 2));
				float size = (sX + sY)/2;
				if(length(mul(unity_ObjectToWorld,float4(0,0,0,1))-_WorldSpaceCameraPos)>=_distance)discard;
				return float4(_Color.r, _Color.g, _Color.b, _dim);
            }
            ENDCG
        }
    }
}