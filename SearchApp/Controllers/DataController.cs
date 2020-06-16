using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Web.Script.Serialization;
using System.IO;
using SearchApp.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SearchApp.Controllers
{
    public class DataController : Controller
    {
        //
        // GET: /Data/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public void SaveNewScene(SceneModel scene)
        {
            var strPath = @"C:\projects\Go\imgprocessing\puzzle_boards\" + scene.SceneName + ".json";
            System.IO.File.WriteAllText(strPath, JsonConvert.SerializeObject(scene));

        }
        [HttpPost]
        public void SaveNewPuzzle(string name, List<PixelModel> pixels)
        {
            using (StreamWriter wr = new StreamWriter(@"C:\projects\Go\imgprocessing\puzzle_boards\" + name + ".txt"))
            {
                pixels.ForEach(p =>
                {
                    wr.Write(p.Letter + "," + p.Position.x + "," + p.Position.y + "," + p.Red + "," + p.Green + "," + p.Blue + " ");
                });
            }
        }
        [HttpPost]
        public void SaveContentStream(string dataDump)
        {
            var strDate = DateTime.Now.ToString();
            var formatArr = strDate.Split(' ');
            var newFormat = formatArr[0].Replace("/", "-");
            using (StreamWriter wr = System.IO.File.AppendText(@"C:\projects\Go\imgprocessing\puzzle_boards\DataDump-" + newFormat + ".txt"))
            {
                wr.Write(dataDump);
            }


        }
        [HttpPost]
        public void SavePuzzle(string name, List<PixelSection> sections)
        {

            sections.ForEach(s =>
            {
                if (System.IO.File.Exists(@"C:\projects\Go\imgprocessing\puzzle_boards\" + name + "_" + s.SectionName + "_board.txt"))
                {
                    System.IO.File.Delete(@"C:\projects\Go\imgprocessing\puzzle_boards\" + name + "_" + s.SectionName + "_board.txt");
                }
                using (StreamWriter wr = new StreamWriter(@"C:\projects\Go\imgprocessing\puzzle_boards\" + name + "_" + s.SectionName + "_board.txt"))
                {
                    s.Pixels.ForEach(p =>
                    {
                        wr.Write(p.Letter + "," + p.Position.x + "," + p.Position.y + "," + p.Red + "," + p.Green + "," + p.Blue + " ");
                    });
                }
            });




            sections.ForEach(s =>
            {
                if (System.IO.File.Exists(@"C:\projects\Go\imgprocessing\puzzle_boards\" + name + "_" + s.SectionName + "_words.txt"))
                {
                    System.IO.File.Delete(@"C:\projects\Go\imgprocessing\puzzle_boards\" + name + "_" + s.SectionName + "_words.txt");
                }
                using (StreamWriter wr = new StreamWriter(@"C:\projects\Go\imgprocessing\puzzle_boards\" + name + "_" + s.SectionName + "_words.txt"))
                {

                    s.Words.ForEach(p =>
                    {
                        if (p.Letter == "$")
                            wr.Write(p.Letter + " ");
                        else
                            wr.Write(p.Letter + "," + p.Position.x + "," + p.Position.y + "," + p.Red + "," + p.Green + "," + p.Blue + "," + p.Direction + "," + p.Index +  " ");
                    });
                }
            });


        }
        [HttpGet]
        public ActionResult GetLoadedSceneJson(string fileName)
        {
            using (StreamReader rd = new StreamReader(@"C:\projects\Go\imgprocessing\puzzle_boards\" + fileName))
            {
                string json = rd.ReadToEnd();
                SceneModel scene = JsonConvert.DeserializeObject<SceneModel>(json);

                var jsonResult = Json(scene, JsonRequestBehavior.AllowGet);
                return jsonResult;
            }
        }

        [HttpGet]
        public ActionResult GetLoadedScenePuzzle(SceneRequestModel scene)
        {
            string folderPath = @"C:\projects\Go\imgprocessing\puzzle_boards\" + scene.scenePuzzleName + "#" + scene.puzzleName + ".txt";
            // var puzzleName = scenePuzzleName.Split('#')[1];

            using (StreamReader rd = new StreamReader(folderPath))
            {
                var puzzleAnonmynous = AnamolyousPuzzle(folderPath, scene.puzzleName);
                var jsonResult = Json(puzzleAnonmynous, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        
        [HttpGet]
        public ActionResult GetSavedPuzzles()
        {
            //var fileNames = new List<SceneModel>();
            // var dict = new Dictionary<string, List<string>>();
            var fileNames = new List<string>();

            //var txtFiles = Directory.EnumerateFiles(@"C:\projects\Go\imgprocessing\puzzle_boards\", "*.txt");
            var txtFiles = Directory.EnumerateFiles(@"C:\projects\Go\imgprocessing\puzzle_boards\", "*.json");
            foreach (string f in txtFiles)
            {

                var nameArr = f.Split('_');
                var name = nameArr[1].Replace(@"boards\", string.Empty).Replace(".txt", string.Empty);
                name = name.Split('#')[0];
                var sceneFiles = fileNames.Where(x => x.Contains(name)).ToList();

                if (sceneFiles.Count() == 0)
                    fileNames.Add(name);
                /*if (!dict.ContainsKey(name))
                {
                    dict.Add(name, new List<string>() { nameArr[2] });
                }
                else
                {
                   var sections = dict[name];
                    var sceneFiles = sections.Where(x => x.Contains(nameArr[2])).ToList();

                    if(sceneFiles.Count() == 0)
                       sections.Add(nameArr[2]);
                }*/

            }
            /* foreach (var entry in dict)
             {
                 fileNames.Add(new SceneModel
                 {
                     Name = entry.Key,
                     Scenes = entry.Value
                 });
             }*/

            var jsonResult = Json(fileNames, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
        [HttpGet]
        public ActionResult GetBlockedColors()
        {
            string line = string.Empty;
            var colors = new List<string>();
            System.IO.StreamReader file =
                 new System.IO.StreamReader(@"C:\projects\Go\imgprocessing\banned_colors.txt");
            while ((line = file.ReadLine()) != null)
            {
                colors.Add(line);
                
            }
            file.Close();
            return Json(colors, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetSingleScene(string scene)
        {
            var sections = new List<PixelSection>();
            //  var fileNames = new List<string>();
            //  var folderPath = @"C:\projects\Go\imgprocessing\puzzle_boards\";
            var dict = new Dictionary<string, PixelSection>();


            var txtFiles = Directory.EnumerateFiles(@"C:\projects\Go\imgprocessing\puzzle_boards\", "*.txt");
            var sceneFiles = txtFiles.Where(f => f.Contains(scene)).ToList();
            sceneFiles.ForEach(f =>
            {
                var fileNameArr = f.Split('_');
                //var sceneName = fileNameArr[2] + "_" + fileNameArr[3];
                var sceneName = fileNameArr[2];
                var returnLetters = new List<PixelModel>();
                var wordLetters = new List<PixelModel>();
                if (f.IndexOf("_board.txt") > 0)
                {

                    using (StreamReader rd = new StreamReader(f))
                    {
                        var data = rd.ReadToEnd();
                        var eachPixelArr = data.Split(' ').ToList();
                        eachPixelArr.ForEach(strPixel =>
                        {
                            if (string.IsNullOrEmpty(strPixel))
                                return;

                            var pixelArr = strPixel.Split(',');
                            // try
                            //  {
                            returnLetters.Add(new PixelModel
                            {
                                Letter = pixelArr[0],
                                Red = Convert.ToInt32(pixelArr[3]),
                                Green = Convert.ToInt32(pixelArr[4]),
                                Blue = Convert.ToInt32(pixelArr[5]),
                                Position = new PositionModel
                                {
                                    x = Convert.ToInt32(pixelArr[1]),
                                    y = Convert.ToInt32(pixelArr[2])
                                }
                            });
                            //  }catch(Exception)
                            //{
                            //    var next = pixelArr;
                            //}

                        });
                    }
                    if (!dict.ContainsKey(sceneName))
                    {
                        var pixel = new PixelSection
                        {
                            SectionName = sceneName,
                            Pixels = returnLetters,
                            Plan = LoadPlan(sceneName),
                            Modulus = VerticalModulus(returnLetters)
                        };
                        dict.Add(sceneName, pixel);
                    }
                    else
                    {
                        var pixelSection = dict[sceneName];
                        pixelSection.Pixels = returnLetters;
                        pixelSection.Plan = LoadPlan(sceneName);
                        pixelSection.Modulus = VerticalModulus(returnLetters);
                        dict[sceneName] = pixelSection;
                    }
                }
                if (f.IndexOf("_words.txt") > 0)
                {

                    using (StreamReader rd = new StreamReader(f))
                    {
                        var data = rd.ReadToEnd();
                        var eachPixelArr = data.Split(' ').ToList();

                        eachPixelArr.ForEach(strPixel =>
                        {
                            if (string.IsNullOrEmpty(strPixel))
                                return;

                            var pixelArr = strPixel.Split(',');
                            if (pixelArr[0] == "$")
                            {
                                wordLetters.Add(new PixelModel
                                {
                                    Letter = pixelArr[0]

                                });
                            }
                            try
                            {
                                wordLetters.Add(new PixelModel
                                {
                                    Letter = pixelArr[0],
                                    Red = Convert.ToInt32(pixelArr[3]),
                                    Green = Convert.ToInt32(pixelArr[4]),
                                    Blue = Convert.ToInt32(pixelArr[5]),
                                    Direction = Convert.ToInt32(pixelArr[6]),
                                    Position = new PositionModel
                                    {
                                        x = Convert.ToInt32(pixelArr[1]),
                                        y = Convert.ToInt32(pixelArr[2])
                                    }
                                });
                            }
                            catch (Exception)
                            {
                                var test = pixelArr;
                            }

                        });
                        if (!dict.ContainsKey(sceneName))
                        {
                            var words = new PixelSection
                            {
                                SectionName = sceneName,
                                Words = wordLetters
                            };
                            dict.Add(sceneName, words);
                        }
                        else
                        {
                            var wordsSection = dict[sceneName];
                            wordsSection.Words = wordLetters;
                            dict[sceneName] = wordsSection;
                        }
                    }
                }


            });
            foreach (var entry in dict)
            {
                sections.Add(entry.Value);
            }


            var jsonResult = Json(sections, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpGet]
        public ActionResult GetPuzzleNames()
        {
            var fileNames = new List<string>();
            var d = System.DateTime.Now.Day;
            var month = System.DateTime.Now.Month;
            var year = System.DateTime.Now.Year;

            string folderPath = @"C:\projects\Go\imgprocessing\puzzles_processed\" + month.ToString() + "_" + d.ToString() + "_" + year.ToString();
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.txt"))
            {
                var parts = file.Split('\\');
                fileNames.Add(parts[parts.Length - 1].Replace(".txt", string.Empty));
            }
            var jsonResult = Json(fileNames, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
        [HttpGet]
        public ActionResult GetWordsPuzzle()
        {
            var returnLetters = new List<PixelModel>();
            var regNumber = new Regex(@"[0-9]+");
            var allContent = string.Empty;
            using (StreamReader rd = new StreamReader(@"C:\projects\Go\imgprocessing\puzzles_processed\baby_tiger_result.txt"))
            {
                allContent = rd.ReadToEnd();
                var eachPixelArr = allContent.Split(' ').ToList();
                eachPixelArr.ForEach(strPixel =>
                {
                    if (string.IsNullOrEmpty(strPixel))
                        return;

                    var pixelArr = strPixel.Split(',');

                    returnLetters.Add(new PixelModel
                    {
                        Letter = pixelArr[0],
                        Red = Convert.ToInt32(pixelArr[3]),
                        Green = Convert.ToInt32(pixelArr[4]),
                        Blue = Convert.ToInt32(pixelArr[5]),
                        Position = new PositionModel
                        {
                            x = Convert.ToInt32(pixelArr[1]),
                            y = Convert.ToInt32(pixelArr[2])
                        }
                    });

                });

            }
            var startChar = returnLetters.Where(pm => pm.Letter.IndexOf("^") >= 0).ToList();
            // StringBuilder accrue = new StringBuilder();
            var pickWords = new List<List<PixelModel>>();
            startChar.ForEach(pm =>
            {

                var word = SearchWords(pm, returnLetters);
                pickWords.Add(word);

            });

            var myAnonymousType = new
            {
                AllLetters = returnLetters,
                Words = pickWords
            };
            var jsonResult = Json(pickWords, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        [HttpGet]
        public ActionResult GetSinglePuzzle(string puzzleName)
        {
            var d = System.DateTime.Now.Day;
            var month = System.DateTime.Now.Month;
            var year = System.DateTime.Now.Year;

            string folderPath = @"C:\projects\Go\imgprocessing\puzzles_processed\" + month.ToString() + "_" + d.ToString() + "_" + year.ToString() + @"\" + puzzleName + ".txt";
            var puzzleAnonmynous = AnamolyousPuzzle(folderPath, puzzleName);
             var jsonResult = Json(puzzleAnonmynous, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpGet]
        public ActionResult GetPuzzles()
        {
            var returnLetters = new List<PixelModel>();
            using (StreamReader rd = new StreamReader(@"C:\projects\Go\imgprocessing\" + "tigerMergedAll" + ".txt"))
            {
                var data = rd.ReadToEnd();
                var eachPixelArr = data.Split(' ').ToList();
                eachPixelArr.ForEach(strPixel =>
                {
                    if (string.IsNullOrEmpty(strPixel))
                        return;

                    var pixelArr = strPixel.Split(',');
                    returnLetters.Add(new PixelModel
                    {
                        Letter = pixelArr[0],
                        Red = Convert.ToInt32(pixelArr[3]),
                        Green = Convert.ToInt32(pixelArr[4]),
                        Blue = Convert.ToInt32(pixelArr[5]),
                        Position = new PositionModel
                        {
                            x = Convert.ToInt32(pixelArr[1]),
                            y = Convert.ToInt32(pixelArr[2])
                        }
                    });

                });
            }
            var jsonResult = Json(returnLetters, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        [HttpGet]
        public ActionResult GetPixelImage(string spriteName)
        {
            var returnPixels = new List<PixelModel>();

            using (StreamReader rd = new StreamReader(@"C:\projects\Go\imgprocessing\" + spriteName + ".txt"))
            {
                var data = rd.ReadToEnd();
                var eachPixelArr = data.Split(' ').ToList();
                //Regex reg = new Regex("[0-9]+,[0-9]+,[0-9]+,[0-9]+,[0-9]+", RegexOptions.Multiline);
                //var matches = reg.Matches(data);
                int cursorX = 0;
                int cursorY = 0;

                //foreach (Match m in matches)
                eachPixelArr.ForEach(strPixel =>
                {
                    if (string.IsNullOrEmpty(strPixel))
                        return;

                    //var strPixel = m.Value;

                    var pixelArr = strPixel.Split(',');
                    returnPixels.Add(new PixelModel
                    {
                        Red = Convert.ToInt32(pixelArr[0]),
                        Green = Convert.ToInt32(pixelArr[1]),
                        Blue = Convert.ToInt32(pixelArr[2]),
                        Position = new PositionModel
                        {
                            x = Convert.ToInt32(pixelArr[3]),
                            y = Convert.ToInt32(pixelArr[4])
                        }
                    });
                    /* cursorX++;
                     if (cursorX > 50)
                     {
                         cursorX = 0;
                         cursorY++;
                     }*/

                }
                  );

            }
            var jsonResult = Json(returnPixels, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        public ActionResult DrawCells()
        {
            return View();
        }
        private string FindCharacters(int direction, PixelModel pm, List<PixelModel> puzzle)
        {
            double x = pm.Position.x;
            double y = pm.Position.y;

            if (direction == 1) //U
            {
                y -= 12;
            }
            else if (direction == 2) //RU
            {
                y -= 12;
                x += 12;
            }
            else if (direction == 3) //R
            {
                x += 12;
            }
            else if (direction == 4) //RD
            {
                x += 12;
                y += 12;
            }
            else if (direction == 5) //D
            {
                y += 12;
            }
            else if (direction == 6) //LD
            {
                x -= 12;
                y += 12;
            }
            else if (direction == 7) //L
            {
                x -= 12;
            }
            else if (direction == 8) //LU
            {
                x -= 12;
                y -= 12;
            }
            //Lets find new pixel
            var nextLetter = puzzle.Where(p => p.Position.x == x && p.Position.y == y).FirstOrDefault();
            if (nextLetter == null)
            {
                return string.Empty;
            }
            var regEnd = new Regex(@"\$[0-9]+");
            if (nextLetter.Letter.IndexOf("$") >= 0)
            {
                var letter = nextLetter.Letter.Replace("$", string.Empty);
                // accrue.Append(letter);
                return letter;
            }
            else
            {
                //accrue.Append(nextLetter.Letter);
                return nextLetter.Letter + ":" + x.ToString() + ":" + y.ToString() + "," + FindCharacters(direction, nextLetter, puzzle);

            }

        }
        private List<PixelModel> SearchWords(PixelModel pm, List<PixelModel> puzzle)
        {
            var pixelLs = new List<PixelModel>();
            var regNum = new Regex("[0-9]+");
            var num = regNum.Match(pm.Letter);
            var direction = Convert.ToInt32(num.Value);
            var cleanLetter = regNum.Replace(pm.Letter, string.Empty);
            cleanLetter = cleanLetter.Replace("^", string.Empty);
            //accrue.Append(cleanLettter);
            var theWord = cleanLetter + ":" + pm.Position.x.ToString() + ":" + pm.Position.y.ToString() + "," + FindCharacters(direction, pm, puzzle);
            var firstWord = new PixelModel
            {
                Letter = cleanLetter,
                Red = pm.Red,
                Green = pm.Green,
                Blue = pm.Blue,
                Direction = direction,
                Position = new PositionModel
                {
                    x = pm.Position.x,
                    y = pm.Position.y
                }
            };
            pixelLs.Add(firstWord);
            var words = theWord.Split(',');
            for (int i = 1; i < words.Length - 1; i++)
            {
                var wordSplit = words[i].Split(':');
                var charP = new PixelModel
                {
                    Letter = wordSplit[0],
                    Direction = direction,
                    Position = new PositionModel
                    {
                        x = Convert.ToInt32(wordSplit[1]),
                        y = Convert.ToInt32(wordSplit[2])
                    }
                };
                pixelLs.Add(charP);
            }
            return pixelLs;
        }
        private PlanModel LoadPlan(string puzzleName)
        {
            var d = System.DateTime.Now.Day;
            var month = System.DateTime.Now.Month;
            var year = System.DateTime.Now.Year;

            Regex fontReg = new Regex("Font:([0-9]+)");
            Regex outPutReg = new Regex(@"Output\:(.+)\.", RegexOptions.Multiline);
            Regex styleReg = new Regex(@"Style:(.+)");
            Regex spaceReg = new Regex("Spacing:([0-9]+)");

            string planPath = @"C:\projects\Go\imgprocessing\processing\" + month.ToString() + "_" + d.ToString() + "_" + year.ToString() + @"\" + "plan.txt";
            using (StreamReader rd = new StreamReader(planPath))
            {
                var planContent = rd.ReadToEnd();
                var nameMatches = outPutReg.Matches(planContent);
                var fontMatches = fontReg.Matches(planContent);
                var styleMatches = styleReg.Matches(planContent);
                var spaceMatches = spaceReg.Matches(planContent);
                var iter = 0;
                var allPlans = new List<PlanModel>();
                foreach (Match val in nameMatches)
                {
                    var plan = new PlanModel
                    {
                        Name = val.Groups[1].Value,
                        Font = Convert.ToInt32(fontMatches[iter].Groups[1].Value),
                        Style = styleMatches[iter].Groups[1].Value.Replace("\r", string.Empty),
                        Spacing = Convert.ToInt32(spaceMatches[iter].Groups[1].Value)
                    };
                    iter++;
                    allPlans.Add(plan);

                }
                var myPlan = allPlans.Where(p => p.Name.Contains(puzzleName)).FirstOrDefault();
                return myPlan;

            }
        }
        private Object AnamolyousPuzzle(string puzzleNamePath, string puzzleName)
        {
            var returnLetters = new List<PixelModel>();
           

           
            using (StreamReader rd = new StreamReader(puzzleNamePath))
            {
                var data = rd.ReadToEnd();
                var index = 0;
                var eachPixelArr = data.Split(' ').ToList();
                eachPixelArr.ForEach(strPixel =>
                {
                    if (string.IsNullOrEmpty(strPixel))
                        return;

                    var pixelArr = strPixel.Split(',');
                    returnLetters.Add(new PixelModel
                    {
                        Letter = pixelArr[0],
                        ID = index,
                        Red = Convert.ToInt32(pixelArr[3]),
                        Green = Convert.ToInt32(pixelArr[4]),
                        Blue = Convert.ToInt32(pixelArr[5]),
                        BackgroundBlue = 255,
                        BackgroundGreen = 255,
                        BackgroundRed = 255,
                        IsVisible = true,
                        Outline = false,
                        Position = new PositionModel
                        {
                            x = Convert.ToInt32(pixelArr[1]),
                            y = Convert.ToInt32(pixelArr[2])
                        }
                    });
                    index++;
                });
            }
          
            var puzzleAnonmynous = new
            {
                AllLetters = returnLetters,
                Words = 0,
                Plan = LoadPlan(puzzleName),
                Modulus = VerticalModulus(returnLetters)

            };

            return puzzleAnonmynous;
        }
        private int VerticalModulus(List<PixelModel> pixels)
        {
            var start = pixels[0].Position.x;
            var cntr = 0;
            for(int i=1;i < pixels.Count(); i++)
            {
                if(pixels[i].Position.x != start)
                {
                    break;
                }
                cntr++;
            }
            return cntr;
        }

    }
}