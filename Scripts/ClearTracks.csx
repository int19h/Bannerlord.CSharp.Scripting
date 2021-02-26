/// <summary>
/// Removes all tracks.
/// </summary>
void ClearTracks() {
    Campaign.Current.RemoveTracks(_ => true);
}
