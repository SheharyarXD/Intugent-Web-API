using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;

namespace IntugentBackend
{
    public class ObjectsService
    {
        public int UserIndex { get; set; }

        // --- Core Services ---
        public Cbfile Cbfile { get; set; } = new();
        public CDefualts CDefualts { get; set; } = new();
        public CLists CLists { get; set; } = new();
        public MainWindow MainWindow { get; set; } = new();

        // --- Data Services ---
        public CProdTargets CProdTargets { get; set; }
        public CDBase CDBase { get; set; }
        public CNNData CNNData { get; set; }
        public CNNModel CNNModel { get; set; } = new();
        public CAnalysisData CAnalysisData { get; set; }
        public cMatrix cMatrix { get; set; } = new();
        public CAppParam CAppParam { get; set; } = new();
        public CForm CForm { get; set; } = new();
        public CForms CForms { get; set; } = new();
        public CJetMix CJetMix { get; set; } = new();
        public CMaterial CMaterial { get; set; } = new();
        public Params Params { get; set; } = new();
        public CRCalc CRCalc { get; set; } = new();
        public CRData CRData { get; set; } = new();
        public CUConv CUConv { get; set; } = new CUConv();
        public CIPProdTargets CIPProdTargets { get; set; }

        // --- Mfg & Rnd Services ---
        public MfgDimStability MfgDimStability { get; set; }
        public MfgFinishedGoods MfgFinishedGoods { get; set; }
        public MfgHome MfgHome { get; set; }
        public MfgInProcess MfgInProcess { get; set; }
        public MfgJetMixing MfgJetMixing { get; set; }
        public MfgPlantData MfgPlantData { get; set; }
        public MfgProcessCheck MfgProcessCheck { get; set; }
        public MfgReports MfgReports { get; set; }
        public MfgAdmin MfgAdmin { get; set; }
        public RNDHome RNDHome { get; set; }
        public RNDFormulations RNDFormulations { get; set; }
        public RNDProperties RNDProperties { get; set; } = new();
        public RNDRawProps RNDRawProps { get; set; } = new();
        public RNDRValues RNDRValues { get; set; }
        public RNDTDRV RNDTDRV { get; set; } = new();
        public AIModel AIModel { get; set; } = new();

        public ObjectsService()
        {
            // 1. Initialize dependencies first
            // Ensure Cbfile, CDefualts, CLists are available as they are common dependencies

            // 2. Initialize Mfg Services (Order based on dependency)
            MfgInProcess = new MfgInProcess(Cbfile);
            MfgAdmin = new MfgAdmin(Cbfile);
            MfgHome = new MfgHome(CDefualts, CLists, Cbfile);
            MfgFinishedGoods = new MfgFinishedGoods(Cbfile);
            MfgDimStability = new MfgDimStability(Cbfile);
            MfgPlantData = new MfgPlantData(Cbfile, CLists, CDefualts);
            MfgProcessCheck = new MfgProcessCheck(Cbfile, CDefualts);
            MfgReports = new MfgReports(Cbfile, CDefualts);
            MfgJetMixing = new MfgJetMixing(CLists);

            // 3. Initialize Data Services
            CProdTargets = new CProdTargets(Cbfile, CDefualts);
            CDBase = new CDBase(Cbfile);
            CNNData = new CNNData(Cbfile);
            CAnalysisData = new CAnalysisData(Cbfile, CDefualts);
            CIPProdTargets = new CIPProdTargets(Cbfile, MfgInProcess);

            // 4. Initialize Rnd Services
            RNDHome = new RNDHome(CDefualts, CLists, Cbfile);
            RNDRValues = new RNDRValues(CLists);
            RNDFormulations = new RNDFormulations(CDefualts, Cbfile, RNDHome);
        }

        public void InitializeSession(int userId)
        {
            this.UserIndex = userId;
        }
    }
}