using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace TowerFall 
{
    public class BigLevelLoaderXML : Scene
    {
		public bool StartLevelOnFinish;
		private Coroutine loader;
		private int randomSeed;

        public XmlElement XML { get; private set; }
        public Session Session { get; private set; }
        public Level Level { get; private set; }
        public bool Finished { get; private set; }


        public BigLevelLoaderXML(Session session) : base()
        {
            StartLevelOnFinish = true;
            Session = session;
            XML = session.MatchSettings.LevelSystem.GetNextRoundLevel(session.MatchSettings, session.RoundIndex, out this.randomSeed);
            Session.MatchSettings.LevelSystem.Theme.OnLoad();
            loader = new Coroutine(Coroutine.DoNoFrameSkip(Load()));
        }

        public override void Update()
        {
            this.loader.Update();
        }

        public override void Render()
        {
            Draw.SpriteBatch.Begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            Draw.Rect(0f, 0f, 420f, 240f, Color.Black);
            Draw.SpriteBatch.End();
        }

        private IEnumerator Load()
        {
            TowerTheme theme = this.Session.MatchSettings.LevelSystem.Theme;
            if (this.Session.MatchSettings.Mode == Modes.Trials)
            {
                Music.Play(theme.Music);
            }
            else if (this.Session.MatchSettings.Mode == Modes.LevelTest)
            {
                Music.Play(theme.Music);
            }
            yield return 0;
            this.Level = new Level(this.Session, this.XML);
            yield return 0;
            this.Level.LoadSeed = this.randomSeed;
            Calc.Random = new Random(this.Level.LoadSeed);
            yield return 0;
            this.Level.Background = this.Session.MatchSettings.LevelSystem.GetBackground(this.Level);
            yield return 0;
            this.Level.Foreground = this.Session.MatchSettings.LevelSystem.GetForeground(this.Level);
            yield return 0;
            this.Session.LevelLoadStart(this.Level);
            yield return 0;
            bool[,] solidsBitData = Calc.GetBitData(this.XML["Solids"].InnerText, 42, 24);
            bool[,] bgBitData = Calc.GetBitData(this.XML["BG"].InnerText, 42, 24);
            int[,] overwriteData = Calc.ReadCSVIntGrid(this.XML["BGTiles"].InnerText, 42, 24);
            if (this.Session.MatchSettings.LevelSystem.Procedural)
            {
                bool[,] array = (bool[,])solidsBitData.Clone();
                XmlNodeList elementsByTagName = this.XML["Entities"].GetElementsByTagName("RandomBlock");
                List<Rectangle> list = new List<Rectangle>();
                foreach (object obj in elementsByTagName)
                {
                    XmlElement xmlElement = (XmlElement)obj;
                    Vector2 vector = xmlElement.Position();
                    Rectangle rectangle = new Rectangle((int)(vector.X / 10f), (int)(vector.Y / 10f), xmlElement.AttrInt("width") / 10, xmlElement.AttrInt("height") / 10);
                    list.Add(rectangle);
                }
                Calc.PushRandom(this.Session.MatchSettings.RandomLevelSeed);
                LevelRandomItems levelRandomItems = new LevelRandomItems();
                solidsBitData = LevelRandomGeometry.GenerateData(list, array);
                levelRandomItems.AddItems(solidsBitData, this.XML["Entities"], this.Level.Session.MatchSettings.TeamMode);
                LevelRandomTreasure.AddRandomTreasure(this.XML, levelRandomItems.BaseSolids, levelRandomItems.MovingPlatforms, this.Level.Session.MatchSettings.TeamMode);
                bgBitData = LevelRandomBGTiles.GenerateBitData(solidsBitData);
                overwriteData = LevelRandomBGDetails.GenerateTileData(levelRandomItems.BaseSolids, bgBitData, solidsBitData, this.XML["Entities"]);
                Calc.PopRandom();
            }
            this.Level.Add<LevelTiles>(this.Level.Tiles = new LevelTiles(this.XML, solidsBitData));
            yield return 0;
            this.Level.Add<LevelBGTiles>(this.Level.BGTiles = new LevelBGTiles(this.XML, bgBitData, solidsBitData, overwriteData));
            yield return 0;
            foreach (object obj2 in this.XML["Entities"])
            {
                XmlElement xmlElement2 = (XmlElement)obj2;
                this.Level.LoadEntity(xmlElement2);
                yield return 0;
            }
            if (this.XML["Entities"].GetElementsByTagName("BlueSwitchBlock").Count > 0 || this.XML["Entities"].GetElementsByTagName("RedSwitchBlock").Count > 0)
            {
                this.Level.Add<SwitchBlockControl>(new SwitchBlockControl(this.Session));
            }
            yield return 0;
            if (this.XML.AttrBool("CanUnlockMoonstone", false) && SaveData.Instance.Unlocks.ShouldShowMoonBreakSequence)
            {
                this.Level.Add<MoonBreakSequence>(new MoonBreakSequence());
            }
            yield return 0;
            if (this.XML.AttrBool("CanUnlockPurple", false) && SaveData.Instance.Unlocks.ShouldOpenPurpleArcherPortal)
            {
                this.Level.Add<PurpleArcherUnlockSequence>(new PurpleArcherUnlockSequence());
            }
            else if (this.Session.MatchSettings.Variants.DarkPortals)
            {
                this.Level.Add<DarkPortalsVariantSequence>(new DarkPortalsVariantSequence());
            }
            this.Level.UpdateEntityLists();
            yield return 0;
            solidsBitData = null;
            bgBitData = null;
            overwriteData = null;
            this.Session.OnLevelLoadFinish();
            yield return 0;
            if (this.StartLevelOnFinish)
            {
                Engine.Instance.Scene = this.Level;
            }
            this.Finished = true;
            yield break;
        }

    }
}

