public class InputActionData
{
    public bool HasBeenCompleted = false;
    public string Title = "";

    public InputActionData() { }
    public InputActionData(bool hasBeenCompleted, string title)
    {
        HasBeenCompleted = hasBeenCompleted;
        Title = title;
    }
}