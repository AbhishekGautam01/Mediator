namespace Mediator.Sample
{
    public class PrintToConsoleRequest : IRequest<bool>
    {
        public string Text { get; internal set; }
    }
}
