using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;

    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    PokemonParty playerParty;
    Pokemon wildPokemon;

    int escapeAttempts;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);

        partyScreen.Init();
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"һ��Ұ����{enemyUnit.Pokemon._base.Name}�����ˡ�");

        ChooseFirstTurn();
    }

    void ChooseFirstTurn()
    {
        if (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed)
            ActionSelection();
        else
            StartCoroutine(EnemyMove());
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("����һ��ѡ��");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;
      
        var move = playerUnit.Pokemon.Moves[currentMove];

        yield return RunMove(playerUnit, enemyUnit, move);
        
        // ���״̬û�б�runmove�޸ģ��Ϳ��Ե����˻غ�
        if (state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }
   
    // �ȱ�����ҵĲ��֣���ֹbug
    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;
        
        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);
        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);

        move.PP++;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon._base.Name}ʹ����{move.Base.Name}");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        targetUnit.PlayHitAnimation();

        if (move.Base.Category == MoveCategory.Status)
        {
            var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
            yield return targetUnit.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);

            yield return RunMoveEffects(move, sourceUnit.Pokemon, targetUnit.Pokemon);
        }
        else
        {
            var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
            yield return targetUnit.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        if (targetUnit.Pokemon.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Pokemon._base.Name}�赹��");
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);

            yield return(CheckForBattleOver(sourceUnit, targetUnit));
        }

        // ״̬������Ҫ
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon._base.Name}�赹��");
            sourceUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);

            yield return (CheckForBattleOver(targetUnit, sourceUnit));
        }
    }

    IEnumerator RunMoveEffects(Move move, Pokemon source, Pokemon target)
    {
        var effects = move.Base.Effects;
        if (effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }
        if (effects.Status == ConditionID.reset)
        {
            source.SetStatus(effects.Status);
        }
        else if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator CheckForBattleOver(BattleUnit sourceUnit ,BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            if (playerParty.Pokemons[0].HP <= 0)
            {
                BattleOver(false);
            }
            else
            {
                var nextPokemon = playerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    OpenPartyScreen();
                else
                    BattleOver(false);
            }
        }
        else
        {
            // ս��ʤ�� �ҷ�������ɫ����״̬�����ֹʤ������
            sourceUnit.Pokemon.CureStatus();
            // ��þ���
            int expYield = faintedUnit.Pokemon._base.ExpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;

            // ϵ����Ĭ��Ϊ1,�����ٵ�����
            float trainerBonus = 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            sourceUnit.Pokemon.Exp += expGain;
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon._base.Name} ����� {expGain} exp");
            yield return sourceUnit.Hud.SetExpSmooth();

            // ���ȼ�����
            while (sourceUnit.Pokemon.CheckForLevelUp())
            {
                if(sourceUnit.Pokemon == playerParty.Pokemons[0])
                {
                    LightManager.AddFalloff(1);
                }
                sourceUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon._base.Name} �������ȼ� {sourceUnit.Pokemon.Level}��");

                yield return sourceUnit.Hud.SetExpSmooth(true);
                yield return sourceUnit.Hud.UpdateHP();
            }
            if(sourceUnit.Pokemon != playerParty.Pokemons[0])
            {
                yield return StartCoroutine(SwitchPokemon(playerParty.Pokemons[0]));
                playerUnit.Pokemon.Exp += expGain;
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon._base.Name} ����� {expGain} exp");
                yield return playerUnit.Hud.SetExpSmooth();

                // ���ȼ�����
                while (playerUnit.Pokemon.CheckForLevelUp())
                {
                    LightManager.AddFalloff(1);
                    playerUnit.Hud.SetLevel();
                    yield return dialogBox.TypeDialog($"{playerUnit.Pokemon._base.Name} �������ȼ� {playerUnit.Pokemon.Level}��");

                    yield return playerUnit.Hud.SetExpSmooth(true);
                    yield return playerUnit.Hud.UpdateHP();
                }
            }
            BattleOver(true);
        }      
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f && damageDetails.Damage > 0)
            yield return dialogBox.TypeDialog("��һ�ι��������˱���Ч����");
    }

    public void HandleUpdate()
    {
        if(state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
            }
            else if (currentAction == 2)
            {
                // Switch
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Run,���ﻹ�����ƣ�û���ٶȻ���
                BattleOver(false);
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("�㲻���л��赹�Ľ�ɫ");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("�㲻���л�ͬһ����ɫ");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        bool currentPokemonFainted = true;
        if (playerUnit.Pokemon.HP > 0)
        {
            currentPokemonFainted = false;
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon._base.Name}��");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"{newPokemon._base.Name}����!");
        yield return new WaitForSeconds(1f);
        if (currentPokemonFainted)
            ChooseFirstTurn();
        else
        {
            if(enemyUnit.Pokemon.HP > 0)
            {
                StartCoroutine(EnemyMove());
            }
            else
            {

            }
        }
    }
}
