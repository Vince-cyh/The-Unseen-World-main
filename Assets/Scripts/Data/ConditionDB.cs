using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "中毒",
                StartMessage = "中毒了！",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}由于中毒扣血");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "烧伤",
                StartMessage = "烧伤了！",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}由于烧伤扣血");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "麻痹",
                StartMessage = "被麻痹了！",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if  (Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}被麻痹了无法动弹");
                        return false;
                    }

                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "冻结",
                StartMessage = "被冻结了！",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if  (Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}不再被冻结");
                        return true;
                    }

                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "摇篮曲",
                StartMessage = "陷入梦境",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.StatusTime = Random.Range(1, 4);
                    Debug.Log($"将会在{pokemon.StatusTime}个回合无法行动");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}苏醒了!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}陷入睡眠");
                    return false;
                }
            }
        },
        {
            ConditionID.reset,
            new Condition()
            {
                Name = "净化",
                StartMessage = "净化了自身的状态"
            }
        }
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz, reset
}   // 毒 烧 睡 瘫痪 冻结 净化
