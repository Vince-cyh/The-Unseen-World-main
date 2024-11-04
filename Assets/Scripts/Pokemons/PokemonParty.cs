using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons => pokemons;  //用来供其他脚本访问宝可梦池
    private void Start()
    {
        foreach(var pokemon in pokemons)
        {
            pokemon.Init();
        }   
    }
    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }
}
