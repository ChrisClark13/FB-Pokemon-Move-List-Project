using System;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using MoveListConsolidator.RawData;
using MoveListConsolidator.ConsolidatedData;

namespace MoveListConsolidator
{
    class Program
    {
        #if DEBUG
            public static string PathToFiles = ".";
        #else
            public static string PathToFiles = "";
        #endif

        static void Main(string[] args)
        {
            Console.WriteLine($"Hewwo Fizztopia, FILES ARE AT {System.IO.Path.GetFullPath(PathToFiles)}");
            var pokemonList = new PokemonList();
            
            var basePokemonList = BasePokemonList.ParseJson(File.ReadAllText($@"{PathToFiles}\helperData\BasePokemonList.json"));
            var alternateFormList = AlternateFormList.ParseJson(File.ReadAllText($@"{PathToFiles}\helperData\AlternateFormList.json"));
            var evolutionList = EvolutionList.ParseJson(File.ReadAllText($@"{PathToFiles}\helperData\EvolutionList.json"));

            pokemonList.InitFromBaseList(basePokemonList);
            pokemonList.ParseAltFormData(alternateFormList);

            // Console.WriteLine(JsonConvert.SerializeObject(pokemonList, Formatting.Indented));

            Console.WriteLine("Processing Veekun Files...");
            foreach(var filePath in Directory.GetFiles($@"{PathToFiles}\rawData","veekun*"))
            {  
                Console.WriteLine(filePath);
                pokemonList.ParseVeekunPokemonList(VeekunPokemonList.ParseJson(File.ReadAllText(filePath)));
            }

            Console.WriteLine("Processing Serebii Files...");
            foreach(var filePath in Directory.GetFiles($@"{PathToFiles}\rawData","serebii*"))
            {
                Console.WriteLine(filePath);
                pokemonList.ParseSerebiiPokemonList(SerebiiPokemonList.ParseJson(File.ReadAllText(filePath)));
            }

            Console.WriteLine("Processing Missing Evolution Moves");
            pokemonList.ParseEvolutionMissingMoves(evolutionList);
            pokemonList.ParseEvolutionMissingMoves(evolutionList);

            Console.WriteLine("Processing Alt Forms with One Additional Move");
            pokemonList.ParseAltFormsWithOneAdditionalMoveList(alternateFormList.AltFormsWithSingleAdditionalMove);

            Directory.CreateDirectory($@"{PathToFiles}\output");
            File.WriteAllText($@"{PathToFiles}\output\pokemonMoveList.json",JsonConvert.SerializeObject(pokemonList, Formatting.Indented));
        }
    }
}
