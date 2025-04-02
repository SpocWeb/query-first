namespace QueryFirst
{
    public static class BuildUpWithTiny
    {
        public static T BuildUp<T>(this T me)
        {
            return (T)TinyIoCContainer.Current.BuildUp(me);
        }
    }
}
