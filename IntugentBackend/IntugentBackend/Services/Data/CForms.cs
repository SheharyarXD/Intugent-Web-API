using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{

    [Serializable]
    public class CForms
    {
        public int nComps = Params.nComps;
        public int nForm = Params.nFormMax;

        public ObservableCollection<CMaterial> POMats = new ObservableCollection<CMaterial>();     //List of materials in the PO side along with material proeprties
        public ObservableCollection<CMaterial> IsoMats = new ObservableCollection<CMaterial>();     //List of materials in the Iso side along with material proeprties
        public ObservableCollection<CMaterial> NCOIndexMats = new ObservableCollection<CMaterial>(); // ???
        public CForm[] FormAr = new CForm[Params.nFormMax];                                         // Each formulatin contains array of pbw of PO materials, Iso materials, index

        public DataTable dtFormIso;
        public DataTable dtFormPO;

    }
}
