﻿using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

public class StoreSellMenu : Menu
{
    private UIMenu sellMenu;
    private IModItems ModItems;
    private MenuPool MenuPool;
    private IActivityPerformable Player;
    private int ItemsSold;
    private Vehicle SellingVehicle;
    private Rage.Object SellingProp;
    private Ped SellingPed;
    private Camera StoreCam;
    private bool ShouldPreviewItem;
    //private TransactionOld Transaction;
    private VehicleExt ToSellVehicle;
    private IEntityProvideable World;
    private ISettingsProvideable Settings;
    private TransactableLocation Store;
    private ITimeControllable Time;
    private IWeapons Weapons;
    private ModItem CurrentModItem;
    private MenuItem CurrentMenuItem;
    private int CurrentTotalPrice;
    private int MoneySpent;
    private WeaponInformation CurrentWeapon;
    private WeaponVariation CurrentWeaponVariation = new WeaponVariation();

    public bool Visible => sellMenu.Visible;
    public bool SoldItem => ItemsSold > 0;
    public StoreSellMenu(MenuPool menuPool, UIMenu parentMenu, TransactableLocation store, IModItems modItems, IActivityPerformable player, IEntityProvideable world, ISettingsProvideable settings, IWeapons weapons, ITimeControllable time)
    {
        ModItems = modItems;
        Player = player;
        World = world;
        Settings = settings;
        MenuPool = menuPool;
        Store = store;
        Time = time;
        Weapons = weapons;
        StoreCam = Camera.RenderingCamera;
        sellMenu = menuPool.AddSubMenu(parentMenu, "Sell");
        if (Store.HasBannerImage)
        {
            sellMenu.SetBannerType(Store.BannerImage);
        }
        else if (Store.RemoveBanner)
        {
            sellMenu.RemoveBanner();
        }
        sellMenu.OnIndexChange += OnIndexChange;
        sellMenu.OnItemSelect += OnItemSelect;
        sellMenu.OnListChange += OnListChange;
    }
    public void Setup()
    {
        if (Settings.SettingsManager.PlayerOtherSettings.GenerateStoreItemPreviews)
        {
            PreloadModels();
        }
        Store.ClearPreviews();
        CreateSellMenu();
    }
    public void Dispose()
    {
        Hide();
        Game.RawFrameRender -= (s, e) => MenuPool.DrawBanners(e.Graphics);
    }
    public void Update()
    {
        if (MenuPool.IsAnyMenuOpen())//.Visible)
        {
            if (SellingProp.Exists())
            {
                SellingProp.SetRotationYaw(SellingProp.Rotation.Yaw + 1f);
            }
            if (SellingVehicle.Exists())
            {
                SellingVehicle.SetRotationYaw(SellingVehicle.Rotation.Yaw + 1f);
            }
        }
        else
        {
            ClearPreviews();
        }
    }
    public override void Hide()
    {
        ClearPreviews();
        sellMenu.Visible = false;
        Player.ButtonPrompts.Clear();
    }
    public override void Show()
    {
        //CreateSellMenu();
        sellMenu.Visible = true;
    }
    public override void Toggle()
    {
        if (!sellMenu.Visible)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    private void CreateSellMenu()
    {
        sellMenu.Clear();
        bool shouldCreateCategories = false;
        if (Store.Menu.Items.Where(x => x.Sellable).Count() >= 7)
        {
            shouldCreateCategories = true;
        }
        if (shouldCreateCategories)
        {
            CreateCategories();
        }
        foreach (MenuItem cii in Store.Menu.Items)
        {
            if (cii != null && cii.Sellable)
            {
                ModItem myItem = ModItems.Get(cii.ModItemName);
                if (myItem != null)
                {
                    if (myItem.ModelItem?.Type == ePhysicalItemType.Vehicle)
                    {
                        AddVehicleEntry(cii, myItem);
                    }
                    else if (myItem.ModelItem?.Type == ePhysicalItemType.Weapon)
                    {
                        AddWeaponEntry(cii, myItem);
                    }
                    else
                    {
                        AddPropEntry(cii, myItem);
                    }
                }
            }
        }
        OnIndexChange(sellMenu, sellMenu.CurrentSelection);
    }
    private void AddVehicleEntry(MenuItem cii, ModItem myItem)
    {
        string formattedPurchasePrice = cii.SalesPrice.ToString("C0");
        string MakeName = NativeHelper.VehicleMakeName(Game.GetHashKey(myItem.ModelItem.ModelName));
        string ClassName = NativeHelper.VehicleClassName(Game.GetHashKey(myItem.ModelItem.ModelName));
        string ModelName = NativeHelper.VehicleModelName(Game.GetHashKey(myItem.ModelItem.ModelName));
        string description;
        if (myItem.Description.Length >= 200)
        {
            description = myItem.Description.Substring(0, 200) + "...";//menu cant show more than 225?, need some for below
        }
        else
        {
            description = myItem.Description;
        }
        description += "~n~~s~";
        if (MakeName != "")
        {
            description += $"~n~Manufacturer: ~b~{MakeName}~s~";
        }
        if (ModelName != "")
        {
            description += $"~n~Model: ~g~{ModelName}~s~";
        }
        if (ClassName != "")
        {
            description += $"~n~Class: ~p~{ClassName}~s~";
        }
        if (myItem.RequiresDLC)
        {
            description += $"~n~~b~DLC Vehicle";
        }
        UIMenu VehicleMenu = null;
        bool FoundCategoryMenu = false;
        foreach (UIMenu uimen in MenuPool.ToList())
        {
            if (uimen.SubtitleText == ClassName)
            {
                FoundCategoryMenu = true;
                VehicleMenu = MenuPool.AddSubMenu(uimen, cii.ModItemName);
                uimen.MenuItems[uimen.MenuItems.Count() - 1].Description = description;
                uimen.MenuItems[uimen.MenuItems.Count() - 1].RightLabel = formattedPurchasePrice;
                EntryPoint.WriteToConsole($"Added Vehicle {myItem.Name} To SubMenu {uimen.SubtitleText}", 5);
                break;
            }
        }
        if (!FoundCategoryMenu && VehicleMenu == null)
        {
            VehicleMenu = MenuPool.AddSubMenu(sellMenu, cii.ModItemName);
            sellMenu.MenuItems[sellMenu.MenuItems.Count() - 1].Description = description;
            sellMenu.MenuItems[sellMenu.MenuItems.Count() - 1].RightLabel = formattedPurchasePrice;
            EntryPoint.WriteToConsole($"Added Vehicle {myItem.Name} To Main Buy Menu", 5);
        }
        if (Store.HasBannerImage)
        {
            VehicleMenu.SetBannerType(Store.BannerImage);
        }
        else if (Store.RemoveBanner)
        {
            VehicleMenu.RemoveBanner();
        }
        description = myItem.Description;
        if (description == "")
        {
            description = $"List Price {formattedPurchasePrice}";
        }
        UIMenuItem Purchase = new UIMenuItem($"Sell", "Select to sell this vehicle") { RightLabel = formattedPurchasePrice };
        VehicleMenu.AddItem(Purchase);
        VehicleMenu.OnItemSelect += OnVehicleItemSelect;
    }


    private void AddPropEntry(MenuItem cii, ModItem myItem)
    {
        string formattedPurchasePrice = cii.SalesPrice.ToString("C0");
        string description = myItem.Description;
        if (description == "")
        {
            description = $"{cii.ModItemName} {formattedPurchasePrice}";
        }
        description += "~n~~s~";
        description += $"~n~Type: ~p~{myItem.FormattedItemType}~s~";
        description += $"~n~~b~{myItem.AmountPerPackage}~s~ Item(s) per Package";
        if (myItem.AmountPerPackage > 1)
        {
            description += $"~n~~b~{((float)cii.SalesPrice / (float)myItem.AmountPerPackage).ToString("C2")} ~s~per Item";
        }
        if (myItem.ChangesHealth)
        {
            description += $"~n~{myItem.HealthChangeDescription}";
        }
        //if (myItem.ConsumeOnPurchase && (myItem.Type == eConsumableType.Eat || myItem.Type == eConsumableType.Drink))
        //{
        //    description += $"~n~~r~Dine-In Only~s~";
        //}

        bool enabled = Player.Inventory.HasItem(cii.ModItemName);
        InventoryItem coolItem = Player.Inventory.Items.Where(x => x.ModItem.Name == cii.ModItemName).FirstOrDefault();
        int MaxSell = 1;
        if (coolItem != null)
        {
            MaxSell = coolItem.Amount;
        }

        sellMenu.AddItem(new UIMenuNumericScrollerItem<int>(cii.ModItemName, description, 1, MaxSell, 1) { Enabled = enabled, Formatter = v => $"{(v == 1 && myItem.MeasurementName == "Item" ? "" : v.ToString() + " ")}{(myItem.MeasurementName != "Item" || v > 1 ? myItem.MeasurementName : "")}{(v > 1 ? "(s)" : "")}{(myItem.MeasurementName != "Item" || v > 1 ? " - " : "")}${(v * cii.PurchasePrice)}", Value = 1 });
        // { RightLabel = formattedPurchasePrice });
    }
    private void OnVehicleItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem.Text == "Sell" && CurrentModItem != null)
        {
            MenuItem menuItem = Store.Menu.Items.Where(x => x.ModItemName == CurrentModItem.Name).FirstOrDefault();
            if (menuItem != null)
            {
                EntryPoint.WriteToConsole($"Vehicle Purchase {menuItem.ModItemName} Player.Money {Player.Money} menuItem.PurchasePrice {menuItem.PurchasePrice}", 5);
                if (Player.Money < menuItem.PurchasePrice)
                {
                    Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", Store.Name, "Insufficient Funds", "We are sorry, we are unable to complete this transation, as you do not have the required funds");
                    return;
                }
                //if (!PurchaseVehicle(CurrentModItem))
                //{
                //    return;
                //}
                Player.GiveMoney(-1 * menuItem.PurchasePrice);
                MoneySpent += menuItem.PurchasePrice;
            }
            sender.Visible = false;
            Dispose();
        }
        if (selectedItem.Text == "Set Plate" && CurrentModItem != null)
        {
            //PlateString = NativeHelper.GetKeyboardInput("");
            //if (SellingVehicle.Exists() && PlateString != "")
            //{
            //    SellingVehicle.LicensePlate = PlateString.Substring(0,8);
            //}
        }
    }

    private void CreateCategories()
    {
        List<WeaponCategory> WeaponCategories = new List<WeaponCategory>();
        List<string> VehicleClasses = new List<string>();
        int TotalWeapons = Store.Menu.Items.Where(x => x.Sellable && ModItems.Get(x.ModItemName)?.ModelItem?.Type == ePhysicalItemType.Weapon).Count();
        int TotalVehicles = Store.Menu.Items.Where(x => x.Sellable && ModItems.Get(x.ModItemName)?.ModelItem?.Type == ePhysicalItemType.Vehicle).Count();
        foreach (MenuItem cii in Store.Menu.Items.Where(x => x.Sellable))
        {
            ModItem myItem = ModItems.Get(cii.ModItemName);
            if (myItem != null)
            {
                if (myItem.ModelItem?.Type == ePhysicalItemType.Weapon)
                {
                    if (TotalWeapons >= 7)
                    {
                        WeaponInformation myWeapon = Weapons.GetWeapon(myItem.ModelItem.ModelName);
                        if (myWeapon != null)
                        {
                            if (!WeaponCategories.Contains(myWeapon.Category))
                            {
                                WeaponCategories.Add(myWeapon.Category);
                                UIMenu WeaponMenu = MenuPool.AddSubMenu(sellMenu, myWeapon.Category.ToString());
                                if (Store.HasBannerImage)
                                {
                                    WeaponMenu.SetBannerType(Store.BannerImage);
                                }
                                else if (Store.RemoveBanner)
                                {
                                    WeaponMenu.RemoveBanner();
                                }
                                WeaponMenu.OnIndexChange += OnIndexChange;
                                WeaponMenu.OnItemSelect += OnItemSelect;
                                WeaponMenu.OnMenuOpen += OnMenuOpen;
                                WeaponMenu.OnMenuClose += OnMenuClose;
                            }
                        }
                    }
                }
                else if (myItem.ModelItem?.Type == ePhysicalItemType.Vehicle)
                {
                    if (TotalVehicles >= 7)
                    {
                        string ClassName = NativeHelper.VehicleClassName(Game.GetHashKey(myItem.ModelItem.ModelName));
                        if (ClassName != "")
                        {
                            if (!VehicleClasses.Contains(ClassName))
                            {
                                VehicleClasses.Add(ClassName);
                                UIMenu VehicleMenu = MenuPool.AddSubMenu(sellMenu, ClassName);
                                if (Store.HasBannerImage)
                                {
                                    VehicleMenu.SetBannerType(Store.BannerImage);
                                }
                                else if (Store.RemoveBanner)
                                {
                                    VehicleMenu.RemoveBanner();
                                }
                                VehicleMenu.OnIndexChange += OnIndexChange;
                                VehicleMenu.OnItemSelect += OnItemSelect;
                                VehicleMenu.OnMenuClose += OnMenuClose;
                                VehicleMenu.OnMenuOpen += OnMenuOpen;
                            }
                        }
                    }
                }
            }
        }
    }
    private void OnMenuClose(UIMenu sender)
    {
        EntryPoint.WriteToConsole($"OnMenuClose {sender.SubtitleText} {sender.CurrentSelection}", 5);
        ClearPreviews();
    }
    private void OnMenuOpen(UIMenu sender)
    {
        EntryPoint.WriteToConsole($"OnMenuOpen {sender.SubtitleText} {sender.CurrentSelection}", 5);
        if (sender.CurrentSelection != -1)
        {
            CreatePreview(sender.MenuItems[sender.CurrentSelection]);
        }

        foreach (UIMenuItem uimen in sender.MenuItems)
        {
            MenuItem menuItem = Store.Menu.Items.Where(x => x.ModItemName == uimen.Text).FirstOrDefault();
            if (menuItem != null)
            {

                EntryPoint.WriteToConsole($"    SELL ON MENU OPEN Sub Level: {menuItem.ModItemName} {uimen.Text}", 5);


                ModItem currentItem = ModItems.Get(menuItem.ModItemName);
                if (currentItem != null && currentItem.ModelItem?.Type == ePhysicalItemType.Weapon)
                {
                    WeaponInformation myGun = Weapons.GetWeapon(currentItem.ModelItem.ModelName);
                    if (myGun != null)
                    {
                        if (NativeFunction.Natives.HAS_PED_GOT_WEAPON<bool>(Player.Character, myGun.Hash, false))
                        {
                            uimen.Enabled = true;
                            EntryPoint.WriteToConsole($"    SELL ON MENU OPEN Sub Level: {myGun.ModelName} ENABLED", 5);
                        }
                        else
                        {
                            uimen.Enabled = false;
                            EntryPoint.WriteToConsole($"    SELL ON MENU OPEN Sub Level: {myGun.ModelName} NOT ENABLED", 5);
                        }
                        
                    }
                }
            }
        }
    }

    private void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        ModItem ToAdd = ModItems.Items.Where(x => x.Name == selectedItem.Text).FirstOrDefault();
        MenuItem menuItem = Store.Menu.Items.Where(x => x.ModItemName == selectedItem.Text).FirstOrDefault();
        bool ExitAfterPurchase = false;
        if (ToAdd != null && menuItem != null)
        {
            CurrentModItem = ToAdd;
            CurrentMenuItem = menuItem;
            if (ToAdd.ModelItem?.Type == ePhysicalItemType.Vehicle)//SubMenu
            {
                CurrentWeapon = null;
                CurrentWeaponVariation = new WeaponVariation();

                CreatePreview(selectedItem);
                EntryPoint.WriteToConsole($"Purchase Menu: {CurrentModItem.Name} OnItemSelect", 5);
            }
            else if (ToAdd.ModelItem?.Type == ePhysicalItemType.Weapon)//SubMenu
            {
                CurrentWeapon = Weapons.GetWeapon(CurrentModItem.ModelItem.ModelName);
                CurrentWeaponVariation = new WeaponVariation();
                CreatePreview(selectedItem);
                EntryPoint.WriteToConsole($"Purchase Menu: {CurrentModItem.Name} OnItemSelect", 5);
            }
            else
            {
                int TotalItems = 1;
                if (selectedItem.GetType() == typeof(UIMenuNumericScrollerItem<int>))
                {
                    UIMenuNumericScrollerItem<int> myItem = (UIMenuNumericScrollerItem<int>)selectedItem;
                    TotalItems = myItem.Value;
                }
                CurrentWeapon = null;
                CurrentWeaponVariation = new WeaponVariation();
                SellItem(CurrentModItem, CurrentMenuItem, TotalItems);
            }
        }
        else
        {
            CurrentModItem = null;
            CurrentMenuItem = null;
        }



        //GameFiber.Sleep(500);
        //while (Player.IsPerformingActivity)
        //{
        //    GameFiber.Sleep(500);
        //}
        //if (ExitAfterPurchase)
        //{
        //    Dispose();
        //}
        //else
        //{
        //    Show();
        //}
    }
    private void OnIndexChange(UIMenu sender, int newIndex)
    {
        if (newIndex != -1)
        {
            CreatePreview(sender.MenuItems[newIndex]);
        }
    }
    private void OnListChange(UIMenu sender, UIMenuListItem listItem, int newIndex)
    {

    }
    private bool SellItem(ModItem modItem, MenuItem menuItem, int TotalItems)
    {
        int TotalPrice = menuItem.PurchasePrice * TotalItems;
        CurrentTotalPrice = TotalPrice;
        if (Player.Money >= TotalPrice)
        {
            bool subtractCash = true;
            ItemsSold++;
            if (modItem.ConsumeOnPurchase)
            {
                Player.ConsumeItem(modItem);
            }
            else
            {
                Player.Inventory.Add(modItem, TotalItems * modItem.AmountPerPackage);
            }
            // }
            if (subtractCash)
            {
                Player.GiveMoney(-1 * TotalPrice);
                MoneySpent += TotalPrice;
            }
            while (Player.IsPerformingActivity)
            {
                GameFiber.Sleep(500);
            }
            return true;
        }
        return false;
    }





    private void CreatePreview(UIMenuItem myItem)
    {
        ClearPreviews();
        // GameFiber.Yield();
        if (myItem != null && Settings.SettingsManager.PlayerOtherSettings.GenerateStoreItemPreviews)
        {
            EntryPoint.WriteToConsole($"SIMPLE TRANSACTION OnIndexChange Text: {myItem.Text}", 5);
            ModItem itemToShow = ModItems.Items.Where(x => x.Name == myItem.Text).FirstOrDefault();
            if (itemToShow != null)
            {
                if (itemToShow.PackageItem?.Type == ePhysicalItemType.Prop || itemToShow.ModelItem?.Type == ePhysicalItemType.Prop)
                {
                    PreviewProp(itemToShow);
                }
                else if (itemToShow.ModelItem?.Type == ePhysicalItemType.Vehicle)
                {
                    PreviewVehicle(itemToShow);
                }
                else if (itemToShow.ModelItem?.Type == ePhysicalItemType.Ped)
                {
                    PreviewPed(itemToShow);
                }
                else if (itemToShow.ModelItem?.Type == ePhysicalItemType.Weapon)
                {
                    PreviewWeapon(itemToShow);
                }
            }
        }
    }



    private void PreviewPed(ModItem itemToShow)
    {
        //GameFiber.Yield();
    }
    private void PreviewProp(ModItem itemToShow)
    {
        try
        {
            string ModelToSpawn = "";
            bool useClose = true;
            if (itemToShow.PackageItem != null)
            {
                ModelToSpawn = itemToShow.PackageItem.ModelName;
                useClose = !itemToShow.PackageItem.IsLarge;
            }
            if (ModelToSpawn == "")
            {
                ModelToSpawn = itemToShow.ModelItem.ModelName;
                useClose = !itemToShow.ModelItem.IsLarge;
            }

            Vector3 Position = Vector3.Zero;
            if (StoreCam.Exists())
            {
                if (useClose)
                {
                    Position = StoreCam.Position + StoreCam.Direction;
                }
                else
                {
                    Position = StoreCam.Position + (StoreCam.Direction.ToNormalized() * 3f);
                }
            }
            else
            {
                Vector3 GPCamPos = NativeFunction.Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
                Vector3 GPCamDir = NativeHelper.GetGameplayCameraDirection();

                if (useClose)
                {
                    Position = GPCamPos + GPCamDir / 2;
                }
                else
                {
                    Position = GPCamPos + GPCamDir.ToNormalized() * 3f;
                }
            }

            if (ModelToSpawn != "" && NativeFunction.Natives.IS_MODEL_VALID<bool>(Game.GetHashKey(ModelToSpawn)))
            {
                SellingProp = new Rage.Object(ModelToSpawn, Position);

                //if (useClose)
                //{
                //    SellingProp = new Rage.Object(ModelToSpawn, StoreCam.Position + StoreCam.Direction);
                //}
                //else
                //{
                //    SellingProp = new Rage.Object(ModelToSpawn, StoreCam.Position + (StoreCam.Direction.ToNormalized() * 3f));
                //}
                //GameFiber.Yield();
                if (SellingProp.Exists())
                {
                    SellingProp.SetRotationYaw(SellingProp.Rotation.Yaw + 45f);
                    if (SellingProp != null && SellingProp.Exists())
                    {
                        NativeFunction.Natives.SET_ENTITY_HAS_GRAVITY(SellingProp, false);
                    }
                }
                EntryPoint.WriteToConsole("SIMPLE TRANSACTION: PREVIEW ITEM RAN", 5);
            }
            else
            {
                if (SellingProp.Exists())
                {
                    SellingProp.Delete();
                }
            }
        }
        catch (Exception ex)
        {
            Game.DisplayNotification($"Error Displaying Model {ex.Message} {ex.StackTrace}");
        }
    }
    private void PreviewVehicle(ModItem itemToShow)
    {
        if (itemToShow != null && itemToShow.ModelItem != null)
        {
            SellingVehicle = new Vehicle(itemToShow.ModelItem.ModelName, Store.ItemPreviewPosition, Store.ItemPreviewHeading);
        }
        //GameFiber.Yield();
        if (SellingVehicle.Exists())
        {
            SellingVehicle.Wash();
            NativeFunction.Natives.SET_VEHICLE_COLOURS(SellingVehicle, 0, 0);
            NativeFunction.Natives.SET_VEHICLE_ON_GROUND_PROPERLY<bool>(SellingVehicle, 5.0f);
        }
    }
    private void PreviewWeapon(ModItem itemToShow)
    {
        try
        {
            EntryPoint.WriteToConsole($"SELL MENU PreviewWeapon RAN {itemToShow ?.Name}");
            if (itemToShow != null && itemToShow.ModelItem != null && itemToShow.ModelItem.ModelName != "")
            {
                Vector3 Position = Vector3.Zero;
                if (StoreCam.Exists())
                {
                    Position = StoreCam.Position + StoreCam.Direction / 2f;
                }
                else
                {
                    Vector3 GPCamPos = NativeFunction.Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
                    Vector3 GPCamDir = NativeHelper.GetGameplayCameraDirection();
                    Position = GPCamPos + GPCamDir / 2f;
                }
                SellingProp = NativeFunction.Natives.CREATE_WEAPON_OBJECT<Rage.Object>(itemToShow.ModelItem.ModelHash, 60, Position.X, Position.Y, Position.Z, true, 1.0f, 0, 0, 1);
                if (SellingProp.Exists())
                {
                    EntryPoint.WriteToConsole($"SELL MENU PreviewWeapon CREATED ITEM {itemToShow?.Name}");
                    float length = SellingProp.Model.Dimensions.X;
                    float width = SellingProp.Model.Dimensions.Y;
                    if (StoreCam.Exists())
                    {
                        Position = StoreCam.Position + (StoreCam.Direction.ToNormalized() * 0.5f) + (StoreCam.Direction.ToNormalized() * length / 2f);//
                    }
                    else
                    {
                        Vector3 GPCamPos = NativeFunction.Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
                        Vector3 GPCamDir = NativeHelper.GetGameplayCameraDirection();
                        Position = GPCamPos + (GPCamDir.ToNormalized() * 0.5f) + (GPCamDir.ToNormalized() * length / 2f);
                    }
                    SellingProp.Position = Position;
                    SellingProp.SetRotationYaw(SellingProp.Rotation.Yaw + 45f);
                    if (SellingProp != null && SellingProp.Exists())
                    {
                        NativeFunction.Natives.SET_ENTITY_HAS_GRAVITY(SellingProp, false);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Game.DisplayNotification($"Error Displaying Model {ex.Message} {ex.StackTrace}");
        }
        //GameFiber.Yield();
    }
    public void ClearPreviews()
    {
        if (SellingProp.Exists())
        {
            SellingProp.Delete();
        }
        if (SellingVehicle.Exists())
        {
            SellingVehicle.Delete();
        }
        if (SellingPed.Exists())
        {
            SellingPed.Delete();
        }
    }
    private void PreloadModels()
    {
        foreach (MenuItem menuItem in Store.Menu.Items)//preload all item models so it doesnt bog the menu down
        {
            try
            {
                if (menuItem.Sellable)
                {
                    ModItem myItem = ModItems.Items.Where(x => x.Name == menuItem.ModItemName).FirstOrDefault();
                    if (myItem != null)
                    {
                        if (myItem.ModelItem != null && myItem.ModelItem.Type == ePhysicalItemType.Weapon && myItem.ModelItem.ModelName != "")
                        {
                            NativeFunction.Natives.REQUEST_WEAPON_ASSET(myItem.ModelItem.ModelHash, 31, 0);
                        }
                        else if (myItem.ModelItem != null && myItem.ModelItem.Type == ePhysicalItemType.Vehicle && myItem.ModelItem.ModelName != "")
                        {
                            Vehicle MyVehicle = new Vehicle(myItem.ModelItem.ModelName, Vector3.Zero, 0f);
                            if (MyVehicle.Exists())
                            {
                                MyVehicle.Delete();
                            }
                        }
                        else if (myItem.PackageItem != null && myItem.PackageItem.Type == ePhysicalItemType.Prop && myItem.PackageItem.ModelName != "")
                        {
                            new Model(myItem.PackageItem.ModelName).LoadAndWait();
                        }
                        else if (myItem.ModelItem != null && myItem.ModelItem.Type == ePhysicalItemType.Prop && myItem.ModelItem.ModelName != "")
                        {
                            new Model(myItem.ModelItem.ModelName).LoadAndWait();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Game.DisplayNotification($"Error Preloading Model {ex.Message} {ex.StackTrace}");
            }
        }
    }

    private void AddWeaponEntry(MenuItem cii, ModItem myItem)
    {
        EntryPoint.WriteToConsole($"Sell Menu Add Weapon Entry ItemName: {myItem.Name}", 5);
        string description;
        if (myItem.Description.Length >= 200)
        {
            description = myItem.Description.Substring(0, 200) + "...";//menu cant show more than 225?, need some for below
        }
        else
        {
            description = myItem.Description;
        }
        description += "~n~~s~";
        if (myItem.RequiresDLC)
        {
            description += $"~n~~b~DLC Weapon";
        }
        string formattedPurchasePrice = cii.SalesPrice.ToString("C0");
        WeaponInformation myWeapon = Weapons.GetWeapon(myItem.ModelItem.ModelName);
        bool hasPedGotWeapon = false;
        if (NativeFunction.Natives.HAS_PED_GOT_WEAPON<bool>(Player.Character, myWeapon.Hash, false))
        {
            hasPedGotWeapon = true;
        }
        else
        {
            hasPedGotWeapon = false;
        }


        UIMenu WeaponMenu = null;
        bool FoundCategoryMenu = false;
        if (myWeapon != null)
        {
            foreach (UIMenu uimen in MenuPool.ToList())
            {
                if (uimen.SubtitleText == myWeapon.Category.ToString() && uimen.ParentMenu == sellMenu)
                {
                    FoundCategoryMenu = true;
                    WeaponMenu = MenuPool.AddSubMenu(uimen, cii.ModItemName);
                    uimen.MenuItems[uimen.MenuItems.Count() - 1].Description = description;
                    uimen.MenuItems[uimen.MenuItems.Count() - 1].RightLabel = formattedPurchasePrice;
                    uimen.MenuItems[uimen.MenuItems.Count() - 1].Enabled = hasPedGotWeapon;
                    EntryPoint.WriteToConsole($"Added Weapon {myItem.Name} To SubMenu {uimen.SubtitleText}", 5);
                    break;
                }
            }
        }
        if (!FoundCategoryMenu && WeaponMenu == null)
        {
            WeaponMenu = MenuPool.AddSubMenu(sellMenu, cii.ModItemName);
            sellMenu.MenuItems[sellMenu.MenuItems.Count() - 1].Description = description;
            sellMenu.MenuItems[sellMenu.MenuItems.Count() - 1].RightLabel = formattedPurchasePrice;
            sellMenu.MenuItems[sellMenu.MenuItems.Count() - 1].Enabled = hasPedGotWeapon;
            EntryPoint.WriteToConsole($"Added Weapon {myItem.Name} To Main Buy Menu", 5);
        }
        //WeaponMenu.OnMenuOpen += OnWeaponMenuOpen;
        if (Store.HasBannerImage)
        {
            WeaponMenu.SetBannerType(Store.BannerImage);
        }
        else if (Store.RemoveBanner)
        {
            WeaponMenu.RemoveBanner();
        }
        UIMenuNumericScrollerItem<int> PurchaseAmmo = new UIMenuNumericScrollerItem<int>($"Sell Ammo", $"Select to sell ammo for this weapon.", cii.SubAmount, 500, cii.SubAmount) { Index = 0, Formatter = v => $"{v} - ${cii.SubPrice * v}" };
        UIMenuItem Purchase = new UIMenuItem($"Sell", "Select to sell this Weapon") { RightLabel = formattedPurchasePrice };
        if (hasPedGotWeapon)
        {
            Purchase.Enabled = true;
        }
        else
        {
            Purchase.Enabled = false;
        }
        if (myWeapon.Category != WeaponCategory.Melee && myWeapon.Category != WeaponCategory.Throwable)
        {
            WeaponMenu.AddItem(PurchaseAmmo);
        }
        WeaponMenu.AddItem(Purchase);
        WeaponMenu.OnItemSelect += OnWeaponItemSelect;
        WeaponMenu.OnMenuOpen += OnWeaponMenuOpen;
    }
    private void OnWeaponItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem.Text == "Sell" && CurrentModItem != null)
        {
            if (CurrentMenuItem != null)
            {
                int TotalPrice = CurrentMenuItem.SalesPrice;
                if (!SellWeapon())
                {
                    return;
                }
                Player.GiveMoney(TotalPrice);
                MoneySpent += TotalPrice;
                OnWeaponMenuOpen(sender);
            }
        }
        //else if (selectedItem.Text == "Purchase Ammo" && CurrentModItem != null)
        //{
        //    int TotalItems = 1;
        //    if (selectedItem.GetType() == typeof(UIMenuNumericScrollerItem<int>))
        //    {
        //        UIMenuNumericScrollerItem<int> myItem = (UIMenuNumericScrollerItem<int>)selectedItem;
        //        TotalItems = myItem.Value;
        //    }
        //    if (CurrentMenuItem != null)
        //    {
        //        int TotalPrice = CurrentMenuItem.SubPrice * TotalItems;
        //        EntryPoint.WriteToConsole($"Weapon Purchase {CurrentMenuItem.ModItemName} Player.Money {Player.Money} menuItem.PurchasePrice {1}", 5);
        //        if (Player.Money < TotalPrice)
        //        {
        //            Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", Store.Name, "Insufficient Funds", "We are sorry, we are unable to complete this transation, as you do not have the required funds");
        //            return;
        //        }
        //        if (!PurchaseAmmo(TotalItems))
        //        {
        //            return;
        //        }
        //        Player.GiveMoney(-1 * TotalPrice);
        //        MoneySpent += TotalPrice;
        //        OnWeaponMenuOpen(sender);
        //    }
        //}
        //else if (selectedItem.GetType() == typeof(UIMenuListScrollerItem<MenuItemExtra>))
        //{
        //    UIMenuListScrollerItem<MenuItemExtra> myItem = (UIMenuListScrollerItem<MenuItemExtra>)selectedItem;
        //    bool isComponentSlot = false;
        //    ComponentSlot selectedSlot;
        //    foreach (ComponentSlot cs in Enum.GetValues(typeof(ComponentSlot)).Cast<ComponentSlot>().ToList())
        //    {
        //        if (cs.ToString() == selectedItem.Text)
        //        {
        //            selectedSlot = cs;
        //            isComponentSlot = true;
        //            if (myItem.SelectedItem.ExtraName == "Default")
        //            {
        //                CurrentWeapon.SetSlotDefault(Player.Character, selectedSlot);
        //                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", Store.Name, "Set Default", $"Set the {selectedSlot} slot to default");
        //                OnWeaponMenuOpen(sender);
        //                return;
        //            }
        //            break;
        //        }
        //    }
        //    WeaponComponent myComponent = CurrentWeapon.PossibleComponents.Where(x => x.Name == myItem.SelectedItem.ExtraName).FirstOrDefault();
        //    if (myComponent != null && CurrentMenuItem != null)
        //    {
        //        EntryPoint.WriteToConsole($"Weapon Component Purchase {CurrentMenuItem.ModItemName} Player.Money {Player.Money} menuItem.PurchasePrice {CurrentMenuItem.PurchasePrice} myComponent {myComponent.Name}", 5);
        //        if (Player.Money < myItem.SelectedItem.PurchasePrice)
        //        {
        //            Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", Store.Name, "Insufficient Funds", "We are sorry, we are unable to complete this transation, as you do not have the required funds");
        //            return;
        //        }
        //        if (CurrentWeapon.HasComponent(Player.Character, myComponent))
        //        {
        //            Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", Store.Name, "Already Owned", "We are sorry, we are unable to complete this transation, as the item is already owned");
        //            return;
        //        }
        //        if (!PurchaseComponent(myComponent))
        //        {
        //            return;
        //        }
        //        Player.GiveMoney(-1 * myItem.SelectedItem.PurchasePrice);
        //        MoneySpent += myItem.SelectedItem.PurchasePrice;
        //        OnWeaponMenuOpen(sender);
        //    }
        //}
    }
    private void OnWeaponMenuOpen(UIMenu sender)
    {
        EntryPoint.WriteToConsole($"OnWeaponMenuOpen RAN!", 5);
        foreach (UIMenuItem uimen in sender.MenuItems)
        {
            if (uimen.GetType() == typeof(UIMenuListScrollerItem<MenuItemExtra>))
            {
                UIMenuListScrollerItem<MenuItemExtra> myItem = (UIMenuListScrollerItem<MenuItemExtra>)(object)uimen;
                foreach (MenuItemExtra stuff in myItem.Items)
                {
                    WeaponComponent myComponent = CurrentWeapon.PossibleComponents.Where(x => x.Name == stuff.ExtraName).FirstOrDefault();
                    if (myComponent != null)
                    {
                        if (CurrentWeapon.HasComponent(Player.Character, myComponent))
                        {
                            myItem.SelectedItem = stuff;
                            stuff.HasItem = true;
                            EntryPoint.WriteToConsole($"OnWeaponMenuOpen RAN! {myComponent.Name} HAS COMPONENT {stuff.HasItem} {myItem.OptionText}", 5);
                        }
                        else
                        {
                            //myItem.SelectedItem = stuff;
                            stuff.HasItem = false;
                            EntryPoint.WriteToConsole($"OnWeaponMenuOpen RAN! {myComponent.Name} DOES NOT HAVE COMPONENT  {stuff.HasItem} {myItem.OptionText}", 5);
                        }
                        // myItem.Formatter = v => v.HasItem ? $"{v.ExtraName} - Equipped" : v.PurchasePrice == 0 ? v.ExtraName : $"{v.ExtraName} - ${v.PurchasePrice}";
                    }
                }
                myItem.Reformat();
            }
            else if (uimen.Text == "Sell")
            {
                if (CurrentWeapon.HasWeapon(Player.Character) && CurrentWeapon.Category != WeaponCategory.Throwable)
                {
                    uimen.Enabled = true;
                }
                else
                {
                    uimen.Enabled = false;
                    // uimen.RightLabel = "Owned";
                }
            }
            else if (uimen.Text == "Sell Ammo")
            {
                if (CurrentWeapon.HasWeapon(Player.Character))
                {
                    uimen.Enabled = true;
                }
                else
                {
                    uimen.Enabled = false;
                }
            }
            EntryPoint.WriteToConsole($"Full Below Level: {uimen.Text}", 5);
        }
    }
    private bool SellWeapon()
    {
        if (CurrentWeapon != null)
        {
            if (NativeFunction.Natives.HAS_PED_GOT_WEAPON<bool>(Player.Character, CurrentWeapon.Hash, false))
            {
                NativeFunction.Natives.REMOVE_WEAPON_FROM_PED(Player.Character, CurrentWeapon.Hash);
                Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", Store.Name, "~g~Sale", $"Thank you for your sale of ~r~{CurrentMenuItem.ModItemName}~s~");
                Player.SetUnarmed();
                return true;
            }
        }
        Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", Store.Name, "~r~Purchase Failed", "We are sorry, we are unable to complete this transation");
        return false;
    }
}