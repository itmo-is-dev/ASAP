using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Models;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using ITMO.Dev.ASAP.WebUI.Markup.SubjectCourse.Components.Assignments;
using ITMO.Dev.ASAP.WebUI.Markup.SubjectCourse.Components.Groups;
using ITMO.Dev.ASAP.WebUI.Markup.SubjectCourse.Components.Queue;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Web;

namespace ITMO.Dev.ASAP.WebUI.Markup.SubjectCourse;

public partial class SubjectCoursePage
{
    private SubjectCourseDto? _course;

    private ICollection<AssignmentDto>? _assignments;
    private ICollection<ExtendedSubjectCourseGroupDto>? _subjectCourseGroups;

    private string? _selectedTab;

    [Parameter]
    public Guid SubjectCourseId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await using ISafeExecutionBuilder<SubjectCourseDto> builder = SafeExecutor
            .Execute(() => SubjectCourseClient.GetAsync(SubjectCourseId));

        builder.Title = "Failed to load subject course";
        builder.OnSuccessAsync(OnSubjectCourseChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        Uri uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        NameValueCollection query = HttpUtility.ParseQueryString(uri.Query);

        string? selected = query.Get("selected");
        await OnSelectedTabChanged(selected);
    }

    private async Task OnSubjectCourseChanged(SubjectCourseDto course)
    {
        _course = course;
        SubjectCourseProvider.OnNext(course.Id);
        SubjectProvider.OnNext(course.SubjectId);

        _assignments = null;
        _subjectCourseGroups = null;

        string? selected = _selectedTab;
        _selectedTab = null;

        await OnSelectedTabChanged(selected);
    }

    private Task AddAssignmentAsync(AssignmentDto assignment)
    {
        _assignments?.Add(assignment);
        return Task.CompletedTask;
    }

    private async Task AddSubjectCourseGroupsAsync(IReadOnlyCollection<SubjectCourseGroupDto> groups)
    {
        ICollection<ExtendedSubjectCourseGroupDto> extended = await MapToExtendedSubjectCourseGroupAsync(groups);

        _subjectCourseGroups ??= new List<ExtendedSubjectCourseGroupDto>();

        foreach (ExtendedSubjectCourseGroupDto group in extended)
        {
            _subjectCourseGroups?.Add(group);
        }
    }

    private Task OnSelectedTabChanged(string? arg)
    {
        if (_selectedTab == arg)
        {
            NavigationManager.NavigateTo($"/adminpanel/courses/{SubjectCourseId}");
            _selectedTab = null;

            return Task.CompletedTask;
        }

        _selectedTab = arg;

        return _selectedTab switch
        {
            AssignmentsTabComponent.Key => ProcessAssignmentsSelected(),
            GroupsTabComponent.Key => ProcessGroupsSelected(),
            SubmissionQueueTabComponent.Key => ProcessQueueSelected(),
            _ => Task.CompletedTask,
        };
    }

    private async Task ProcessAssignmentsSelected()
    {
        NavigationManager.NavigateTo($"/adminpanel/courses/{SubjectCourseId}?selected={AssignmentsTabComponent.Key}");

        if (_course is null || _assignments is not null)
            return;

        await using ISafeExecutionBuilder builder = SafeExecutor.Execute(async () =>
        {
            IReadOnlyCollection<AssignmentDto> assignments = await SubjectCourseClient.GetAssignmentsAsync(_course.Id);
            _assignments = assignments.ToList();
        });
    }

    private async Task ProcessGroupsSelected()
    {
        NavigationManager.NavigateTo($"/adminpanel/courses/{SubjectCourseId}?selected={GroupsTabComponent.Key}");

        if (_course is null || _subjectCourseGroups is not null)
            return;

        await using ISafeExecutionBuilder<IReadOnlyCollection<SubjectCourseGroupDto>> builder = SafeExecutor
            .Execute(async () => await SubjectCourseClient.GetGroupsAsync(_course.Id));

        builder.OnSuccessAsync(OnSubjectCourseGroupsLoadedAsync);
    }

    private async Task ProcessQueueSelected()
    {
        string uri = $"/adminpanel/courses/{SubjectCourseId}?selected={SubmissionQueueTabComponent.Key}";
        NavigationManager.NavigateTo(uri);

        if (_course is null || _subjectCourseGroups is not null)
            return;

        await using ISafeExecutionBuilder<IReadOnlyCollection<SubjectCourseGroupDto>> builder = SafeExecutor
            .Execute(async () => await SubjectCourseClient.GetGroupsAsync(_course.Id));

        builder.OnSuccessAsync(OnSubjectCourseGroupsLoadedAsync);
    }

    private async Task ShowAddSubjectCourseGroupsAsync()
    {
        if (_addSubjectCourseGroupModal is null)
            return;

        await ProcessGroupsSelected();
        await _addSubjectCourseGroupModal.ShowAsync();
    }

    private async Task OnSubjectCourseGroupsLoadedAsync(IReadOnlyCollection<SubjectCourseGroupDto> groups)
    {
        _subjectCourseGroups = await MapToExtendedSubjectCourseGroupAsync(groups);
    }

    private async Task<ICollection<ExtendedSubjectCourseGroupDto>> MapToExtendedSubjectCourseGroupAsync(
        IReadOnlyCollection<SubjectCourseGroupDto> groups)
    {
        IEnumerable<Guid> groupIds = groups.Select(x => x.StudentGroupId);
        IReadOnlyCollection<StudyGroupDto> studyGroups = await StudyGroupClient.GetAsync(groupIds);

        return groups
            .Join(
                studyGroups,
                x => x.StudentGroupId,
                x => x.Id,
                (courseGroup, group) => (courseGroup, group))
            .Select(pair => new ExtendedSubjectCourseGroupDto(pair.courseGroup, pair.group))
            .ToArray();
    }
}