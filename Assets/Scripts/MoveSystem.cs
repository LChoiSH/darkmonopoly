using System;
using System.Collections;
using UnityEngine;
using UnitSystem;

public class MoveSystem : MonoBehaviour
{
    [SerializeField] private float stepDuration = 0.15f;
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0,0,1,1);

    private MapBuilder map;
    private Unit playerUnit;

    public int CurrentIndex { get; private set; }
    public bool IsMoving => playerUnit != null && playerUnit.Mover.IsMoving;

    public event Action<int> OnTilePass;   // index
    public event Action<int> OnTileArrive; // index

    public void Init(MapBuilder mapBuilder, Unit target, int startIndex)
    {
        map = mapBuilder;
        playerUnit = target;
        CurrentIndex = Mathf.Clamp(startIndex, 0, map.Count - 1);
        playerUnit.Mover.SnapTo(map.Pos(CurrentIndex));
    }

    public void MoveSteps(int steps)
    {
        if (IsMoving || steps <= 0) return;

        StartCoroutine(MoveRoutine(steps));
    }

    private IEnumerator MoveRoutine(int steps)
    {
        // playerUnit.Mover.IsMoving = true;

        for (int s = 0; s < steps; s++)
        {
            int next = map.Next(CurrentIndex);

            // 1) 내부 인덱스 즉시 갱신(권위)
            int from = CurrentIndex;
            CurrentIndex = next;

            // 2) 한 칸 이동 연출
            yield return LerpRealPos(playerUnit.transform, playerUnit.transform.position, map.Pos(next), stepDuration);

            // 3) Pass/Arrive 콜백
            bool isLast = (s == steps - 1);
            if (!isLast) OnTilePass?.Invoke(CurrentIndex);
            else OnTileArrive?.Invoke(CurrentIndex);
        }

        // playerUnit.Mover.IsMoving = false;
    }

    private IEnumerator LerpRealPos(Transform target, Vector3 from, Vector3 to, float dur)
    {
        float t = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;
            float k = ease.Evaluate(Mathf.Clamp01(t / dur));
            target.transform.position = Vector3.LerpUnclamped(from, to, k);
            yield return null;
        }

        playerUnit.transform.position = to; // 스냅 마감
    }

    // 텔레포트 등으로 즉시 인덱스/좌표 바꿔야 할 때
    public void TeleportTo(int index, bool snap = true)
    {
        CurrentIndex = Mathf.Clamp(index, 0, map.Count - 1);
        if (snap) playerUnit.Mover.SnapTo(map.Pos(CurrentIndex));
    }
}
