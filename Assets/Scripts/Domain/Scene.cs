using UniSwitcher.Domain;

namespace Domain
{
    public struct Scene : IScene
    {
        public static Scene[] Levels => new[]
        {
            new Scene("Assets/Scenes/Levels/Lv1.unity"),
            new Scene("Assets/Scenes/Levels/Lv2.unity"),
            new Scene("Assets/Scenes/Levels/Lv3.unity"),
            new Scene("Assets/Scenes/Levels/Lv4.unity"),
            new Scene("Assets/Scenes/Levels/Lv5.unity"),
        };

        public static Scene Main => new Scene("Assets/Scenes/Main.unity");

        private string _rawValue;

        private Scene(string path)
        {
            _rawValue = path;
        }

        public string GetRawValue()
        {
            return _rawValue;
        }
    }
}