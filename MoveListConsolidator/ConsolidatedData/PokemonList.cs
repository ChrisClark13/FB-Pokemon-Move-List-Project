using System;
using System.Collections.Generic;
using System.Linq;
using MoveListConsolidator.RawData;

namespace MoveListConsolidator.ConsolidatedData
{
    public class PokemonList
    {
        public List<Pokemon> Pokemon = new List<Pokemon>();

        public void InitFromBaseList(BasePokemonList list)
        {
            list.Pokemon.ForEach(basePokemon =>
            {
                var pokemon = new Pokemon();
                pokemon.Name = basePokemon.Name;
                pokemon.DexNum = basePokemon.DexNum;
                Pokemon.Add(pokemon);
            });
        }

        public void ParseAltFormData(AlternateFormList list)
        {
            var alolaNums = list.AlolaForms.Select(alolaForm => alolaForm.DexNum);
            Pokemon.FindAll(pokemon => alolaNums.Contains(pokemon.DexNum)).ForEach(pokemon => pokemon.AltForms.Add("Alola"));

            var galarianNums = list.GalarianForms.Select(galarianForm => galarianForm.DexNum);
            Pokemon.FindAll(pokemon => galarianNums.Contains(pokemon.DexNum)).ForEach(pokemon => pokemon.AltForms.Add("Galarian"));

            list.AltForms.ForEach(altForm =>
            {
                var pokemon = Pokemon.Find(pokemon => altForm.DexNum == pokemon.DexNum);
                pokemon.DefaultForm = altForm.DefaultForm;
                pokemon.AltForms.AddRange(altForm.AltForms);
            });
        }

        public void ParseVeekunPokemonList(VeekunPokemonList veekunPokemonList)
        {
            veekunPokemonList.Pokemon.ForEach(vp => Pokemon.Find(p => vp.DexNumValue == p.DexNum).ParseVeekunPokemon(vp));
        }

        public void ParseSerebiiPokemonList(SerebiiPokemonList serebiiPokemonList)
        {
            serebiiPokemonList.Pokemon.ForEach(sp => Pokemon.Find(p => sp.DexNumValue == p.DexNum).ParseSerebiiPokemon(sp));
        }

        public void ParseAltFormsWithOneAdditionalMoveList(List<AlternateFormList.AltFormWithSingleAdditionalMove> list)
        {
            list.ForEach(af => Pokemon.Find(p => af.DexNum == p.DexNum).ParseAltFormWithOneAdditionalMove(af));
        }

        public void ParseEvolutionMissingMoves(EvolutionList list)
        {
            list.Evolutions.ForEach(e => {
                var pokemon = Pokemon.Find(p => p.DexNum == e.DexNum && (string.IsNullOrEmpty(e.Form) || e.Form == p.DefaultForm || p.AltForms.Contains(e.Form)));
                var form = string.IsNullOrEmpty(e.Form) ? pokemon.DefaultForm : e.Form;
                
                var evolvedPokemon = Pokemon.Find(p => p.DexNum == e.EvolvesIntoDexNum && (string.IsNullOrEmpty(e.EvolvesIntoForm) || e.EvolvesIntoForm == p.DefaultForm || p.AltForms.Contains(e.EvolvesIntoForm)));
                var evolvedForm = string.IsNullOrEmpty(e.EvolvesIntoForm) ? evolvedPokemon.DefaultForm : e.EvolvesIntoForm;

                evolvedPokemon.ProcessPreEvolvedStageMoves(pokemon, evolvedForm, form);
            });
        }
    }
}