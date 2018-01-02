using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace MonoGameDemo
{
    class InputHandler
    {
        public event EventHandler<InputPressedEventArgs> InputPressedEvent;
        public event EventHandler<InputReleasedEventArgs> InputReleasedEvent;
        private static InputHandler instance;
        private Dictionary<InputAction, Keys> keyboardBindings;
        private Dictionary<InputAction, Buttons> gamePadBindings;
        private KeyboardState previousKeyboardState;
        private GamePadState previousGamePadState;
        private List<PlayerIndex> playerIndexes;

        private InputHandler()
        {
            playerIndexes = new List<PlayerIndex>() { PlayerIndex.One };
            previousKeyboardState = Keyboard.GetState();
            previousGamePadState = GamePad.GetState(playerIndexes[0]);
            SetDefaultBindings();
        }

        public static InputHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputHandler();
                }

                return instance;
            }
        }

        public void Update()
        {
            CheckKeyboard();

            foreach (PlayerIndex playerIndex in playerIndexes)
            {
                if (GamePad.GetState(playerIndex).IsConnected)
                {
                    CheckGamePad(playerIndex);
                }
            }
        }

        private void CheckKeyboard()
        {
            var currentState = Keyboard.GetState();

            foreach (KeyValuePair<InputAction, Keys> keyBinding in keyboardBindings)
            {
                 if (currentState.IsKeyDown(keyBinding.Value) && previousKeyboardState.IsKeyUp(keyBinding.Value))
                {
                    this.InputPressedEvent.Invoke(this, new InputPressedEventArgs(keyBinding.Key));
                }
                else if (currentState.IsKeyUp(keyBinding.Value) && previousKeyboardState.IsKeyDown(keyBinding.Value))
                {
                    this.InputReleasedEvent.Invoke(this, new InputReleasedEventArgs(keyBinding.Key));
                }

            }
            previousKeyboardState = currentState;
        }

        private void CheckGamePad(PlayerIndex playerIndex)
        {
            var currentState = GamePad.GetState(playerIndex);

            foreach (KeyValuePair<InputAction, Buttons> buttonBinding in gamePadBindings)
            {
                if (currentState.IsButtonDown(buttonBinding.Value) && previousGamePadState.IsButtonUp(buttonBinding.Value))
                {
                    this.InputPressedEvent.Invoke(this, new InputPressedEventArgs(buttonBinding.Key));
                }
                else if (currentState.IsButtonUp(buttonBinding.Value) && previousGamePadState.IsButtonDown(buttonBinding.Value))
                {
                    this.InputReleasedEvent.Invoke(this, new InputReleasedEventArgs(buttonBinding.Key));
                }
            }
            previousGamePadState = currentState;
        }

        private void SetDefaultBindings()
        {
            keyboardBindings = new Dictionary<InputAction, Keys>() {
                { InputAction.MovementLeft, Keys.Left },
                { InputAction.MovementRight, Keys.Right },
                { InputAction.MovementJump, Keys.Up },
                { InputAction.ResetLevel, Keys.F5 },
                { InputAction.ToggleMenu, Keys.Enter }
            };

            gamePadBindings = new Dictionary<InputAction, Buttons>() {
                { InputAction.MovementLeft, Buttons.DPadLeft },
                { InputAction.MovementRight, Buttons.DPadRight },
                { InputAction.MovementJump, Buttons.A },
                { InputAction.ResetLevel, Buttons.LeftShoulder },
                { InputAction.ToggleMenu, Buttons.Start }
            };
        }
    }

    public enum InputAction
    {
        MovementLeft,
        MovementRight,
        MovementJump,
        ResetLevel,
        ToggleMenu
    }
}
