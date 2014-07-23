using System;
using System.Text;
using FoosNet.Network;

namespace FoosNet.PlayerFilters
{
    public class DefaultNameTransformation : IPlayerTransformation
    {
        public IFoosPlayer Process(IFoosPlayer player)
        {
            var emailParts = player.Email.Split(new[] {'@'}, StringSplitOptions.RemoveEmptyEntries);
            if (emailParts.Length > 0)
            {
                var nameParts = emailParts[0].Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
                var sb = new StringBuilder();
                for (int i = 0; i < nameParts.Length; i++)
                {
                    if (i != 0) sb.Append(" ");
                    sb.Append(char.ToUpper(nameParts[i][0]));
                    if(nameParts[i].Length>1) sb.Append(nameParts[i].Substring(1));
                }
                player.DisplayName = sb.ToString();
            }
            return player;
        }
    }

    public class StatusToUnknownTransformation : IPlayerTransformation
    {
        public IFoosPlayer Process(IFoosPlayer player)
        {
            player.Status = Status.Unknown;
            return player;
        }
    }
}
