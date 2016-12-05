using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace NodeJS_XNA {

    public class KeyboardManager {

        public KeyboardState newState, oldState;

        public void Update() {
            oldState = newState;
            newState = Keyboard.GetState();
        }

        public bool press(Keys k) {
            bool isPressed = false;
            if (oldState.IsKeyDown(k) && newState.IsKeyUp(k)) isPressed = true;
            return isPressed;
        }

        public bool hold(Keys k)
        {
            bool isPressed = false;
            if (oldState.IsKeyDown(k) && newState.IsKeyDown(k)) isPressed = true;
            return isPressed;
        }

        public bool down(Keys k)
        {
            bool isPressed = false;
            if (newState.IsKeyDown(k)) isPressed = true;
            return isPressed;
        }
    }
}
