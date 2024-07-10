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
				fullDexes.Add(GatherResourceDexData("rogue_modern", "Modern", 9, "Rogue Dex.csv"));

				// Hacky, gather based on 3 but allow up to 9 in reality
				var dexData = GatherDexData("rogue_classicplus", "Classic Plus", 4, "national");
				dexData.GenLimit = 9;
				fullDexes.Add(dexData);

				fullDexes.Add(GatherDexData("kanto_RBY", "Red/Blue/Yellow", 1, "kanto"));
                fullDexes.Add(GatherDexData("kanto_letsgo", "Let's Go!", 7, "letsgo-kanto"));
                fullDexes.Add(GatherDexData("kanto_RBYplus", "Let's Go! Plus", 9, "letsgo-kanto"));

                fullDexes.Add(GatherDexData("johto_GSC", "Gold/Silver/Crystal", 2, "original-johto"));
				fullDexes.Add(GatherDexData("johto_HGSS", "HeartGold/SoulSilver", 4, "updated-johto"));
                fullDexes.Add(GatherDexData("johto_GSCplus", "Crystal Plus", 9, "original-johto"));

                fullDexes.Add(GatherDexData("hoenn_RSE", "Ruby/Sapphire/Emerald", 3, "hoenn"));
				fullDexes.Add(GatherDexData("hoenn_ORAS", "OmegaRuby/AlphaSapphire", 6, "updated-hoenn"));
                fullDexes.Add(GatherDexData("hoenn_RSEplus", "Emerald Plus", 9, "hoenn"));

                fullDexes.Add(GatherDexData("sinnoh_DP", "Diamond/Pearl", 4, "original-sinnoh"));
				fullDexes.Add(GatherDexData("sinnoh_PL", "Platinum", 4, "extended-sinnoh"));
                fullDexes.Add(GatherDexData("sinnoh_PLplus", "Platinum Plus", 9, "extended-sinnoh"));

                fullDexes.Add(GatherDexData("unova_BW", "Black/White", 5, "original-unova"));
                fullDexes.Add(GatherDexData("unova_BWplus", "Black/White Plus", 5, "original-unova"));
                fullDexes.Add(GatherDexData("unova_BW2", "Black 2/White 2", 5, "updated-unova"));
                fullDexes.Add(GatherDexData("unova_BW2plus", "Black 2/White 2 Plus", 9, "updated-unova"));

                fullDexes.Add(GatherDexData("kalos", "X/Y", 6, "kalos-central", "kalos-coastal", "kalos-mountain"));
                fullDexes.Add(GatherDexData("kalos_XYplus", "X/Y Plus", 9, "kalos-central", "kalos-coastal", "kalos-mountain"));

                fullDexes.Add(GatherDexData("alola_SM", "Sun/Moon", 7, "original-alola", "original-melemele", "original-akala", "original-ulaula", "original-poni"));
				fullDexes.Add(GatherDexData("alola_USUM", "UltraSun/UltraMoon", 7, "updated-alola", "updated-melemele", "updated-akala", "updated-ulaula", "updated-poni"));
                fullDexes.Add(GatherDexData("alola_USUMplus", "UltraSun/UltraMoon Plus", 7, "updated-alola", "updated-melemele", "updated-akala", "updated-ulaula", "updated-poni"));

                fullDexes.Add(GatherDexData("galar_swsh", "Sword/Shield", 8, "galar"));
				fullDexes.Add(GatherDexData("galar_isleofarmor", "IsleOfArmor", 8, "isle-of-armor"));
				fullDexes.Add(GatherDexData("galar_crowntundra", "CrownTundra", 8, "crown-tundra"));
				fullDexes.Add(GatherDexData("galar_fulldlc", "Sword/Shield + DLC", 8, "galar", "isle-of-armor", "crown-tundra"));
                fullDexes.Add(GatherDexData("galar_swshplus", "Sword/Shield Plus", 8, "galar", "isle-of-armor", "crown-tundra"));

                fullDexes.Add(GatherDexData("paldea_scvi", "Scarlet/Violet", 9, "paldea"));
				fullDexes.Add(GatherDexData("paldea_kitakami", "The Teal Mask", 9, "kitakami"));
				fullDexes.Add(GatherDexData("paldea_blueberry", "Indigo Disk", 9, "blueberry"));
				fullDexes.Add(GatherDexData("paldea_fulldlc", "Scarlet/Violet + DLC", 9, "paldea", "kitakami", "blueberry"));
                fullDexes.Add(GatherDexData("paldea_scviplus", "Scarlet/Violet Plus", 9, "paldea", "kitakami", "blueberry"));

                fullDexes.Add(GatherResourceDexData("orre_colo", "Colosseum", 4, "Orre/Colo Dex.csv"));
                fullDexes.Add(GatherResourceDexData("orre_xd", "XD: Gale of Darkness", 9, "Orre/XD Dex.csv"));
                fullDexes.Add(GatherResourceDexData("orre_full", "Colosseum & XD", 4, "Orre/Orre Dex.csv"));
                fullDexes.Add(GatherResourceDexData("orre_plus", "Colosseum & XD Plus", 9, "Orre/Orre Dex Plus.csv"));

                fullDexes.Add(GatherResourceDexData("fiore_ranger", "Ranger", 3, "Ranger/Fiore Dex.csv"));
                fullDexes.Add(GatherResourceDexData("fiore_rangerplus", "Ranger Plus", 9, "Ranger/Fiore Dex Plus.csv"));

                fullDexes.Add(GatherResourceDexData("almia_shadows", "Shadows of Almia", 4, "Ranger/Almia Dex.csv"));
                fullDexes.Add(GatherResourceDexData("almia_shadowsplus", "Shadows of Almia Plus", 9, "Ranger/Almia Dex Plus.csv"));

                fullDexes.Add(GatherResourceDexData("oblivia_guardian", "Guardian Signs", 4, "Ranger/Oblivia Dex.csv"));
                fullDexes.Add(GatherResourceDexData("oblivia_guardianplus", "Guardian Signs Plus", 9, "Ranger/Oblivia Dex Plus.csv"));

                fullDexes.Add(GatherDexData("ransei_conquest", "Conquest", 5, "conquest-gallery"));
                fullDexes.Add(GatherDexData("ransei_conquestplus", "Conquest Plus", 9, "conquest-gallery"));

                fullDexes.Add(GatherDexData("hisui_legendsarceus", "Legends Arceus", 8, "hisui"));
                fullDexes.Add(GatherDexData("hisui_legendsplus", "Legends Arceus Plus", 9, "hisui"));

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

			bool isHGSS = target.InternalName.Equals("johto_HGSS", StringComparison.CurrentCultureIgnoreCase);
			//bool isClassicPlus = target.InternalName.Equals("rogue_classicplus", StringComparison.CurrentCultureIgnoreCase);

			bool isPlusGen1 = target.InternalName.Equals("kanto_RBYplus", StringComparison.CurrentCultureIgnoreCase);
            bool isPlusGen2 = target.InternalName.Equals("johto_GSCplus", StringComparison.CurrentCultureIgnoreCase);
            bool isPlusGen3 = target.InternalName.Equals("hoenn_RSEplus", StringComparison.CurrentCultureIgnoreCase);
            bool isPlusGen4 = target.InternalName.Equals("sinnoh_PLplus", StringComparison.CurrentCultureIgnoreCase)
				|| target.InternalName.Equals("rogue_classicplus", StringComparison.CurrentCultureIgnoreCase);
            bool isPlusGen5 = target.InternalName.Equals("unova_BWplus", StringComparison.CurrentCultureIgnoreCase)
				|| target.InternalName.Equals("unova-BW2plus", StringComparison.CurrentCultureIgnoreCase)
                || target.InternalName.Equals("ransei_conquestplus", StringComparison.CurrentCultureIgnoreCase);
            bool isPlusGen6 = target.InternalName.Equals("kalos_XYplus", StringComparison.CurrentCultureIgnoreCase);
            bool isPlusGen7 = target.InternalName.Equals("alola_usumplus", StringComparison.CurrentCultureIgnoreCase);
            bool isPlusGen8 = target.InternalName.Equals("galar_swshplus", StringComparison.CurrentCultureIgnoreCase);
            bool isPlusHisui = target.InternalName.Equals("hisui_legendsplus", StringComparison.CurrentCultureIgnoreCase);
            bool isPlusGen9 = target.InternalName.Equals("paldea_scviplus", StringComparison.CurrentCultureIgnoreCase);

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

					// HGSS dex is missing a lot of evos from regional dex, as they weren't techincally obtainable until post game
					// so forcefully insert them here
					if (isHGSS)
						AppendDexMon_ExtraHGSS(species, target);
					//else if (isClassicPlus)
					//	AppendDexMon_ExtraClassicPlus(species, target);

					if (isPlusGen1)
                    {
                        AppendDexMon_ExtraGen2(species, target);
                        AppendDexMon_ExtraGen3(species, target);
                        AppendDexMon_ExtraGen4(species, target);
                        AppendDexMon_ExtraGen6(species, target);
                        AppendDexMon_ExtraGen8(species, target);
                        AppendDexMon_ExtraHisui(species, target);
                        AppendDexMon_ExtraGen9(species, target);
                    }
					else if (isPlusGen2)
                    {
                        AppendDexMon_ExtraGen3(species, target);
                        AppendDexMon_ExtraGen4(species, target);
                        AppendDexMon_ExtraGen6(species, target);
                        AppendDexMon_ExtraGen8(species, target);
                        AppendDexMon_ExtraHisui(species, target);
                        AppendDexMon_ExtraGen9(species, target);
                    }
                    else if (isPlusGen3)
                    {
                        AppendDexMon_ExtraGen4(species, target);
                        AppendDexMon_ExtraGen6(species, target);
                        AppendDexMon_ExtraGen8(species, target);
                        AppendDexMon_ExtraHisui(species, target);
                        AppendDexMon_ExtraGen9(species, target);
                    }
                    else if (isPlusGen4 || isPlusGen5)
                    {
                        AppendDexMon_ExtraGen6(species, target);
                        AppendDexMon_ExtraGen8(species, target);
                        AppendDexMon_ExtraHisui(species, target);
                        AppendDexMon_ExtraGen9(species, target);
                    }
                    else if (isPlusGen6 || isPlusGen7)
                    {
                        AppendDexMon_ExtraGen8(species, target);
                        AppendDexMon_ExtraHisui(species, target);
                        AppendDexMon_ExtraGen9(species, target);
                    }
                    else if (isPlusGen8)
                    {
                        AppendDexMon_ExtraHisui(species, target);
                        AppendDexMon_ExtraGen9(species, target);
                    }
                    else if (isPlusHisui)
                    {
                        AppendDexMon_ExtraGen9(species, target);
                    }
                    else if (isPlusGen9)
                    {
                        AppendDexMon_ExtraHisui(species, target);
                    }

                    else if (dexId == "blueberry")
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

		private static void AppendDexMon_ExtraHGSS(string species, PokedexData target)
		{
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
			else if (species.Equals("electabuzz", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("electivire");
			}
			else if (species.Equals("magmar", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("magmortar");
			}
			else if (species.Equals("umbreon", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("leafeon");
				target.Mons.Add("glaceon");
			}
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
			else if (species.Equals("piloswine", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("mamoswine");
			}
			else if (species.Equals("kirlia", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("gallade");
			}
			else if (species.Equals("nosepass", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("probopass");
			}
			else if (species.Equals("roselia", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("roserade");
			}
			else if (species.Equals("dusclops", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("dusknoir");
			}
			else if (species.Equals("snorunt", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("froslass");
			}
		}

		private static void AppendDexMon_ExtraClassicPlus(string species, PokedexData target)
		{
			// Add any future evos from past gen3 
			// Also reorder dex so families are grouped together
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
			else if (species.Equals("primeape", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("annihilape");
			}
			else if (species.Equals("poliwrath", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("politoed");
			}
			else if (species.Equals("magneton", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("magnezone");
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
			else if (species.Equals("lickitung", StringComparison.CurrentCultureIgnoreCase))
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
				target.Mons.Add("blissey");
			}
			else if (species.Equals("tangela", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("tangrowth");
			}
			else if (species.Equals("seadra", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("kingdra");
			}
			else if (species.Equals("persian", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("perrserker");
			}
			else if (species.Equals("farfetchd", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("sirfetchd");
			}
			else if (species.Equals("mr-mime", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons[target.Mons.Count - 1] = "mime-jr";
				target.Mons.Add("mr-mime");
				target.Mons.Add("mr-rime");
			}
			else if (species.Equals("scyther", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("scizor");
				target.Mons.Add("kleavor");
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
				target.Mons.Add("electivire");
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
				target.Mons.Add("porygon_z");
			}
			else if (species.Equals("snorlax", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons[target.Mons.Count - 1] = "munchlax";
				target.Mons.Add("snorlax");
			}
			else if (species.Equals("togetic", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("togekiss");
			}
			else if (species.Equals("quagsire", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("clodsire");
			}
			else if (species.Equals("qwilfish", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("overqwil");
			}
			else if (species.Equals("corsola", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("cursola");
			}
			else if (species.Equals("marill", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons[target.Mons.Count - 1] = "azurill";
				target.Mons.Add("marill");
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
			else if (species.Equals("murkrow", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("honchkrow");
			}
			else if (species.Equals("misdreavus", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("mismagius");
			}
			else if (species.Equals("wobbuffet", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons[target.Mons.Count - 1] = "wynaut";
				target.Mons.Add("wobbuffet");
			}
			else if (species.Equals("girafarig", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("farigiraf");
			}
			else if (species.Equals("dunsparce", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("dudunsparce");
			}
			else if (species.Equals("gligar", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("gliscor");
			}
			else if (species.Equals("sneasel", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("weavile");
				target.Mons.Add("sneasler");
			}
			else if (species.Equals("ursaring", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("ursaluna");
			}
			else if (species.Equals("piloswine", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("mamoswine");
			}
			else if (species.Equals("mantine", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons[target.Mons.Count - 1] = "mantyke";
				target.Mons.Add("mantine");
			}
			else if (species.Equals("stantler", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("wyrdeer");
			}
			else if (species.Equals("linoone", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("obstagoon");
			}
			else if (species.Equals("gardevoir", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("gallade");
			}
			else if (species.Equals("nosepass", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("probopass");
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
			else if (species.Equals("chimecho", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons[target.Mons.Count - 1] = "chingling";
				target.Mons.Add("chimecho");
			}
			else if (species.Equals("glalie", StringComparison.CurrentCultureIgnoreCase))
			{
				target.Mons.Add("froslass");
			}
		}

        private static void AppendDexMon_ExtraGen2(string species, PokedexData target)
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
                target.Mons.Add("kleavor");
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
                target.Mons.Add("leafeon");
                target.Mons.Add("glaceon");
                target.Mons.Add("sylveon");
            }
            else if (species.Equals("porygon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("porygon2");
                target.Mons.Add("porygon_z");
            }
        }
        private static void AppendDexMon_ExtraGen3(string species, PokedexData target)
        {
            if (species.Equals("marill", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "azurill";
                target.Mons.Add("marill");
            }
        }
        private static void AppendDexMon_ExtraGen4(string species, PokedexData target)
        {
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
            else if (species.Equals("chansey", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "happiny";
                target.Mons.Add("chansey");
            }
            else if (species.Equals("tangela", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("tangrowth");
            }
            else if (species.Equals("mr-mime", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "mime-jr";
                target.Mons.Add("mr-mime");
            }
            else if (species.Equals("electabuzz", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("electivire");
            }
            else if (species.Equals("magmar", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("magmortar");
            }
            else if (species.Equals("umbreon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("leafeon");
                target.Mons.Add("glaceon");
                target.Mons.Add("sylveon");
            }
            else if (species.Equals("porygon2", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("porygon_z");
            }
            else if (species.Equals("snorlax", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "munchlax";
                target.Mons.Add("snorlax");
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
            else if (species.Equals("wobbuffet", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "wynaut";
                target.Mons.Add("wobbuffet");
            }
            else if (species.Equals("gligar", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("gliscor");
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
            else if (species.Equals("chimecho", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons[target.Mons.Count - 1] = "chingling";
                target.Mons.Add("chimecho");
            }
            else if (species.Equals("glalie", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("froslass");
            }
        }
        private static void AppendDexMon_ExtraGen6(string species, PokedexData target)
        {
            if (species.Equals("glaceon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("sylveon");
            }
        }
        private static void AppendDexMon_ExtraGen8(string species, PokedexData target)
        {
            if (species.Equals("persian", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("perrserker");
            }
            else if (species.Equals("farfetchd", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("sirfetchd");
            }
            else if (species.Equals("mr-mime", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("mr-rime");
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
        }
        private static void AppendDexMon_ExtraHisui(string species, PokedexData target)
        {
            if (species.Equals("weavile", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("sneasler");
            }
            else if (species.Equals("scizor", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("kleavor");
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
        }
        private static void AppendDexMon_ExtraGen9(string species, PokedexData target)
        {
            if (species.Equals("primeape", StringComparison.CurrentCultureIgnoreCase))
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

        private static void AppendDexMon_ExtraGen2Eevee(string species, PokedexData target)
        {
            if (species.Equals("flareon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("espeon");
                target.Mons.Add("umbreon");
                target.Mons.Add("leafeon");
                target.Mons.Add("glaceon");
                target.Mons.Add("sylveon");
            }
        }
        private static void AppendDexMon_ExtraGen4Eevee(string species, PokedexData target)
        {
            if (species.Equals("umbreon", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("leafeon");
                target.Mons.Add("glaceon");
                target.Mons.Add("sylveon");
            }
        }

        private static void AppendDexMon_ExtraGen2Scyther(string species, PokedexData target)
        {
            if (species.Equals("scyther", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("scizor");
                target.Mons.Add("kleavor");
            }
        }
        private static void AppendDexMon_ExtraGen9Scyther(string species, PokedexData target)
        {
            if (species.Equals("scizor", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("kleavor");
            }
        }

        private static void AppendDexMon_ExtraGen4Sneasel(string species, PokedexData target)
        {
            if (species.Equals("sneasel", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("weavile");
                target.Mons.Add("sneasler");
            }
        }
        private static void AppendDexMon_ExtraGen9Sneasel(string species, PokedexData target)
        {
            if (species.Equals("weavile", StringComparison.CurrentCultureIgnoreCase))
            {
                target.Mons.Add("sneasler");
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
