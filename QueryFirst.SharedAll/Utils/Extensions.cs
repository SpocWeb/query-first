namespace QueryFirst.CoreLib.Utils
{
    public static class Extensions
    {
        public static bool ParseOptimistic(this bool _, string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (input.ToLower() == "true")
                    return true;
                else return false;
            }
            else return false;
        }
    }
}
