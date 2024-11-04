using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase Base;
    [SerializeField] int level;

    public PokemonBase _base{ 
        get{
            return Base;
        }
    }
    public int Level {
        get{
            return level;
        }
        set
        {
            level = value;
            OnLevelUp?.Invoke(); // 触发等级提升事件
        }
    }
    public event Action OnLevelUp;

    public bool isPlayerUnit { get; set; }

    public int Exp { get; set; }
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }

    // 使用字典 这样就可以方便访问内部属性了
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public bool HpChanged { get; set; }

    public void Init()
    {       // 生成招式来学习
            Moves = new List<Move>();
            foreach(var move in _base.LearnableMoves)
            {
                if (move.Level <= Level)
                {    
                   Moves.Add(new Move(move.Base));
                }
                
                if(Moves.Count >= 4)
                    break;
            }

        CalculateStats();
        if (isPlayerUnit)
        {
            HP = PlayerPrefs.GetInt("HP");
        }
        else
        {
            HP = MaxHp;
        }
        ResetStatBoost();
        
        Exp = Base.GetExpForLevel(Level);

        
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
        };
    }
    
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}的{stat}提升!");
            else
                StatusChanges.Enqueue($"{Base.Name}的{stat}削弱!");

            Debug.Log($"{stat} 被修改到 {StatBoosts[stat]}");
        }
    }

    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++Level;
            HP = MaxHp;
            PlayerPrefs.SetInt("HP", MaxHp);
            HpChanged = true;
            SoundManager.PlayLevelUp();
            return true;
        }

        return false;
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed
    {
        get{
            return GetStat(Stat.Speed);}
    }

    public int MaxHp 
    {
        get { return Mathf.FloorToInt((_base.MaxHp * Level) / 100f) + 10; }
    }

    public DamageDetails TakeDamage(Move move,Pokemon attacker)
    {
        float critical = 1f;// 暴击
        if (UnityEngine.Random.value * 100f <= 10.25f)
            critical = 2f;

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

        float modifiers = UnityEngine.Random.Range(0.85f, 1f);
        float a =(2*attacker.Level +10)/250f;
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        if (move.Base.Power == 0) damage=0;

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false,
            Damage = damage
        };

        UpdateHP(damage);

        if (isPlayerUnit)
        {
            PlayerPrefs.SetInt("HP", HP);
        }
        
        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HpChanged = true;
    }

    public void SetStatus(ConditionID conditionId)
    {
        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}{Status.StartMessage}");
    }

    public void CureStatus()
    {
        Status = null;
    }

    public Move GetRandomMove()
    {
        int r = UnityEngine.Random.Range(0, Moves.Count); // 使用 UnityEngine.Random
        return Moves[r];
    }

    public bool OnBeforeMove()
    {
        if (Status?.OnBeforeMove != null)
        {
            return Status.OnBeforeMove(this);
        }

        return true;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
    public int Damage { get; set; }
}
