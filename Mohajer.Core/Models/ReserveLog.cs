using System;

namespace Mohajer.Core.Models
{
    public class ReserveLog
    {
        public int Id { get; set; }
        public Food Food { get; set; }
        public ReserveOperation Operation { get; set; }
        public ReserveResult Result { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public enum ReserveOperation
    {
        Reserve, Unreserve
    }

    public enum ReserveResult
    {
        Successful,
        ConnectionProblem,
        NotEnoughMoney
    }
}