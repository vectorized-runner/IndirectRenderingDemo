using System;
using Unity.Mathematics;
using UnityEngine;

// Should match the Shader layout exactly
public struct PerInstanceData
{
	public Matrix4x4 LocalToWorld;
	public Vector4 Color;
}

public unsafe class IndirectRenderingManager : MonoBehaviour
{
	public int ObjectCount = 100_000;
	public float SpawnRadius = 10_000.0f;

	public Material material;

	private ComputeBuffer _perInstanceDataBuffer;
	private ComputeBuffer _argsBuffer;

	private Mesh mesh;
	private Bounds _renderBounds;
	private Unity.Mathematics.Random _random;
	private static readonly int _perInstanceDataId = Shader.PropertyToID("_PerInstanceData");

	private void Start()
	{
		// TODO: Why?
		Mesh mesh = CreateQuad();
		this.mesh = mesh;

		_renderBounds = new Bounds(float3.zero, new float3(SpawnRadius));

		InitializeBuffers();

		_random = new Unity.Mathematics.Random(1);
	}

	private void InitializeBuffers()
	{
		var args = new uint[] { 0, 0, 0, 0, 0 };
		args[0] = mesh.GetIndexCount(0);
		args[1] = (uint)ObjectCount;
		args[2] = mesh.GetIndexStart(0);
		args[3] = mesh.GetBaseVertex(0);
		_argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
		_argsBuffer.SetData(args);

		var properties = new PerInstanceData[ObjectCount];
		for (int i = 0; i < ObjectCount; i++)
		{
			var props = new PerInstanceData();
			var position = _random.NextFloat3(-SpawnRadius, SpawnRadius);
			var rotation = _random.NextQuaternionRotation();
			var scale = new float3(1, 1, 1);
			props.LocalToWorld = Matrix4x4.TRS(position, rotation, scale);
			props.Color = new Color(_random.NextFloat(), _random.NextFloat(), _random.NextFloat(), 1.0f);
			properties[i] = props;
		}

		_perInstanceDataBuffer = new ComputeBuffer(ObjectCount, sizeof(PerInstanceData));
		_perInstanceDataBuffer.SetData(properties);
		material.SetBuffer(_perInstanceDataId, _perInstanceDataBuffer);
	}

	private Mesh CreateQuad(float width = 1f, float height = 1f)
	{
		// TODO:
		throw new NotImplementedException();
	}


	private void Update()
	{
		Graphics.DrawMeshInstancedIndirect(mesh, 0, material, _renderBounds, _argsBuffer);
	}

	private void OnDisable()
	{
		_perInstanceDataBuffer?.Release();
		_argsBuffer?.Release();
	}
}