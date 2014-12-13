using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    // :(
    public class ItemUsePickState : State
    {
        private ItemUsePickWindow _pickWindow;
        private StateMachine _stateMachine;

        public ItemUsePickState(ItemUsePickWindow pickWindow) 
        {
            _pickWindow = pickWindow;
        }

        public override void Step(StateMachine stateMachine, float dt)
        {
            _stateMachine = stateMachine; // :(
            if (CheckForCancel())
            {
                End();
            }
            else
            {
                var dir = Locator.Get<InputManager>().Get4DirectionalArrowKeysInputUp(); // :(
                if (dir != Vector3.zero)
                {
                    _pickWindow.Picks.TileSelectedX += (int)dir.x;
                    _pickWindow.Picks.TileSelectedY += (int)-dir.y;
                    _pickWindow.SetWindowNameToCurrentSelection();
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    Locator.Get<ItemUsePickWindow>().SelectUse();
                    End();
                }
            }
        }

        private bool CheckForCancel() 
        {
            return Locator.Get<InputManager>().Get4DirectionalInput() != Vector3.zero || Input.GetKey(KeyCode.Escape);
        }

        public void End()
        {
            Locator.Get<TabbedUI>().Remove(_pickWindow);
            _stateMachine.SetState(new IdleState());
        }
    }
}
