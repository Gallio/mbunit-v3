using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Win32;

namespace Gallio.TDNetRunner.Facade
{
    /// <summary>
    /// Provides access to TDNet options (via the registry).
    /// </summary>
    [Serializable]
    public sealed class FacadeOptions
    {
        private const string RootRegKey = @"HKEY_CURRENT_USER\Software\MutantDesign\TestDriven.NET\Options\General";

        private IList<string> filterCategoryNames;
        private FacadeFilterCategoryMode filterCategoryMode;

        public FacadeOptions()
        {
            filterCategoryNames = new string[0];
            filterCategoryMode = FacadeFilterCategoryMode.Disabled;
        }

        public IList<string> FilterCategoryNames
        {
            get { return new ReadOnlyCollection<string>(filterCategoryNames); }
            set
            {
                if (value == null || value.Contains(null))
                    throw new ArgumentNullException("value");
                filterCategoryNames = value;
            }
        }

        public FacadeFilterCategoryMode FilterCategoryMode
        {
            get { return filterCategoryMode; }
            set { filterCategoryMode = value; }
        }

        public static FacadeOptions ReadFromRegistry()
        {
            return new FacadeOptions
            {
                FilterCategoryNames = GetFilterCategoryNames(),
                FilterCategoryMode = GetFilterCategoryMode()
            };
        }

        private static string[] GetFilterCategoryNames()
        {
            string value = Registry.GetValue(RootRegKey, "Categories", null) as string;
            if (string.IsNullOrEmpty(value))
                return new string[0];

            var names = value.Split(';');
            for (int i = 0; i < names.Length; i++)
                names[i] = names[i].Trim();
            return names;
        }

        private static FacadeFilterCategoryMode GetFilterCategoryMode()
        {
            string value = Registry.GetValue(RootRegKey, "CategoriesFilter", null) as string;

            if (value != null)
            {
                if (string.Compare(value, "include", true) == 0)
                    return FacadeFilterCategoryMode.Include;
                if (string.Compare(value, "exclude", true) == 0)
                    return FacadeFilterCategoryMode.Exclude;
            }

            return FacadeFilterCategoryMode.Disabled;
        }
    }
}
