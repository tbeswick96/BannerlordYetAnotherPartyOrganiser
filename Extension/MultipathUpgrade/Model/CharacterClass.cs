using YAPO.MultipathUpgrade.Enum;

namespace YAPO.MultipathUpgrade.Model
{
    public class CharacterClass
    {
        public CharacterClass(CharacterClassType classType, EquipmentProperties equipmentProperties)
        {
            ClassType = classType;
            EquipmentProperties = equipmentProperties;
        }

        public CharacterClassType ClassType { get; }
        public EquipmentProperties EquipmentProperties { get; }
    }
}
