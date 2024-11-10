namespace Libs.justbake.Command
{
    public readonly struct Result
    {
        public bool Success { get; }
        public string ErrorMessage { get; }
            
        public Result(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
}