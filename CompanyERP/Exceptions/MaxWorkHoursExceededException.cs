namespace CompanyERP.Exceptions
{
    public class MaxWorkHoursExceededException : Exception
    {
        public MaxWorkHoursExceededException(string message) : base(message) { }
    }
}
