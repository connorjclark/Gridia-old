namespace Gridia
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    public class TargetSelectState : State
    {
        #region Fields

        private static readonly KeyCode[] _selectKeyCodes = 
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
            KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8,
            KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Minus, KeyCode.Plus,
            KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P
        };

        private readonly GridiaDriver _driver;
        private readonly GridiaGame _game;
        private readonly TileMap _tileMap;

        private Dictionary<KeyCode, Creature> _keyCodeToCreature;

        #endregion Fields

        #region Constructors

        public TargetSelectState()
        {
            _driver = Locator.Get<GridiaDriver>();
            _tileMap = Locator.Get<TileMap>();
            _game = Locator.Get<GridiaGame>();
        }

        #endregion Constructors

        #region Methods

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

        public override void OnGUI()
        {
            if (_keyCodeToCreature == null) return;
            // draw ...

            foreach (var entry in _keyCodeToCreature)
            {
                DrawKeyCodeOverCreature(entry.Value, entry.Key);
            }

            // on mouse over, select entity ...

            // end state on click ...
        }

        public override void Step(StateMachine stateMachine, float dt)
        {
            var keyCodeUp = _keyCodeToCreature.Keys.FirstOrDefault(Input.GetKeyUp);
            if (keyCodeUp != KeyCode.None)
            {
                _driver.SelectedCreature = _keyCodeToCreature[keyCodeUp];
                ReturnToIdle(stateMachine);
            }
            if (CheckForCancel())
            {
                ReturnToIdle(stateMachine);
            }
        }

        private bool CheckForCancel()
        {
            return Locator.Get<InputManager>().Get4DirectionalWasdInput() != Vector3.zero
                   || Input.GetKey(KeyCode.Escape)
                   || Input.GetKeyUp(KeyCode.T);
        }

        private void DrawKeyCodeOverCreature(Creature creature, KeyCode keyCode)
        {
            var rect = _driver.GetScreenRectOfLocation(creature.Position);
            GridiaConstants.GUIDrawSelector(rect, new Color32(255, 255, 0, 100));
            rect.y -= _driver.tileSize * 0.5f;
            GUI.color = Color.white;
            GUI.Box(rect, keyCode.ToShortString());
        }

        private void ReturnToIdle(StateMachine stateMachine)
        {
            stateMachine.SetState(new IdleState());
        }

        #endregion Methods
    }
}