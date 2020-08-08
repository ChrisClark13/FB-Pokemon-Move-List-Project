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
        [JsonProperty(Order = -3)]
        public string DefaultForm = "Normal";
        [JsonProperty(Order = -2)]
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

        public void ParseVeekunPokemon(VeekunPokemon veekunPokemon)
        {
            var form = veekunPokemon.Form.Equals("Normal", StringComparison.InvariantCultureIgnoreCase) ? DefaultForm : veekunPokemon.Form;

            Console.WriteLine($"Processing Veekun Pokemon: {Name} ({form})");

            var levelUpMoveList = FindOrCreateFormLevelUpMoveList(form);

            veekunPokemon.LevelUpMoves.ForEach(vm => levelUpMoveList.AddMove(vm.Name, vm.LevelValue));
            levelUpMoveList.SortMoves();

            ParseVeekunMoveList(veekunPokemon.EggMoves, EggMoves, form);
            ParseVeekunMoveList(veekunPokemon.TutorMoves, TutorMoves, form);
            ParseVeekunMoveList(veekunPokemon.MachineMoves, MachineMoves, form);
        }

        private void ParseVeekunMoveList(List<RawDataMove> source, List<Move> dest, string form)
        {
            if (source == null)
                return;

            //Add all moves that aren't already in the list
            dest.AddRange(
                source.Where(vm => !dest.Any(m => m.Name == vm.Name))
                .Select(vm => new Move
                {
                    Name = vm.Name,
                    Forms = { form }
                })
            );

            //For all moves that are already in the list, update their corresponding move.
            foreach (var m in source.Join(dest, vm => vm.Name, m => m.Name, (vm, m) => m))
            {
                if (!m.Forms.Contains(form))
                    m.Forms.Add(form);
            }
        }

        public void ParseSerebiiPokemon(SerebiiPokemon serebiiPokemon)
        {
            Console.WriteLine($"Processing Serebii Pokemon: {Name}");

            //Handle default form
            var levelUpMoveList = FindOrCreateFormLevelUpMoveList(DefaultForm);
            serebiiPokemon.LevelUpMoves.ForEach(sm => levelUpMoveList.AddMove(sm.Name, sm.LevelValue));

            //Handle alt forms
            foreach (var form in AltForms)
            {
                if (form.Equals("Alola", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (serebiiPokemon.AlolanFormLevelUpMoves != null)
                    {
                        var list = FindOrCreateFormLevelUpMoveList(form);
                        serebiiPokemon.AlolanFormLevelUpMoves.ForEach(alolaMove => list.AddMove(alolaMove.Name, alolaMove.LevelValue));
                    }
                }
                else if (form.Equals("Galarian", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (serebiiPokemon.GalarianFormLevelUpMoves != null)
                    {
                        var list = FindOrCreateFormLevelUpMoveList(form);
                        serebiiPokemon.GalarianFormLevelUpMoves.ForEach(galarianMove => list.AddMove(galarianMove.Name, galarianMove.LevelValue));
                    }
                }
                else
                {
                    var altForm = serebiiPokemon.AltForms.Find(altForm => altForm.Form.Equals(form, StringComparison.InvariantCultureIgnoreCase));
                    var list = FindOrCreateFormLevelUpMoveList(form);
                    if (altForm != null)
                        altForm.LevelUpMoves.ForEach(altMove => list.AddMove(altMove.Name, altMove.LevelValue));
                }
            }

            ParseSerebiiMoveList(serebiiPokemon.EggMoves, EggMoves);
            ParseSerebiiMoveList(serebiiPokemon.TutorMoves, TutorMoves);
            ParseSerebiiMoveList(serebiiPokemon.MachineMoves, MachineMoves);

            SortMoves();
        }

        private void ParseSerebiiMoveList(List<SerebiiMove> source, List<Move> dest)
        {
            if (source == null)
                return;

            //Fix source data because of website weirdness.
            source.Where(m => m.Forms != null).SelectMany(m => m.Forms.Where(f => f.Form == Name)).ToList().ForEach(f => f.Form = DefaultForm);
            source.Where(m => m.Forms != null).SelectMany(m => m.Forms.Where(f => f.Form == "Alolan")).ToList().ForEach(f => f.Form = "Alola");

            //Add all moves that aren't already in the list
            dest.AddRange(
                source.Where(sm => !dest.Any(m => m.Name == sm.Name))
                .Select(sm => new Move
                {
                    Name = sm.Name,
                    Forms = (sm.Forms != null) ? sm.Forms.Select(f => f.Form).ToList() : new List<string> { DefaultForm }.Concat(AltForms).ToList()
                })
            );

            //For all moves that are already in the list, update their corresponding move.
            foreach ((var forms, var m) in source.Join(dest, sm => sm.Name, m => m.Name, (sm, m) =>
                Tuple.Create((sm.Forms != null) ? sm.Forms.Select(f => f.Form).ToList() : new List<string> { DefaultForm }.Concat(AltForms).ToList(), m)))
            {
                m.Forms.AddRange(forms.Where(form => !m.Forms.Contains(form)));
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

            pokemon.EggMoves.Where(pm => pm.Forms.Contains(preEvolvedForm) && !EggMoves.Any(m => m.Name == pm.Name && m.Forms.Contains(form)))
                .ToList().ForEach(pm =>
                {
                    Console.WriteLine($"Adding Egg Move \"{pm.Name}\" from {pokemon.Name} ({preEvolvedForm}) to {Name} ({form})");
                    var existingMove = EggMoves.Find(m => m.Name == pm.Name);
                    if (existingMove != null)
                        existingMove.Forms.Add(form);
                    else
                        EggMoves.Add(new Move { Name = pm.Name, Forms = { form } });
                });

            pokemon.TutorMoves.Where(pm => !EvolutionList.AllNonTransferableMoves.Contains(pm.Name) && pm.Forms.Contains(preEvolvedForm) && !TutorMoves.Any(m => m.Name == pm.Name && m.Forms.Contains(form)))
            .ToList().ForEach(pm =>
            {
                Console.WriteLine($"Adding Tutor Move \"{pm.Name}\" from {pokemon.Name} ({preEvolvedForm}) to {Name} ({form})");
                var existingMove = TutorMoves.Find(m => m.Name == pm.Name);
                if (existingMove != null)
                    existingMove.Forms.Add(form);
                else
                    TutorMoves.Add(new Move { Name = pm.Name, Forms = { form } });
            });

            pokemon.MachineMoves.Where(pm => pm.Forms.Contains(preEvolvedForm) && !MachineMoves.Any(m => m.Name == pm.Name && m.Forms.Contains(form)))
            .ToList().ForEach(pm =>
            {
                Console.WriteLine($"Adding Machine Move \"{pm.Name}\" from {pokemon.Name} ({preEvolvedForm}) to {Name} ({form})");
                var existingMove = MachineMoves.Find(m => m.Name == pm.Name);
                if (existingMove != null)
                    existingMove.Forms.Add(form);
                else
                    MachineMoves.Add(new Move { Name = pm.Name, Forms = { form } });
            });

            SortMoves();
        }
    }
}