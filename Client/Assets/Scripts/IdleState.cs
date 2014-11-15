using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class IdleState : State
    {
        private int downSlot;
        private int sourceIndex;
        private String mouseDownLocation;
        private GridiaDriver _driver;

        // :(
        public IdleState() 
        {
            _driver = Locator.Get<GridiaDriver>();
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
                    _driver.invGui.SlotSelected = numKeyPressed - 1;
                }
            }

            if (Input.GetKey(KeyCode.Space)) 
            {
                End(stateMachine, dt, new ItemUseState("inv", _driver.invGui.SlotSelected));
                return;
            }

            if (ProcessDirectionalInput() != Vector3.zero)
            {
                End(stateMachine, dt, new PlayerMovementState());
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (_driver.invGui.MouseDownSlot != -1)
                {
                    mouseDownLocation = "inv";
                    sourceIndex = _driver.invGui.MouseDownSlot;
                    _driver.mouseDownItem = _driver.invGui.GetItemAt(_driver.invGui.MouseDownSlot);
                }
                else if (!_driver.isMouseOverGUI())
                {
                    mouseDownLocation = "world";
                    var downCoord = _driver.getTileLocationOfMouse();
                    _driver.mouseDownItem = _driver._game.tileMap.GetTile((int)downCoord.x, (int)downCoord.y, (int)downCoord.z).Item;
                    sourceIndex = _driver._game.tileMap.ToIndex(downCoord);
                }
            }
            else if (Input.GetMouseButtonUp(0) && mouseDownLocation != null)
            {
                String dest;
                int destIndex;
                if (_driver.invGui.MouseUpSlot != -1)
                {
                    dest = "inv";
                    destIndex = _driver.invGui.MouseUpSlot;
                }
                else
                {
                    var tileLocUp = _driver.getTileLocationOfMouse();
                    if (tileLocUp == _driver._game.view.Focus.Position)
                    {
                        dest = "inv";
                        destIndex = -1;
                    }
                    else 
                    {
                        dest = "world";
                        destIndex = _driver._game.tileMap.ToIndex(tileLocUp);
                    }
                }

                Locator.Get<ConnectionToGridiaServerHandler>().MoveItem(mouseDownLocation, dest, sourceIndex, destIndex);

                _driver.mouseDownItem =  null;
                mouseDownLocation = null;
            }
        }

        private void End(StateMachine stateMachine, float dt, State newState) 
        {
            _driver.mouseDownItem = null;
            stateMachine.SetState(newState);
            stateMachine.Step(dt);
            _driver.mouseDownItem = null;
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
