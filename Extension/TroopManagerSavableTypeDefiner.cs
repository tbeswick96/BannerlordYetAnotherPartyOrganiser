using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;
using TroopManager.Services;

namespace TroopManager {
    public class TroopManagerSavableTypeDefiner : SaveableTypeDefiner {
        public TroopManagerSavableTypeDefiner() : base(13337000) { }

        protected override void DefineClassTypes() {
            AddClassDefinition(typeof(TroopSorterService), 1);
        }

        protected override void DefineContainerDefinitions() {
            ConstructContainerDefinition(typeof(List<TroopSorterService>));
            ConstructContainerDefinition(typeof(Dictionary<string, TroopSorterService>));
            ConstructContainerDefinition(typeof(Dictionary<MBGUID, TroopSorterService>));
        }

        protected override void DefineGenericClassDefinitions() {
            ConstructGenericClassDefinition(typeof(MBObjectManager.ObjectTypeRecord<TroopSorterService>));
        }

        protected override void DefineEnumTypes() {
            AddEnumDefinition(typeof(SortMode), 2);
            AddEnumDefinition(typeof(SortDirection), 3);
            AddEnumDefinition(typeof(SortSide), 4);
        }
    }
}
