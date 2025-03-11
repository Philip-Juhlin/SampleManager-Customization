
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.ObjectModel;

namespace Customization.Tasks
{
    [SampleManagerTask("CustomStartupTask")]
    public class FindDuplicateLoginsTask : SampleManagerTask
    {
        protected override void SetupTask()
        {
            base.SetupTask();
            if (Library.Environment.GetGlobalString("ACCESS_LOG_MODE") != "FULL")
            {
                string Msg = "Checking duplicate logins requires full access log mode to run, your LIMS administrator";
                Logger.Warn(Msg);
                Library.Utils.FlashMessage(Msg, "Warning", MessageButtons.OK, MessageIcon.Warning, MessageDefaultButton.Button1);
                Exit();
            }

            string CurrentUserId = ((Personnel)Library.Environment.CurrentUser).Identity.Trim();
            string CurrentPid = Process.GetCurrentProcess().Id.ToString();

            using (Process process = new Process())
            {
                process.StartInfo.WorkingDirectory = Library.File.GetFile("smp$programs", "smp.exe").DirectoryName;
                process.StartInfo.FileName = Library.File.GetFile("smp$programs", "smp.exe").Name;
                process.StartInfo.Arguments = $" -instance {Library.Environment.GetGlobalString("INSTANCE")} -users";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                using (StreamReader UsersOutput = process.StandardOutput)
                {
                    string currentLine;

                    while ((currentLine = UsersOutput.ReadLine()) != null)
                    {
                        if (currentLine.Contains(CurrentUserId)
                        && currentLine.ToUpper().Contains("FOREGROUND")
                        && !currentLine.Contains(CurrentPid))
                        {
                            // Found another session for the same user
                            string OtherPid = currentLine.Trim().Substring(currentLine.Trim().Length - 7).Trim();

                            IQuery q = EntityManager.CreateQuery(AccessLog.StructureTableName);
                            q.AddEquals(AccessLogPropertyNames.Operator, CurrentUserId);
                            q.AddAnd();
                            q.AddEquals(AccessLogPropertyNames.ServerPid, OtherPid);
                            q.AddOrder(AccessLogPropertyNames.SessionId, false /*descending*/);

                            IEntityCollection AccessLogEntries = EntityManager.Select(q);

                            string LastLogin;
                            string ClientName;


                            if (AccessLogEntries.Count > 0)
                            {
                                AccessLog AccessLogEntry = (AccessLog)AccessLogEntries[0];
                                LastLogin = AccessLogEntry.ServerDate.ToString();
                                ClientName = AccessLogEntry.ClientName;

                                StringBuilder Msg = new StringBuilder();
                                //below lines instant kill previous sessions 20240910 remove and uncomment below commands for msg options
                                Msg.AppendLine($"Another SampleManager session for {CurrentUserId} has been found.");
                                Msg.AppendLine($"The session was started from workstation {ClientName} on {LastLogin}");
                                Msg.AppendLine("Session has been terminated");
                                Library.Utils.FlashMessage(Msg.ToString(), "Multiple logins");


                                Process processToKill = Process.GetProcessById(Convert.ToInt32(OtherPid));
                                processToKill.Kill();



                                /*
                                StringBuilder Msg = new StringBuilder();

                                Msg.AppendLine($"Another SampleManager session for {CurrentUserId} has been found.");
                                Msg.AppendLine($"The session was started from workstation {ClientName} on {LastLogin}");
                                Msg.AppendLine("Is it OK to close that session?");

                                if (Library.Utils.FlashMessageYesNo(Msg.ToString(), "Proceed?"))
                                {
                                Process processToKill = Process.GetProcessById(Convert.ToInt32(OtherPid));
                                processToKill.Kill();
                                }*/

                            }
                        }
                    }
                }
            }
            Exit();
        }

    }
}
