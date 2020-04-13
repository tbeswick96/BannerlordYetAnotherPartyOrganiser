using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace TroopManager {
    public class SavableSorterTypeDefiner : SaveableTypeDefiner {
        public SavableSorterTypeDefiner() : base(13337000) { }

        protected override void DefineClassTypes() {
            AddClassDefinition(typeof(Sorter), 1);
        }

        protected override void DefineContainerDefinitions() {
            ConstructContainerDefinition(typeof(List<Sorter>));
            ConstructContainerDefinition(typeof(Dictionary<string, Sorter>));
            ConstructContainerDefinition(typeof(Dictionary<MBGUID, Sorter>));
        }

        protected override void DefineGenericClassDefinitions() {
            ConstructGenericClassDefinition(typeof(MBObjectManager.ObjectTypeRecord<Sorter>));
        }

        protected override void DefineEnumTypes() {
            AddEnumDefinition(typeof(SortingMode), 2);
            AddEnumDefinition(typeof(SortingDirection), 3);
        }
    }
}
