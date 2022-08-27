﻿using ExtensionsMethods;
using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Player.ActiveTasks
{
    public class GangDeliveryTask
    {
        private ITaskAssignable Player;
        private ITimeReportable Time;
        private IGangs Gangs;
        private PlayerTasks PlayerTasks;
        private IPlacesOfInterest PlacesOfInterest;
        private List<DeadDrop> ActiveDrops = new List<DeadDrop>();
        private ISettingsProvideable Settings;
        private IEntityProvideable World;
        private ICrimes Crimes;
        private IModItems ModItems;
        private IShopMenus ShopMenus;
        private Gang HiringGang;
        private DeadDrop DeadDrop;
        private PlayerTask CurrentTask;
        private int GameTimeToWaitBeforeComplications;
        private bool HasAddedComplications;
        private bool WillAddComplications;
        private int MoneyToRecieve;
        //private int MoneyToPickup;
        private GangDen HiringGangDen;
        private ModItem ItemToDeliver;
        private int NumberOfItemsToDeliver;

        private bool HasDen => HiringGangDen != null;

        public GangDeliveryTask(ITaskAssignable player, ITimeReportable time, IGangs gangs, PlayerTasks playerTasks, IPlacesOfInterest placesOfInterest, List<DeadDrop> activeDrops, ISettingsProvideable settings, IEntityProvideable world, ICrimes crimes, IModItems modItems, IShopMenus shopMenus)
        {
            Player = player;
            Time = time;
            Gangs = gangs;
            PlayerTasks = playerTasks;
            PlacesOfInterest = placesOfInterest;
            ActiveDrops = activeDrops;
            Settings = settings;
            World = world;
            Crimes = crimes;
            ModItems = modItems;
            ShopMenus = shopMenus;
        }
        public void Setup()
        {

        }
        public void Dispose()
        {
            if (DeadDrop != null)
            {
                DeadDrop.Deactivate();
            }
        }
        public void Start(Gang ActiveGang)
        {
            HiringGang = ActiveGang;
            if (PlayerTasks.CanStartNewTask(HiringGang?.ContactName))
            {
                GetHiringDen();
                if (HasDen)
                {
                    GetRequiredPayment();
                    SendInitialInstructionsMessage();
                    AddTask();
                }
                else
                {
                    SendTaskAbortMessage();
                }
            }
        }
        private void GetHiringDen()
        {
            HiringGangDen = PlacesOfInterest.PossibleLocations.GangDens.FirstOrDefault(x => x.AssociatedGang?.ID == HiringGang.ID);
        }
        private void GetRequiredPayment()
        {
            int PaymentAmount = RandomItems.GetRandomNumberInt(HiringGang.DeliveryPaymentMin, HiringGang.DeliveryPaymentMax).Round(100);
            List<string> PossibleItems = new List<string>() { "Marijuana", "SPANK", "Toilet Cleaner", "Cocaine", "Crack", "Heroin", "Methamphetamine" };
            string ChosenItem = "";
            ItemToDeliver = null;
            foreach(string possibleItem in PossibleItems.OrderBy(a => RandomItems.MyRand.Next()).ToList())
            {
                if(!HiringGangDen.Menu.Items.Any(x => x.Purchaseable && x.ModItemName == possibleItem))
                {
                    ChosenItem = possibleItem;
                }
            }
            if(ChosenItem != "")
            {
                ItemToDeliver = ModItems.Get(ChosenItem);
            }
            if(ItemToDeliver != null)
            {
                Tuple<int, int> Prices = ShopMenus.GetPrices(ItemToDeliver.Name);
                float MoreThaMax = (float)Prices.Item2 * 1.5f;
                NumberOfItemsToDeliver = (int)(PaymentAmount / MoreThaMax);
                MoneyToRecieve = PaymentAmount;
                EntryPoint.WriteToConsole($"GANG DELIVERY Item: {ItemToDeliver.Name} Number: {NumberOfItemsToDeliver} Lowest: {Prices.Item1}  Highest {Prices.Item2} Payment: {MoneyToRecieve}");
            }
            if (MoneyToRecieve <= 0)
            {
                MoneyToRecieve = 500;
            }
            MoneyToRecieve = MoneyToRecieve.Round(10);
        }
        private void AddTask()
        {
            PlayerTasks.AddTask(HiringGang.ContactName, MoneyToRecieve, 500, 0, -1000, 4, "Pickup for Gang");
            HiringGangDen.ExpectedItem = ItemToDeliver;
            HiringGangDen.ExpectedItemAmount = NumberOfItemsToDeliver;

            CurrentTask = PlayerTasks.GetTask(HiringGang.ContactName);
            CurrentTask.IsReadyForPayment = true;


            GameTimeToWaitBeforeComplications = RandomItems.GetRandomNumberInt(3000, 10000);
            HasAddedComplications = false;
            WillAddComplications = false;// RandomItems.RandomPercent(Settings.SettingsManager.TaskSettings.RivalGangHitComplicationsPercentage);
        }
        private void SendInitialInstructionsMessage()
        {
            List<string> Replies = new List<string>() {
                    $"I want you to find us {NumberOfItemsToDeliver} {ItemToDeliver.MeasurementName}(s) of {ItemToDeliver.Name}. We need it quick. Bring it to the {HiringGang.DenName} on {HiringGangDen.FullStreetAddress}. ${MoneyToRecieve}",
                    $"Go get {NumberOfItemsToDeliver} {ItemToDeliver.MeasurementName}(s) of {ItemToDeliver.Name} from somewhere, I don't wanna know. Don't take too long. Bring it to the {HiringGang.DenName} on {HiringGangDen.FullStreetAddress}. ${MoneyToRecieve} to you when you drop it off",
                    $"We need you to find {NumberOfItemsToDeliver} {ItemToDeliver.MeasurementName}(s) of {ItemToDeliver.Name}. We can't wait all week. Take it to the {HiringGang.DenName} on {HiringGangDen.FullStreetAddress}. You'll get ${MoneyToRecieve} when I get my item.",
                    };
            Player.CellPhone.AddPhoneResponse(HiringGang.ContactName, HiringGang.ContactIcon, Replies.PickRandom());
        }
        private void SendTaskAbortMessage()
        {
            List<string> Replies = new List<string>() {
                    "Nothing yet, I'll let you know",
                    "I've got nothing for you yet",
                    "Give me a few days",
                    "Not a lot to be done right now",
                    "We will let you know when you can do something for us",
                    "Check back later.",
                    };
            Player.CellPhone.AddPhoneResponse(HiringGang.ContactName, Replies.PickRandom());
        }
    }
}
