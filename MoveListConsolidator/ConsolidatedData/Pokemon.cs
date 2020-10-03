using System;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

using MoveListConsolidator.RawData;

namespace MoveListConsolidator.ConsolidatedData
{
    public class Pokemon : BasePokemon
    {
        public string DefaultForm = "Normal";
        public List<string> AltForms = new List<string>();
        public List<LevelUpMoveList> LevelUpMoveLists = new List<LevelUpMoveList>();
        public List<Move> EggMoves = new List<Move>();
        public List<Move> TutorMoves = new List<Move>();
        public List<Move> MachineMoves = new List<Move>();

        public void SortMoves()
        {
            LevelUpMoveLists.ForEach(list => list.SortMoves());
            EggMoves.Sort((a, b) => a.Name.CompareTo(b.Name));
            TutorMoves.Sort((a, b) => a.Name.CompareTo(b.Name));
            MachineMoves.Sort((a, b) => a.Name.CompareTo(b.Name));
        }

        private LevelUpMoveList FindOrCreateFormLevelUpMoveList(string form)
        {
            var levelUpMoveList = LevelUpMoveLists.Find(l => l.Form.Equals(form, StringComparison.InvariantCultureIgnoreCase));
            if (levelUpMoveList == null)
            {
                levelUpMoveList = new LevelUpMoveList();
                levelUpMoveList.Form = form;
                LevelUpMoveLists.Add(levelUpMoveList);
            }
            return levelUpMoveList;
        }

        public void ParseVeekunPokemon(VeekunPokemon veekunPokemon, string filePath)
        {
            var form = veekunPokemon.Form.Equals("Normal", StringComparison.InvariantCultureIgnoreCase) ? DefaultForm : veekunPokemon.Form;

            Console.WriteLine($"Processing Veekun Pokemon: {Name} ({form})");

            var levelUpMoveList = FindOrCreateFormLevelUpMoveList(form);

            veekunPokemon.LevelUpMoves.ForEach(vm => levelUpMoveList.AddMove(vm.Name, vm.LevelValue));
            levelUpMoveList.SortMoves();

            ParseVeekunMoveList(veekunPokemon.EggMoves, EggMoves, form, filePath);
            ParseVeekunMoveList(veekunPokemon.TutorMoves, TutorMoves, form, filePath);
            ParseVeekunMoveList(veekunPokemon.MachineMoves, MachineMoves, form, filePath);
        }

        private void ParseVeekunMoveList(List<RawDataMove> source, List<Move> dest, string form, string filePath)
        {
            if (source == null)
                return;

            //Add all moves that aren't already in the list
            dest.AddRange(
                source.Where(vm => !dest.Any(m => m.Name == vm.Name))
                .Select(vm => new Move
                {
                    Name = vm.Name,
                    Forms = { form },
                    FormSources = { new Move.FormSource { Form = form, Sources = { filePath } } }
                })
            );

            //For all moves that are already in the list, update their corresponding move.
            foreach (var m in source.Join(dest, vm => vm.Name, m => m.Name, (vm, m) => m))
            {
                if (!m.Forms.Contains(form))
                {
                    m.Forms.Add(form);
                    m.FormSources.Add(new Move.FormSource { Form = form, Sources = { filePath } });
                }
                else
                {
                    m.FormSources.Find(fs => fs.Form == form && !fs.Sources.Contains(filePath))?.Sources.Add(filePath);
                }
            }
        }

        public void ParseSerebiiPokemon(SerebiiPokemon serebiiPokemon, bool isSwSh, string filePath)
        {
            Console.WriteLine($"Processing Serebii Pokemon: {Name} isSwSh: {isSwSh}");

            if (serebiiPokemon.LevelUpMoves != null)
            {
                //Handle default form
                var levelUpMoveList = FindOrCreateFormLevelUpMoveList(DefaultForm);
                serebiiPokemon.LevelUpMoves.ForEach(sm => levelUpMoveList.AddMove(sm.Name, sm.LevelValue));
            }

            //Handle alt forms
            foreach (var form in AltForms)
            {
                if (form.Equals("Alola", StringComparison.InvariantCultureIgnoreCase) || form.Equals("Alolan", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (serebiiPokemon.AlolanFormLevelUpMoves != null)
                    {
                        var list = FindOrCreateFormLevelUpMoveList("Alola");
                        serebiiPokemon.AlolanFormLevelUpMoves.ForEach(alolaMove => list.AddMove(alolaMove.Name, alolaMove.LevelValue));
                    }
                }
                else if (form.Equals("Galarian", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (serebiiPokemon.GalarianFormLevelUpMoves != null)
                    {
                        var list = FindOrCreateFormLevelUpMoveList("Galarian");
                        serebiiPokemon.GalarianFormLevelUpMoves.ForEach(galarianMove => list.AddMove(galarianMove.Name, galarianMove.LevelValue));
                    }
                }
                else if (serebiiPokemon.AltForms != null)
                {
                    var altForm = serebiiPokemon.AltForms.Find(altForm => altForm.Form.Equals(form, StringComparison.InvariantCultureIgnoreCase));
                    var list = FindOrCreateFormLevelUpMoveList(form);
                    if (altForm != null)
                        altForm.LevelUpMoves.ForEach(altMove => list.AddMove(altMove.Name, altMove.LevelValue));
                }
            }

            ParseSerebiiMoveList(serebiiPokemon.EggMoves, EggMoves, isSwSh, filePath);
            ParseSerebiiMoveList(serebiiPokemon.TutorMoves, TutorMoves, isSwSh, filePath);
            ParseSerebiiMoveList(serebiiPokemon.MachineMoves, MachineMoves, isSwSh, filePath);

            SortMoves();
        }

        private void ParseSerebiiMoveList(List<SerebiiMove> source, List<Move> dest, bool isSwSh, string filePath)
        {
            if (source == null)
                return;

            //Fix source data because of website weirdness.
            source.Where(m => m.Forms != null).SelectMany(m => m.Forms.Where(f => f.Form == Name)).ToList().ForEach(f => f.Form = DefaultForm);
            source.Where(m => m.Forms != null).SelectMany(m => m.Forms.Where(f => f.Form == "Alolan")).ToList().ForEach(f => f.Form = "Alola");

            if (Name == "Meowstic")
            {
                source.Find(sm => sm.Name == "Imprison");
            }

            List<string> GetFormNames(List<SerebiiMove.FormEntry> forms)
            {
                if (forms != null)
                    return forms.Select(f => f.Form).ToList();
                else if (isSwSh)
                    return new List<string> { DefaultForm }.Concat(AltForms).ToList();
                else
                    return new List<string> { DefaultForm }.Concat(AltForms).Where(f => f != "Galarian").ToList();
            };

            //Add all moves that aren't already in the list
            dest.AddRange(
                source.Where(sm => !dest.Any(m => m.Name == sm.Name))
                .Select(sm => new Move
                {
                    Name = sm.Name,
                    Forms = GetFormNames(sm.Forms),
                    FormSources = GetFormNames(sm.Forms).Select(f => new Move.FormSource { Form = f, Sources = { filePath } }).ToList()
                })
            );

            //For all moves that are already in the list, update their corresponding move.
            foreach ((var forms, var m) in source.Join(dest, sm => sm.Name, m => m.Name, (sm, m) =>
                Tuple.Create(GetFormNames(sm.Forms), m)))
            {
                m.Forms.AddRange(forms.Where(form => !m.Forms.Contains(form)));

                forms.ForEach(f => {
                    if (!m.FormSources.Any(fs => fs.Form == f)) {
                        m.FormSources.Add(new Move.FormSource { Form = f});
                    }
                });

                m.FormSources.Where(fs => forms.Contains(fs.Form) && !fs.Sources.Contains(filePath)).Select(fm => fm.Sources).ToList().ForEach(sources => sources.Add(filePath));
            }

            SortMoves();
        }

        public void ParseAltFormWithOneAdditionalMove(AlternateFormList.AltFormWithSingleAdditionalMove altForm)
        {
            var moveListJson = JsonConvert.SerializeObject(FindOrCreateFormLevelUpMoveList(DefaultForm));

            foreach (var form in altForm.Forms)
            {
                Console.WriteLine($"Processing Alt Form (With One Additional Move): {Name} ({form.Form}) - Move: {form.Move}");

                if (!AltForms.Contains(form.Form))
                    AltForms.Add(form.Form);

                var levelUpMoveList = JsonConvert.DeserializeObject<LevelUpMoveList>(moveListJson);
                levelUpMoveList.Form = form.Form;
                levelUpMoveList.AddMove(form.Move, 1);
                LevelUpMoveLists.Add(levelUpMoveList);

                EggMoves.Where(m => m.Forms.Contains(DefaultForm)).ToList().ForEach(m => m.Forms.Add(form.Form));
                TutorMoves.Where(m => m.Forms.Contains(DefaultForm)).ToList().ForEach(m => m.Forms.Add(form.Form));
                MachineMoves.Where(m => m.Forms.Contains(DefaultForm)).ToList().ForEach(m => m.Forms.Add(form.Form));
            }

            SortMoves();
        }

        public void ProcessPreEvolvedStageMoves(Pokemon pokemon, string form, string preEvolvedForm)
        {
            var levelUpMoveList = LevelUpMoveLists.Find(l => l.Form == form);
            var preEvolvedLevelUpMoveList = pokemon.LevelUpMoveLists.Find(l => l.Form == preEvolvedForm);

            preEvolvedLevelUpMoveList.LevelUpMoves.Where(pm => !levelUpMoveList.LevelUpMoves.Any(m => m.Name == pm.Name))
                .ToList().ForEach(pm =>
                {
                    Console.WriteLine($"Adding Level Up Move \"{pm.Name} ({pm.Level})\" from {pokemon.Name} ({preEvolvedForm}) to {Name} ({form})");
                    levelUpMoveList.AddMove(pm.Name, pm.Level);
                });

            void AddPreEvolvedMoves(List<Move> sourceList, List<Move> destList, string debugMoveType)
            {
                sourceList.Where(pm => !EvolutionList.AllNonTransferableMoves.Contains(pm.Name)).Where(pm => pm.Forms.Contains(preEvolvedForm) && !destList.Any(m => m.Name == pm.Name && m.Forms.Contains(form)))
                .ToList().ForEach(pm =>
                {
                    Console.WriteLine($"Adding {debugMoveType} \"{pm.Name}\" from {pokemon.Name} ({preEvolvedForm}) to {Name} ({form})");
                    var existingMove = destList.Find(m => m.Name == pm.Name);
                    if (existingMove != null)
                    {
                        existingMove.Forms.Add(form);
                        var formSourcesForm = existingMove.FormSources.Find(fs => fs.Form == form);
                        if (formSourcesForm == null)
                        {
                            formSourcesForm = new Move.FormSource { Form = form };
                            existingMove.FormSources.Add(formSourcesForm);
                        }
                        var s = $"Move of previous stage {pokemon.Name} ({preEvolvedForm})";
                        if (!formSourcesForm.Sources.Contains(s))
                           formSourcesForm.Sources.Add(s);
                    }
                    else
                        destList.Add(new Move
                        {
                            Name = pm.Name,
                            Forms = { form },
                            FormSources = { new Move.FormSource {
                                Form = form,
                                Sources = { $"Move of previous stage {pokemon.Name} ({preEvolvedForm})" } 
                                } 
                            }
                        });
                });
            }

            AddPreEvolvedMoves(pokemon.EggMoves, EggMoves, "Egg Move");
            AddPreEvolvedMoves(pokemon.TutorMoves, TutorMoves, "Tutor Move");
            AddPreEvolvedMoves(pokemon.MachineMoves, MachineMoves, "Machine Move");

            SortMoves();
        }
    }
}