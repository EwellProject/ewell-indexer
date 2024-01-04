
namespace Ewell.Indexer.Plugin.GraphQL;

public class WhitelistPageResultDto : PageResult<WhitelistDto>
{
    public WhitelistPageResultDto(long total, List<WhitelistDto> data) : base(total, data)
    {
    }
}