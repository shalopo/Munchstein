using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace Munchstein
{
    public partial class Game : Form, ILevelContext
    {
        static readonly int DRAW_SCALE = 64;

        LevelsSequence LevelsSequence { get; set; }
        int LevelIndex { get; set; }
        ILevelFactory LevelFactory => LevelsSequence[LevelIndex];
        Level Level { get; set; }
        BufferedGraphics _staticGraphics;

        LevelReplayContext _levelReplayContext;

        string _msgToDisplay;
        DateTime _msgExpiry = DateTime.MinValue;

        Hint _hintToDisplay;
        DateTime _hintExpiry = DateTime.MinValue;

        DateTime _lastRenderTime = DateTime.MinValue;
        DateTime _lastFpsMeasurementTime = DateTime.MinValue;
        int _framesRendered = 0;
        int _fps = 0;
        bool _showGridlines = false;

        //const double MAX_FPS = 60;
        //const double MIN_RENDER_MS_DIFF = 1000.0 / MAX_FPS;

        public Game()
        {
            InitializeComponent();

            LevelsSequence = new LevelsSequence { new Levels.Easy.ConfusingJumpsLevel() };

            DebugLevel(Levels.Easy.LevelsSequenceFactory.Create());

            LevelIndex = 0;
            StartLevel();

            KeyDown += OnKeyDown;
        }

        private void DebugLevel(LevelsSequence levelsSequence)
        {
            _showGridlines = true;
            LevelsSequence = levelsSequence;
        }

        public void DisplayMessage(string msg, int? seconds = null)
        {
            if (!_levelReplayContext.Oneshot(msg))
            {
                return;
            }

            _msgToDisplay = msg;
            _msgExpiry = DateTime.UtcNow + new TimeSpan(0, 0, seconds.GetValueOrDefault(4));
        }

        public void DisplayHint(Hint hint)
        {
            _hintToDisplay = hint;
            _hintExpiry = DateTime.UtcNow + new TimeSpan(0, 0, 4);
        }

        public void SuppressHint(Hint hint)
        {
            _levelReplayContext.SuppressHint(hint);
        }

        private void StartLevel()
        {
            _msgToDisplay = null;
            _hintToDisplay = null;
            _levelReplayContext = new LevelReplayContext();

            InitLevel();

            if (_staticGraphics != null)
            {
                _staticGraphics.Dispose();
                _staticGraphics = null;
            }
        }

        private void InitLevel()
        {
            Level = LevelFactory.Create(this);
        }

        public void RestartLevel()
        {
            InitLevel();
        }

        public void NotifyLevelComplete()
        {
            Thread.Sleep(500);

            LevelIndex++;
            
            if (LevelIndex >= LevelsSequence.NumLevels)
            {
                LevelsSequence = Levels.Credits.LevelsSequenceFactory.Create();
                LevelIndex = 0;
            }

            StartLevel();
        }

        void DrawStaticObjects(Graphics g)
        {
            if (_showGridlines)
            {
                DrawGridlines(g);
            }

            if (Level.Door != null)
            {
                DrawDoor(g, Level.Door);
            }

            foreach (Platform platform in Level.Platforms)
            {
                DrawPlatform(g, platform);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_staticGraphics == null)
            {
                _staticGraphics = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
                DrawStaticObjects(_staticGraphics.Graphics);
                _staticGraphics.Render();
            }

            var g = e.Graphics;

            _staticGraphics.Render(g);

            var renderTimeDiff = DateTime.UtcNow - _lastRenderTime;

            //if (renderTimeDiff.TotalMilliseconds < MIN_RENDER_MS_DIFF)
            //{
            //    var sleep_ms = (int)Math.Round(MIN_RENDER_MS_DIFF - renderTimeDiff.TotalMilliseconds);
            //    Thread.Sleep(sleep_ms);

            //    g.DrawString(sleep_ms.ToString(), new Font("arial", 12), Brushes.White, 0, 10);
            //    //renderTimeDiff += new TimeSpan(0, 0, 0, 0, sleep_ms);
            //    renderTimeDiff = DateTime.UtcNow - _lastRenderTime;
            //}

            _lastRenderTime = DateTime.UtcNow;

            HandleContinuousKeys();

            const int STEPS_PER_SECOND = 200;
            var num_steps = (int)Math.Round(STEPS_PER_SECOND / 1000.0 * renderTimeDiff.TotalMilliseconds);

            for (int i = 0; i < num_steps; i++)
            {
                Level.Step(0.01);
            }

            if (!DrawMessage(g))
            {
                DrawHint(g);
            }

            DrawActor(g, Level.Actor);

            UpdateFps();
            g.DrawString(_fps.ToString(), new Font("arial", 12), Brushes.White, 0, 0);

            Invalidate();
        }

        private void DrawGridlines(Graphics g)
        {
            var pen = new Pen(Color.FromArgb(100, 50, 50, 50), 1);
            var font = new Font("arial", 10);

            int MAX_X = Size.Width / DRAW_SCALE;
            int MAX_Y = Size.Height / DRAW_SCALE;

            for (int x = 0; x < MAX_X; x++)
            {
                g.DrawLine(pen, new Point(TransformX(x), 0), new Point(TransformX(x), Height));
                g.DrawString(x.ToString(), font, Brushes.LightGray, Transform(new Point2(x, 0.2)));
            }

            for (int y = 0; y < MAX_Y; y++)
            {
                g.DrawLine(pen, new Point(0, TransformY(y)), new Point(Width, TransformY(y)));
                g.DrawString(y.ToString(), font, Brushes.LightGray, Transform(new Point2(0, y + 0.2)));
            }
        }

        private void UpdateFps()
        {
            _framesRendered++;

            if ((DateTime.Now - _lastFpsMeasurementTime).TotalSeconds >= 1)
            {
                _fps = _framesRendered;
                _framesRendered = 0;
                _lastFpsMeasurementTime = DateTime.Now;
            }
        }

        private int TransformX(double x)
        {
            return (int)Math.Round(x * DRAW_SCALE);
        }

        private int TransformY(double y)
        {
            return Height - (int)Math.Round(y * DRAW_SCALE);
        }

        private Point Transform(Point2 p)
        {
            return new Point(TransformX(p.X), TransformY(p.Y));
        }

        private Size Transform(Vector2 v)
        {
            return new Size((int)Math.Round(v.X * DRAW_SCALE), (int)Math.Round(v.Y * DRAW_SCALE));
        }

        private Rectangle Transform(BoxBoundary box)
        {
            return new Rectangle(Transform(box.TopLeft), Transform(box.Size));
        }

        private void DrawPlatform(Graphics g, Platform platform)
        {
            if (platform.IsPassThrough)
            {
                g.DrawRectangle(new Pen(Brushes.White), Transform(platform.Box));
            }
            else
            {
                g.FillRectangle(Brushes.White, Transform(platform.Box));
            }
        }

        private void DrawDoor(Graphics g, Door door)
        {
            g.FillRectangle(Brushes.Yellow, Transform(door.Box));
        }

        private void DrawActor(Graphics g, Actor actor)
        {
            g.FillRectangle(Brushes.Blue, new Rectangle(Transform(actor.Box.TopLeft), Transform(actor.Box.Size)));
        }

        private bool DrawMessage(Graphics g)
        {
            if (_msgToDisplay == null)
            {
                return false;
            }

            if (DateTime.UtcNow > _msgExpiry)
            {
                _msgToDisplay = null;
                return false;
            }

            var font = new Font("Courier new", 42);
            g.DrawString(_msgToDisplay, font, Brushes.Orange, new PointF(40, 60));

            return true;
        }

        private void DrawHint(Graphics g)
        {
            if (_hintToDisplay == null)
            {
                return;
            }

            if (DateTime.UtcNow > _hintExpiry || !_levelReplayContext.CanShowHint(_hintToDisplay))
            {
                _hintToDisplay = null;
                return;
            }

            var font = new Font("Courier new", 42);
            g.DrawString(_hintToDisplay.Message, font, Brushes.Orange, new PointF(40, 60));
        }

        private void HandleContinuousKeys()
        {
            var actor = Level.Actor;

            if (Keyboard.IsKeyDown(Key.Right))
            {
                actor.MoveSideways(1);
            }
            else if (Keyboard.IsKeyDown(Key.Left))
            {
                actor.MoveSideways(-1);
            }
            else
            {
                actor.Stop();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var actor = Level.Actor;

            switch (e.KeyCode)
            {
                case Keys.Down:
                    actor.Down();
                    break;
                case Keys.Up:
                    actor.Action();
                    break;
                case Keys.Space:
                    actor.Jump();
                    break;
                case Keys.R:
                    RestartLevel();
                    break;
                default:
                    break;
            }
        }

    }
}
