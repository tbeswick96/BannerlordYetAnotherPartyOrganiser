using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace YAPO.Fixes.Formations
{
    public class FixedFormationsSaveableTypeDefiner : SaveableTypeDefiner
    {
        public FixedFormationsSaveableTypeDefiner() : base(13537200)
        {
            // Nothing to do
        }

        protected override void DefineEnumTypes()
        {
            AddEnumDefinition(typeof(FormationClass), 13537250);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<BasicCharacterObject, FormationClass>));
        }
    }
}
