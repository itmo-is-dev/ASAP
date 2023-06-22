using ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Models;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;
using ITMO.Dev.ASAP.WebUI.Markup.Structure.SubjectCourses.Assignments;
using ITMO.Dev.ASAP.WebUI.Markup.Structure.SubjectCourses.Groups;
using ITMO.Dev.ASAP.WebUI.Markup.Structure.SubjectCourses.Queues;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Web;

namespace ITMO.Dev.ASAP.WebUI.Markup.Structure.SubjectCourses;

public partial class SubjectCoursePage : IDisposable
{
    private IDisposable? _subscription;

    private SubjectCourseDto? _course;

    private string? _selectedTab;

    [Parameter]
    public Guid SubjectCourseId { get; set; }

    private string? GithubOrganizationName => _course?.Associations
        .OfType<GithubSubjectCourseAssociationDto>()
        .FirstOrDefault()
        ?
        .GithubOrganizationName;

    public void Dispose()
    {
        _subscription?.Dispose();
    }

    protected override async Task OnParametersSetAsync()
    {
        _subscription = new SubscriptionBuilder()
            .Subscribe(ViewModel.SubjectCourse.Subscribe(x =>
            {
                _course = x;
                StateHasChanged();
            }))
            .Subscribe(ViewModel.Selection.Subscribe(OnSelectionUpdated))
            .Build();

        await ViewModel.SelectSubjectCourseAsync(SubjectCourseId);
    }

    protected override void OnInitialized()
    {
        Uri uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        NameValueCollection query = HttpUtility.ParseQueryString(uri.Query);

        string? selected = query.Get("selected");
        OnTabSelected(selected);
    }

    private void OnTabSelected(string? arg)
    {
        if (_selectedTab == arg)
        {
            ViewModel.SelectTab(SubjectCourseSelection.None);
            _selectedTab = null;

            return;
        }

        _selectedTab = arg;

        SubjectCourseSelection selection = _selectedTab switch
        {
            AssignmentsTabComponent.Key => SubjectCourseSelection.Assignments,
            GroupsTabComponent.Key => SubjectCourseSelection.Groups,
            SubmissionQueueTabComponent.Key => SubjectCourseSelection.Queues,
            _ => SubjectCourseSelection.None,
        };

        ViewModel.SelectTab(selection);
    }

    private void OnSelectionUpdated(SubjectCourseSelection selection)
    {
        string uri = selection switch
        {
            SubjectCourseSelection.Assignments
                => $"/adminpanel/courses/{SubjectCourseId}?selected={AssignmentsTabComponent.Key}",

            SubjectCourseSelection.Groups
                => $"/adminpanel/courses/{SubjectCourseId}?selected={GroupsTabComponent.Key}",

            SubjectCourseSelection.Queues
                => $"/adminpanel/courses/{SubjectCourseId}?selected={SubmissionQueueTabComponent.Key}",

            SubjectCourseSelection.None or _ => $"/adminpanel/courses/{SubjectCourseId}",
        };

        NavigationManager.NavigateTo(uri);
    }
}