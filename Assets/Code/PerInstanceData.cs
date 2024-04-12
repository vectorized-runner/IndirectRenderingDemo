using Unity.Mathematics;

// Should match the Shader/Compute layout exactly
public struct PerInstanceData
{
	public float4x4 LocalToWorld;
	public float4 Color;
}