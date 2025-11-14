namespace BlackSmith.Domain.Networking.Lobby
{
    /// <summary>
    /// ロビー名を表す値オブジェクト
    /// </summary>
    public record LobbyName
    {
        public string Value { get; }

        public LobbyName(string value)
        {
            Value = value;
        }
    }
}