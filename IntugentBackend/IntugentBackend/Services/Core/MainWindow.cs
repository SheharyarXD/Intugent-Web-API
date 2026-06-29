using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Data;
using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;

namespace IntugentBackend.Services.Core
{
    public class MainWindow
    {
        public string sServiceLink;

        public Cbfile cbfile;
        public CLists cLists;
        public CDefualts cDefualts;

        public (CDefualts, CLists, Cbfile) MainWindowConstructor(int IDLocation)
        {
            string sMsg;

            // ReadConfigFile();

            /* CTelClient.SetClient();
             CTelClient.TelTrace("Config. file read");  //Azue Insight Trace Message
             CTelClient.TelTrace("User " + Environment.UserName + " on computer " + Environment.MachineName);  //Azue Insight Trace Message
 */


            //                        InitializePages();
            // gPageName.Text = "Intugent PI Login Page - Use Standard Industries' Login Page to login";
            // CStatusBar.sHelpPage = "PI_Introduction.html";
            // CStatusBar.SetText("Welcome to PI:  Please Use Standard Industries' Login Page to login");
            // CPages.PageOktaAuth_1 = new Page1OktaAuth();
            // MainFrame.Navigate(CPages.PageOktaAuth_1);
            //           MainFrame.Content = CPages.PageOktaAuth_1;
            //  MainFrame.NavigationService.Navigate(CPages.PageOktaAuth_1);

            //  CTelClient.TelTrace("App. & Okta authorization page Initialized");  //Azure Insight Trace Message

            //check for the local user.  If not local user, get Azure secrets and Okta authentication
            cbfile = new Cbfile();
            cLists = new CLists();
            cDefualts = new CDefualts();
            cDefualts.sEmployee = Environment.UserName;

            // TRYING TO RUN LOCALLY....!!!!!!!!!!!!!!
            cDefualts.sEmployee = "AShafi";


            cDefualts.sLocation = string.Empty;

            if (Environment.UserName == CLocalData.User && Environment.UserDomainName == CLocalData.Machine) cbfile.bConAz = false; else cbfile.bConAz = true;
            if (cbfile.bConAz == false) cDefualts.sEmployee = Environment.UserName;

            /*            else            //Virtual machine on GAF network
                        {
                            if (!GetAzureSecrets()) { this.Close(); return; }
                        }
            */
            //Connect to database and check for the user in the database
            //           MessageBox.Show("Feedback from Okta\n\n" + okta1.sMsg);
            try
            {
                if (!cbfile.bConAz) //MANUALLY ALTERING CONDITION............!!!!!!!!!!!!!!!
                {
                    //                    CTelClient.TelTrace("Azure secrets obtained");  //Azue Insight Trace Message

                    //                   CAzure.Msi = " ";
                    /*                   CAzure.Msi = "e05378c1-8df3-4b0c-aef8-9bb8a5033484";

                                                        CAzure.DB_Url = "https://database.windows.net/.default";
                                                        CAzure.Db_ConStr = "Server=tcp:dmt-uat-use-dbm-shared.database.windows.net;Database=Intugent-uat-use-db;TrustServerCertificate=True;";
                                     */
                    //                   MessageBox.Show("Welcome " + CDefualts.sEmployee + "\n\n Intugent PI will be connecting to Azure Database", cbfile.sAppName);
                    //   var credential = new Azure.Identity.DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = CAzure.Msi });
                    //   var token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { CAzure.DB_Url }));
                    //   cbfile.conAZ = new Microsoft.Data.SqlClient.SqlConnection(CAzure.Db_ConStr);
                    //    cbfile.conAZ.AccessToken = token.Token;
                    //    CTelClient.TelTrace("Azure sql conn. made");  //Azue Insight Trace Message
                }
                else
                {
                    //   MessageBox.Show("Welcome " + CDefualts.sEmployee + "\n\n Intugent PI will be connecting to Local Database", cbfile.sAppName);
                    cbfile.conAZ = new SqlConnection(ObjectsService.ConnectionString);

                    MainWindow_Rendered(IDLocation);


                }


                return (cDefualts, cLists, cbfile);
            }
            catch (Exception ex)
            {
                sMsg = "Could not connect to the server.";
                //  MessageBox.Show(sMsg, cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError("Initial connection to AzureSQL\n\n" + ex.Message);
                //  CTelClient.TelExceptionFlush(ex, sMsg);  //Azue Insight Trace Message
                // this.Close(); return;
                return (null, null, null);
            }



        }


        //      private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        public void MainWindow_Rendered(int IDLocation)
        {

            string sMsg, stmp;
            if (!cbfile.bConAz || cDefualts.sEmployee == " AShafi")
                if (!cbfile.bConAz)
                { //MessageBox.Show("Can not link to the database", cbfile.sAppName); return;
                }
            if (cDefualts.sEmployee == "AShafi" || cDefualts.sEmployee == "Asjad")
            {
                cDefualts.sEmployee = "Asjad.Shafi@gaf.com";
                if (IDLocation == 1)
                {
                    cDefualts.sGroup = "OKTA_GAF_Intugent_RnD_Users";
                    //cDefualts.sLocation = "Global R&D"; cDefualts.IDLocation = 3;
                }
                else if (IDLocation == 2)
                {
                    cDefualts.sGroup = "OKTA_GAF_Intugent_Mfg_Users_Gainesville";
                    //cDefualts.sLocation = "Gainsville TX"; cDefualts.IDLocation = 1;
                }
                else if (IDLocation == 3)
                {
                    cDefualts.sGroup = "OKTA_GAF_Intugent_Mfg_Users_Statesboro";
                    //cDefualts.sLocation = "Statesboro, GA"; cDefualts.IDLocation = 5;
                }
                else if (IDLocation == 4)
                {
                    cDefualts.sGroup = "OKTA_GAF_Intugent_Mfg_Users_CedarCity";
                    //cDefualts.sLocation = "Cedar City"; cDefualts.IDLocation = 2;
                }
                else if (IDLocation == 5)
                {
                    cDefualts.sGroup = "OKTA_GAF_Intugent_Mfg_Users_NewColumbia";
                    //cDefualts.sLocation = "New COlumbia, PA"; cDefualts.IDLocation = 6;
                }
                else if (IDLocation == 6)
                {
                    cDefualts.sGroup = "OKTA_GAF_Intugent_Admins";
                    //cDefualts.sLocation = "Admin"; cDefualts.IDLocation = 4;
                }
                /*                CPages.PageOktaAuth_1.gGroup.Items.Add("OKTA_GAF_Intugent_RnD_Users");
                                CPages.PageOktaAuth_1.gGroup.Items.Add("OKTA_GAF_Intugent_Mfg_Users_Gainesville");
                                CPages.PageOktaAuth_1.gGroup.Items.Add("OKTA_GAF_Intugent_Mfg_Users_Statesboro");
                                CPages.PageOktaAuth_1.gGroup.Items.Add("OKTA_GAF_Intugent_Mfg_Users_CedarCity");
                                CPages.PageOktaAuth_1.gGroup.Items.Add("OKTA_GAF_Intugent_Mfg_Users_NewColumbia");
                                CPages.PageOktaAuth_1.gGroup.Items.Add("OKTA_GAF_Intugent_Admins");

                                CPages.PageOktaAuth_1.gGroupBox.Visibility = System.Windows.Visibility.Visible;
                                CPages.PageOktaAuth_1.gwbs.Visibility = System.Windows.Visibility.Hidden;*/

                // MANUALLY ADDING GROUP
                //cDefualts.sGroup = "OKTA_GAF_Intugent_Mfg_Users_Gainesville";
                ActivatePI();

                return;
                //                ActivatePI();
            }


            //                       { CDefualts.sEmployee = string.Empty; CDefualts.sGroup = string.Empty; okta1.GetOktaData1Async(); }





            bool bUserChecked = true;
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            if (!windowsIdentity.IsAuthenticated)
            { //MessageBox.Show("Your account could not be authenticated"); bUserChecked = false;
            }
            string sDomain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            if (Environment.UserDomainName != cbfile.sDomain || sDomain != cbfile.sDomain2)
            {// MessageBox.Show("Your account could not be authenticated"); bUserChecked = false;
            }

            //           MessageBox.Show(windowsIdentity.AuthenticationType.ToString());
            stmp = windowsIdentity.Name;
            if (!stmp.Contains(cbfile.sDomain))
            { //MessageBox.Show("Your account could not be authenticated"); bUserChecked = false;
            }
            //           MessageBox.Show(Environment.UserName + "\n\n" + Environment.UserDomainName);
            //           sDomain = windowsIdentity.User.AccountDomainSid.Translate(typeof(NTAccount)).Value;
            //           MessageBox.Show(sDomain);
            //           sDomain = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().ToString();
            //           MessageBox.Show(sDomain);

            //            CDefualts.sEmployee = stmp.Substring(cbfile.sDomain.Length+1);
            if (!bUserChecked) return;

            string sql = "select [Group] from tblLocations";
            SqlDataAdapter da = new SqlDataAdapter(sql, cbfile.conAZ);
            int itmp = da.Fill(cLists.dtUserGroups);

            /*            IdentityReferenceCollection irc = windowsIdentity.Groups;
                        bool bGroup;
                        foreach (IdentityReference ir in irc)
                        {
                            bGroup = false;
                            stmp = ir.Translate(typeof(NTAccount)).Value;
                            if (stmp.Contains(cbfile.sDomain)) stmp = stmp.Substring(cbfile.sDomain.Length + 1);
                            foreach (DataRow row in CLists.dtUserGroups.Rows) if ((string)row["Group"] == stmp) bGroup = true;
                            if (bGroup)
                            { CPages.PageOktaAuth_1.gGroup.Items.Add(stmp); }
                        }
                        if (CPages.PageOktaAuth_1.gGroup.Items.Count < 1) { MessageBox.Show("Your account could not be authenticated"); return; }
                        else if (CPages.PageOktaAuth_1.gGroup.Items.Count == 1)
                        {
                            CDefualts.sEmployee = Environment.UserName;
                            CDefualts.sGroup = CPages.PageOktaAuth_1.gGroup.Items[0].ToString();
                            CStatusBar.SetText("Welcome " + CDefualts.sEmployee); ;
                            ActivatePI();
                        }
                        else
                        {
                            CDefualts.sEmployee = Environment.UserName;
                            CPages.PageOktaAuth_1.gGroupBox.Visibility = System.Windows.Visibility.Visible;
                            CPages.PageOktaAuth_1.gwbs.Visibility = System.Windows.Visibility.Hidden;
                        }*/

        }


        public void ActivatePI()
        {
            //               this.Activate();

            //  if (!GetUserInfo()) { this.Close(); return; }
            // CTelClient.TelTrace("User information retrieved from the database");  //Azue Insight Trace Message

            //  InitializePages();
            //            CPages.PageRecipe_1.Startup();
            //           CPages.PageRndHome_1.Startup();
            //           CPages.PageRValues_1.Startup();
            //           CPages.PageMfgHome_1.Startup();
            //           CPages.PageJetMixing_1.Startup();

            //    CTelClient.TelTrace("All pages initialized");  //Azue Insight Trace Message


            if (cDefualts.IDLocation == 3)
            {
                /*
                                gPageName.Text = "R&D Home Page";
                                CStatusBar.sHelpPage = "RndHomePage.html";
                                MainFrame.NavigationService.Navigate(CPages.PageRndHome_1);
                                gRND.IsSelected = true;*/

            }
            else if (cDefualts.IDLocation == 4)
            {
                /*

                                gRAd.Visibility = Visibility.Visible;
                                gMAd.Visibility = Visibility.Visible;
                                gsAIHome.Visibility = Visibility.Visible;
                                gsAIData.Visibility = Visibility.Visible; gsAIData.IsEnabled = false;
                                gsAIModel.Visibility = Visibility.Visible; gsAIModel.IsEnabled = false;


                                gPageName.Text = "Mfg. Home Page";
                                CStatusBar.sHelpPage = "MfgAdmin.html";
                                MainFrame.NavigationService.Navigate(CPages.PageAdminMfg1_1);
                                gMAd.IsSelected = true;*/
            }
            else
            {
                /*
                                gMFG.Visibility = Visibility.Visible;
                                gMIN.Visibility = Visibility.Visible;
                                gMFI.Visibility = Visibility.Visible;
                                gMDS.Visibility = Visibility.Visible;
                                gMPL.Visibility = Visibility.Visible;
                                gCJM.Visibility = Visibility.Visible;
                                gMPC.Visibility = Visibility.Visible;
                                gMRP.Visibility = Visibility.Visible;
                                gAN1.Visibility = Visibility.Visible;
                                gAN2.Visibility = Visibility.Visible;

                                gPageName.Text = "Manufacturing Home Page";
                                CStatusBar.sHelpPage = "MfgHomePage.html";
                                MainFrame.NavigationService.Navigate(CPages.PageMfgHome_1);
                                gMFG.IsSelected = true;*/

            }

            GetUserInfo();
            GetSqlLists();
            // CTelClient.TelTrace("Lists for listboxes obtained");  //Azue Insight Trace Message

            //SetOptionBoxes();

            //            CTelClient.TelTrace("Listboxes uploaded");  //Azue Insight Trace Message

            //            CStatusBar.SetText("Welcome " + CDefualts.sEmployee); ;

        }


        /* public void InitializePages()
         {

             CPages.PageRndHome_1 = new PageRndHome();
             CTelClient.TelTrace("RND Home page initialized");  //Azue Insight Trace Message
             CPages.PageRecipe_1 = new PageRecipe(); CTelClient.TelTrace("RND Formulation page initialized");  //Azue Insight Trace Message
             CPages.PageRawProps_1 = new PageRawProps(); CTelClient.TelTrace("RND Raw Prop page initialized");  //Azue Insight Trace Message
             CPages.PageProperties_1 = new PageProperties(); CTelClient.TelTrace("RND Properties page initialized");  //Azue Insight Trace Message
             CPages.PageTDRV_1 = new PageTDRV(); CTelClient.TelTrace("RND TDRV page initialized");  //Azue Insight Trace Message
             CPages.PageRValues_1 = new PageRValues(); CTelClient.TelTrace("RND RValue page initialized");
             CPages.PageRValuesAged_1 = new PageRValuesAged(); CTelClient.TelTrace("7 page initialized");
             CPages.PageFoamKinetics_1 = new PageFoamKinetics(); CTelClient.TelTrace("8 page initialized");
             CPages.PageInProcess_1 = new PageInProcess(); CTelClient.TelTrace("10 page initialized");
             CPages.PageFinishedGoods_1 = new PageFinishedGoods(); CTelClient.TelTrace("11 page initialized");
             CPages.PageDimStability_1 = new PageDimStability(); CTelClient.TelTrace("13 page initialized");
             CPages.PagePlantData_1 = new PagePlantData(); CTelClient.TelTrace("14 page initialized");
             CPages.PageMfgHome_1 = new PageMfgHome(); CTelClient.TelTrace("9 page initialized");  //must be after in process, finishedgoods, dimstability and plant data pages as it refers to these pages

             CPages.PageJetMixing_1 = new PageJetMixing(); CTelClient.TelTrace("15 page initialized");
             CPages.PageAnalysis1_1 = new PageAnalysis1(); CTelClient.TelTrace("16 page initialized");
             CPages.PageAnalysis2_1 = new PageAnalysis2(); CTelClient.TelTrace("17 page initialized");
             CPages.PageAnalysis3_1 = new PageAnalysis3(); CTelClient.TelTrace("18 page initialized");
             CPages.PageProcessCheck_1 = new PageProcessCheck1(); CTelClient.TelTrace("18 page initialized");
             CPages.PageMfgReports_1 = new PageMfgReports(); CTelClient.TelTrace("19 page initialized");
             CPages.PageAdminMfg1_1 = new PageAdminMfg1(); CTelClient.TelTrace("20 page initialized");
             CPages.PageAdminRND1_1 = new PageAdminRND1(); CTelClient.TelTrace("21 page initialized");

             CPages.PageAIHome_1 = new PageAIHome();
             CPages.PageData_1 = new PageData();
             CPages.PageModel_1 = new PageModel();

             //            CPages.PageOktaAuth_1 = new Page1OktaAuth();

         }
 */


        /*        private void tc_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                    ((TabItem)(tc.SelectedItem)).IsSelected = true;
                    //            MessageBox.Show(((TabItem)(tc.SelectedItem)).Name.ToString());

                    sActiveTab = ((TabItem)(tc.SelectedItem)).Name.ToString();
                    SwitchPages(sActiveTab);

                }
        */
        /*private void SwitchPages(string sPageName)
        {
            //           CStatusBar.SetText("");

            switch (sPageName)
            {
                case "gRND":
                    MainFrame.NavigationService.Navigate(CPages.PageRndHome_1);
                    gPageName.Text = "R&D Home Page";
                    CStatusBar.sHelpPage = "RndHomePage.html";
                    break;
                case "gFRE":
                    MainFrame.NavigationService.Navigate(CPages.PageRecipe_1);
                    gPageName.Text = "Formulation Recipe";
                    CStatusBar.sHelpPage = "FormulationInfo.html";
                    break;
                case "gRPR":
                    MainFrame.NavigationService.Navigate(CPages.PageRawProps_1);
                    gPageName.Text = "Raw Lab Data to Calculate Properties";
                    CStatusBar.sHelpPage = "RDRawProps.html";
                    break;
                case "gFPR":
                    MainFrame.NavigationService.Navigate(CPages.PageProperties_1);
                    gPageName.Text = "Formulation Properties and Data Files";
                    CStatusBar.sHelpPage = "RDPropsDataFiles.html";
                    break;
                case "gTDR":
                    MainFrame.NavigationService.Navigate(CPages.PageTDRV_1);
                    gPageName.Text = "TDRV - Temperature Dependent R Values";
                    CStatusBar.sHelpPage = "TDRV.html";
                    break;
                case "gFRV":
                    MainFrame.NavigationService.Navigate(CPages.PageRValues_1);
                    gPageName.Text = "Initial R Values as a Function of Bubble Size and Density";
                    CStatusBar.sHelpPage = "InitialRValues.html";
                    break;
                case "gFRA":
                    MainFrame.NavigationService.Navigate(CPages.PageRValuesAged_1);
                    //  MainFrame.NavigationService.Navigate(CPages.PageRValues_1);
                    gPageName.Text = "Aged R Values as a Function of Time";
                    CStatusBar.sHelpPage = "TDRV.html";
                    break;
                case "gFKI":
                    MainFrame.NavigationService.Navigate(CPages.PageFoamKinetics_1);
                    gPageName.Text = "Dynamics of reactions and foam expansion";
                    CStatusBar.sHelpPage = "RxnKinetics.htm";
                    break;
                case "gMFG":
                    MainFrame.NavigationService.Navigate(CPages.PageMfgHome_1);
                    gPageName.Text = "Manufacturing Home Page";
                    CStatusBar.sHelpPage = "MfgHomePage.html";
                    break;
                case "gMIN":
                    MainFrame.NavigationService.Navigate(CPages.PageInProcess_1);
                    gPageName.Text = "In Process Board Properties";
                    CStatusBar.sHelpPage = "InProcess.html";
                    break;
                case "gMFI":
                    MainFrame.NavigationService.Navigate(CPages.PageFinishedGoods_1);
                    gPageName.Text = "Finished Board Properties";
                    CStatusBar.sHelpPage = "FinshedGoods.html";
                    break;
                case "gMDS":
                    MainFrame.NavigationService.Navigate(CPages.PageDimStability_1);
                    gPageName.Text = "Post Expansion Dimensional Stability Data";
                    CStatusBar.sHelpPage = "DimStability.html";
                    break;
                case "gMPL":
                    MainFrame.NavigationService.Navigate(CPages.PagePlantData_1);
                    gPageName.Text = "Relevant Process Data";
                    CStatusBar.sHelpPage = "ProcessData.html";
                    break;
                case "gCJM":
                    MainFrame.NavigationService.Navigate(CPages.PageJetMixing_1);
                    gPageName.Text = "Evaluate mixing of Poly and Iso Jets";
                    CStatusBar.sHelpPage = "JetMixing.html";
                    break;
                case "gMPC":
                    MainFrame.NavigationService.Navigate(CPages.PageProcessCheck_1);
                    gPageName.Text = "In Process Check Sheet";
                    //                   CStatusBar.sHelpPage = "Analysis3.htm";
                    break;
                case "gMRP":
                    MainFrame.NavigationService.Navigate(CPages.PageMfgReports_1);
                    gPageName.Text = "Mfg Reports";
                    //                   CStatusBar.sHelpPage = "Analysis3.htm";
                    break;
                case "gAN1":
                    MainFrame.NavigationService.Navigate(CPages.PageAnalysis1_1);
                    gPageName.Text = "Analysis 1";
                    CStatusBar.sHelpPage = "Analysis1.htm";
                    break;
                case "gAN2":
                    MainFrame.NavigationService.Navigate(CPages.PageAnalysis2_1);
                    gPageName.Text = "Analysis 2";
                    CStatusBar.sHelpPage = "Analysis2.htm";
                    break;
                case "gAN3":
                    MainFrame.NavigationService.Navigate(CPages.PageAnalysis3_1);
                    gPageName.Text = "Analysis 3";
                    CStatusBar.sHelpPage = "Analysis3.htm";
                    break;
                case "gRAd":
                    MainFrame.NavigationService.Navigate(CPages.PageAdminRND1_1);
                    gPageName.Text = "R&D Admin";
                    CStatusBar.sHelpPage = "Admin Page for R&D";
                    break;
                case "gMAd":
                    MainFrame.NavigationService.Navigate(CPages.PageAdminMfg1_1);
                    gPageName.Text = "Mfg. Admin";
                    CStatusBar.sHelpPage = "Admin Page for Mfg";
                    break;

                case "gsAIHome":
                    MainFrame.NavigationService.Navigate(CPages.PageAIHome_1);
                    gPageName.Text = "sAI Home Page - Select a Model to work on";
                    CStatusBar.sHelpPage = "sAIHomePage.html";
                    break;
                case "gsAIData":
                    MainFrame.NavigationService.Navigate(CPages.PageData_1);
                    gPageName.Text = "Training Data";
                    CStatusBar.sHelpPage = "Data";
                    break;
                case "gsAIModel":
                    MainFrame.NavigationService.Navigate(CPages.PageModel_1);
                    gPageName.Text = "sAI Model - Train the Model";
                    CStatusBar.sHelpPage = "Model.html";
                    break;
                default:
                    break;

            }
        }*/



        public CLists GetSqlLists()
        {
            int itmp;
            string sql, sMsg;
            SqlDataAdapter da;
            DataRow dr;

            try
            {
                sql = "select * from Roster " +
                 "Left join tblLocations on tblLocations.ID = Roster.IDLocation " +
                 " where Employees = '" + cDefualts.sEmployee + "'";
                //                " where Employees = 'Harrison Maxwell'";
                da = new SqlDataAdapter(sql, cbfile.conAZ);

                itmp = da.Fill(cLists.dtLoc);
                if (itmp > 0)
                {
                    //                   CDefualts.sLocation = CLists.dtLoc.Rows[0]["sLocation"].ToString();
                    //                  CDefualts.IDLocation = (int)CLists.dtLoc.Rows[0]["IDLocation"];
                    //                   CDefualts.IDEmployee = (int)CLists.dtLoc.Rows[0]["ID"];

                    if (cLists.dtLoc.Rows[0]["sPourHead1"] == DBNull.Value) cDefualts.sLocMfg1 = "Loc1"; else cDefualts.sLocMfg1 = (string)cLists.dtLoc.Rows[0]["sPourHead1"];
                    if (cLists.dtLoc.Rows[0]["sPourHead2"] == DBNull.Value) cDefualts.sLocMfg2 = "Loc1"; else cDefualts.sLocMfg2 = (string)cLists.dtLoc.Rows[0]["sPourHead2"];
                    if (cLists.dtLoc.Rows[0]["sPourHead3"] == DBNull.Value) cDefualts.sLocMfg3 = "Loc1"; else cDefualts.sLocMfg3 = (string)cLists.dtLoc.Rows[0]["sPourHead3"];
                }

                sql = "select [Group] from tblLocations";
                da = new SqlDataAdapter(sql, cbfile.conAZ);
                itmp = da.Fill(cLists.dtUserGroups);

                cLists.dtLoc.Clear();
                sql = "select * from Roster  order by Employees";
                da = new SqlDataAdapter(sql, cbfile.conAZ);
                itmp = da.Fill(cLists.dtLoc);

                sql = "Select    [Product Code],  [Product Code]+' - ' +  [Product Description] as Product  from [Product Matrix] Order by [Product Code] Asc";
                if (cDefualts.IDLocation != 3 && cDefualts.IDLocation != 4)
                {
                    sql = "Select    [Product Code],  [Product Code]+' - ' +  [Product Description] as Product  from [Product Matrix] Where IDLoc = " + cDefualts.IDLocation.ToString() + " Order by [Product Code] Asc";
                } //Filter products for Mfg group
                da = new SqlDataAdapter(sql, cbfile.conAZ);
                itmp = da.Fill(cLists.dtComProd);

                sql = "SELECT * FROM [tblLists] Order by [iCode] Asc";
                da = new SqlDataAdapter(sql, cbfile.conAZ);
                itmp = da.Fill(cLists.dtLists);
                /* moved to plant data page.
                                sql = "SELECT * FROM [tblProcessParams]";
                                da = new SqlDataAdapter(sql, cbfile.conAZ);
                                itmp = da.Fill(CLists.dtProcessParams);
                */

                return cLists;
            }
            catch (SqlException ex)
            {
                sMsg = "Erros while obtaining the listbox choices from the database";
                //MessageBox.Show(sMsg, cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //CTelClient.TelException(ex, sMsg);
                return null;
            }
            finally { }
        }

        /*    public static void SetOptionBoxes()
            {
                DataRow dr;
                var bValid = true;
                int indx;

                cLists.dvComProd = (cLists.dtComProd.DefaultView).ToTable().DefaultView; //Make a copy
                dr = cLists.dtComProd.NewRow();
                dr["Product Code"] = CDefualts.sProdMfgAll;
                dr["Product"] = CDefualts.sProdMfgAll;
                cLists.dtComProd.Rows.InsertAt(dr, 0);
                CLists.dvComProdAll = CLists.dtComProd.DefaultView;

                #region Mfg specific lists
                if (CDefualts.IDLocation != 3)
                {
                    /*MfgHome.location = CDefualts.sLocation;
                    MfgHome.dvComProd = CLists.dvComProdAll;*//*

                    //new MfgHome();


                    CPages.PageProcessCheck_1.gProdID.ItemsSource = CLists.dvComProd;
                    CPages.PageProcessCheck_1.gProdID.SelectedValue = CDefualts.sProdMfg;

                    CPages.PageProcessCheck_1.gOperator.ItemsSource = CLists.dvEmployeesMfg;
                    CPages.PageProcessCheck_1.gOperator.SelectedValue = CDefualts.IDEmployee;

                    CPages.PageProcessCheck_1.gType.ItemsSource = CLists.dvPCType; 
                    CPages.PageProcessCheck_1.gType.SelectedIndex = 0;

                    
                    CPages.PageProcessCheck_1.gTopBoardPrint.ItemsSource = CLists.dvPCTopBoard; 
                    CPages.PageProcessCheck_1.gTopBoardPrint.SelectedIndex = 0;
                    
                    CPages.PageProcessCheck_1.gBottomBoardPrint.ItemsSource = CLists.dvPCBottomBoard;
                    CPages.PageProcessCheck_1.gBottomBoardPrint.SelectedIndex = 0;
                    
                    CPages.PageProcessCheck_1.gPerferation.ItemsSource = CLists.dvPCPerferation; 
                    CPages.PageProcessCheck_1.gPerferation.SelectedIndex = 0;
                    
                    CPages.PageProcessCheck_1.gFlipper.ItemsSource = CLists.dvPCFlipper; 
                    CPages.PageProcessCheck_1.gFlipper.SelectedIndex = 0;

                    
                    CPages.PageProcessCheck_1.gAdhesion.ItemsSource = CLists.dvPCFacerAdh; 
                    CPages.PageProcessCheck_1.gAdhesion.SelectedIndex = 0;
                    
                    CPages.PageProcessCheck_1.gEdgeCut.ItemsSource = CLists.dvPCEdgeCut; 
                    CPages.PageProcessCheck_1.gEdgeCut.SelectedIndex = 0;
                    
                    CPages.PageProcessCheck_1.gHooder.ItemsSource = CLists.dvPCHooderQuality;
                    CPages.PageProcessCheck_1.gHooder.SelectedIndex = 0;
                    
                    CPages.PageProcessCheck_1.gBoardQuality.ItemsSource = CLists.dvPCBoardQuality; 
                    CPages.PageProcessCheck_1.gBoardQuality.SelectedIndex = 0;
                    CPages.PageProcessCheck_1.gProdID.ItemsSource = CLists.dvComProd; 
                    CPages.PageProcessCheck_1.gProdID.SelectedIndex = 0;*//*

                }
                #endregion

                #region RND specific Lists

                if (CDefualts.IDLocation == 3 || CDefualts.IDLocation == 4)
                {
                    *//*CPages.PageRndHome_1.gLoc1.Text = CDefualts.sLocation;

                    CPages.PageRndHome_1.gProd1.ItemsSource = CLists.dvComProdAll;
                    CPages.PageRecipe_1.gProductID.ItemsSource = CLists.dvComProd;
                    CPages.PageRecipe_1.gProductID.SelectedValue = "Experimental";
                    CPages.PageProperties_1.gProdList.ItemsSource = CLists.dvComProd;
                    //            CPages.PageProperties_1.gProdList.value = "Experimental";
                    CLists.dvLists = CLists.dtLists.DefaultView;
                    CLists.dvLists.RowFilter = "sList = 'Testing Status RND'";  //Testing Status
                    CLists.dvTestingStatRND = CLists.dvLists.ToTable().DefaultView;
                    CPages.PageRndHome_1.gTestStat1.ItemsSource = CLists.dvTestingStatRND;
                    //            CPages.PageRndHome_1.gTestStat1.SelectedValue = CDefualts.iRNDTestingStat;

                    CLists.dvLists.RowFilter = "sList = 'RND Study Type'";  //Testing Status
                    CLists.dvRunTypeRND = CLists.dvLists.ToTable().DefaultView;
                    CPages.PageRndHome_1.gStudyType.ItemsSource = CLists.dvRunTypeRND;
                    //            CPages.PageRndHome_1.gStudyType.SelectedValue = CDefualts.iRNDRunType; //RND and study type are used interchangably

                    CLists.dvLists.RowFilter = "sList = 'RndProps'";  //Testing Status
                    CLists.dvPropsRND = CLists.dvLists.ToTable().DefaultView;
                    //               CPages.PageRndHome_1.gRndProp1.ItemsSource = CLists.dvPropsRND;
                    //               CPages.PageRndHome_1.gRndProp2.ItemsSource = CLists.dvPropsRND;
                    //            CPages.PageRndHome_1.gRndProp1.SelectedValue = "Density";
                    //            CPages.PageRndHome_1.gRndProp2.SelectedValue = "R90D75FFinal";

                    CLists.dvLists.RowFilter = "sList = 'RND Study Type' and ID <> 59";  //Testing Status
                    CLists.dvRunTypeRND2 = CLists.dvLists.ToTable().DefaultView;
                    CPages.PageRecipe_1.gStudyType.ItemsSource = CLists.dvRunTypeRND2;
                    CPages.PageRecipe_1.gStudyType.SelectedValue = 1;

                    CLists.dvEmployees = CLists.dtLoc.DefaultView;
                    CLists.dvEmployees.RowFilter = "IDLocation = 3";  //Employees
                    CLists.dvEmployees.Sort = "Employees Asc";
                    CLists.dvEmployeesRND = CLists.dvEmployees.ToTable().DefaultView;
                    CPages.PageRecipe_1.gChemist.ItemsSource = CLists.dvEmployeesRND;
                    CPages.PageRecipe_1.gChemist.SelectedValue = 24;
                    CPages.PageRecipe_1.gOperator.ItemsSource = CLists.dvEmployeesRND;
                    CPages.PageRecipe_1.gOperator.SelectedValue = 28;*//*

                }

                #endregion

            }*/

        /*        public static void ReadConfigFile()
                {
                    CAzure.Msi = ConfigurationManager.AppSettings.Get("Azure-Msi");
                    CAzure.Vault_Url = ConfigurationManager.AppSettings.Get("Azure-Vault-Url");
                 //   okta1.Port1 = int.Parse(ConfigurationManager.AppSettings.Get("Okta-Port1"));
                 //   okta1.Port2 = int.Parse(ConfigurationManager.AppSettings.Get("Okta-Port2"));
                    CLocalData.Db_ConStr = ConfigurationManager.AppSettings.Get("Local-Db-ConStr");
                    CLocalData.Machine = ConfigurationManager.AppSettings.Get("Local-Machine");
                    CLocalData.User = ConfigurationManager.AppSettings.Get("Local-User");
                    CDefualts.sGroup = ConfigurationManager.AppSettings.Get("Local-Group");
                    sServiceLink = ConfigurationManager.AppSettings.Get("Service-Incident");

                  //  CTelClient.sInsKey = ConfigurationManager.AppSettings.Get("Azure-Insights-Instrument-Key");
                  //  CTelClient.sIngestionEndpoint = ConfigurationManager.AppSettings.Get("Azure-Insights-IngestionEndpoint");
                }*/

        public bool GetUserInfo()
        {
            string sMsg;
            SqlDataAdapter da; double dtmp;

            /*Check that group assigned to user is valid.  The code will be commented until Okta starts providing the groups*/
            try
            {
                //               CDefualts.sLocation = "OKTA_GAF_Intugent_Admins";    //comment when Okta is ready
                //               CDefualts.sLocation = @"OKTA_GAF_Intugent_Mfg_Users_CedarCity";    //comment when Okta is ready

                if (cDefualts.sGroup == "OKTA_GAF_Intugent_Analyst")
                {
                    sMsg = "Welcome " + cbfile.sAppName + " to " + cbfile.sAppName + "\n\nThe group " + cDefualts.sGroup + " is not implemented yet" +
           ". \n\n" + cbfile.sAppName + " will close.";
                    //MessageBox.Show(sMsg, cbfile.sAppName); 
                    return false;
                }

                string sSqlQuery = "select * from dbo.tblLocations  where [Group] = '" + cDefualts.sGroup + "'";
                cLists.daEmployee = new SqlDataAdapter(sSqlQuery, cbfile.conAZ);
                cLists.dtEmployee.Clear();
                int itmp = cLists.daEmployee.Fill(cLists.dtEmployee);
                if (itmp < 1)
                {

                    sMsg = "Welcome " + cbfile.sAppName + " to " + cbfile.sAppName + "\n\nThe group " + cDefualts.sGroup + " assigned to you is unknown to " + cbfile.sAppName +
                        ". \n\n" + cbfile.sAppName + " will close.";
                    //  MessageBox.Show(sMsg, cbfile.sAppName); 
                    return false;
                }

                cLists.drEmployee = cLists.dtEmployee.Rows[0];
                cDefualts.IDLocation = (int)cLists.drEmployee["ID"];
                cDefualts.sLocation = (string)cLists.drEmployee["sLocation"];
                if (cLists.drEmployee["fDelTimeButton"] != DBNull.Value) cDefualts.dDelTimeButton = (double)cLists.drEmployee["fDelTimeButton"];
                if (cLists.drEmployee["fDelTimeCalc"] != DBNull.Value) cDefualts.dDelTimeCalc = (double)cLists.drEmployee["fDelTimeCalc"];

            }
            catch (SqlException ex)
            {
                sMsg = "Error while checking user's group in the database";
                // MessageBox.Show(ex.Message, cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "n\n" + ex.Message);
                //  CTelClient.TelExceptionFlush(ex, sMsg);
                return false;
            }

            /*Check if user is in the database.  If not, add the user */

            try
            {
                string sSqlQuery = "select * from Roster  where Employees = '" + cDefualts.sEmployee + "'";
                cLists.daEmployee = new SqlDataAdapter(sSqlQuery, cbfile.conAZ);
                cLists.dtEmployee.Clear();
                int itmp = cLists.daEmployee.Fill(cLists.dtEmployee);

                if (itmp < 1)
                {

                    sMsg = "Welcome to " + cbfile.sAppName + " " + cDefualts.sEmployee + "\n\n Add your name to Intugent PI databases?";
                    // if (MessageBox.Show(sMsg, cbfile.sAppName, MessageBoxButton.YesNo) != MessageBoxResult.Yes) return false;

                    cLists.drEmployee = cLists.dtEmployee.NewRow();  //Create a new recode
                    cLists.drEmployee["Employees"] = cDefualts.sEmployee;
                    cLists.drEmployee["IDLocation"] = cDefualts.IDLocation;     //Must be commented after implementing Okta
                    cLists.dtEmployee.Rows.Add(cLists.drEmployee);
                    new SqlCommandBuilder(cLists.daEmployee);
                    cLists.daEmployee.Update(cLists.dtEmployee);

                    sSqlQuery = "select * from Roster  where Employees = '" + cDefualts.sEmployee + "'";
                    cLists.daEmployee = new SqlDataAdapter(sSqlQuery, cbfile.conAZ);
                    cLists.dtEmployee.Clear();
                    itmp = cLists.daEmployee.Fill(cLists.dtEmployee);

                    if (itmp < 1)        //Getback record ID
                    {
                        sMsg = "Error while adding " + cbfile.sAppName + "to Database.\n\n Intugent PI will close?";
                        // MessageBox.Show(sMsg, cbfile.sAppName);
                        return false;
                    }
                }

                cLists.drEmployee = cLists.dtEmployee.Rows[0];
                //cDefualts.IDLocation = (int)cLists.drEmployee["IDLocation"];  //Must be commented after implementing Okta
                cDefualts.IDEmployee = (int)cLists.drEmployee["ID"];
                if (cDefualts.IDLocation != (int)cLists.drEmployee["IDLocation"])
                {
                    cLists.drEmployee["IDLocation"] = cDefualts.IDLocation;
                    cLists.UpdateEmployee();
                }

            }
            catch (SqlException ex)
            {
                sMsg = "Error while retieving info about " + cDefualts.sEmployee + " from the database";
                //   MessageBox.Show(sMsg, cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError("Unauthorized Access is Prohibited \n\n" + ex.Message);
                //   CTelClient.TelExceptionFlush(ex, sMsg);
                //   Close();
                return false;
            }


            return true;

        }

    }
}
