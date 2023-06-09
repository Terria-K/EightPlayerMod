﻿using System;
using System.IO;
using System.Reflection;
using System.Xml;
using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.ModInterop;
using MonoMod.Utils;
using TowerFall;

namespace EightPlayerMod 
{
    [Fort("com.fortrise.eightplayer", "EightPlayer")]
    public sealed class EightPlayerModule : FortModule
    {
        public static EightPlayerModule Instance;
        public static bool IsEightPlayer;
        public static bool LaunchedEightPlayer;
        public static bool CanVersusLevelSet;
        public static bool CanCoopLevelSet;
        public Atlas EightPlayerAtlas;
        public Atlas EightPlayerBGAtlas;
        public SpriteData EightPlayerSpriteData;

        public static MatchTeams StandardTeams;
        public static EightPlayerMatchTeams EightPlayerTeams = new EightPlayerMatchTeams(Allegiance.Neutral);

        public override Type SaveDataType => typeof(EightPlayerSaveData);
        public static EightPlayerSaveData SaveData => (EightPlayerSaveData)Instance.InternalSaveData;

        public EightPlayerModule() 
        {
            Instance = this;
        }

        public override void LoadContent()
        {
            EightPlayerAtlas = Content.LoadAtlas("Atlas/atlas.xml", "Atlas/atlas.png");
            EightPlayerBGAtlas = Content.LoadAtlas("Atlas/bgatlas.xml", "Atlas/bgatlas.png");
            EightPlayerSpriteData = Content.LoadSpriteData("Atlas/spriteData.xml", EightPlayerAtlas);
            FakeVersusTowerData.Load(0, "Levels/Versus/00 - Sacred Ground");
            FakeVersusTowerData.Load(1, "Levels/Versus/01 - Twilight Spire");
            FakeVersusTowerData.Load(2, "Levels/Versus/02 - Backfire");
            FakeVersusTowerData.Load(3, "Levels/Versus/03 - Flight");
            FakeVersusTowerData.Load(4, "Levels/Versus/04 - Mirage");
            FakeVersusTowerData.Load(5, "Levels/Versus/05 - Thornwood");
            FakeVersusTowerData.Load(6, "Levels/Versus/06 - Frostfang Keep");
            FakeVersusTowerData.Load(7, "Levels/Versus/07 - Kings Court");
            FakeVersusTowerData.Load(8, "Levels/Versus/08 - Sunken City");
            FakeVersusTowerData.Load(9, "Levels/Versus/09 - Moonstone");
            FakeVersusTowerData.Load(10, "Levels/Versus/10 - TowerForge");
            FakeVersusTowerData.Load(11, "Levels/Versus/11 - Ascension");
            FakeVersusTowerData.Load(12, "Levels/Versus/12 - The Amaranth");
            FakeVersusTowerData.Load(13, "Levels/Versus/13 - Dreadwood");
            FakeVersusTowerData.Load(14, "Levels/Versus/14 - Darkfang");
            FakeVersusTowerData.Load(15, "Levels/Versus/15 - Cataclysm");

            FakeDarkWorldTowerData.Load("0 - The Amaranth", "Content/Levels/DarkWorldLevels");
            FakeDarkWorldTowerData.Load("1 - Dreadwood", "Content/Levels/DarkWorldLevels");
            FakeDarkWorldTowerData.Load("2 - Darkfang", "Content/Levels/DarkWorldLevels");
            FakeDarkWorldTowerData.Load("3 - Cataclysm", "Content/Levels/DarkWorldLevels");
            FakeDarkWorldTowerData.Load("4 - Dark Gauntlet", "Content/Levels/DarkWorldLevels");
        }

        public override void Load()
        {
            TFGame.Players = new bool[8];
            TFGame.Characters = new int[8];
            TFGame.AltSelect = new ArcherData.ArcherTypes[8];
            TFGame.CoOpCrowns = new bool[8];
            for (int i = 4; i < 8; i++) 
            {
                TFGame.Characters[i] = i;
            }
            typeof(Player).GetField("wasColliders", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, new Collider[8]);
            ScreenPatch.Load();
            BackdropPatch.Load();
            RollcallPatch.Load();
            MainMenuPatch.Load();
            MiasmaPatch.Load();
            MapButtonPatch.Load();
            KingReaperPatch.Load();
            PlayerInputPatch.Load();
            TFGamePatch.Load();
            TreasureSpawnerPatch.Load();
            RoundLogicPatch.Load();
            ArcherPortraitPatch.Load();
            GifExporterPatch.Load();
            VersusStartPatch.Load();
            MatchSettingsPatch.Load();
            EightPlayerMatchTeams.Load();
            VersusMatchResultsPatch.Load();
            VersusRoundResultsPatch.Load();
            ReplayViewerPatch.Load();
            LavaPatch.Load();
            OrbLogicPatch.Load();
            WideQuestTowerStats.Load();
            QuestSavePatch.Load();
            SmallVersusPlayerMatchResults.Load();
            VersusAwardsPatch.Load();
            LevelRandomPatch.Load();
            DarkWorldLevelSystemPatch.Load();
            CoOpDataDisplayPatch.Load();
            QuestGameOverPatch.Load();
            AmaranthBossPatch.Load();
            DarkWorldHUDPatch.Load();
            CyclopsEyePatch.Load();
            CataclysmEyePatch.Load();
            VariantPatch.Load();
            DreadwoodBossControlPatch.Load();
            BottomMiasmaPatch.Load();
            LockDarkWorld();

            typeof(ModExports).ModInterop();
        }

        public override void Initialize()
        {
        }

        public override void Unload()
        {
            ScreenPatch.Unload();
            BackdropPatch.Unload();
            RollcallPatch.Unload();
            MainMenuPatch.Unload();
            MiasmaPatch.Unload();
            MapButtonPatch.Unload();
            KingReaperPatch.Unload();
            PlayerInputPatch.Unload();
            TFGamePatch.Unload();
            TreasureSpawnerPatch.Unload();
            RoundLogicPatch.Unload();
            ArcherPortraitPatch.Unload();
            GifExporterPatch.Unload();
            VersusStartPatch.Unload();
            MatchSettingsPatch.Unload();
            EightPlayerMatchTeams.Unload();
            VersusMatchResultsPatch.Unload();
            VersusRoundResultsPatch.Unload();
            ReplayViewerPatch.Unload();
            LavaPatch.Unload();
            OrbLogicPatch.Unload();
            WideQuestTowerStats.Unload();
            QuestSavePatch.Unload();
            SmallVersusPlayerMatchResults.Unload();
            VersusAwardsPatch.Unload();
            LevelRandomPatch.Unload();
            DarkWorldLevelSystemPatch.Unload();
            CoOpDataDisplayPatch.Unload();
            QuestGameOverPatch.Unload();
            AmaranthBossPatch.Unload();
            DarkWorldHUDPatch.Unload();
            CyclopsEyePatch.Unload();
            CataclysmEyePatch.Unload();
            VariantPatch.Unload();
            DreadwoodBossControlPatch.Unload();
            BottomMiasmaPatch.Unload();
            UnlockDarkWorld();
        }

        // Remove it soon, when it's finished or you need to test it
        public static void LockDarkWorld() 
        {
            On.TowerFall.DarkWorldButton.OnConfirm += Lock;
        }

        public static void UnlockDarkWorld() 
        {
            On.TowerFall.DarkWorldButton.OnConfirm -= Lock;
        }

        private static void Lock(On.TowerFall.DarkWorldButton.orig_OnConfirm orig, DarkWorldButton self)
        {
            if (EightPlayerModule.LaunchedEightPlayer)
            {
                var selfDynamic = DynamicData.For(self);
                Sounds.ui_invalid.Play(160f, 1f);
                selfDynamic.Set("shake", 30f);
                MenuInput.RumbleAll(1f, 20);
                CoOpDataDisplay first = self.Scene.Layers[-1].GetFirst<CoOpDataDisplay>();
                first?.DarkWorldShake();
                return;
            }
            orig(self);
        }
    }

    [ModExportName("com.fortrise.EightPlayerMod")]
    public static class ModExports 
    {
        public static bool IsEightPlayer() => EightPlayerModule.IsEightPlayer;
        public static bool LaunchedEightPlayer() => EightPlayerModule.LaunchedEightPlayer;
    }

    public static class Commands 
    {
        [Command("totalInputs")]
        public static void ShowTotalInput(string[] args) 
        {
            Logger.Log(TFGame.PlayerInputs.Length);
            Logger.Log(ArcherData.Archers.Length);
        }

        [Command("widescreen")]
        public static void TurnOnWidescreen(string[] args) 
        {
            EightPlayerModule.IsEightPlayer = !EightPlayerModule.IsEightPlayer;
            if (EightPlayerModule.IsEightPlayer) 
            {
                Engine.Instance.Screen.Resize(420, 240, 3f);
                WrapMath.AddWidth = new Vector2(420, 0f);
            }
            else 
            {
                Engine.Instance.Screen.Resize(320, 240, 3f);
                WrapMath.AddWidth = new Vector2(320, 0f);
            }
        }

        [Command("resize")]
        public static void Resize(string[] args) 
        {
            if (args.Length == 0)
                return;
            var folderPath = args[0];
            var directories = Directory.GetDirectories(folderPath);

            foreach (var dir in directories) 
            {
                var dirName = new DirectoryInfo(dir).Name;
                var files = Directory.GetFiles(dir);

                foreach (var file in files) 
                {
                    if (!Path.GetExtension(file).Contains("oel"))
                        continue;
                    var filename = Path.GetFileName(file);
                    var xml = Calc.LoadXML(file)["level"];

                    if (xml.GetAttribute("width") == "320") 
                    {
                        xml.SetAttr("width", 420);
                        InsertColumns(xml["BG"], "00000");
                        InsertColumns(xml["BGTiles"], "-1,-1,-1,-1,-1,");
                        InsertColumns(xml["Solids"], "00000");
                        InsertColumns(xml["SolidTiles"], "-1,-1,-1,-1,-1,");
                        foreach (XmlElement item2 in xml["Entities"])
                        {
                            int num6 = item2.AttrInt("x");
                            num6 += 50;
                            item2.SetAttr("x", num6);
                            if (item2.Name == "Spawner") 
                            {
                                foreach (XmlElement node in item2) 
                                {
                                    int nodeAttrX = node.AttrInt("x");
                                    nodeAttrX += 50;
                                    node.SetAttr("x", nodeAttrX);
                                }
                            }
                        }
                        var path = $"DumpLevels/Current/{dirName}/{filename}";
                        if (!Directory.Exists(Path.GetDirectoryName(path)))
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                        xml.OwnerDocument.Save(path);
                    }
                }
            }
        }

        private static void InsertColumns(XmlElement xml, string insert)
        {
            if (xml == null || !(xml.InnerText != ""))
            {
                return;
            }
            string text = xml.InnerText;
            int num = 0;
            while (num < text.Length)
            {
                if (text[num] == '\n')
                {
                    num++;
                    continue;
                }
                text = text.Insert(num, insert);
                num = text.IndexOf('\n', num);
                if (num == -1)
                {
                    break;
                }
                num++;
            }
            xml.InnerText = text;
        }

        [Command("unlockme")]
        public static void Unlock8PDarkWorld(string[] args) 
        {
            EightPlayerModule.UnlockDarkWorld();
        }
    }
}
