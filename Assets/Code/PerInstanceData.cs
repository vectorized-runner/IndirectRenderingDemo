using Unity.Mathematics;

// Should match the Shader layout exactly
public struct PerInstanceData
{
	public float4x4 LocalToWorld;
	public float4 Color;
}