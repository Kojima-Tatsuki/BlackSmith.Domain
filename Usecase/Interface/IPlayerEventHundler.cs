namespace BlackSmith.Usecase.Interface
{
    public interface IPlayerEventHundler
    {
    }

    public interface IOnPlayerHealthChangedEventHundler : IPlayerEventHundler
    {
        void OnChangedHealthPoint(int currnethelath, int maxHealth);
    }
}
