namespace Interfaces
{
    /// <summary>
    /// Defines every class that wants to receive notifications about <see cref="EnemyEnter"/>
    /// </summary>
    public interface IEnterActivation
    {
        public void SetActive(bool isActive);
    }
}