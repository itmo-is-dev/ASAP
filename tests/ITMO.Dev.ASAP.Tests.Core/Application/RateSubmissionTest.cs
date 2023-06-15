using FluentAssertions;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.ValueObject;
using ITMO.Dev.ASAP.Tests.Core.Extensions;
using ITMO.Dev.ASAP.Tests.Core.Fixtures;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Core.Application;

[Collection(nameof(CoreDatabaseCollectionFixture))]
public class RateSubmissionTest : CoreDatabaseTestBase
{
    public RateSubmissionTest(CoreDatabaseFixture database) : base(database) { }

    [Fact]
    public async Task UpdateSubmission_Should_NoThrow()
    {
        Submission first = await PersistenceContext.GetSubmissionAsync(SubmissionStateKind.Active);

        first.Rate(new Fraction(0.5), Points.None);
        first.State.Kind.Should().Be(SubmissionStateKind.Completed);

        first.UpdatePoints(new Fraction(0.5), Points.None);
        first.State.Kind.Should().Be(SubmissionStateKind.Completed);
    }
}