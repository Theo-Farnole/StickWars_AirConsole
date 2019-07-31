public enum ControllerView
{
    Load,
    Play,
    Wait,
    NoPlace
}

public static class ControllerViewExtension
{
    public static string ToString(this ControllerView cv)
    {
        switch (cv)
        {
            case ControllerView.Load:
                return "Load";

            case ControllerView.Play:
                return "Play";

            case ControllerView.Wait:
                return "Wait";

            case ControllerView.NoPlace:
                return "NoPlace";
        }

        return "-1";
    }
}
