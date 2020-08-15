using System;
using JetBrains.Annotations;

namespace YAPO.Configuration.Models {
    [UsedImplicitly]
    public class SorterConfigurationSave {
        public DateTime LastSaved;
        public SorterConfiguration Other;
        public SorterConfiguration Party;
        public string SaveName;
    }
}
