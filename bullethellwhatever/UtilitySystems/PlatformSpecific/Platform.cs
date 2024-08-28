namespace bullethellwhatever.UtilitySystems.PlatformSpecific
{
    public abstract class Platform
    {
        public abstract string SavePath
        {
            get;
            set;
        }
        public abstract void OpenURL();


    }
}
