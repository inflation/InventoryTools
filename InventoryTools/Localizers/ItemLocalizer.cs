using System.Collections.Generic;
using CriticalCommonLib.Enums;
using CriticalCommonLib.Models;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace InventoryTools.Localizers;

public class ItemLocalizer
{
    private readonly ExcelSheet<Addon> _addonSheet;
    private readonly UiText _text;
    private Dictionary<uint, string> _cabinetNames;

    public ItemLocalizer(ExcelSheet<Addon> addonSheet, UiText text)
    {
        _addonSheet = addonSheet;
        _text = text;
        _cabinetNames = new();
    }

    public string CabinetName(InventoryItem inventoryItem)
    {
        if (inventoryItem.SortedContainer != InventoryType.Armoire)
        {
            return "";
        }

        var cabinetCategory = inventoryItem.Item.CabinetCategory;
        if (cabinetCategory == null)
        {
            return _text.Get("Unknown Cabinet");
        }

        if (_cabinetNames.TryGetValue(cabinetCategory.Base.Category.RowId, out string? cabinetName))
        {
            return cabinetName;
        }

        cabinetName = _addonSheet.GetRowOrDefault(cabinetCategory.Base.Category.RowId)?.Text.ExtractText() ??
                      _text.Get("Addon Text Not Found");

        _cabinetNames[cabinetCategory.Base.Category.RowId] = cabinetName;

        return cabinetName;
    }

    public string ItemDescription(InventoryItem inventoryItem)
    {
        if (inventoryItem.IsEmpty)
        {
            return _text.Get("Empty");
        }

        var _item = inventoryItem.Item.NameString.ToString();
        if (inventoryItem.IsHQ)
        {
            _item += _text.Get(" (HQ)");
        }
        else if (inventoryItem.IsCollectible)
        {
            _item += _text.Get(" (Collectible)");
        }
        else
        {
            _item += _text.Get(" (NQ)");
        }

        if (inventoryItem.SortedCategory == InventoryCategory.Currency)
        {
            _item += " - " + SortedContainerName(inventoryItem);
        }
        else
        {
            _item += " - " + SortedContainerName(inventoryItem) + " - " + (inventoryItem.SortedSlotIndex + 1);
        }


        return _item;
    }

    public string FormattedBagLocation(InventoryItem inventoryItem)
    {
        if (inventoryItem.SortedContainer is InventoryType.GlamourChest or InventoryType.Currency or InventoryType.RetainerGil or InventoryType.FreeCompanyGil or InventoryType.Crystal or InventoryType.RetainerCrystal)
        {
            return SortedContainerName(inventoryItem);
        }
        return SortedContainerName(inventoryItem) + " - " + (inventoryItem.SortedSlotIndex + 1);
    }

    public string SortedContainerName(InventoryItem inventoryItem)
    {
        return inventoryItem.SortedContainer == InventoryType.Armoire
            ? _text.Format("Armoire - {0}", CabinetName(inventoryItem))
            : _text.Get(ContainerName(inventoryItem.SortedContainer));
    }

    private static string ContainerName(InventoryType type)
    {
        if(type is InventoryType.Bag0 or InventoryType.RetainerBag0)
        {
            return "Bag 1";
        }
        if(type is InventoryType.Bag1 or InventoryType.RetainerBag1)
        {
            return "Bag 2";
        }
        if(type is InventoryType.Bag2 or InventoryType.RetainerBag2)
        {
            return "Bag 3";
        }
        if(type is InventoryType.Bag3 or InventoryType.RetainerBag3)
        {
            return "Bag 4";
        }
        if(type is InventoryType.RetainerBag4)
        {
            return "Bag 5";
        }
        if(type is InventoryType.SaddleBag0)
        {
            return "Saddlebag Left";
        }
        if(type is InventoryType.SaddleBag1)
        {
            return "Saddlebag Right";
        }
        if(type is InventoryType.PremiumSaddleBag0)
        {
            return "Premium Saddlebag Left";
        }
        if(type is InventoryType.PremiumSaddleBag1)
        {
            return "Premium Saddlebag Right";
        }
        if(type is InventoryType.ArmoryBody)
        {
            return "Armory - Body";
        }
        if(type is InventoryType.ArmoryEar)
        {
            return "Armory - Ear";
        }
        if(type is InventoryType.ArmoryFeet)
        {
            return "Armory - Feet";
        }
        if(type is InventoryType.ArmoryHand)
        {
            return "Armory - Hand";
        }
        if(type is InventoryType.ArmoryHead)
        {
            return "Armory - Head";
        }
        if(type is InventoryType.ArmoryLegs)
        {
            return "Armory - Legs";
        }
        if(type is InventoryType.ArmoryMain)
        {
            return "Armory - Main";
        }
        if(type is InventoryType.ArmoryNeck)
        {
            return "Armory - Neck";
        }
        if(type is InventoryType.ArmoryOff)
        {
            return "Armory - Offhand";
        }
        if(type is InventoryType.ArmoryRing)
        {
            return "Armory - Ring";
        }
        if(type is InventoryType.ArmoryWaist)
        {
            return "Armory - Waist";
        }
        if(type is InventoryType.ArmoryWrist)
        {
            return "Armory - Wrist";
        }
        if(type is InventoryType.ArmorySoulCrystal)
        {
            return "Armory - Soul Crystal";
        }
        if(type is InventoryType.GearSet0)
        {
            return "Equipped Gear";
        }
        if(type is InventoryType.RetainerEquippedGear)
        {
            return "Equipped Gear";
        }
        if(type is InventoryType.FreeCompanyBag0)
        {
            return "Free Company Chest - 1";
        }
        if(type is InventoryType.FreeCompanyBag1)
        {
            return "Free Company Chest - 2";
        }
        if(type is InventoryType.FreeCompanyBag2)
        {
            return "Free Company Chest - 3";
        }
        if(type is InventoryType.FreeCompanyBag3)
        {
            return "Free Company Chest - 4";
        }
        if(type is InventoryType.FreeCompanyBag4)
        {
            return "Free Company Chest - 5";
        }
        if(type is InventoryType.FreeCompanyBag5)
        {
            return "Free Company Chest - 6";
        }
        if(type is InventoryType.FreeCompanyBag6)
        {
            return "Free Company Chest - 7";
        }
        if(type is InventoryType.FreeCompanyBag7)
        {
            return "Free Company Chest - 8";
        }
        if(type is InventoryType.FreeCompanyBag8)
        {
            return "Free Company Chest - 9";
        }
        if(type is InventoryType.FreeCompanyBag9)
        {
            return "Free Company Chest - 10";
        }
        if(type is InventoryType.FreeCompanyBag10)
        {
            return "Free Company Chest - 11";
        }
        if(type is InventoryType.RetainerMarket)
        {
            return "Market";
        }
        if(type is InventoryType.GlamourChest)
        {
            return "Glamour Chest";
        }
        if(type is InventoryType.Currency)
        {
            return "Currency";
        }
        if(type is InventoryType.FreeCompanyGil)
        {
            return "Free Company - Gil";
        }
        if(type is InventoryType.RetainerGil)
        {
            return "Currency";
        }
        if(type is InventoryType.FreeCompanyCrystal)
        {
            return "Free Company - Crystals";
        }
        if(type is InventoryType.FreeCompanyCurrency)
        {
            return "Free Company - Currency";
        }
        if(type is InventoryType.Crystal or InventoryType.RetainerCrystal)
        {
            return "Crystals";
        }
        if(type is InventoryType.HousingExteriorAppearance)
        {
            return "Housing Exterior Appearance";
        }
        if(type is InventoryType.HousingInteriorAppearance)
        {
            return "Housing Interior Appearance";
        }
        if(type is InventoryType.HousingExteriorStoreroom or InventoryType.HousingExteriorStoreroom2)
        {
            return "Housing Exterior Storeroom";
        }
        if(type is InventoryType.HousingInteriorStoreroom1 or InventoryType.HousingInteriorStoreroom2 or InventoryType.HousingInteriorStoreroom3 or InventoryType.HousingInteriorStoreroom4 or InventoryType.HousingInteriorStoreroom5 or InventoryType.HousingInteriorStoreroom6 or InventoryType.HousingInteriorStoreroom7 or InventoryType.HousingInteriorStoreroom8 or InventoryType.HousingInteriorStoreroom9 or InventoryType.HousingInteriorStoreroom10 or InventoryType.HousingInteriorStoreroom11)
        {
            return "Housing Interior Storeroom";
        }
        if(type is InventoryType.HousingInteriorPlacedItems1 or InventoryType.HousingInteriorPlacedItems2 or InventoryType.HousingInteriorPlacedItems3 or InventoryType.HousingInteriorPlacedItems4 or InventoryType.HousingInteriorPlacedItems5 or InventoryType.HousingInteriorPlacedItems6 or InventoryType.HousingInteriorPlacedItems7 or InventoryType.HousingInteriorPlacedItems8 or InventoryType.HousingInteriorPlacedItems9 or InventoryType.HousingInteriorPlacedItems10 or InventoryType.HousingInteriorPlacedItems11 or InventoryType.HousingInteriorPlacedItems12)
        {
            return "Housing Interior Placed Items";
        }
        if(type is InventoryType.HousingExteriorPlacedItems or InventoryType.HousingExteriorPlacedItems2)
        {
            return "Housing Exterior Placed Items";
        }

        return type.ToString();
    }
}
