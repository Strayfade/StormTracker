using System;
using System.Windows;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;
using FortniteReplayReader;

namespace StormTracker
{
    class Program
    {
        static void Main()
        {
            //
            int StormPhaseCompare = 2;
            //

            // credit https://stackoverflow.com/questions/9993561/c-sharp-open-file-path-starting-with-userprofile
            var pathWithEnv = "%USERPROFILE%\\AppData\\Local\\FortniteGame\\Saved\\Demos";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            var sortedFiles = new DirectoryInfo(filePath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();

            string[] repl = { "", "" };
            for (int i = 0; i < repl.Length; i++)
            {
                FileInfo localInfo = sortedFiles[i];
                repl[i] = localInfo.FullName.ToString();
            }

            string[] replays = repl;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Located " + replays.Length + " most recent replays! Decompiling...");
            Console.WriteLine("");
            List<List<List<float>>> MATCHSafeZoneData = new List<List<List<float>>>();
            List<double> SafeZoneDistances = new List<double>();
            List<double> fVectorDistances = new List<double>();
            Console.Title = "StormTracker | Made By Strayfade#5205";
            Thread.Sleep(1000);
            for (int i = 0; i < repl.Length; i++)
            {
                string fileName = replays[i].ToString();
                FileInfo fi = new FileInfo(fileName);
                long filesize = fi.Length;
                if (filesize > (10 * 1000000))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine("Decompiling Replay " + (i + 1) + " of " + replays.Length + ": (" + replays[i] + ")");
                    Console.Title = "StormTracker | Made By Strayfade#5205 | Decompiling " + (i + 1) + " of " + replays.Length;

                    // credit @shiqan for making FortniteReplayDecompressor to read replay data
                    // decompress replay chunks using the Oodle Compression SDK 
                    // https://github.com/Shiqan/FortniteReplayDecompressor
                    var replayFile = replays[i];
                    var reader = new ReplayReader(parseMode: Unreal.Core.Models.Enums.ParseMode.Full);
                    var replay = reader.ReadReplay(replayFile);

                    var reboots = replay.MapData.RebootVans;
                    var drops = replay.MapData.SupplyDrops;
                    var safeZones = replay.MapData.SafeZones;

                    var release = replay.Header.Branch;

                    //Console.WriteLine("Replay Version: " + release.ToString());

                    List<List<float>> SafeZoneData = new List<List<float>>();
                    try
                    {
                        for (int z = 0; z < replay.MapData.SafeZones.Count; z++)
                        {
                            if (z < replay.MapData.SafeZones.Count)
                            {
                                if (safeZones[z].NextCenter != null)
                                {
                                    List<float> SafeZonePhaseDataCoords = new List<float>();

                                    //Console.WriteLine("");
                                    SafeZonePhaseDataCoords.Add(safeZones[z].NextCenter.X);
                                    SafeZonePhaseDataCoords.Add(safeZones[z].NextCenter.Y);
                                    SafeZoneData.Add(SafeZonePhaseDataCoords);
                                    double szSafeZone = (SafeZonePhaseDataCoords[0] + SafeZonePhaseDataCoords[1]);
                                    SafeZoneDistances.Add(szSafeZone);
                                }
                            }
                        }
                        MATCHSafeZoneData.Add(SafeZoneData);
                    }
                    finally
                    {
                        Console.Title = "StormTracker For Release " + release.Replace("++", "") + " | Made By Strayfade#5205";
                    }
                }
            }

            //First digit is replay number. Second digit is SafeZone phase. Third digit is ???. Fourth Digit is X/Y
            //MATCHSafeZoneData[0][0][0] Match 1, Phase 1 Storm, (0 = X, 1 = Y)

            // Calculate Storm
            List<List<List<float>>> InputData = MATCHSafeZoneData;

            float returnX = 0;
            float returnY = 0;

            List<float> allStormX = new List<float>();
            List<float> allStormY = new List<float>();

            List<float> returnValue = new List<float>();

            //Store available storm positions
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Found " + InputData.Count + " Matches with a Storm!");
            Console.Title = "StormTracker | Made By Strayfade#5205 | Storm Data Retrieved";
            for (int i = 0; i < InputData.Count; i++)
            {
                //MATCHSafeZoneData[0][0][0] Match 1, Phase 1 Storm, (0 = X, 1 = Y)
                if (i < InputData.Count)
                {
                    if ((StormPhaseCompare - 1) < InputData[i].Count)
                    {
                        allStormX.Add(InputData[i][StormPhaseCompare - 1][0]);
                        allStormY.Add(InputData[i][StormPhaseCompare - 1][1]);
                        Console.WriteLine("Retrieved Storm Data For Match #" + (i + 1) + "!");
                    }
                }
            }
            
            Console.WriteLine("");

            //Calculate average of all storm positions

            if (allStormX.Count < 1 || allStormY.Count < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("Not enough matches which satisfy arguments.");
                Console.WriteLine("Must have at least one match which reaches Storm Phase " + (StormPhaseCompare) + ".");
                Console.Title = "StormTracker | Made By Strayfade#5205 | Error: No Matches Reach Query";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                for (int i = 0; i < allStormX.Count; i++)
                {
                    returnX += allStormX[i];
                }
                returnX /= allStormX.Count;
                Console.WriteLine("Average X: " + returnX.ToString());

                for (int i = 0; i < allStormY.Count; i++)
                {
                    returnY += allStormY[i];
                }
                returnY /= allStormY.Count;
                Console.WriteLine("Average Y: " + returnY.ToString());
                returnValue.Add(returnX);
                returnValue.Add(returnY);
                double frontendX = Math.Round(returnX / 1000);
                double frontendY = Math.Round(returnY / 1000);
                frontendX /= 100;
                frontendY /= 100;
                double storedX = returnX;
                double storedY = returnY;

                Console.WriteLine("");
                for (int f = 0; f < allStormX.Count; f++)
                {
                    Console.WriteLine("Game " + (f + 1) + " Storm Position: (" + allStormX[f] + ", " + allStormY[f] + ")");
                    if (f > 1)
                    {
                        double distance = (SafeZoneDistances[f - 1] - SafeZoneDistances[f - 2]);
                        //Console.WriteLine("Distance From Last: " + distance);
                    }
                }

                Console.WriteLine("");
                Console.WriteLine("Found " + allStormX.Count + " Matches with required Storm Phase!");
                Console.WriteLine("Next Phase " + StormPhaseCompare + " Storm: (" + returnValue[0].ToString() + ", " + returnValue[1].ToString() + ")");

                Console.WriteLine("Converting UU to KM/MapSpace...");
                Thread.Sleep(1500);
                List<float> final = new List<float>();
                final.Add(returnX);
                final.Add(returnY);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("");
                Console.WriteLine("");
                Console.Write("Next Storm Prediction: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine(" (" + frontendX.ToString() + ", " + frontendY.ToString() + ")");

                Console.Title = "StormTracker    |    Next Storm:  [" + frontendX.ToString() + ", " + frontendY.ToString() + "]";
                Thread.Sleep(1500);
                if (!System.IO.File.Exists("C:\\Program Files\\StormTracker\\storedData.txt"))
                {
                    System.IO.Directory.CreateDirectory("C:\\Program Files\\StormTracker");
                    using (System.IO.FileStream fs = System.IO.File.Create("C:\\Program Files\\StormTracker\\storedData.txt"))
                    {
                        for (byte i = 0; i < 100; i++)
                        {
                            fs.WriteByte(i);
                        }
                    }
                }
                System.IO.File.WriteAllText("C:\\Program Files\\StormTracker\\storedData.txt", storedX.ToString() + "\n" + storedY.ToString());
                Console.WriteLine("Saved Data.");
                Console.Title = "StormTracker    |    Next Storm:  [" + frontendX.ToString() + ", " + frontendY.ToString() + "] (Saved)";
            }
            while(true)
            {

            }
        }
    }
}
