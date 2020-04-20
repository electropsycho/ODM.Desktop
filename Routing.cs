namespace ODM.UI.WPF
{
    using ReactiveUI;

    /// <summary>
    /// The routing.
    /// </summary>
    public class Routing : IScreen
    {
        public RoutingState Router { get; }

        public Routing()
        {
            this.Router = new RoutingState();
        }
    }
}