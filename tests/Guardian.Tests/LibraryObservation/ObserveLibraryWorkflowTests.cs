using Guardian.Application.LibraryObservation;
using Guardian.Domain.Identity;

namespace Guardian.Tests.LibraryObservation;

public sealed class ObserveLibraryWorkflowTests
{
    [Fact]
    public async Task ObservationEstablishesCompleteStableSourceWorkContext()
    {
        StubLibraryObservationSource source = new(
        [
            new ObservedLibraryWork("library-a", "item-1", "First Series"),
            new ObservedLibraryWork("library-b", "item-2", "Second Movie"),
        ]);

        LibraryObservationResult result = await new ObserveLibraryWorkflow(source).ObserveAsync();

        Assert.Equal(2, result.Count);
        Assert.Collection(
            result.SourceWorks,
            first =>
            {
                Assert.Equal("library-a", first.LibraryId);
                Assert.Equal("item-1", first.ExternalId);
                Assert.Equal("First Series", first.Name);
                Assert.Equal(
                    SourceWorkId.FromStableReference("library-a:item-1"),
                    first.Id);
                Assert.Equal(first.Id, first.SourceWork.Id);
            },
            second => Assert.Equal(
                SourceWorkId.FromStableReference("library-b:item-2"),
                second.Id));
    }

    [Fact]
    public async Task RepeatedObservationUsesTheSameSourceWorkIdentity()
    {
        StubLibraryObservationSource source = new(
        [new ObservedLibraryWork("library-a", "item-1", "First Series")]);
        ObserveLibraryWorkflow workflow = new(source);

        LibraryObservationResult first = await workflow.ObserveAsync();
        LibraryObservationResult second = await workflow.ObserveAsync();

        Assert.Equal(first.SourceWorks.Single().Id, second.SourceWorks.Single().Id);
    }

    [Fact]
    public async Task DuplicateExternalItemsProduceOneSourceWork()
    {
        StubLibraryObservationSource source = new(
        [
            new ObservedLibraryWork("library-a", "item-1", "First Series"),
            new ObservedLibraryWork("library-a", "item-1", "First Series"),
        ]);

        LibraryObservationResult result = await new ObserveLibraryWorkflow(source).ObserveAsync();

        Assert.Single(result.SourceWorks);
    }

    private sealed class StubLibraryObservationSource(
        IReadOnlyList<ObservedLibraryWork> works) : ILibraryObservationSource
    {
        public Task<IReadOnlyList<ObservedLibraryWork>> ReadWorksAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(works);
    }
}
