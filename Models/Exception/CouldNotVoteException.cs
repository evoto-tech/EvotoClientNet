namespace Models.Exception
{
    public class CouldNotVoteException : System.Exception
    {
        public CouldNotVoteException()
        {
        }

        public CouldNotVoteException(string msg) : base(msg)
        {
        }
    }
}