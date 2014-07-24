using System.ComponentModel;

namespace FoosNet.Network
{
    public interface IFoosPlayer : INotifyPropertyChanged
    {
        string Email { get; set; }
        string DisplayName { get; set; }
        Status Status { get; set; }
    }
}
