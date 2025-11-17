using UnityEngine;
using UnitSystem;
using VInspector;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Refs")]
    public MapBuilder map;
    public Unit player;
    public MoveSystem moveSystem;

    [Header("Start")]
    public int startIndex = 0; // 우하단 타일 인덱스

    public TurnSystem turnSystem;
    public ManaSystem manaSystem;
    public CardController cardController;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        moveSystem.Init(map, player, startIndex);
        moveSystem.OnTilePass += HandlePass;
        moveSystem.OnTileArrive += HandleArrive;

        turnSystem = new TurnSystem();

        // turnSystem.onPrepare ;
        // turnSystem.onPlayerStart += () => manaSystem.SetMana(manaSystem.Mana + 1);
        // manaSystem.SetMana(manaSystem.Mana);
        manaSystem.Init();

        cardController.ResetDrawPileFromDeck();

        turnSystem.onPlayerStart += () => cardController.DrawCards(3);
        turnSystem.onPlayerStart += () => manaSystem.ManaFill(manaSystem.Mana);
        turnSystem.onPlayerEnd += () => cardController.DiscardAllCard();

        GameStart();
    }
    
    public void GameStart()
    {
        turnSystem.Init();
    }

    // 외부(주사위/버튼)에서 호출
    public void RequestMove(int steps) => moveSystem.MoveSteps(steps);

    private void HandlePass(int index)
    {
        var tile = map.Node(index);
        // switch (tile.type)
        // {
        //     case TileType.Go:
        //         Debug.Log($"[PASS] {tile.displayName} (+{tile.amount})");
        //         // Bank += tile.amount;
        //         break;
        //     case TileType.Tax:
        //         // 통과형 세금 정책이 있으면 처리
        //         break;
        // }
    }

    private void HandleArrive(int index)
    {
        var tile = map.Node(index);
        // switch (tile.type)
        // {
        //     case TileType.Normal:
        //         Debug.Log($"[ARRIVE] {tile.displayName}");
        //         break;

        //     case TileType.Tax:
        //         Debug.Log($"[ARRIVE] TAX {tile.amount}");
        //         // Bank -= tile.amount;
        //         break;

        //     case TileType.Teleport:
        //         if (tile.targetIndex >= 0 && tile.targetIndex < map.Count)
        //         {
        //             Debug.Log($"[ARRIVE] TELEPORT -> {tile.targetIndex}");
        //             moveSystem.TeleportTo(tile.targetIndex, snap:true);
        //             // 텔레포트 후 추가 규칙 있으면 여기서
        //         }
        //         break;

        //     case TileType.Go:
        //         Debug.Log($"[ARRIVE] GO");
        //         break;

        //     case TileType.Custom:
        //         // 프로젝트 맞춤 처리 지점
        //         break;
        // }
    }

    // 데모
    [ContextMenu("Test Move 7"), Button]
    private void TestMove() => RequestMove(7);
}
