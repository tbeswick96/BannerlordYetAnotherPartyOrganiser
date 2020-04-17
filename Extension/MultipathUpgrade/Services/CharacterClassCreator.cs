using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using YAPO.MultipathUpgrade.Enum;
using YAPO.MultipathUpgrade.Model;

namespace YAPO.MultipathUpgrade.Services
{
    public static class CharacterClassCreator
    {
        public static CharacterClass CreateCharacterClass(CharacterObject character)
        {
            return new CharacterClass(DetermineCharacterClassType(character), SetEquipmentProperties(character));
        }

        private static CharacterClassType DetermineCharacterClassType(CharacterObject character)
        {
            var characterByDefaultFormationGroup = GetCharacterClassTypeByDefaultFormationGroup(character);
            if (characterByDefaultFormationGroup != CharacterClassType.UNKNOWN)
                return characterByDefaultFormationGroup;

            if (character.IsInfantry)
            {
                return CharacterClassType.INFANTRY;
            }

            if (character.IsArcher)
            {
                if (character.IsMounted)
                {
                    return CharacterClassType.HORSE_ARCHER;
                }
                return CharacterClassType.RANGED;
            }

            if (character.IsMounted)
            {
                return CharacterClassType.CAVALRY;
            }

            return CharacterClassType.UNKNOWN;
        }

        private static CharacterClassType GetCharacterClassTypeByDefaultFormationGroup(CharacterObject character)
        {
            switch (character.DefaultFormationGroup)
            {
                case 0:
                    return CharacterClassType.INFANTRY;
                case 1:
                    return CharacterClassType.RANGED;
                case 2:
                    return CharacterClassType.CAVALRY;
                case 3:
                    return CharacterClassType.HORSE_ARCHER;
            }

            return CharacterClassType.UNKNOWN;
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

        public static bool HasItemType(CharacterObject character, ItemObject.ItemTypeEnum itemType)
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
