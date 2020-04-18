using System;

namespace YAPO.MultipathUpgrade.Enum
{
    [Flags]
    public enum EquipmentProperties
    {
        NONE = 0,
        HAS_SHIELD = 1,
        HAS_CROSS_BOW = 2,
        HAS_BOW = 4
    }
}
