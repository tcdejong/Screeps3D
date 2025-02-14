﻿using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D.Rooms.Views
{
    internal class RoadView : MonoBehaviour
    {
        private int _x;
        private int _y;
        private RoadNetworkView _roadNetworkView;
        private Dictionary<string, Renderer> _offshoots;

        public void Init(RoadNetworkView roadNetworkView, int x, int y)
        {
            if (_offshoots == null)
            {
                _offshoots = new Dictionary<string, Renderer>();
                foreach (var renderer in GetComponentsInChildren<Renderer>())
                {
                    _offshoots[renderer.gameObject.name] = renderer;
                }
            }

            this._x = x;
            this._y = y;
            this._roadNetworkView = roadNetworkView;
            CheckNeighbors();
        }

        private void CheckNeighbors()
        {
            var foundOffshoot = false;
            for (var xDelta = -1; xDelta <= 1; xDelta++)
            {
                for (var yDelta = -1; yDelta <= 1; yDelta++)
                {
                    var rx = _x + xDelta;
                    var ry = _y + yDelta;
                    if (xDelta == 0 && yDelta == 0)
                        continue;
                    if (_roadNetworkView.roads[rx, ry] == null)
                        continue;
                    var key = xDelta.ToString() + yDelta.ToString();
                    _offshoots[key].enabled = true;
                    foundOffshoot = true;
                }
            }
            if (!foundOffshoot)
            {
                _offshoots["base"].enabled = true;
            }
        }
    }
}