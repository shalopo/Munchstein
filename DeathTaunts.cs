﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public class DeathTaunts
    {
        public DeathTaunts(ILevelContext levelContext) => _levelContext = levelContext;

        private readonly ILevelContext _levelContext;
        private readonly Dictionary<Platform, string> _taunts = new Dictionary<Platform, string>();
        private string _defaultTaunt;

        public void Add(Platform platform, string msg)
        {
            if (platform == null)
            {
                _defaultTaunt = msg;
            }
            else
            {
                _taunts.Add(platform, msg);
            }
        }

        public void NotifyDeath(Platform platform)
        {
            if (platform == null)
            {
                return;
            }

            if (_taunts.ContainsKey(platform))
            {
                _levelContext.DisplayMessage(_taunts[platform]);
            }
            else if (_defaultTaunt != null)
            {
                _levelContext.DisplayMessage(_defaultTaunt);
            }
        }
    }
}
