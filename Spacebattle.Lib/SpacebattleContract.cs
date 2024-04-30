
using System.Runtime.Serialization;
using CoreWCF.OpenApi.Attributes;

namespace Spacebattle.Lib
{
    [DataContract(Name = "SpacebattleContract")]
    public class SpacebattleContract
    {

        [DataMember(Name = "type", Order = 1)]
        [OpenApiProperty(Description = "Command type description.")]
        public required string CommandType { get; set; }

        [DataMember(Name = "game id", Order = 2)]
        [OpenApiProperty(Description = "Game id description.")]
        public required string GameId { get; set; }

        [DataMember(Name = "game item id", Order = 3)]
        [OpenApiProperty(Description = "Game item id description.")]
        public required int GameItemId { get; set; }

        [DataMember(Name = "params", Order = 4)]
        [OpenApiProperty(Description = "Params description.")]
        public required IDictionary<string, object> GameParameters { get; set; }
    }

}
