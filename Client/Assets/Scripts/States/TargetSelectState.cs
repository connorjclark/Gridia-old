using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gridia
{
    public class TargetSelectState : State
    {
        private static KeyCode IntToKeyCode(int i)
        {
            return i + KeyCode.Alpha0;
        }

        private static String KeyCodeToString(KeyCode keyCode)
        {
            if (keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9)
            {
                var num = (int) keyCode - (int) KeyCode.Alpha0;
                return num + "";
            }
            switch (keyCode)
            {
                case KeyCode.Plus:
                    return "+";
                case KeyCode.Minus:
                    return "-";
                default:
                    return keyCode.ToString();
            }
        }

        private static readonly KeyCode[] _selectKeyCodes =
        {
            IntToKeyCode(1), IntToKeyCode(2), IntToKeyCode(3), IntToKeyCode(4),
            IntToKeyCode(5), IntToKeyCode(6), IntToKeyCode(7), IntToKeyCode(8),
            IntToKeyCode(9), IntToKeyCode(0), KeyCode.Minus, KeyCode.Plus,
            KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P
        };

        private readonly GridiaDriver _driver;
        private readonly TileMap _tileMap;
        private readonly GridiaGame _game;
        private Dictionary<KeyCode, Creature> _keyCodeToCreature;

        public TargetSelectState()
        {
            _driver = Locator.Get<GridiaDriver>();
            _tileMap = Locator.Get<TileMap>();
            _game = Locator.Get<GridiaGame>();
        }

        public override void Enter(StateMachine stateMachine)
        {
            _driver.SelectedCreature = null;
            var rangex = _game.View.Width/2;
            var rangey = _game.View.Height/2;
            var creatures = _game.GetCreaturesNearPlayer(rangex, rangey, _selectKeyCodes.Length);
            if (creatures.Count == 0)
            {
                ReturnToIdle(stateMachine);
            }
            else
            {
                _keyCodeToCreature = new Dictionary<KeyCode, Creature>();
                creatures.ForEach(cre =>
                {
                    var keyCode = _selectKeyCodes[_keyCodeToCreature.Count];
                    _keyCodeToCreature[keyCode] = cre;
                });
            }
        }

        public override void Step(StateMachine stateMachine, float dt)
        {
            var keyCodeUp = _keyCodeToCreature.Keys.FirstOrDefault(Input.GetKeyUp);
            if (keyCodeUp != KeyCode.None)
            {
                _driver.SelectedCreature = _keyCodeToCreature[keyCodeUp];
                ReturnToIdle(stateMachine);
            }
            if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.T))
            {
                ReturnToIdle(stateMachine);
            }
        }

        private void DrawKeyCodeOverCreature(Creature creature, KeyCode keyCode)
        {
            var rect = _driver.GetScreenRectOfLocation(creature.Position);
            GridiaConstants.GUIDrawSelector(rect, new Color32(255, 255, 0, 100));
            rect.y -= _driver.tileSize * 0.5f;
            GUI.Box(rect, KeyCodeToString(keyCode));
        }

        public override void OnGUI()
        {
            // draw ...

            foreach (var entry in _keyCodeToCreature)
            {
                DrawKeyCodeOverCreature(entry.Value, entry.Key);
            }

            // on mouse over, select entity ...

            // end state on click ...
        }

        private void ReturnToIdle(StateMachine stateMachine)
        {
            stateMachine.SetState(new IdleState());
        }
    }
}
