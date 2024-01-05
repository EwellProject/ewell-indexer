namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class UserRecordResultDto : PageResult<UserRecordDto>
{
    public UserRecordResultDto(long total, List<UserRecordDto> data) : base(total, data)
    {
    }
}