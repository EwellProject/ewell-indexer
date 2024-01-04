using Volo.Abp.Application.Dtos;

namespace Ewell.Indexer.Plugin.GraphQL;

public class GetProjectDto : PagedResultRequestDto
{
    public string ChainId { get; set; }
    public long StartBlockHeight { get; set; }
    public string WhitelistId { get; set; }
}