using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;
using YAPO.Services;

namespace YAPO {
    public class YapoSavableTypeDefiner : SaveableTypeDefiner {
        public YapoSavableTypeDefiner() : base(13337000) { }

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

    [SaveableEnum(13337150)]
    public enum SortMode {
        NONE,
        ALPHABETICAL,
        TYPE,
        GROUP,
        TIER,
        CULTURE,
        COUNT
    }

    [SaveableEnum(13337200)]
    public enum SortDirection {
        ASCENDING,
        DESCENDING
    }

    [SaveableEnum(13337250)]
    public enum SortSide {
        OTHER,
        PARTY
    }
}
