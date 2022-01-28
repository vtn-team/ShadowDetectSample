using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class ShadowProjector : MonoBehaviour
{
    [SerializeField]
    private int _renderTextureSize = 512;

    private Camera _camera;
    [SerializeField]
    private RenderTexture _renderTexture;

    static public Matrix4x4 MatPV;

    private void OnEnable()
    {
        _camera = GetComponent<Camera>();
        //_renderTexture = new RenderTexture(_renderTextureSize, _renderTextureSize, 0, RenderTextureFormat.RGB565, 0);
        _camera.targetTexture = _renderTexture;
        _camera.SetReplacementShader(Shader.Find("ShadowProjector"), null);
        _camera.depth = -10000; // ��Ƀ����_�����O�������̂ŏ��������Ă���
        _camera.clearFlags = CameraClearFlags.Color;
        _camera.backgroundColor = Color.white;
    }

    private void OnPostRender()
    {
        var viewMatrix = _camera.worldToCameraMatrix;
        var projectionMatrix = GL.GetGPUProjectionMatrix(_camera.projectionMatrix, true);
        Shader.SetGlobalMatrix("_ShadowProjectorMatrixVP", projectionMatrix * viewMatrix);
        MatPV = projectionMatrix * viewMatrix;
        Shader.SetGlobalTexture("_ShadowProjectorTexture", _renderTexture);
        // �v���W�F�N�^�[�̈ʒu��n��
        // _ObjectSpaceLightPos�̂悤�Ȋ�����w��0�������Ă�����Orthographic�̑O�������Ƃ݂Ȃ�
        var projectorPos = Vector4.zero;
        projectorPos = _camera.orthographic ? transform.forward : transform.position;
        projectorPos.w = _camera.orthographic ? 0 : 1;
        Shader.SetGlobalVector("_ShadowProjectorPos", projectorPos);
    }
}