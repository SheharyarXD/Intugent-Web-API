using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Core
{
    public class Cbfile
    {
        public  bool bConAz = false;        //Change this to true when using GAF Azure Sql database
        public  bool bCanSwitchRecord = true;

        public  bool bChanged;
        public  string sAppName = "Intugent PI";
        public  string sDomain = "GAF.COM";
        public  string sDomain2 = "corp.gaf.com";
        public  TelemetryClient telClient;
        public  DateTime dateDefault = new DateTime(1900, 01, 01);

        public  string sVersion = "1.0.0";
        public  string sFileName = null;
        public  string sFileExt = ".ipr";
        public  string strLiscenceFilePath = "";

        public  string sDBFile = @"\Database\IntugentInsulationFoam.mdb";
        //       public  string sHelpFile = @"\Help\IntugentPI.chm";
        //       public  string sHelpFile = @"IntugentPI.chm";
        public  string sHelpFile = @"IntugentPI.pdf";
        public  string sIntDir = @"C:\Program Files\InsulationFoams";
        public  string sOptionsFile = @"\Options.text";

        public  char[] delimiterChars = { ' ', ',', ':', '\t' };
        public  String sDBConn()
        {
            string sCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=";
            return sCon + sIntDir + sDBFile;
        }
        public  string sConSql = @"Data Source=XD-1510\SQLEXPRESS; Initial Catalog=IntugentPI;" +
            "Integrated Security=SSPI;";
        public  int iIDMfg = 1943;
        //       public  int iIDRND = 1;
        public  int iIndexRND = 0;
        public  int iIDMfgIndex = 0;

        public  SqlConnection conAZ;

        public  string sHost = "azsmtpgw.gaf.com";
        public  string sSender = "donotreply@gaf.com";
        public  string sNoRecSwitchMsg = "Intugent PI is pulling process data.  It may take ~ 30 seconds.  You can not switch to a different data record in the mean time.";



/*        public  void SendEmail(string[] sLines)
        {
            string sUserState = "Intugent PI Data Export";
            MemoryStream stream = null; StreamWriter writer = null;

            int irow;

            SmtpClient client = new SmtpClient(sHost);
            MailAddress from = new MailAddress(sSender);
            MailAddress to = new MailAddress(CDefualts.sEmployee);
            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            message.Body = "Data exported from Intugent PI";
            // Include some non-ASCII characters in body and subject.
            message.Body += "\n\n  This is an automated message.  Do not reply to this email.  This email address does not receive any email.";
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = "Data exported from Intugent PI";
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            try
            {
                using (stream = new MemoryStream())
                using (writer = new StreamWriter(stream))    // using UTF-8 encoding by default
                {
                    for (irow = 0; irow < sLines.Length; irow++) writer.WriteLine(sLines[irow]);
                    stream.Position = 0;
                    message.Attachments.Add(new Attachment(stream, "Intugent PI Data.tsv"));
                    client.Send(message);
                    //CStatusBar.SetText("Email with attached file containing data was sent at " + DateTime.Now.ToString("hh:mm:ss:tt"));
                }

                // Set the method that is called back when the send operation ends.
                //                client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            }
            catch (Exception ex)
            {
                string sMsg = "Error in emailing data";
                //MessageBox.Show(sMsg, Cbfile.sAppName);
                //CTelClient.TelException(ex, "Error in sending data from memory file via email. \n\n" + ex.Message);
            }
            //            finally { if (writer != null) writer.Dispose(); if (stream != null) stream.Dispose(); }
            // Clean up.
            //          message.Dispose();

        }
*/


/*        public  void SendAttachment(string sFile, string sSubject, string sBody)
        {
            string userState = "Intugent PI Message";
            //Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                SmtpClient client = new SmtpClient(sHost);
                MailAddress from = new MailAddress(sSender);
                MailAddress to = new MailAddress(CDefualts.sEmployee);
                // Specify the message content.
                MailMessage message = new MailMessage(from, to);

                message.Body = sBody;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = sSubject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.Attachments.Add(new Attachment(sFile));
                client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                client.SendAsync(message, userState);
            }
            catch (Exception ex)
            {
                string sMsg = "Error in emailing data";
              //  MessageBox.Show(sMsg, Cbfile.sAppName);
              //  CTelClient.TelException(ex, "Error in sending help file via email. \n\n" + ex.Message);
            }

            finally
            {
               // Mouse.OverrideCursor = null;
            }

        }
*/
/*        private  void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

          //  if (e.Cancelled) MessageBox.Show("Attempt to email the data file was cancelled.", Cbfile.sAppName);
            if (e.Error != null)
            {
                string sMsg = "Error while sending email."; 
                //MessageBox.Show(token + "\n\n" + e.Error.ToString(), Cbfile.sAppName); CTelClient.TelTrace(sMsg + "\n\n" + e.Error.Message);
            }
            else
            {
                //CStatusBar.SetText("Email with attached file containing data was sent at " + DateTime.Now.ToString("hh:mm:ss:tt"));
            }
        }*/
    }
}
