using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnitSystem;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class UnitManager : MonoBehaviour
{
    private static UnitManager _instance;
    public static UnitManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("UnitManager");
                _instance = obj.AddComponent<UnitManager>();
            }
            return _instance;
        }
    }

    [SerializeField] private UnitFactory unitFactory;
    // TODO: Remove
    [SerializeField] private RectTransform unitUIWrap;
    [SerializeField] private HpBar hpBar;

    private Dictionary<int, List<Unit>> madeUnits = new Dictionary<int, List<Unit>>();
    private Dictionary<int, Unit> runtimeIdDic = new();

    public IEnumerable<Unit> MadeUnits => madeUnits.Values.SelectMany(unitList => unitList);
    public IEnumerable<Unit> GetTeamUnits(int teamNumber) => madeUnits.TryGetValue(teamNumber, out var teamUnits) ? teamUnits : null;
    public IEnumerable<Unit> GetEnemyUnits(int teamNumber) => madeUnits.Where(pair => pair.Key != teamNumber).SelectMany(pair => pair.Value);
    public List<Unit> GetUnitsById(int teamNumber, string unitId) => madeUnits.TryGetValue(teamNumber, out var teamUnits) ? teamUnits.FindAll(unit => unit.Id == unitId) : null;
    public Unit GetUnitByRuntimeId(string runtimeId) => GetUnitByRuntimeId(int.Parse(runtimeId));
    public Unit GetUnitByRuntimeId(int runtimeId) => runtimeIdDic.TryGetValue(runtimeId, out Unit target) ? target : null;
    public int TeamCount(int team) => madeUnits.ContainsKey(team) ? madeUnits[team].Count : 0;
    public UnitFactory UnitFactory => unitFactory;

    public event Action<Unit> onUnitRegister;
    public event Action<Unit> onUnitUnregister;
    public event Action<int> onTeamCountChanged;

    private void Awake()
    {
        // 중복 방지
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(Unit unit)
    {
        if (!madeUnits.ContainsKey(unit.Team)) madeUnits[unit.Team] = new List<Unit>();
        if (madeUnits[unit.Team].Contains(unit)) return;

        madeUnits[unit.Team].Add(unit);
        runtimeIdDic[unit.GetInstanceID()] = unit;

        // TODO: remove
        HpBar madeHpBar = Instantiate(hpBar, unitUIWrap);
        madeHpBar.SetDefender(unit.Defender);

        onUnitRegister?.Invoke(unit);
        onTeamCountChanged?.Invoke(unit.Team);
    }

    public void Unregister(Unit unit)
    {
        if (!madeUnits.ContainsKey(unit.Team)) return;

        madeUnits[unit.Team].Remove(unit);
        runtimeIdDic.Remove(unit.GetInstanceID());

        onUnitUnregister?.Invoke(unit);
        onTeamCountChanged?.Invoke(unit.Team);
    }

#if UNITY_EDITOR
    public async void ReadCSV()
    {
        const string UnitPath = "Assets/CSV/Unit.csv";

        var unitCSV = await CSVReader.ReadByAddressablePathAsync(UnitPath);

        if (unitCSV == null)
        {
            Debug.LogError("There are not csv");
            return;
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        Debug.Log($"Stage Read Success");
    }
#endif
}
    