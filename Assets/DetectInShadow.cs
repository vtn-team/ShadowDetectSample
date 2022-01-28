using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectInShadow : MonoBehaviour
{
    [SerializeField] Camera _shadowCamera;
    [SerializeField] RenderTexture _shadowMap;
    [SerializeField] UnityEngine.UI.Image _test;

    Texture2D _texture;

    private void Start()
    {
        _texture = new Texture2D(_shadowMap.width, _shadowMap.height);
    }

    Color GetPixel(Vector2 pos)
    {
        CopyToTexture2D();

        return _texture.GetPixel((int)pos.x, (int)pos.y);
    }

    private void CopyToTexture2D()
    {
        // �A�N�e�B�u�ȃ����_�[�e�N�X�`�����L���b�V�����Ă���
        var currentRT = RenderTexture.active;

        // �A�N�e�B�u�ȃ����_�[�e�N�X�`�����ꎞ�I��Target�ɕύX����
        RenderTexture.active = _shadowMap;

        // Texture2D.ReadPixels()�ɂ��A�N�e�B�u�ȃ����_�[�e�N�X�`���̃s�N�Z�������e�N�X�`���Ɋi�[����
        _texture.ReadPixels(new Rect(0, 0, _shadowMap.width, _shadowMap.height), 0, 0);
        _texture.Apply();

        // �A�N�e�B�u�ȃ����_�[�e�N�X�`�������ɖ߂�
        RenderTexture.active = currentRT;
    }

    void Update()
    {
        Vector3 spos = _shadowCamera.WorldToScreenPoint(this.transform.position);
        _test.rectTransform.localPosition = new Vector2( (spos.x - _shadowMap.width/2) / _shadowMap.width * 128, (spos.y - _shadowMap.height / 2) / _shadowMap.height * 128);

        var cols = GetPixel(spos);
        if(cols.b < 0.9 || cols.r < 0.9 || cols.g < 0.9)
        {
            Debug.Log("in shadow");
        }
    }
}
