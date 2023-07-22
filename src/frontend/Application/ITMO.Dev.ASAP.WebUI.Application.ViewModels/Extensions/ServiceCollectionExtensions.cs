using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.GroupAssignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Navigation;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.StudentGroups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Students;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Groups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.SubjectCourses.Queues;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Events.Subjects;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.StudentGroups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Assignments;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Groups;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses.Queues;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.Subjects;
using ITMO.Dev.ASAP.WebUI.Application.ViewModels.Messaging;
using ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.Assignments;
using ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.StudentGroups;
using ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses.Assignments;
using ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses.Groups;
using ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.SubjectCourses.Queues;
using ITMO.Dev.ASAP.WebUI.Application.ViewModels.Structure.Subjects;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection collection)
    {
        collection.AddSingleton<IMessageProvider, MessageProvider>();
        collection.AddSingleton<IMessagePublisher, MessagePublisher>();

        collection.AddAssignmentsMessageStreams();
        collection.AddGroupAssignmentsMessageStreams();
        collection.AddNavigationMessageStream();
        collection.AddStudentGroupsMessageStreams();
        collection.AddStudentMessageStreams();
        collection.AddSubjectCoursesMessageStreams();
        collection.AddSubjectMessageStreams();

        collection.AddAssignmentsViewModels();
        collection.AddStudentGroupViewModels();
        collection.AddSubjectCourseViewModels();
        collection.AddSubjectViewModels();

        return collection;
    }

    private static void AddReplayMessageStream<T>(this IServiceCollection collection)
    {
        var subject = new ReplaySubject<T>(1);

        collection.AddSingleton(subject.AsObservable());
        collection.AddSingleton(subject.AsObserver());
    }

    private static void AddMessageStream<T>(this IServiceCollection collection)
    {
        var subject = new Subject<T>();

        collection.AddSingleton(subject.AsObservable());
        collection.AddSingleton(subject.AsObserver());
    }

    private static void AddAssignmentsMessageStreams(this IServiceCollection collection)
    {
        collection.AddReplayMessageStream<AssigmentSelectedEvent>();
        collection.AddMessageStream<AssignmentCreatedEvent>();
        collection.AddMessageStream<AssignmentUpdatedEvent>();
        collection.AddMessageStream<AssignmentVisibleChangedEvent>();
        collection.AddMessageStream<CurrentAssignmentLoadedEvent>();
        collection.AddMessageStream<GroupAssignmentsListUpdatedEvent>();
    }

    private static void AddAssignmentsViewModels(this IServiceCollection collection)
    {
        collection.AddScoped<IAssignment, Assignment>();
        collection.AddScoped<GroupAssignmentFactory>();
    }

    private static void AddGroupAssignmentsMessageStreams(this IServiceCollection collection)
    {
        collection.AddMessageStream<GroupAssignmentUpdatedEvent>();
    }

    private static void AddNavigationMessageStream(this IServiceCollection collection)
    {
        collection.AddMessageStream<NavigatedToGlobalPageEvent>();
        collection.AddMessageStream<NavigatedToGroupsPageEvent>();
        collection.AddMessageStream<NavigatedToSettingsPageEvent>();
        collection.AddMessageStream<NavigatedToStudentsPageEvent>();
        collection.AddMessageStream<NavigatedToSubjectsPageEvent>();
        collection.AddMessageStream<NavigatedToUsersPageEvent>();
    }

    private static void AddStudentGroupsMessageStreams(this IServiceCollection collection)
    {
        collection.AddReplayMessageStream<StudentGroupSelectedEvent>();
        collection.AddMessageStream<StudentGroupStudentsUpdatedEvent>();
        collection.AddMessageStream<StudentGroupUpdatedEvent>();
    }

    private static void AddStudentGroupViewModels(this IServiceCollection collection)
    {
        collection.AddScoped<IStudentGroup, StudentGroup>();
    }

    private static void AddStudentMessageStreams(this IServiceCollection collection)
    {
        collection.AddMessageStream<StudentTransferredEvent>();
    }

    private static void AddSubjectCoursesMessageStreams(this IServiceCollection collection)
    {
        collection.AddReplayMessageStream<CurrentSubjectCourseLoadedEvent>();
        collection.AddMessageStream<SubjectCourseCreatedEvent>();
        collection.AddMessageStream<SubjectCourseListUpdatedEvent>();
        collection.AddMessageStream<SubjectCourseGroupListUpdatedEvent>();
        collection.AddReplayMessageStream<SubjectCourseSelectedEvent>();
        collection.AddReplayMessageStream<SubjectCourseSelectionUpdatedEvent>();
        collection.AddMessageStream<SubjectCourseUpdatedEvent>();

        // Adding assignments
        collection.AddMessageStream<SubjectCourseAssignmentListUpdatedEvent>();

        // Adding groups
        collection.AddMessageStream<AddSubjectCourseGroupsVisibleEvent>();
        collection.AddMessageStream<SubjectCourseGroupListUpdatedEvent>();
        collection.AddMessageStream<SubjectCourseGroupUpdatedEvent>();

        // Adding queues
        collection.AddMessageStream<SubjectCourseQueueListUpdatedEvent>();
        collection.AddReplayMessageStream<SubjectCourseQueueLoadedEvent>();
        collection.AddReplayMessageStream<SubjectCourseQueueSelectedEvent>();
    }

    private static void AddSubjectCourseViewModels(this IServiceCollection collection)
    {
        collection.AddScoped<ISubjectCourse, SubjectCourseViewModel>();
        collection.AddScoped<ISubjectCourseList, SubjectCourseList>();

        collection.AddScoped<ISubjectCourseAssignmentList, SubjectCourseAssignmentList>();

        collection.AddScoped<ISubjectCourseGroupList, SubjectCourseGroupList>();

        collection.AddScoped<ISubjectCourseQueue, SubjectCourseQueue>();
        collection.AddScoped<ISubjectCourseQueueList, SubjectCourseQueueList>();
    }

    private static void AddSubjectMessageStreams(this IServiceCollection collection)
    {
        collection.AddReplayMessageStream<CurrentSubjectLoadedEvent>();
        collection.AddMessageStream<SubjectCreatedEvent>();
        collection.AddMessageStream<SubjectListUpdatedEvent>();
        collection.AddReplayMessageStream<SubjectSelectedEvent>();
        collection.AddMessageStream<SubjectUpdatedEvent>();
    }

    private static void AddSubjectViewModels(this IServiceCollection collection)
    {
        collection.AddScoped<ISubjectManager, SubjectManager>();
        collection.AddScoped<ISubjectList, SubjectList>();
    }
}