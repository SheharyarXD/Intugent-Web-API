using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{
    [Serializable]
    public class CMaterial : INotifyPropertyChanged    //Material lists for user interactions
    {
        public int ID;
        public double GassToLiqWtRatio, MolWt, GassMolWt, GasBoilingTemp, GasCond_A, GasCond_B, GasVis_A, GasVis_B, GasVapPAtm_A, GasVapPAtm_B, GasVapPAtm_C;
        public string GasName;



        //               public double Pbw1 { get; set; }

        private string _Pbw1; public string Pbw1 { get { return _Pbw1; } set { _Pbw1 = value; RaisePropertyChanged(); } }
        private string _Pbw2; public string Pbw2 { get { return _Pbw2; } set { _Pbw2 = value; RaisePropertyChanged(); } }
        private string _Pbw3; public string Pbw3 { get { return _Pbw3; } set { _Pbw3 = value; RaisePropertyChanged(); } }
        private string _Pbw4; public string Pbw4 { get { return _Pbw4; } set { _Pbw4 = value; RaisePropertyChanged(); } }
        private string _Pbw5; public string Pbw5 { get { return _Pbw5; } set { _Pbw5 = value; RaisePropertyChanged(); } }
        private string _Pbw6; public string Pbw6 { get { return _Pbw6; } set { _Pbw6 = value; RaisePropertyChanged(); } }
        private string _Pbw7; public string Pbw7 { get { return _Pbw7; } set { _Pbw7 = value; RaisePropertyChanged(); } }
        private string _Pbw8; public string Pbw8 { get { return _Pbw8; } set { _Pbw8 = value; RaisePropertyChanged(); } }

        //, POpbw2, POpbw3, POpbw4, POpbw5;
        private string _MatName;
        public string MatName
        {
            get { return _MatName; }
            set
            {
                _MatName = value;
                RaisePropertyChanged();
            }
        }

        private string _MatType;
        public string MatType
        {
            get { return _MatType; }
            set
            {
                _MatType = value;
                RaisePropertyChanged();
            }
        }

        private double _MatOHNum;
        public double MatOHNum
        {
            get { return _MatOHNum; }
            set
            {
                _MatOHNum = value;
                RaisePropertyChanged();
            }
        }

        private double _MatNco;
        public double MatNco
        {
            get { return _MatNco; }
            set
            {
                _MatNco = value;
                RaisePropertyChanged();
            }
        }

        private double _MatFunc;    //Functionality
        public double MatFunc
        {
            get { return _MatFunc; }
            set
            {
                _MatFunc = value;
                RaisePropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
            //                      MessageBox.Show("xxx");
        }

    }
}
