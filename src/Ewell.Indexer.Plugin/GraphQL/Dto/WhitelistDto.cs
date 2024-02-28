namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class WhitelistDto : BlockInfoDto
{
    public string Id { get; set; }
    public bool IsAvailable { get; set; }
}