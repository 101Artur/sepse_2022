using Sandbox.Game.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage.Game;

namespace SolarMap.Managers
{
	public class OreManager
	{

		private readonly StringBuilder sb = new StringBuilder();

		/// <summary>
		/// Returns a string of resources seperated by comma. 
		/// </summary>
		public List<string> GetResources(MyPlanet myPlanet)
		{
			return GetResourcesFromOreMappings(myPlanet.Generator.OreMappings).Distinct().ToList();
		}

		/// <summary>
		/// Returns a string of resources seperated by comma. 
		/// </summary>
		public string GetResourcesAsString(MyPlanet myPlanet)
		{

			sb.Clear();

			// Non-distinct ores. 
			List<string> resources = GetResourcesFromOreMappings(myPlanet.Generator.OreMappings);

			// Check surface textures for ice and snow.
			string surfaceMaterial = myPlanet.Generator.DefaultSurfaceMaterial.Material;
			if (surfaceMaterial.Contains("Snow") || surfaceMaterial.Contains("Ice"))
				resources.Add("H2O");

			// Distinct ores. 
			string[] distinctResources = resources.Distinct().ToArray();

			// Create the string. 
			for (int i = 0; i < distinctResources.Length; i++)
			{
				sb.Append(distinctResources[i]);
				sb.Append(i + 1 < distinctResources.Length ? ", " : ".");
				if (i == 4 || (i != 0 && i % 12 == 0))
					sb.AppendLine();
			}

			return sb.ToString();

		}

		/// <summary>
		/// Return non-distinct ore mappings. 
		/// </summary>
		private List<string> GetResourcesFromOreMappings(MyPlanetOreMapping[] oreMappings)
		{

			List<string> resources = new List<string>();

			for (int i = 0; i < oreMappings.Length; i++)
			{

				string[] split = oreMappings[i].Type.Split('_'); // Types are typically written as e.g. "Iron_02", or "Nickel_01".
				string resource = split.Length > 0 ? split[0] : oreMappings[i].Type; // Retrieve e.g. "Iron" or the whole Type name. 
				resource = TryGetElSymbol(resource); // Attempt to get the elemental symbol, first two characters, or whole Type name. 

				// Check for H2O, and split it into two.
				/*
				if (resource.Contains("H2O"))
				{
					resources.AddRange(new List<string> { "H", "O" });
				}
				else
				{
					resources.Add(resource);
				}
				*/

				resources.Add(resource);

			}

			return resources;

		}

		/// <summary>
		/// Returns the elemental symbol or returns default first two characters of ore name or full ore name. 
		/// </summary>
		private string TryGetElSymbol(string ore)
		{
			string atom;
			if (OreToElSymbol.TryGetValue(ore, out atom))
			{
				return atom;
			}
			else
			{
				return ore.Length > 2 ? ore.Substring(0, 2) : ore;
			}
		}

		/// <summary>
		/// Dictionary of ore names to their elemental symbols.
		/// </summary>
		private readonly Dictionary<string, string> OreToElSymbol = new Dictionary<string, string>()
		{

			{ "Lithium", "Li" },
			{ "Beryllium", "Be" },
			{ "Boron", "B" },
			{ "Carbon", "C" },
			{ "Nitrogen", "N" },
			{ "Oxygen", "O" },
			{ "Fluorine", "F" },

			{ "Sodium", "Na" },
			{ "Magnesium", "Mg" },
			{ "Aluminium", "Al" },
			{ "Silicon", "Si" },
			{ "Phosphorus", "P" },
			{ "Sulfur", "S" },
			{ "Chlorine", "Cl" },

			{ "Potassium", "K" },
			{ "Calcium", "Ca" },
			{ "Scandium", "Sc" },
			{ "Titanium", "Ti" },
			{ "Vanadium", "V" },
			{ "Chromium", "Cr" },
			{ "Manganese", "Mn" },
			{ "Iron", "Fe" },
			{ "Cobalt", "Co" },
			{ "Copper", "Cu" },
			{ "Zinc", "Zn" },
			{ "Gallium", "Ga" },
			{ "Germanium", "Ge" },
			{ "Arsenic", "As" },
			{ "Selenium", "Se" },
			{ "Bromine", "Br" },

			{ "Rubidium", "Rb" },
			{ "Strontium", "Sr" },
			{ "Yttrium", "Y" },
			{ "Zirconium", "Zr" },
			{ "Niobium", "Nb" },
			{ "Molybdenum", "Mo" },
			{ "Technetium", "Tc" },
			{ "Ruthenium", "Ru" },
			{ "Rhodium", "Rh" },
			{ "Palladium", "Pd" },
			{ "Silver", "Ag" },
			{ "Cadmium", "Cd" },
			{ "Indium", "In" },
			{ "Tin", "Sn" },
			{ "Antimony", "Sb" },
			{ "Tellurium", "Te" },
			{ "Iodine", "I" },

			{ "Caesium", "Cs" },
			{ "Barium", "Ba" },
				{ "Lanthanum", "La" },
				{ "Cerium", "Ce" },
				{ "Praseodymium", "Pr" },
				{ "Neodymium", "Nd" },
				{ "Promethium", "Pm" },
				{ "Samarium", "Sm" },
				{ "Europium", "Eu" },
				{ "Gadolinium", "Gd" },
				{ "Terbium", "Tb" },
				{ "Dysprosium", "Dy" },
				{ "Holmium", "Ho" },
				{ "Erbium", "Er" },
				{ "Thulium", "Tm" },
				{ "Ytterbium", "Yb" },
				{ "Lutetium", "Lu" },
			{ "Hafnium", "Hf" },
			{ "Tantalum", "Ta" },
			{ "Tungsten", "W" },
			{ "Rhenium", "Re" },
			{ "Osmium", "Os" },
			{ "Iridium", "Ir" },
			{ "Platinum", "Pt" },
			{ "Gold", "Au" },
			{ "Mercury", "Hg" },
			{ "Thallium", "Tl" },
			{ "Lead", "Pb" },
			{ "Bismuth", "Bi" },
			{ "Polonium", "Pt" },
			{ "Astatine", "At" },

			{ "Francium", "Fr" },
			{ "Radium", "Ra" },
				{ "Actinium", "Ac" },
				{ "Thorium", "Th" },
				{ "Protactinium", "Pa" },
				{ "Uranium", "U" },
				{ "Neptunium", "Np" },
				{ "Plutonium", "Pu" },
				{ "Americium", "Am" },
				{ "Curium", "Cm" },
				{ "Berkelium", "Bk" },
				{ "Californium", "Cf" },
				{ "Einsteinium", "Es" },
				{ "Fermium", "Fm" },
				{ "Mendelevium", "Md" },
				{ "Nobelium", "No" },
				{ "Lawrencium", "Lr" },
			{ "Rutherfordium", "Rf" },
			{ "Dubnium", "Db" },
			{ "Seaborgium", "Sg" },
			{ "Bohrium", "Bh" },
			{ "Hassium", "Hs" },
			{ "Meitnerium", "Mt" },
			{ "Darmstadtium", "Ds" },
			{ "Roentgenium", "Rg" },
			{ "Copernicium", "Cn" },
			{ "Ununtrium", "Uut" },
			{ "Flerovium", "Fl" },
			{ "Ununpentium", "Uup" },
			{ "Livermorium", "Lv" },
			{ "Ununseptium", "Uus" },

			// SE
			{ "Ice", "H2O" },
			{ "Uraninite", "U" },
			{ "TritonIce", "H20" },
			{ "Snow", "H20" },

			// ...
			{ "Water", "H2O" },

			// Workshop
			{ "Bauxite", "Al" },
			{ "Coal", "C" },
			{ "Autunite", "Ae" },
			{ "Stromatolith", "So" },
			{ "Spice", "Pp" },
			{ "Duranium", "Dr" },
			{ "Azurium", "Az" },
			{ "Faradium", "Fd" },
			{ "Hyperium", "Hp" },
			{ "Kaelnium", "Kl" },
			{ "Kaldaryn", "Kr" },
			{ "Petracite", "Pc" },
			{ "R-Delta", "Rt" },

		};

	}
}
