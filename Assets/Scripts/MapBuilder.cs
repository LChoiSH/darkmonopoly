using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] private Tile tile;

    public List<Tile> madeTiles;
    public int mapSize = 8;
    public float gap = 0.2f;
    public int Count => madeTiles.Count;
    public Vector3 Pos(int index) => madeTiles[index % Count].transform.position;
    public int Next(int index) => (index + 1) % Count;
    public int Prev(int index) => index == 0 ? Count - 1 : index + 1;
    public Tile Node(int index) => madeTiles[index % Count];

    [Button]
    public void MakeMap()
    {
        if (madeTiles != null && madeTiles.Count > 0)
        {
            for (int i = 0; i < madeTiles.Count; i++)
            {
                DestroyImmediate(madeTiles[i].gameObject);
            }

            madeTiles.Clear();
        }

        // 1) 그리드 경로(정수 좌표, 보드 좌측하단을 (0,0)으로) 생성
        var path = MakePath(mapSize);

        // 2) 그리드 → 월드 좌표 변환해서 프리팹 배치
        foreach (var grid in path)
        {
            Vector2 worldPos = GridToWorldOriginCentered(grid, mapSize, 1 + gap);
            Tile madeTile = Instantiate(tile, worldPos, Quaternion.identity, this.transform);
            // go.name = $"Tile_{orderedTiles.Count:D2}_({grid.x},{grid.y})";
            madeTiles.Add(madeTile);
        }
    }

    public static List<Vector2Int> MakePath(int n)
    {
        var path = new List<Vector2Int>(4 * n - 4);
        if (n < 3) return path;

        int max = n - 1;

        // 1) 하단 행: (max,0) → (0,0)
        for (int x = max; x >= 0; x--)
            path.Add(new Vector2Int(x, 0));

        // 2) 좌측 열: (0,1) → (0,max)
        for (int y = 1; y <= max; y++)
            path.Add(new Vector2Int(0, y));

        // 3) 상단 행: (1,max) → (max,max)
        for (int x = 1; x <= max; x++)
            path.Add(new Vector2Int(x, max));

        // 4) 우측 열: (max,max-1) → (max,1)
        for (int y = max - 1; y >= 1; y--)
            path.Add(new Vector2Int(max, y));

        return path;
    }

    public static Vector2 GridToWorldOriginCentered(Vector2Int grid, int n, float stride)
    {
        float half = (n - 1) * 0.5f;               // 중앙 정렬 오프셋
        float worldX = (grid.x - half) * stride;   // +0: 원점 기준
        float worldY = (grid.y - half) * stride;
        return new Vector2(worldX, worldY);
    }

    public List<int> GetRandomTiles(int tileCount)
    {
        if (tileCount >= madeTiles.Count)
        {
            List<int> result = new();

            tileCount = madeTiles.Count;

            for (int i = 0; i < tileCount; i++)
            {
                result.Add(i);
            }

            return result;
        }

        System.Random rng = new System.Random();
        var chosen = new HashSet<int>(tileCount);

        for (int t = Count - tileCount; t < Count; t++)
        {
            int r = rng.Next(0, t + 1);
            if (!chosen.Add(r)) chosen.Add(t);
        }

        return chosen.ToList();
    }

    // public List<Tile> GetRandomTiles(int tileCount)
    // {
    //     if (tileCount >= madeTiles.Count) return madeTiles;

    //     System.Random rng = new System.Random();
    //     var chosen = new HashSet<int>(tileCount);

    //     for (int t = Count - tileCount; t < Count; t++)
    //     {
    //         int r = rng.Next(0, t + 1);
    //         if (!chosen.Add(r)) chosen.Add(t);
    //     }

    //     List<Tile> result = new();

    //     foreach (int x in chosen)
    //     {
    //         result.Add(madeTiles[x]);
    //     }

    //     return result;
    // }

    // === 유틸: 순환 인덱스 ===
    public static int NextIndex(int idx, int count) => (idx + 1) % count;
    public static int PrevIndex(int idx, int count) => (idx - 1 + count) % count;
}
