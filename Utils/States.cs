namespace ODM.UI.WPF.Utils
{
    //public enum States
    //{
    //    OpenSettinView, 
    //    OpenLoginView, 
    //    OpenMainView,
    //    DoNothing
    //}

    public class StateMessage
    {
        /// <summary>
        /// The states.
        /// </summary>
        private readonly (bool, string) states;

        public StateMessage((bool, string) states)
        {
            this.states = states;
        }

        public bool IsLoggedIn => this.states.Item1;
        public string Message => this.states.Item2;
    }
}