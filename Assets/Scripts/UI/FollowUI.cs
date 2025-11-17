using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUI : MonoBehaviour
{
    [SerializeField] RectTransform uiCanvasRoot;   // Screen Space - Camera Canvas의 RectTransform
    [SerializeField] Transform worldTarget;        // 유닛 머리 위(소켓) Transform
    [SerializeField] Camera cam;                   // UI가 참조하는 카메라
    [SerializeField] Vector2 screenOffset = new Vector2(0, 0); // 화면 픽셀 기준 오프셋

    RectTransform _rt;
    Vector2 _lastPos;

    void Awake()
    {
        _rt = (RectTransform)transform;

        if (!cam) cam = Camera.main;
        if (!uiCanvasRoot) uiCanvasRoot = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }
    
    public void SetTarget(Transform target, Vector2 offset)
    {
        this.worldTarget = target;
        screenOffset = offset;
    }

    void LateUpdate()
    {
        if (!worldTarget) { gameObject.SetActive(false); return; }

        var sp = cam.WorldToScreenPoint(worldTarget.position);
        // 카메라 뒤면 숨김
        if (sp.z <= 0f) { if (_rt.gameObject.activeSelf) _rt.gameObject.SetActive(false); return; }

        if (!_rt.gameObject.activeSelf) _rt.gameObject.SetActive(true);

        sp += (Vector3)screenOffset;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvasRoot, sp, cam, out var local);
        // 픽셀 스냅(옵션)
        local.x = Mathf.Round(local.x);
        local.y = Mathf.Round(local.y);

        // 변화 있을 때만 적용(미세한 Rebuild 방지)
        if ((local - _lastPos).sqrMagnitude > 0.01f) {
            _rt.anchoredPosition = local;
            _lastPos = local;
        }
    }
}
