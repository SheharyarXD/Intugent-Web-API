using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;

namespace IntugentBackend.Services.Rnd
{
    public class RNDProperties
    {
        //public CPropData PropData = new CPropData();
        public DataTable dtReacE = new DataTable();
        public DataTable dtPhotoE = new DataTable();
        public DataTable dtReacC = new DataTable();
        public DataTable dtPhotoC = new DataTable();
        public DataTable dtPropsE = new DataTable();
        public DataTable dtPropsC = new DataTable();
        public DataTable dtDataFiles = new DataTable();
        public DataTable dtComProd = new DataTable();
        public DataTable dtNotes = new DataTable();

        public int gDataFileRow = 0, gDataFileCol = 1;
        public RNDProperties()
        {

            dtReacE.Columns.Add("PropName", typeof(string));
            dtPhotoE.Columns.Add("PropName", typeof(string));
            dtPropsE.Columns.Add("PropName", typeof(string));
            dtDataFiles.Columns.Add("sDataFileType", typeof(string));
            dtComProd.Columns.Add("Form#", typeof(int));
            dtComProd.Columns.Add("sProdName", typeof(string));
            dtNotes.Columns.Add("Form#", typeof(int));
            dtNotes.Columns.Add("sNote", typeof(string));

            for (int i = 1; i < Params.nFormMax + 1; i++)
            {
                dtReacE.Columns.Add("PropE" + i, typeof(string));
                dtReacC.Columns.Add("PropC" + i, typeof(string));
                dtPhotoE.Columns.Add("PropE" + i, typeof(string));
                dtPhotoC.Columns.Add("PropC" + i, typeof(string));
                dtPropsE.Columns.Add("PropE" + i, typeof(string));
                dtPropsC.Columns.Add("PropC" + i, typeof(string));
                dtDataFiles.Columns.Add("sFile" + i, typeof(string));

                dtComProd.Rows.Add(); dtNotes.Rows.Add();
                dtComProd.Rows[i - 1]["Form#"] = dtNotes.Rows[i - 1]["Form#"] = i;
            }

            for (int i = 0; i < 10; i++)
            { dtReacE.Rows.Add(); dtReacC.Rows.Add(); }

            int ir = -1;
            dtReacE.Rows[ir + 1]["PropName"] = "   Mixing Time (sec)";
            dtReacE.Rows[ir + 2]["PropName"] = "   15% Height Time (sec)";
            dtReacE.Rows[ir + 3]["PropName"] = "   50% Height Time (sec)";
            dtReacE.Rows[ir + 4]["PropName"] = "   80% Height Time (sec)";
            dtReacE.Rows[ir + 5]["PropName"] = "   Cup Edge Time (sec)";
            dtReacE.Rows[ir + 6]["PropName"] = "   98% Height Time (sec)";
            dtReacE.Rows[ir + 7]["PropName"] = "   Max.Temp. Time (sec)";
            dtReacE.Rows[ir + 8]["PropName"] = "   Max.Temp. (deg F)";
            dtReacE.Rows[ir + 9]["PropName"] = "   Max Height (in)";
            dtReacE.Rows[ir + 10]["PropName"] = "   Sample Mass (gm)";


            for (int i = 0; i < 4; i++)
            { dtPhotoE.Rows.Add(); dtPhotoC.Rows.Add(); }

            ir = -1;
            dtPhotoE.Rows[ir + 1]["PropName"] = "   FTIR - PIR/ PUR";
            dtPhotoE.Rows[ir + 2]["PropName"] = "   Photoacoustic - Isocyanate";
            dtPhotoE.Rows[ir + 3]["PropName"] = "   Photoacoustic - Carbodiimide";
            dtPhotoE.Rows[ir + 4]["PropName"] = "   Photoacoustic - Trimer";

            for (int i = 0; i < 10; i++)
            { dtPropsE.Rows.Add(); dtPropsC.Rows.Add(); }

            ir = 0;
            dtPropsE.Rows[ir]["PropName"] = "Physical Properties";
            dtPropsE.Rows[ir + 1]["PropName"] = "   Compressives(psi)";
            dtPropsE.Rows[ir + 2]["PropName"] = "   % Closed Cells";
            dtPropsE.Rows[ir + 3]["PropName"] = "   Cell Size (micron)";
            dtPropsE.Rows[ir + 4]["PropName"] = "   Cell Count";
            dtPropsE.Rows[ir + 5]["PropName"] = "   Isotopy";
            dtPropsE.Rows[ir + 6]["PropName"] = "▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬";

            ir += 7;
            dtPropsE.Rows[ir]["PropName"] = "Physical Properties";
            dtPropsE.Rows[ir + 1]["PropName"] = "   Hot Plates - % Thickness";
            dtPropsE.Rows[ir + 2]["PropName"] = "   Hot Plates - % Mass";

            for (int i = 0; i < 3; i++) dtDataFiles.Rows.Add();
            ir = 0;
            dtDataFiles.Rows[ir]["sDataFileType"] = "FTIR Spectra";
            dtDataFiles.Rows[ir + 1]["sDataFileType"] = "TGA";
            dtDataFiles.Rows[ir + 2]["sDataFileType"] = "Foamat Data (.csv)";

            //gReacE.ItemsSource = dtReacE.DefaultView;
            //gReacC.ItemsSource = dtReacC.DefaultView;
            //gPhotoE.ItemsSource = dtPhotoE.DefaultView;
            //gPhotoC.ItemsSource = dtPhotoC.DefaultView;
            //gPropsE.ItemsSource = dtPropsE.DefaultView;
            //gPropsC.ItemsSource = dtPropsC.DefaultView;
            //gDataFiles.ItemsSource = dtDataFiles.DefaultView;
            //gProd.ItemsSource = dtComProd.DefaultView;
            //gNotes.ItemsSource = dtNotes.DefaultView;


            //GetPropList();


        }

    }
}
