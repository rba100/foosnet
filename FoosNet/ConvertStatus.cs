using System;
using System.Globalization;
using System.Windows.Data;
using FoosNet.Network;

namespace FoosNet
{
    class ConvertStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Status status = (Status)value;
            switch (status)
            {
                case Status.Available:
                    return "Images/PlayerAvailable.png";
                case Status.Busy:
                    return "Images/AmberOrb.ico";
                case Status.Offline:
                    return "Images/RedOrb.ico";
                case Status.Unknown:
                    return "Images/BlueOrb.ico";
                case Status.Useless:
                    return "Images/BlueOrb.ico";
                default:
                    throw new Exception("Set new status!");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
