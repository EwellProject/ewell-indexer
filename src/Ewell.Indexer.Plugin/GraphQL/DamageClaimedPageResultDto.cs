
namespace Ewell.Indexer.Plugin.GraphQL;

public class DamageClaimedPageResultDto : PageResult<DamageClaimedDto>
{
    public DamageClaimedPageResultDto(long total, List<DamageClaimedDto> data) : base(total, data)
    {
    }
}