using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public abstract class LevelBuilder
    {
        public ILevelContext LevelContext { get; private set; }
        private readonly List<Platform> _platforms = new List<Platform>();
        protected DeathTaunts DeathTaunts { get; set; }

        protected IReadOnlyList<Platform> Platforms => _platforms;

        protected Platform Add(Platform platform)
        {
            _platforms.Add(platform);
            return platform;
        }

        public Level Create(ILevelContext levelContext)
        {
            LevelContext = levelContext;

            DeathTaunts = new DeathTaunts(LevelContext);

            Build();

            var level = new Level(LevelContext, _platforms);

            PostBuild(level);

            if (level.Door != null)
            {
                level.Door.Location -= Vector2.X_UNIT * (level.Door.Size - 1) / 4.0;
            }

            level.OnActorDeath += actor => DeathTaunts.NotifyDeath(actor.LastPlatform);

            level.Init();

            return level;
        }

        protected abstract void Build();
        protected virtual void PostBuild(Level level) { }
    }

    public class LevelFactory<T> : ILevelFactory
        where T : LevelBuilder, new()
    {
        public Level Create(ILevelContext levelContext)
        {
            return new T().Create(levelContext);
        }
    }

}
