using Raylib_cs;
using System.Numerics;
using System;
using System.IO;
using System.Diagnostics;

namespace ChessChallenge.Application
{
    public static class MenuUI
    {
        static float lastHeight = 0;

        static float scrollOffset = 0;


        
        
        public static void DrawButtons(ChallengeController controller)
        {

            int maxHeight = UIHelper.ScaleInt(800);
            int startHeightUnscaled = 125;
            int startHeight = UIHelper.ScaleInt(startHeightUnscaled);

            scrollOffset -= Raylib.GetMouseWheelMove() * 25;
            scrollOffset = Math.Clamp(scrollOffset, 0, Math.Max(lastHeight - maxHeight + UIHelper.ScaleInt(100),0.1f));

            Vector2 buttonPos = UIHelper.Scale(new Vector2(260, startHeightUnscaled-scrollOffset));
            Vector2 buttonSize = UIHelper.Scale(new Vector2(260, 55));
            float spacingY = buttonSize.Y * 1.2f;
            float breakSpacing = spacingY * 0.6f;

            float startY = buttonPos.Y;

            

            string[] customButtons = {"1", "2", "3","4"};
            string[] customTooltips = {"1", "2", "3","4"};
            bool[] pressedValues = ButtonsInColumn(customButtons, ref buttonPos, spacingY, buttonSize, customTooltips);

            Seperator(ref buttonPos, buttonSize.X, UIHelper.ScaleInt(25));

            bool[] pressedValues2 = ButtonsInColumn(customButtons, ref buttonPos, spacingY, buttonSize, customTooltips);

            Seperator(ref buttonPos, buttonSize.X, UIHelper.ScaleInt(25));
            
            
            


            // Game Buttons
            if (NextButtonInRow("Human vs Bot", ref buttonPos, spacingY, buttonSize))
            {
                var whiteType = controller.HumanWasWhiteLastGame ? ChallengeController.PlayerType.MyBot : ChallengeController.PlayerType.Human;
                var blackType = !controller.HumanWasWhiteLastGame ? ChallengeController.PlayerType.MyBot : ChallengeController.PlayerType.Human;
                controller.StartNewGame(whiteType, blackType);
            }
            if (NextButtonInRow("Bot vs Bot", ref buttonPos, spacingY, buttonSize))
            {
                controller.StartNewBotMatch(ChallengeController.PlayerType.MyBot, ChallengeController.PlayerType.MyBot);
            }
            if (NextButtonInRow("Bot vs EvilBot", ref buttonPos, spacingY, buttonSize))
            {
                controller.StartNewBotMatch(ChallengeController.PlayerType.MyBot, ChallengeController.PlayerType.EvilBot);
            }

            // Page buttons
            buttonPos.Y += breakSpacing;

            if (NextButtonInRow("Save Games", ref buttonPos, spacingY, buttonSize))
            {
                string pgns = controller.AllPGNs;
                string directoryPath = Path.Combine(FileHelper.AppDataPath, "Games");
                Directory.CreateDirectory(directoryPath);
                string fileName = FileHelper.GetUniqueFileName(directoryPath, "games", ".txt");
                string fullPath = Path.Combine(directoryPath, fileName);
                File.WriteAllText(fullPath, pgns);
                ConsoleHelper.Log("Saved games to " + fullPath, false, ConsoleColor.Blue);
            }
            if (NextButtonInRow("Rules & Help", ref buttonPos, spacingY, buttonSize))
            {
                FileHelper.OpenUrl("https://github.com/SebLague/Chess-Challenge");
            }
            if (NextButtonInRow("Documentation", ref buttonPos, spacingY, buttonSize))
            {
                FileHelper.OpenUrl("https://seblague.github.io/chess-coding-challenge/documentation/");
            }
            if (NextButtonInRow("Submission Page", ref buttonPos, spacingY, buttonSize))
            {
                FileHelper.OpenUrl("https://forms.gle/6jjj8jxNQ5Ln53ie6");
            }

            // Window and quit buttons
            buttonPos.Y += breakSpacing;

            bool isBigWindow = Raylib.GetScreenWidth() > Settings.ScreenSizeSmall.X;
            string windowButtonName = isBigWindow ? "Smaller Window" : "Bigger Window";
            if (NextButtonInRow(windowButtonName, ref buttonPos, spacingY, buttonSize))
            {
                Program.SetWindowSize(isBigWindow ? Settings.ScreenSizeSmall : Settings.ScreenSizeBig);
            }
            if (NextButtonInRow("Exit (ESC)", ref buttonPos, spacingY, buttonSize))
            {
                Environment.Exit(0);
            }

            

            lastHeight = buttonPos.Y - startY;

            void Seperator(ref Vector2 pos, float width, float spacing, float lineHeight = 10)
            {
                if(pos.Y > maxHeight+startHeight)
                    return;

                UIHelper.DrawSeperator(new Vector2(pos.X, pos.Y-spacingY/3f), width, Color.GRAY);
                pos.Y += spacing;
            }

            bool NextButtonInRow(string name, ref Vector2 pos, float spacingY, Vector2 size)
            {
                if(pos.Y > maxHeight+startHeight)
                    return false;

                bool pressed = UIHelper.Button(name, pos, size);
                pos.Y += spacingY;
                return pressed;
            }

            bool[] ButtonsInColumn(string[] names, ref Vector2 pos, float spacingY, Vector2 fullSize, string[]? tooltips = null)
            {
                if(pos.Y > maxHeight+startHeight)
                    return new bool[names.Length];

                bool[] pressedValues = new bool[names.Length];

                float spacingX = ((fullSize.X) / names.Length) * 0.1f;

                Vector2 partialSize = new Vector2(((fullSize.X - spacingX*(names.Length-1)) / names.Length) , fullSize.Y);

                Vector2 posCopy = new Vector2(pos.X-(fullSize.X/2f)+partialSize.X/2f, pos.Y);
                
                for(int i = 0; i < names.Length; i++)
                {
                    string item = names[i];

                    string? tooltip = tooltips != null && tooltips.Length > i ? tooltips[i] : "";
                    bool pressed = UIHelper.Button(item, posCopy, partialSize, tooltip);

                    posCopy.X += partialSize.X + spacingX;
                    
                    if(i == names.Length-1)
                        pos.Y += spacingY;
                    

                    if (pressed)
                        pressedValues[i] = true;
                    else
                        pressedValues[i] = false;
                    
                    
                }

                
                return pressedValues;
            }
        }
    }
}