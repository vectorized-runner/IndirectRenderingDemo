Shader "Custom/InstancedIndirectColor"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct VertInput
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct FragInput
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            struct PerInstanceData
            {
                float4x4 localToWorld;
                float4 color;
            };

            StructuredBuffer<PerInstanceData> _PerInstanceData;

            FragInput vert(VertInput input, uint instanceID: SV_InstanceID)
            {
                FragInput output;

                const float4 worldPos = mul(_PerInstanceData[instanceID].localToWorld, input.vertex);
                output.vertex = UnityObjectToClipPos(worldPos);
                output.color = _PerInstanceData[instanceID].color;

                return output;
            }

            fixed4 frag(FragInput i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}