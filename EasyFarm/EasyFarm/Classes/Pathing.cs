﻿using System;
using System.Linq;
using System.Windows.Threading;
using EasyFarm.Engine;
using FFACETools;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace EasyFarm.PathingTools
{
    public class Pathing
    {
        #region Members
        private static DispatcherTimer PathMonitor = null;
        private static FFACE.Position LastPosition = null;
        private static GameEngine Engine = null;
        #endregion

        #region Contructors
        private Pathing()
        {
            PathMonitor = new DispatcherTimer();
            LastPosition = new FFACE.Position();
        }

        public Pathing(ref GameEngine GameEngine)
            : this()
        {
            Engine = GameEngine;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Returns the nearest point to us on the waypoint
        /// route. It may return null if out of range.
        /// </summary>
        /// <param name="Route"></param>
        /// <returns></returns>
        public FFACE.Position GetNearestPoint()
        {
            return Engine.Config.Waypoints.Where(x => Engine.FFInstance.Instance.Navigator.DistanceTo(x) < 50).Min();
        }

        /// <summary>
        /// Returns the path for the player to run.
        /// It may be reversed depending on if the player
        /// is nearing the end of the path.
        /// 
        /// Alters the internal list of waypoints!
        /// </summary>
        /// <returns></returns>
        public IList<FFACE.Position> GetPath()
        {
            if (Engine.Config.Waypoints.Count <= 0)
            {
                return Engine.Config.Waypoints;
            }

            if (Engine.FFInstance.Instance.Navigator.DistanceTo(Engine.Config.Waypoints.Last()) < 5)
            {
                return Engine.Config.Waypoints.Reverse().ToArray();
            }

            return Engine.Config.Waypoints;
        }

        /// <summary>
        /// Moves player to specified waypoint
        /// </summary>
        /// <param name="position"></param>
        public void GotoWaypoint(FFACE.Position position)
        {
            Engine.FFInstance.Instance.Navigator.Goto(position, false);
        }

        /// <summary>
        /// Clears all waypoints
        /// </summary>
        public void ClearWaypoints()
        {
            Engine.Config.Waypoints.Clear();
        }

        /// <summary>
        /// Adds a waypoint to the current
        /// waypoint list.
        /// </summary>
        public void AddWaypoint()
        {
            FFACE.Position Current = Engine.FFInstance.Instance.Player.Position;

            if (!LastPosition.Equals(Current))
            {
                Engine.Config.Waypoints.Add(Current);
                LastPosition = Current;
            }
        }
        #endregion

        public IList<FFACE.Position> GetRemainingPath()
        {
            FFACE.Position NearestPoint = GetNearestPoint();

            if (NearestPoint == null)
            {
                return new FFACE.Position[0];
            }

            return Engine.Config.Waypoints.SkipWhile(element => element != NearestPoint).ToArray();
        }

        public double Distance(FFACE.Position A, FFACE.Position B)
        {
            double Distance =
                Math.Sqrt(
                Math.Pow((A.X - B.X), 2) +
                Math.Pow((A.Z - B.Z), 2)
            );

            return Distance;
        }
    }
}