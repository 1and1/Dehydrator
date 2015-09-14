namespace Dehydrator.EntityFramework
{
    /// <summary>
    /// Implements the transaction interface but does nothing. Used to handle nested transactions.
    /// </summary>
    public sealed class FakeTransaction : ITransaction
    {
        public void Commit()
        {
        }

        public void Dispose()
        {
        }
    }
}