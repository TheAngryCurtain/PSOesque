namespace UI
{
    namespace Enums
    {
        public enum ScreenId
        {
            None = -1,
            Company,
            Legal,
            Title,
            Loading,
			Lobby,
            HUD,
            CollectionMiniGame
        }

        public enum UIScreenAnimState
        {
            None = -1,
            Intro,
            Outro
        }

        public enum UIScreenAnimEvent
        {
            None = -1,
            Start,
            End
        }

        public enum UINavigationType
        {
            None = -1,
            Advance,
            Back
        }
    }
}
