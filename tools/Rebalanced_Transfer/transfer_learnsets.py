from pathlib import Path
import json
import re
import os

pokemon_path = Path("./tools/Rebalanced_Transfer/src").resolve()
level_up_path = pokemon_path / "level_up_learnsets.h"
tutor_path = pokemon_path / "tutor_learnsets.h"
tmhm_path = pokemon_path / "tmhm_learnsets.h"
egg_path = pokemon_path / "egg_moves.h"
sets_path = pokemon_path / "additional_sets.json"

profiles_path = Path("./tools/Pokabbie/PokemonDataGenerator/PokemonDataGenerator/Resources/PokemonProfiles/").resolve()
rebalanced_path = profiles_path / "Rebalanced"
ex_path = profiles_path / "Ex"
origin_path = Path("./tools/Pokabbie/PokemonDataGenerator/PokemonDataGenerator/bin/Debug/content_cache/pokemon_profiles/ex").resolve()

new_moves = {}
new_sets = {}

with level_up_path.open('r') as f:
    lastmon = ''
    linenum = 0
    ignore_mode = False
    rebalanced = False
    moves = []
    for line in f.readlines():
        linenum += 1
        name_match = re.search(r"s(?P<name>\w*)LevelUpLearnset\[\] = {", line)
        if name_match != None:
            lastmon = name_match.group("name")
            lastmon = re.sub(r"([a-z])([A-Z])", r"\1_\2", lastmon).lower()
            rebalanced = False
            ignore_mode = False
            moves = []
            continue
        if re.search(r"#ifdef ROGUE_DRAYANO", line) != None:
            ignore_mode = False
            rebalanced = True
            continue
        if re.search(r"#else", line) != None:
            ignore_mode = True
            continue
        if re.search(r"#endif", line) != None:
            ignore_mode = False
            continue
        move_match = re.search(r"LEVEL_UP_MOVE\(\s?(?P<level>\d*), (?P<name>\w*)\),", line)
        if not ignore_mode and move_match != None:
            moves.append({"Move": move_match.group("name"), "Level": move_match.group("level")})
            continue
        if re.search(r"LEVEL_UP_END", line) != None:
            if rebalanced:
                new_moves[lastmon] = {"lvl": moves}
            else:                
                new_moves[lastmon] = {}

with tutor_path.open('r') as f:
    lastmon = ''
    linenum = 0
    moves = []
    rebalanced = False
    for line in f.readlines():
        linenum += 1
        name_match = re.search(r"\[SPECIES_(?P<name>\w*)\]", line)
        if name_match != None:
            lastmon = name_match.group("name").lower()
            moves = []
        if re.search(r"#ifdef ROGUE_DRAYANO", line) != None:
            rebalanced = True
            continue
        if re.search(r"#endif", line) != None:
            rebalanced = False
            continue
        if rebalanced:
            move_match = re.search(r"TUTOR\((?P<name>\w*)\)", line)
            if move_match != None:
                moves.append(move_match.group("name"))
        if re.search(r"\,", line) != None:
            if len(moves) > 0:
                new_moves[lastmon]["tutor"] = moves

with tmhm_path.open('r') as f:
    lastmon = ''
    linenum = 0
    moves = []
    rebalanced = False
    for line in f.readlines():
        linenum += 1
        name_match = re.search(r"\[SPECIES_(?P<name>\w*)\]", line)
        if name_match != None:
            lastmon = name_match.group("name").lower()
            moves = []
        if re.search(r"#ifdef ROGUE_DRAYANO", line) != None:
            rebalanced = True
            continue
        if re.search(r"#endif", line) != None:
            rebalanced = False
            continue
        if rebalanced:
            move_match = re.search(r"TMHM\((TM|HM)\d\d_(?P<name>\w*)\)", line)
            if move_match != None:
                moves.append("MOVE_" + move_match.group("name"))
        if re.search(r"\,", line) != None:
            if len(moves) > 0 and (re.search(r"(deerling|sawsbuck|sinistea|polteageist|alcremie|zarude)_", lastmon) == None):
                new_moves[lastmon]["tmhm"] = moves

with egg_path.open('r') as f:
    lastmon = ''
    linenum = 0
    moves = []
    rebalanced = False
    for line in f.readlines():
        linenum += 1
        name_match = re.search(r"egg_moves\((?P<name>\w*)\,", line)
        if name_match != None:
            lastmon = name_match.group("name").lower()
            moves = []
        if re.search(r"#ifdef ROGUE_DRAYANO", line) != None:
            rebalanced = True
            continue
        if re.search(r"#endif", line) != None:
            rebalanced = False
            continue
        if rebalanced:
            move_match = re.search(r"(?P<name>MOVE_\w*)", line)
            if move_match != None:
                moves.append(move_match.group("name"))
        if re.search(r"\,", line) != None:
            if len(moves) > 0:
                new_moves[lastmon]["egg"] = moves

with sets_path.open('r', encoding='utf8') as f:
    sets = json.load(f)
    for mon, v in sets.items():
        mon = mon.lower().replace('-', '_').replace(' ', '_').replace("alola", "alolan").replace("galar", "galarian").replace(".", "").replace("meowstic_f", "meowstic_female")
        new_sets[mon] = []
        suffix_match = re.search(r"(?P<name>\w*)_(therian|small|super|large)", mon)
        if suffix_match != None:
            new_moves[mon] = new_moves[suffix_match.group("name")].copy()
        new_moves[mon]["extra"] = []
        if "extra_moves" in v:
            new_moves[mon]["extra"] = ["MOVE_" + re.sub(r"( )|(-)", "_", m).upper() for m in v["extra_moves"]]            
        if "movesets" in v:
            for tier, sets in v["movesets"].items():
                for setname, comp_set in sets.items():
                    new_set = {}
                    new_set["Moves"] = []
                    new_set["Item"] = "ITEM_" + re.sub(r"( )|(-)", "_", comp_set["item"]).upper()
                    new_set["Ability"] = "ABILITY_" + re.sub(r"( )|(-)", "_", comp_set["ability"]).upper()
                    new_set["HiddenPower"] = None
                    for move in comp_set["moves"]:
                        hp_match = re.search(r"Hidden Power (?P<name>\w*)", move)
                        newmove = ""
                        if hp_match != None:
                            new_set["HiddenPower"] = "TYPE_" + re.sub(r"( )|(-)", "_", hp_match.group("name")).upper()
                            newmove = "MOVE_HIDDEN_POWER"
                            new_moves[mon]["extra"].append("MOVE_HIDDEN_POWER")
                        else:
                            newmove = "MOVE_" + re.sub(r"( )|(-)", "_", move).upper()
                        new_set["Moves"].append(newmove)
                        new_moves[mon]["extra"].append(newmove)
                    new_set["SourceTiers"] = [tier.upper()]
                    new_set["Name"] = setname
                    new_sets[mon].append(new_set)

print(len(new_moves))

for mon, moves in new_moves.items():
    if len(moves) > 0:
        origin_file = origin_path / f"F_species_{mon}.json"
        d = {}
        if origin_file.exists():
            with origin_file.open('r') as f:
                d = json.load(f)
        else:
            ex_file = ex_path / f"species_{mon}.json"
            with ex_file.open('r') as f:
                d = json.load(f)
        if "lvl" in moves:
            d["LevelUpMoves"] = moves["lvl"]
        if "tutor" in moves:
            d["TutorMoves"] = list(set(d["TutorMoves"]) | set(moves["tutor"]))
        if "tmhm" in moves:
            d["TutorMoves"] = list(set(d["TutorMoves"]) | set(moves["tmhm"]))
        if "egg" in moves:
            d["TutorMoves"] = list(set(d["TutorMoves"]) | set(moves["egg"]))
        if "extra" in moves:
            d["TutorMoves"] = list(set(d["TutorMoves"]) | set(moves["extra"]))
        d["TutorMoves"] = list(set(d["TutorMoves"]) - set([m["Move"] for m in d["LevelUpMoves"]]))

        if mon in new_sets:
            comp_sets = new_sets[mon]
            for new_set in comp_sets:
                compatible_sets = [i for i, s in enumerate(d["CompetitiveSets"]) if s["Name"] == new_set["Name"] and new_set["SourceTiers"][0] in s["SourceTiers"] and new_set["Item"] == s["Item"]]
                if len(compatible_sets) > 0:
                    old_set = d["CompetitiveSets"][compatible_sets[0]]
                    old_set["Moves"] = new_set["Moves"]
                    old_set["Ability"] = new_set["Ability"]
                    old_set["Item"] = new_set["Item"]
                    old_set["HiddenPower"] = new_set["HiddenPower"]
                else:
                    new_set["Nature"] = "NATURE_HARDY"
                    new_set["TeraType"] = None
                    d["CompetitiveSets"].append(new_set)

        rebalanced_file = rebalanced_path / f"species_{mon}.json"
        with rebalanced_file.open('w', encoding='utf-8') as f:
            json.dump(d, f, indent=4)