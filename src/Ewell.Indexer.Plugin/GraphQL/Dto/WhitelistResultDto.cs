namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class WhitelistResultDto : PageResult<WhitelistDto>
{
    public WhitelistResultDto(long total, List<WhitelistDto> data) : base(total, data)
    {
    }
}