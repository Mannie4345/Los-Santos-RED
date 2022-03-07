﻿using LosSantosRED.lsr;
using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlacesOfInterest : IPlacesOfInterest
{
    private readonly string ConfigFileName = "Plugins\\LosSantosRED\\Locations.xml";
    //private List<GameLocation> LocationsList;
    private IShopMenus ShopMenus;
    private IGangs Gangs;
    public PossibleLocations PossibleLocations { get; private set; }
    public PlacesOfInterest(IShopMenus shopMenus, IGangs gangs)
    {
        ShopMenus = shopMenus;
        Gangs = gangs;
        PossibleLocations = new PossibleLocations();
    }

    public void ReadConfig()
    {
        if (File.Exists(ConfigFileName))
        {
            PossibleLocations = Serialization.DeserializeParam<PossibleLocations>(ConfigFileName);
        }
        else
        {
            DefaultConfig();
            Serialization.SerializeParam(PossibleLocations, ConfigFileName);
        }
    }
    //public List<DeadDrop> DeadDrops { get; private set; } = new List<DeadDrop>();
    //public List<ScrapYard> ScrapYards { get; private set; } = new List<ScrapYard>();
    public List<InteractableLocation> GetAllInteractableLocations()
    {
        List<InteractableLocation> AllLocations = new List<InteractableLocation>();
        AllLocations.AddRange(PossibleLocations.DeadDrops);
        AllLocations.AddRange(PossibleLocations.ScrapYards);
        AllLocations.AddRange(PossibleLocations.GangDens);
        AllLocations.AddRange(PossibleLocations.GunStores);
        AllLocations.AddRange(PossibleLocations.Hotels);
        AllLocations.AddRange(PossibleLocations.Residences);
        AllLocations.AddRange(PossibleLocations.CityHalls);

        return AllLocations;
    }
    public List<GameLocation> GetAllPlaces()
    {
        return PossibleLocations.LocationsList;
    }
    public GameLocation GetClosestLocation(Vector3 Position, LocationType Type)
    {
        return PossibleLocations.LocationsList.Where(x => x.Type == Type).OrderBy(s => Position.DistanceTo2D(s.EntrancePosition)).FirstOrDefault();
    }
    public List<GameLocation> GetLocations(LocationType Type)
    {
        return PossibleLocations.LocationsList.Where(x => x.Type == Type).ToList();
    }  
    private void DefaultConfig()
    {
        List<MenuItem> ToolMenu = ShopMenus.GetMenu("ToolMenu")?.Items;
        List<MenuItem> CheapHotelMenu = ShopMenus.GetMenu("CheapHotelMenu")?.Items;
        List<MenuItem> ExpensiveHotelMenu = ShopMenus.GetMenu("ExpensiveHotelMenu")?.Items;
        List<MenuItem> HookerMenu = ShopMenus.GetMenu("HookerMenu")?.Items;
        List<MenuItem> ConvenienceStoreMenu = ShopMenus.GetMenu("ConvenienceStoreMenu")?.Items;
        List<MenuItem> TwentyFourSevenMenu = ShopMenus.GetMenu("TwentyFourSevenMenu")?.Items;
        List<MenuItem> FancyDeliMenu = ShopMenus.GetMenu("FancyDeliMenu")?.Items;
        List<MenuItem> FancyFishMenu = ShopMenus.GetMenu("FancyFishMenu")?.Items;
        List<MenuItem> FancyGenericMenu = ShopMenus.GetMenu("FancyGenericMenu")?.Items;
        List<MenuItem> GrainOfTruthMenu = ShopMenus.GetMenu("GrainOfTruthMenu")?.Items;
        List<MenuItem> FruitVineMenu = ShopMenus.GetMenu("FruitVineMenu")?.Items;
        List<MenuItem> GasStationMenu = ShopMenus.GetMenu("GasStationMenu")?.Items;
        List<MenuItem> RonMenu = ShopMenus.GetMenu("RonMenu")?.Items;
        List<MenuItem> XeroMenu = ShopMenus.GetMenu("XeroMenu")?.Items;
        List<MenuItem> LTDMenu = ShopMenus.GetMenu("LTDMenu")?.Items;
        List<MenuItem> GenericMenu = ShopMenus.GetMenu("GenericMenu")?.Items;
        List<MenuItem> ChihuahuaHotDogMenu = ShopMenus.GetMenu("ChihuahuaHotDogMenu")?.Items;
        List<MenuItem> BeefyBillsMenu = ShopMenus.GetMenu("BeefyBillsMenu")?.Items;
        List<MenuItem> PizzaMenu = ShopMenus.GetMenu("PizzaMenu")?.Items;
        List<MenuItem> DonutMenu = ShopMenus.GetMenu("DonutMenu")?.Items;
        List<MenuItem> FruitMenu = ShopMenus.GetMenu("FruitMenu")?.Items;
        List<MenuItem> UpNAtomMenu = ShopMenus.GetMenu("UpNAtomMenu")?.Items;
        List<MenuItem> TacoFarmerMenu = ShopMenus.GetMenu("TacoFarmerMenu")?.Items;
        List<MenuItem> HeadShopMenu = ShopMenus.GetMenu("HeadShopMenu")?.Items;
        List<MenuItem> LiquorStoreMenu = ShopMenus.GetMenu("LiquorStoreMenu")?.Items;
        List<MenuItem> BarMenu = ShopMenus.GetMenu("BarMenu")?.Items;
        List<MenuItem> CoffeeMenu = ShopMenus.GetMenu("CoffeeMenu")?.Items;
        List<MenuItem> SandwichMenu = ShopMenus.GetMenu("SandwichMenu")?.Items;
        List<MenuItem> BiteMenu = ShopMenus.GetMenu("BiteMenu")?.Items;
        List<MenuItem> TacoBombMenu = ShopMenus.GetMenu("TacoBombMenu")?.Items;
        List<MenuItem> BurgerShotMenu = ShopMenus.GetMenu("BurgerShotMenu")?.Items;
        List<MenuItem> WigwamMenu = ShopMenus.GetMenu("WigwamMenu")?.Items;
        List<MenuItem> ViceroyMenu = ShopMenus.GetMenu("ViceroyMenu")?.Items;
        List<MenuItem> CluckinBellMenu = ShopMenus.GetMenu("CluckinBellMenu")?.Items;
        List<MenuItem> AlDentesMenu = ShopMenus.GetMenu("AlDentesMenu")?.Items;
        List<MenuItem> BenefactorGallavanterMenu = ShopMenus.GetMenu("BenefactorGallavanterMenu")?.Items;
        List<MenuItem> NoodleMenu = ShopMenus.GetMenu("NoodleMenu")?.Items;
        List<MenuItem> WeedMenu = ShopMenus.GetMenu("WeedMenu")?.Items;
        List<MenuItem> WeedAndCigMenu = ShopMenus.GetMenu("WeedAndCigMenu")?.Items;

        List<MenuItem> VapidMenu = ShopMenus.GetMenu("VapidMenu")?.Items;
        List<MenuItem> PharmacyMenu = ShopMenus.GetMenu("PharmacyMenu")?.Items;
        List<MenuItem> SandersMenu = ShopMenus.GetMenu("SandersMenu")?.Items;
        List<MenuItem> HelmutMenu = ShopMenus.GetMenu("HelmutMenu")?.Items;
        List<MenuItem> BravadoMenu = ShopMenus.GetMenu("BravadoMenu")?.Items;
        List<MenuItem> LuxuryAutosMenu = ShopMenus.GetMenu("LuxuryAutosMenu")?.Items;
        List<MenuItem> PremiumDeluxeMenu = ShopMenus.GetMenu("PremiumDeluxeMenu")?.Items;
        List<MenuItem> LarrysRVMenu = ShopMenus.GetMenu("LarrysRVMenu")?.Items;
        List<MenuItem> BeanMachineMenu = ShopMenus.GetMenu("BeanMachineMenu")?.Items;
        List<MenuItem> AmmunationMenu = ShopMenus.GetMenu("AmmunationMenu")?.Items;
        //List<MenuItem> WeaponsMenu = ShopMenus.GetMenu("WeaponsMenu")?.Items;
      //  List<MenuItem> FamiliesGangDenMenu = ShopMenus.GetMenu("FamiliesGangDenMenu")?.Items;
       // List<MenuItem> GenericGangDenMenu = ShopMenus.GetMenu("GenericGangDenMenu")?.Items;

        List<MenuItem> LostDenMenu = ShopMenus.GetMenu("LostDenMenu")?.Items;
        List<MenuItem> FamiliesDenMenu = ShopMenus.GetMenu("FamiliesDenMenu")?.Items;
        List<MenuItem> VagosDenMenu = ShopMenus.GetMenu("VagosDenMenu")?.Items;
        List<MenuItem> BallasDenMenu = ShopMenus.GetMenu("BallasDenMenu")?.Items;
        List<MenuItem> VarriosDenMenu = ShopMenus.GetMenu("VarriosDenMenu")?.Items;
        List<MenuItem> MarabunteDenMenu = ShopMenus.GetMenu("MarabunteDenMenu")?.Items;
        List<MenuItem> TriadsDenMenu = ShopMenus.GetMenu("TriadsDenMenu")?.Items;
        List<MenuItem> KkangpaeDenMenu = ShopMenus.GetMenu("KkangpaeDenMenu")?.Items;
        List<MenuItem> GambettiDenMenu = ShopMenus.GetMenu("GambettiDenMenu")?.Items;




        //List<MenuItem> GunShop1 = ShopMenus.GetMenu("GunShop1")?.Items;
        //List<MenuItem> GunShop2 = ShopMenus.GetMenu("GunShop2")?.Items;
        //List<MenuItem> GunShop3 = ShopMenus.GetMenu("GunShop3")?.Items;
        //List<MenuItem> GunShop4 = ShopMenus.GetMenu("GunShop4")?.Items;
        //List<MenuItem> GunShop5 = ShopMenus.GetMenu("GunShop5")?.Items;


        List<MenuItem> ScrapMenu = ShopMenus.GetMenu("ScrapMenu")?.Items;

        PossibleLocations.DeadDrops.AddRange(new List<DeadDrop>() {

            new DeadDrop(new Vector3(74.97916f,-608.9933f,43.22042f), 249.4708f, "Dead Drop", "the LS 24 newspaper stand near the IAA building" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(75.60421f,-607.5473f,43.22063f), 249.4708f, "Dead Drop", "the Daily Rag newspaper stand by the IAA building" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(75.88783f,-606.5583f,43.22063f), 249.4708f, "Dead Drop", "the Las Mietras newspaper stand by the IAA building" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(76.75777f,-605.3703f,43.22094f), 68.89698f, "Dead Drop", "the trash can by the IAA building" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(-174.7691f,-674.9272f,33.27862f), 249.5148f, "Dead Drop", "the phone booth by the Arcadius Center" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(-223.6883f,-703.9772f,32.59268f), 70.00474f, "Dead Drop", "the mailbox by Schlongberg & Sachs" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(-251.0725f,-739.3169f,31.99848f), 187.8322f, "Dead Drop", "the Daily Rag newspaper stand by Schlongberg & Sachs" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(-263.7065f,-850.4099f,30.48533f), 160.0125f, "Dead Drop", "the Daily Rag newspaper stand by Go Postal" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(-1460.144f,-627.849f,29.69636f), 209.8004f, "Dead Drop", "the dumpster by Swallow" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(-1438.361f,-722.1968f,22.61556f), 171.4681f, "Dead Drop", "the daily rag newspaper stand by Pescado Rojo" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(-1364.685f,-795.9746f,18.32434f), 140.7789f, "Dead Drop", "the trash can in front of the Hedera Hotel" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(-1183.647f,-1257.851f,5.911644f), 260.6368f, "Dead Drop", "the trash can by Taco Libre" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(-1205.34f,-1377.286f,3.174809f), 76.95641f, "Dead Drop", "the trash can by Steamboat Beers" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(803.5943f,-2052.02f,28.30254f), 275.8022f, "Dead Drop", "the trash can by the PiBwasser Plant" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(1047.76f,-2464.003f,27.51101f), 44.61864f, "Dead Drop", "the dumpster by the gun dealers" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(1382.768f,-2042.507f,51.00203f), 30.00222f, "Dead Drop", "the dumpster by Covington Engineering" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(1316.705f,-1657.766f,50.23988f), 309.4677f, "Dead Drop", "the dumpster behind Los Santos Tattoos" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(426.0467f,100.2095f,99.24073f), 337.0558f, "Dead Drop", "the mailbox by Stargaze" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new DeadDrop(new Vector3(436.3528f,88.48115f,98.49297f), 159.5108f, "Dead Drop", "the dumpster by Wandlust" ) { OpenTime = 0,CloseTime = 24, IsEnabled = false },
        });



        PossibleLocations.ScrapYards.AddRange(new List<ScrapYard>() {
            new ScrapYard(new Vector3(1520.797f, -2113.375f, 76.86716f), 270.4797f, "Wesley's Scrap Yard", "Don't Ask, Don't Tell!") { OpenTime = 0, CloseTime = 24 },
            new ScrapYard(new Vector3(909.7432f, 3554.745f, 33.81702f), 211.2794f, "Marina Drive Scrap", "Top value for your 'questionable' provenance ") { OpenTime = 0, CloseTime = 24 },
            new ScrapYard(new Vector3(-195.9066f, 6264.628f, 31.48937f), 41.33705f, "Red's Machine Supplies", "Parts Bought and Sold!") { OpenTime = 0, CloseTime = 24 },


        });




        PossibleLocations.GangDens.AddRange(new List<GangDen>()
        {

            new GangDen(new Vector3(514.9427f, 190.9465f, 104.745f), 356.6495f, "Gambetti Safehouse", "","GambettiDenMenu", "AMBIENT_GANG_GAMBETTI") { OpenTime = 0,CloseTime = 24, IsEnabled = false },

            new GangDen(new Vector3(1662.302f, 4776.384f, 42.00795f), 279.1427f, "Pavano Safehouse", "","PavanoDenMenu", "AMBIENT_GANG_PAVANO") { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new GangDen(new Vector3(-229.6159f, 6445.189f, 31.19745f), 139.3764f, "Lupisella Safehouse", "","LupisellaDenMenu", "AMBIENT_GANG_LUPISELLA") { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new GangDen(new Vector3(-1629.715f, 36.49737f, 62.93618f), 333.3146f, "Messina Safehouse", "","MessinaDenMenu", "AMBIENT_GANG_MESSINA") { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new GangDen(new Vector3(-3228.478f, 1092.326f, 10.76322f), 253.458f, "Ancelotti Safehouse", "","AncelottiDenMenu", "AMBIENT_GANG_ANCELOTTI") { OpenTime = 0,CloseTime = 24, IsEnabled = false },



            new GangDen(new Vector3(1389.966f, 1131.907f, 114.3344f), 91.72424f, "Madrazo Cartel Den", "","MadrazoDenMenu", "AMBIENT_GANG_MADRAZO") { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new GangDen(new Vector3(-615.221f, -1787.458f, 23.69615f), 210.6709f, "Armenian Hangout", "","ArmenianDenMenu", "AMBIENT_GANG_ARMENIAN") { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new GangDen(new Vector3(-1157.501f, -1451.861f, 4.468448f), 216.5082f, "Yardies Chill Spot", "","YardiesDenMenu", "AMBIENT_GANG_YARDIES") { OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new GangDen(new Vector3(275.2381f, -3015.519f, 5.945963f), 91.01478f, "Diablos Hangout", "","DiablosDenMenu", "AMBIENT_GANG_DIABLOS") { OpenTime = 0,CloseTime = 24, IsEnabled = false },






            new GangDen(new Vector3(-223.1647f, -1601.309f, 34.88379f), 266.3889f, "The Families Den", "The OGs","FamiliesDenMenu", "AMBIENT_GANG_FAMILY") { BannerImagePath = "families.png",OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new GangDen(new Vector3(86.11255f, -1959.272f, 21.12167f), 318.5057f, "Ballas Den", "","BallasDenMenu", "AMBIENT_GANG_BALLAS") { BannerImagePath = "ballas.png",OpenTime = 0,CloseTime = 24, IsEnabled = false  },
            new GangDen(new Vector3(967.6899f, -1867.115f, 31.44757f), 176.7243f, "Vagos Den", "","VagosDenMenu", "AMBIENT_GANG_MEXICAN") { BannerImagePath = "",OpenTime = 0,CloseTime = 24, IsEnabled = false  },
            new GangDen(new Vector3(1193.61f, -1656.411f, 43.02641f), 31.55427f, "Varrios Los Aztecas Den", "","VarriosDenMenu", "AMBIENT_GANG_SALVA") { BannerImagePath = "varrios.png",OpenTime = 0,CloseTime = 24, IsEnabled = false  },
            new GangDen(new Vector3(1299.267f, -1752.92f, 53.88011f), 110.3803f, "Marabute Grande Den", "","MarabunteDenMenu", "AMBIENT_GANG_MARABUNTE") { OpenTime = 0,CloseTime = 24, IsEnabled = false  },
            new GangDen(new Vector3(-1144.041f, 4908.383f, 220.9688f), 33.69744f, "Altruist Cult Den", "","GenericGangDenMenu", "AMBIENT_GANG_CULT") { BannerImagePath = "altruist.png",OpenTime = 0,CloseTime = 24, IsEnabled = false  },
            new GangDen(new Vector3(-766.3793f, -917.0612f, 21.29704f), 268.4079f, "Kkangpae Den", "","KkangpaeDenMenu", "AMBIENT_GANG_KKANGPAE") {  OpenTime = 0,CloseTime = 24, IsEnabled = false  },
            new GangDen(new Vector3(959.721f, 3618.905f, 32.67253f), 93.92658f, "Reckneck Den", "","GenericGangDenMenu", "AMBIENT_GANG_HILLBILLY") { OpenTime = 0,CloseTime = 24, IsEnabled = false  },
            new GangDen(new Vector3(981.8542f, -103.0203f, 74.84874f), 220.3094f,  "Lost M.C. Clubhouse", "","LostDenMenu", "AMBIENT_GANG_LOST") { BannerImagePath = "lostmc.png", OpenTime = 0,CloseTime = 24, IsEnabled = false },
            new GangDen(new Vector3(101.6865f, -819.3801f, 31.31512f), 341.2845f,  "Triad Den", "","TriadsDenMenu", "AMBIENT_GANG_WEICHENG") { BannerImagePath = "triad.png", OpenTime = 0,CloseTime = 24,IsEnabled = false },


            //new ScrapYard(new Vector3(1520.797f, -2113.375f, 76.86716f), 270.4797f, "Wesley's Scrap Yard", "Don't Ask, Don't Tell!") { OpenTime = 0, CloseTime = 24 },
            //new ScrapYard(new Vector3(909.7432f, 3554.745f, 33.81702f), 211.2794f, "Marina Drive Scrap", "Top value for your 'questionable' provenance ") { OpenTime = 0, CloseTime = 24 },
            //new ScrapYard(new Vector3(-195.9066f, 6264.628f, 31.48937f), 41.33705f, "Red's Machine Supplies", "Parts Bought and Sold!") { OpenTime = 0, CloseTime = 24 }, 


        }) ;


        PossibleLocations.GunStores.AddRange(new List<GunStore>()

        {
            new GunStore(new Vector3(1049.596f, -2428.15f, 30.30457f), 84.97017f, "Underground Guns #1", "General shop","GunShop1") { IsEnabled = true, IsIllegalShop = true },
            new GunStore(new Vector3(-232.552f, -1311.643f, 31.29598f), 3.180501f, "Underground Guns #2", "Specializes in ~o~Pistols~s~","GunShop5") { IsEnabled = false, IsIllegalShop = true, MoneyToUnlock = 5000 },       
            new GunStore(new Vector3(334.3036f, -1978.458f, 24.16728f), 49.9404f, "Underground Guns #3", "Specializes in ~o~Sub-Machine Guns~s~","GunShop3") { IsEnabled = false, IsIllegalShop = true, MoneyToUnlock = 10000 },
            new GunStore(new Vector3(-258.3577f, 6247.281f, 31.48922f), 314.4655f, "Underground Guns #4", "Specializes in ~o~Assault Rifles~s~","GunShop4") { IsEnabled = false, IsIllegalShop = true, MoneyToUnlock = 15000 },
            new GunStore(new Vector3(1673.425f, 4957.921f, 42.34893f), 227.3988f, "Underground Guns #5", "Specializes in ~o~Heavy Weapons~s~","GunShop2") { IsEnabled = false, IsIllegalShop = true, MoneyToUnlock = 25000 },

        });

        PossibleLocations.Hotels.AddRange(new List<Hotel>()

        {

            new Hotel(new Vector3(-1183.073f, -1556.673f, 5.036984f), 122.3785f, "Vespucci Hotel", "Vespucci Hotel","CheapHotelMenu") {OpenTime = 0, CloseTime = 24 },
            new Hotel(new Vector3(-1343.127f, -1091.096f, 6.936333f), 299.9456f, "Venetian", "Venetian","ExpensiveHotelMenu") {OpenTime = 0, CloseTime = 24 },
            new Hotel(new Vector3(-1309.048f, -931.2507f, 13.35856f), 23.25741f, "Crown Jewel Hotel", "Crown Jewel Hotel","CheapHotelMenu") {OpenTime = 0, CloseTime = 24 },
            new Hotel(new Vector3(-1660.706f, -533.756f, 36.02398f), 141.6077f, "Banner", "Banner","ExpensiveHotelMenu") {OpenTime = 0, CloseTime = 24,CameraPosition = new Vector3(-1660.326f, -566.6978f, 39.62436f), CameraRotation = new Rotator(-11.99999f, 1.091059E-07f, 4.528234f) },//, new Vector3(0f, 0f, 0f), //Camera Position LocationName: bannerhotel
            new Hotel(new Vector3(-1856.868f, -347.9391f, 49.83775f), 141.5183f, "Von Krastenburg", "Von Krastenburg","ExpensiveHotelMenu") {OpenTime = 0, CloseTime = 24 },//needs zoom out
            new Hotel(new Vector3(-1356.452f, -791.2153f, 20.24218f), 129.4868f, "Hedera", "Hedera","ExpensiveHotelMenu") {OpenTime = 0, CloseTime = 24 },//needs zoom out
            new Hotel(new Vector3(-2007.835f, -314.862f, 32.09708f), 46.05545f, "The Jetty", "The Jetty","ExpensiveHotelMenu") {OpenTime = 0, CloseTime = 24 },//needs zoome out
            new Hotel(new Vector3(-823.0718f, -1223.552f, 7.365416f), 54.09635f, "The Viceroy", "","ViceroyMenu"){BannerImagePath = "viceroy.png",OpenTime = 0, CloseTime = 24, CameraPosition = new Vector3(-847.939f, -1207.791f, 7.15155f), CameraDirection = new Vector3(0.9588153f, -0.1468293f, 0.2431342f), CameraRotation = new Rotator(14.0716f, 0f, -98.70642f) },//needs zoome out
            new Hotel(new Vector3(-287.0405f, -1060.003f, 27.20538f), 252.0524f, "Banner", "","ExpensiveHotelMenu") {CameraPosition = new Vector3(-233.506f, -1048.275f, 34.58431f), CameraDirection = new Vector3(-0.9516708f, -0.2260422f, -0.2079124f), CameraRotation = new Rotator(-12.00004f, 0f, 103.3614f) },
            new Hotel(new Vector3(68.509f, -958.8935f, 29.80383f), 161.9325f, "The Emissary", "","ExpensiveHotelMenu"){CameraPosition = new Vector3(81.53342f, -1010.819f, 63.66661f), CameraDirection = new Vector3(-0.1635272f, 0.9643815f, -0.2079113f), CameraRotation = new Rotator(-11.99998f, 2.182118E-07f, 9.623925f) },
            new Hotel(new Vector3(313.3858f, -225.0208f, 54.22117f), 160.1122f, "Pink Cage", "","CheapHotelMenu"),
            new Hotel(new Vector3(307.3867f, -727.7486f, 29.31678f), 254.8814f, "Alesandro Hotel", "","CheapHotelMenu"),
            new Hotel(new Vector3(-702.4747f, -2274.476f, 13.45538f), 225.7683f, "Opium Nights", "","ExpensiveHotelMenu") ,
            new Hotel(new Vector3(379.4438f, -1781.435f, 29.46008f), 47.01642f, "Motel & Beauty", "","CheapHotelMenu") ,
            new Hotel(new Vector3(570.0554f, -1745.989f, 29.22319f), 260.0757f, "Billings Gate Motel", "","CheapHotelMenu"),
            new Hotel(new Vector3(-104.5376f, 6315.921f, 31.57622f), 141.414f, "Dream View Motel", "Mostly Bug Free!","CheapHotelMenu"),
            new Hotel(new Vector3(317.7083f, 2623.256f, 44.46722f), 306.9629f, "Eastern Motel", "","CheapHotelMenu"),
            new Hotel(new Vector3(1142.035f, 2664.177f, 38.16088f), 86.68575f, "The Motor Motel", "","CheapHotelMenu"),
            new Hotel(new Vector3(-477.0448f, 217.5538f, 83.70456f), 355.1573f, "The Generic Hotel", "","ExpensiveHotelMenu"),
            new Hotel(new Vector3(-309.8708f, 221.5867f, 87.92822f), 6.029551f, "Pegasus Hotel", "","ExpensiveHotelMenu") { CameraPosition = new Vector3(-347.6738f, 229.8998f, 98.77297f), CameraDirection = new Vector3(0.9052147f, -0.3525268f, -0.2372999f), CameraRotation = new Rotator(-13.72723f, -5.712704E-06f, -111.2779f) },
            new Hotel(new Vector3(-60.74598f, 360.7194f, 113.0564f), 243.5531f, "Gentry Manor Hotel", "","ExpensiveHotelMenu"){ CameraPosition = new Vector3(40.25754f, 259.9668f, 126.4436f), CameraDirection = new Vector3(-0.6868073f, 0.7189223f, -0.1069881f), CameraRotation = new Rotator(-6.141719f, -9.445725E-06f, 43.69126f) },
            new Hotel(new Vector3(-1273.729f, 316.0054f, 65.51177f), 152.4087f, "The Richman Hotel", "","ExpensiveHotelMenu"),
            new Hotel(new Vector3(286.5596f, -936.6477f, 29.46787f), 138.6224f, "Elkridge Hotel", "","ExpensiveHotelMenu") {CameraPosition = new Vector3(257.891f, -952.2925f, 43.25403f), CameraDirection = new Vector3(0.8390263f, 0.4567426f, -0.2956704f), CameraRotation = new Rotator(-17.19774f, -1.072479E-05f, -61.43736f) },
            new Hotel(new Vector3(104.8123f, -932.9781f, 29.81516f), 248.7484f, "The Emissary", "","ExpensiveHotelMenu"),
            new Hotel(new Vector3(329.0126f, -69.0122f, 73.03772f), 158.678f, "Vinewood Gardens", "","ExpensiveHotelMenu"),
            new Hotel(new Vector3(63.68047f, -261.8232f, 52.35384f), 335.7221f, "Cheep Motel", "POOL!","CheapHotelMenu"),
            new Hotel(new Vector3(-875.8169f, -2110.466f, 9.918293f), 41.67873f, "Crastenburg", "","ExpensiveHotelMenu"),
            new Hotel(new Vector3(435.6202f, 214.7496f, 103.1663f), 340.5429f, "Hotel Von Crastenburg","","ExpensiveHotelMenu"),
        });


        PossibleLocations.Residences.AddRange(new List<Residence>()
        {
            //Apartments
            new Residence(new Vector3(-1150.072f, -1521.705f, 10.62806f), 225.8192f, "7611 Goma St", "") { OpenTime = 0,CloseTime = 24, InteriorID = 24578, PurchasePrice = 550000, RentalDays = 28, RentalFee = 2250 },
            //new Residence(new Vector3(-1221.032f, -1232.806f, 11.02771f), 12.79515f, "Del Pierro Apartments", "") {OpenTime = 0,CloseTime = 24, InteriorID = -1, TeleportEnterPosition = new Vector3(266.1081f, -1007.534f, -101.0086f), TeleportEnterHeading = 358.3953f},
            
            //House
            new Residence(new Vector3(983.7811f, 2718.655f, 39.50342f), 175.7726f, "345 Route 68", "") {OpenTime = 0,CloseTime = 24, PurchasePrice = 234000, RentalDays = 28, RentalFee = 1400  },
            new Residence(new Vector3(980.1745f, 2666.563f, 40.04342f), 1.699184f, "140 Route 68", "") {OpenTime = 0,CloseTime = 24, PurchasePrice = 225000, RentalDays = 28, RentalFee = 1250  },
            new Residence(new Vector3(-3030.045f, 568.7484f, 7.823583f), 280.3508f, "566 Ineseno Road", "") {OpenTime = 0,CloseTime = 24, PurchasePrice = 556000, RentalDays = 28, RentalFee = 2200 },
            new Residence(new Vector3(-3031.865f, 525.1087f, 7.424246f), 267.6694f, "800 Ineseno Road", "") {OpenTime = 0,CloseTime = 24, PurchasePrice = 467000, RentalDays = 28, RentalFee = 1850  },
            new Residence(new Vector3(-3039.567f, 492.8512f, 6.772703f), 226.448f, "805 Ineseno Road", "") {OpenTime = 0,CloseTime = 24, PurchasePrice = 450000, RentalDays = 28, RentalFee = 1750  },
            new Residence(new Vector3(195.0935f, 3031.064f, 43.89068f), 273.1078f, "125 Joshua Road", "") {OpenTime = 0,CloseTime = 24, PurchasePrice = 298000, RentalDays = 28, RentalFee = 1580  },


            new Residence(new Vector3(191.0053f, 3082.26f, 43.47285f), 277.6117f, "610N Joshua Road", "dump grande senora house 200K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 289, RentalDays = 28, RentalFee = 1675  },
            new Residence(new Vector3(241.7666f, 3107.666f, 42.48719f), 93.76467f, "620N Joshua Road", "Dumpy tgrand house 200K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 275000, RentalDays = 28, RentalFee = 1610  },
            new Residence(new Vector3(162.8214f, 3119.749f, 43.42594f), 192.0786f, "621N Joshua Road", "burned down shithole 50K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 57000, RentalDays = 28, RentalFee = 850  },
            new Residence(new Vector3(247.5913f, 3169.535f, 42.78756f), 90.61945f, "630N Joshua Road", "Kinda dump 200K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 267000, RentalDays = 28, RentalFee = 1580  },



            new Residence(new Vector3(-214.8853f, 6444.098f, 31.31351f), 315.979f, "1280N Procopio Drive", "Paleto Bay, nice almost beach house 450K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 460000, RentalDays = 28, RentalFee = 1700  },
            new Residence(new Vector3(-272.7155f, 6400.906f, 31.50496f), 215.1084f, "1275N Procopio Drive", "Palento Beach House 450K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 470000, RentalDays = 28, RentalFee = 1850  },
            new Residence(new Vector3(-247.7424f, 6370.079f, 31.84554f), 45.0573f, "1276N Procopio Drive", "paleto bay 450K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 476000, RentalDays = 28, RentalFee = 1790  },
            new Residence(new Vector3(-227.3544f, 6377.188f, 31.75924f), 47.93699f, "1278N Procopio Drive", "Paleto Bay 400K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 414000, RentalDays = 28, RentalFee = 1670  },
            new Residence(new Vector3(-245.4691f, 6413.878f, 31.4606f), 130.3203f, "1281N Procopio Drive", "Paleto 425K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 455000, RentalDays = 28, RentalFee = 1770  },
            new Residence(new Vector3(-213.7807f, 6395.94f, 33.08509f), 40.99633f, "1282N Procopio Drive", "Paleto 425K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 435000, RentalDays = 28, RentalFee = 1680  },
            new Residence(new Vector3(-189.0498f, 6409.355f, 32.29676f), 48.98f, "1285N Procopio Drive", "Paleto 400K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 445000, RentalDays = 28, RentalFee = 1650  },
            new Residence(new Vector3(-280.4388f, 6350.718f, 32.60079f), 24.08426f, "1271N Procopio Drive", "Paleto Crap 250K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 247000, RentalDays = 28, RentalFee = 1380  },
            new Residence(new Vector3(-359.5872f, 6334.424f, 29.84736f), 226.0376f, "1260N Procopio Drive", "Paleto Big 550K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 565000, RentalDays = 28, RentalFee = 2200  },
            new Residence(new Vector3(-332.7523f, 6301.959f, 33.08874f), 69.82259f, "1262N Procopio Drive", "Paleto big, dumpy 250K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 257000, RentalDays = 28, RentalFee = 1480  },
            new Residence(new Vector3(-407.2561f, 6314.223f, 28.94128f), 230.5133f, "1259N Procopio Drive", "Paleto Nice 400K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 414000, RentalDays = 28, RentalFee = 1870  },
            new Residence(new Vector3(-437.4564f, 6261.807f, 30.06895f), 228.8042f, "1252N Procopio Drive", "Paleto Big with Garage 500K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 520000, RentalDays = 28, RentalFee = 2100  },

            new Residence(new Vector3(1880.423f, 3921.08f, 33.21722f), 100.0703f, "785N Niland Ave", "Sandy Shores, Dumpy 200K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 224000, RentalDays = 28, RentalFee = 1280  },
            new Residence(new Vector3(-37.54279f, 170.3245f, 95.35922f), 303.8834f, "Elgin House Apartment 23E", "Upscale apartment 350K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 404000, RentalDays = 28, RentalFee = 1650  },
            new Residence(new Vector3(9.063264f, 52.93087f, 71.64354f), 344.3057f, "0605 Apartment 4F", "West Viewood Upscale Apartment 300K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 375000, RentalDays = 28, RentalFee = 1570  },
            new Residence(new Vector3(76.05615f, -86.96131f, 63.00647f), 249.8842f, "1144 Apartment 2B", "Upscale Apartment in Hawick 300K") {OpenTime = 0,CloseTime = 24, PurchasePrice = 380000, RentalDays = 28, RentalFee = 1660  },




        });


        PossibleLocations.CityHalls.AddRange(new List<CityHall>()
        {
            new CityHall(new Vector3(-609.1187f, -87.8642f, 42.93483f), 247.5173f, "Rockford Hills City Hall", "") { OpenTime = 9,CloseTime = 18, CameraPosition = new Vector3(-591.665f, -98.11681f, 51.31879f), CameraDirection = new Vector3(-0.9335647f, -0.06657825f, -0.3521709f), CameraRotation = new Rotator(-20.62015f, -7.411738E-06f, 94.07921f)},
            new CityHall(new Vector3(233.2825f, -411.1329f, 48.11194f), 338.9129f, "Los Santos City Hall", "") { OpenTime = 9,CloseTime = 18, CameraPosition = new Vector3(236.4192f, -348.1718f, 79.68157f), CameraDirection = new Vector3(0.0002627021f, -0.9024973f, -0.4306954f), CameraRotation = new Rotator(-25.5117f, 6.462261E-07f, -179.9833f)},


            new CityHall(new Vector3(329.4892f, -1580.714f, 32.79719f), 135.1704f, "Davis City Hall", "") { OpenTime = 9,CloseTime = 18, CameraPosition = new Vector3(273.4156f, -1601.353f, 42.17516f), CameraDirection = new Vector3(0.9521951f, 0.2629484f, -0.1555077f), CameraRotation = new Rotator(-8.946239f, -9.507168E-06f, -74.56252f)},
            new CityHall(new Vector3(-1286.212f, -566.4031f, 31.7124f), 314.9816f, "Del Perro City Hall", "") { OpenTime = 9,CloseTime = 18, CameraPosition = new Vector3(-1262.947f, -546.7318f, 43.99615f), CameraDirection = new Vector3(-0.7403942f, -0.5856928f, -0.3298186f), CameraRotation = new Rotator(-19.25776f, -1.085254E-05f, 128.3459f)},






        });

        PossibleLocations.LocationsList.AddRange(new List<GameLocation>
        {
            //Hospital
            new GameLocation(new Vector3(364.7124f, -583.1641f, 28.69318f), 280.637f, LocationType.Hospital, "Pill Box Hill Hospital","") {OpenTime = 0,CloseTime = 24, InteriorID = 78338 },
            new GameLocation(new Vector3(338.208f, -1396.154f, 32.50927f), 77.07102f, LocationType.Hospital, "Central Los Santos Hospital","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(1842.057f, 3668.679f, 33.67996f), 228.3818f, LocationType.Hospital, "Sandy Shores Hospital","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(-244.3214f, 6328.575f, 32.42618f), 219.7734f, LocationType.Hospital, "Paleto Bay Hospital","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(1151.386f, -1529.34f, 35.36609f), 337.4726f, LocationType.Hospital, "St. Fiacre", "") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(241.2085f, -1378.962f, 33.74176f), 140.41f, LocationType.Morgue, "Los Santos County Coroner Office", "") {OpenTime = 0,CloseTime = 24, InteriorID = 60418, TeleportEnterPosition = new Vector3(253.351f, -1364.622f, 39.53437f), TeleportEnterHeading = 327.1821f },

            //Grave
            new GameLocation(new Vector3(-1654.301f, -148.7047f, 59.91496f), 299.5774f, LocationType.Grave, "Grave 1","") {OpenTime = 0,CloseTime = 24 },

            //Fire
            new GameLocation(new Vector3(1185.842f, -1464.118f, 34.90073f), 356.2903f, LocationType.FireStation, "LSCFD Fire Station 7", "") {OpenTime = 0,CloseTime = 24, InteriorID = 81666 },
            new GameLocation(new Vector3(213.8019f, -1640.523f, 29.68287f), 319.3789f, LocationType.FireStation, "Davis Fire Station", "") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(-633.0533f, -122.0594f, 39.01375f), 79.69817f, LocationType.FireStation, "Rockford Hills Fire Station", "") {OpenTime = 0,CloseTime = 24 },




            //Police
            new GameLocation(new Vector3(358.9726f, -1582.881f, 29.29195f), 323.5287f, LocationType.Police, "Davis Police Station","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(1858.19f, 3679.873f, 33.75724f), 218.3256f, LocationType.Police, "Sandy Shores Police Station","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(-437.973f, 6021.403f, 31.49011f), 316.3756f, LocationType.Police, "Paleto Bay Police Station","") {OpenTime = 0,CloseTime = 24 ,InteriorID = 3842},
            new GameLocation(new Vector3(440.0835f, -982.3911f, 30.68966f), 47.88088f, LocationType.Police, "Mission Row Police Station","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(815.8774f, -1290.531f, 26.28391f), 74.91704f, LocationType.Police, "La Mesa Police Station","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(642.1356f, -3.134667f, 82.78872f), 215.299f, LocationType.Police, "Vinewood Police Station","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(-557.0687f, -134.7315f, 38.20231f), 214.5968f, LocationType.Police, "Rockford Hills Police Station","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(-1093.817f, -807.1993f, 19.28864f), 22.23846f, LocationType.Police, "Vespucci Police Station","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(-1633.314f, -1010.025f, 13.08503f), 351.7007f, LocationType.Police, "Del Perro Police Station","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(-1311.877f, -1528.808f, 4.410581f), 233.9121f, LocationType.Police, "Vespucci Beach Police Station","") {OpenTime = 0,CloseTime = 24 },
            new GameLocation(new Vector3(102.9145f, -743.9487f, 45.75473f), 79.8266f, LocationType.Police, "FIB Downtown","") {InteriorID = 58882, OpenTime = 0,CloseTime = 24 },
            
            //Other
            new GameLocation(new Vector3(-248.491f, -2010.509f, 34.574f), 0f,LocationType.Stadium,"Maze Bank Arena","") {OpenTime = 0,CloseTime = 24, InteriorID = 78338 },
                 
            //Beauty
            new GameLocation(new Vector3(187.7006f, -1812.874f, 28.94536f), 323.7488f, LocationType.BeautyShop, "Carson's Beauty Supply", ""),
            new GameLocation(new Vector3(442.8802f, -2044.086f, 23.73479f), 317.6541f, LocationType.BeautyShop, "Chantelle's Beauty Salon", ""),
            new GameLocation(new Vector3(-342.2452f, -1482.83f, 30.70707f), 269.4036f, LocationType.BeautyShop, "Discount Beauty Store", ""),
            new GameLocation(new Vector3(-3050.165f, 625.066f, 7.269026f), 290.7953f, LocationType.BeautyShop, "Belinda May's Beauty Salon", ""),
            new GameLocation(new Vector3(1705.34f, 3780.338f, 34.7582f), 214.8316f, LocationType.BeautyShop, "Aunt Tammy's Hair Salon", ""),

            //Liquor
            new GameLocation(new Vector3(-1226.09f, -896.166f, 12.4057f), 22.23846f,new Vector3(-1221.119f, -908.5667f, 12.32635f), 33.35855f, LocationType.LiquorStore, "Rob's Liquors","Thats My Name, Don't Rob Me!") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22, InteriorID = 50178 },
            new GameLocation(new Vector3(-2974.098f, 390.9085f, 15.03413f), 84.05217f,new Vector3(-2966.361f, 390.1463f, 15.04331f), 88.73301f, LocationType.LiquorStore, "Rob's Liquors", "Thats My Name, Don't Rob Me!"){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22, InteriorID = 19202 },
            new GameLocation(new Vector3(-1491.638f, -384.0242f, 40.08308f), 136.3043f,new Vector3(-1486.196f, -377.7115f, 40.16343f), 133.357f, LocationType.LiquorStore, "Rob's Liquors", "Thats My Name, Don't Rob Me!") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22, InteriorID = 98818 },
            new GameLocation(new Vector3(1141.463f, -980.9073f, 46.41084f), 275.826f,new Vector3(1133.534f, -983.1248f, 46.41584f), 276.1162f, LocationType.LiquorStore, "Rob's Liquors", "Thats My Name, Don't Rob Me!"){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22, InteriorID = 73986 },
            new GameLocation(new Vector3(1166.318f, 2702.173f, 38.17925f), 175.7192f,new Vector3(1165.581f, 2710.774f, 38.15771f), 176.1093f, LocationType.LiquorStore, "Scoops Liquor Barn", "Scoops Liquor Barn"){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22, InteriorID = 33026 },
            new GameLocation(new Vector3(-596.1056f, 277.69f, 82.16035f), 170.2155f, LocationType.LiquorStore, "Eclipse Liquor.Deli", "") { Menu = LiquorStoreMenu },
            new GameLocation(new Vector3(-239.7314f, 244.0006f, 92.03992f), 318.9458f, LocationType.LiquorStore, "Ellen's Liquor Lover", "") { Menu = LiquorStoreMenu },
            new GameLocation(new Vector3(807.3504f, -1073.531f, 28.92093f), 134.3175f, LocationType.LiquorStore, "Liquor Market", "") { Menu = LiquorStoreMenu },
            new GameLocation(new Vector3(234.4411f, -1946.841f, 22.95617f), 3.404058f, LocationType.LiquorStore, "Liquor Store", "") { Menu = LiquorStoreMenu },
            new GameLocation(new Vector3(1212.343f, -445.0079f, 66.96259f), 74.05521f, LocationType.LiquorStore, "Liquor", "") { Menu = LiquorStoreMenu },
            new GameLocation(new Vector3(-1208.469f, -1384.053f, 4.085135f), 68.08948f, LocationType.LiquorStore, "Steamboat Beers", "Steamboat Beers") { Menu = LiquorStoreMenu },
            new GameLocation(new Vector3(-1106.07f, -1287.686f, 5.421459f), 161.3398f, LocationType.LiquorStore, "Vespucci Liquor Market", "Vespucci Liquor Market") { Menu = LiquorStoreMenu },
            new GameLocation(new Vector3(-697.8242f, -1182.286f, 10.71113f), 132.7831f, LocationType.LiquorStore, "Liquor Market", "Liquor Market") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-882.7062f, -1155.351f, 5.162508f), 215.8305f, LocationType.LiquorStore, "Liquor Hole", "Liquor Hole") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22,BannerImage = "liquorhole.png" },
            new GameLocation(new Vector3(-601.9684f, 244.0188f, 82.3046f), 358.6468f, LocationType.LiquorStore, "Liquor Hole", "Liquor Hole") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22,BannerImage = "liquorhole.png" },
            new GameLocation(new Vector3(456.5478f, 130.5207f, 99.28537f), 162.9724f, LocationType.LiquorStore, "Vinewood Liquor", "Vinewood Liquor") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(1391.861f, 3606.275f, 34.98093f), 199.2899f,new Vector3(1391.861f, 3606.275f, 34.98093f), 199.2899f, LocationType.LiquorStore, "Liquor Ace", "Liquor Ace") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(1952.552f, 3840.833f, 32.17612f), 298.8575f, LocationType.LiquorStore, "Sandy Shores Liquor", "Sandy Shores Liquor") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(2455.443f, 4058.518f, 38.06472f), 250.6311f, LocationType.LiquorStore, "Liquor Market 24/7", "Liquor Market"){ Menu = LiquorStoreMenu, OpenTime = 0, CloseTime = 24 },
            new GameLocation(new Vector3(2481.348f, 4100.31f, 38.13171f), 249.6295f, LocationType.LiquorStore, "Liquor Store", "Liquor Store"){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(2566.804f, 4274.108f, 41.98908f), 239.0765f, LocationType.LiquorStore, "Grape Smuggler's Liquor", "Grape Liquor"){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(98.82836f, -1308.733f, 29.27369f), 121.1285f, LocationType.LiquorStore, "The Brewer's Drop", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(169.9896f, -1336.975f, 29.30038f), 284.8854f, LocationType.LiquorStore, "Liquor Beer & Wine", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-43.36616f, -1042.139f, 28.33997f), 76.74245f, LocationType.LiquorStore, "Downtown Liquor.Deli", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(382.3881f, -1076.69f, 29.42185f), 267.7594f, LocationType.LiquorStore, "Downtown Liquor.Deli", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-57.89392f, -91.45875f, 57.75766f), 118.0359f, LocationType.LiquorStore, "Liquor", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(456.2009f, -2059.208f, 23.9267f), 274.6541f, LocationType.LiquorStore, "South LS Liquor", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(129.7891f, -1643.382f, 29.29159f), 38.8853f, LocationType.LiquorStore, "Liquor", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(463.9041f, -1852.109f, 27.79801f), 3.177461f, LocationType.LiquorStore, "Liquor Store", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-156.2888f, 6327.238f, 31.58083f), 316.2542f, LocationType.LiquorStore, "Del Vecchio Liquor", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-406.0117f, 6062.374f, 31.50013f), 132.2045f, LocationType.LiquorStore, "Liquor", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(579.2075f, 2677.982f, 41.84144f), 10.21047f, LocationType.LiquorStore, "Jr. Market Liquors", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(910.836f, 3644.788f, 32.67847f), 180.167f, LocationType.LiquorStore, "Liquor Market", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },  
            
            //HeadShop
            new GameLocation(new Vector3(-1191.582f, -1197.779f, 7.617113f), 146.801f, LocationType.Headshop, "Pipe Dreams", "Pipe Dreams") {Menu = HeadShopMenu },
            new GameLocation(new Vector3(65.60603f, -137.4155f, 55.11251f), 214.0327f, LocationType.Headshop, "Pipe Dreams", "") {Menu = HeadShopMenu },
            new GameLocation(new Vector3(278.8327f, -1027.653f, 29.21136f), 184.1326f, LocationType.Headshop, "Pipe Down Cigars", "") {Menu = HeadShopMenu },
            new GameLocation(new Vector3(-1154.942f, -1373.176f, 5.061489f), 305.589f, LocationType.Headshop, "Amnesiac Smoke Shop", "Amnesiac Smoke Shop") {Menu = WeedAndCigMenu },
            new GameLocation(new Vector3(-269.2553f, 243.7069f, 90.40055f), 1.693904f, LocationType.Headshop, "Pipe Down", "") {Menu = HeadShopMenu },

            //Dispensary
            new GameLocation(new Vector3(-1161.365f, -1427.646f, 4.623186f), 31.50553f, LocationType.Dispensary, "Doctor Kush", "Doctor kush") {Menu = WeedMenu },
            new GameLocation(new Vector3(-502.4879f, 32.92564f, 44.71512f), 179.9803f, LocationType.Dispensary, "Serenity Wellness", "Serenity Wellness") {Menu = WeedMenu },
            new GameLocation(new Vector3(169.5722f, -222.869f, 54.23643f), 342.0811f, LocationType.Dispensary, "High Time", "") {Menu = WeedMenu },
            new GameLocation(new Vector3(-1381.142f, -941.0327f, 10.17387f), 126.4558f, LocationType.Headshop, "Seagrass Herbals", "Seagrass Herbals") {Menu = WeedMenu },
            new GameLocation(new Vector3(1175.02f, -437.1972f, 66.93162f), 259.358f, LocationType.Dispensary, "Mile High Organics", "") {Menu = WeedMenu },

            //Convenience
            new GameLocation(new Vector3(547f, 2678f, 42.1565f), 22.23846f,new Vector3(549.6005f, 2669.846f, 42.1565f), 96.91093f, LocationType.ConvenienceStore, "24/7 Route 68","As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png", InteriorID = 41474},
            new GameLocation(new Vector3(-3236.767f,1005.609f,12.33137f), 122.6316f,new Vector3(-3243.302f, 1000.005f, 12.83071f), 352.6259f, LocationType.ConvenienceStore, "24/7 Chumash","As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png", InteriorID = 16386 },
            new GameLocation(new Vector3(2560f, 385f, 108f), 22.23846f,new Vector3(2555.339f, 380.9034f, 108.6229f), 347.3629f, LocationType.ConvenienceStore, "24/7 Palomino Freeway","As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png", InteriorID = 62722 },
            new GameLocation(new Vector3(29.32254f, -1350.485f, 29.33319f), 170.9901f,new Vector3(24.39647f, -1345.484f, 29.49702f), 252.9084f, LocationType.ConvenienceStore, "24/7 Strawberry", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png", InteriorID = 33282 },
            new GameLocation(new Vector3(-3037.729f, 589.7671f, 7.814812f), 289.0175f,new Vector3(-3039.787f, 584.1979f, 7.908929f), 12.80189f, LocationType.ConvenienceStore, "24/7 Banham Canyon", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png", InteriorID = 97538 },
            new GameLocation(new Vector3(376.3202f, 322.694f, 103.4389f), 162.5363f,new Vector3(372.6485f, 327.0293f, 103.5664f), 257.6475f, LocationType.ConvenienceStore, "24/7 Downtown Vinewood", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png", InteriorID = 46850 },
            new GameLocation(new Vector3(2682.938f, 3282.287f, 55.24056f), 243.885f,new Vector3(2676.595f, 3280.101f, 55.24113f), 325.0921f, LocationType.ConvenienceStore, "24/7 Senora Freeway", "As fast as you") { Menu = TwentyFourSevenMenu,OpenTime = 0, CloseTime = 24 ,BannerImage = "247.png", InteriorID = 13826},
            new GameLocation(new Vector3(1730.507f, 6410.014f, 35.00065f), 153.9039f,new Vector3(1728.436f, 6416.584f, 35.03722f), 241.2023f, LocationType.ConvenienceStore, "24/7 Senora Freeway","As fast as you") { Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png", InteriorID = 36354 },
            new GameLocation(new Vector3(1965.801f, 3739.945f, 32.322f), 207.564f,new Vector3(1959.352f, 3741.18f, 32.34374f), 303.8849f, LocationType.ConvenienceStore, "24/7 Sandy Shores","As fast as you") { Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png", InteriorID = 55554 },
            new GameLocation(new Vector3(-53.5351f, -1757.196f, 29.43954f), 146.0623f,new Vector3(-45.89098f, -1757.345f, 29.42101f), 52.66933f, LocationType.ConvenienceStore, "LtD Davis", "A one-stop shop!"){ Menu = LTDMenu,OpenTime = 0, CloseTime = 24, InteriorID = 80642 },
            new GameLocation(new Vector3(-578.0112f, -1012.898f, 22.32503f), 359.4114f, LocationType.ConvenienceStore, "24/7", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(-696.9965f, -858.7673f, 23.69209f), 85.51252f, LocationType.ConvenienceStore, "24/7", "24/7"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(152.5101f, 237.4131f, 106.9718f), 165.2823f, LocationType.ConvenienceStore, "24/7", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(201.8985f, -26.30606f, 69.90953f), 249.8224f, LocationType.ConvenienceStore, "24/7", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(528.017f, -152.1372f, 57.20173f), 44.64286f, LocationType.ConvenienceStore, "24/7", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(-1264.064f, -1162.599f, 6.764161f), 161.218f, LocationType.ConvenienceStore, "Fruit Of The Vine", "Fruit Of The Vine"){ Menu = FruitVineMenu },
            new GameLocation(new Vector3(-1270.649f, -304.9037f, 37.06938f), 257.2106f, LocationType.ConvenienceStore, "Fruit Of The Vine", "Fruit Of The Vine") { Menu = FruitVineMenu },
            new GameLocation(new Vector3(164.9962f, 351.1263f, 109.6859f), 4.847032f, LocationType.ConvenienceStore, "Fruit Of The Vine", "Fruit Of The Vine") { Menu = FruitVineMenu },
            new GameLocation(new Vector3(-144.3732f, -65.01408f, 54.60635f), 159.0404f, LocationType.ConvenienceStore, "Fruit Of The Vine", "") { Menu = FruitVineMenu },
            new GameLocation(new Vector3(-1412.015f, -320.1292f, 44.37897f), 92.48502f, LocationType.ConvenienceStore, "The Grain Of Truth", "The Grain Of Truth") { Menu = GrainOfTruthMenu },
            new GameLocation(new Vector3(-1370.819f, -684.5463f, 25.01069f), 214.6929f, LocationType.ConvenienceStore, "The Grain Of Truth", "The Grain Of Truth") { Menu = GrainOfTruthMenu },
            new GameLocation(new Vector3(1707.748f, 4792.387f, 41.98377f), 90.42564f, LocationType.ConvenienceStore, "Supermarket", "Supermarket"){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-1539.045f, -900.472f, 10.16951f), 129.0318f, LocationType.ConvenienceStore, "Del Perro Food Market","No Robberies Please!"){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-1359.607f, -963.3494f, 9.699487f), 124.3222f, LocationType.ConvenienceStore, "A&R Market", "A&R Market"){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(53.28459f, -1478.863f, 29.28546f), 187.4217f, LocationType.ConvenienceStore, "Gabriela's Market", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-551.5444f, -855.8014f, 28.28332f), 2.39254f, LocationType.ConvenienceStore, "Save-A-Cent", "Save-A-Cent"){ Menu = ConvenienceStoreMenu, OpenTime = 0, CloseTime = 24 },
            new GameLocation(new Vector3(-1312.64f, -1181.899f, 4.890057f), 271.5434f, LocationType.ConvenienceStore, "Beach Buddie", "Beach Buddie"){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-661.5522f, -915.5651f, 24.61216f), 260.1033f, LocationType.ConvenienceStore, "Convenience Store", "Convenience Store"){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(393.8511f, -804.2294f, 29.29397f), 272.0436f, LocationType.ConvenienceStore, "Food Market", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(410.4163f, -1910.432f, 25.45381f), 88.30214f, LocationType.ConvenienceStore, "Long Pig Mini Mart", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-170.9145f, -1449.77f, 31.64507f), 50.18372f, LocationType.ConvenienceStore, "Cert-L Market", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-3152.666f, 1110.093f, 20.87176f), 245.5282f, LocationType.ConvenienceStore, "Nelsons", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(1331.857f, -1643.031f, 52.13754f), 352.7884f, LocationType.ConvenienceStore, "Convenience Store", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(1089.146f, -776.6399f, 58.34211f), 357.3385f, LocationType.ConvenienceStore, "Chico's Hypermarket", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(1168.982f, -291.6786f, 69.02177f), 324.2124f, LocationType.ConvenienceStore, "Gabriela's Market", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(105.3277f, -1689.715f, 29.29169f), 138.498f, LocationType.ConvenienceStore, "Convenience Store", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-297.9112f, -1332.895f, 31.29597f), 314.8022f, LocationType.ConvenienceStore, "Long Pig Mini Market", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(87.1592f, -1670.73f, 29.16286f), 75.61347f, LocationType.ConvenienceStore, "Convenience Store", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(877.7466f, -132.4732f, 78.59199f), 322.8992f, LocationType.ConvenienceStore, "B.J.'s Market", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-59.0772f, 6523.902f, 31.49085f), 314.5225f, LocationType.ConvenienceStore, "Willie's Supermarket", ""){ Menu = ConvenienceStoreMenu },

            //Gas      
            new GameLocation(new Vector3(-711.9264f, -917.7573f, 19.21472f), 180.3014f,new Vector3(-705.7453f, -913.6598f, 19.21559f), 83.75771f, LocationType.GasStation, "LtD Little Seoul", "A one-stop shop!"){ Menu = LTDMenu,OpenTime = 0, CloseTime = 24, InteriorID = 47874 },
            new GameLocation(new Vector3(1698.097f, 4929.837f, 42.0781f), 48.2484f,new Vector3(1698.044f, 4922.526f, 42.06367f), 314.3236f, LocationType.GasStation, "LtD Grapeseed", "A one-stop shop!"){ Menu = LTDMenu,OpenTime = 0, CloseTime = 24, InteriorID = 45570 },
            new GameLocation(new Vector3(1159.861f, -327.4188f, 69.21286f), 188.791f,new Vector3(1164.927f, -323.7075f, 69.2051f), 90.6181f, LocationType.GasStation, "LtD Mirror Park","A one-stop shop!"){ Menu = LTDMenu,OpenTime = 0, CloseTime = 24, InteriorID = 2050 },
            new GameLocation(new Vector3(166.2001f, -1553.691f, 29.26175f), 218.9514f, LocationType.GasStation, "Ron", "") { Menu = RonMenu, OpenTime = 4, CloseTime = 22, CameraPosition = new Vector3(175.2995f, -1593.878f, 39.27175f), CameraDirection = new Vector3(-0.1031758f, 0.9726905f, -0.2079136f), CameraRotation = new Rotator(-12.00011f, 0f, 6.054868f) },
            new GameLocation(new Vector3(-1427.998f, -268.4702f, 46.2217f), 132.4002f, LocationType.GasStation, "Ron", "Ron"){ Menu = RonMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(2559.112f, 373.5359f, 108.6211f), 265.8011f, LocationType.GasStation, "Ron", "") { Menu = RonMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-1429.33f,-270.8909f,46.2077f), 325.7301f, LocationType.GasStation, "Ron","") { Menu = RonMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-2544.116f, 2315.928f, 33.21614f), 3.216755f, LocationType.GasStation, "Ron", ""){ Menu = RonMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(818.2819f, -1040.907f, 26.75079f), 358.5326f, LocationType.GasStation, "Ron", ""){ Menu = RonMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(1211.169f, -1388.923f, 35.3769f), 180.4454f, LocationType.GasStation, "Ron", ""){ Menu = RonMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-1817.871f,787.0063f,137.917f), 89.38248f, LocationType.GasStation, "LtD","A one-stop shop!"){ Menu = LTDMenu,OpenTime = 0, CloseTime = 24 },
            new GameLocation(new Vector3(-531.5529f, -1220.763f, 18.455f), 347.6858f, LocationType.GasStation, "Xero", ""){ Menu = XeroMenu,OpenTime = 0, CloseTime = 24,BannerImage = "xero.png" },
            new GameLocation(new Vector3(289.5112f, -1266.584f, 29.44076f), 92.24692f, LocationType.GasStation, "Xero", ""){ Menu = XeroMenu,OpenTime = 0, CloseTime = 24,BannerImage = "xero.png" },
            new GameLocation(new Vector3(-92.79028f, 6409.667f, 31.64035f), 48.08112f, LocationType.GasStation, "Xero", ""){ Menu = XeroMenu,OpenTime = 0, CloseTime = 24,BannerImage = "xero.png" },
            new GameLocation(new Vector3(46.75933f, 2789.635f, 58.10043f), 139.5097f, LocationType.GasStation, "Xero", ""){ Menu = XeroMenu,OpenTime = 0, CloseTime = 24,BannerImage = "xero.png" },
            new GameLocation(new Vector3(160.4977f,6635.249f,31.61175f), 70.88637f, LocationType.GasStation, "Dons Country Store & Gas","Country Manners!") { Menu = GasStationMenu },
            new GameLocation(new Vector3(266.2746f,2599.669f,44.7383f), 231.9223f, LocationType.GasStation, "Harmony General Store & Gas","Always in Harmony!") { Menu = GasStationMenu },
            new GameLocation(new Vector3(1039.753f,2666.26f,39.55253f), 143.6208f, LocationType.GasStation, "Grande Senora Cafe & Gas","Extra Grande!") { Menu = GasStationMenu },
            new GameLocation(new Vector3(2001.239f, 3779.786f, 32.18078f), 208.5214f, LocationType.GasStation, "Sandy's Gas", "And Full Service!"){ Menu = GasStationMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(646.0997f, 267.417f, 103.2494f), 58.99448f, LocationType.GasStation, "Globe Oil", "Globe Oil") { Menu = GasStationMenu },
            new GameLocation(new Vector3(-342.2267f, -1475.199f, 30.74949f), 265.7801f, LocationType.GasStation, "Globe Oil", "") { Menu = GasStationMenu },
            new GameLocation(new Vector3(1776.308f, 3327.483f, 41.43329f), 328.0875f, LocationType.GasStation, "Flywheels Gas", "Flywheels Gas") { Menu = GasStationMenu },
            new GameLocation(new Vector3(1201.978f, 2654.854f, 37.85188f), 315.5364f, LocationType.GasStation, "Route 68 Store", "") { Menu = GasStationMenu },
            //Food Stand
            new GameLocation(new Vector3(403.3527f, 106.0655f, 101.4575f), 241.199f,new Vector3(403.3527f, 106.0655f, 101.4575f), 241.199f, LocationType.FoodStand, "Beefy Bills Burger Bar", "Extra BEEFY!"){ Menu = BeefyBillsMenu,BannerImage = "beefybills.png" },
            new GameLocation(new Vector3(245.8918f, 161.5893f, 104.9487f), 3.803493f,new Vector3(245.8918f, 161.5893f, 104.9487f), 3.803493f, LocationType.FoodStand, "Beefy Bills Burger Bar", "Extra BEEFY!"){ Menu = BeefyBillsMenu,BannerImage = "beefybills.png" },
            new GameLocation(new Vector3(-1268.011f, -1432.715f, 4.353373f), 134.2259f,new Vector3(-1268.011f, -1432.715f, 4.353373f), 134.2259f, LocationType.FoodStand, "Beefy Bills Burger Bar","Extra BEEFY!"){ Menu = BeefyBillsMenu,BannerImage = "beefybills.png" },
            new GameLocation(new Vector3(-1232.426f, -1485.006f, 4.362638f), 137.5475f,new Vector3(-1232.426f, -1485.006f, 4.362638f), 137.5475f, LocationType.FoodStand, "Beefy Bills Burger Bar", "Extra BEEFY!"){ Menu = BeefyBillsMenu,BannerImage = "beefybills.png" },
            new GameLocation(new Vector3(821.2138f, -2977.05f, 6.02066f), 272.7679f,new Vector3(821.2138f, -2977.05f, 6.02066f), 272.7679f, LocationType.FoodStand, "Beefy Bills Burger Bar", "Extra BEEFY!"){ Menu = BeefyBillsMenu,BannerImage = "beefybills.png" },
            new GameLocation(new Vector3(240.8329f, 167.2296f, 105.0605f), 167.5996f,new Vector3(240.8329f, 167.2296f, 105.0605f), 167.5996f, LocationType.FoodStand, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(-1516.382f, -952.5892f, 9.278718f), 317.7292f,new Vector3(-1516.382f, -952.5892f, 9.278718f), 317.7292f, LocationType.FoodStand, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes.") { Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(1604.818f, 3822.332f, 34.69806f), 200.7076f,new Vector3(1607.818f, 3822.332f, 34.69806f), 200.7076f, LocationType.FoodStand, "Chihuahua Hot Dog", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(-1248.932f, -1474.449f, 4.277946f), 306.3787f,new Vector3(-1248.932f, -1474.449f, 4.277946f), 306.3787f, LocationType.FoodStand, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(821.8197f, -2973.398f, 6.020657f), 276.5136f,new Vector3(821.8197f, -2973.398f, 6.020657f), 276.5136f, LocationType.FoodStand, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(-1219.656f, -1504.36f, 4.36032f), 98.7149f,new Vector3(-1219.656f, -1504.36f, 4.36032f), 98.7149f, LocationType.FoodStand, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(821.7623f, -2973.566f, 6.020659f), 269.9576f,new Vector3(821.7623f, -2973.566f, 6.020659f), 269.9576f, LocationType.FoodStand, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(2106.954f, 4947.911f, 40.95187f), 319.9109f,new Vector3(2106.954f, 4947.911f, 40.95187f), 319.9109f, LocationType.FoodStand, "Attack-A-Taco", "Heavy Shelling!") { Menu = TacoFarmerMenu },
            new GameLocation(new Vector3(-1148.969f, -1601.963f, 4.390241f), 35.73399f,new Vector3(-1145.969f, -1602.963f, 4.390241f), 35.73399f, LocationType.FoodStand, "Gyro Day", "Gyro Day") { Menu = GenericMenu },
            new GameLocation(new Vector3(1604.578f, 3828.483f, 34.4987f), 142.3778f,new Vector3(1604.578f, 3828.483f, 34.4987f), 142.3778f, LocationType.FoodStand, "Tough Nut Donut", "Our DoNuts are Crazy!"){ Menu = DonutMenu },
            new GameLocation(new Vector3(1087.509f, 6510.788f, 21.0551f), 185.487f,new Vector3(1087.509f, 6510.788f, 21.0551f), 185.487f, LocationType.FoodStand, "Roadside Fruit", "Should Be OK To Eat") { Menu = FruitMenu },
            new GameLocation(new Vector3(2526.548f, 2037.936f, 19.82413f), 263.8982f,new Vector3(2526.548f, 2037.936f, 19.82413f), 263.8982f, LocationType.FoodStand, "Roadside Fruit", "Should Be OK To Eat") { Menu = FruitMenu },
            new GameLocation(new Vector3(1263.013f, 3548.566f, 35.14751f), 187.8834f,new Vector3(1263.013f, 3548.566f, 35.14751f), 187.8834f, LocationType.FoodStand, "Roadside Fruit", "Should Be OK To Eat") { Menu = FruitMenu },
            new GameLocation(new Vector3(1675.873f, 4883.532f, 42.06379f), 57.34329f,new Vector3(1675.873f, 4883.532f, 42.06379f), 57.34329f, LocationType.FoodStand, "Grapeseed Fruit", "Grapeseed Fruit") { Menu = FruitMenu },
            new GameLocation(new Vector3(-462.6676f, 2861.85f, 34.90421f), 162.4888f,new Vector3(-462.6676f, 2861.85f, 34.90421f), 162.4888f, LocationType.FoodStand, "Roadside Fruit", "Roadside Fruit") { Menu = FruitMenu },
            //Bar
            new GameLocation(new Vector3(224.5178f, 336.3819f, 105.5973f), 340.0694f, LocationType.Bar, "Pitchers", "Pitchers") { Menu = BarMenu },
            new GameLocation(new Vector3(219.5508f, 304.9488f, 105.5861f), 250.1051f, LocationType.Bar, "Singletons", "Singletons") { Menu = BarMenu },
            new GameLocation(new Vector3(1982.979f, 3053.596f, 47.21508f), 226.3188f,new Vector3(1982.979f, 3053.596f, 47.21508f), 226.3188f, LocationType.Bar, "Yellow Jacket Inn", "Yellow Jacket Inn") { Menu = BarMenu },
            new GameLocation(new Vector3(-262.8396f, 6291.08f, 31.49327f), 222.9271f, LocationType.Bar, "The Bay Bar", ""){ Menu = BarMenu },
            new GameLocation(new Vector3(-576.9105f, 239.0964f, 82.63644f), 354.0043f, LocationType.Bar, "The Last Resort", "") { Menu = BarMenu },
            new GameLocation(new Vector3(255.3016f, -1013.603f, 29.26964f), 70.28053f, LocationType.Bar, "Shenanigan's Bar", "") { Menu = BarMenu },
            new GameLocation(new Vector3(1218.175f, -416.5078f, 67.78294f), 74.95883f, LocationType.Bar, "Mirror Park Tavern", "") { Menu = BarMenu },
            new GameLocation(new Vector3(-1388.5f, -586.6741f, 30.21859f), 31.53231f,new Vector3(-1391.372f, -605.995f, 30.31955f), 116.404f, LocationType.Bar, "Bahama Mama's", "") { Menu = BarMenu,InteriorID = 107778, TeleportEnterPosition = new Vector3(-1387.984f, -587.4419f, 30.31951f), TeleportEnterHeading = 210.6985f, VendorModels = new List<string>() {"s_f_y_clubbar_01","s_m_y_clubbar_01","a_f_y_clubcust_01" } },
            new GameLocation(new Vector3(-564.6519f, 276.2436f, 83.12064f), 175.5771f,new Vector3(-561.9947f, 284.9062f, 82.17636f), 262.2369f, LocationType.Bar, "Tequila-La", "Tequila-La") { Menu = BarMenu, InteriorID = 72706, VendorModels = new List<string>() {"s_f_y_clubbar_01","s_m_y_clubbar_01","a_f_y_clubcust_01" } },//need better coordinates
            //Restaurant Fancy
            new GameLocation(new Vector3(-1487.163f, -308.0127f, 47.02639f), 231.5184f, LocationType.Restaurant, "Las Cuadras Restaurant", "Las Cuadras Restaurant")  {Menu = FancyDeliMenu },
            new GameLocation(new Vector3(-1473.121f, -329.6028f, 44.81668f), 319.3725f, LocationType.Restaurant, "Las Cuadras Deli", "Las Cuadras Deli")  {Menu = FancyDeliMenu },
            new GameLocation(new Vector3(-1221.254f, -1095.873f, 8.115647f), 111.3174f, LocationType.Restaurant, "Prawn Vivant", "Prawn Vivant") {Menu = FancyFishMenu },
            new GameLocation(new Vector3(-1256.581f, -1079.491f, 8.398257f), 339.5968f, LocationType.Restaurant, "Surfries Diner", "Surfries Diner") {Menu = FancyDeliMenu },
            new GameLocation(new Vector3(-1247.504f, -1105.777f, 8.109305f), 289.9685f, LocationType.Restaurant, "The Split Kipper Fish", "The Split Kipper Fish") {Menu = FancyFishMenu },
            new GameLocation(new Vector3(-1230.032f, -1174.862f, 7.700727f), 330.9398f, LocationType.Restaurant, "Pot Heads Seafood", "Pot Heads Seafood") {Menu = FancyFishMenu },
            new GameLocation(new Vector3(-1111.103f, -1454.387f, 5.582287f), 304.9954f, LocationType.Restaurant, "Coconut Cafe", "Coconut Cafe") {Menu = FancyDeliMenu },
            new GameLocation(new Vector3(-1037.587f, -1397.168f, 5.553192f), 76.84702f, LocationType.Restaurant, "La Spada", "La Spada") {Menu = FancyFishMenu },
            new GameLocation(new Vector3(-1129.253f, -1373.276f, 5.056143f), 164.9213f, LocationType.Restaurant, "Marlin's Cafe", "Marlin's Cafe") {Menu = FancyDeliMenu },
            new GameLocation(new Vector3(2561.869f, 2590.851f, 38.08311f), 294.6638f, LocationType.Restaurant, "Rex's Diner", "Rex's Diner") {Menu = FancyDeliMenu },
            new GameLocation(new Vector3(2697.521f, 4324.264f, 45.98642f), 41.67124f, LocationType.Restaurant, "Park View Diner", "Park View Diner") {Menu = FancyGenericMenu },
            new GameLocation(new Vector3(-1389.63f, -744.4225f, 24.62544f), 127.01f, LocationType.Restaurant, "Haute", "Haute") {Menu = FancyDeliMenu },
            new GameLocation(new Vector3(-1392.125f, -732.2938f, 24.64698f), 37.13289f, LocationType.Restaurant, "Les Bianco's", "Les Bianco's") {Menu = FancyGenericMenu },
            new GameLocation(new Vector3(-1420.425f, -709.2584f, 24.60311f), 126.5269f, LocationType.Restaurant, "Pescado Rojo", "Pescado Rojo") {Menu = FancyFishMenu },
            new GameLocation(new Vector3(-1335.517f, -660.6063f, 26.51026f), 212.5534f, LocationType.Restaurant, "The Fish Net", "The Fish Net") {Menu = FancyFishMenu },
            new GameLocation(new Vector3(95.79929f, -1682.817f, 29.25364f), 138.4551f, LocationType.Restaurant, "Yum Fish", "") {Menu = FancyFishMenu },
            new GameLocation(new Vector3(-238.903f, -777.356f, 34.09171f), 71.47642f, LocationType.Restaurant, "Cafe Redemption", "") {Menu = FancyDeliMenu },
            new GameLocation(new Vector3(370.4181f, -1027.565f, 29.33361f), 184.4234f, LocationType.Restaurant, "Ground & Pound Cafe", "") {Menu = FancyGenericMenu },
            new GameLocation(new Vector3(502.6628f, 113.1527f, 96.62571f), 164.3104f, LocationType.Restaurant, "Jazz Desserts", ""){Menu = FancyGenericMenu },
            new GameLocation(new Vector3(16.29523f, -166.5288f, 55.82795f), 341.2336f, LocationType.Restaurant, "The Fish Net", ""){Menu = FancyFishMenu },
            new GameLocation(new Vector3(281.4706f, -800.5342f, 29.31682f), 227.4364f, LocationType.Restaurant, "Pescado Azul", ""){Menu = FancyFishMenu },
            new GameLocation(new Vector3(-630.0808f, -2266.577f, 5.933444f), 242.2394f, LocationType.Restaurant, "Poppy House", ""){Menu = FancyGenericMenu },
            new GameLocation(new Vector3(-122.6395f, 6389.315f, 32.17757f), 44.91608f, LocationType.Restaurant, "Mojito Inn", ""){Menu = FancyGenericMenu },
            new GameLocation(new Vector3(793.877f, -735.7111f, 27.96293f), 89.91832f, LocationType.Restaurant, "Casey's Diner", "") {Menu = FancyDeliMenu },
            new GameLocation(new Vector3(-361.3964f, 275.7073f, 86.42198f), 212.2804f, LocationType.Restaurant, "Last Train Diner", "") {Menu = FancyDeliMenu },
            new GameLocation(new Vector3(-3047.75f, 615.6793f, 7.405877f), 248.6187f, LocationType.Restaurant, "Mom's Pie Diner", "") {Menu = FancyDeliMenu },
            new GameLocation(new Vector3(-40.46507f, 228.3116f, 107.968f), 78.85908f, LocationType.Restaurant, "Haute", ""){Menu = FancyFishMenu },
            new GameLocation(new Vector3(398.0291f, 175.6789f, 103.8558f), 71.74517f, LocationType.Restaurant, "Clappers", "") {Menu = FancyGenericMenu },
            //Sandwiches
            new GameLocation(new Vector3(-1249.812f, -296.1564f, 37.35062f), 206.9039f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
            new GameLocation(new Vector3(-1539.498f, -427.3804f, 35.59194f), 233.1319f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
            new GameLocation(new Vector3(229.5384f, -22.3363f, 74.98735f), 160.0777f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
            new GameLocation(new Vector3(-240.7315f, -346.1899f, 30.02782f), 47.8591f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
            new GameLocation(new Vector3(-263.1924f, -904.2821f, 32.3108f), 338.4021f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
            new GameLocation(new Vector3(385.958f, -1010.523f, 29.41794f), 271.5127f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
            new GameLocation(new Vector3(1139.359f, -463.8952f, 66.85857f), 261.1642f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
            new GameLocation(new Vector3(100.5837f, 209.4958f, 107.9911f), 342.4262f, LocationType.Restaurant, "The Pink Sandwich", "The Pink Sandwich") {Menu = SandwichMenu,OpenTime = 0, CloseTime = 24 },
            //Asian
            new GameLocation(new Vector3(-798.0056f, -632.0029f, 29.02696f), 169.2606f, LocationType.Restaurant, "S.Ho", "S.Ho Noodles") {Menu = NoodleMenu,BannerImage ="sho.png" },
            new GameLocation(new Vector3(-638.5052f, -1249.646f, 11.81044f), 176.4081f, LocationType.Restaurant, "S.Ho", "S.Ho Noodles") {Menu = NoodleMenu,BannerImage ="sho.png" },
            new GameLocation(new Vector3(-700.9553f, -884.5563f, 23.79126f), 41.62328f, LocationType.Restaurant, "S.Ho", "S.Ho Noodles") {Menu = NoodleMenu,BannerImage ="sho.png" },
            new GameLocation(new Vector3(-1229.61f, -285.7077f, 37.73843f), 205.5755f, LocationType.Restaurant, "Noodle Exchange", "You Won't Want To Share!") {Menu = NoodleMenu },
            new GameLocation(new Vector3(-1199.53f, -1162.439f, 7.696731f), 107.0593f, LocationType.Restaurant, "Noodle Exchange", "You Won't Want To Share!") {Menu = NoodleMenu },
            new GameLocation(new Vector3(272.8409f, -965.4847f, 29.31605f), 27.34526f, LocationType.Restaurant, "Noodle Exchange", "You Won't Want To Share!") {Menu = NoodleMenu },
            new GameLocation(new Vector3(-655.6034f, -880.3672f, 24.67554f), 265.7094f, LocationType.Restaurant, "Wook Noodle House", "Wook Noodle House") {Menu = NoodleMenu },
            new GameLocation(new Vector3(-680.4404f, -945.5441f, 20.93157f), 180.6927f, LocationType.Restaurant, "Wook Noodle House", "Wook Noodle House") {Menu = NoodleMenu },
            new GameLocation(new Vector3(-654.8373f, -885.7593f, 24.67703f), 273.4168f, LocationType.Restaurant, "Park Jung Restaurant", "Park Jung Restaurant") {Menu = GenericMenu },
            new GameLocation(new Vector3(-661.5396f, -907.5895f, 24.60632f), 278.5222f, LocationType.Restaurant, "Hwan Cafe", "Hwan Cafe") {Menu = GenericMenu },
            new GameLocation(new Vector3(-163.0659f, -1440.267f, 31.42698f), 55.5593f, LocationType.Restaurant, "Wok It Off", "") {Menu = GenericMenu },
            new GameLocation(new Vector3(1894.635f, 3715.372f, 32.74969f), 119.2431f, LocationType.Restaurant, "Chinese Food", "") {Menu = GenericMenu },
            //Italian
            new GameLocation(new Vector3(-1182.659f, -1410.577f, 4.499721f), 215.9843f, LocationType.Restaurant, "Al Dentes", "Al Dentes") {Menu = AlDentesMenu },
            new GameLocation(new Vector3(-213.0357f, -40.15178f, 50.04371f), 157.8173f, LocationType.Restaurant, "Al Dentes", "Al Dentes") {Menu = AlDentesMenu },
            new GameLocation(new Vector3(-1393.635f, -919.5128f, 11.24511f), 89.35195f, LocationType.Restaurant, "Al Dentes", "Al Dentes"){ Menu = AlDentesMenu },
            new GameLocation(new Vector3(215.2669f, -17.14256f, 74.98737f), 159.7144f, LocationType.Restaurant, "Pizza This...", "Pizza This...") {Menu = PizzaMenu },
            new GameLocation(new Vector3(538.3118f, 101.4798f, 96.52515f), 159.4801f, LocationType.Restaurant, "Pizza This...", "Pizza This...") {Menu = PizzaMenu },
            new GameLocation(new Vector3(287.5003f, -964.0207f, 29.41863f), 357.0406f, LocationType.Restaurant, "Pizza This...", "Pizza This...") {Menu = PizzaMenu },
            new GameLocation(new Vector3(443.7377f, 135.1464f, 100.0275f), 161.2897f, LocationType.Restaurant, "Guidos Takeout 24/7", "Guidos Takeout 24/7") {Menu = PizzaMenu },
            new GameLocation(new Vector3(-1320.907f, -1318.505f, 4.784881f), 106.5257f, LocationType.Restaurant, "Pebble Dash Pizza", "Pebble Dash Pizza"){ Menu = PizzaMenu },
            new GameLocation(new Vector3(-1334.007f, -1282.623f, 4.835985f), 115.3464f, LocationType.Restaurant, "Slice N Dice Pizza","Slice UP!"){ Menu = PizzaMenu},
            new GameLocation(new Vector3(-1296.815f, -1387.3f, 4.544102f), 112.4694f, LocationType.Restaurant, "Sharkies Bites","Take A Bite Today!"){ Menu = PizzaMenu, IsWalkup = true },
            new GameLocation(new Vector3(-1342.607f, -872.2929f, 16.87064f), 312.7196f, LocationType.Restaurant, "Giovanni's Italian", "Giovanni's Italian"){ Menu = PizzaMenu },
            //Burger
            new GameLocation(new Vector3(-1535.117f, -454.0615f, 35.92439f), 319.1095f, LocationType.Restaurant, "Wigwam", "No need for reservations") { Menu = WigwamMenu ,BannerImage = "wigwam.png"},
            new GameLocation(new Vector3(-860.8414f, -1140.393f, 7.39234f), 171.7175f, LocationType.Restaurant, "Wigwam", "No need for reservations") { Menu = WigwamMenu,BannerImage = "wigwam.png" },
            new GameLocation(new Vector3(-1540.86f, -454.866f, 40.51906f), 321.1314f, LocationType.Restaurant, "Up-N-Atom", "Never Frozen, Often Microwaved") {Menu = UpNAtomMenu,OpenTime = 0, CloseTime = 24,BannerImage = "upnatom.png"},
            new GameLocation(new Vector3(81.31124f, 275.1125f, 110.2102f), 162.7602f, LocationType.Restaurant, "Up-N-Atom", "Never Frozen, Often Microwaved") {Menu = UpNAtomMenu,OpenTime = 0, CloseTime = 24,BannerImage = "upnatom.png"},
            new GameLocation(new Vector3(1591.054f, 6451.071f, 25.31714f), 158.0088f, LocationType.Restaurant, "Up-N-Atom Diner", "Never Frozen, Often Microwaved") {Menu = UpNAtomMenu,OpenTime = 0, CloseTime = 24,BannerImage = "upnatom.png"},
            new GameLocation(new Vector3(-1183.638f, -884.3126f, 13.79987f), 303.1936f, LocationType.Restaurant, "Burger Shot", "Burger Shot") {Menu = BurgerShotMenu },
            new GameLocation(new Vector3(1241.453f, -366.7538f, 69.08221f), 164.3345f, LocationType.Restaurant, "Horny's Burgers", ""){Menu = BeefyBillsMenu },
            new GameLocation(new Vector3(-512.6821f, -683.3517f, 33.18555f), 3.720508f, LocationType.Restaurant, "Snr. Buns", "Snr. Buns") {Menu = GenericMenu },
            new GameLocation(new Vector3(-526.9481f, -679.6907f, 33.67113f), 35.17997f, LocationType.Restaurant, "Snr. Muffin", "Snr. Muffin") {Menu = GenericMenu },//??? 
            new GameLocation(new Vector3(125.9558f, -1537.896f, 29.1772f), 142.693f, LocationType.Restaurant, "La Vaca Loca", "") {Menu = BeefyBillsMenu, CameraPosition = new Vector3(137.813f, -1561.211f, 37.43506f), CameraDirection = new Vector3(-0.1290266f, 0.9696004f, -0.2079113f), CameraRotation = new Rotator(-11.99998f, -2.182118E-07f, 7.579925f) },
            new GameLocation(new Vector3(-241.8231f, 279.747f, 92.04223f), 177.4421f, LocationType.Restaurant, "Spitroasters", ""){Menu = BeefyBillsMenu },
            //Bagels&Donuts
            new GameLocation(new Vector3(-1318.507f, -282.2458f, 39.98732f), 115.4663f, LocationType.Restaurant, "Dickies Bagels", "Holy Dick!") { Menu = CoffeeMenu },
            new GameLocation(new Vector3(-1204.364f, -1146.246f, 7.699615f), 109.2444f, LocationType.Restaurant, "Dickies Bagels", "Holy Dick!") { Menu = CoffeeMenu },
            new GameLocation(new Vector3(354.0957f, -1028.134f, 29.33102f), 182.3497f, LocationType.Restaurant, "Rusty Browns", "") { Menu = CoffeeMenu,BannerImage = "rustybrowns.png" },
            new GameLocation(new Vector3(55.28403f, -799.5469f, 31.58599f), 341.3315f, LocationType.Restaurant, "Ground & Pound Cafe", "") { Menu = CoffeeMenu },
            //Coffee
            new GameLocation(new Vector3(-1283.567f, -1130.118f, 6.795891f), 143.1178f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = BeanMachineMenu,BannerImage = "beanmachine.png" },
            new GameLocation(new Vector3(-1549.39f, -435.5105f, 35.88667f), 234.6563f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = BeanMachineMenu,BannerImage = "beanmachine.png" },
            new GameLocation(new Vector3(-835.4522f, -610.4766f, 29.02697f), 142.0655f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = BeanMachineMenu,BannerImage = "beanmachine.png" },
            new GameLocation(new Vector3(-602.2112f, -1105.766f, 22.32427f), 273.8795f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = BeanMachineMenu,BannerImage = "beanmachine.png" },
            new GameLocation(new Vector3(-659.5289f, -814.0433f, 24.53778f), 232.0023f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = BeanMachineMenu ,BannerImage = "beanmachine.png"},
            new GameLocation(new Vector3(-687.0801f, -855.6792f, 23.89398f), 0.2374549f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = BeanMachineMenu,BannerImage = "beanmachine.png" },
            new GameLocation(new Vector3(-1345.296f, -609.976f, 28.61888f), 304.4266f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = BeanMachineMenu,BannerImage = "beanmachine.png" },
            new GameLocation(new Vector3(-270.3488f, -977.3488f, 31.21763f), 164.5747f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = BeanMachineMenu,BannerImage = "beanmachine.png" },
            new GameLocation(new Vector3(127.9072f, -1028.778f, 29.43674f), 336.4557f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = BeanMachineMenu,BannerImage = "beanmachine.png" },
            new GameLocation(new Vector3(-627.6302f, 239.2284f, 81.88939f), 86.57707f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = BeanMachineMenu,BannerImage = "beanmachine.png" },
            new GameLocation(new Vector3(280.5522f, -964.0756f, 29.41863f), 357.4615f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = BeanMachineMenu,BannerImage = "beanmachine.png" },
            new GameLocation(new Vector3(-1206.975f, -1135.029f, 7.693257f), 109.1408f, LocationType.Restaurant, "Cool Beans Coffee Co", "Cool Beans Coffee Co")  { Menu = CoffeeMenu },
            new GameLocation(new Vector3(-1278.833f, -876.438f, 11.9303f), 123.2498f, LocationType.Restaurant, "Cool Beans Coffee Co", "Cool Beans Coffee Co") { Menu = CoffeeMenu },
            new GameLocation(new Vector3(1169.704f, -403.1992f, 72.24859f), 344.0863f, LocationType.Restaurant, "Cool Beans Coffee Co", "Cool Beans Coffee Co") { Menu = CoffeeMenu },
            new GameLocation(new Vector3(265.4628f, -981.3839f, 29.36569f), 72.07395f, LocationType.Restaurant, "Cool Beans Coffee Co", "Cool Beans Coffee Co") { Menu = CoffeeMenu },
            new GameLocation(new Vector3(-1108.847f, -1355.264f, 5.035112f), 206.1676f, LocationType.Restaurant, "Crucial Fix Coffee", "Crucial Fix Coffee") { Menu = CoffeeMenu },
            new GameLocation(new Vector3(189.0311f, -231.234f, 54.07472f), 340.4597f, LocationType.Restaurant, "Crucial Fix Coffee", "") { Menu = CoffeeMenu },
            new GameLocation(new Vector3(273.174f, -833.0611f, 29.41237f), 185.6476f, LocationType.Restaurant, "Crucial Fix Coffee", "") { Menu = CoffeeMenu },
            new GameLocation(new Vector3(-576.6631f, -677.8674f, 32.36259f), 306.9058f, LocationType.Restaurant, "Hit-N-Run Coffee", "Hit-N-Run Coffee"){ Menu = CoffeeMenu },
            new GameLocation(new Vector3(-1253.337f, -296.6488f, 37.31522f), 206.5786f, LocationType.Restaurant, "{java.update();}", "Enjoy Hot Coffee") { Menu = CoffeeMenu },
            new GameLocation(new Vector3(-509.1889f, -22.9895f, 45.60899f), 354.7263f, LocationType.Restaurant, "Little Teapot", "Little Teapot") { Menu = CoffeeMenu },
            //Mexican
            new GameLocation(new Vector3(10.96682f, -1605.874f, 29.3931f), 229.8729f, LocationType.Restaurant, "The Taco Farmer", "Open All Hours!") {Menu = TacoFarmerMenu, OpenTime = 0, CloseTime = 24 },
            new GameLocation(new Vector3(649.765f, 2728.621f, 41.9959f), 276.2882f, LocationType.Restaurant, "Taco Farmer", "Open All Hours!") {Menu = TacoFarmerMenu, OpenTime = 0, CloseTime = 24 },
            new GameLocation(new Vector3(-1168.281f, -1267.279f, 6.198249f), 111.9682f, LocationType.Restaurant, "Taco Libre", "Taco Libre") {Menu = TacoFarmerMenu },
            new GameLocation(new Vector3(-657.5089f, -679.4656f, 31.46727f), 317.9819f, LocationType.Restaurant, "Taco Bomb", "My taco looks so tasty!") {Menu = TacoBombMenu,BannerImage = "tacobomb.png" },
            new GameLocation(new Vector3(-1196.981f, -791.5534f, 16.40427f), 134.7115f, LocationType.Restaurant, "Taco Bomb", "My taco looks so tasty!") {Menu = TacoBombMenu,BannerImage = "tacobomb.png" },
            new GameLocation(new Vector3(-1553.112f, -439.9938f, 40.51905f), 228.7506f, LocationType.Restaurant, "Taco Bomb", "My taco looks so tasty!") {Menu = TacoBombMenu,BannerImage = "tacobomb.png" },
            new GameLocation(new Vector3(99.21678f, -1419.307f, 29.42156f), 323.9604f, LocationType.Restaurant, "Aguila Burrito", "") {Menu = TacoFarmerMenu },
            new GameLocation(new Vector3(445.9454f, -1241.581f, 30.27799f), 179.553f, LocationType.Restaurant, "Attack-A-Taco", "") {Menu = TacoFarmerMenu },
            new GameLocation(new Vector3(1093.13f, -362.9193f, 67.06821f), 168.6222f, LocationType.Restaurant, "Hearty Taco", "") {Menu = TacoFarmerMenu },
            new GameLocation(new Vector3(438.8823f, -1465.908f, 29.35293f), 69.18111f, LocationType.Restaurant, "Hearty Taco", "") {Menu = TacoFarmerMenu },
            new GameLocation(new Vector3(174.9638f, -2025.427f, 18.32407f), 123.4303f, LocationType.Restaurant, "Mom's Tacos", "") {Menu = TacoFarmerMenu },
            new GameLocation(new Vector3(1138.658f, -962.4339f, 47.54031f), 330.8736f, LocationType.Restaurant, "Mom's Tacos", "") {Menu = TacoFarmerMenu },
            //Ice Cream
            new GameLocation(new Vector3(-1193.966f, -1543.693f, 4.373522f), 124.3727f, LocationType.Restaurant, "The Sundae Post", "The Sundae Post") {Menu = DonutMenu },
            new GameLocation(new Vector3(-1171.529f, -1435.118f, 4.461945f), 32.60835f, LocationType.Restaurant, "Ice Maiden", "Ice Maiden") {Menu = GenericMenu, IsWalkup = true },
            //Juice and Smoothies
            new GameLocation(new Vector3(-1137.926f, -1624.695f, 4.410712f), 127.6497f, LocationType.Restaurant, "Vitamin Seaside Juice Bar", "Vitamin Seaside Juice Bar"){ Menu = FruitMenu },
            new GameLocation(new Vector3(-1187.057f, -1536.73f, 4.379496f), 32.85152f, LocationType.Restaurant, "Muscle Peach Juice Bar", "Muscle Peach Juice Bar"){ Menu = FruitMenu },
            new GameLocation(new Vector3(-1236.263f, -287.9641f, 37.63038f), 205.8273f, LocationType.Restaurant, "Limey's Juice and Smoothies", "No Limes About It") {Menu = GenericMenu },
            new GameLocation(new Vector3(250.3842f, -1026.535f, 29.25663f), 124.0668f, LocationType.Restaurant, "Limey's Juice and Smoothies", "") {Menu = GenericMenu },
            new GameLocation(new Vector3(-1182.576f, -1248.037f, 6.991587f), 110.7639f, LocationType.Restaurant, "Limey's Juice Bar", "No Limes About It") {Menu = GenericMenu },
            new GameLocation(new Vector3(-471.7646f, -18.46761f, 45.75837f), 357.2652f, LocationType.Restaurant, "Fruit Machine", "Fruit Machine"){Menu = GenericMenu },
            new GameLocation(new Vector3(2741.563f, 4413.093f, 48.62326f), 201.5914f, LocationType.Restaurant, "Big Juice Stand", "Big Juice Stand"){Menu = GenericMenu },
            new GameLocation(new Vector3(1791.592f, 4594.844f, 37.68291f), 182.8134f, LocationType.Restaurant, "Alamo Fruit", "Alamo Fruit"){ Menu = FruitMenu },
            new GameLocation(new Vector3(1199.951f, -501.2592f, 65.17791f), 113.7728f, LocationType.Restaurant, "Squeeze One Out", ""){ Menu = FruitMenu },
            //Chicken
            new GameLocation(new Vector3(-584.761f, -872.753f, 25.91489f), 353.0746f, LocationType.Restaurant, "Lucky Plucker", "Lucky Plucker") {Menu = GenericMenu },
            new GameLocation(new Vector3(2580.543f, 464.6521f, 108.6232f), 176.5548f, LocationType.Restaurant, "Bishop's Chicken", "Bishop's Chicken") {Menu = GenericMenu },
            new GameLocation(new Vector3(169.3292f, -1634.163f, 29.29167f), 35.89598f, LocationType.Restaurant, "Bishop's Chicken", "") {Menu = GenericMenu },
            new GameLocation(new Vector3(133.0175f, -1462.702f, 29.35705f), 48.47223f, LocationType.Restaurant, "Lucky Plucker", "") {Menu = GenericMenu },
            new GameLocation(new Vector3(-138.4921f, -256.509f, 43.59497f), 290.1001f, LocationType.Restaurant, "Cluckin' Bell", "") {Menu = CluckinBellMenu },
            new GameLocation(new Vector3(-184.9376f, -1428.169f, 31.47968f), 33.8636f, LocationType.Restaurant, "Cluckin' Bell", "") {Menu = CluckinBellMenu },
            //General    
            new GameLocation(new Vector3(-1222.546f, -807.5845f, 16.59777f), 305.3918f, LocationType.Restaurant, "Lettuce Be", "Lettuce Be") {Menu = GenericMenu },
            new GameLocation(new Vector3(-1196.705f, -1167.969f, 7.695099f), 108.4535f, LocationType.Restaurant, "Lettuce Be", "Lettuce Be") {Menu = GenericMenu },
            new GameLocation(new Vector3(-1535.082f, -422.2449f, 35.59194f), 229.4618f, LocationType.Restaurant, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu },
            new GameLocation(new Vector3(49.24896f, -1000.381f, 29.35741f), 335.6092f, LocationType.Restaurant, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu },
            new GameLocation(new Vector3(-1271.224f, -1200.703f, 5.366248f), 70.19876f, LocationType.Restaurant, "The Nut Buster", "The Nut Buster") {Menu = GenericMenu },
            new GameLocation(new Vector3(166.2677f, -1450.995f, 29.24164f), 142.858f, LocationType.Restaurant, "Ring Of Fire Chili House", "") {Menu = GenericMenu },
            //Drive Thru
            new GameLocation(new Vector3(95.41846f, 285.0295f, 110.2042f), 251.8247f, LocationType.DriveThru, "Up-N-Atom Burger", "Never Frozen, Often Microwaved") {Menu = UpNAtomMenu,OpenTime = 0, CloseTime = 24, BannerImage = "upnatom.png"},
            new GameLocation(new Vector3(15.48935f, -1595.832f, 29.28254f), 319.2816f, LocationType.DriveThru, "The Taco Farmer", "Open All Hours!") {Menu = TacoFarmerMenu },
            new GameLocation(new Vector3(-576.9321f, -880.5195f, 25.70123f), 86.01214f, LocationType.DriveThru, "Lucky Plucker", "Lucky Plucker") {Menu = GenericMenu },
            new GameLocation(new Vector3(2591.213f, 478.8892f, 108.6423f), 270.9569f,LocationType.DriveThru, "Bishop's Chicken", "Bishop's Chicken") {Menu = GenericMenu },
            new GameLocation(new Vector3(144.34f, -1541.275f, 28.36799f), 139.819f, LocationType.DriveThru, "La Vaca Loca", "") {Menu = BeefyBillsMenu },
            new GameLocation(new Vector3(145.3499f, -1460.568f, 28.71129f), 49.75111f, LocationType.DriveThru, "Lucky Plucker", "") {Menu = GenericMenu },
            new GameLocation(new Vector3(1256.509f, -357.1387f, 68.52029f), 347.8622f, LocationType.DriveThru, "Horny's Burgers", "Horny's Burgers") {Menu = BeefyBillsMenu },
            
            //Bank
            new GameLocation(new Vector3(-813.9924f, -1114.698f, 11.18181f), 297.7995f, LocationType.Bank, "Fleeca Bank", "Fleeca Bank"),
            new GameLocation(new Vector3(-350.1604f, -45.84864f, 49.03682f), 337.4063f, LocationType.Bank, "Fleeca Bank", "Fleeca Bank"),
            new GameLocation(new Vector3(-1318f, -831.5065f, 16.97263f), 125.3848f, LocationType.Bank, "Maze Bank", "Maze Bank"),
            new GameLocation(new Vector3(150.9058f, -1036.347f, 29.33961f), 340.9843f, LocationType.Bank, "Fleeca Bank", ""),
            new GameLocation(new Vector3(315.2256f, -275.1059f, 53.92431f), 345.6797f, LocationType.Bank, "Fleeca Bank", ""),
            new GameLocation(new Vector3(-3142.849f, 1131.727f, 20.84295f), 247.9002f, LocationType.Bank, "Blaine County Savings", ""),
            new GameLocation(new Vector3(-2966.905f, 483.1484f, 15.6927f), 86.25156f, LocationType.Bank, "Fleeca Bank", ""),
            new GameLocation(new Vector3(1175.215f, 2702.15f, 38.17273f), 176.9885f, LocationType.Bank, "Fleeca Bank", ""),


            //Pharmacy
            new GameLocation(new Vector3(114.2954f, -4.942202f, 67.82149f), 195.4308f, LocationType.Pharmacy, "Pop's Pills", "") {Menu = PharmacyMenu },
            new GameLocation(new Vector3(68.94705f, -1570.043f, 29.59777f), 50.85398f, LocationType.Pharmacy, "Dollar Pills", "") {Menu = PharmacyMenu },
            new GameLocation(new Vector3(326.7227f, -1074.448f, 29.47332f), 359.3641f, LocationType.Pharmacy, "Family Pharmacy", "") {Menu = PharmacyMenu },
            new GameLocation(new Vector3(805.143f, -1063.586f, 28.42115f), 90.00111f, LocationType.Pharmacy, "Meltz's Pharmacy", "") {Menu = PharmacyMenu },
            new GameLocation(new Vector3(1225.14f, -391.3563f, 68.68563f), 28.81875f, LocationType.Pharmacy, "Pharmacy", "") {Menu = PharmacyMenu },
            new GameLocation(new Vector3(151.0329f, 6647.418f, 31.594f), 135.0961f, LocationType.Pharmacy, "Pop's Pills", "") {Menu = PharmacyMenu },
            new GameLocation(new Vector3(-172.4879f, 6381.202f, 31.47279f), 222.4285f, LocationType.Pharmacy, "Bay Side Drugs", "") {Menu = PharmacyMenu },
            new GameLocation(new Vector3(214.0241f, -1835.08f, 27.54375f), 318.7183f, LocationType.Pharmacy, "Family Pharmacy", "") {Menu = PharmacyMenu },
            new GameLocation(new Vector3(591.2585f, 2744.49f, 42.0425f), 184.8661f, LocationType.Pharmacy, "Dollar Pills", "") {Menu = PharmacyMenu },
            new GameLocation(new Vector3(825.8448f, -97.80138f, 80.59971f), 321.7153f, LocationType.Pharmacy, "Pharmacy", "") {Menu = PharmacyMenu },
            new GameLocation(new Vector3(84.03228f, -810.2528f, 31.41642f), 350.9262f, LocationType.Pharmacy, "Family Pharmacy", "") {Menu = PharmacyMenu },




            //Hardware
            new GameLocation(new Vector3(2747.406f, 3473.213f, 55.67021f), 249.8152f, LocationType.HardwareStore, "You Tool", "You Tool") {Menu = ToolMenu,BannerImage = "youtool.png", CameraPosition = new Vector3(2780.472f, 3473.511f, 73.06239f), CameraDirection = new Vector3(-0.9778581f, -0.02382228f, -0.2079087f), CameraRotation = new Rotator(-11.99983f, 0f, 91.39555f) },
            new GameLocation(new Vector3(339.4021f, -776.9934f, 29.2665f), 68.51967f, LocationType.HardwareStore, "Krapea", ""){Menu = ToolMenu },
            new GameLocation(new Vector3(-10.88182f, 6499.395f, 31.50508f), 44.30542f, LocationType.HardwareStore, "Bay Hardware", ""){Menu = ToolMenu },
            new GameLocation(new Vector3(-3153.697f, 1053.398f, 20.88735f), 338.4756f, LocationType.HardwareStore, "Hardware", ""){Menu = ToolMenu },
            new GameLocation(new Vector3(343.2759f, -1297.948f, 32.5097f), 164.2121f, LocationType.HardwareStore, "Bert's Tool Supply", ""){Menu = ToolMenu },

            //PawnShop
            new GameLocation(new Vector3(412.5109f, 314.9815f, 103.1327f), 207.4105f, LocationType.PawnShop, "F.T. Pawn", "") {Menu = ToolMenu },
            new GameLocation(new Vector3(182.735f, -1318.845f, 29.31752f), 246.2635f, LocationType.PawnShop, "Pawn & Jewelery", "") {Menu = ToolMenu },

            //Strip
            new GameLocation(new Vector3(-379.3227f, 217.9178f, 83.65989f), 355.8053f, LocationType.StripClub, "Hornbills", ""),

            //Car Dealer
            new GameLocation(new Vector3(-69.16984f, 63.42498f, 71.89044f), 150.3918f, LocationType.CarDealer, "Benefactor/Gallivanter", "") { BannerImage = "benefactorgallivanter.png", Menu = BenefactorGallavanterMenu,
                CameraPosition = new Vector3(231.7523f, -993.08f, -97.99996f), CameraDirection = new Vector3(-0.9534805f, 0.1368595f, -0.2685973f), CameraRotation = new Rotator(-15.58081f, 0f, 81.83174f),
                ItemPreviewPosition = new Vector3(226.205f, -992.613f, -98.99996f), ItemPreviewHeading = 177.2006f,
                ItemDeliveryPosition = new Vector3(-83.40893f, 80.80059f, 71.08399f), ItemDeliveryHeading = 150.8571f},
            new GameLocation(new Vector3(-176.7741f, -1158.648f, 23.81366f), 359.6327f, LocationType.CarDealer, "Vapid", "") { BannerImage = "vapid.png",Menu = VapidMenu
                ,CameraPosition = new Vector3(231.7523f, -993.08f, -97.99996f), CameraDirection = new Vector3(-0.9534805f, 0.1368595f, -0.2685973f), CameraRotation = new Rotator(-15.58081f, 0f, 81.83174f),
                ItemPreviewPosition = new Vector3(226.205f, -992.613f, -98.99996f), ItemPreviewHeading = 177.2006f,
                ItemDeliveryPosition = new Vector3(-223.3041f, -1166.967f, 22.99067f), ItemDeliveryHeading = 347.7626f},
            new GameLocation(new Vector3(286.8117f, -1148.615f, 29.29189f), 0.5211872f, LocationType.CarDealer, "Sanders Motorcycles", "") { BannerImage = "sanders.png",Menu = SandersMenu
                ,CameraPosition = new Vector3(231.7523f, -993.08f, -97.99996f), CameraDirection = new Vector3(-0.9534805f, 0.1368595f, -0.2685973f), CameraRotation = new Rotator(-15.58081f, 0f, 81.83174f),
                ItemPreviewPosition = new Vector3(226.205f, -992.613f, -98.99996f), ItemPreviewHeading = 177.2006f,
                ItemDeliveryPosition = new Vector3(259.3008f, -1150.141f, 29.29169f), ItemDeliveryHeading = 358.0416f},
            new GameLocation(new Vector3(-247.2263f, 6213.266f, 31.93902f), 143.0866f, LocationType.CarDealer, "Helmut's European Autos", "") { BannerImage = "helmut.png",Menu = HelmutMenu
                ,CameraPosition = new Vector3(231.7523f, -993.08f, -97.99996f), CameraDirection = new Vector3(-0.9534805f, 0.1368595f, -0.2685973f), CameraRotation = new Rotator(-15.58081f, 0f, 81.83174f),
                ItemPreviewPosition = new Vector3(226.205f, -992.613f, -98.99996f), ItemPreviewHeading = 177.2006f,
                ItemDeliveryPosition = new Vector3(-214.548f, 6195.725f, 31.48873f), ItemDeliveryHeading = 314.937f},
            new GameLocation(new Vector3(-38.83289f, -1108.61f, 26.46652f), 158.283f, LocationType.CarDealer, "Premium Deluxe Motorsport", "") { Menu = PremiumDeluxeMenu
                ,CameraPosition = new Vector3(-46.13059f, -1103.091f, 27.9145f), CameraDirection = new Vector3(0.3461686f, 0.9154226f, -0.2053503f), CameraRotation = new Rotator(-11.85001f, -8.374705E-05f, -20.7142f),
                ItemPreviewPosition = new Vector3(-43.94203f, -1096.923f, 26.44f), ItemPreviewHeading = 165.1469f,
                ItemDeliveryPosition = new Vector3(-56.35966f, -1116.532f, 26.4349f), ItemDeliveryHeading = 2.403779f,InteriorID = 7170},
            new GameLocation(new Vector3(-802.8875f, -223.7307f, 37.21824f), 117.6851f, LocationType.CarDealer, "Luxury Autos", "") { Menu = LuxuryAutosMenu
                ,CameraPosition = new Vector3(231.7523f, -993.08f, -97.99996f), CameraDirection = new Vector3(-0.9534805f, 0.1368595f, -0.2685973f), CameraRotation = new Rotator(-15.58081f, 0f, 81.83174f),
                ItemPreviewPosition = new Vector3(226.205f, -992.613f, -98.99996f), ItemPreviewHeading = 177.2006f,
                ItemDeliveryPosition = new Vector3(-768.5347f, -245.1849f, 37.2452f), ItemDeliveryHeading = 197.8712f},
            new GameLocation(new Vector3(1224.667f, 2728.353f, 38.00491f), 181.2344f, LocationType.CarDealer, "Larry's RV Sales",""){ Menu = LarrysRVMenu
                ,CameraPosition = new Vector3(231.7523f, -993.08f, -97.99996f), CameraDirection = new Vector3(-0.9534805f, 0.1368595f, -0.2685973f), CameraRotation = new Rotator(-15.58081f, 0f, 81.83174f),
                ItemPreviewPosition = new Vector3(226.205f, -992.613f, -98.99996f), ItemPreviewHeading = 177.2006f,
                ItemDeliveryPosition = new Vector3(1236.887f, 2709.858f, 38.00579f), ItemDeliveryHeading = 201.5402f},

            //Gang Dens
            //new GameLocation(new Vector3(-223.1647f, -1601.309f, 34.88379f), 266.3889f, LocationType.GangDen, "The Families Den", "The OGs") { BannerImage = "families.png",OpenTime = 0,CloseTime = 24, Menu = FamiliesDenMenu, GangID = "AMBIENT_GANG_FAMILY", IsEnabled = false },
            //new GameLocation(new Vector3(86.11255f, -1959.272f, 21.12167f), 318.5057f, LocationType.GangDen, "Ballas Den", "") { BannerImage = "ballas.png",OpenTime = 0,CloseTime = 24, Menu = BallasDenMenu, GangID = "AMBIENT_GANG_BALLAS", IsEnabled = false  },
            //new GameLocation(new Vector3(967.6899f, -1867.115f, 31.44757f), 176.7243f, LocationType.GangDen, "Vagos Den", "") { BannerImage = "",OpenTime = 0,CloseTime = 24, Menu = VagosDenMenu, GangID = "AMBIENT_GANG_MEXICAN", IsEnabled = false  },
            //new GameLocation(new Vector3(1193.61f, -1656.411f, 43.02641f), 31.55427f, LocationType.GangDen, "Varrios Los Aztecas Den", "") { BannerImage = "varrios.png",OpenTime = 0,CloseTime = 24, Menu = VagosDenMenu, GangID = "AMBIENT_GANG_SALVA", IsEnabled = false  },
            //new GameLocation(new Vector3(1299.267f, -1752.92f, 53.88011f), 110.3803f, LocationType.GangDen, "Marabute Grande Den", "") { OpenTime = 0,CloseTime = 24, Menu = MarabunteDenMenu, GangID = "AMBIENT_GANG_MARABUNTE", IsEnabled = false  },
            //new GameLocation(new Vector3(-1144.041f, 4908.383f, 220.9688f), 33.69744f, LocationType.GangDen, "Altruist Cult Den", "") { BannerImage = "altruist.png",OpenTime = 0,CloseTime = 24, GangID = "AMBIENT_GANG_CULT", IsEnabled = false  },
            //new GameLocation(new Vector3(-766.3793f, -917.0612f, 21.29704f), 268.4079f, LocationType.GangDen, "Kkangpae Den", "") {  OpenTime = 0,CloseTime = 24, Menu = KkangpaeDenMenu, GangID = "AMBIENT_GANG_KKANGPAE", IsEnabled = false  },
            //new GameLocation(new Vector3(959.721f, 3618.905f, 32.67253f), 93.92658f, LocationType.GangDen, "Reckneck Den", "") { OpenTime = 0,CloseTime = 24, GangID = "AMBIENT_GANG_HILLBILLY", IsEnabled = false  },
            //new GameLocation(new Vector3(981.8542f, -103.0203f, 74.84874f), 220.3094f, LocationType.GangDen, "Lost M.C. Clubhouse", "") { BannerImage = "lostmc.png", OpenTime = 0,CloseTime = 24, Menu = LostDenMenu, GangID = "AMBIENT_GANG_LOST", IsEnabled = false },
            ////new GameLocation(new Vector3(514.9427f, 190.9465f, 104.745f), 356.6495f, LocationType.GangDen, "Gambetti Safehouse", "") { OpenTime = 0,CloseTime = 24, Menu = GambettiDenMenu, GangID = "AMBIENT_GANG_GAMBETTI", IsEnabled = false },
            //new GameLocation(new Vector3(101.6865f, -819.3801f, 31.31512f), 341.2845f, LocationType.GangDen, "Triad Den", "") { BannerImage = "triad.png", OpenTime = 0,CloseTime = 24, Menu = TriadsDenMenu, GangID = "AMBIENT_GANG_WEICHENG", IsEnabled = false },



        #if DEBUG

            //Bus Stop
            new GameLocation(new Vector3(307.3152f, -766.6166f, 29.24787f), 155.4713f, LocationType.BusStop, "PillBoxHospitalStop", ""),
            new GameLocation(new Vector3(355.6272f, -1064.027f, 28.86697f), 270.2965f, LocationType.BusStop, "LaMesaPoliceStop1", ""),

            ////Apartments
            //new GameLocation(new Vector3(-1150.072f, -1521.705f, 10.62806f), 225.8192f, LocationType.Apartment, "7611 Goma St", "") { OpenTime = 0,CloseTime = 24, InteriorID = 24578 },
            //new GameLocation(new Vector3(-1221.032f, -1232.806f, 11.02771f), 12.79515f, LocationType.Apartment, "Del Pierro Apartments", "") {OpenTime = 0,CloseTime = 24, InteriorID = -1, TeleportEnterPosition = new Vector3(266.1081f, -1007.534f, -101.0086f), TeleportEnterHeading = 358.3953f},
            
            ////House
            //new GameLocation(new Vector3(983.7811f, 2718.655f, 39.50342f), 175.7726f, LocationType.House, "345 Route 68", "") {OpenTime = 0,CloseTime = 24,IsPurchaseable = true },
            //new GameLocation(new Vector3(980.1745f, 2666.563f, 40.04342f), 1.699184f, LocationType.House, "140 Route 68", "") {OpenTime = 0,CloseTime = 24,IsPurchaseable = true },
            //new GameLocation(new Vector3(-3030.045f, 568.7484f, 7.823583f), 280.3508f, LocationType.House, "566 Ineseno Road", "") {OpenTime = 0,CloseTime = 24,IsPurchaseable = true},
            //new GameLocation(new Vector3(-3031.865f, 525.1087f, 7.424246f), 267.6694f, LocationType.House, "800 Ineseno Road", "") {OpenTime = 0,CloseTime = 24,IsPurchaseable = true },
            //new GameLocation(new Vector3(-3039.567f, 492.8512f, 6.772703f), 226.448f, LocationType.House, "805 Ineseno Road", "") {OpenTime = 0,CloseTime = 24,IsPurchaseable = true },
            //new GameLocation(new Vector3(195.0935f, 3031.064f, 43.89068f), 273.1078f, LocationType.House, "125 Joshua Road", "") {OpenTime = 0,CloseTime = 24,IsPurchaseable = true },
        #endif


        });


                /*prop_vend_snak_01.yft candy vending machine
        prop_vend_coffe_01.ydr coffee machine
        prop_vend_condom_01.ydr condom machine, but up on wall
        prop_vend_fags_01.ydr cigarette machine, but up on wall
        prop_vend_soda_01.yft eCola machine
        prop_vend_soda_02.yft sprunkmacine full
        prop_vend_water_01.yft water machine full raine
        prop_vend_fridge01.yft frigde with soda like at the ends of grocery store aisle
        prop_gumball_01.yft 103 , 3 is most like gumba;ll, 2 is like those m& m machines with 3 items*/



        /*            new GameLocation(new Vector3(2683.969f,3282.098f,55.24052f), 89.53969f, LocationType.GasStation, "24/7 Supermarket Grande Senora" ,"As fast as you"),//,new List<Vector3>() { new Vector3(2678.073f, 3265.522f, 54.7076f),new Vector3(2681.173f, 3262.774f, 54.70736f) }),
            new GameLocation(new Vector3(1725f, 6410f, 35f), 22.23846f, LocationType.GasStation, "24/7 Mount Chilliad (Gas)","As fast as you"),//,new List<Vector3>() { new Vector3(1706.173f, 6412.223f, 32.22713f), new Vector3(1701.657f, 6414.528f, 32.1186f), new Vector3(1697.71f, 6416.565f, 32.08189f),new Vector3(1706.173f, 6412.223f, 32.22713f)
                                                                                                                                                //, new Vector3(1697.869f,6420.53f,32.05283f), new Vector3(1701.852f,6418.417f,32.05503f), new Vector3(1705.852f,6416.659f,32.05479f) }),
            new GameLocation(new Vector3(-1429.33f,-270.8909f,46.2077f), 325.7301f, LocationType.GasStation, "Ron Morningwood","Good Morningwood!"),//,new List<Vector3>() { new Vector3(-1428.23f,-277.0434f,45.79089f), new Vector3(-1436.362f,-267.6647f,45.79237f),
                                                                                                                                                         // new Vector3(-1440.16f,-270.164f,45.79181f),new Vector3(-1431.339f,-280.2279f,45.79009f),
                                                                                                                                                         // new Vector3(-1434.153f,-282.5898f,45.79139f), new Vector3(-1442.416f,-273.0687f,45.7986f),
                                                                                                                                                         // new Vector3(-1446.231f,-276.2419f,45.80196f),new Vector3(-1437.798f,-285.5625f,45.77643f) }),
            new GameLocation(new Vector3(160.4977f,6635.249f,31.61175f), 70.88637f, LocationType.GasStation, "Dons Country Store & Gas","Country Manners!"),//,new List<Vector3>() { new Vector3(188.7231f,6607.43f,31.84954f), new Vector3(184.8491f,6606.112f,31.85245f), new Vector3(181.3225f,6605.81f,31.84829f)
                                                                                                                                               // , new Vector3(178.1896f,6604.389f,31.89782f), new Vector3(174.0998f,6604.168f,31.84834f), new Vector3(171.1875f,6603.454f,32.04737f) }),
            new GameLocation(new Vector3(266.2746f,2599.669f,44.7383f), 231.9223f, LocationType.GasStation, "Harmony General Store & Gas","Always in Harmony!"), //,new List<Vector3>() { new Vector3(262.5423f, 2610.143f, 44.3814f),new Vector3(265.0521f, 2604.807f, 44.38421f) }),

            new GameLocation(new Vector3(1039.753f,2666.26f,39.55253f), 143.6208f, LocationType.GasStation, "Grande Senora Cafe & Gas","Extra Grande!"),//,new List<Vector3>() { new Vector3(1043.293f,2677.189f,38.90083f), new Vector3(1035.182f,2677.444f,38.90271f), new Vector3(1034.835f,2670.396f,38.9343f)
                                                                                                                                                //, new Vector3(1043.761f,2670.147f,38.94082f), new Vector3(1043.412f,2672.185f,38.95105f), new Vector3(1034.338f,2672.41f,38.94936f) }),


            new GameLocation(new Vector3(-1817.871f,787.0063f,137.917f), 89.38248f, LocationType.GasStation, "LTD Richmond Glen",""),//,new List<Vector3>() { new Vector3(-1804.758f,792.6874f,138.5142f), new Vector3(-1809.721f,798.1087f,138.5137f),
                                                                                                                                                        //  new Vector3(-1807.576f,801.251f,138.5144f),new Vector3(-1802.39f,796.0137f,138.5141f),
                                                                                                                                                       //   new Vector3(-1798.391f,798.9479f,138.5154f), new Vector3(-1802.964f,805.196f,138.5681f),
                                                                                                                                                       //   new Vector3(-1800.726f,807.3672f,138.515f),new Vector3(-1796.539f,803.0259f,138.5148f),
                                                                                                                                                        //  new Vector3(-1792.259f,804.6542f,138.5133f),new Vector3(-1796.816f,810.0197f,138.5144f),
                                                                                                                                                       //   new Vector3(-1794.411f,813.2302f,138.5146f),new Vector3(-1789.586f,808.2133f,138.5163f)}),*/
    }
}


