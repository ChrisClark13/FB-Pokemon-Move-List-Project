using System;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using MoveListConsolidator.RawData;
using MoveListConsolidator.ConsolidatedData;
using System.Collections.Generic;

namespace MoveListConsolidator
{
    class Program
    {
#if DEBUG
        public static string PathToFiles = ".";
#else
            public static string PathToFiles = ".";
#endif

        static void Main(string[] args)
        {
            Console.WriteLine($"Hewwo Fizztopia, FILES ARE AT {System.IO.Path.GetFullPath(PathToFiles)}");

            // var logPath = $@"{PathToFiles}\output\log.txt";
            // Console.WriteLine($@"Outputting log to {logPath}");
            // FileStream logStream = new FileStream(logPath, FileMode.OpenOrCreate, FileAccess.Write);
            // StreamWriter logWriter = new StreamWriter(logStream);
            // Console.SetOut(logWriter);

            var pokemonList = new PokemonList();

            var basePokemonList = BasePokemonList.ParseJson(File.ReadAllText($@"{PathToFiles}\helperData\BasePokemonList.json"));
            var alternateFormList = AlternateFormList.ParseJson(File.ReadAllText($@"{PathToFiles}\helperData\AlternateFormList.json"));
            var evolutionList = EvolutionList.ParseJson(File.ReadAllText($@"{PathToFiles}\helperData\EvolutionList.json"));
            var spellingFixes = SpellingFixes.ParseJson(File.ReadAllText($@"{PathToFiles}\helperData\SpellingFixes.json"));

            pokemonList.InitFromBaseList(basePokemonList);
            pokemonList.ParseAltFormData(alternateFormList);

            // Console.WriteLine(JsonConvert.SerializeObject(pokemonList, Formatting.Indented));

            Console.WriteLine("Processing Veekun Files...");
            foreach (var filePath in Directory.GetFiles($@"{PathToFiles}\rawData", "veekun*.json"))
            {
                Console.WriteLine(filePath);

                var veekunPokemon = VeekunPokemonList.ParseJson(File.ReadAllText(filePath));

                Console.WriteLine("Processing Spelling Fixes");
                foreach (var moveFix in spellingFixes.MoveFixes)
                {
                    veekunPokemon.Pokemon.ForEach(p =>
                    {
                        if (p.LevelUpMoves != null)
                            p.LevelUpMoves.FindAll(
                                    m => m.Name.Equals(moveFix.OldName, StringComparison.InvariantCultureIgnoreCase)
                                ).ToList().ForEach(m => { Console.WriteLine($"{p.DexNum}: Fixing {m.Name} to {moveFix.NewName}"); m.Name = moveFix.NewName; });

                        if (p.EggMoves != null)
                            p.EggMoves.FindAll(
                                    m => m.Name.Equals(moveFix.OldName, StringComparison.InvariantCultureIgnoreCase)
                                ).ToList().ForEach(m => { Console.WriteLine($"{p.DexNum}: Fixing {m.Name} to {moveFix.NewName}"); m.Name = moveFix.NewName; });

                        if (p.TutorMoves != null)
                            p.TutorMoves.FindAll(
                                    m => m.Name.Equals(moveFix.OldName, StringComparison.InvariantCultureIgnoreCase)
                                ).ToList().ForEach(m => { Console.WriteLine($"{p.DexNum}: Fixing {m.Name} to {moveFix.NewName}"); m.Name = moveFix.NewName; });

                        if (p.MachineMoves != null)
                            p.MachineMoves.FindAll(
                                    m => m.Name.Equals(moveFix.OldName, StringComparison.InvariantCultureIgnoreCase)
                                ).ToList().ForEach(m => { Console.WriteLine($"{p.DexNum}: Fixing {m.Name} to {moveFix.NewName}"); m.Name = moveFix.NewName; });
                    });
                }
                pokemonList.ParseVeekunPokemonList(veekunPokemon, filePath);
            }

            Console.WriteLine("Processing Serebii Files...");
            foreach (var filePath in Directory.GetFiles($@"{PathToFiles}\rawData", "serebii*.json"))
            {
                Console.WriteLine(filePath);
                Console.WriteLine("Processing Spelling Fixes");

                var serebiiPokemon = SerebiiPokemonList.ParseJson(File.ReadAllText(filePath));

                foreach (var moveFix in spellingFixes.MoveFixes)
                {
                    serebiiPokemon.Pokemon.ForEach(p =>
                    {
                        if (p.LevelUpMoves != null)
                            p.LevelUpMoves.FindAll(
                                    m => m.Name.Equals(moveFix.OldName, StringComparison.InvariantCultureIgnoreCase)
                                ).ToList().ForEach(m => { Console.WriteLine($"{p.DexNum}: Fixing {m.Name} to {moveFix.NewName}"); m.Name = moveFix.NewName; });

                        if (p.AlolanFormLevelUpMoves != null)
                            p.AlolanFormLevelUpMoves.FindAll(
                                    m => m.Name.Equals(moveFix.OldName, StringComparison.InvariantCultureIgnoreCase)
                                ).ToList().ForEach(m => { Console.WriteLine($"{p.DexNum}: Fixing {m.Name} to {moveFix.NewName}"); m.Name = moveFix.NewName; });

                        if (p.GalarianFormLevelUpMoves != null)
                            p.GalarianFormLevelUpMoves.FindAll(
                                    m => m.Name.Equals(moveFix.OldName, StringComparison.InvariantCultureIgnoreCase)
                                ).ToList().ForEach(m => { Console.WriteLine($"{p.DexNum}: Fixing {m.Name} to {moveFix.NewName}"); m.Name = moveFix.NewName; });

                        if (p.AltForms != null)
                            p.AltForms.ForEach(altForm => altForm.LevelUpMoves.FindAll(
                                    m => m.Name.Equals(moveFix.OldName, StringComparison.InvariantCultureIgnoreCase)
                                ).ToList().ForEach(m => { Console.WriteLine($"{p.DexNum}: Fixing {m.Name} to {moveFix.NewName}"); m.Name = moveFix.NewName; }));

                        if (p.EggMoves != null)
                            p.EggMoves.FindAll(
                                    m => m.Name.Equals(moveFix.OldName, StringComparison.InvariantCultureIgnoreCase)
                                ).ToList().ForEach(m => { Console.WriteLine($"{p.DexNum}: Fixing {m.Name} to {moveFix.NewName}"); m.Name = moveFix.NewName; });

                        if (p.TutorMoves != null)
                            p.TutorMoves.FindAll(
                                    m => m.Name.Equals(moveFix.OldName, StringComparison.InvariantCultureIgnoreCase)
                                ).ToList().ForEach(m => { Console.WriteLine($"{p.DexNum}: Fixing {m.Name} to {moveFix.NewName}"); m.Name = moveFix.NewName; });

                        if (p.MachineMoves != null)
                            p.MachineMoves.FindAll(
                                    m => m.Name.Equals(moveFix.OldName, StringComparison.InvariantCultureIgnoreCase)
                                ).ToList().ForEach(m => { Console.WriteLine($"{p.DexNum}: Fixing {m.Name} to {moveFix.NewName}"); m.Name = moveFix.NewName; });
                    });
                }

                var isSwSh = filePath.Contains("swsh");
                pokemonList.ParseSerebiiPokemonList(serebiiPokemon, isSwSh, filePath);
            }

            Console.WriteLine("Processing Form Name Fixes");
            {
                Dictionary<string, int> fixCount = new Dictionary<string, int>();
                spellingFixes.FormFixes.ForEach(fix => fixCount[fix.NewName] = 0);

                pokemonList.Pokemon.ForEach(p =>
                {
                    var forms = new List<string> { p.DefaultForm };
                    forms.AddRange(p.AltForms);
                    foreach (var fix in forms.Join(spellingFixes.FormFixes.Where(fix => fix.DexNum == 0 || fix.DexNum == p.DexNum), f => f, fix => fix.OldName, (f, fix) => fix))
                    {
                        Console.WriteLine($"Fixing {fix.OldName} to {fix.NewName} in {p.Name}");
                        fixCount[fix.NewName]++;
                        {
                            var index = p.AltForms.FindIndex(f => f == fix.OldName);
                            if (index != -1)
                                p.AltForms[index] = fix.NewName;
                            else
                                p.DefaultForm = fix.NewName;
                        }
                        p.LevelUpMoveLists.Find(l => l.Form == fix.OldName).Form = fix.NewName;

                        p.EggMoves.ForEach(m =>
                        {
                            var index = m.Forms.FindIndex(f => f == fix.OldName);
                            if (index != -1)
                                m.Forms[index] = fix.NewName;
                        });
                        p.TutorMoves.ForEach(m =>
                        {
                            var index = m.Forms.FindIndex(f => f == fix.OldName);
                            if (index != -1)
                                m.Forms[index] = fix.NewName;
                        });
                        p.MachineMoves.ForEach(m =>
                        {
                            var index = m.Forms.FindIndex(f => f == fix.OldName);
                            if (index != -1)
                                m.Forms[index] = fix.NewName;
                        });
                    }
                });

                foreach(var (k, v) in fixCount) {
                    Console.WriteLine($"Fixed {k} {v} time(s)");
                }
            }

            Console.WriteLine("Processing Missing Evolution Moves");
            pokemonList.ParseEvolutionMissingMoves(evolutionList);
            Console.WriteLine("Processing Missing Evolution Moves AGAIN (Should be no output)");
            pokemonList.ParseEvolutionMissingMoves(evolutionList);

            Console.WriteLine("Processing Alt Forms with One Additional Move");
            pokemonList.ParseAltFormsWithOneAdditionalMoveList(alternateFormList.AltFormsWithSingleAdditionalMove);

            Console.WriteLine($@"Placing output in {PathToFiles}\output\pokemonMoveList.json");
            Directory.CreateDirectory($@"{PathToFiles}\output");
            File.WriteAllText($@"{PathToFiles}\output\pokemonMoveList.json", JsonConvert.SerializeObject(pokemonList, Formatting.Indented));
        }
    }
}
