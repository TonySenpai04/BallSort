public interface IProgressStorage
{
    ProgressData Load();
    void Save(ProgressData data);
}
