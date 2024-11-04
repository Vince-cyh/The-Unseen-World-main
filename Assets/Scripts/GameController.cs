using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] NPCController npcController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] RPGUI rpgUI;

    private SoundManager soundManager;

    GameState state;

    private void Start()
    {
        GameObject soundManagerObject = GameObject.Find("SoundManager");
        if (soundManagerObject != null)
        {
            soundManager = soundManagerObject.GetComponent<SoundManager>();
        }

        playerController.OnEncountered += StartBattle;
        npcController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }

    void StartBattle(Collider2D collider)
    {
        soundManager.StopMusic();
        soundManager.PlayBattleMusic();
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        rpgUI.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = collider.gameObject.GetComponent<MapArea>().GetRandomWildPokemon();

        battleSystem.StartBattle(playerParty, wildPokemon);
    }

    void EndBattle(bool won)
    {
        soundManager.StopBattleMusic();
        soundManager.PlayMusic();
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        rpgUI.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state== GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }
}
