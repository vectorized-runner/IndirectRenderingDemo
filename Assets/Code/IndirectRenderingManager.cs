using Unity.Mathematics;
using UnityEngine;

public unsafe class IndirectRenderingManager : MonoBehaviour
{
	public int ObjectCount = 100_000;
	public float SpawnRadius = 10_000.0f;

	public Material Material;
	public Mesh Mesh;

	private ComputeBuffer _perInstanceDataBuffer;
	private ComputeBuffer _argsBuffer;

	private Bounds _renderBounds;
	private Unity.Mathematics.Random _random;
	private static readonly int _perInstanceDataId = Shader.PropertyToID("_PerInstanceData");

	private void Start()
	{
		_renderBounds = new Bounds(float3.zero, new float3(SpawnRadius));
		_random = new Unity.Mathematics.Random(1);

		InitBuffers();
	}

	private void InitBuffers()
	{
		var args = new uint[] { 0, 0, 0, 0, 0 };
		args[0] = Mesh.GetIndexCount(0);
		args[1] = (uint)ObjectCount;
		args[2] = Mesh.GetIndexStart(0);
		args[3] = Mesh.GetBaseVertex(0);
		const int count = 1;
		var stride = args.Length * sizeof(uint);
		_argsBuffer = new ComputeBuffer(count, stride, ComputeBufferType.IndirectArguments);
		_argsBuffer.SetData(args);

		var properties = new PerInstanceData[ObjectCount];
		for (int i = 0; i < ObjectCount; i++)
		{
			var props = new PerInstanceData();
			var position = _random.NextFloat3(-SpawnRadius, SpawnRadius);
			var rotation = _random.NextQuaternionRotation();
			var scale = new float3(1, 1, 1);
			props.LocalToWorld = float4x4.TRS(position, rotation, scale);
			props.Color = new float4(_random.NextFloat3(), 1.0f);
			properties[i] = props;
		}

		_perInstanceDataBuffer = new ComputeBuffer(ObjectCount, sizeof(PerInstanceData));
		_perInstanceDataBuffer.SetData(properties);
		Material.SetBuffer(_perInstanceDataId, _perInstanceDataBuffer);
	}

	private void Update()
	{
		Graphics.DrawMeshInstancedIndirect(Mesh, 0, Material, _renderBounds, _argsBuffer);
	}

	private void OnDestroy()
	{
		_perInstanceDataBuffer?.Release();
		_argsBuffer?.Release();
	}
}