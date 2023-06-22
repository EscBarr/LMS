using Ionic.Zip;
using LMS.EntityСontext;
using LMS.Models;
using System.Diagnostics;

namespace LMS.Controllers
{
    public class DroneTest
    {
        public TestResultModel TestResultModel { get; set; }
        private ZipFile RepoData { get; set; }

        private string UserName { get; set; }

        public DroneTest(ZipFile OutputData, string userName)
        {
            RepoData = OutputData;
            UserName = userName;
        }

        public async Task ExecuteTask()
        {
            var test2 = Path.GetPathRoot(Environment.CurrentDirectory);
            var userDirectory = Path.Combine(test2, "Testing", UserName);
            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }
            else
            {
                DirectoryCleanup(userDirectory);
            }
            RepoData.Save(UserName);
            RepoData.ExtractAll(userDirectory);
            DirectoryInfo di = new DirectoryInfo(userDirectory);
            DirectoryInfo direct = new DirectoryInfo(userDirectory);
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                direct = dir;
            }

            string YmlPath = string.Empty;
            int cntr = 0;
            foreach (FileInfo file in direct.GetFiles())
            {
                if (file.Extension == ".yml")
                {
                    cntr++;
                    YmlPath = file.FullName;
                }
            }
            bool FileCorrect = true;

            string[] lines = File.ReadAllLines(YmlPath);

            foreach (string line in lines)
            {
                if (line.Contains("type:", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!line.Contains("type: docker", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        FileCorrect = false;
                        break;
                    }
                }
            }

            TestResultModel = new TestResultModel();
            if (cntr == 0 || cntr > 1 || !FileCorrect)
            {
                TestResultModel.UmlFounded = false;
                DirectoryCleanup(userDirectory);
                return;
            }
            TestResultModel.UmlFounded = true;
            string ResultCmd = "/C cd /d " + direct.FullName + "&&" + "drone exec " + YmlPath;

            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = ResultCmd,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = Process.Start(info))
            {
                //* Read the output (or the error)
                string output = process.StandardOutput.ReadToEnd();
                string err = process.StandardError.ReadToEnd();
                process.WaitForExit();
                TestResultModel.stdOut = output;
                TestResultModel.stdErr = err;
                TestResultModel.ProcessExtCode = process.ExitCode;
            }

            DirectoryCleanup(userDirectory);
        }

        private void DirectoryCleanup(string userDirectory)
        {
            DirectoryInfo di = new DirectoryInfo(userDirectory);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}