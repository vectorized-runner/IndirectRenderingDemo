using UnityEngine;

// Should match the Shader layout exactly
public struct PerInstanceData
{
	public Matrix4x4 LocalToWorld;
	public Vector4 Color;
}