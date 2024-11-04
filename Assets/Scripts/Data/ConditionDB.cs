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
                Name = "�ж�",
                StartMessage = "�ж��ˣ�",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}�����ж���Ѫ");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "����",
                StartMessage = "�����ˣ�",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}�������˿�Ѫ");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "���",
                StartMessage = "������ˣ�",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if  (Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}��������޷�����");
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
                Name = "����",
                StartMessage = "�������ˣ�",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if  (Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}���ٱ�����");
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
                Name = "ҡ����",
                StartMessage = "�����ξ�",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.StatusTime = Random.Range(1, 4);
                    Debug.Log($"������{pokemon.StatusTime}���غ��޷��ж�");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}������!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon._base.Name}����˯��");
                    return false;
                }
            }
        },
        {
            ConditionID.reset,
            new Condition()
            {
                Name = "����",
                StartMessage = "�����������״̬"
            }
        }
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz, reset
}   // �� �� ˯ ̱�� ���� ����
