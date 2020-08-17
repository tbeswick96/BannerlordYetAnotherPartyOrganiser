﻿namespace YAPO.MultipathUpgrade.Enum {
    public enum RangedPreference {
        NONE,
        BOWS,
        CROSSBOWS
    }

    public class RangedPreferenceObject {
        private readonly string _name;

        public RangedPreferenceObject(RangedPreference value, string name) {
            Value = value;
            _name = name;
        }

        public RangedPreference Value { get; }

        public override string ToString() => _name;
    }
}
