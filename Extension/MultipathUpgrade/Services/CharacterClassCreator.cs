using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using YAPO.MultipathUpgrade.Enum;
using YAPO.MultipathUpgrade.Model;

namespace YAPO.MultipathUpgrade.Services
{
    public static class CharacterClassCreator
    {
        public static CharacterClass CreateCharacterClass(CharacterObject character) => new CharacterClass(DetermineCharacterClassType(character), SetEquipmentProperties(character));

        private static CharacterClassType DetermineCharacterClassType(CharacterObject character)
        {
            CharacterClassType characterByDefaultFormationGroup = GetCharacterClassTypeByDefaultFormationGroup(character);
            if (characterByDefaultFormationGroup != CharacterClassType.UNKNOWN)
            {
                return characterByDefaultFormationGroup;
            }

            if (character.IsInfantry)
            {
                return CharacterClassType.INFANTRY;
            }

            if (character.IsArcher)
            {
                return character.IsMounted ? CharacterClassType.HORSE_ARCHER : CharacterClassType.RANGED;
            }

            return character.IsMounted ? CharacterClassType.CAVALRY : CharacterClassType.UNKNOWN;
        }

        private static CharacterClassType GetCharacterClassTypeByDefaultFormationGroup(BasicCharacterObject character)
        {
            return character.DefaultFormationGroup switch
            {
                0 => CharacterClassType.INFANTRY,
                1 => CharacterClassType.RANGED,
                2 => CharacterClassType.CAVALRY,
                3 => CharacterClassType.HORSE_ARCHER,
                _ => CharacterClassType.UNKNOWN
            };
        }

        private static EquipmentProperties SetEquipmentProperties(CharacterObject character)
        {
            EquipmentProperties properties = EquipmentProperties.NONE;
            if (HasItemType(character, ItemObject.ItemTypeEnum.Crossbow))
            {
                properties |= EquipmentProperties.HAS_CROSS_BOW;
            }
            if (HasItemType(character, ItemObject.ItemTypeEnum.Bow))
            {
                properties |= EquipmentProperties.HAS_BOW;
            }
            if (HasItemType(character, ItemObject.ItemTypeEnum.Shield))
            {
                properties |= EquipmentProperties.HAS_SHIELD;
            }

            return properties;
        }

        private static bool HasItemType(BasicCharacterObject character, ItemObject.ItemTypeEnum itemType)
        {
            for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
            {
                ItemObject item = character.Equipment[equipmentIndex].Item;
                if (item != null && item.Type == itemType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
