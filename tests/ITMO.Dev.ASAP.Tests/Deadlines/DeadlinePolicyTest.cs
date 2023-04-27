using FluentAssertions;
using ITMO.Dev.ASAP.Core.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Core.Deadlines.DeadlinePolicies;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.ValueObject;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Deadlines;

public class DeadlinePolicyTest : TestBase
{
    [Fact]
    public async Task GetDeadlinePenaltyFromSubject()
    {
        SubjectCourse subjectCourse = await Context.SubjectCourses.FirstAsync();

        subjectCourse.DeadlinePolicies.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetDeadlinePenaltyFromPolicy()
    {
        FractionDeadlinePenalty fractionPenalty = new(TimeSpan.FromDays(7), Fraction.FromDenormalizedValue(20));
        AbsoluteDeadlinePenalty absolutePenalty = new(TimeSpan.FromHours(3), new Points(1));

        DeadlinePolicy policy = new(Guid.NewGuid());
        policy.AddDeadlinePenalty(fractionPenalty);
        policy.AddDeadlinePenalty(absolutePenalty);
        Context.DeadlinePolicies.Add(policy);

        policy.DeadlinePenalties.Should().NotBeEmpty();
        await Context.SaveChangesAsync();
    }
}