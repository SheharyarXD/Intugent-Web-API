using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Admin
{
    public class AIModel
    {

        //        double[] y; 
       public DataTable dtNeurons = new DataTable();
       public DataTable dtWeigts = new DataTable();
        public AIModel() {
            //            nnModel.Reset();
            dtNeurons.Columns.Add("Layer #", typeof(int));
            dtNeurons.Columns.Add("Description", typeof(string));
            dtNeurons.Columns.Add("# of Neurons", typeof(int));

            dtWeigts.Columns.Add("Node Layer i", typeof(string));
            dtWeigts.Columns.Add("#0", typeof(string));

            //           CPages.PageModel_1.nnModel.nInputNeurons = CNNData.nInputNeurons;
        }

    }
}
