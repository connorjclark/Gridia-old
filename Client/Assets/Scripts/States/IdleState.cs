using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class IdleState : State
    {
        private int sourceIndex;
        private int mouseDownLocation = -1;
        private GridiaDriver _driver;
        private GridiaGame _game;

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
                    _driver.tabbedGui.ToggleVisiblity(numKeyPressed - 1);
                }
                else 
                {
                    _driver.invGui.SlotSelected = numKeyPressed == 0 ? 9 : numKeyPressed - 1;
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

            var wasdKeysUp = _inputManager.Get4DirectionalInputUp();
            var wasdKeys = _inputManager.Get4DirectionalInput();
            if (wasdKeysUp != Vector3.zero)
            {
                var destinationUp = Locator.Get<GridiaGame>().view.Focus.Position + wasdKeysUp;
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
                var destination = _game.view.Focus.Position + wasdKeys;
                if (Locator.Get<TileMap>().Walkable(destination))
                {
                    Locator.Get<GridiaDriver>().RemoveAllOpenContainers();
                    End(stateMachine, dt, new PlayerMovementState(wasdKeys));
                    return;
                }
            }

            var arrowKeysUp = _inputManager.Get4DirectionalArrowKeysInputUp();
            if (arrowKeysUp != Vector3.zero)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    _driver.invGui.SlotSelectedX += (int)arrowKeysUp.x;
                    _driver.invGui.SlotSelectedY += (int)-arrowKeysUp.y;
                    _driver.invGui.SetWindowNameToCurrentSelection();
                } else if (_driver.SelectedContainer != null)
                {
                    _driver.SelectedContainer.SlotSelectedX += (int)arrowKeysUp.x;
                    _driver.SelectedContainer.SlotSelectedY -= (int)arrowKeysUp.y;
                }
                else
                {
                    _game.SelectorDelta += arrowKeysUp;
                    _game.hideSelector = false;
                }
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _game.PickUpItemAtSelection();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                _game.UseItemAtSelection(_driver.invGui.SlotSelected);
                return;
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
                _driver.invGui.EquipItemAtCurrentSelection();
            }

            if (Input.GetMouseButtonDown(0))
            {
                var openContainerWithMouseDown = _driver.GetOpenContainerWithMouseDown();
                if (openContainerWithMouseDown != null)
                {
                    mouseDownLocation = openContainerWithMouseDown.ContainerId;
                    sourceIndex = openContainerWithMouseDown.MouseDownSlot;
                    _driver.mouseDownItem = _driver.invGui.GetItemAt(openContainerWithMouseDown.MouseDownSlot);
                }
                else if (!_driver.isMouseOverGUI())
                {
                    mouseDownLocation = 0;
                    var downCoord = _driver.getTileLocationOfMouse();
                    _driver.mouseDownItem = _driver._game.tileMap.GetTile((int)downCoord.x, (int)downCoord.y, (int)downCoord.z).Item;
                    sourceIndex = _driver._game.tileMap.ToIndex(downCoord);
                }
            }
            else if (Input.GetMouseButtonUp(0) && mouseDownLocation != -1)
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
                    var tileLocUp = _driver.getTileLocationOfMouse();
                    if (tileLocUp == _driver._game.tileMap.Wrap(_driver._game.view.Focus.Position))
                    {
                        dest = _driver.invGui.ContainerId;
                        destIndex = -1;
                    }
                    else 
                    {
                        dest = 0;
                        destIndex = _driver._game.tileMap.ToIndex(tileLocUp);
                    }
                }

                Locator.Get<ConnectionToGridiaServerHandler>().MoveItem(mouseDownLocation, dest, sourceIndex, destIndex);

                _driver.mouseDownItem =  null;
                mouseDownLocation = -1;
            }
        }

        private void PickUpItemAt(Vector3 loc) 
        {
            var pickupItemLoc = _driver._game.tileMap.Wrap(loc);
            var pickupItemIndex = _driver._game.tileMap.ToIndex(pickupItemLoc);
            Locator.Get<ConnectionToGridiaServerHandler>().MoveItem(0, _driver.invGui.ContainerId, pickupItemIndex, -1);
        }

        private void End(StateMachine stateMachine, float dt, State newState) 
        {
            _driver.mouseDownItem = null;
            stateMachine.SetState(newState);
            stateMachine.Step(dt);
            _driver.mouseDownItem = null;
            _game.hideSelector = true; // :( OnEnd()
        }

        private int GetNumberKeyPressed()
        {
            for (int i = 0; i < 10; i++)
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
