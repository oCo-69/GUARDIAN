using Guardian.Application.LibraryObservation;
using Guardian.Domain.Identity;

namespace Guardian.Application.CandidateReview;

public sealed class ReviewCandidatesWorkflow
{
    public static CandidateReviewContext Create(
        ObservedSourceWork sourceWork,
        IEnumerable<IdentityCandidate> candidates)
    {
        ArgumentNullException.ThrowIfNull(sourceWork);
        ArgumentNullException.ThrowIfNull(candidates);

        IdentityCandidate[] reviewCandidates = candidates.ToArray();
        if (reviewCandidates.Any(candidate => candidate.SourceWorkId != sourceWork.Id))
        {
            throw new ArgumentException(
                "All Candidates must concern the reviewed SourceWork.",
                nameof(candidates));
        }

        return new CandidateReviewContext(sourceWork, Array.AsReadOnly(reviewCandidates));
    }
}

public sealed record CandidateReviewContext
{
    private readonly CandidateReviewDecisionState decisionState =
        CandidateReviewDecisionState.NoDecisionEstablished;

    internal CandidateReviewContext(
        ObservedSourceWork sourceWork,
        IReadOnlyList<IdentityCandidate> candidates)
    {
        SourceWork = sourceWork;
        Candidates = candidates;
    }

    public ObservedSourceWork SourceWork { get; }

    public IReadOnlyList<IdentityCandidate> Candidates { get; }

    public CandidateReviewDecisionState DecisionState => decisionState;
}

public enum CandidateReviewDecisionState
{
    NoDecisionEstablished,
}
