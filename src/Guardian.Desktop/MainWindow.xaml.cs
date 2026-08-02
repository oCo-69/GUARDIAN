using System.IO;
using System.Net.Http;
using System.Windows;
using Guardian.Application.CandidateDiscovery;
using Guardian.Application.CandidateReview;
using Guardian.Application.CurrentUnderstanding;
using Guardian.Application.EditorialDecision;
using Guardian.Application.Identity;
using Guardian.Application.LibraryObservation;
using Guardian.Application.Persistence;
using Guardian.Domain.Identity;
using Guardian.Infrastructure;
using Guardian.Jellyfin.LibraryObservation;
using Guardian.Providers.Tmdb;

namespace Guardian.Desktop;

public partial class MainWindow : Window
{
    private LibraryObservationResult? observedLibrary;
    private CandidateReviewContext? reviewContext;
    private AcceptEditorialDecisionWorkflow? acceptanceWorkflow;
    private readonly SqliteEditorialMemoryStore memoryStore = new(
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Guardian", "editorial-memory.db"));

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void WindowLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            EditorialMemorySnapshot snapshot = await memoryStore.LoadAsync();
            if (snapshot.Decisions.Count == 0)
            {
                return;
            }

            IdentityCorrespondenceWorkflow restored = IdentityCorrespondenceWorkflow.Restore(
                snapshot.Decisions,
                snapshot.HistoryEvents);
            CurrentUnderstandingExplanation explanation = new ExplainCurrentUnderstandingWorkflow(restored)
                .Explain(snapshot.Decisions[0].SourceWorkId);
            ExplanationTextBox.Text = FormatExplanation(explanation);
            StatusTextBlock.Text = $"Restored {snapshot.Decisions.Count} Decision(s) and " +
                $"{snapshot.HistoryEvents.Count} HistoryEvent(s) from editorial memory.";
        }
        catch (Exception exception)
        {
            SetFailure(exception);
        }
    }

    private async void ObserveLibraryClick(object sender, RoutedEventArgs e)
    {
        try
        {
            using HttpClient client = CreateHttpClient(JellyfinUrlTextBox.Text);
            observedLibrary = await new ObserveLibraryWorkflow(
                new JellyfinLibraryObservationSource(client, EmptyToNull(JellyfinTokenBox.Password)))
                .ObserveAsync();
            SourceWorksList.ItemsSource = observedLibrary.SourceWorks;
            StatusTextBlock.Text = $"Observed {observedLibrary.Count} SourceWork(s). Select one to discover Candidates.";
            DiscoverButton.IsEnabled = observedLibrary.Count > 0;
        }
        catch (Exception exception)
        {
            SetFailure(exception);
        }
    }

    private async void DiscoverCandidatesClick(object sender, RoutedEventArgs e)
    {
        if (SourceWorksList.SelectedItem is not ObservedSourceWork sourceWork)
        {
            StatusTextBlock.Text = "Select a SourceWork first.";
            return;
        }

        try
        {
            using HttpClient client = CreateHttpClient("https://api.themoviedb.org/");
            DiscoverCandidatesResult result = await new DiscoverCandidatesWorkflow(
                new TmdbCandidateDiscoverySource(client, TmdbTokenBox.Password))
                .DiscoverAsync(sourceWork);
            if (result.Status == DiscoverCandidatesStatus.Failed)
            {
                StatusTextBlock.Text = $"Candidate discovery failed: {result.Failure}.";
                return;
            }

            reviewContext = ReviewCandidatesWorkflow.Create(sourceWork, result.Candidates);
            CandidatesList.ItemsSource = reviewContext.Candidates.Select(candidate =>
                new CandidateDisplay(candidate)).ToArray();
            AcceptButton.IsEnabled = reviewContext.Candidates.Count > 0;
            StatusTextBlock.Text = $"Review prepared with {reviewContext.Candidates.Count} Candidate(s). No decision established.";
        }
        catch (Exception exception)
        {
            SetFailure(exception);
        }
    }

    private async void AcceptCandidateClick(object sender, RoutedEventArgs e)
    {
        if (reviewContext is null || CandidatesList.SelectedItem is not CandidateDisplay selected)
        {
            StatusTextBlock.Text = "Select a Candidate first.";
            return;
        }

        try
        {
            acceptanceWorkflow = new AcceptEditorialDecisionWorkflow(reviewContext);
            EditorialDecisionAcceptanceResult result = acceptanceWorkflow.Accept(
                selected.Candidate.Id,
                new EditorialAuthority(ActorTextBox.Text));
            if (!result.ReviewCompleted)
            {
                StatusTextBlock.Text = $"Decision was not established: {result.Validation.Status}.";
                return;
            }

            await memoryStore.SaveAsync(result.Decision!, result.HistoryEvent!);

            CurrentUnderstandingExplanation explanation = acceptanceWorkflow
                .ExplainCurrentUnderstanding(reviewContext.SourceWork.Id);
            ExplanationTextBox.Text = FormatExplanation(explanation);
            StatusTextBlock.Text = "Editorial decision accepted. Current understanding is Known.";
            AcceptButton.IsEnabled = false;
        }
        catch (Exception exception)
        {
            SetFailure(exception);
        }
    }

    private void SourceWorkSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        DiscoverButton.IsEnabled = SourceWorksList.SelectedItem is ObservedSourceWork;
        CandidatesList.ItemsSource = null;
        AcceptButton.IsEnabled = false;
    }

    private void CandidateSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) =>
        AcceptButton.IsEnabled = CandidatesList.SelectedItem is CandidateDisplay;

    private static HttpClient CreateHttpClient(string address)
    {
        if (!Uri.TryCreate(address, UriKind.Absolute, out Uri? uri))
        {
            throw new InvalidOperationException("Enter a valid absolute service URL.");
        }

        return new HttpClient { BaseAddress = uri };
    }

    private static string? EmptyToNull(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    private void SetFailure(Exception exception) =>
        StatusTextBlock.Text = $"Operation failed: {exception.Message}";

    private static string FormatExplanation(CurrentUnderstandingExplanation explanation)
    {
        string knowledge = explanation.IsKnown && explanation.Knowledge.AcceptedWorkId is WorkId workId
            ? $"Known: {explanation.SourceWorkId} → {workId}"
            : "Unknown accepted correspondence.";
        string decision = explanation.SupportingDecision is IdentityDecision supporting
            ? $"Supporting Decision: {supporting.Id} (authority: {supporting.Authority.ActorId})"
            : "Supporting Decision: none";
        return $"{knowledge}\n{decision}\nDecisions recorded: {explanation.Decisions.Count}\nHistoryEvents recorded: {explanation.HistoryEvents.Count}";
    }

    private sealed record CandidateDisplay(IdentityCandidate Candidate)
    {
        public string DisplayText =>
            $"{Candidate.Title ?? Candidate.WorkId.ToString()}" +
            (Candidate.Year is int year ? $" ({year})" : string.Empty) +
            $" — {Candidate.MediaType ?? "Unknown"}";
    }
}
