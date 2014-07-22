namespace FoosNet.Network
{
    public interface IPlayerTransformation
    {
        IFoosPlayer Process(IFoosPlayer player);
    }
}
