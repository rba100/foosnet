namespace FoosNet.Network
{
    public interface IFoosPlayer
    {
        // Properties
        string Email { get; }
        string DisplayName { get; }
        Status Status { get; }
    }
}