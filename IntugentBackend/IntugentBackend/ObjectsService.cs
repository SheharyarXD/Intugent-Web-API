using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;

namespace IntugentBackend
{
    public class ObjectsService
    {
        public int UserIndex;
        // Core services
        public Cbfile Cbfile { get; set; }
        public CDefualts CDefualts { get; set; }
        public CLists CLists { get; set; }
        public MainWindow MainWindow { get; set; }

        // Data services
        public CProdTargets CProdTargets { get; set; }
        public CDBase CDBase { get; set; }
        public CNNData CNNData { get; set; }
        public CNNModel CNNModel { get; set; }
        public CAnalysisData CAnalysisData { get; set; }
        public cMatrix cMatrix { get; set; }
        public CAppParam CAppParam { get; set; }
        public CForm CForm { get; set; }
        public CForms CForms { get; set; }
        public CIPProdTargets CIPProdTargets { get; set; }
        public CJetMix CJetMix { get; set; }
        public CMaterial CMaterial { get; set; }
        public Params Params { get; set; }
        public CRCalc CRCalc { get; set; }
        public CRData CRData { get; set; }
        public CUConv CUConv { get; set; }



        // Mfg services
        public MfgDimStability MfgDimStability { get; set; }
        public MfgFinishedGoods MfgFinishedGoods { get; set; }
        public MfgHome MfgHome { get; set; }
        public MfgInProcess MfgInProcess { get; set; }
        public MfgJetMixing MfgJetMixing { get; set; }
        public MfgPlantData MfgPlantData { get; set; }
        public MfgProcessCheck MfgProcessCheck { get; set; }
        public MfgReports MfgReports { get; set; }

        // Rnd services
        public RNDFormulations RNDFormulations { get; set; }
        public RNDProperties RNDProperties { get; set; }
        public RNDRValues RNDRValues { get; set; }
        public RNDRawProps RNDRawProps { get; set; }
        public RNDTDRV RNDTDRV { get; set; }
        public RNDHome RNDHome { get; set; }

        // Admin services
        public AIModel AIModel { get; set; }
        public MfgAdmin MfgAdmin { get; set; }

        public ObjectsService()
        {
            // Core services - initialize first (no dependencies)
            Cbfile = new Cbfile();
            CDefualts = new CDefualts();
            CLists = new CLists();
            MainWindow = new MainWindow();

            // Data services - depend on Core
            CProdTargets = new CProdTargets(Cbfile, CDefualts);
            CDBase = new CDBase(Cbfile);
            CNNData = new CNNData(Cbfile);
            CNNModel = new CNNModel();
            CAnalysisData = new CAnalysisData(Cbfile, CDefualts);
            cMatrix = new cMatrix();
            CAppParam = new CAppParam();
            CForm = new CForm();
            CForms = new CForms();
            CJetMix = new CJetMix();
            CMaterial = new CMaterial();
            Params = new Params();
            CRCalc = new CRCalc();
            CRData = new CRData();
            CUConv = new CUConv();


            // Admin services
            AIModel = new AIModel();
            MfgAdmin = new MfgAdmin(Cbfile);
            // Mfg services
            MfgHome = new MfgHome(CDefualts, CLists, Cbfile);
            MfgFinishedGoods = new MfgFinishedGoods(Cbfile);
            MfgDimStability = new MfgDimStability (Cbfile);
         //  MfgPlantData = new MfgPlantData(Cbfile, CLists, CDefualts);
          // MfgInProcess = new MfgInProcess(Cbfile);
            MfgProcessCheck = new MfgProcessCheck(Cbfile, CDefualts);
            MfgReports = new MfgReports(Cbfile, CDefualts);
          // MfgJetMixing = new MfgJetMixing(CLists);

            // Rnd services - RNDHome needed before RNDFormulations
            RNDHome = new RNDHome(CDefualts, CLists, Cbfile);
            RNDProperties = new RNDProperties();
            RNDRawProps = new RNDRawProps();
            RNDTDRV = new RNDTDRV();
            // RNDRValues = new RNDRValues(CLists);
            // RNDFormulations = new RNDFormulations(CDefualts, Cbfile, RNDHome);
        }
    }
}
