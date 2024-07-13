using Newtonsoft.Json.Linq;
using PokemonDataGenerator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonDataGenerator.Pokedex
{

	public static class PokedexGenerator
	{

		private class PokedexData
		{
			public string InternalName;
			public string DisplayName;
			public int GenLimit;
			public List<string> Mons;
		}

		private static readonly string c_API = "https://pokeapi.co/api/v2/";
		private static Dictionary<string, string> m_DexApiLinks = null;


		public static void GeneratePokedexEntries(bool isVanillaVersion)
		{
			GatherAPILinks();

			List<PokedexData> fullDexes = new List<PokedexData>();

			if (isVanillaVersion)
			{
				fullDexes.Add(GatherDexData("kanto_RBY", "Red/Blue/Yellow", 1, "kanto"));
				fullDexes.Add(GatherDexData("johto_GSC", "Gold/Silver/Crystal", 2, "original-johto"));
				fullDexes.Add(GatherDexData("hoenn_RSE", "Ruby/Sapphire/Emerald", 3, "hoenn"));

				fullDexes.Add(GatherDexData("national_gen1", "Gen. 1", 1, "national"));
				fullDexes.Add(GatherDexData("national_gen2", "Gen. 2", 2, "national"));
				fullDexes.Add(GatherDexData("national_gen3", "Gen. 3", 3, "national"));
			}
			else
			{
				// Purposely order so the most recent regional dex is first
				fullDexes.Add(GatherResourceDexData("rogue_modern", "Modern", 9, "Rogue/Rogue Dex.csv"));

				// Hacky, gather based on 3 but allow up to 9 in reality
				var dexData = GatherDexData("rogue_classicplus", "Classic Plus", 4, "national");
				dexData.GenLimit = 9;
				fullDexes.Add(dexData);

				fullDexes.Add(GatherDexData("kanto_RBY", "Red/Blue/Yellow", 1, "kanto"));
                fullDexes.Add(GatherDexData("kanto_FRLG", "FireRed/LeafGreen", 3, "kanto")); // UIE: New Dex: FRLG post-game dex, including new evos and post-game catchable Gen 2 Pokemon
                fullDexes.Add(GatherDexData("kanto_letsgo", "Let's Go!", 7, "letsgo-kanto"));
                fullDexes.Add(GatherDexData("kanto_plus", "FireRed/LeafGreen Plus", 9, "letsgo-kanto"));

                fullDexes.Add(GatherDexData("johto_GSC", "Gold/Silver/Crystal", 2, "original-johto"));
				fullDexes.Add(GatherDexData("johto_HGSS", "HeartGold/SoulSilver", 4, "updated-johto"));
                fullDexes.Add(GatherDexData("johto_plus", "Crystal Plus", 9, "original-johto"));

                fullDexes.Add(GatherDexData("hoenn_RS", "Ruby/Sapphire", 3, "hoenn"));
                fullDexes.Add(GatherDexData("hoenn_emerald", "Emerald", 3, "hoenn"));
                //fullDexes.Add(GatherDexData("hoenn_pinball", "Ruby & Sapphire Pinball", 3, "hoenn")); // UIE: This is a joke
                fullDexes.Add(GatherDexData("hoenn_ORAS", "OmegaRuby/AlphaSapphire", 6, "updated-hoenn"));
                fullDexes.Add(GatherDexData("hoenn_plus", "Emerald Plus", 9, "hoenn"));

                fullDexes.Add(GatherDexData("sinnoh_DP", "Diamond/Pearl", 4, "original-sinnoh"));
				fullDexes.Add(GatherDexData("sinnoh_PL", "Platinum", 4, "extended-sinnoh"));
                fullDexes.Add(GatherDexData("sinnoh_plus", "Platinum Plus", 9, "extended-sinnoh"));

                fullDexes.Add(GatherDexData("unova_BW", "Black/White", 5, "original-unova"));
                fullDexes.Add(GatherDexData("unova_plus", "Black/White Plus", 5, "original-unova"));
                fullDexes.Add(GatherDexData("unova_BW2", "Black 2/White 2", 5, "updated-unova"));
                fullDexes.Add(GatherDexData("unova_plus2", "Black 2/White 2 Plus", 9, "updated-unova"));

                fullDexes.Add(GatherDexData("kalos_XY", "X/Y", 6, "kalos-central", "kalos-coastal", "kalos-mountain"));
                fullDexes.Add(GatherDexData("kalos_central", "Central Kalos", 6, "kalos-central"));
                fullDexes.Add(GatherDexData("kalos_coastal", "Coastal Kalos", 6, "kalos-coastal"));
                fullDexes.Add(GatherDexData("kalos_mountain", "Mountain Kalos", 6, "kalos-mountain"));
                fullDexes.Add(GatherDexData("kalos_plus", "X/Y Plus", 9, "kalos-central", "kalos-coastal", "kalos-mountain"));

                fullDexes.Add(GatherDexData("alola_SM", "Sun/Moon", 7, "original-alola", "original-melemele", "original-akala", "original-ulaula", "original-poni"));
                fullDexes.Add(GatherDexData("alola_USUM", "UltraSun/UltraMoon", 7, "updated-alola", "updated-melemele", "updated-akala", "updated-ulaula", "updated-poni"));
                fullDexes.Add(GatherDexData("alola_melemele", "Melemele Island (US/UM)", 7, "updated-melemele"));
                fullDexes.Add(GatherDexData("alola_akala", "Akala Island (US/UM)", 7, "updated-akala"));
                fullDexes.Add(GatherDexData("alola_ulaula", "Ula'ula Island (US/UM)", 7, "updated-ulaula"));
                fullDexes.Add(GatherDexData("alola_poni", "Poni Island (US/UM)", 7, "updated-poni"));
                fullDexes.Add(GatherDexData("alola_plus", "UltraSun/UltraMoon Plus", 9, "updated-alola", "updated-melemele", "updated-akala", "updated-ulaula", "updated-poni"));

                fullDexes.Add(GatherDexData("galar_swsh", "Sword/Shield", 8, "galar"));
				fullDexes.Add(GatherDexData("galar_isleofarmor", "Isle of Armor", 8, "isle-of-armor"));
				fullDexes.Add(GatherDexData("galar_crowntundra", "Crown Tundra", 8, "crown-tundra"));
				fullDexes.Add(GatherDexData("galar_fulldlc", "Sword/Shield + DLC", 8, "galar", "isle-of-armor", "crown-tundra"));
                fullDexes.Add(GatherDexData("galar_plus", "Sword/Shield Plus", 9, "galar", "isle-of-armor", "crown-tundra"));

                fullDexes.Add(GatherDexData("paldea_scvi", "Scarlet/Violet", 9, "paldea"));
				fullDexes.Add(GatherDexData("paldea_kitakami", "The Teal Mask", 9, "kitakami"));
				fullDexes.Add(GatherDexData("paldea_blueberry", "Indigo Disk", 9, "blueberry"));
				fullDexes.Add(GatherDexData("paldea_fulldlc", "Scarlet/Violet + DLC", 9, "paldea", "kitakami", "blueberry"));
                fullDexes.Add(GatherDexData("paldea_plus", "Scarlet/Violet Plus", 9, "paldea", "kitakami", "blueberry"));

                fullDexes.Add(GatherResourceDexData("orre_colosseum", "Colosseum", 3, "Orre/Colo Dex.csv"));
                fullDexes.Add(GatherResourceDexData("orre_XD", "XD: Gale of Darkness", 3, "Orre/XD Dex.csv"));
                fullDexes.Add(GatherResourceDexData("orre_coloXD", "Colosseum & XD", 3, "Orre/Orre Dex.csv"));
                fullDexes.Add(GatherResourceDexData("orre_plus", "Colosseum & XD Plus", 9, "Orre/Orre Dex Plus.csv"));

                fullDexes.Add(GatherResourceDexData("fiore_ranger", "Ranger", 3, "Ranger/Fiore Dex.csv"));
                fullDexes.Add(GatherResourceDexData("fiore_plus", "Ranger Plus", 9, "Ranger/Fiore Dex Plus.csv"));

                fullDexes.Add(GatherResourceDexData("almia_shadows", "Shadows of Almia", 4, "Ranger/Almia Dex.csv"));
                fullDexes.Add(GatherResourceDexData("almia_plus", "Shadows of Almia Plus", 9, "Ranger/Almia Dex Plus.csv"));

                fullDexes.Add(GatherResourceDexData("oblivia_guardian", "Guardian Signs", 4, "Ranger/Oblivia Dex.csv"));
                fullDexes.Add(GatherResourceDexData("oblivia_plus", "Guardian Signs Plus", 9, "Ranger/Oblivia Dex Plus.csv"));

                fullDexes.Add(GatherDexData("ransei_conquest", "Conquest", 5, "conquest-gallery"));
                fullDexes.Add(GatherDexData("ransei_plus", "Conquest Plus", 9, "conquest-gallery"));

                fullDexes.Add(GatherDexData("hisui_legendsarceus", "Legends Arceus", 9, "hisui"));
                fullDexes.Add(GatherDexData("hisui_plus", "Legends Arceus Plus", 9, "hisui"));

                //fullDexes.Add(GatherResourceDexData("extras_unown", "Unown Dex", 9, "Extras/Unown Dex.csv")); //DOES NOT WORK, defaults to Magikarp 99% of the time

                fullDexes.Add(GatherDexData("national_gen1", "Gen. 1", 1, "national"));
				fullDexes.Add(GatherDexData("national_gen2", "Gen. 2", 2, "national"));
				fullDexes.Add(GatherDexData("national_gen3", "Gen. 3", 3, "national"));
				fullDexes.Add(GatherDexData("national_gen4", "Gen. 4", 4, "national"));
				fullDexes.Add(GatherDexData("national_gen5", "Gen. 5", 5, "national"));
				fullDexes.Add(GatherDexData("national_gen6", "Gen. 6", 6, "national"));
				fullDexes.Add(GatherDexData("national_gen7", "Gen. 7", 7, "national"));
				fullDexes.Add(GatherDexData("national_gen8", "Gen. 8", 8, "national"));
				fullDexes.Add(GatherDexData("national_gen9", "Gen. 9", 9, "national"));
			}

			Dictionary<string, List<PokedexData>> regionVariants = new Dictionary<string, List<PokedexData>>();

			foreach (var dex in fullDexes)
			{
				string region = dex.InternalName.Split('_')[0];

				if (!regionVariants.ContainsKey(region))
					regionVariants[region] = new List<PokedexData>();

				regionVariants[region].Add(dex);
			}

			string outputDir = Path.GetFullPath($"pokedex_generator");
			ExportConstants(Path.Combine(GameDataHelpers.RootDirectory, "include\\constants\\rogue_pokedex.h"), fullDexes, regionVariants);
			ExportData(Path.Combine(GameDataHelpers.RootDirectory, "src\\data\\rogue_pokedex.h"), fullDexes, regionVariants);


			return;
		}

		private static void GatherAPILinks()
		{
			m_DexApiLinks = new Dictionary<string, string>();

			JObject dex = ContentCache.GetJsonContent(c_API + "pokedex");
			Console.WriteLine("Gathering Dex info...");

			while (true)
			{
				foreach (JObject result in dex["results"])
				{
					string name = result["name"].ToString();
					string url = result["url"].ToString();

					Console.WriteLine("\tFound " + name);
					m_DexApiLinks.Add(name, url);
				}

				var nextUri = dex["next"].ToString();
				if (!string.IsNullOrEmpty(nextUri))
				{
					dex = ContentCache.GetJsonContent(nextUri.ToString());
				}
				else
					break;
			}
		}

		private static PokedexData GatherDexData(string name, string displayName, int genLimit, params string[] dexIds)
		{
			Console.WriteLine($"Generating {name} dex data");

			PokedexData data = new PokedexData();
			data.InternalName = name;
			data.DisplayName = displayName;
			data.GenLimit = genLimit;
			data.Mons = new List<string>();

			foreach (string dexId in dexIds)
			{
				AppendDexMons(dexId, data);
			}

			return data;
		}

		private static bool IsSpeciesIgnored(string species)
		{
			if (!GameDataHelpers.IsVanillaVersion)
			{
				//switch (FormatKeyword(species))
				//{
				//	case "ARCHALUDON":
				//	case "HYDRAPPLE":
				//	case "GOUGING_FIRE":
				//	case "RAGING_BOLT":
				//	case "IRON_BOULDER":
				//	case "IRON_CROWN":
				//	case "TERAPAGOS":
				//	case "PECHARUNT":
				//		return true;
				//}
			}
			else
			{
				// For vanilla just ignore any species we don't have
				if (!GameDataHelpers.SpeciesDefines.ContainsKey($"SPECIES_{FormatKeyword(species)}"))
					return true;
			}

			return false;
		}

		private static void AppendDexMons(string dexId, PokedexData target)
		{
			string uri = m_DexApiLinks[dexId];
			JObject dex = ContentCache.GetJsonContent(uri);

            bool isFRLG = target.InternalName.Equals("kanto_FRLG", StringComparison.CurrentCultureIgnoreCase);
            bool isHGSS = target.InternalName.Equals("johto_HGSS", StringComparison.CurrentCultureIgnoreCase);
            bool isEmerald = target.InternalName.Equals("hoenn_emerald", StringComparison.CurrentCultureIgnoreCase);
            bool isORAS = target.InternalName.Equals("hoenn_ORAS", StringComparison.CurrentCultureIgnoreCase);
            bool isBDSP = target.InternalName.Equals("sinnoh_BDSP", StringComparison.CurrentCultureIgnoreCase);
            bool isSM = target.InternalName.Equals("alola_SM", StringComparison.CurrentCultureIgnoreCase);
            bool isUSUM = target.InternalName.Equals("alola_USUM", StringComparison.CurrentCultureIgnoreCase);
            bool isSwSh = target.InternalName.Equals("galar_crowntundra", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("galar_fulldlc", StringComparison.CurrentCultureIgnoreCase);
            bool isScVi = target.InternalName.Equals("paldea_blueberry", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("paldea_fulldlc", StringComparison.CurrentCultureIgnoreCase);

            bool isPinball = target.InternalName.Equals("hoenn_pinball", StringComparison.CurrentCultureIgnoreCase); // UIE: This is a joke

            bool isFRLGPlus = target.InternalName.Equals("kanto_plus", StringComparison.CurrentCultureIgnoreCase);
            bool isEmeraldPlus = target.InternalName.Equals("hoenn_plus", StringComparison.CurrentCultureIgnoreCase);
            bool isClassicPlus = target.InternalName.Equals("rogue_classicplus", StringComparison.CurrentCultureIgnoreCase)
				|| target.InternalName.Equals("kanto_plus", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("johto_plus", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("hoenn_plus", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("sinnoh_plus", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("unova_plus", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("unova_plus2", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("kalos_plus", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("alola_plus", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("galar_plus", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("paldea_plus", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("ransei_plus", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("hisui_plus", StringComparison.CurrentCultureIgnoreCase);


            foreach (JObject entry in dex["pokemon_entries"])
			{
				string species = entry["pokemon_species"]["name"].ToString();
				string speciesDefine = $"SPECIES_{FormatKeyword(species)}";

				if (IsSpeciesIgnored(species))
					continue;

				if (!GameDataHelpers.SpeciesDefines.ContainsKey(speciesDefine))
					throw new InvalidDataException($"Failed to find define for {speciesDefine}");

				// Exclude species here
				if (dexId.StartsWith("national"))
				{
					if (target.GenLimit != 9)
					{
						// Ignore gen9
						if (GameDataHelpers.SpeciesDefines[speciesDefine].StartsWith("GEN9_START"))
							continue;
					}

					switch (target.GenLimit)
					{
						case 1:
							if (GameDataHelpers.GetSpeciesNum(speciesDefine) > GameDataHelpers.GetSpeciesNum("SPECIES_MEW"))
								continue;
							break;
						case 2:
							if (GameDataHelpers.GetSpeciesNum(speciesDefine) > GameDataHelpers.GetSpeciesNum("SPECIES_CELEBI"))
								continue;
							break;
						case 3:
							if (GameDataHelpers.GetSpeciesNum(speciesDefine) > GameDataHelpers.GetSpeciesNum("SPECIES_DEOXYS"))
								continue;
							break;
						case 4:
							if (GameDataHelpers.GetSpeciesNum(speciesDefine) > GameDataHelpers.GetSpeciesNum("SPECIES_ARCEUS"))
								continue;
							break;
						case 5:
							if (GameDataHelpers.GetSpeciesNum(speciesDefine) > GameDataHelpers.GetSpeciesNum("SPECIES_MELOETTA"))
								continue;
							break;
						case 7:
							if (GameDataHelpers.GetSpeciesNum(speciesDefine) > GameDataHelpers.GetSpeciesNum("SPECIES_VOLCANION"))
								continue;
							break;
						case 8:
							if (GameDataHelpers.GetSpeciesNum(speciesDefine) > GameDataHelpers.GetSpeciesNum("SPECIES_ENAMORUS"))
								continue;
							break;
					}
				}

				if (!target.Mons.Contains(species))
				{
					target.Mons.Add(species);

					if (isFRLG)
                    {
                        AppendDexMon_ExtraFRLG(species, target); // UIE: Use this to add post-game locked evos to RBY dex
                        AppendDexMon_ExtraSevii(species, target); // UIE: Use this to add post-game Gen 2 mon to end of dex
                    }
                    else if (isFRLGPlus)
                    {
                        AppendDexMon_ExtraSeviiPlus(species, target); // UIE: Use this to add post-game Gen 2 mon to end of dex
                    }
                    else if (isHGSS)
                    {
                        AppendDexMon_ExtraHGSS(species, target); // HGSS dex is missing a lot of evos from regional dex, as they weren't techincally obtainable until post game so forcefully insert them here
                        //AppendDexMon_ExtraHGSSPostGame(species, target); // HGSS dex is missing a lot of evos from regional dex, as they weren't techincally obtainable until post game so forcefully insert them here
                    }
                    else if (isEmerald)
                    {
                        AppendDexMon_ExtraEmeraldSafari(species, target); // UIE: Use this to add the 40-ish post-game Gen 1 & 2 mon
                    }
                    else if (isEmeraldPlus)
                    {
                        AppendDexMon_ExtraEmeraldSafariPlus(species, target); // UIE: Use this to add the 40-ish post-game Gen 1 & 2 mon
                    }
                    else if (isPinball)
                    {
                        AppendDexMon_ExtraPinball(species, target); // UIE: This is a joke
                    }
                    else if (isORAS)
                    {
                        AppendDexMon_ExtraORASDexNav(species, target);
                        //AppendDexMon_ExtraMirageSpots(species, target);
                    }
                    else if (isBDSP)
                    {
                        //AppendDexMon_ExtraRamanasPark(species, target);
                    }
                    else if (isSM)
                    {
                        //AppendDexMon_ExtraIslandScanSM(species, target);
                    }
                    else if (isUSUM)
                    {
                        //AppendDexMon_ExtraIslandScanUSUM(species, target);
                        //AppendDexMon_ExtraUltraSpace(species, target);
                    }
                    else if (isSwSh)
                    {
                        AppendDexMon_ExtraSwSh(species, target);
                        //AppendDexMon_ExtraDynaAdv(species, target);
                        //AppendDexMon_ExtraSwShCompatible(species, target);
                    }
                    else if (isScVi)
                    {
                        AppendDexMon_ExtraScVi(species, target);
                        //AppendDexMon_ExtraSnacksworth(species, target);
                        //AppendDexMon_ExtraScViCompatible(species, target);
                    }
                    else if (isClassicPlus) // UIE: I've majorly expanded this function to accomodate up to Gen 9. Seems to correctly ignore mon that don't exist in the dex or don't need to be added
                    {
                        AppendDexMon_ExtraClassicPlus(species, target);
                    }

                    if (dexId == "blueberry")
						AppendDexMon_ScViBlueberry(species, target);
				}
			}

			return;
		}

		private static PokedexData GatherResourceDexData(string name, string displayName, int genLimit, params string[] dexIds)
		{
			Console.WriteLine($"Generating {name} dex data");

			PokedexData data = new PokedexData();
			data.InternalName = name;
			data.DisplayName = displayName;
			data.GenLimit = genLimit;
			data.Mons = new List<string>();

			foreach (string dexId in dexIds)
			{
				AppendResourceDexMons(dexId, data);
			}

			return data;
		}

		private static void AppendResourceDexMons(string dexId, PokedexData target)
		{
			string csvContent = ContentCache.GetHttpContent("res://Pokedex/" + dexId);

			using (StringReader reader = new StringReader(csvContent))
			{
				string line;

				while ((line = reader.ReadLine()) != null)
				{
					// Skip header
					if (line.StartsWith("Name,"))
						continue;

					string[] columns = line.Split(',');

					string species = columns[0].ToLower().Trim();
					if (!target.Mons.Contains(species))
					{
						target.Mons.Add(species);
					}
				}
			}

			return;
		}

        private static void AppendDexMon_ExtraFRLG(string species, PokedexData target)
        {
            if (species.Equals("pikachu", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "pichu";
                target.Mons.Add("pikachu");
            }
            else if (species.Equals("clefairy", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "cleffa";
                target.Mons.Add("clefairy");
            }
            else if (species.Equals("jigglypuff", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "igglybuff";
                target.Mons.Add("jigglypuff");
            }
            else if (species.Equals("golbat", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("crobat");
            }
            else if (species.Equals("vileplume", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("bellossom");
            }
            else if (species.Equals("poliwrath", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("politoed");
            }
            else if (species.Equals("slowbro", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("slowking");
            }
            else if (species.Equals("onix", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("steelix");
            }
            else if (species.Equals("hitmonlee", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "tyrogue";
                target.Mons.Add("hitmonlee");
            }
            else if (species.Equals("hitmonchan", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("hitmontop");
            }
            else if (species.Equals("chansey", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("blissey");
            }
            else if (species.Equals("seadra", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("kingdra");
            }
            else if (species.Equals("scyther", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("scizor");
            }
            else if (species.Equals("jynx", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "smoochum";
                target.Mons.Add("jynx");
            }
            else if (species.Equals("electabuzz", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "elekid";
                target.Mons.Add("electabuzz");
            }
            else if (species.Equals("magmar", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "magby";
                target.Mons.Add("magmar");
            }
            else if (species.Equals("flareon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("espeon");
                target.Mons.Add("umbreon");
            }
            else if (species.Equals("porygon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("porygon2");
            }
            if (species.Equals("marill", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "azurill";
                target.Mons.Add("marill");
            }
            else if (species.Equals("wobbuffet", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "wynaut";
                target.Mons.Add("wobbuffet");
            }
        }
        // Adds (21) new evos that become accessible in the post-game
        private static void AppendDexMon_ExtraSevii(string species, PokedexData target)
        {
            if (species.Equals("mew", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("sentret");
                target.Mons.Add("furret");
                target.Mons.Add("ledyba");
                target.Mons.Add("ledian");
                target.Mons.Add("spinarak");
                target.Mons.Add("ariados");
                target.Mons.Add("togepi");
                target.Mons.Add("togetic");
                target.Mons.Add("natu");
                target.Mons.Add("xatu");
                target.Mons.Add("azurill"); // Hoenn mon accessible via breeding
                target.Mons.Add("marill");
                target.Mons.Add("azumarill");
                target.Mons.Add("hoppip");
                target.Mons.Add("skiploom");
                target.Mons.Add("jumpluff");
                target.Mons.Add("yanma");
                target.Mons.Add("wooper");
                target.Mons.Add("quagsire");
                target.Mons.Add("misdreavus");
                target.Mons.Add("unown");
                target.Mons.Add("wynaut"); // Hoenn mon accessible via breeding
                target.Mons.Add("wobbuffet");
                target.Mons.Add("dunsparce");
                target.Mons.Add("qwilfish");
                target.Mons.Add("heracross");
                target.Mons.Add("sneasel");
                target.Mons.Add("slugma");
                target.Mons.Add("magcargo");
                target.Mons.Add("swinub");
                target.Mons.Add("piloswine");
                target.Mons.Add("remoraid");
                target.Mons.Add("octillery");
                target.Mons.Add("delibird");
                target.Mons.Add("mantine");
                target.Mons.Add("skarmory");
                target.Mons.Add("phanpy");
                target.Mons.Add("donphan");
                target.Mons.Add("raikou");
                target.Mons.Add("entei");
                target.Mons.Add("suicune");
                target.Mons.Add("larvitar");
                target.Mons.Add("pupitar");
                target.Mons.Add("tyranitar");
                target.Mons.Add("lugia"); // Naval Rock
                target.Mons.Add("ho-oh"); // Naval Rock
                target.Mons.Add("celebi"); // Event-exclusive, may remove
                target.Mons.Add("deoxys"); // Birth Island
            }
        }
        // Adds (46) [Gen 2] & (2) [Gen 3] Pokemon obtainable on the Sevii Islands
        // Adds (3) [Gen 2] & (1) [Gen 3] Legendaries from events
        private static void AppendDexMon_ExtraSeviiPlus(string species, PokedexData target)
        {
            if (species.Equals("mew", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("sentret");
                target.Mons.Add("furret");
                target.Mons.Add("ledyba");
                target.Mons.Add("ledian");
                target.Mons.Add("spinarak");
                target.Mons.Add("ariados");
                target.Mons.Add("togepi");
                target.Mons.Add("togetic");
                target.Mons.Add("togekiss");
                target.Mons.Add("natu");
                target.Mons.Add("xatu");
                target.Mons.Add("azurill"); // Hoenn mon accessible via breeding
                target.Mons.Add("marill");
                target.Mons.Add("azumarill");
                target.Mons.Add("hoppip");
                target.Mons.Add("skiploom");
                target.Mons.Add("jumpluff");
                target.Mons.Add("yanma");
                target.Mons.Add("yanmega");
                target.Mons.Add("wooper");
                target.Mons.Add("quagsire");
                target.Mons.Add("clodsire");
                target.Mons.Add("misdreavus");
                target.Mons.Add("mismagius");
                target.Mons.Add("unown");
                target.Mons.Add("wynaut"); // Hoenn mon accessible via breeding
                target.Mons.Add("wobbuffet");
                target.Mons.Add("dunsparce");
                target.Mons.Add("dudunsparce");
                target.Mons.Add("qwilfish");
                target.Mons.Add("overqwil");
                target.Mons.Add("heracross");
                target.Mons.Add("sneasel");
                target.Mons.Add("weavile");
                target.Mons.Add("sneasler");
                target.Mons.Add("slugma");
                target.Mons.Add("magcargo");
                target.Mons.Add("swinub");
                target.Mons.Add("piloswine");
                target.Mons.Add("mamoswine");
                target.Mons.Add("remoraid");
                target.Mons.Add("octillery");
                target.Mons.Add("delibird");
                target.Mons.Add("mantyke");
                target.Mons.Add("mantine");
                target.Mons.Add("skarmory");
                target.Mons.Add("phanpy");
                target.Mons.Add("donphan");
                target.Mons.Add("raikou");
                target.Mons.Add("entei");
                target.Mons.Add("suicune");
                target.Mons.Add("larvitar");
                target.Mons.Add("pupitar");
                target.Mons.Add("tyranitar");
                target.Mons.Add("lugia"); // Naval Rock
                target.Mons.Add("ho-oh"); // Naval Rock
                target.Mons.Add("celebi"); // Event-exclusive, may remove
                target.Mons.Add("deoxys"); // Birth Island
            }
        }
        // Copy of the above function with ExtraClassic evolutions manually added because I don't know how to code

        private static void AppendDexMon_ExtraHGSS(string species, PokedexData target)
        {
            // UIE: I don't believe it's actually possible to get Magnezone
			//if (species.Equals("magneton", StringComparison.CurrentCultureIgnoreCase))
			//{
			//	target.Mons.Add("magnezone");
			//}
			if (species.Equals("lickitung", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("lickilicky");
            }
            else if (species.Equals("rhydon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("rhyperior");
            }
            else if (species.Equals("chansey", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "happiny";
                target.Mons.Add("chansey");
            }
			else if (species.Equals("tangela", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("tangrowth");
			}
			else if (species.Equals("electabuzz", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("electivire");
			}
			else if (species.Equals("magmar", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("magmortar");
			}
            // UIE: I don't believe it's actually possible to get Leafeon or Glaceon in HGSS
			//else if (species.Equals("umbreon", StringComparison.CurrentCultureIgnoreCase))
			//{
			//	target.Mons.Add("leafeon");
			//	target.Mons.Add("glaceon");
			//}
			else if (species.Equals("togetic", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("togekiss");
			}
			else if (species.Equals("aipom", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("ambipom");
			}
			else if (species.Equals("yanma", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("yanmega");
			}
			else if (species.Equals("murkrow", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("honchkrow");
			}
			else if (species.Equals("misdreavus", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("mismagius");
			}
			else if (species.Equals("gligar", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("gliscor");
			}
			else if (species.Equals("sneasel", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("weavile");
            }
            else if (species.Equals("wobbuffet", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "wynaut";
                target.Mons.Add("wobbuffet");
            }
            else if (species.Equals("piloswine", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("mamoswine");
			}
            // UIE: Commenting out mon that aren't in HGSS regional dex. These would be added by AppendHGSSPostGame, if necessary
			//else if (species.Equals("kirlia", StringComparison.CurrentCultureIgnoreCase))
			//{
			//	target.Mons.Add("gallade");
			//}
            // UIE: I don't believe it's actually possible to get Probopass
			//else if (species.Equals("nosepass", StringComparison.CurrentCultureIgnoreCase))
			//{
			//	target.Mons.Add("probopass");
			//}
			//else if (species.Equals("roselia", StringComparison.CurrentCultureIgnoreCase))
			//{
			//	target.Mons.Add("roserade");
			//}
			//else if (species.Equals("dusclops", StringComparison.CurrentCultureIgnoreCase))
			//{
			//	target.Mons.Add("dusknoir");
			//}
			//else if (species.Equals("snorunt", StringComparison.CurrentCultureIgnoreCase))
			//{
			//	target.Mons.Add("froslass");
			//}
        }
        // Adds (15) new evos that become accessible in the post-game
        private static void AppendDexMon_ExtraHGSSPostGame(string species, PokedexData target)
        {
            if (species.Equals("celebi", StringComparison.CurrentCultureIgnoreCase))
            {
                // Gift Starter
                target.Mons.Add("treeko");
                target.Mons.Add("grovyle");
                target.Mons.Add("sceptile");
                target.Mons.Add("torchic");
                target.Mons.Add("combusken");
                target.Mons.Add("blaziken");
                target.Mons.Add("mudkip");
                target.Mons.Add("marshtomp");
                target.Mons.Add("swampert");
                // Headbutt Trees
                target.Mons.Add("taillow");
                target.Mons.Add("swellow");
                target.Mons.Add("starly");
                target.Mons.Add("staravia");
                target.Mons.Add("staraptor");
                target.Mons.Add("burmy");
                target.Mons.Add("wormadam");
                target.Mons.Add("mothim");
                target.Mons.Add("cherubi");
                target.Mons.Add("cherrim");
                // Swarm Pokemon
                target.Mons.Add("poochyena");
                target.Mons.Add("mightyena");
                target.Mons.Add("wingull");
                target.Mons.Add("pelipper");
                target.Mons.Add("ralts");
                target.Mons.Add("kirlia");
                target.Mons.Add("gardevoir");
                target.Mons.Add("gallade");
                target.Mons.Add("sableye");
                target.Mons.Add("mawile");
                target.Mons.Add("gulpin");
                target.Mons.Add("swalot");
                target.Mons.Add("swablu");
                target.Mons.Add("altaria");
                target.Mons.Add("baltoy");
                target.Mons.Add("claydol");
                target.Mons.Add("clamperl");
                target.Mons.Add("huntail");
                target.Mons.Add("gorebyss");
                target.Mons.Add("relicanth");
                target.Mons.Add("luvdisc");
                target.Mons.Add("buneary");
                target.Mons.Add("lopunny");
                // Bug Catching Contest
                target.Mons.Add("wurmple");
                target.Mons.Add("silcoon");
                target.Mons.Add("beautifly");
                target.Mons.Add("cascoon");
                target.Mons.Add("dustox");
                target.Mons.Add("nincada");
                target.Mons.Add("ninjask");
                target.Mons.Add("shedinja");
                target.Mons.Add("kricketot");
                target.Mons.Add("kricketune");
                target.Mons.Add("combee");
                target.Mons.Add("vespiquen");
                // Safari Zone
                target.Mons.Add("zigzagoon");
                target.Mons.Add("linoone");
                target.Mons.Add("slakoth");
                target.Mons.Add("vigoroth");
                target.Mons.Add("slaking");
                target.Mons.Add("aron");
                target.Mons.Add("lairon");
                target.Mons.Add("aggron");
                target.Mons.Add("zangoose");
                target.Mons.Add("spheal");
                target.Mons.Add("sealeo");
                target.Mons.Add("walrein");
                target.Mons.Add("bronzor");
                target.Mons.Add("bronzong");
                target.Mons.Add("lotad");
                target.Mons.Add("lombre");
                target.Mons.Add("ludicolo");
                target.Mons.Add("trapinch");
                target.Mons.Add("vibrava");
                target.Mons.Add("flygon");
                target.Mons.Add("cacnea");
                target.Mons.Add("cacturne");
                target.Mons.Add("hippopotas");
                target.Mons.Add("hippowdon");
                target.Mons.Add("carnivine");
                target.Mons.Add("surskit");
                target.Mons.Add("masquerain");
                target.Mons.Add("electrike");
                target.Mons.Add("manectric");
                target.Mons.Add("shinx");
                target.Mons.Add("luxio");
                target.Mons.Add("luxray");
                target.Mons.Add("seedot");
                target.Mons.Add("nuzleaf");
                target.Mons.Add("shiftry");
                target.Mons.Add("nosepass");
                target.Mons.Add("riolu");
                target.Mons.Add("lucario");
                target.Mons.Add("shuppet");
                target.Mons.Add("banette");
                target.Mons.Add("beldum");
                target.Mons.Add("metang");
                target.Mons.Add("metagross");
                target.Mons.Add("bidoof");
                target.Mons.Add("bibarel");
                target.Mons.Add("budew");
                target.Mons.Add("roselia");
                target.Mons.Add("roserade");
                target.Mons.Add("duskull");
                target.Mons.Add("dusclops");
                target.Mons.Add("dusknoir");
                target.Mons.Add("chingling");
                target.Mons.Add("chimecho");
                target.Mons.Add("bagon");
                target.Mons.Add("shelgon");
                target.Mons.Add("salamence");
                target.Mons.Add("pachirisu");
                target.Mons.Add("buizel");
                target.Mons.Add("floatzel");
                target.Mons.Add("serviper");
                target.Mons.Add("barboach");
                target.Mons.Add("whiscash");
                target.Mons.Add("croagunk");
                target.Mons.Add("toxicroak");
                target.Mons.Add("meditite");
                target.Mons.Add("medicham");
                target.Mons.Add("volbeat");
                target.Mons.Add("lunatone");
                target.Mons.Add("corphish");
                target.Mons.Add("crawdaunt");
                target.Mons.Add("gible");
                target.Mons.Add("gabite");
                target.Mons.Add("garchomp");
                target.Mons.Add("shroomish");
                target.Mons.Add("breloom");
                target.Mons.Add("illumise");
                target.Mons.Add("solrock");
                target.Mons.Add("skorupi");
                target.Mons.Add("drapion");
                target.Mons.Add("torkoal");
                target.Mons.Add("spinda");
                // Hoenn Sound Channel
                target.Mons.Add("whismur");
                target.Mons.Add("loudred");
                target.Mons.Add("exploud");
                target.Mons.Add("makuhita");
                target.Mons.Add("hariyama");
                target.Mons.Add("plusle");
                target.Mons.Add("minun");
                target.Mons.Add("numel");
                target.Mons.Add("camerupt");
                target.Mons.Add("spoink");
                target.Mons.Add("grumpig");
                target.Mons.Add("absol");
                // Sinnoh Sound Channel
                target.Mons.Add("chatot");
                // PokeWalker
                target.Mons.Add("skitty");
                target.Mons.Add("delcatty");
                target.Mons.Add("carvanha");
                target.Mons.Add("sharpedo");
                target.Mons.Add("wailmer");
                target.Mons.Add("wailord");
                target.Mons.Add("feebas");
                target.Mons.Add("milotic");
                target.Mons.Add("castform");
                target.Mons.Add("kecleon");
                target.Mons.Add("tropius");
                target.Mons.Add("snorunt");
                target.Mons.Add("glalie");
                target.Mons.Add("froslass");
                target.Mons.Add("shellos");
                target.Mons.Add("gastrodon");
                target.Mons.Add("spiritomb");
                target.Mons.Add("finneon");
                target.Mons.Add("lumineon");
                target.Mons.Add("snover");
                target.Mons.Add("abomasnow");
                // Fossil Pokemon
                target.Mons.Add("lileep");
                target.Mons.Add("cradily");
                target.Mons.Add("anorith");
                target.Mons.Add("armaldo");
                // Post-Game Legendaries
                target.Mons.Add("kyogre");
                target.Mons.Add("groudon");
                target.Mons.Add("rayquaza");
                target.Mons.Add("latios");
                target.Mons.Add("latias");
                // Event Pokemon
                target.Mons.Add("palkia");
                target.Mons.Add("dialga");
                target.Mons.Add("giratina");
            }
        }
        // Adds (164) [Gen 3-4] Pokemon obtainable through various post-game methods
        // Adds (9) [Gen 3] Starters gifted by Steven
        // Adds (5) [Gen 3] Legendaries obtainable in the post-game
        // Adds (3) [Gen 4] Legendaries from an event

        private static void AppendDexMon_ExtraEmeraldSafari(string species, PokedexData target)
        {
            if (species.Equals("deoxys", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("hoothoot");
                target.Mons.Add("noctowl");
                target.Mons.Add("spinarak");
                target.Mons.Add("ariados");
                target.Mons.Add("mareep");
                target.Mons.Add("flaaffy");
                target.Mons.Add("ampharos");
                target.Mons.Add("aipom");
                target.Mons.Add("sunkern");
                target.Mons.Add("sunflora");
                target.Mons.Add("gligar");
                target.Mons.Add("snubbull");
                target.Mons.Add("granbull");
                target.Mons.Add("stantler");
                target.Mons.Add("wooper");
                target.Mons.Add("quagsire");
                target.Mons.Add("remoraid");
                target.Mons.Add("octillery");
                target.Mons.Add("ledyba");
                target.Mons.Add("ledian");
                target.Mons.Add("pineco");
                target.Mons.Add("forretress");
                target.Mons.Add("teddiursa");
                target.Mons.Add("ursaring");
                target.Mons.Add("houndour");
                target.Mons.Add("houndoom");
                target.Mons.Add("miltank");
                target.Mons.Add("shuckle");
                target.Mons.Add("meowth"); // Battle Frontier trade
                target.Mons.Add("persian");
                target.Mons.Add("ditto"); // Desert Underpass
                target.Mons.Add("sudowoodo"); // Battle Palace
                target.Mons.Add("smeargle"); // Artisan Cave
                target.Mons.Add("chikorita"); // Gift from Birch
                target.Mons.Add("bayleef");
                target.Mons.Add("meganium");
                target.Mons.Add("cyndaquil"); // Gift from Birch
                target.Mons.Add("quilava");
                target.Mons.Add("typhlosion");
                target.Mons.Add("totodile"); // Gift from Birch
                target.Mons.Add("croconaw");
                target.Mons.Add("feraligatr");
                target.Mons.Add("lugia"); // Navel Rock
                target.Mons.Add("ho-oh"); // Navel Rock
                target.Mons.Add("mew"); // Faraway Island
            }
        }
        // Adds (28) [Gen 2] Pokemon obtainable in the expanded Safari Zone in Emerald
        // Adds (9) [Gen 2] Starters gifted by Birch
        // Adds (1) [Gen 1] Pokemon obtainable from an in-game trade & (3) [Gen 2] Pokemon obtainable in the post-game (Total: 4)
        // Adds (1) [Gen 1] & (2) [Gen 2] Legendaries from events (Total: 3)
        private static void AppendDexMon_ExtraEmeraldSafariPlus(string species, PokedexData target)
        {
            if (species.Equals("deoxys", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("hoothoot");
                target.Mons.Add("noctowl");
                target.Mons.Add("spinarak");
                target.Mons.Add("ariados");
                target.Mons.Add("mareep");
                target.Mons.Add("flaaffy");
                target.Mons.Add("ampharos");
                target.Mons.Add("aipom");
                target.Mons.Add("ambipom");
                target.Mons.Add("sunkern");
                target.Mons.Add("sunflora");
                target.Mons.Add("gligar");
                target.Mons.Add("gliscor");
                target.Mons.Add("snubbull");
                target.Mons.Add("granbull");
                target.Mons.Add("stantler");
                target.Mons.Add("wyrdeer");
                target.Mons.Add("wooper");
                target.Mons.Add("quagsire");
                target.Mons.Add("clodsire");
                target.Mons.Add("remoraid");
                target.Mons.Add("octillery");
                target.Mons.Add("ledyba");
                target.Mons.Add("ledian");
                target.Mons.Add("pineco");
                target.Mons.Add("forretress");
                target.Mons.Add("teddiursa");
                target.Mons.Add("ursaring");
                target.Mons.Add("ursaluna");
                target.Mons.Add("houndour");
                target.Mons.Add("houndoom");
                target.Mons.Add("miltank");
                target.Mons.Add("shuckle");
                target.Mons.Add("meowth"); // Battle Frontier trade
                target.Mons.Add("persian");
                target.Mons.Add("perrserker");
                target.Mons.Add("ditto"); // Desert Underpass
                target.Mons.Add("bonsly");
                target.Mons.Add("sudowoodo"); // Battle Palace
                target.Mons.Add("smeargle"); // Artisan Cave
                target.Mons.Add("chikorita"); // Gift from Birch
                target.Mons.Add("bayleef");
                target.Mons.Add("meganium");
                target.Mons.Add("cyndaquil"); // Gift from Birch
                target.Mons.Add("quilava");
                target.Mons.Add("typhlosion");
                target.Mons.Add("totodile"); // Gift from Birch
                target.Mons.Add("croconaw");
                target.Mons.Add("feraligatr");
                target.Mons.Add("lugia"); // Navel Rock
                target.Mons.Add("ho-oh"); // Navel Rock
                target.Mons.Add("mew"); // Faraway Island
            }
        }
        // Copy of the above function with ExtraClassic evolutions manually added because I don't know how to code
        private static void AppendDexMon_ExtraPinball(string species, PokedexData target) // This is a joke
        {
            if (species.Equals("deoxys", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "chikorita";
                target.Mons.Add("cyndaquil");
                target.Mons.Add("totodile");
                target.Mons.Add("aerodactyl");
            }
        }
        // Adds (3) [Gen 2] Starters
        // Adds (1) [Gen 1] Pokemon
        private static void AppendDexMon_ExtraORASDexNav(string species, PokedexData target)
        {
            if (species.Equals("deoxys", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("chikorita"); // Gift from Birch
                target.Mons.Add("bayleef");
                target.Mons.Add("meganium");
                target.Mons.Add("cyndaquil"); // Gift from Birch
                target.Mons.Add("quilava");
                target.Mons.Add("typhlosion");
                target.Mons.Add("totodile"); // Gift from Birch
                target.Mons.Add("croconaw");
                target.Mons.Add("feraligatr");
                target.Mons.Add("snivy"); // Gift from Birch 2
                target.Mons.Add("servine");
                target.Mons.Add("serperior");
                target.Mons.Add("tepig"); // Gift from Birch 2
                target.Mons.Add("pignite");
                target.Mons.Add("emboar");
                target.Mons.Add("oshawott"); // Gift from Birch 2
                target.Mons.Add("dewott");
                target.Mons.Add("samurott");
                target.Mons.Add("turtwig"); // Gift from Birch 3
                target.Mons.Add("grotle");
                target.Mons.Add("torterra");
                target.Mons.Add("chimchar"); // Gift from Birch 3
                target.Mons.Add("monferno");
                target.Mons.Add("infernape");
                target.Mons.Add("piplup"); // Gift from Birch 3
                target.Mons.Add("prinplup");
                target.Mons.Add("empoleon");
                target.Mons.Add("togepi"); // Egg
                target.Mons.Add("togetic");
                target.Mons.Add("togekiss");
                // dexnav
                target.Mons.Add("lillipup");
                target.Mons.Add("herdier");
                target.Mons.Add("stoutland");
                target.Mons.Add("sewaddle");
                target.Mons.Add("swadloon");
                target.Mons.Add("leavanny");
                target.Mons.Add("zorua");
                target.Mons.Add("zoroark");
                target.Mons.Add("tympole");
                target.Mons.Add("palpitoad");
                target.Mons.Add("seismitoad");
                target.Mons.Add("gothita");
                target.Mons.Add("gothorita");
                target.Mons.Add("gothitelle");
                target.Mons.Add("shellos");
                target.Mons.Add("gastrodon");
                target.Mons.Add("chatot");
                target.Mons.Add("pidove");
                target.Mons.Add("tranquill");
                target.Mons.Add("unfezant");
                target.Mons.Add("krabby");
                target.Mons.Add("kingler");
                target.Mons.Add("frillish");
                target.Mons.Add("jellicent");
                target.Mons.Add("skrelp");
                target.Mons.Add("dragalge");
                target.Mons.Add("clauncher");
                target.Mons.Add("clawitzer");
                target.Mons.Add("trubbish");
                target.Mons.Add("garbodor");
                target.Mons.Add("gible");
                target.Mons.Add("gabite");
                target.Mons.Add("garchomp");
                target.Mons.Add("sandile");
                target.Mons.Add("krokorok");
                target.Mons.Add("krookodile");
                target.Mons.Add("dwebble");
                target.Mons.Add("crustle");
                target.Mons.Add("ponyta");
                target.Mons.Add("rapidash");
                target.Mons.Add("tyrogue");
                target.Mons.Add("hitmonlee");
                target.Mons.Add("hitmonchan");
                target.Mons.Add("hitmontop");
                target.Mons.Add("throh");
                target.Mons.Add("sawk");
                target.Mons.Add("scraggy");
                target.Mons.Add("scrafty");
                target.Mons.Add("bouffalant");
                target.Mons.Add("klefki");
                target.Mons.Add("misdreavus");
                target.Mons.Add("mismagius");
                target.Mons.Add("skorupi");
                target.Mons.Add("drapion");
                target.Mons.Add("cleffa");
                target.Mons.Add("clefairy");
                target.Mons.Add("clefable");
                target.Mons.Add("eevee");
                target.Mons.Add("vaporeon");
                target.Mons.Add("jolteon");
                target.Mons.Add("flareon");
                target.Mons.Add("espeon");
                target.Mons.Add("umbreon");
                target.Mons.Add("leafeon");
                target.Mons.Add("glaceon");
                target.Mons.Add("sylveon");
                target.Mons.Add("joltik");
                target.Mons.Add("galvantula");
                target.Mons.Add("rattata");
                target.Mons.Add("raticate");
                target.Mons.Add("deerling");
                target.Mons.Add("sawsbuck");
                target.Mons.Add("aipom");
                target.Mons.Add("shinx");
                target.Mons.Add("luxio");
                target.Mons.Add("drowzee");
                target.Mons.Add("hypno");
                target.Mons.Add("elgyem");
                target.Mons.Add("beheeyem");
                target.Mons.Add("finneon");
                target.Mons.Add("lumineon");
                target.Mons.Add("alomomola");
                target.Mons.Add("seel");
                target.Mons.Add("dewgong");
                target.Mons.Add("paras");
                target.Mons.Add("parasect");
                target.Mons.Add("cottonee");
                target.Mons.Add("whimsicott");
                target.Mons.Add("phantump");
                target.Mons.Add("trevenant");
                target.Mons.Add("onix");
                target.Mons.Add("steelix");
                target.Mons.Add("timburr");
                target.Mons.Add("gurdurr");
                target.Mons.Add("conkeldurr");
                target.Mons.Add("axew");
                target.Mons.Add("fraxure");
                target.Mons.Add("haxorus");
                target.Mons.Add("diglett");
                target.Mons.Add("dugtrio");
                target.Mons.Add("roggenrola");
                target.Mons.Add("boldore");
                target.Mons.Add("gigalith");
                target.Mons.Add("mankey");
                target.Mons.Add("primeape");
                target.Mons.Add("weedle");
                target.Mons.Add("kakuna");
                target.Mons.Add("beedrill");
                target.Mons.Add("pidgey");
                target.Mons.Add("pidgeotto");
                target.Mons.Add("pidgeot");
                target.Mons.Add("buneary");
                target.Mons.Add("lopunny");
                target.Mons.Add("druddigon");
                target.Mons.Add("deino");
                target.Mons.Add("zweilous");
                target.Mons.Add("hydreigon");
                target.Mons.Add("delibird");
                target.Mons.Add("cubchoo");
                target.Mons.Add("beartic");
                target.Mons.Add("growlithe");
                target.Mons.Add("arcanine");
                target.Mons.Add("bronzor");
                target.Mons.Add("bronzong");
                target.Mons.Add("murkrow");
                target.Mons.Add("honchkrow");
                target.Mons.Add("drifloon");
                target.Mons.Add("drifblim");
                target.Mons.Add("rufflet");
                target.Mons.Add("braviary");
                // post-game story legendaries
                target.Mons.Add("lugia");
                target.Mons.Add("ho-oh");
                target.Mons.Add("heatran");
                target.Mons.Add("regigigas");
            }
        }
        // Adds (130) Pokemon obtainable in the post-game via DexNav and Soaring in ORAS 
        // Adds (27) [Gen 2, 4, & 5] Starters gifted by Birch
        // Adds (3) Pokemon from a gifted egg
        // Adds (2) [Gen 2] & (2) [Gen 4] Legendaries with post-game quests (Total: 4)
        private static void AppendDexMon_ExtraMirageSpots(string species, PokedexData target)
        {
            if (species.Equals("deoxys", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("raikou");
                target.Mons.Add("entei");
                target.Mons.Add("suicune");
                target.Mons.Add("cresselia");
                target.Mons.Add("uxie");
                target.Mons.Add("mesprit");
                target.Mons.Add("azelf");
                target.Mons.Add("dialga");
                target.Mons.Add("palkia");
                target.Mons.Add("giratina");
                target.Mons.Add("tornadus");
                target.Mons.Add("thundurus");
                target.Mons.Add("landorus");
                target.Mons.Add("cobalion");
                target.Mons.Add("terrakion");
                target.Mons.Add("virizion");
                target.Mons.Add("reshiram");
                target.Mons.Add("zekrom");
                target.Mons.Add("kyurem");
            }
        }
        // Adds (3) [Gen 2], (7) [Gen 4], & (9) [Gen 5] Legendaries obtainable from Mirage Spots in ORAS (Total: 19)


        private static void AppendDexMon_ExtraRamanasPark(string species, PokedexData target)
        {
            if (species.Equals("deoxys", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("regirock");
                target.Mons.Add("regice");
                target.Mons.Add("registeel");
                target.Mons.Add("raikou");
                target.Mons.Add("entei");
                target.Mons.Add("suicune");
                target.Mons.Add("ho-oh");
                target.Mons.Add("articuno");
                target.Mons.Add("zapdos");
                target.Mons.Add("moltres");
                target.Mons.Add("lugia");
                target.Mons.Add("latios");
                target.Mons.Add("latias");
                target.Mons.Add("kyogre");
                target.Mons.Add("groudon");
                target.Mons.Add("rayquaza");
                target.Mons.Add("mewtwo");
            }
        }
        // Adds (4) [Gen 1], (5) [Gen 2], & (8) [Gen 3] Legendaries obtainable from Mirage Spots in ORAS (Total: 17)

        private static void AppendDexMon_ExtraIslandScanSM(string species, PokedexData target)
        {
            if (species.Equals("salamence", StringComparison.CurrentCultureIgnoreCase))
            {
                // Island Scan - Melemele
                target.Mons.Add("totodile"); // Mon
                target.Mons.Add("croconaw");
                target.Mons.Add("feraligatr");
                target.Mons.Add("deino"); // Tues
                target.Mons.Add("zweilous");
                target.Mons.Add("hydreigon");
                target.Mons.Add("horsea"); // Wed
                target.Mons.Add("seadra");
                target.Mons.Add("kingdra");
                target.Mons.Add("klink"); // Thurs
                target.Mons.Add("klang");
                target.Mons.Add("klinklang");
                target.Mons.Add("chikorita"); // Fri
                target.Mons.Add("bayleef");
                target.Mons.Add("meganium");
                target.Mons.Add("litwick"); // Sat
                target.Mons.Add("lampent");
                target.Mons.Add("chandelure");
                target.Mons.Add("cyndaquil"); // Sun
                target.Mons.Add("quilava");
                target.Mons.Add("typhlosion");
            }
            if (species.Equals("lanturn", StringComparison.CurrentCultureIgnoreCase))
            {
                // Island Scan - Akala
                target.Mons.Add("spheal"); // Mon
                target.Mons.Add("sealeo");
                target.Mons.Add("walrein");
                target.Mons.Add("shinx");
                target.Mons.Add("luxio"); // Tues
                target.Mons.Add("luxray");
                target.Mons.Add("honedge"); // Wed
                target.Mons.Add("doublade");
                target.Mons.Add("aegislash");
                target.Mons.Add("venipede"); // Thurs
                target.Mons.Add("whirlipede");
                target.Mons.Add("scolipede");
                target.Mons.Add("bellsprout"); // Fri
                target.Mons.Add("weepinbell");
                target.Mons.Add("victreebel");
                target.Mons.Add("azurill");
                target.Mons.Add("marill"); // Sat
                target.Mons.Add("azumarill");
                target.Mons.Add("gothita"); // Sun
                target.Mons.Add("gothorita");
                target.Mons.Add("gothitelle");
            }
            if (species.Equals("bisharp", StringComparison.CurrentCultureIgnoreCase))
            {
                // Island Scan - Ula'Ula
                target.Mons.Add("swinub"); // Mon
                target.Mons.Add("piloswine");
                target.Mons.Add("mamoswine");
                target.Mons.Add("solosis");
                target.Mons.Add("duosion"); // Tues
                target.Mons.Add("reuniclus");
                target.Mons.Add("budew");
                target.Mons.Add("roselia"); // Wed
                target.Mons.Add("roserade");
                target.Mons.Add("starly");
                target.Mons.Add("staravia"); // Thurs
                target.Mons.Add("staraptor");
                target.Mons.Add("slakoth");
                target.Mons.Add("vigoroth"); // Fri
                target.Mons.Add("slaking");
                target.Mons.Add("axew"); // Sat
                target.Mons.Add("fraxure");
                target.Mons.Add("haxorus");
                target.Mons.Add("rhyhorn"); // Sun
                target.Mons.Add("rhydon");
                target.Mons.Add("rhyperior");
            }
            if (species.Equals("aerodactyl", StringComparison.CurrentCultureIgnoreCase))
            {
                // Island Scan - Poni
                target.Mons.Add("timburr");
                target.Mons.Add("gurdurr");
                target.Mons.Add("conkeldurr"); // Mon
                target.Mons.Add("togepi");
                target.Mons.Add("togetic");
                target.Mons.Add("togekiss"); // Tues
                target.Mons.Add("sewaddle");
                target.Mons.Add("swadloon");
                target.Mons.Add("leavanny"); // Wed
                target.Mons.Add("snivy");
                target.Mons.Add("servine");
                target.Mons.Add("serperior"); // Thurs
                target.Mons.Add("oshawott");
                target.Mons.Add("dewott");
                target.Mons.Add("samurott"); // Fri
                target.Mons.Add("tepig");
                target.Mons.Add("pignite");
                target.Mons.Add("emboar"); // Sat
                target.Mons.Add("tynamo");
                target.Mons.Add("eelektrik");
                target.Mons.Add("eelektross"); // Sun
            }
        }
        // Adds (84) [Gen 1-6] Pokemon obtainable through Island Scan in SM
        private static void AppendDexMon_ExtraIslandScanUSUM(string species, PokedexData target)
        {
            if (species.Equals("salamence", StringComparison.CurrentCultureIgnoreCase))
            {
                // Island Scan - Melemele
                //SM
                target.Mons.Add("totodile"); // Mon
                target.Mons.Add("croconaw");
                target.Mons.Add("feraligatr");
                target.Mons.Add("deino"); // Tues
                target.Mons.Add("zweilous");
                target.Mons.Add("hydreigon");
                target.Mons.Add("klink"); // Thurs
                target.Mons.Add("klang");
                target.Mons.Add("klinklang");
                target.Mons.Add("chikorita"); // Fri
                target.Mons.Add("bayleef");
                target.Mons.Add("meganium");
                target.Mons.Add("cyndaquil"); // Sun
                target.Mons.Add("quilava");
                target.Mons.Add("typhlosion");
                // USUM
                target.Mons.Add("squirtle"); // Mon
                target.Mons.Add("wartortle");
                target.Mons.Add("blastoise");
                target.Mons.Add("onix"); // Tues
                target.Mons.Add("steelix");
                target.Mons.Add("horsea"); // Wed
                target.Mons.Add("seadra");
                target.Mons.Add("kingdra");
                target.Mons.Add("scatterbug"); // Thurs
                target.Mons.Add("spewpa");
                target.Mons.Add("vivillion");
                target.Mons.Add("bulbasaur"); // Fri
                target.Mons.Add("ivysaur");
                target.Mons.Add("venusaur");
                target.Mons.Add("litwick"); // Sat
                target.Mons.Add("lampent");
                target.Mons.Add("chandelure");
                target.Mons.Add("charmander"); // Sun
                target.Mons.Add("charmeleon");
                target.Mons.Add("charizard");
            }
            if (species.Equals("lanturn", StringComparison.CurrentCultureIgnoreCase))
            {
                // Island Scan - Akala
                //SM
                target.Mons.Add("shinx");
                target.Mons.Add("luxio"); // Tues
                target.Mons.Add("luxray");
                target.Mons.Add("venipede"); // Thurs
                target.Mons.Add("whirlipede");
                target.Mons.Add("scolipede");
                target.Mons.Add("bellsprout"); // Fri
                target.Mons.Add("weepinbell");
                target.Mons.Add("victreebel");
                target.Mons.Add("azurill");
                target.Mons.Add("marill"); // Sat
                target.Mons.Add("azumarill");
                target.Mons.Add("gothita"); // Sun
                target.Mons.Add("gothorita");
                target.Mons.Add("gothitelle");
                // USUM
                target.Mons.Add("spheal"); // Mon
                target.Mons.Add("sealeo");
                target.Mons.Add("walrein");
                target.Mons.Add("torchic");
                target.Mons.Add("combusken"); // Tues
                target.Mons.Add("blaziken");
                target.Mons.Add("honedge"); // Wed
                target.Mons.Add("doublade");
                target.Mons.Add("aegislash");
                target.Mons.Add("weedle");
                target.Mons.Add("kakuna");
                target.Mons.Add("beedrill"); // Thurs
                target.Mons.Add("treecko");
                target.Mons.Add("grovyle"); // Fri
                target.Mons.Add("sceptile");
                target.Mons.Add("mudkip");
                target.Mons.Add("marshtomp"); // Sat
                target.Mons.Add("swampert");
                target.Mons.Add("ralts"); // Sun
                target.Mons.Add("kirlia");
                target.Mons.Add("gardevoir");
                target.Mons.Add("gallade");
            }
            if (species.Equals("bisharp", StringComparison.CurrentCultureIgnoreCase))
            {
                // Island Scan - Ula'Ula
                //SM
                target.Mons.Add("solosis");
                target.Mons.Add("duosion"); // Tues
                target.Mons.Add("reuniclus");
                target.Mons.Add("budew");
                target.Mons.Add("roselia"); // Wed
                target.Mons.Add("roserade");
                target.Mons.Add("starly");
                target.Mons.Add("staravia"); // Thurs
                target.Mons.Add("staraptor");
                target.Mons.Add("slakoth");
                target.Mons.Add("vigoroth"); // Fri
                target.Mons.Add("slaking");
                //USUM
                target.Mons.Add("swinub"); // Mon
                target.Mons.Add("piloswine");
                target.Mons.Add("mamoswine");
                target.Mons.Add("piplup");
                target.Mons.Add("prinplup"); // Tues
                target.Mons.Add("empoleon");
                target.Mons.Add("turtwig");
                target.Mons.Add("grotle"); // Wed
                target.Mons.Add("torterra");
                target.Mons.Add("pidgey");
                target.Mons.Add("pidgeotto");
                target.Mons.Add("pidgeot"); // Thurs
                target.Mons.Add("chimchar");
                target.Mons.Add("monferno"); // Fri
                target.Mons.Add("infernape");
                target.Mons.Add("axew"); // Sat
                target.Mons.Add("fraxure");
                target.Mons.Add("haxorus");
                target.Mons.Add("rhyhorn"); // Sun
                target.Mons.Add("rhydon");
                target.Mons.Add("rhyperior");
            }
            if (species.Equals("aerodactyl", StringComparison.CurrentCultureIgnoreCase))
            {
                // Island Scan - Poni
                //SM
                target.Mons.Add("timburr");
                target.Mons.Add("gurdurr");
                target.Mons.Add("conkeldurr"); // Mon
                target.Mons.Add("togepi");
                target.Mons.Add("togetic");
                target.Mons.Add("togekiss"); // Tues
                target.Mons.Add("snivy");
                target.Mons.Add("servine");
                target.Mons.Add("serperior"); // Thurs
                target.Mons.Add("oshawott");
                target.Mons.Add("dewott");
                target.Mons.Add("samurott"); // Fri
                target.Mons.Add("tepig");
                target.Mons.Add("pignite");
                target.Mons.Add("emboar"); // Sat
                //USUM
                target.Mons.Add("aron");
                target.Mons.Add("lairon");
                target.Mons.Add("aggron"); // Mon
                target.Mons.Add("rotom"); // Tues
                target.Mons.Add("sewaddle");
                target.Mons.Add("swadloon");
                target.Mons.Add("leavanny"); // Wed
                target.Mons.Add("chespin");
                target.Mons.Add("quilladin");
                target.Mons.Add("chesnaught"); // Thurs
                target.Mons.Add("froakie");
                target.Mons.Add("frogadier");
                target.Mons.Add("greninja"); // Fri
                target.Mons.Add("fennekin");
                target.Mons.Add("braixen");
                target.Mons.Add("delphox"); // Sat
                target.Mons.Add("tynamo");
                target.Mons.Add("eelektrik");
                target.Mons.Add("eelektross"); // Sun
            }
        }
        // Adds (82) [Gen 1-6] Pokemon obtainable through Island Scan in USUM
        // Includes (59) [Gen 1-6] Pokemon obtainable through Island Scan in SM that CANNOT be obtained in USUM
        private static void AppendDexMon_ExtraUltraSpace(string species, PokedexData target)
        {
            if (species.Equals("lunala", StringComparison.CurrentCultureIgnoreCase))
            {
                // Red Wormholes - Cliff World
                target.Mons.Add("taillow");
                target.Mons.Add("swellow");
                target.Mons.Add("swablu");
                target.Mons.Add("altaria");
                target.Mons.Add("yanma");
                target.Mons.Add("yanmega");
                target.Mons.Add("sigilyph");
                target.Mons.Add("ducklett");
                target.Mons.Add("swanna");
                // Blue Wormholes - Water World
                target.Mons.Add("wooper");
                target.Mons.Add("quagsire");
                target.Mons.Add("lotad");
                target.Mons.Add("lombre");
                target.Mons.Add("ludicolo");
                target.Mons.Add("buizel");
                target.Mons.Add("floatzel");
                target.Mons.Add("stunfisk");
                target.Mons.Add("binacle");
                target.Mons.Add("barbaracle");
                // Green Wormholes - Rocky World
                target.Mons.Add("seedot");
                target.Mons.Add("nuzleaf");
                target.Mons.Add("shiftry");
                target.Mons.Add("spoink");
                target.Mons.Add("grumpig");
                target.Mons.Add("skorupi");
                target.Mons.Add("drapion");
                target.Mons.Add("audino");
                target.Mons.Add("helioptile");
                target.Mons.Add("heliolisk");
                // Yellow Wormholes - Cave World
                target.Mons.Add("slugma");
                target.Mons.Add("magcargo");
                target.Mons.Add("meditite");
                target.Mons.Add("medicham");
                target.Mons.Add("hippopotas");
                target.Mons.Add("hippowdon");
                target.Mons.Add("snover");
                target.Mons.Add("abomasnow");
                target.Mons.Add("dwebble");
                target.Mons.Add("crustle");
            }
            // UIE: Separate func for legendaries
            if (species.Equals("lunala", StringComparison.CurrentCultureIgnoreCase))
            {
                // Red Wormholes - Cliff World (Legendaries)
                target.Mons.Add("articuno");
                target.Mons.Add("zapdos");
                target.Mons.Add("moltres");
                target.Mons.Add("cresselia");
                target.Mons.Add("ho-oh");
                target.Mons.Add("yveltal");
                target.Mons.Add("tornadus");
                target.Mons.Add("thundurus");
                target.Mons.Add("landorus");
                target.Mons.Add("rayquaza");
                // Blue Wormholes - Water World (Legendaries)
                target.Mons.Add("uxie");
                target.Mons.Add("mesprit");
                target.Mons.Add("azelf");
                target.Mons.Add("latios");
                target.Mons.Add("latias");
                target.Mons.Add("lugia");
                target.Mons.Add("kyogre");
                target.Mons.Add("suicune");
                target.Mons.Add("kyurem");
                // Green Wormholes - Rocky World (Legendaries)
                target.Mons.Add("cobalion");
                target.Mons.Add("terrakion");
                target.Mons.Add("virizion");
                target.Mons.Add("mewtwo");
                target.Mons.Add("raikou");
                target.Mons.Add("entei");
                target.Mons.Add("dialga");
                target.Mons.Add("reshiram");
                target.Mons.Add("zekrom");
                target.Mons.Add("xerneas");
                // Yellow Wormholes - Cave World (Legendaries)
                target.Mons.Add("regirock");
                target.Mons.Add("regice");
                target.Mons.Add("registeel");
                target.Mons.Add("regigigas");
                target.Mons.Add("heatran");
                target.Mons.Add("groudon");
                target.Mons.Add("palkia");
                target.Mons.Add("giratina");
            }
        }
        // Adds (39) [Gen 1-6] Pokemon and (37) Legendaries obtainable through Ultra Space Wormholes in USUM

        private static void AppendDexMon_ExtraSwSh(string species, PokedexData target)
        {
            if (species.Equals("eternatus", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("rowlet"); // Gift from Diglett Trainer
                target.Mons.Add("dartrix");
                target.Mons.Add("decidueye");
                target.Mons.Add("litten"); // Gift from Diglett Trainer
                target.Mons.Add("torracat");
                target.Mons.Add("incineroar");
                target.Mons.Add("popplio"); // Gift from Diglett Trainer
                target.Mons.Add("brionne");
                target.Mons.Add("primarina");
                target.Mons.Add("keldeo"); // Event in Ballimere Lake
                target.Mons.Add("regigigas"); // Event in Giant's Bed
                target.Mons.Add("cosmog"); // Event in Freezington
                target.Mons.Add("cosmoem");
                target.Mons.Add("solgaleo");
                target.Mons.Add("lunala");
                target.Mons.Add("poipole"); // gift in Max Lair
                target.Mons.Add("naganadel");
            }
        }
        // Adds (9) [Gen 7] Starters & (1) Gen 4, (1) [Gen 5], and (6) [Gen 7] Legendaries obtainable in Crown Tundra but aren't in the dex for some reason
        private static void AppendDexMon_ExtraDynaAdv(string species, PokedexData target)
        {
            if (species.Equals("eternatus", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("treecko");
                target.Mons.Add("grovyle");
                target.Mons.Add("sceptile");
                target.Mons.Add("torchic");
                target.Mons.Add("combusken");
                target.Mons.Add("blaziken");
                target.Mons.Add("mudkip");
                target.Mons.Add("marshtomp");
                target.Mons.Add("swampert");
                target.Mons.Add("mewtwo");
                target.Mons.Add("raikou");
                target.Mons.Add("entei");
                target.Mons.Add("suicune");
                target.Mons.Add("lugia");
                target.Mons.Add("ho-oh");
                target.Mons.Add("latias");
                target.Mons.Add("latios");
                target.Mons.Add("kyogre");
                target.Mons.Add("groudon");
                target.Mons.Add("rayquaza");
                target.Mons.Add("uxie");
                target.Mons.Add("mesprit");
                target.Mons.Add("azelf");
                target.Mons.Add("dialga");
                target.Mons.Add("palkia");
                target.Mons.Add("heatran");
                target.Mons.Add("giratina");
                target.Mons.Add("cresselia");
                target.Mons.Add("tornadus");
                target.Mons.Add("thundurus");
                target.Mons.Add("landorus");
                target.Mons.Add("reshiram");
                target.Mons.Add("zekrom");
                target.Mons.Add("kyurem");
                target.Mons.Add("xerneas");
                target.Mons.Add("yveltal");
                target.Mons.Add("zygarde");
                target.Mons.Add("tapu koko");
                target.Mons.Add("tapu lele");
                target.Mons.Add("tapu bulu");
                target.Mons.Add("tapu fini");
                //target.Mons.Add("solgaleo"); // Also in ExtraSWSH
                //target.Mons.Add("lunala"); // Also in ExtraSWSH
                target.Mons.Add("necrozma");
                target.Mons.Add("nihilego");
                target.Mons.Add("buzzwole");
                target.Mons.Add("pheromosa");
                target.Mons.Add("xurkitree");
                target.Mons.Add("celesteela");
                target.Mons.Add("kartana");
                target.Mons.Add("guzzlord");
                target.Mons.Add("stakataka");
                target.Mons.Add("blacephalon");
            }
        }
        // Adds (9) [Gen 3] Starters and (42) [Gen 1-7] Legendaries available through Dynamax Adventures
        private static void AppendDexMon_ExtraSwShCompatible(string species, PokedexData target)
        {
            if (species.Equals("eternatus", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("mew"); // Poke Ball Plus
                target.Mons.Add("celebi"); // event
                target.Mons.Add("jirachi"); // event
                target.Mons.Add("victini"); // Unused Crown Tundra event
                target.Mons.Add("genesect"); // event
                target.Mons.Add("diancie");
                target.Mons.Add("volcanion"); // event
                target.Mons.Add("marshadow"); // event
                target.Mons.Add("magearna");
                target.Mons.Add("zeraora"); // Home-distribution
                target.Mons.Add("meltan"); // Tradable from Let's Go
                target.Mons.Add("melmetal"); // Tradable from Let's Go
            }
        }
        // Adds (12) Legendaries that can be transferred to SwSh, but not obtained within it


        private static void AppendDexMon_ExtraScVi(string species, PokedexData target)
        {
            if (species.Equals("miraidon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("meloetta"); // Coastal Biome event
            }
        }
        // Adds (1) Legendary obtainable in Indigo Disk
        private static void AppendDexMon_ExtraSnacksworth(string species, PokedexData target)
        {
            if (species.Equals("miraidon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("articuno");
                target.Mons.Add("zapdos");
                target.Mons.Add("moltres");
                target.Mons.Add("ho-oh");
                target.Mons.Add("lugia");
                target.Mons.Add("raikou");
                target.Mons.Add("entei");
                target.Mons.Add("suicune");
                target.Mons.Add("cobalion");
                target.Mons.Add("terrakion");
                target.Mons.Add("virizion");
                target.Mons.Add("groudon");
                target.Mons.Add("kyogre");
                target.Mons.Add("rayquaza");
                target.Mons.Add("reshiram");
                target.Mons.Add("zekrom");
                target.Mons.Add("kyurem");
                //target.Mons.Add("cosmog");
                //target.Mons.Add("cosmoem");
                target.Mons.Add("solgaleo");
                target.Mons.Add("lunala");
                target.Mons.Add("necrozma");
                target.Mons.Add("kubfu");
                target.Mons.Add("urshifu");
                target.Mons.Add("glastrier");
                target.Mons.Add("spectrier");

                target.Mons.Add("mewtwo"); // Poke Portal News event
            }
        }
        // Adds (24) Legendaries obtainable in quests from Snacksworth in ScVi
        // Adds (1) Legendary obtainable from Tera Raid events
        private static void AppendDexMon_ExtraScViCompatible(string species, PokedexData target)
        {
            if (species.Equals("miraidon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("perrserker");
                target.Mons.Add("wyrdeer");
                target.Mons.Add("ursaluna");
                target.Mons.Add("sneasler");

                target.Mons.Add("mew"); // Available in event
                target.Mons.Add("regirock");
                target.Mons.Add("regice");
                target.Mons.Add("registeel");
                target.Mons.Add("latias");
                target.Mons.Add("latios");
                target.Mons.Add("jirachi");
                target.Mons.Add("deoxys");
                target.Mons.Add("uxie");
                target.Mons.Add("mesprit");
                target.Mons.Add("azelf");
                target.Mons.Add("dialga");
                target.Mons.Add("palkia");
                target.Mons.Add("heatran");
                target.Mons.Add("regigigas");
                target.Mons.Add("giratina");
                target.Mons.Add("cresselia");
                target.Mons.Add("phione");
                target.Mons.Add("manaphy");
                target.Mons.Add("darkrai"); // Available in event
                target.Mons.Add("shayman");
                target.Mons.Add("arceus");
                target.Mons.Add("tornadus");
                target.Mons.Add("thundurus");
                target.Mons.Add("landorus");
                target.Mons.Add("keldeo");
                target.Mons.Add("diancie");
                target.Mons.Add("hoopa");
                target.Mons.Add("volcanion");
                target.Mons.Add("magearna");
                target.Mons.Add("zacian");
                target.Mons.Add("zamazenta");
                target.Mons.Add("eternatus");
                target.Mons.Add("zarude");
                target.Mons.Add("regieleki");
                target.Mons.Add("regidrago");
                target.Mons.Add("calyrex");
                target.Mons.Add("enamorus");
            }
        }
        // Adds (38) Legendaries and (4) regional form evos that can be transferred to ScVi, but not obtained within it

        private static void AppendDexMon_ExtraClassicPlus(string species, PokedexData target)
        {
            // Hacky way to prevent double-printing baby Pokemon
            if (species.Equals("pichu", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("pikachu");
            }
            else if (species.Equals("pikachu", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "pichu";
                target.Mons.Add("pikachu");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("cleffa", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("clefairy");
            }
            else if (species.Equals("clefairy", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "cleffa";
                target.Mons.Add("clefairy");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("igglybuff", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("jigglypuff");
            }
            else if (species.Equals("jigglypuff", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "igglybuff";
                target.Mons.Add("jigglypuff");
            }
            else if (species.Equals("golbat", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("crobat");
            }
            else if (species.Equals("vileplume", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("bellossom");
            }
            else if (species.Equals("poliwrath", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("politoed");
            }
            else if (species.Equals("slowbro", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("slowking");
            }
            else if (species.Equals("onix", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("steelix");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("tyrogue", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("hitmonlee");
            }
            else if (species.Equals("hitmonlee", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "tyrogue";
                target.Mons.Add("hitmonlee");
            }
            else if (species.Equals("hitmonchan", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("hitmontop");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("happiny", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("chansey");
                target.Mons.Add("blissey");
            }
            else if (species.Equals("chansey", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "happiny";
                target.Mons.Add("chansey");
                target.Mons.Add("blissey");
            }
            else if (species.Equals("seadra", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("kingdra");
            }
            else if (species.Equals("scyther", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("scizor");
                target.Mons.Add("kleavor");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("smoochum", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("jynx");
            }
            else if (species.Equals("jynx", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "smoochum";
                target.Mons.Add("jynx");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("elekid", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("electabuzz");
                target.Mons.Add("electivire");
            }
            else if (species.Equals("electabuzz", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "elekid";
                target.Mons.Add("electabuzz");
                target.Mons.Add("electivire");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("magby", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("magmar");
                target.Mons.Add("magmortar");
            }
            else if (species.Equals("magmar", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "magby";
                target.Mons.Add("magmar");
                target.Mons.Add("magmortar");
            }
            else if (species.Equals("flareon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("espeon");
                target.Mons.Add("umbreon");
                target.Mons.Add("leafeon");
                target.Mons.Add("glaceon");
                target.Mons.Add("sylveon");
            }
            else if (species.Equals("porygon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("porygon2");
                target.Mons.Add("porygon-z");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("azurill", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("marill");
            }
            else if(species.Equals("marill", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "azurill";
                target.Mons.Add("marill");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("wynaut", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("wobbuffet");
            }
            else if (species.Equals("wobbuffet", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "wynaut";
                target.Mons.Add("wobbuffet");
            }
            if (species.Equals("magneton", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("magnezone");
            }
            else if (species.Equals("lickitung", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("lickilicky");
            }
            else if (species.Equals("rhydon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("rhyperior");
            }
            else if (species.Equals("tangela", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("tangrowth");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("mime-jr", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("mr-mime");
                target.Mons.Add("mr-rime");
            }
            else if (species.Equals("mr-mime", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "mime-jr";
                target.Mons.Add("mr-mime");
                target.Mons.Add("mr-rime");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("munchlax", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("snorlax");
            }
            else if (species.Equals("snorlax", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "munchlax";
                target.Mons.Add("snorlax");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("bonsly", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("sudowoodo");
            }
            else if (species.Equals("sudowoodo", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "bonsly";
                target.Mons.Add("sudowoodo");
            }
            else if (species.Equals("aipom", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("ambipom");
            }
            else if (species.Equals("yanma", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("yanmega");
            }
            if (species.Equals("sneasel", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("weavile");
                target.Mons.Add("sneasler");
            }
            else if (species.Equals("murkrow", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("honchkrow");
            }
            else if (species.Equals("misdreavus", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("mismagius");
            }
            else if (species.Equals("gligar", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("gliscor");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("mantyke", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("mantine");
            }
            else if (species.Equals("mantine", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "mantyke";
                target.Mons.Add("mantine");
            }
            else if (species.Equals("togetic", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("togekiss");
            }
            else if (species.Equals("piloswine", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("mamoswine");
            }
            else if (species.Equals("gardevoir", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("gallade");
            }
            else if (species.Equals("nosepass", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("probopass");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("budew", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("roselia");
                target.Mons.Add("roserade");
            }
            else if (species.Equals("roselia", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "budew";
                target.Mons.Add("roselia");
                target.Mons.Add("roserade");
            }
            else if (species.Equals("dusclops", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("dusknoir");
            }
            // Hacky way to prevent double-printing baby Pokemon
            else if (species.Equals("chingling", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("chimecho");
            }
            else if (species.Equals("chimecho", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "chingling";
                target.Mons.Add("chimecho");
            }
            else if (species.Equals("glalie", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("froslass");
            }
            else if (species.Equals("persian", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("perrserker");
            }
            else if (species.Equals("farfetchd", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("sirfetchd");
            }
            else if (species.Equals("linoone", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("obstagoon");
            }
            else if (species.Equals("corsola", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("cursola");
            }
            else if (species.Equals("qwilfish", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("overqwil");
            }
            else if (species.Equals("registeel", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("regieleki");
                target.Mons.Add("regidrago");
            }
            else if (species.Equals("ursaring", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("ursaluna");
            }
            else if (species.Equals("stantler", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("wyrdeer");
            }
            else if (species.Equals("basculin", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("basculegion");
            }
            else if (species.Equals("landorus", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("enamorus");
            }
            else if (species.Equals("primeape", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("annihilape");
            }
            else if (species.Equals("quagsire", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("clodsire");
            }
            else if (species.Equals("girafarig", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("farigiraf");
            }
            else if (species.Equals("dunsparce", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("dudunsparce");
            }
            else if (species.Equals("bisharp", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("kingambit");
            }
            else if (species.Equals("appletun", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("dipplin");
                target.Mons.Add("hydrapple");
            }
            else if (species.Equals("duraludon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("archaludon");
            }
            else if (species.Equals("dugtrio", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("wiglett");
                target.Mons.Add("wugtrio");
            }
            else if (species.Equals("tentacruel", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("toedscool");
                target.Mons.Add("toedscruel");
            }
            else if (species.Equals("polteageist", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("poltchageist");
                target.Mons.Add("sinistcha");
            }
        }

        private static void AppendDexMon_ScViBlueberry(string species, PokedexData target)
		{
			if (species.Equals("archaludon", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("applin");
				target.Mons.Add("dipplin");
			}
		}

		private static void ExportConstants(string fileName, List<PokedexData> data, Dictionary<string, List<PokedexData>> regionData)
		{
			Console.WriteLine($"Exporting to '{fileName}'");
			Directory.CreateDirectory(Path.GetDirectoryName(fileName));

			StringBuilder content = new StringBuilder();

			content.AppendLine("// == WARNING ==");
			content.AppendLine("// DO NOT EDIT THIS FILE");
			content.AppendLine("// This file was automatically generated by PokemonDataGenerator");
			content.AppendLine("//");
			content.AppendLine($"");

			// Variants
			content.AppendLine($"#define POKEDEX_VARIANT_START    0");
			content.AppendLine($"");

			int counter = 0;
			foreach (var dex in data)
			{
				int dexIdx = counter++;
				content.AppendLine($"#define POKEDEX_VARIANT_{FormatKeyword(dex.InternalName)}    {dexIdx}");
			}

			content.AppendLine($"");
			content.AppendLine($"#define POKEDEX_VARIANT_END    POKEDEX_VARIANT_{FormatKeyword(data.Last().InternalName)}");
			content.AppendLine($"");
			content.AppendLine($"#define POKEDEX_VARIANT_COUNT    (POKEDEX_VARIANT_END - POKEDEX_VARIANT_START + 1)");
			content.AppendLine($"#define POKEDEX_VARIANT_NONE     (255)");
			content.AppendLine($"");

			// Regions
			content.AppendLine($"");
			content.AppendLine($"#define POKEDEX_REGION_START    0");
			content.AppendLine($"");

			counter = 0;
			foreach (var region in regionData)
			{
				int regionIdx = counter++;
				content.AppendLine($"#define POKEDEX_REGION_{FormatKeyword(region.Key)}    {regionIdx}");
			}

			content.AppendLine($"");
			content.AppendLine($"#define POKEDEX_REGION_END    POKEDEX_REGION_{FormatKeyword(regionData.Last().Key)}");
			content.AppendLine($"");
			content.AppendLine($"#define POKEDEX_REGION_COUNT    (POKEDEX_REGION_END - POKEDEX_REGION_START + 1)");
			content.AppendLine($"#define POKEDEX_REGION_NONE     (255)");
			content.AppendLine($"");

			string fullStr = content.ToString();
			File.WriteAllText(fileName, fullStr);
		}

		private static void ExportData(string fileName, List<PokedexData> data, Dictionary<string, List<PokedexData>> regionData)
		{
			Console.WriteLine($"Exporting to '{fileName}'");
			Directory.CreateDirectory(Path.GetDirectoryName(fileName));

			StringBuilder content = new StringBuilder();

			content.AppendLine("// == WARNING ==");
			content.AppendLine("// DO NOT EDIT THIS FILE");
			content.AppendLine("// This file was automatically generated by PokemonDataGenerator");
			content.AppendLine("//");

			// Add all mons in dex
			foreach (var dex in data)
			{
				content.AppendLine($"");
				content.AppendLine($"const u8 sRogueDexVariantName_{FormatKeyword(dex.InternalName)}[] = _(\"{dex.DisplayName}\");");

				content.AppendLine($"");
				content.AppendLine($"const u16 sRogueDexVariant_{FormatKeyword(dex.InternalName)}[] = ");
				content.AppendLine($"{{");

				foreach (var mon in dex.Mons)
				{
					content.AppendLine($"	SPECIES_{FormatKeyword(mon)},");
				}

				content.AppendLine($"}};");
			}

			// Add all regional dex variants
			foreach (var region in regionData)
			{
				string displayName = region.Key;
				displayName = char.ToUpper(displayName[0]).ToString() + string.Join("", displayName.Skip(1));

				content.AppendLine($"");
				content.AppendLine($"const u8 sRogueDexRegionName_{FormatKeyword(region.Key)}[] = _(\"{displayName}\");");

				content.AppendLine($"");
				content.AppendLine($"const u16 sRogueDexRegion_{FormatKeyword(region.Key)}[] = ");
				content.AppendLine($"{{");

				foreach (var dex in region.Value)
				{
					content.AppendLine($"\tPOKEDEX_VARIANT_{FormatKeyword(dex.InternalName)},");
				}

				content.AppendLine($"}};");
			}

			// Group all of the data into table

			// Variant
			content.AppendLine($"");
			content.AppendLine($"const struct RoguePokedexVariant gPokedexVariants[POKEDEX_VARIANT_COUNT] = ");

			content.AppendLine($"{{");
			foreach (var dex in data)
			{
				content.AppendLine($"\t[POKEDEX_VARIANT_{FormatKeyword(dex.InternalName)}] = ");
				content.AppendLine($"\t{{");
				content.AppendLine($"\t\t.displayName = sRogueDexVariantName_{FormatKeyword(dex.InternalName)},");
				content.AppendLine($"\t\t.speciesList = sRogueDexVariant_{FormatKeyword(dex.InternalName)},");
				content.AppendLine($"\t\t.speciesCount = ARRAY_COUNT(sRogueDexVariant_{FormatKeyword(dex.InternalName)}),");
				content.AppendLine($"\t\t.genLimit = {dex.GenLimit},");
				content.AppendLine($"\t}},");
			}
			content.AppendLine($"}};");

			// Regions
			content.AppendLine($"");
			content.AppendLine($"const struct RoguePokedexRegion gPokedexRegions[POKEDEX_REGION_COUNT] = ");

			// Group all of the data into table
			content.AppendLine($"{{");
			foreach (var region in regionData)
			{
				content.AppendLine($"\t[POKEDEX_REGION_{FormatKeyword(region.Key)}] = ");
				content.AppendLine($"\t{{");
				content.AppendLine($"\t\t.displayName = sRogueDexRegionName_{FormatKeyword(region.Key)},");
				content.AppendLine($"\t\t.variantList = sRogueDexRegion_{FormatKeyword(region.Key)},");
				content.AppendLine($"\t\t.variantCount = ARRAY_COUNT(sRogueDexRegion_{FormatKeyword(region.Key)}),");
				content.AppendLine($"\t}},");
			}
			content.AppendLine($"}};");

			string fullStr = content.ToString();
			File.WriteAllText(fileName, fullStr);
		}

		private static string FormatKeyword(string keyword)
		{
			return keyword.Trim()
				.Replace(".", "")
				.Replace("’", "")
				.Replace("'", "")
				.Replace("%", "")
				.Replace(":", "")
				.Replace(" ", "_")
				.Replace("-", "_")
				.Replace("é", "e")
				.ToUpper();
		}
	}
}
