using System;
using TaleWorlds.Library;
using YAPO.Global;

namespace YAPO.ViewModels
{
    public class TypeSortOptionVm : ViewModel
    {
        private readonly Action<TypeSortOptionVm, int> _onChangeOrderOfTypeOption;

        public TypeSortOptionVm(TypeSortOption value,
                                Action<TypeSortOptionVm, int> onChangeOrderOfTypeOption)
        {
            Value = value;
            Label = value.AsString();
            _onChangeOrderOfTypeOption = onChangeOrderOfTypeOption;
        }

        public TypeSortOption Value { get; set; }

        [DataSourceProperty]
        public string Label { get; set; }
    }
}
