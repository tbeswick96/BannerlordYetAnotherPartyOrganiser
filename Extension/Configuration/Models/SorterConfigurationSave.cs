using System;
using JetBrains.Annotations;

namespace YAPO.Configuration.Models
{
    [UsedImplicitly]
    public class SorterConfigurationSave
    {
        public string Id;
        public DateTime LastSaved;
        public SorterConfiguration Party;
        public SorterConfiguration Other;
    }
}
