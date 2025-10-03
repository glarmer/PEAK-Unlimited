using System.Text;

namespace PEAKUnlimited.Model.GameInfo
{
    /// <summary>
    /// Gets information about campfire instances. 
    /// </summary>
    public class CampfireInfo : IGameInfo<Campfire>
    {
        /// <inheritdoc/>
        public string GetInfoMessage(Campfire gameInstance)
        {
            var infoStringBuilder = new StringBuilder();

            infoStringBuilder.AppendLine("Campfire Info:");
            infoStringBuilder.AppendLine($"- Next Segment: {gameInstance.advanceToSegment.ToString()}");
            infoStringBuilder.AppendLine($"- Lit: {gameInstance.Lit}");

            return infoStringBuilder.ToString();
        }
    }
}
