using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        int DRAW_SCALE = 64;

        LevelsSequence LevelsSequence { get; set; }
        int LevelIndex { get; set; }
        ILevelFactory LevelFactory => LevelsSequence[LevelIndex];
        Level Level { get; set; }
        BufferedGraphics _staticGraphics;
        bool _paused = false;

        LevelReplayContext _levelReplayContext;

        string _msgToDisplay;
        DateTime _msgExpiry = DateTime.MinValue;

        Hint _hintToDisplay;
        DateTime _hintExpiry = DateTime.MinValue;

        DateTime _lastRenderTime = DateTime.MinValue;
        DateTime _lastFpsMeasurementTime = DateTime.MinValue;
        int _framesRendered = 0;
        int _fps = 0;

        bool _debug = Debugger.IsAttached;

        public Game()
        {
            InitializeComponent();

            LevelsSequence = Levels.Easy.LevelsSequenceFactory.Create();

            //LevelsSequence = new LevelsSequence { new LevelFactory<Levels.Easy.OrientationFlipLevel>() };
            //LevelsSequence = new LevelsSequence { new LevelFactory<Levels.DebugLevel>() };

            LevelIndex = 0;
            StartLevel();

            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
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
            InitLevel();

            _msgToDisplay = null;
            _hintToDisplay = null;
            _levelReplayContext = new LevelReplayContext();
            DisposeStaticGraphicsBuffer();
        }

        private void InitLevel()
        {
            Level = LevelFactory.Create(this);
            _paused = false;
        }

        public void RestartLevel()
        {
            InitLevel();
        }

        public void NotifyLevelComplete()
        {
            NextLevel();
        }

        private void NextLevel()
        {
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
            DrawGridlines(g);

            if (Level.Door != null)
            {
                DrawDoor(g, Level.Door);
            }

            foreach (Platform platform in Level.Platforms)
            {
                DrawPlatform(g, platform);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            DRAW_SCALE = Math.Min(Width / 30, Height / 18);
            RerenderStaticGraphics();
        }

        private void RerenderStaticGraphics()
        {
            DisposeStaticGraphicsBuffer();
            RenderStaticGraphicsBuffer();
        }

        private BufferedGraphics RenderStaticGraphicsBuffer()
        {
            if (_staticGraphics == null && Level != null)
            {
                _staticGraphics = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
                DrawStaticObjects(_staticGraphics.Graphics);
                _staticGraphics.Render();
            }

            return _staticGraphics;
        }

        private void DisposeStaticGraphicsBuffer()
        {
            if (_staticGraphics != null)
            {
                _staticGraphics.Dispose();
                _staticGraphics = null;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            RenderStaticGraphicsBuffer().Render(g);

            var renderTimeDiff = DateTime.UtcNow - _lastRenderTime;

            _lastRenderTime = DateTime.UtcNow;

            const int STEPS_PER_SECOND = 200;
            var num_steps = (int)Math.Round(STEPS_PER_SECOND / 1000.0 * renderTimeDiff.TotalMilliseconds);

            if (!_paused)
            {
                for (int i = 0; i < num_steps; i++)
                {
                    HandleContinuousKeys();
                    Level.Step(0.01);
                }
            }

            if (!DrawMessage(g))
            {
                DrawHint(g);
            }

            if (Level.Munch != null)
            {
                DrawMunch(g, Level.Munch);
            }

            DrawActor(g, Level.Actor);

            UpdateFps();

            if (_debug)
            {
                DrawDebugInfo(g);
            }

            if (_paused)
            {
                DrawPausedIndication(g);
            }

            Invalidate();
        }

        private void DrawPausedIndication(Graphics g)
        {
            g.DrawString("paused", new Font("arial", 12), Brushes.Red, 500, 0);
        }

        private void DrawDebugInfo(Graphics g)
        {
            g.DrawString(_fps.ToString(), new Font("arial", 12), Brushes.White, 0, 0);

            var location = Level.Actor.Location;
            g.DrawString($"({location.X:N4},{location.Y:N4})", new Font("arial", 12), Brushes.White, 40, 0);

            var velocity = Level.Actor.Velocity;
            g.DrawString($"({velocity.X:N4},{velocity.Y:N4})", new Font("arial", 12), Brushes.White, 200, 0);
        }

        private void DrawMunch(Graphics g, Munch munch)
        {
            g.FillEllipse(Brushes.Green, Transform(munch.Box));
        }

        private void DrawGridlines(Graphics g)
        {
            var pen = new Pen(Color.FromArgb(100, 50, 50, 50), 1);
            var font = new Font("arial", 10);

            int MAX_X = Size.Width / DRAW_SCALE;
            int MAX_Y = Size.Height / DRAW_SCALE;

            for (int x = 0; x <= MAX_X; x++)
            {
                g.DrawLine(pen, new Point(TransformX(x), 0), new Point(TransformX(x), Height));

                if (_debug)
                {
                    g.DrawString(x.ToString(), font, Brushes.LightGray, Transform(new Point2(x, 0.2)));
                }
            }

            for (int y = 0; y <= MAX_Y; y++)
            {
                g.DrawLine(pen, new Point(0, TransformY(y)), new Point(Width, TransformY(y)));

                if (_debug)
                {
                    g.DrawString(y.ToString(), font, Brushes.LightGray, Transform(new Point2(0, y + 0.2)));
                }
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

        private Rectangle Transform(Box2 box)
        {
            return new Rectangle(Transform(box.TopLeft), Transform(box.Size));
        }

        private void DrawPlatform(Graphics g, Platform platform)
        {
            switch (platform.Type)
            {
                case PlatformType.CONCRETE:
                    g.FillRectangle(Brushes.DimGray, Transform(platform.Box));
                    break;
                case PlatformType.CONCRETE_POINT:
                    g.FillRectangle(Brushes.Purple, Transform(platform.Box));
                    break;
                case PlatformType.PASSTHROUGH:
                    g.DrawLine(new Pen(Brushes.White, 3), Transform(platform.Box.TopLeft), Transform(platform.Box.TopRight));
                    break;
                case PlatformType.ONEWAY:
                    g.DrawLine(new Pen(Brushes.Red, 3), Transform(platform.Box.TopLeft), Transform(platform.Box.TopRight));
                    break;
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

            if (DateTime.UtcNow > _msgExpiry && !_paused)
            {
                _msgToDisplay = null;
                return false;
            }

            DrawMessageInner(g, _msgToDisplay);

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

            DrawMessageInner(g, _hintToDisplay.Message);
        }

        private void DrawMessageInner(Graphics g, string msg)
        {
            var font = new Font("Courier new", 40);
            g.DrawString(msg, font, Brushes.DarkOrange, new RectangleF(40, 60, Width, DRAW_SCALE * 4));
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

            if (Keyboard.IsKeyDown(Key.Space))
            {
                actor.PrepareForJump();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P)
            {
                PauseUnpause();
                return;
            }

            if (_paused)
            {
                return;
            }

            var actor = Level.Actor;

            switch (e.KeyCode)
            {
                case Keys.Down:
                    actor.Drop();
                    break;
                case Keys.Up:
                    actor.Action();
                    break;
                case Keys.R:
                    RestartLevel();
                    break;
                case Keys.J:
                    if (e.Control && e.Shift)
                    {
                        NextLevel();
                    }
                    break;
                case Keys.D:
                    if (e.Control && e.Shift)
                    {
                        _debug = !_debug;
                        RerenderStaticGraphics();
                    }
                    break;
                case Keys.OemPeriod:
                    if (_debug && e.Control && e.Shift)
                    {
                        Level.Actor.Location += new Vector2(0.05, 0);
                    }
                    break;
                case Keys.Oemcomma:
                    if (_debug && e.Control && e.Shift)
                    {
                        Level.Actor.Location += new Vector2(-0.05, 0);
                    }
                    break;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (_paused)
            {
                return;
            }

            var actor = Level.Actor;
            switch (e.KeyCode)
            {
                case Keys.Space:
                    actor.ReleaseJumpIfPreparing();
                    break;
            }
        }

        private void PauseUnpause()
        {
            _paused = !_paused;
        }
    }
}
