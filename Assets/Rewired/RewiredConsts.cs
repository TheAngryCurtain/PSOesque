/* Rewired Constants
   This list was generated on 04/07/2018 16:13:12
   The list applies to only the Rewired Input Manager from which it was generated.
   If you use a different Rewired Input Manager, you will have to generate a new list.
   If you make changes to the exported items in the Rewired Input Manager, you will need to regenerate this list.
*/

namespace RewiredConsts {
    public static class Action {
        // Default
        // UI
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "Navigate Horizontal")]
        public const int Navigate_Horizontal = 5;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "Navigate Vertical")]
        public const int Navigate_Vertical = 6;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "Confirm")]
        public const int Confirm = 7;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "Cancel")]
        public const int Cancel = 8;
        // Gameplay
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Gameplay", friendlyName = "Move Horizontal")]
        public const int Move_Horizontal = 0;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Gameplay", friendlyName = "Move Vertical")]
        public const int Move_Vertical = 1;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Gameplay", friendlyName = "Camera Horizontal")]
        public const int Camera_Horizontal = 2;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Gameplay", friendlyName = "Camera Vertical")]
        public const int Camera_Vertical = 3;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Gameplay", friendlyName = "Interact")]
        public const int Interact = 4;
    }
    public static class Category {
        public const int Default = 0;
    }
    public static class Layout {
        public static class Joystick {
            public const int Default = 0;
        }
        public static class Keyboard {
            public const int Default = 0;
        }
        public static class Mouse {
            public const int Default = 0;
        }
        public static class CustomController {
            public const int Default = 0;
        }
    }
}
