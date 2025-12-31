public interface IGameRepository
{
    void Initialize();
    void SaveGameResult(string userId, string seed, int moves, double playTime);
}