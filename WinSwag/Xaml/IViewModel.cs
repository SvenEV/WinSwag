namespace WinSwag.Xaml
{
    /// <summary>
    /// Marks a type as a view model for a certain model type.
    /// The annotated type must have a constructor with exactly one parameter of the model type.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IViewModel<TModel>
    {
    }
}
