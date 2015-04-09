using UnityEngine;

namespace Gridia
{
    public class IdleState : State
    {
        private int _sourceIndex;
        private int _mouseDownLocation = -1;
        private readonly GridiaDriver _driver;
        private readonly GridiaGame _game;

        // :(
        public IdleState() 
        {
            _driver = Locator.Get<GridiaDriver>();
            _game = Locator.Get<GridiaGame>();
            _game.SelectorDelta = Vector3.zero;
        }

        public override void Step(StateMachine stateMachine, float dt)
        {
            var numKeyPressed = GetNumberKeyPressed();
            if (numKeyPressed != -1)
            {
                if (Input.GetKey(KeyCode.LeftShift)) 
                {
                    _driver.TabbedGui.ToggleVisiblity(numKeyPressed - 1);
                }
                else 
                {
                    var actionIndex = numKeyPressed == 0 ? 9 : numKeyPressed - 1;
                }
            }

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt)) 
            {
                End(stateMachine, dt, new ItemMovementState(_game.GetSelectorCoord()));
                return;
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                _driver.MoveSelectedContainerToNext();
            }

            var wasdKeysUp = InputManager.Get4DirectionalInputUp();
            var wasdKeys = InputManager.Get4DirectionalInput();
            if (wasdKeysUp != Vector3.zero)
            {
                var destinationUp = Locator.Get<GridiaGame>().View.Focus.Position + wasdKeysUp;
                if (Locator.Get<TileMap>().GetCreatureAt(destinationUp) != null || Locator.Get<TileMap>().GetTile(destinationUp).Floor == 0)
                {
                    Locator.Get<ConnectionToGridiaServerHandler>().Hit(destinationUp);
                }
                else if (Locator.Get<TileMap>().GetTile(destinationUp).Item.Item.Class == Item.ItemClass.Container)
                {
                    Locator.Get<ConnectionToGridiaServerHandler>().ContainerRequest(destinationUp);
                }
            }
            else if (wasdKeys != Vector3.zero)
            {
                var destination = _game.View.Focus.Position + wasdKeys;
                if (Locator.Get<TileMap>().Walkable(destination))
                {
                    Locator.Get<GridiaDriver>().RemoveAllOpenContainers();
                    End(stateMachine, dt, new PlayerMovementState(wasdKeys));
                    return;
                }
            }

            var arrowKeysUp = InputManager.Get4DirectionalArrowKeysInputUp();
            if (arrowKeysUp != Vector3.zero)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    _driver.InvGui.SlotSelectedX += (int)arrowKeysUp.x;
                    _driver.InvGui.SlotSelectedY += (int)-arrowKeysUp.y;
                    _driver.InvGui.SetWindowNameToCurrentSelection();
                } else if (_driver.SelectedContainer != null)
                {
                    _driver.SelectedContainer.SlotSelectedX += (int)arrowKeysUp.x;
                    _driver.SelectedContainer.SlotSelectedY -= (int)arrowKeysUp.y;
                }
                else
                {
                    _game.SelectorDelta += arrowKeysUp;
                    _game.HideSelector = false;
                }
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _game.PickUpItemAtSelection();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                if (_driver.SelectedCreature == null)
                {
                    _game.UseItemAtSelection(_driver.InvGui.SlotSelected);
                    return;
                }
            }
            else if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                _game.UseItemAtSelection(-1);
                return;
            }
            else if (Input.GetKeyUp(KeyCode.Q))
            {
                _game.DropItemAtSelection();
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                _driver.InvGui.EquipItemAtCurrentSelection();
            }

            if (Input.GetMouseButtonDown(0))
            {
                var openContainerWithMouseDown = _driver.GetOpenContainerWithMouseDown();
                if (openContainerWithMouseDown != null)
                {
                    _mouseDownLocation = openContainerWithMouseDown.ContainerId;
                    _sourceIndex = openContainerWithMouseDown.MouseDownSlot;
                    _driver.MouseDownItem = _driver.InvGui.GetItemAt(openContainerWithMouseDown.MouseDownSlot);
                }
                else if (!_driver.IsMouseOverGui())
                {
                    _mouseDownLocation = 0;
                    var downCoord = _driver.GetTileLocationOfMouse();
                    _driver.MouseDownItem = _driver.Game.TileMap.GetTile((int)downCoord.x, (int)downCoord.y, (int)downCoord.z).Item;
                    _sourceIndex = _driver.Game.TileMap.ToIndex(downCoord);
                }
            }
            else if (Input.GetMouseButtonUp(0) && _mouseDownLocation != -1)
            {
                int dest, destIndex;

                var openContainerWithMouseUp = _driver.GetOpenContainerWithMouseUp();
                if (openContainerWithMouseUp != null)
                {
                    dest = openContainerWithMouseUp.ContainerId;
                    destIndex = openContainerWithMouseUp.MouseUpSlot;
                }
                else
                {
                    var tileLocUp = _driver.GetTileLocationOfMouse();
                    if (tileLocUp == _driver.Game.TileMap.Wrap(_driver.Game.View.Focus.Position))
                    {
                        dest = _driver.InvGui.ContainerId;
                        destIndex = -1;
                    }
                    else 
                    {
                        dest = 0;
                        destIndex = _driver.Game.TileMap.ToIndex(tileLocUp);
                    }
                }

                Locator.Get<ConnectionToGridiaServerHandler>().MoveItem(_mouseDownLocation, dest, _sourceIndex, destIndex);

                _driver.MouseDownItem =  null;
                _mouseDownLocation = -1;
            }
        }

        private void End(StateMachine stateMachine, float dt, State newState) 
        {
            _driver.MouseDownItem = null;
            stateMachine.SetState(newState);
            stateMachine.Step(dt);
            _driver.MouseDownItem = null;
            _game.HideSelector = true; // :( OnEnd()
        }

        private int GetNumberKeyPressed()
        {
            for (var i = 0; i < 10; i++)
            {
                if (Input.GetKeyDown("" + i) || Input.GetKeyDown("[" + i + "]"))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
