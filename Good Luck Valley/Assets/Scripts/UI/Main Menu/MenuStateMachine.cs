namespace GoodLuckValley.UI.MainMenu
{
    public class MenuStateMachine
    {
        #region PROPERTIES
        public MenuState PreviousState { get; private set; }
        public MenuState CurrentState { get; private set; }
        #endregion

        public void Initialize(MenuState startingState)
        {
            CurrentState = startingState;
            CurrentState.OnEnter();
        }

        public void ChangeState(MenuState newState)
        {
            // Set previous state
            PreviousState = CurrentState;

            // Exit the current state
            CurrentState.OnExit();

            // Set the new state and enter it
            CurrentState = newState;
            CurrentState.OnEnter();
        }
    }
}