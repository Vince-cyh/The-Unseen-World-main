using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RPGUI : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;
    private PokemonParty pokemonParty;

    // Start is called before the first frame update
    void Start()
    {
        pokemonParty = GetComponentInParent<PokemonParty>();

        if (pokemonParty == null)
        {
            Debug.LogError("PokemonParty is not found in parent!");
            return;
        }

        if (pokemonParty.Pokemons.Count > 0)
        {
            var firstPokemon = pokemonParty.Pokemons[0];
            if (firstPokemon != null)
            {
                
                PlayerPrefs.SetInt("HP", firstPokemon.MaxHp);

                firstPokemon.HP = PlayerPrefs.GetInt("HP");
                playerUnit.Pokemon = firstPokemon;
                playerHud.SetData(firstPokemon);
            }
            else
            {
                Debug.LogError("First Pokemon in the party is null!");
            }
        }
        else
        {
            Debug.LogError("PokemonParty has no Pokemons!");
        }
    }

    void Update()
    {
        var firstPokemon = pokemonParty.Pokemons[0];
        if (firstPokemon == null)
        {
            Debug.LogError("First Pokemon in the party is null in Update!");
            return;
        }

        if (playerHud == null)
        {
            Debug.LogError("PlayerHud is not assigned in Update!");
            return;
        }

        // 获取HP值并更新firstPokemon的HP
        PlayerPrefs.SetInt("HP", firstPokemon.HP);
        int HP = PlayerPrefs.GetInt("HP");
        firstPokemon.HP = HP;

        // 确保HUD更新
        playerHud.SetData(firstPokemon);
        StartCoroutine(playerHud.UpdateHP());

        // 判断游戏结束
        if(firstPokemon.HP <= 0)
        {
            SceneManager.LoadScene(6);
        }
    }

    public bool Healing(int Heal)
    {
        var firstPokemon = pokemonParty.Pokemons[0];

        int HP = PlayerPrefs.GetInt("HP");
        if (HP < firstPokemon.MaxHp)
        {
            HP = Mathf.Clamp(HP + Heal, 0, firstPokemon.MaxHp);
            PlayerPrefs.SetInt("HP", HP);
            firstPokemon.HP = HP;
            if (playerHud != null)
            {
                playerHud.SetData(firstPokemon);
                StartCoroutine(playerHud.UpdateHP());
            }
            else
            {
                Debug.LogError("PlayerHud is not assigned in Healing!");
            }
            return true;
        }
        return false;
    }

    public bool Injuring(int Injure)
    {
        var firstPokemon = pokemonParty.Pokemons[0];
        int HP = PlayerPrefs.GetInt("HP");
        if (HP > 0)
        {
            HP = Mathf.Clamp(HP - Injure, 0, firstPokemon.MaxHp);
            PlayerPrefs.SetInt("HP", HP);
            firstPokemon.HP = HP;
            if (playerHud != null)
            {
                StartCoroutine(playerHud.UpdateHP());
            }
            else
            {
                Debug.LogError("PlayerHud is not assigned in Injuring!");
            }
            return true;
        }
        return false;
    }
}