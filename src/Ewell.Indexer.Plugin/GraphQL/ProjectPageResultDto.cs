namespace Ewell.Indexer.Plugin.GraphQL;

public class ProjectPageResultDto : PageResult<ProjectDto>
{
    public ProjectPageResultDto(long total, List<ProjectDto> data) : base(total, data)
    {
    }
}