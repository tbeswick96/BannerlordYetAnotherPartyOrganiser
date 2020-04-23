using TaleWorlds.Library;
using YAPO.Global;

namespace YAPO.ViewModels
{
    public class TypeSortOptionVm : ViewModel
    {
        public TypeSortOptionVm(TypeSortOption value)
        {
            Value = value;
            Label = value.AsString();
        }

        public TypeSortOption Value { get; set; }

        [DataSourceProperty]
        public string Label { get; set; }
    }
}
