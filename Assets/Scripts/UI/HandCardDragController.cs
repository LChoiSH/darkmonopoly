using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HandCardDragController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Hierarchy")]
    [Tooltip("Horizontal Layout Group가 붙은 부모")]
    [SerializeField] private RectTransform cardWrap;

    [Tooltip("드래그 전용 레이어(레이아웃 없음). CardCanvas 아래 빈 RectTransform로 하나 만들어 연결")]
    [SerializeField] private RectTransform dragLayer;

    [Header("Raycast")]
    [Tooltip("이 CardCanvas가 속한 Canvas 또는 상위 Canvas의 GraphicRaycaster")]
    [SerializeField] private GraphicRaycaster graphicRaycaster;

    [Header("Board Drop")]
    [Tooltip("드롭 타깃(보드/타일) 레이어 마스크")]
    [SerializeField] private LayerMask boardLayerMask = ~0;

    [Tooltip("정확히 타일 기준으로만 인정하려면, 타일에 Collider2D 필수")]
    [SerializeField] private bool requireCollider2D = true;

    // 드롭 성공 시 알림 (외부에서 구독해서 실제 카드 사용 처리)
    public System.Action<CardUI, Vector3> OnCardUsed;

    // 내부 상태
    private RectTransform selectedRt;
    private CanvasGroup selectedCg;
    private CardUI selectedCardUI;          // 선택된 CardUI
    private Vector2 pointerOffsetLocal;     // dragLayer 좌표계에서 포인터-카드 오프셋
    private int originalIndex;              // cardWrap 내 원래 형제 인덱스
    private RectTransform placeholder;      // 드래그 중 자리 유지용

    // --------- Unity Lifecycle ---------
    void Update()
    {
        #if UNITY_EDITOR
        // P키: 일시정지 (재개는 Unity Pause 버튼이나 Ctrl+Shift+P 사용)
        if (Input.GetKeyDown(KeyCode.P) && !EditorApplication.isPaused)
        {
            EditorApplication.isPaused = true;
            Debug.Log("Editor Paused - Unity Pause 버튼이나 단축키로 재개하세요");
        }
        #endif
    }

    // --------- 유틸: 현재 Canvas의 렌더모드에 맞는 카메라 추출 ---------
    private Camera GetEventCam(PointerEventData e)
    {
        var canvas = GetComponentInParent<Canvas>();
        if (!canvas) return e?.pressEventCamera ?? Camera.main;
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null; // Overlay는 null 카메라가 정답
        return e?.pressEventCamera ?? canvas.worldCamera ?? Camera.main;
    }

    // --------- 유틸: 포인터 아래 CardUI RectTransform 찾기 ---------
    private RectTransform GetCardUnderPointer(Vector2 screenPos)
    {
        if (!graphicRaycaster) graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        if (!graphicRaycaster) return null;

        var ped = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        graphicRaycaster.Raycast(ped, results);

        foreach (var r in results)
        {
            // CardUI가 붙은 오브젝트(혹은 자식)만 집음
            var card = r.gameObject.GetComponentInParent<RectTransform>();
            if (card != null && card.transform.parent == cardWrap) // CardWrap의 직계 자식만
                return card;
        }
        return null;
    }

    // -------------------------- IPointerDown --------------------------
    public void OnPointerDown(PointerEventData eventData)
    {
        // 선택 후보만 저장 (검증/준비는 BeginDrag에서)
        selectedRt = GetCardUnderPointer(eventData.position);
        selectedCardUI = selectedRt ? selectedRt.GetComponent<CardUI>() : null;
    }

    // -------------------------- IBeginDrag ----------------------------
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!selectedRt) return;

        // 마나 체크 - 마나가 부족하면 드래그 취소
        if (selectedCardUI != null && selectedCardUI.Card != null)
        {
            int cardCost = selectedCardUI.Card.Cost;
            if (GameManager.Instance.manaSystem.CurrentMana < cardCost)
            {
                Debug.LogWarning($"Not enough mana to use [{selectedCardUI.Card.Title}]. Need {cardCost}, have {GameManager.Instance.manaSystem.CurrentMana}");
                selectedRt = null;
                selectedCardUI = null;
                return;
            }
        }

        // 컴포넌트 확보
        selectedCg = selectedRt.GetComponent<CanvasGroup>() ?? selectedRt.gameObject.AddComponent<CanvasGroup>();
        // selectedLe = selectedRt.GetComponent<LayoutElement>() ?? selectedRt.gameObject.AddComponent<LayoutElement>();

        // 원래 자리 인덱스 저장
        originalIndex = selectedRt.GetSiblingIndex();

        // 자리표 생성(원래 자리를 유지하고, HLG가 레이아웃을 계속 계산하도록)
        placeholder = new GameObject("Placeholder").AddComponent<RectTransform>();
        placeholder.SetParent(cardWrap, worldPositionStays: false);

        // CardUI의 RectTransform 설정 복사
        placeholder.anchorMin = selectedRt.anchorMin;
        placeholder.anchorMax = selectedRt.anchorMax;
        placeholder.sizeDelta = selectedRt.sizeDelta;
        placeholder.pivot = selectedRt.pivot;

        placeholder.SetSiblingIndex(originalIndex);

        // 드롭 레이캐스트 차단
        selectedCg.blocksRaycasts = false;

        // DragLayer로 이동 (worldPositionStays=true로 먼저 세계좌표 유지)
        selectedRt.SetParent(dragLayer, worldPositionStays: true);
        selectedRt.SetAsLastSibling();

        // DragLayer 좌표계에서 포인터-카드 오프셋 저장
        var cam = GetEventCam(eventData);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragLayer, eventData.position, cam, out var pInDrag))
        {
            // anchoredPosition은 이미 dragLayer 기준 값
            pointerOffsetLocal = selectedRt.anchoredPosition - pInDrag;
        }
    }

    // -------------------------- IDrag --------------------------------
    public void OnDrag(PointerEventData eventData)
    {
        if (!selectedRt) return;

        var cam = GetEventCam(eventData);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragLayer, eventData.position, cam, out var pInDrag))
        {
            selectedRt.anchoredPosition = pInDrag + pointerOffsetLocal;
        }
    }

    // -------------------------- IEndDrag ------------------------------
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!selectedRt) return;

        bool droppedOnBoard = TryRaycastBoard(eventData, out var hitPoint);
        bool droppedOutsideCardWrap = !IsPointerOverCardWrap(eventData);

        bool shouldUseCard = false;
        Vector3 usePoint = Vector3.zero;

        if (selectedCardUI != null && selectedCardUI.Card != null)
        {
            if (selectedCardUI.Card.NeedsTarget)
            {
                // 타겟 필요: 보드에 raycast 성공해야 사용
                if (droppedOnBoard)
                {
                    shouldUseCard = true;
                    usePoint = hitPoint;
                }
            }
            else
            {
                // 타겟 불필요: CardWrap 밖에 드롭하면 사용
                if (droppedOutsideCardWrap)
                {
                    shouldUseCard = true;
                    usePoint = hitPoint; // 보드 위치 (있으면) 또는 default
                }
            }
        }

        if (shouldUseCard)
        {
            // 카드 사용 성공
            OnCardUsed?.Invoke(selectedCardUI, usePoint);
            if (placeholder) Destroy(placeholder.gameObject);
        }
        else
        {
            // 실패: 원래 부모로 복귀
            selectedRt.SetParent(cardWrap, worldPositionStays: false);
            selectedRt.SetSiblingIndex(originalIndex);
            if (placeholder) Destroy(placeholder.gameObject);
        }

        // 공통 정리
        selectedCg.blocksRaycasts = true;
        selectedRt = null; selectedCg = null; selectedCardUI = null;
        placeholder = null;
    }

    // --------- 보드 레이캐스트 (2D 물리, Collider2D 기준) ---------
    private bool TryRaycastBoard(PointerEventData e, out Vector3 hitPoint)
    {
        var cam = GetEventCam(e);
        var ray = cam ? cam.ScreenPointToRay(e.position) : new Ray(
            new Vector3(e.position.x, e.position.y, 0f), Vector3.forward);

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, boardLayerMask);
        if (hit.collider != null)
        {
            if (requireCollider2D && hit.collider is Collider2D)
            {
                hitPoint = hit.point;
                return true;
            }
            // Collider2D만 요구하지 않으면 히트만으로도 성공 처리
            hitPoint = hit.point;
            return true;
        }

        hitPoint = default;
        return false;
    }

    // --------- CardWrap 영역 체크 ---------
    private bool IsPointerOverCardWrap(PointerEventData e)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(cardWrap, e.position, GetEventCam(e));
    }
}
