// ///////////////////////////////////////////////////////////////////
// This file is a part of EasyFarm for Final Fantasy XI
// Copyright (C) 2013-2017 Mykezero
// 
// EasyFarm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// EasyFarm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// If not, see <http://www.gnu.org/licenses/>.
// ///////////////////////////////////////////////////////////////////

using System.Linq;
using EasyFarm.Classes;
using EasyFarm.UserSettings;
using MemoryAPI;

namespace EasyFarm.States
{
    /// <summary>
    ///     A class for defeating monsters.
    /// </summary>
    public class BattleState : AgentState
    {
        private readonly RestState _restState;

        public BattleState(StateMemory fface) : base(fface)
        {
            _restState = new RestState(fface);
        }

        public override bool Check()
        {
            if (_restState.Check()) return false;

            // Make sure we don't need trusts
            if (new SummonTrustsState(Memory).Check()) return false;

            // Mobs has not been pulled if pulling moves are available. 
            if (!IsFighting) return false;

            // target null or dead. 
            if (!UnitFilters.MobFilter(EliteApi, Target)) return false;

            // Engage is enabled and we are not engaged. We cannot proceed. 
            if (Config.Instance.IsEngageEnabled) return EliteApi.Player.Status.Equals(Status.Fighting);

            // Engage is not checked, so just proceed to battle. 
            return true;
        }

        public override void Enter()
        {
            Player.Stand(EliteApi);
            EliteApi.Navigator.Reset();
        }

        public override void Run()
        {
            // Cast only one action to prevent blocking curing. 
            var action = Config.Instance.BattleLists["Battle"].Actions
                .FirstOrDefault(x => ActionFilters.TargetedFilter(EliteApi, x, Target));
            if (action == null) return;
            Executor.UseTargetedActions(new[] {action}, Target);
        }
    }
}