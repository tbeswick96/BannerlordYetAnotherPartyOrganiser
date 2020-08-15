using TaleWorlds.Library;
using YAPO.Global;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace YAPO {
    public class SortByModeOptionVm : ViewModel {
        private readonly PartyVmMixin _parentVm;

        public SortByModeOptionVm(PartyVmMixin parentVm, SortMode value) {
            _parentVm = parentVm;
            Value = value;
            Label = value.AsString();
        }

        public SortMode Value { get; set; }

        [DataSourceProperty]
        public string Label { get; set; }

        public void ExecuteSelectPartySortBy() {
            _parentVm.CurrentPartySortByMode = Value;
        }

        public void ExecuteSelectPartyThenBy() {
            _parentVm.CurrentPartyThenByMode = Value;
        }

        public void ExecuteSelectOtherSortBy() {
            _parentVm.CurrentOtherSortByMode = Value;
        }

        public void ExecuteSelectOtherThenBy() {
            _parentVm.CurrentOtherThenByMode = Value;
        }
    }
}
